using Newtonsoft.Json;
using NLog;
using NotifiedObjectsFramework;
using RLCCore.Domain;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Core.IO
{
    public class SaveController : NotifyPropertyChangedBase
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        private const string projectFileName = "Project.rlc";

        private string currentSaveFile = "";

        public bool NeedSelectSavePath => string.IsNullOrWhiteSpace(currentSaveFile) || !File.Exists(currentSaveFile);

        public string WorkDirectory { get; private set; }
        private string workFilePath;
        

        public void UpdateSoundTrackFile(string oldFileName, string newFilePath, SequencePlayer player)
        {
            string newFileName = Path.GetFileName(newFilePath);
            string workFilePath = Path.Combine(WorkDirectory, newFileName);
            if(newFileName == oldFileName) {
                player.Close();
            }
            File.Copy(newFilePath, workFilePath, true);
            if(string.IsNullOrWhiteSpace(oldFileName)) {
                return;
            }
            player.Close();
            player.OpenFile(workFilePath);
            if(newFileName != oldFileName) {
                File.Delete(Path.Combine(WorkDirectory, oldFileName));
            }
        }

        public void DeleteClientFolder(RemoteClient client)
        {
            if(client is null) {
                throw new ArgumentNullException(nameof(client));
            }
            string clientPath = GetClientFolder(client.Number, client.Name);
            if(Directory.Exists(clientPath)) {
                Directory.Delete(clientPath, true);
            }
        }

        public string GetClientFolder(int clientNumber, string clientName)
        {
            return Path.Combine(WorkDirectory, "Clients", $"{clientNumber}_{clientName}");
        }

        public void ClearTempFolder()
        {
            string tmpPath = Path.GetTempPath();
            string tempSavePath = Path.Combine(tmpPath, "RemoteLedControl");
            if(!Directory.Exists(tempSavePath)) {
                return;
            }
            var tmpSubDirectories = Directory.GetDirectories(tempSavePath);
            foreach(var subDirectory in tmpSubDirectories) {
                try {
                    Directory.Delete(subDirectory, true);
                }
                catch(Exception) {
                    logger.Error($"Невозможно удалить временный каталог, возможно занят другим процессом: {subDirectory}");
                }
            }
        }

        private void CreateWorkDirectory()
        {
            string tmpPath = Path.Combine(Path.GetTempPath(), "RemoteLedControl");
            if(!Directory.Exists(tmpPath)) {
                WorkDirectory = Path.Combine(tmpPath, "1");
                Directory.CreateDirectory(Path.Combine(WorkDirectory, "Clients"));
            }
            else {
                var subDirectories = Directory.GetDirectories(tmpPath);
                int maxWorkDirectory = 0;
                if(subDirectories.Any()) {
                    maxWorkDirectory = subDirectories.Select(x => {
                        string name = new DirectoryInfo(x).Name;
                        if(int.TryParse(name, out int result)) {
                            return result;
                        }
                        return 0;
                    }).Max();
                }

                WorkDirectory = Path.Combine(tmpPath, (maxWorkDirectory + 1).ToString());
                Directory.CreateDirectory(Path.Combine(WorkDirectory, "Clients"));
            }           

            workFilePath = Path.Combine(WorkDirectory, projectFileName);
        }

        public void Create()
        {
            CreateWorkDirectory();

            using(FileStream fileStream = new FileStream(workFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)) {
                FileInfo workFileInfo = new FileInfo(workFilePath);
                if(!workFileInfo.Exists) {
                    fileStream.WriteByte(1);
                }
                fileStream.SetLength(0);
            }
            currentSaveFile = null;
        }

        public RemoteControlProject Load(string loadFilePath)
        {
            if(string.IsNullOrWhiteSpace(loadFilePath)) {
                throw new ArgumentNullException(nameof(loadFilePath));
            }
            if(!File.Exists(loadFilePath)) {
                throw new ArgumentException($"Такого файла не существует: {loadFilePath}");
            }

            currentSaveFile = loadFilePath;

            CreateWorkDirectory();
            ZipUtility.ExtractZipFile(currentSaveFile, WorkDirectory);

            string projectFilePath = Path.Combine(WorkDirectory, projectFileName);
            string projectContent;
            using(Stream stream = new FileStream(projectFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read)) 
            using(StreamReader sr = new StreamReader(stream)) { 
                projectContent = sr.ReadToEnd();
            }
            RemoteControlProject project = JsonConvert.DeserializeObject<RemoteControlProject>(projectContent);            
            return project;
        }

        public void SaveAs(string saveFilePath, RemoteControlProject project)
        {
            if(string.IsNullOrWhiteSpace(saveFilePath)) {
                throw new ArgumentNullException(nameof(saveFilePath));
            }

            if(project is null) {
                throw new ArgumentNullException(nameof(project));
            }

            using(var stream = new FileStream(workFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            using(StreamWriter sw = new StreamWriter(stream, Encoding.UTF8, 1024, true)) {
                stream.SetLength(0);
                sw.Write(JsonConvert.SerializeObject(project));
            }

            ZipUtility.ZipFolder(saveFilePath, WorkDirectory);

            currentSaveFile = saveFilePath;
        }

        public void Save(RemoteControlProject project)
        {
            if(string.IsNullOrWhiteSpace(currentSaveFile)) {
                throw new InvalidOperationException($"Не выбран файл сохранения");
            }
            SaveAs(currentSaveFile, project);
        }
    }
}
