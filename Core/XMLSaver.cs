using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Core
{
    /// <summary>
    /// Класс осуществляющий сохранение данных в XML файл
    /// </summary>
    public class XMLSaver
    {
        public Project Fields;

        //Запись настроек в файл
        public void WriteXml(string SavePath)
        {
            if (Fields != null)
            {
                XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };
                XmlWriter w = XmlDictionaryWriter.Create(SavePath, settings);
                NetDataContractSerializer ser = new NetDataContractSerializer();
                ser.WriteObject(w, Fields);
                w.Close();
            }


        }
        //Чтение насроек из файла
        public void ReadXml(string OpenPath)
        {
            if (Fields != null)
            {
                if (File.Exists(OpenPath))
                {
                    FileStream fs = new FileStream(OpenPath, FileMode.Open);
                    XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                    NetDataContractSerializer ser = new NetDataContractSerializer();
                    Fields = (ser.ReadObject(reader, true) as Project);
                    fs.Close();

                }
            }
        }
    }
}
