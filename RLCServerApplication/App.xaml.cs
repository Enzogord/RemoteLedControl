using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Core.IO;
using Core.RemoteOperations;
using Core.Sequence;
using NLog;
using RLCCore;
using RLCCore.Settings;
using RLCServerApplication.Services;
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

        private static string version;
        public static string Version {
            get {
                if(string.IsNullOrWhiteSpace(version)) {
                    var currentAssembly = Assembly.GetExecutingAssembly();
                    var versionInfo = currentAssembly.GetName().Version;
                    if(versionInfo != null) {
                        version = $"{versionInfo.Major}.{versionInfo.Minor}.{versionInfo.Build}";
                    }
                }
                return version;
            }
        }

        private void InitMainViewModel()
        {
            var networkController = new NetworkController();
            var saveController = new SaveController();
            var fileDialogService = new FileDialogService();
            var playerFactory = new PlayerFactory(App.Current.Dispatcher);
            var clientConnectionsController = new ClientConnectionsController();
            var sessionController = new WorkSessionController(saveController, networkController, playerFactory, clientConnectionsController);
            var removableDrivesProvider = new RemovableDrivesProvider();
            var userDialogService = new UserDialogService();

            mainWindowViewModel = new MainWindowViewModel(sessionController, fileDialogService, fileDialogService, userDialogService, removableDrivesProvider, clientConnectionsController);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new Mutex(true, "RemoteLedControl", out createdMutex);

            if(createdMutex && mutex.WaitOne()) {
                base.OnStartup(e);
                SubscribeUnhandledException();

                InitMainViewModel();
                MainWindowView mainWindowView = new MainWindowView() { DataContext = mainWindowViewModel };
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
            mutex.ReleaseMutex();
            base.OnExit(e);
            Environment.Exit(0);
        }

        private void SubscribeUnhandledException()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var exception = (Exception)e.ExceptionObject;
                logger.Fatal((Exception)e.ExceptionObject, $"Поймано необработаное исключение в Application Domain. Is terminating: {e.IsTerminating}");
                mainWindowViewModel.ShowException(exception);
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
            mainWindowViewModel.ShowException(e.Exception);
        }
    }
}
