using Newtonsoft.Json;
using NotifiedObjectsFramework;
using RLCCore.Domain;
using RLCCore.Serialization;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Core.IO
{
    public class SaveController : NotifyPropertyChangedBase
    {
        private readonly FileHolder fileHolder;

        private string saveFilePath;
        public string SaveFilePath {
            get => saveFilePath;
            set {
                SetField(ref saveFilePath, value, () => SaveFilePath);
                OnPropertyChanged(nameof(WorkPath));
            }
        }

        public string SavePath => Path.GetDirectoryName(SaveFilePath);

        private string WorkPath {
            get {
                string tmpPath = Path.GetTempPath();
                string projectName = Path.GetFileNameWithoutExtension(SaveFilePath);
                string tempSavePath = Path.Combine(tmpPath, "RemoteLedControl", projectName);
                Directory.CreateDirectory(tempSavePath);
                return tempSavePath;
            }
        }

        public SaveController(FileHolder fileHolder)
        {
            this.fileHolder = fileHolder ?? throw new ArgumentNullException(nameof(fileHolder));
        }

        public string GetSaveFullPath(string fileName)
        {
            return Path.Combine(SavePath, fileName);
        }

        public string GetWorkFullPath(string fileName)
        {
            return Path.Combine(WorkPath, fileName);
        }

        public string GetClientWorkFullPath(string clientFolderName, string fileName)
        {
            return Path.Combine(WorkPath, "Clients", clientFolderName, fileName);
        }

        public void CopyToWorkDirectory(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string fileWorkPath = Path.Combine(WorkPath, fileName);
            File.Copy(filePath, fileWorkPath, true);
        }

        public void ClearTempFolder()
        {
            string tmpPath = Path.GetTempPath();
            string tempSavePath = Path.Combine(tmpPath, "RemoteLedControl");
            if(!Directory.Exists(tempSavePath)) {
                return;
            }
            Directory.Delete(tempSavePath, true);
        }

        private string CreateSaveFolder(string saveFilePath)
        {
            string directory = Path.GetDirectoryName(saveFilePath);
            string fileName = Path.GetFileName(saveFilePath);
            string projectDirectoryName = Path.GetFileNameWithoutExtension(saveFilePath);
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            if(directoryInfo.Name == projectDirectoryName) {
                Directory.CreateDirectory(Path.Combine(directory, "Clients"));
                return saveFilePath;
            }
            string projectDirectoryPath = Path.Combine(directory, projectDirectoryName);
            Directory.CreateDirectory(projectDirectoryPath);
            Directory.CreateDirectory(Path.Combine(projectDirectoryPath, "Clients"));
            return Path.Combine(projectDirectoryPath, fileName);
        }

        public void Create(RemoteControlProject project, string saveFilePath)
        {
            if(project is null) {
                throw new ArgumentNullException(nameof(project));
            }

            if(string.IsNullOrWhiteSpace(saveFilePath)) {
                throw new ArgumentException(nameof(saveFilePath));
            }
            saveFilePath = CreateSaveFolder(saveFilePath);

            fileHolder.UnholdAndCloseFile(saveFilePath);

            FileStream fileStream = new FileStream(saveFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            fileHolder.HoldFile(fileStream);
            FileInfo saveFileInfo = new FileInfo(saveFilePath);
            if(!saveFileInfo.Exists) {
                fileStream.WriteByte(1);
            }
            fileStream.SetLength(0);

            SaveFilePath = saveFilePath;
        }

        public RemoteControlProject Load(string loadFilePath)
        {
            var stream = fileHolder.GetFileStream(loadFilePath);
            if(stream == null) {
                stream = new FileStream(loadFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                fileHolder.HoldFile(stream);
                SaveFilePath = loadFilePath;
            }

            StreamReader sr = new StreamReader(stream);
            string saveContent = sr.ReadToEnd();
            RemoteControlProject project = JsonConvert.DeserializeObject<RemoteControlProject>(saveContent);

            CopyToWorkDirectory(GetSaveFullPath(project.SoundtrackFileName));

            foreach(var client in project.Clients) {
                string uniqueClientName = $"{client.Number}_{client.Name}";
                string savePathFile = Path.Combine(SavePath, "Clients", uniqueClientName);
                string workPathFile = Path.Combine(WorkPath, "Clients", uniqueClientName);
                Directory.CreateDirectory(workPathFile);
                File.Copy(Path.Combine(savePathFile, client.Cyclogramm.FileName + ".cyc"), Path.Combine(workPathFile, client.Cyclogramm.FileName + ".cyc"), true);
                fileHolder.HoldFile(new FileStream(Path.Combine(workPathFile, client.Cyclogramm.FileName + ".cyc"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read));
            }

            return project;
        }

        private void CopyToSaveDirectory(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string sourceFilePath = Path.Combine(WorkPath, fileName);
            string destFilePath = Path.Combine(SavePath, fileName);
            File.Copy(sourceFilePath, destFilePath, true);
        }

        public void Save(RemoteControlProject project)
        {
            if(project is null) {
                throw new ArgumentNullException(nameof(project));
            }
            SettingWriter settingsWriter = new SettingWriter();

            CopyToSaveDirectory(project.SoundtrackFileName);
            foreach(var client in project.Clients) {
                string uniqueClientName = $"{client.Number}_{client.Name}";
                Directory.CreateDirectory(Path.Combine(SavePath, "Clients", uniqueClientName));
                File.Copy(Path.Combine(WorkPath, "Clients", uniqueClientName, client.Cyclogramm.FileName + ".cyc"), Path.Combine(SavePath, "Clients", uniqueClientName, client.Cyclogramm.FileName + ".cyc"), true);

                settingsWriter.WriteSettings(Path.Combine(SavePath, "Clients", uniqueClientName, "set.txt"), project, client);
            }

            var stream = fileHolder.GetFileStream(SaveFilePath);
            stream.SetLength(0);
            using(StreamWriter sw = new StreamWriter(stream, Encoding.UTF8, 1024, true)) {
                var content = JsonConvert.SerializeObject(project);
                sw.Write(content);
            }
        }
    }
}
