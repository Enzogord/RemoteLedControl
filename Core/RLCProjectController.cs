using System;
using System.IO;
using System.Text;
using Core;
using Core.ClientConnectionService;
using Core.IO;
using Core.Messages;
using Core.RemoteOperations;
using Newtonsoft.Json;
using NLog;
using NotifiedObjectsFramework;
using RLCCore.Domain;
using RLCCore.RemoteOperations;
using RLCCore.Settings;
using SNTPService;
using UDPCommunication;

namespace RLCCore
{
    public sealed class RLCProjectController : NotifyPropertyChangedBase, IDisposable
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public FileHolder FileHolder { get; }
        public SaveController SaveController { get; }

        private ProjectWorkModes workMode;
        public ProjectWorkModes WorkMode {
            get => workMode;
            set {
                SetField(ref workMode, value, () => WorkMode);
                OnPropertyChanged(nameof(CanCreateNewProject));
            }
        }

        private NetworkController networkController;
        public NetworkController NetworkController {
            get => networkController;
            private set => SetField(ref networkController, value, () => NetworkController);
        }
        
        private RemoteControlProject currentProject;
        public RemoteControlProject CurrentProject {
            get => currentProject;
            set => SetField(ref currentProject, value, () => CurrentProject);
        }

        private bool servicesIsReady;
        public bool ServicesIsReady {
            get => servicesIsReady;
            set => SetField(ref servicesIsReady, value, () => ServicesIsReady);
        }

        private RemoteClientsOperator remoteClientsOperator;
        public RemoteClientsOperator RemoteClientsOperator {
            get => remoteClientsOperator;
            private set => SetField(ref remoteClientsOperator, value, () => RemoteClientsOperator);
        }

        private ISequenceTimeProvider timeProvider;
        public ISequenceTimeProvider TimeProvider {
            get => timeProvider;
            set => SetField(ref timeProvider, value, () => TimeProvider);
        }

        private SntpService sntpService;
        private UDPService<RLCMessage> udpService;
        private RemoteClientConnectionService remoteClientConnector;

        public RLCProjectController(NetworkController networkController)
        {
            WorkMode = ProjectWorkModes.Setup;
            NetworkController = networkController ?? throw new ArgumentNullException(nameof(networkController));

            FileHolder = new FileHolder();
            SaveController = new SaveController();
            SaveController.ClearTempFolder();
        }

        public void SwitchToSetupMode()
        {
            if(WorkMode == ProjectWorkModes.Setup) {
                return;
            }
            StopServices();
            WorkMode = ProjectWorkModes.Setup;
        }

        public void SwitchToTestMode()
        {
            if(WorkMode == ProjectWorkModes.Test) {
                return;
            }
            StartServices();
            WorkMode = ProjectWorkModes.Test;
        }

        public void SwitchToWorkMode()
        {
            if(WorkMode == ProjectWorkModes.Work) {
                return;
            }
            StartServices();
            WorkMode = ProjectWorkModes.Work;
        }

        public void StartServices()
        {
            if(ServicesIsReady) {
                return;
            }

            var ipAddress = networkController.GetServerIPAddress();

            //SNTP service
            if(sntpService != null) {
                sntpService.Stop();
            }
            sntpService = new SntpService(CurrentProject.SntpPort);
            sntpService.InterfaceAddress = ipAddress;

            try {
                sntpService.Start();
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка при запуске службы синхронизации времени");
                sntpService = null;
                ServicesIsReady = false;
                throw;
            }

            //UDP service
            if(udpService != null) {
                udpService.StopReceiving();                
            }
            udpService = new UDPService<RLCMessage>(ipAddress, CurrentProject.RlcPort);
            udpService.OnReceiveMessage += (sender, endPoint, message) => {
                logger.Debug($"Receive message: {message.MessageType}");
                if(message.MessageType == MessageType.RequestServerIp) {
                    udpService.Send(RLCMessageFactory.SendServerIP(CurrentProject.Key, ipAddress), endPoint.Address, CurrentProject.RlcPort);
                }
            };

            try {
                udpService.StartReceiving();
            }
            catch(Exception ex) {
                sntpService.Stop();
                sntpService = null;
                udpService = null;
                ServicesIsReady = false;
                logger.Error(ex, "Ошибка при запуске службы UDP сообщений");
                throw;
            }

            //TCP service
            if(remoteClientConnector != null) {
                remoteClientConnector.Stop();
            }
            IConnectorMessageService connectorMessageService = new ConnectorMessageService(CurrentProject.Key, CurrentProject.Clients);
            remoteClientConnector = new RemoteClientConnectionService(NetworkController.GetServerIPAddress(), CurrentProject.RlcPort, connectorMessageService, 200);

            //Clients operator
            RemoteClientsOperator = new RemoteClientsOperator(CurrentProject.Key, CurrentProject.Clients, CurrentProject, NetworkController, remoteClientConnector);
            if(TimeProvider != null) {
                RemoteClientsOperator.TimeProvider = TimeProvider;
            }

            try {
                remoteClientConnector.Start();
            }
            catch(Exception ex) {
                sntpService.Stop();
                udpService.StopReceiving();
                sntpService = null;
                udpService = null;
                remoteClientConnector = null;
                RemoteClientsOperator = null;
                ServicesIsReady = false;
                logger.Error(ex, "Ошибка при запуске сервиса соединения с удаленными клиентами");
                throw;
            }

            ServicesIsReady = true;
        }

        public void StopServices()
        {
            if(RemoteClientsOperator != null) {
                RemoteClientsOperator.Stop();
            }
            if(sntpService != null) {
                sntpService.Stop();
            }
            
            if(udpService != null) {
                udpService.StopReceiving();
            }

            if(remoteClientConnector != null) {
                remoteClientConnector.Stop();
            }

            sntpService = null;
            udpService = null;
            remoteClientConnector = null;
            RemoteClientsOperator = null;

            ServicesIsReady = false;
        }

        private uint GenerateProjectKey()
        {
            byte[] bytes = new byte[4];

            Random Rand = new Random();
            Rand.NextBytes(bytes);

            return (uint)((bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + (bytes[3] << 0));
        }

        public bool CanCreateNewProject => WorkMode == ProjectWorkModes.Setup;

        public void CreateProject()
        {
            if(!CanCreateNewProject) {
                return;
            }

            CurrentProject = new RemoteControlProject(GenerateProjectKey());
            SaveController.Create();
        }

        public void LoadProject(Stream loadStream)
        {
            if(loadStream is null) {
                throw new ArgumentNullException(nameof(loadStream));
            }

            RemoteControlProject project = SaveController.Load(loadStream);
            CurrentProject = project;
        }

        public bool NeedSelectSavePath => SaveController.NeedSelectSavePath;
        public void SaveProject()
        {
            SaveController.Save(CurrentProject);
        }

        public void SaveProjectAs(Stream saveStream)
        {
            SaveController.SaveAs(saveStream, CurrentProject);
        }

        public void Dispose()
        {
            if(sntpService != null) {
                sntpService.Stop();
            }
            if(udpService != null) {
                udpService.StopReceiving();
            }
            if(remoteClientConnector != null) {
                remoteClientConnector.Stop();
            }
        }
    }
}
