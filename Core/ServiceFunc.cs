using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Core
{
    public enum TypesString : Byte
    {
        NotSet = 1,
        EnglishRusianDigitUnder = 2,
        EnglishDigitUnder = 3,
        Digit = 4,
        AllWithoutSpace = 5,
        EnglishRusianDigitUnderDefis = 6,
        EnglishDigitUnderDefis = 7
    }

    public enum TypesNumeric : Byte
    {
        t_uint32 = 1,
        t_uint16 = 2,
        t_byte = 3,
        t_byte_with_zero = 4
    }

    public static class ServiceFunc
    {
        public static Color AcceptColor = Color.FromArgb(155,255,155);
        public static Color RefuseColor = Color.FromArgb(255, 155, 155);

        /// <summary>
        /// Проверяет введенную информацию в текстовое поле на соответствие условиям и убирает из строки символы не удовлетворяющие условиям.
        /// </summary>
        public static string StringValidation(string StrIn, TypesString type)
        {            
            string result = "";
            string CtrlStr = "";

            switch (type)
            {
                case TypesString.NotSet:
                    result = StrIn;
                    break;
                case TypesString.EnglishRusianDigitUnder:
                    CtrlStr = "_1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZАаБбВвГгДдЕеЁёЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчШшЩщЪъЫыЬьЭэЮюЯя";
                    break;
                case TypesString.EnglishRusianDigitUnderDefis:
                    CtrlStr = "-_1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZАаБбВвГгДдЕеЁёЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчШшЩщЪъЫыЬьЭэЮюЯя";
                    break;
                case TypesString.EnglishDigitUnder:
                    CtrlStr = "_1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;
                case TypesString.EnglishDigitUnderDefis:
                    CtrlStr = "-_1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;
                case TypesString.Digit:
                    CtrlStr = "1234567890";
                    break;
                default:
                    break;
            }

            if ((type != TypesString.NotSet) && (type != TypesString.AllWithoutSpace))
            {
                for (int i = 0; i < StrIn.Length; i++)
                {
                    foreach (char ch in CtrlStr)
                    {
                        if (StrIn[i] == ch)
                        {
                            result += StrIn[i];
                            break;
                        }
                    }
                }
            }
            else if (type == TypesString.AllWithoutSpace)
            {
                for (int i = 0; i < StrIn.Length; i++)
                {

                    if (StrIn[i] != ' ')
                    {
                        result += StrIn[i];
                    }

                }
            }
            return result;
        }

        /// <summary>
        /// Проверяет стекстовую строку с числом на соответствие определенным типам
        /// </summary>
        /// <param name="StrIn">Входящая строка с числом</param>
        /// <param name="type">Тип числа, по которому будет производиться проверка</param>
        /// <param name="strOut">Исходящая строка с правильным числом, если возможно было скорректировать</param>
        /// <returns>true если число правильно, или скорректировано, false если число невозможно скорректировать</returns>
        public static bool NumericValidation(string StrIn, TypesNumeric type, out string strOut)
        {
            bool result = false;
            strOut = "";
            string NewStr;
            switch (type)
            {
                case TypesNumeric.t_uint32:
                    if (StrIn.Length > 18)
                    {
                        NewStr = StrIn.Remove(17);
                    }
                    else
                    {
                        NewStr = StrIn;
                    }
                    UInt64 TmpUint64;
                    if (UInt64.TryParse(NewStr, out TmpUint64))
                    {
                        if (TmpUint64 > UInt32.MaxValue)
                        {                            
                            TmpUint64 = UInt32.MaxValue;
                            strOut = TmpUint64.ToString();
                            result = true;
                        }
                        else if (TmpUint64 > 0)
                        {
                            strOut = TmpUint64.ToString();
                            result = true;
                        }
                        else
                        {
                            strOut = NewStr;
                            result = false;
                        }
                    }
                    else
                    {
                        strOut = NewStr;
                        result = false;
                    }
                    break;
                case TypesNumeric.t_uint16:
                    if (StrIn.Length > 6)
                    {
                        NewStr = StrIn.Remove(6);
                    }
                    else
                    {
                        NewStr = StrIn;
                    }
                    UInt32 TmpUint32;
                    if (UInt32.TryParse(NewStr, out TmpUint32))
                    {
                        if (TmpUint32 > UInt16.MaxValue)
                        {
                            TmpUint32 = UInt16.MaxValue;
                            strOut = TmpUint32.ToString();
                            result = true;
                        }
                        else if (TmpUint32 > 0)
                        {
                            strOut = TmpUint32.ToString();
                            result = true;
                        }
                        else
                        {
                            strOut = NewStr;
                            result = false;
                        }
                    }
                    else
                    {
                        strOut = NewStr;
                        result = false;
                    }
                    break;
                case TypesNumeric.t_byte:
                    if (StrIn.Length > 4)
                    {
                        NewStr = StrIn.Remove(3);
                    }
                    else
                    {
                        NewStr = StrIn;
                    }
                    UInt16 TmpUint16;
                    if (UInt16.TryParse(NewStr, out TmpUint16))
                    {
                        if (TmpUint16 > Byte.MaxValue)
                        {
                            TmpUint16 = Byte.MaxValue;
                            strOut = TmpUint16.ToString();
                            result = true;
                        }
                        else if (TmpUint16 > 0)
                        {
                            strOut = TmpUint16.ToString();
                            result = true;
                        }
                        else
                        {
                            strOut = NewStr;
                            result = false;
                        }
                    }
                    else
                    {
                        strOut = NewStr;
                        result = false;
                    }
                    break;
                case TypesNumeric.t_byte_with_zero:
                    if (StrIn.Length > 4)
                    {
                        NewStr = StrIn.Remove(3);
                    }
                    else
                    {
                        NewStr = StrIn;
                    }
                    UInt16 _TmpUint16;
                    if (UInt16.TryParse(NewStr, out _TmpUint16))
                    {
                        if (_TmpUint16 > Byte.MaxValue)
                        {
                            _TmpUint16 = Byte.MaxValue;
                            strOut = _TmpUint16.ToString();
                            result = true;
                        }
                        else if (_TmpUint16 >= 0)
                        {
                            strOut = _TmpUint16.ToString();
                            result = true;
                        }
                        else
                        {
                            strOut = NewStr;
                            result = false;
                        }
                    }
                    else
                    {
                        strOut = NewStr;
                        result = false;
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        public static bool TextBoxValidation(TextBox TB, TypesString typeString)
        {
            bool result = false;
            if (TB.Text != "")
            {
                TB.Text = StringValidation(TB.Text, typeString);
                result = true;
            }
            return result;
        }

        public static bool TextBoxValidation(TextBox TB, TypesNumeric typeNumeric)
        {
            bool result = false;
            string TmpStr;
            if (TB.Text != "")
            {
                if (NumericValidation(StringValidation(TB.Text, TypesString.Digit), typeNumeric, out TmpStr))
                {
                    TB.Text = TmpStr;
                    result = true;
                }
                else
                {
                    TB.Text = TmpStr;
                    result = false;
                }
            }
            return result;
        }

        public static bool ColoringTextBox(TextBox textBox, bool isValid)
        {            
            if (isValid)
            {
                textBox.BackColor = AcceptColor;
            }
            else
            {
                textBox.BackColor = RefuseColor;
            }
            return isValid;
        }

        public static void GetIPAdressList(ComboBox CB)
        {
            CB.Items.Clear();
            string myHost = Dns.GetHostName();
            string myIP = Dns.GetHostEntry(myHost).AddressList[0].ToString();            
            for (int i = 0; i < Dns.GetHostEntry(myHost).AddressList.Count<IPAddress>(); i++)
            {
                if (Dns.GetHostEntry(myHost).AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    CB.Items.Add(Dns.GetHostEntry(myHost).AddressList[i]);
                }
            }
            if (CB.Items.Count > 0)
            {
                CB.SelectedIndex = 0;
            }
        }

        public static bool CheckFolderAccess(string CheckPath)
        {
            bool result;
            if (Directory.Exists(CheckPath))
            {
                try
                {
                    Directory.Delete(CheckPath);
                }
                catch (IOException ex)
                {
                    //файл занят другим процессом, в данном случае папка
                    if (ex.HResult == -2147024864)
                    {
                        return false;
                    }
                    //Папка не пуста
                    else if (ex.HResult == -2147024751)
                    {
                        return true;
                    }
                }
                Directory.CreateDirectory(CheckPath);
                result = true;
            }
            else
            {
                result = true;
            }
            return result;
        }

        //public static void CopyDirectory(string SourcePath, string DestinationPath, bool Overwrite)
        //{
        //    if (Directory.Exists(DestinationPath))
        //    {
        //        if (Overwrite)
        //        {
        //            Directory.Delete(DestinationPath, true);
        //            Directory.CreateDirectory(DestinationPath);
        //        }                             
        //    }
        //    else
        //    {
        //        Directory.CreateDirectory(DestinationPath);
        //    }            
        //    if (Directory.Exists(SourcePath))
        //    {
        //        int length = SourcePath.Length;
        //        string[] files = Directory.GetFiles(SourcePath,"*.*", SearchOption.AllDirectories);

        //        // Copy the files and overwrite destination files if they already exist.
        //        foreach (string s in files)
        //        {
        //            // Use static Path methods to extract only the file name from the path.
        //            //string fileName = Path.(s);
        //            string a = s.Substring(length);
        //            string destFile = DestinationPath + a;
        //            File.Copy(s, destFile, true);
        //        }
        //    }
        //}


        public static void CopyDirectory(string SourceDirectory, string DestinationDirectory, out CopiedInfo copiedInfo, out AlreadyCopiedInfo alreadyCopiedInfo)
        {
            DirectoryInfo SourceDir = new DirectoryInfo(SourceDirectory);
            DirectoryInfo DestinationDir = new DirectoryInfo(DestinationDirectory);
            copiedInfo = new CopiedInfo();
            copiedInfo = GetSize(SourceDir);
            alreadyCopiedInfo = new AlreadyCopiedInfo();
            CopyAll(SourceDir, DestinationDir, out alreadyCopiedInfo);
        }

        public static void CopyDirectory(string SourceDirectory, string DestinationDirectory)
        {
            DirectoryInfo SourceDir = new DirectoryInfo(SourceDirectory);
            DirectoryInfo DestinationDir = new DirectoryInfo(DestinationDirectory);
            AlreadyCopiedInfo alreadyCopiedInfo = new AlreadyCopiedInfo();
            CopyAll(SourceDir, DestinationDir, out alreadyCopiedInfo);
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo destination, out AlreadyCopiedInfo alreadyCopiedInfo)
        {
            alreadyCopiedInfo = new AlreadyCopiedInfo();
            if (Directory.Exists(destination.FullName) == false)
            {
                Directory.CreateDirectory(destination.FullName);
            }

            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(destination.ToString(), fi.Name), true);
                alreadyCopiedInfo.AlreadyCopiedFilesCount += 1;
                alreadyCopiedInfo.AlreadyCopiedFilesSize += fi.Length;
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                AlreadyCopiedInfo LocalCopiedInfo = new AlreadyCopiedInfo();
                DirectoryInfo nextDestSubDir = destination.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextDestSubDir, out LocalCopiedInfo);
                alreadyCopiedInfo.AlreadyCopiedFilesCount += LocalCopiedInfo.AlreadyCopiedFilesCount;
                alreadyCopiedInfo.AlreadyCopiedFilesSize += LocalCopiedInfo.AlreadyCopiedFilesSize;
            }
        }

        private static CopiedInfo GetSize(DirectoryInfo source)
        {
            CopiedInfo result = new CopiedInfo();
            FileInfo[] FilesArray = source.GetFiles("*.*", SearchOption.AllDirectories);
            result.CopiedFilesCount = FilesArray.Length;
            foreach (FileInfo fi in FilesArray)
            {
                result.CopiedFilesSize += fi.Length;
            }
            return result;
        }

        public static bool FileNotExistsOrHaveAccess(FileInfo file)
        {
            bool result = false;
            FileStream tmpStream;
            if (file.Exists)
            {
                try
                {
                    tmpStream = file.OpenWrite();
                    tmpStream.Close();
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            else
            {
                result = true;
            }

            return result;
        }

        public struct CopiedInfo
        {
            public int CopiedFilesCount;
            public long CopiedFilesSize;
        }

        public struct AlreadyCopiedInfo
        {
            public int AlreadyCopiedFilesCount;
            public long AlreadyCopiedFilesSize;
        }

    }
}
