using System;
using Core;
using Core.IO;
using Core.RemoteOperations;
using Core.Sequence;
using NetworkServer.UDP;
using NLog;
using NotifiedObjectsFramework;
using RLCCore.Domain;
using RLCCore.Settings;
using SNTPService;

namespace RLCCore
{
    public sealed class WorkSessionController : NotifyPropertyChangedBase
    {
        Logger logger = LogManager.GetCurrentClassLogger();


        public SaveController SaveController { get; }
        public NetworkController NetworkController { get; }

        private WorkSession session;
        private readonly PlayerFactory playerFactory;
        private readonly ClientConnectionsController clientConnectionsController;

        public WorkSession Session {
            get => session;
            set {
                SetField(ref session, value);
                OnPropertyChanged(nameof(CanSaveProject));
                OnPropertyChanged(nameof(CanLoadProject));
                UpdateSessionSubscription();
            }
        }

        private void UpdateSessionSubscription()
        {
            if(Session == null) {
                return;
            }

            CreateNotificationBinding().AddProperty(nameof(CanLoadProject), nameof(CanCreateProject))
                .SetNotifier(Session)
                .BindToProperty(x => x.State)
                .End();
        }

        public WorkSessionController(SaveController saveController, NetworkController networkController, PlayerFactory playerFactory, ClientConnectionsController clientConnectionsController)
        {
            SaveController = saveController ?? throw new ArgumentNullException(nameof(saveController));
            NetworkController = networkController ?? throw new ArgumentNullException(nameof(networkController));
            this.playerFactory = playerFactory ?? throw new ArgumentNullException(nameof(playerFactory));
            this.clientConnectionsController = clientConnectionsController ?? throw new ArgumentNullException(nameof(clientConnectionsController));
            SaveController.ClearTempFolder();
        }


        #region Create

        public void CreateProject()
        {
            if(!CanCreateProject) {
                return;
            }
            var project = new RemoteControlProject(GenerateProjectKey());
            SaveController.Create();
            CreateSession(project);
        }

        private uint GenerateProjectKey()
        {
            byte[] bytes = new byte[4];

            Random rand = new Random();
            rand.NextBytes(bytes);

            return (uint)((bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + (bytes[3] << 0));
        }

        public void CreateSession(RemoteControlProject project)
        {
            ClearSession();
            var udpServer = new UdpServer();
            var player = playerFactory.CreatePlayer();
            var clientOperator = new UdpClientOperator(udpServer, project, clientConnectionsController, NetworkController, player);


            var sntpService = new SntpService();
            var sntpServer = new SntpServer(sntpService, NetworkController, project);
            Session = new WorkSession(project, player, clientOperator, sntpServer, SaveController, NetworkController);
        }

        private void ClearSession()
        {
            if(Session == null) {
                return;
            }
            Session.StopServices();
        }

        public bool CanCreateProject {
            get {
                if(Session == null) {
                    return true;
                }
                return Session.State == SessionState.Setup;
            }
        }

        #endregion Create

        #region Load

        public void LoadProject(string loadFilePath)
        {
            if(string.IsNullOrWhiteSpace(loadFilePath)) {
                throw new ArgumentNullException(nameof(loadFilePath));
            }

            RemoteControlProject project = SaveController.Load(loadFilePath);
            CreateSession(project);
        }

        public bool CanLoadProject => Session == null || Session.State == SessionState.Setup;

        #endregion Load

        #region Save

        public bool NeedSelectSavePath => SaveController.NeedSelectSavePath;

        public void SaveProject()
        {
            SaveController.Save(session.Project);
        }

        public void SaveProjectAs(string saveFile)
        {
            SaveController.SaveAs(saveFile, session.Project);
        }

        public bool CanSaveProject => Session != null;

        #endregion Save

    }
}
