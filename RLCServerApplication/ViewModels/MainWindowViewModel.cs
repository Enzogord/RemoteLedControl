using Core.IO;
using Core.Services.FileDialog;
using Core.Services.UserDialog;
using RLCCore;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;
using System;
using System.Windows;

namespace RLCServerApplication.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ISaveFileService saveFileService;
        private readonly IOpenFileService openFileService;
        private readonly IUserDialogService userDialogService;
        private readonly RemovableDrivesProvider removableDrivesProvider;

        public WorkSessionController sessionController { get; private set; }

        private WorkSessionViewModel workSessionViewModel;
        public WorkSessionViewModel WorkSessionViewModel {
            get => workSessionViewModel;
            private set => SetField(ref workSessionViewModel, value);
        }

        private ExceptionViewModel exceptionViewModel;
        public ExceptionViewModel ExceptionViewModel {
            get => exceptionViewModel;
            set => SetField(ref exceptionViewModel, value);
        }

        public MainWindowViewModel(
            WorkSessionController sessionController,
            ISaveFileService saveFileService,
            IOpenFileService openFileService,
            IUserDialogService userDialogService,
            RemovableDrivesProvider removableDrivesProvider)
        {
            this.sessionController = sessionController ?? throw new ArgumentNullException(nameof(sessionController));
            this.saveFileService = saveFileService ?? throw new ArgumentNullException(nameof(saveFileService));
            this.openFileService = openFileService ?? throw new ArgumentNullException(nameof(openFileService));
            this.userDialogService = userDialogService ?? throw new ArgumentNullException(nameof(userDialogService));
            this.removableDrivesProvider = removableDrivesProvider ?? throw new ArgumentNullException(nameof(removableDrivesProvider));
            ConfigureBindings();
        }

        private void ConfigureBindings()
        {
            CreateNotificationBinding()
                .AddAction(CreateWorkSessionViewModel)
                .SetNotifier(sessionController)
                .BindToProperty(x => x.Session)
                .End();
        }

        private void CreateWorkSessionViewModel()
        {
            if(sessionController.Session == null) {
                return;
            }

            var settingsViewModel = new SettingsViewModel(sessionController.Session, openFileService, sessionController.NetworkController);
            var remoteClientsViewModel = new RemoteClientsViewModel(sessionController.Session, removableDrivesProvider, sessionController.SaveController);
            WorkSessionViewModel = new WorkSessionViewModel(sessionController.Session, settingsViewModel, remoteClientsViewModel);
        }

        public void ShowException(Exception exception)
        {
            if(exception == null) {
                return;
            }

            ExceptionViewModel = new ExceptionViewModel(exception);
            ExceptionViewModel.Close += (s, e) => ExceptionViewModel = null;
        }

        #region Commands

        #region CreateProjectCommand

        private DelegateCommand createProjectCommand;
        public DelegateCommand CreateProjectCommand {
            get {
                if(createProjectCommand == null) {
                    CreateCreateProjectCommand();
                }
                return createProjectCommand;
            }
        }

        private void CreateCreateProjectCommand()
        {
            createProjectCommand = new DelegateCommand(
                () => CreateProject(),
                () => sessionController.CanCreateProject
            );
            createProjectCommand.CanExecuteChangedWith(sessionController, x => x.CanCreateProject);
        }

        private void CreateProject()
        {            
            if(WorkSessionViewModel != null) {
                var result = userDialogService.AskQuestion(
                    "Уже есть открытый проект, все не сохраненные изменения будут утеряны, все равно открыть новый проект?",
                    "Внимание!",
                    UserDialogActions.YesNo,
                    UserDialogInformationLevel.Question);

                if(result == UserDialogResult.No) {
                    return;
                }
            }

            sessionController.CreateProject();
        }

        #endregion CreateProjectCommand	

        #region SaveProjectCommand

        private DelegateCommand saveProjectCommand;

        public DelegateCommand SaveProjectCommand {
            get {
                if(saveProjectCommand == null) {
                    CreateSaveProjectCommand();
                }
                return saveProjectCommand;
            }
        }

        private void CreateSaveProjectCommand()
        {
            saveProjectCommand = new DelegateCommand(
                () => {
                    if(sessionController.NeedSelectSavePath) {
                        SaveProjectAsCommand.Execute();
                        return;
                    }
                    sessionController.SaveProject();
                    MessageBox.Show("Сохранено");
                },
                () => sessionController.CanSaveProject
            );
            saveProjectCommand.CanExecuteChangedWith(sessionController, x => x.CanSaveProject);
        }

        #endregion SaveProjectCommand	

        #region SaveAsCommand

        public DelegateCommand saveProjectAsCommand;
        public DelegateCommand SaveProjectAsCommand {
            get {
                if(saveProjectAsCommand == null) {
                    CreateSaveAsCommand();
                }
                return saveProjectAsCommand;
            }
        }

        private void CreateSaveAsCommand()
        {
            saveProjectAsCommand = new DelegateCommand(
                () => {
                    string filter = "RemoteLedControl save file|*.rlcsave";
                    string filePath = saveFileService.SaveFile(filter, "Сохранение проекта", ".rlcsave", true, false, false);
                    if(string.IsNullOrWhiteSpace(filePath)) {
                        return;
                    }

                    sessionController.SaveProjectAs(filePath);
                    MessageBox.Show("Сохранено");
                },
                () => sessionController.CanSaveProject
            );
            saveProjectAsCommand.CanExecuteChangedWith(sessionController, x => x.CanSaveProject);
        }

        #endregion SaveAsCommand

        #region LoadCommand

        private DelegateCommand loadProjectCommand;
        public DelegateCommand LoadProjectCommand {
            get {
                if(loadProjectCommand == null) {
                    CreateLoadCommand();
                }
                return loadProjectCommand;
            }
        }

        private void CreateLoadCommand()
        {
            loadProjectCommand = new DelegateCommand(
                () => {
                    string filter = "RemoteLedControl save file|*.rlcsave";
                    string filePath = openFileService.OpenFile(filter, "Открытие проекта", true, true);
                    if(string.IsNullOrWhiteSpace(filePath)) {
                        return;
                    }
                    sessionController.LoadProject(filePath);
                },
                () => sessionController.CanLoadProject
            );
            loadProjectCommand.CanExecuteChangedWith(sessionController, x => x.CanLoadProject);
        }

        #endregion LoadCommand

        #endregion Commands
    }
}
