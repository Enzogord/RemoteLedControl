using System;
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

        MainWindowViewModel mainWindowViewModel;

        private void InitMainViewModel()
        {
            mainWindowViewModel = Bootstrapper.RootViewModel;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Current.Exit += (sender, eventArg) => Bootstrapper.Stop();
            SubscribeUnhandledException();

            Bootstrapper.Start();
            InitMainViewModel();
            new MainWindowView() { DataContext = mainWindowViewModel }.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            mainWindowViewModel.StopServicesCommand.Execute(null);

            base.OnExit(e);
            Environment.Exit(0);
        }

        private void SubscribeUnhandledException()
        {
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

    }
}
