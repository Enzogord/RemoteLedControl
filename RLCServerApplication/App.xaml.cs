using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RLCServerApplication.ViewModels;
using RLCServerApplication.Views;

namespace RLCServerApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
