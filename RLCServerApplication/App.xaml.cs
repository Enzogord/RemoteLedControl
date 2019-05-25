using System;
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
    }
}
