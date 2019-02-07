﻿using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Core
{
    public class RLCProjectController : NotifyPropertyBase
    {
        private string currentProjectPath; 

        private Project currentProject;
        public Project CurrentProject {
            get => currentProject;
            set {
                if(SetField(ref currentProject, value, () => CurrentProject) && value != null) {
                    Server = new UDPServer(currentProject.Key);
                }
            }
        }

        private UDPServer server;
        public UDPServer Server {
            get => server;
            set => SetField(ref server, value, () => Server);
        }


        public RLCProjectController()
        {
            TestDefaultConfig();
        }

        public void CreateProject()
        {
            
        }

        public void LoadProject()
        {

        }

        public void SaveProject()
        {
            if (ServiceFunc.CheckFolderAccess(CurrentProject.AbsoluteFolderPath + "\\" + CurrentProject.ClientsFolderName)) {
                Save();
            } else {
                MessageBox.Show("Нет доступа к папке \"" + CurrentProject.AbsoluteFolderPath + "\\" + CurrentProject.ClientsFolderName + "\". Возможно некоторые файлы в ней открыты в другой программе", "Ошибка сохранения", MessageBoxButtons.OK);
                return;
            }
        }

        public void SaveProjectAs()
        {
            SaveFileDialog sd = new SaveFileDialog();
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
            }
        }

        private void Save()
        {
            for (int i = 0; i < CurrentProject.ClientList.Count; i++) {
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
            if (CurrentProject.BindedAudioFile != null) {
                try {
                    CurrentProject.BindedAudioFile = CurrentProject.BindedAudioFile.CopyTo(Path.Combine(CurrentProject.AbsoluteFolderPath, Path.GetFileName(CurrentProject.BindedAudioFile.FullName)), true);
                }
                catch (Exception) {

                }
            }

            XMLSaver xmlsaver = new XMLSaver();
            xmlsaver.Fields = CurrentProject;
            xmlsaver.WriteXml(CurrentProject.AbsoluteFilePath);
            CurrentProject.Saved = true;
        }

        private void SaveAs(string FolderPath, string FilePath)
        {
            for (int i = 0; i < CurrentProject.ClientList.Count; i++) {
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
            if (CurrentProject.BindedAudioFile != null) {
                try {
                    CurrentProject.BindedAudioFile = CurrentProject.BindedAudioFile.CopyTo(Path.Combine(FolderPath, Path.GetFileName(CurrentProject.BindedAudioFile.FullName)), true);
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
            CurrentProject.Saved = true;
        }

        [Obsolete("Тестовая конфигурация платы, для проверки программы в дебаге")]
        private void TestDefaultConfig()
        {
            CurrentProject = new Project(158018933);
            var client = new Client("test", "1");
            client.AddPin("0", "2");
            client.AddPin("2", "2");
            client.AddPin("4", "2");
            client.AddPin("5", "2");
            client.UDPPort = 11010;
            client.WifiSSID = "EnzoWiFi";
            client.WifiPassword = "direihoom1";
            client.LEDCount = "2";
            CurrentProject.ClientList.Add(client);

            string music = @"C:\Users\Enzo\Desktop\October\002. Aviators - Monumental.mp3";
            CurrentProject.BindedAudioFile = new FileInfo(music);

            Server.UDPPort = 11010;
        }
    }
}
