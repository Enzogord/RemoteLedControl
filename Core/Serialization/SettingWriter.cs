using System.IO;
using System.Text;

namespace RLCCore.Serialization
{
    public class SettingWriter
    {
        char keyValueDelimiter;
        char parameterDelimiter;

        public SettingWriter(char keyValueDelimiter = '=', char parameterDelimiter = ';')
        {
            this.keyValueDelimiter = keyValueDelimiter;
            this.parameterDelimiter = parameterDelimiter;
        }

        public void WriteSettings(string filePath, params ISettingsProvider[] settingsProvider)
        {
            using(FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                WriteSettings(stream, settingsProvider);
            }
        }

        public void WriteSettings(Stream stream, params ISettingsProvider[] settingsProvider)
        {
            using(StreamWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                foreach(var sp in settingsProvider) {
                    foreach(var settings in sp.GetSettings()) {
                        writer.WriteLine($"{settings.Key}{keyValueDelimiter}{settings.Value}{parameterDelimiter}");
                    }
                }
            }
        }
    }
}
