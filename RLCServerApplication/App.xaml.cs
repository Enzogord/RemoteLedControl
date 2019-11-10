﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using RLCServerApplication.ViewModels;
using RLCServerApplication.Views;

namespace RLCServerApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        bool createdMutex = true;
        Mutex mutex;

        MainWindowViewModel mainWindowViewModel;

        private void InitMainViewModel()
        {
            mainWindowViewModel = Bootstrapper.RootViewModel;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new Mutex(true, "RemoteLedControl", out createdMutex);

            if(createdMutex && mutex.WaitOne()) {
                base.OnStartup(e);
                Current.Exit += (sender, eventArg) => Bootstrapper.Stop();
                SubscribeUnhandledException();

                Bootstrapper.Start();
                InitMainViewModel();
                MainWindowView mainWindowView = new MainWindowView() { DataContext = mainWindowViewModel };
                mainWindowView.InitPlayer();
                mainWindowView.Show();
            }
            else {
                MessageBox.Show("Может быть открыто только одно приложение.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                mutex.Close();
                Environment.Exit(0);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            mainWindowViewModel.Close();
            mutex.ReleaseMutex();
            base.OnExit(e);
            Environment.Exit(0);
        }

        private void SubscribeUnhandledException()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {                
                logger.Fatal((Exception)e.ExceptionObject, $"Поймано необработаное исключение в Application Domain. Is terminating: {e.IsTerminating}");
                MessageBox.Show($"Возникла непредвиденная ошибка.{Environment.NewLine} Техническая информация: {(Exception)e.ExceptionObject}", "Исключение", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs eventArgs) =>
            {
                eventArgs.SetObserved();
                (eventArgs.Exception).Handle(ex =>
                {
                    logger.Fatal(ex, "Поймано необработаное исключение UnobservedTaskException.");
                    return true;
                });
            };
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            MessageBox.Show($"Возникла непредвиденная ошибка.{Environment.NewLine} Техническая информация: {e.Exception}", "Исключение", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
