using System;
using System.IO;
using System.Text;
using Core;
using Core.ClientConnectionService;
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

        private ProjectWorkModes workMode;
        public ProjectWorkModes WorkMode {
            get => workMode;
            set => SetField(ref workMode, value, () => WorkMode);
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

        public void LoadProject(Stream saveFile)
        {
            if(saveFile == null) {
                throw new ArgumentNullException(nameof(saveFile));
            }
            StreamReader sr = new StreamReader(saveFile);
            string saveContent = sr.ReadToEnd();

            RemoteControlProject project = JsonConvert.DeserializeObject<RemoteControlProject>(saveContent);
            CurrentProject = project;
        }

        public void SaveProject(Stream saveFile)
        {
            if(saveFile == null) {
                throw new ArgumentNullException(nameof(saveFile));
            }

            using(StreamWriter sw = new StreamWriter(saveFile, Encoding.UTF8)) {
                var content = JsonConvert.SerializeObject(CurrentProject);
                sw.Write(content);
            }
        }

        public void SaveProjectAs()
        {
            /*SaveFileDialog sd = new SaveFileDialog();
            sd.InitialDirectory = Environment.CurrentDirectory;
            sd.Filter = "XML files|*.xml";

            if (sd.ShowDialog() == DialogResult.OK) {
                // Если выбранный путь не совпадает с путем который указан в текущем открытом проекте, сохранить как новый проект
                if (sd.FileName != CurrentProject.AbsoluteFilePath) {
                    string FileName = Path.GetFileNameWithoutExtension(sd.FileName);
                    string FolderPath = Path.GetDirectoryName(sd.FileName);
                    string FolderName = Path.GetFileName(FolderPath);
                    string FilePath = sd.FileName;

                    if (FolderName != FileName) {
                        if (Directory.Exists(FolderPath + "\\" + FileName)) {
                            MessageBox.Show("Папка " + FileName + " уже существует. Невозможно сохранить проект", "Ошибка сохранения", MessageBoxButtons.OK);
                            return;
                        } else {
                            Directory.CreateDirectory(FolderPath + "\\" + FileName);
                            FolderPath = FolderPath + "\\" + FileName;
                            FilePath = FolderPath + "\\" + FileName + ".xml";
                        }
                    } else {
                        if (Directory.Exists(FolderPath + "\\" + CurrentProject.ClientsFolderName)) {
                            if (!ServiceFunc.CheckFolderAccess(FolderPath + "\\" + CurrentProject.ClientsFolderName)) {
                                MessageBox.Show("Нет доступа к папке \"" + FolderPath + "\\" + CurrentProject.ClientsFolderName + "\". Возможно некоторые файлы в ней открыты в другой программе", "Ошибка сохранения", MessageBoxButtons.OK);
                                return;
                            } else {
                                Directory.Delete(FolderPath + "\\" + CurrentProject.ClientsFolderName, true);
                            }
                        }
                    }
                    Directory.CreateDirectory(FolderPath + "\\" + CurrentProject.ClientsFolderName);

                    //Сохранить как
                    SaveAs(FolderPath, FilePath);

                } else //Иначе если совпадает, пересохранить по этомуже пути
                  {
                    SaveProject();
                }
            }*/
        }

        private void Save()
        {
            /*for (int i = 0; i < CurrentProject.ClientList.Count; i++) {
                if (!CurrentProject.ClientList[i].Saved) {
                    if (CurrentProject.ClientList[i].DeletedCyclogramm != null) {

                        if (File.Exists(CurrentProject.AbsoluteFolderPath + CurrentProject.ClientList[i].RelativePath + "Data.cyc")) {
                            File.Delete(CurrentProject.AbsoluteFolderPath + CurrentProject.ClientList[i].RelativePath + "Data.cyc");
                        }
                    }

                    if (CurrentProject.ClientList[i].Renamed) {
                        Directory.Move(CurrentProject.AbsoluteFolderPath + CurrentProject.ClientList[i].OldRelativePath, CurrentProject.AbsoluteFolderPath + CurrentProject.ClientList[i].RelativePath);
                    }

                    if (!Directory.Exists(CurrentProject.AbsoluteFolderPath + CurrentProject.ClientList[i].RelativePath)) {
                        Directory.CreateDirectory(CurrentProject.AbsoluteFolderPath + CurrentProject.ClientList[i].RelativePath);
                    }
                    CurrentProject.ClientList[i].SaveClientSettingFile(CurrentProject.AbsoluteFolderPath + CurrentProject.ClientList[i].RelativePath + "\\" + CurrentProject.ClientsConfigFileName);

                    if (CurrentProject.ClientList[i].Cyclogramm != null) {
                        if (!CurrentProject.ClientList[i].Cyclogramm.Saved) {
                            string tmpcycpath = CurrentProject.GetAbsoluteTEMPPath() + CurrentProject.ClientList[i].RelativePath + "\\Data.cyc";
                            if (File.Exists(tmpcycpath)) {
                                if (File.Exists(CurrentProject.AbsoluteFolderPath + CurrentProject.ClientList[i].RelativePath + "\\Data.cyc")) {
                                    File.Delete(CurrentProject.AbsoluteFolderPath + CurrentProject.ClientList[i].RelativePath + "\\Data.cyc");
                                }
                                File.Move(tmpcycpath, CurrentProject.AbsoluteFolderPath + CurrentProject.ClientList[i].RelativePath + "\\Data.cyc");
                                CurrentProject.ClientList[i].Cyclogramm.Saved = true;
                            }
                        }
                    }

                    CurrentProject.ClientList[i].Renamed = false;
                    CurrentProject.ClientList[i].Saved = true;
                    if (CurrentProject.ClientList[i].DeletedCyclogramm != null) {
                        CurrentProject.ClientList[i].DeletedCyclogramm = null;
                    }

                }
            }
            if (CurrentProject.DeletedClientList != null) {
                for (int i = 0; i < CurrentProject.DeletedClientList.Count; i++) {
                    try {
                        Directory.Delete(CurrentProject.AbsoluteFolderPath + CurrentProject.DeletedClientList[i].RelativePath, true);
                    }
                    catch (DirectoryNotFoundException e) {
                    }

                    catch (Exception) {
                        continue;
                    }
                    CurrentProject.DeletedClientList.RemoveAt(i);
                }
            }
            if (CurrentProject.AudioFile != null) {
                try {
                    CurrentProject.AudioFile = CurrentProject.AudioFile.CopyTo(Path.Combine(CurrentProject.AbsoluteFolderPath, Path.GetFileName(CurrentProject.AudioFile.FullName)), true);
                }
                catch (Exception) {

                }
            }

            XMLSaver xmlsaver = new XMLSaver();
            xmlsaver.Fields = CurrentProject;
            xmlsaver.WriteXml(CurrentProject.AbsoluteFilePath);
            CurrentProject.Saved = true;*/
        }

        private void SaveAs(string FolderPath, string FilePath)
        {
            /*for (int i = 0; i < CurrentProject.ClientList.Count; i++) {
                Directory.CreateDirectory(FolderPath + CurrentProject.ClientList[i].RelativePath);
                CurrentProject.ClientList[i].SaveClientSettingFile(FolderPath + CurrentProject.ClientList[i].RelativePath + "\\" + CurrentProject.ClientsConfigFileName);
                if (CurrentProject.ClientList[i].Cyclogramm != null) {
                    string tmpcycpath;
                    if (CurrentProject.ClientList[i].Cyclogramm.Saved) {
                        tmpcycpath = CurrentProject.AbsoluteFolderPath + CurrentProject.ClientList[i].RelativePath + "\\Data.cyc";
                    } else {
                        tmpcycpath = CurrentProject.GetAbsoluteTEMPPath() + CurrentProject.ClientList[i].RelativePath + "\\Data.cyc";
                    }

                    if (File.Exists(tmpcycpath)) {
                        if (CurrentProject.ClientList[i].Cyclogramm.Saved) {
                            File.Copy(tmpcycpath, FolderPath + CurrentProject.ClientList[i].RelativePath + "\\Data.cyc");
                        } else {
                            File.Move(tmpcycpath, FolderPath + CurrentProject.ClientList[i].RelativePath + "\\Data.cyc");
                        }

                        CurrentProject.ClientList[i].Cyclogramm.Saved = true;
                    }
                }
                CurrentProject.ClientList[i].Renamed = false;
                CurrentProject.ClientList[i].Saved = true;
            }
            if (CurrentProject.AudioFile != null) {
                try {
                    CurrentProject.AudioFile = CurrentProject.AudioFile.CopyTo(Path.Combine(FolderPath, Path.GetFileName(CurrentProject.AudioFile.FullName)), true);
                }
                catch (Exception) {

                }
            }
            XMLSaver xmlsaver = new XMLSaver();
            xmlsaver.Fields = CurrentProject;
            xmlsaver.WriteXml(FilePath);
            if (CurrentProject.DeletedClientList != null) {
                CurrentProject.DeletedClientList = null;
            }
            CurrentProject.AbsoluteFilePath = FilePath;
            CurrentProject.Saved = true;*/
        }

        public void Dispose()
        {
            sntpService.Stop();
            udpService.StopReceiving();
            remoteClientConnector.Stop();
        }
    }
}
