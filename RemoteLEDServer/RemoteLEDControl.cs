using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Core;
using System.Net;
using System.Net.Sockets;
using System.IO;
using CSCore.CoreAudioAPI;
using System.Diagnostics;
using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using Microsoft.VisualBasic.FileIO;
using RLCPlayer;

namespace RemoteLEDServer
{
    public partial class RemoteLEDControl : Form
    {
        Project project;

        public RemoteLEDControl()
        {
            InitializeComponent();
        }

        private void RemoteLEDControl_Load(object sender, EventArgs e)
        {
            ComboBoxAudioOutputs_Fill();
            dataGridView_Clients.AutoGenerateColumns = false;
            dataGridView_PinList.AutoGenerateColumns = false;
        }

        #region Forms Control Events

        /// <summary>
        ///  Создать проект
        /// </summary>
        private void ToolStripMenuItem_CreateProject_Click(object sender, EventArgs e)
        {

            //Создать
            FormCreateProject f = new FormCreateProject();
            if (f.ShowDialog() == DialogResult.OK)
            {
                ClearProject();
                project = new Project(f.ProjectKey);
                project.Saved = false;
                ToolStripMenuItem_SaveProject.Enabled = true;
                ToolStripMenuItem_SaveAsProject.Enabled = true;
                OnProjectStart();
            }
            else
            {
                if (project == null)
                {
                    ToolStripMenuItem_SaveProject.Enabled = false;
                    ToolStripMenuItem_SaveAsProject.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Открыть проект
        /// </summary>
        private void ToolStripMenuItem_OpenProject_Click(object sender, EventArgs e)
        {
            //Открыть            
            OpenFileDialog od = new OpenFileDialog();
            od.InitialDirectory = Environment.CurrentDirectory;
            od.Filter = "XML files|*.xml";
            od.Multiselect = false;
            if (od.ShowDialog() == DialogResult.OK)
            {
                XMLSaver xml = new XMLSaver();
                xml.Fields = new Project(1);
                try
                {
                    xml.ReadXml(od.FileName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка открытия файла, возможно файл не предназначен для этой программы.");
                    return;
                }

                string FileName = Path.GetFileNameWithoutExtension(od.FileName);
                string FolderPath = Path.GetDirectoryName(od.FileName);
                string FolderName = Path.GetFileName(FolderPath);

                if (FolderName != FileName)
                {
                    if (MessageBox.Show("Проект должен находиться в папке проекта с тем же названием, будет создана папка " +
                        FileName + ", и в нее перемещен файл проекта и приложенные файлы. Продолжить открытие?",
                        "Предупреждение", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        if (Directory.Exists(FolderPath + "\\" + FileName))
                        {
                            MessageBox.Show("Папка \"" + FileName + "\" уже существует. Невозможно продолжить", "Ошибка сохранения", MessageBoxButtons.OK);
                            return;
                        }
                        else
                        {
                            Directory.CreateDirectory(FolderPath + "\\" + FileName);
                            string FilePath = FolderPath + "\\" + FileName + "\\" + FileName + ".xml";
                            File.Move(od.FileName, FilePath);
                            Directory.Move(FolderPath + "\\" + xml.Fields.ClientsFolderName, FolderPath + "\\" + FileName + "\\" + xml.Fields.ClientsFolderName);
                            xml = new XMLSaver();
                            xml.Fields = new Project(1);
                            try
                            {
                                xml.ReadXml(FilePath);
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Ошибка открытия файла, возможно файл не предназначен для этой программы.");
                                return;
                            }

                            ClearProject();
                            project = xml.Fields;
                            project.RuningThreadsList = new List<Thread>();
                            project.AbsoluteFilePath = FilePath;
                            if (project.Server != null)
                            {
                                if (project.Server.ProjectKey != project.Key)
                                {
                                    project.Server.ProjectKey = project.Key;
                                }
                            }
                            //project.Server = new UDPServer(project.Key);
                            project.Saved = true;
                            ToolStripMenuItem_SaveProject.Enabled = true;
                            ToolStripMenuItem_SaveAsProject.Enabled = true;
                            OnProjectStart();
                            return;
                        }

                    }
                    else
                    {
                        return;
                    }

                }

                xml = new XMLSaver();
                xml.Fields = new Project(1);
                try
                {
                    xml.ReadXml(od.FileName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка открытия файла, возможно файл не предназначен для этой программы.");
                    return;
                }

                ClearProject();
                project = xml.Fields;
                project.RuningThreadsList = new List<Thread>();
                project.AbsoluteFilePath = od.FileName;
                if (project.Server != null)
                {
                    if (project.Server.ProjectKey != project.Key)
                    {
                        project.Server.ProjectKey = project.Key;
                    }
                }
                //project.Server = new UDPServer(project.Key);
                project.Saved = true;
                ToolStripMenuItem_SaveProject.Enabled = true;
                ToolStripMenuItem_SaveAsProject.Enabled = true;
                if (project.BindedAudioFile != null)
                {
                    if (project.BindedAudioFile.Exists)
                    {
                        rlcPlayer1.InitializePlayer(project.BindedAudioFile.FullName);
                    }
                    else
                    {
                        MessageBox.Show("Сохраненного в проекта файла \"" + project.BindedAudioFile.FullName + "\" не существует на диске. Необходимо вручную добавить аудио файл в плеер.", "Ошибка открытия аудио файла");
                    }
                }
                OnProjectStart();
            }

        }

        /// <summary>
        /// Сохранить проект
        /// </summary>
        private void ToolStripMenuItem_SaveProject_Click(object sender, EventArgs e)
        {
            //Сохранить
            if (project == null)
            {
                return;
            }

            if (project.AbsoluteFilePath == null)
            {
                SaveProjectAs();
            }
            else
            {
                SaveProject();
            }
        }

        /// <summary>
        /// Сохранить проект как
        /// </summary>
        private void ToolStripMenuItem_SaveAsProject_Click(object sender, EventArgs e)
        {
            //Сохранить как
            if (project == null)
            {
                return;
            }
            SaveProjectAs();
        }

        private void SaveProjectAs()
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.InitialDirectory = Environment.CurrentDirectory;
            sd.Filter = "XML files|*.xml";

            if (sd.ShowDialog() == DialogResult.OK)
            {
                // Если выбранный путь не совпадает с путем который указан в текущем открытом проекте, сохранить как новый проект
                if (sd.FileName != project.AbsoluteFilePath)
                {
                    string FileName = Path.GetFileNameWithoutExtension(sd.FileName);
                    string FolderPath = Path.GetDirectoryName(sd.FileName);
                    string FolderName = Path.GetFileName(FolderPath);
                    string FilePath = sd.FileName;

                    if (FolderName != FileName)
                    {
                        if (Directory.Exists(FolderPath + "\\" + FileName))
                        {
                            MessageBox.Show("Папка " + FileName + " уже существует. Невозможно сохранить проект", "Ошибка сохранения", MessageBoxButtons.OK);
                            return;
                        }
                        else
                        {
                            Directory.CreateDirectory(FolderPath + "\\" + FileName);
                            FolderPath = FolderPath + "\\" + FileName;
                            FilePath = FolderPath + "\\" + FileName + ".xml";
                        }
                    }
                    else
                    {
                        if (Directory.Exists(FolderPath + "\\" + project.ClientsFolderName))
                        {
                            if (!ServiceFunc.CheckFolderAccess(FolderPath + "\\" + project.ClientsFolderName))
                            {
                                MessageBox.Show("Нет доступа к папке \"" + FolderPath + "\\" + project.ClientsFolderName + "\". Возможно некоторые файлы в ней открыты в другой программе", "Ошибка сохранения", MessageBoxButtons.OK);
                                return;
                            }
                            else
                            {
                                Directory.Delete(FolderPath + "\\" + project.ClientsFolderName, true);
                            }
                        }
                    }
                    Directory.CreateDirectory(FolderPath + "\\" + project.ClientsFolderName);

                    //Сохранить как
                    SaveAs(FolderPath, FilePath);

                }
                else //Иначе если совпадает, пересохранить по этомуже пути
                {
                    SaveProject();
                }
            }
            ClientSettingValidation();
        }

        private void SaveProject()
        {
            if (ServiceFunc.CheckFolderAccess(project.AbsoluteFolderPath + "\\" + project.ClientsFolderName))
            {
                // Сохранить
                Save();
            }
            else
            {
                MessageBox.Show("Нет доступа к папке \"" + project.AbsoluteFolderPath + "\\" + project.ClientsFolderName + "\". Возможно некоторые файлы в ней открыты в другой программе", "Ошибка сохранения", MessageBoxButtons.OK);
                return;
            }
            ClientSettingValidation();
        }

        private void Save()
        {
            for (int i = 0; i < project.ClientList.Count; i++)
            {
                if (!project.ClientList[i].Saved)
                {
                    if (project.ClientList[i].DeletedCyclogramm != null)
                    {

                        if (File.Exists(project.AbsoluteFolderPath + project.ClientList[i].RelativePath + "Data.cyc"))
                        {
                            File.Delete(project.AbsoluteFolderPath + project.ClientList[i].RelativePath + "Data.cyc");
                        }
                    }

                    if (project.ClientList[i].Renamed)
                    {
                        Directory.Move(project.AbsoluteFolderPath + project.ClientList[i].OldRelativePath, project.AbsoluteFolderPath + project.ClientList[i].RelativePath);
                    }

                    if (!Directory.Exists(project.AbsoluteFolderPath + project.ClientList[i].RelativePath))
                    {
                        Directory.CreateDirectory(project.AbsoluteFolderPath + project.ClientList[i].RelativePath);
                    }
                    project.ClientList[i].SaveClientSettingFile(project.AbsoluteFolderPath + project.ClientList[i].RelativePath + "\\" + project.ClientsConfigFileName);

                    if (project.ClientList[i].Cyclogramm != null)
                    {
                        if (!project.ClientList[i].Cyclogramm.Saved)
                        {
                            string tmpcycpath = project.GetAbsoluteTEMPPath() + project.ClientList[i].RelativePath + "\\Data.cyc";
                            if (File.Exists(tmpcycpath))
                            {
                                if (File.Exists(project.AbsoluteFolderPath + project.ClientList[i].RelativePath + "\\Data.cyc"))
                                {
                                    File.Delete(project.AbsoluteFolderPath + project.ClientList[i].RelativePath + "\\Data.cyc");
                                }
                                File.Move(tmpcycpath, project.AbsoluteFolderPath + project.ClientList[i].RelativePath + "\\Data.cyc");
                                project.ClientList[i].Cyclogramm.Saved = true;
                            }
                        }
                    }                    

                    project.ClientList[i].Renamed = false;
                    project.ClientList[i].Saved = true;
                    if (project.ClientList[i].DeletedCyclogramm != null)
                    {
                        project.ClientList[i].DeletedCyclogramm = null;
                    }

                }
            }
            if (project.DeletedClientList != null)
            {
                for (int i = 0; i < project.DeletedClientList.Count; i++)
                {
                    try
                    {
                        Directory.Delete(project.AbsoluteFolderPath + project.DeletedClientList[i].RelativePath, true);
                    }
                    catch (DirectoryNotFoundException e)
                    {
                    }

                    catch (Exception)
                    {
                        continue;
                    }
                    project.DeletedClientList.RemoveAt(i);
                }
            }
            if (project.BindedAudioFile != null)
            {
                try
                {
                    project.BindedAudioFile = project.BindedAudioFile.CopyTo(Path.Combine(project.AbsoluteFolderPath, Path.GetFileName(project.BindedAudioFile.FullName)), true);
                }
                catch (Exception)
                {

                }
            }
            
            XMLSaver xmlsaver = new XMLSaver();
            xmlsaver.Fields = project;
            xmlsaver.WriteXml(project.AbsoluteFilePath);
            project.Saved = true;
        }

        private void SaveAs(string FolderPath, string FilePath)
        {
            for (int i = 0; i < project.ClientList.Count; i++)
            {
                Directory.CreateDirectory(FolderPath + project.ClientList[i].RelativePath);
                project.ClientList[i].SaveClientSettingFile(FolderPath + project.ClientList[i].RelativePath + "\\" + project.ClientsConfigFileName);
                if (project.ClientList[i].Cyclogramm != null)
                {
                    string tmpcycpath;
                    if (project.ClientList[i].Cyclogramm.Saved)
                    {
                        tmpcycpath = project.AbsoluteFolderPath + project.ClientList[i].RelativePath + "\\Data.cyc";
                    }
                    else
                    {
                        tmpcycpath = project.GetAbsoluteTEMPPath() + project.ClientList[i].RelativePath + "\\Data.cyc";
                    }

                    if (File.Exists(tmpcycpath))
                    {
                        if (project.ClientList[i].Cyclogramm.Saved)
                        {
                            File.Copy(tmpcycpath, FolderPath + project.ClientList[i].RelativePath + "\\Data.cyc");
                        }
                        else
                        {
                            File.Move(tmpcycpath, FolderPath + project.ClientList[i].RelativePath + "\\Data.cyc");
                        }

                        project.ClientList[i].Cyclogramm.Saved = true;
                    }
                }
                project.ClientList[i].Renamed = false;
                project.ClientList[i].Saved = true;
            }
            if (project.BindedAudioFile != null)
            {
                try
                {
                    project.BindedAudioFile = project.BindedAudioFile.CopyTo(Path.Combine(FolderPath, Path.GetFileName(project.BindedAudioFile.FullName)), true);
                }
                catch (Exception)
                {

                }                
            }            
            XMLSaver xmlsaver = new XMLSaver();
            xmlsaver.Fields = project;
            xmlsaver.WriteXml(FilePath);
            if (project.DeletedClientList != null)
            {
                project.DeletedClientList = null;
            }
            project.AbsoluteFilePath = FilePath;
            project.Saved = true;
        }

        /// <summary>
        /// Сохраняет настройки сервера в проекте
        /// </summary>
        private void button_SaveServerSetting_Click(object sender, EventArgs e)
        {
            //Сохранить сервер
            if (project != null)
            {
                if (project.Server.UDPPort != ushort.Parse(textBox_localPort.Text))
                {
                    if (project.Server.IsRun)
                    {
                        project.Server.StopReceiving();
                    }
                    project.Server.UDPPort = ushort.Parse(textBox_localPort.Text);
                    project.Server.ServerIPAdress = (comboBox_IP.SelectedItem as IPAddress);
                    try
                    {
                        project.Server.StartReceiving();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ошибка старта сервера. Возможно порт занят. Попробуйте использовать другой порт");
                    }
                }
                else
                {
                    project.Server.ServerIPAdress = (comboBox_IP.SelectedItem as IPAddress);
                }
            }
        }

        private void textBox_localPort_TextChanged(object sender, EventArgs e)
        {
            bool b = ServiceFunc.TextBoxValidation((sender as TextBox), TypesNumeric.t_uint16);
            ServiceFunc.ColoringTextBox((sender as TextBox), b);
            (sender as TextBox).Select((sender as TextBox).TextLength, 0);
            button_SaveServerSetting.Enabled = b;
        }

        /// <summary>
        /// Событие при отрисовке ячейки датагрида списка клиентов
        /// </summary>
        private void dataGridView_Clients_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (((sender as DataGridView).Rows[e.RowIndex].DataBoundItem as Client).OnlineStatus)
                {
                    (sender as DataGridView).Rows[e.RowIndex].Cells["Column_Status"].Style.BackColor = ServiceFunc.AcceptColor;
                    (sender as DataGridView).Rows[e.RowIndex].Cells["Column_Status"].Style.SelectionBackColor = ServiceFunc.AcceptColor;
                    (sender as DataGridView).Rows[e.RowIndex].Cells["Column_Status"].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    (sender as DataGridView).Rows[e.RowIndex].Cells["Column_Status"].Style.BackColor = ServiceFunc.RefuseColor;
                    (sender as DataGridView).Rows[e.RowIndex].Cells["Column_Status"].Style.SelectionBackColor = ServiceFunc.RefuseColor;
                    (sender as DataGridView).Rows[e.RowIndex].Cells["Column_Status"].Style.SelectionForeColor = Color.Black;
                }
            }
        }

        private void button_AddClient_Click(object sender, EventArgs e)
        {
            ClientValues TmpClientValues;
            TmpClientValues.Number = textBox_ClientNumber.Text;
            TmpClientValues.Name = textBox_ClientName.Text;
            TmpClientValues.WifiSSID = textBox_ClientSSID.Text;
            TmpClientValues.WifiPassword = textBox_ClientPasswordWifi.Text;
            TmpClientValues.UDPPort = ushort.Parse(textBox_ClientUDPPort.Text);
            TmpClientValues.LEDCount = textBox_ClientLEDCount.Text;
            TmpClientValues.RelativeFolderPath = "\\" + project.ClientsFolderName + "\\" + TmpClientValues.Name;
            int index = project.AddClient(TmpClientValues);
            comboBox_Client.SelectedIndex = index;
            LoadClientControlValues();
            //if (project.Saved && project.AbsoluteFolderPath != null)
            //{
            //    SaveClientSettingFile(project.AbsoluteFolderPath + (comboBox_Client.SelectedItem as Client).RelativePath + "\\" + project.ClientsConfigFileName);
            //}
            ClientSettingValidation();
        }

        private void button_DeleteClient_Click(object sender, EventArgs e)
        {
            if (comboBox_Client.Items.Count > 0)
            {
                if (MessageBox.Show("Вся информация о клиенте будет безвозвратно удалена, Вы действительно хотите продолжить?", "Удаление клиента", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    project.DeleteClient(comboBox_Client.SelectedItem as Client);
                    ClientSettingValidation();
                }
            }
        }

        private void button_SaveClient_Click(object sender, EventArgs e)
        {
            int index = project.ClientList.FindIndex(x => x.Number == Byte.Parse(textBox_ClientNumber.Text));
            if (index != -1)
            {
                project.ClientList[index].Name = textBox_ClientName.Text;
                project.ClientList[index].WifiSSID = textBox_ClientSSID.Text;
                project.ClientList[index].WifiPassword = textBox_ClientPasswordWifi.Text;
                project.ClientList[index].UDPPort = ushort.Parse(textBox_ClientUDPPort.Text);
                project.ClientList[index].LEDCount = textBox_ClientLEDCount.Text;
                LoadDataSourceClient();
                //if (project.Saved && project.AbsoluteFolderPath != null)
                //{
                //    SaveClientSettingFile(project.AbsoluteFolderPath + (comboBox_Client.SelectedItem as Client).RelativePath + "\\" + project.ClientsConfigFileName);
                //}
                ClientSettingValidation();
            }
        }

        private void comboBox_Client_SelectionChangeCommitted(object sender, EventArgs e)
        {
            LoadClientControlValues();
            LoadPinList();
            //LoadCyclogrammList();
            ClearPinAndCyclogrammTextBox();
        }

        private void comboBox_Client_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPinList();
            //LoadCyclogrammList();
            ClearPinAndCyclogrammTextBox();
        }

        private void button_AddPin_Click(object sender, EventArgs e)
        {
            (comboBox_Client.SelectedItem as Client).AddPin(textBox_PinNumber.Text, textBox_PinLEDCount.Text);
            ClientSettingValidation();
        }

        private void dataGridView_PinList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                (comboBox_Client.SelectedItem as Client).DeletePin((dataGridView_PinList.SelectedRows[0].DataBoundItem as Pin));
                LoadPinList();
                ClientSettingValidation();
            }
        }

        private void button_AddCyclogram_Click(object sender, EventArgs e)
        {
            if (project.AbsoluteFolderPath != null)
            {

                Cyclogramm TmpCyclogramm = new Cyclogramm();
                TmpCyclogramm.Parent = (comboBox_Client.SelectedItem as Client);
                TmpCyclogramm.Converted = false;
                TmpCyclogramm.Saved = false;

                string tempstring = project.GetAbsoluteTEMPPath() + (comboBox_Client.SelectedItem as Client).RelativePath;
                if (!Directory.Exists(tempstring))
                {
                    Directory.CreateDirectory(tempstring);
                }

                string OutputFileName = tempstring + "\\Data.cyc";

                FormConvertion progressForm = new FormConvertion();
                progressForm.InputFile = textBox_InputCyclogramm.Text;
                progressForm.OutputFile = OutputFileName;
                switch (progressForm.ShowDialog())
                {
                    case DialogResult.Yes:
                        FileInfo fi = new FileInfo(OutputFileName);
                        TmpCyclogramm.FileSize = (int)Math.Ceiling((double)fi.Length / 1024D);
                        (comboBox_Client.SelectedItem as Client).Cyclogramm = TmpCyclogramm;
                        (comboBox_Client.SelectedItem as Client).Cyclogramm.Converted = true;
                        break;
                    case DialogResult.No:
                        MessageBox.Show("Ошибка при конвертировании циклограммы, попробуйте снова. Данная циклограмма будет удалена из программы.");
                        break;
                    default:
                        break;
                }

                ClearPinAndCyclogrammTextBox();
                CyclogrammControlValidation();
            }
            else
            {
                MessageBox.Show("Необходимо сохранить проект");
            }
        }

        private void button_FindInputCyclogram_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "CSV files|*.csv";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox_InputCyclogramm.Text = ofd.FileName;
            }
        }

        //private void dataGridView_CyclogrammList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if ((project.ClientList.Count > 0) && (comboBox_Client.Items.Count > 0))
        //    {
        //        DataGridView DataGrid = (sender as DataGridView);
        //        Cyclogramm Cyc = DataGrid.Rows[e.RowIndex].DataBoundItem as Cyclogramm;
        //        if ((DataGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn) && (e.RowIndex >= 0))
        //        {
        //            if (!Cyc.Converted)
        //            {
        //                Convertion(Cyc);
        //            }
        //        }
        //    }
        //}

        //private void Convertion(Cyclogramm CurCyc)
        //{
        //    CyclogrammConverter Convertion = new CyclogrammConverter(CurCyc.OutputFileName, CurCyc.InputFileName);
        //    Convertion.project = project;
        //    Convertion.OnConverterSendMessage += (byte str2) =>
        //    {
        //        BeginInvoke(new Action(delegate ()
        //        {
        //            if (str2 == 98)
        //            {
        //                CurCyc.Converted = true;
        //            }
        //            else
        //            {
        //                CurCyc.ConvertedStr = (str2 + 2).ToString() + "%";
        //            }
        //        }));

        //    };
        //    Convertion.OnConvertionDone += (int ThreadID, bool done) =>
        //    {
        //        BeginInvoke(new Action(delegate ()
        //        {
        //            project.RemoveThread(ThreadID);
        //            if (!done)
        //            {
        //                MessageBox.Show("Ошибка при конвертировании циклограммы \"" + CurCyc.Name + "\", попробуйте снова. Данная циклограмма будет удалена из списка.");
        //                (comboBox_Client.SelectedItem as Client).DeleteCyclogramm(CurCyc);
        //            }
        //        }));
        //    };
        //    Thread newThread = new Thread((Convertion.Start));
        //    newThread.Start();
        //    project.AddThread(newThread);
        //}

        //public bool ConvertCyclogramm(string InputFile, string OutputFile)
        //{
        //    bool result = false;         
        //    try
        //    {
        //        progressBar_Cyclogramm.Visible = true;
        //        FileStream FileInput = new FileStream(InputFile, FileMode.Open, FileAccess.Read);
        //        byte CurrentByte;
        //        int CurrentByteBuffer;
        //        byte Percent = 0;
        //        uint[] ArPer = new uint[100];
        //        ArPer[0] = (uint)(FileInput.Length / 100);
        //        for (int i = 1; i < 99; i++)
        //        {
        //            ArPer[i] += ArPer[i - 1] + ArPer[0];
        //        }
        //        int ColorByteCount = 1; //счетчик от 1 до 3 (Определяет значение одного цвета)
        //        string colorBuffer = "";
        //        FileStream FileOutput = new FileStream(OutputFile, FileMode.OpenOrCreate, FileAccess.Write);
        //        FileOutput.Position = FileOutput.Length;
        //        while (FileInput.Position <= FileInput.Length)
        //        {
        //            if (ArPer[Percent] == FileInput.Position)
        //            {
        //                //OnConverterSendMessage(Percent);
        //                //progressForm.Progress = Percent;
        //                //progressBar_Cyclogramm.Value = Percent;
        //                this.Invalidate();
        //                Percent++;
        //            }

        //            CurrentByteBuffer = FileInput.ReadByte();
        //            if (CurrentByteBuffer > 0)
        //            {
        //                CurrentByte = (byte)(CurrentByteBuffer);
        //            }
        //            else
        //            {
        //                break;
        //            }

        //            // Проверка, является ли текущий байт запятой "," символом перевода на новую строку или символом возврата каретки
        //            if ((CurrentByte == 44) || (CurrentByte == 13) || (CurrentByte == 10))
        //                continue;

        //            if (ColorByteCount <= 3)
        //            {
        //                colorBuffer += Convert.ToChar(CurrentByte);
        //                ColorByteCount++;
        //            }

        //            if (ColorByteCount == 4)
        //            {
        //                ColorByteCount = 1;
        //                FileOutput.WriteByte(Convert.ToByte(colorBuffer));
        //                colorBuffer = "";
        //            }
        //        }
        //        FileInput.Close();
        //        FileOutput.Close();
        //        progressBar_Cyclogramm.Visible = false;
        //        result = true;
        //    }
        //    catch (Exception)
        //    {
        //        result = false;
        //        progressBar_Cyclogramm.Visible = false;
        //        MessageBox.Show("Ошибка при конвертировании циклограммы, попробуйте снова. Данная циклограмма будет удалена из программы.");
        //    }
        //    return result;
        //}
        //public void ConvertCyclogramm(string InputFile, string OutputFile, Form1 formDialog)
        //{
        //    bool result = false;
        //    try
        //    {
        //        progressBar_Cyclogramm.Visible = true;
        //        FileStream FileInput = new FileStream(InputFile, FileMode.Open, FileAccess.Read);
        //        byte CurrentByte;
        //        int CurrentByteBuffer;
        //        byte Percent = 0;
        //        uint[] ArPer = new uint[100];
        //        ArPer[0] = (uint)(FileInput.Length / 100);
        //        for (int i = 1; i < 99; i++)
        //        {
        //            ArPer[i] += ArPer[i - 1] + ArPer[0];
        //        }
        //        int ColorByteCount = 1; //счетчик от 1 до 3 (Определяет значение одного цвета)
        //        string colorBuffer = "";
        //        FileStream FileOutput = new FileStream(OutputFile, FileMode.OpenOrCreate, FileAccess.Write);
        //        FileOutput.Position = FileOutput.Length;
        //        while (FileInput.Position <= FileInput.Length)
        //        {
        //            if (ArPer[Percent] == FileInput.Position)
        //            {
        //                //OnConverterSendMessage(Percent);
        //                formDialog.Progress = Percent;
        //                //progressBar_Cyclogramm.Value = Percent;
        //                this.Invalidate();
        //                Percent++;
        //            }

        //            CurrentByteBuffer = FileInput.ReadByte();
        //            if (CurrentByteBuffer > 0)
        //            {
        //                CurrentByte = (byte)(CurrentByteBuffer);
        //            }
        //            else
        //            {
        //                break;
        //            }

        //            // Проверка, является ли текущий байт запятой "," символом перевода на новую строку или символом возврата каретки
        //            if ((CurrentByte == 44) || (CurrentByte == 13) || (CurrentByte == 10))
        //                continue;

        //            if (ColorByteCount <= 3)
        //            {
        //                colorBuffer += Convert.ToChar(CurrentByte);
        //                ColorByteCount++;
        //            }

        //            if (ColorByteCount == 4)
        //            {
        //                ColorByteCount = 1;
        //                FileOutput.WriteByte(Convert.ToByte(colorBuffer));
        //                colorBuffer = "";
        //            }
        //        }
        //        FileInput.Close();
        //        FileOutput.Close();
        //        //progressBar_Cyclogramm.Visible = false;
        //        formDialog.Result = true;
        //        //result = true;
        //        formDialog.Close();
        //    }
        //    catch (Exception)
        //    {
        //        formDialog.Result = false;
        //        //progressBar_Cyclogramm.Visible = false;
        //        //MessageBox.Show("Ошибка при конвертировании циклограммы, попробуйте снова. Данная циклограмма будет удалена из программы.");
        //        formDialog.Close();
        //    }
        //}

        private void button_RefreshIPList_Click(object sender, EventArgs e)
        {
            ServiceFunc.GetIPAdressList(comboBox_IP);
        }

        #endregion

        #region Client TextBox Validation methods

        private void textBox_ClientNumber_TextChanged(object sender, EventArgs e)
        {
            ClientSettingValidation();
            (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        }

        private void textBox_ClientName_TextChanged(object sender, EventArgs e)
        {
            ClientSettingValidation();
            (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        }

        private void textBox_ClientSSID_TextChanged(object sender, EventArgs e)
        {
            ClientSettingValidation();
            (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        }

        private void textBox_ClientPassowordWifi_TextChanged(object sender, EventArgs e)
        {
            ClientSettingValidation();
            (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        }

        private void textBox_ClientUDPPort_TextChanged(object sender, EventArgs e)
        {
            ClientSettingValidation();
            (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        }

        private void textBox_ClientLEDCount_TextChanged(object sender, EventArgs e)
        {
            ClientSettingValidation();
            (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        }

        private void textBox_PinNumber_TextChanged(object sender, EventArgs e)
        {
            if (ServiceFunc.TextBoxValidation(sender as TextBox, TypesNumeric.t_byte_with_zero))
            {
                if (PinValidation((sender as TextBox).Text))
                {
                    ServiceFunc.ColoringTextBox((sender as TextBox), true);
                    button_AddPin.Enabled = true;
                }
                else
                {
                    ServiceFunc.ColoringTextBox((sender as TextBox), false);
                    button_AddPin.Enabled = false;
                }
            }
            else
            {
                ServiceFunc.ColoringTextBox((sender as TextBox), false);
                button_AddPin.Enabled = false;
            }
        }

        private bool PinValidation(string PinText)
        {
            bool result = false;
            byte tmpByte;
            if (byte.TryParse(PinText, out tmpByte))
            {
                switch (tmpByte)
                {
                    case 0:
                        result = true;
                        break;
                    case 2:
                        result = true;
                        break;
                    case 4:
                        result = true;
                        break;
                    case 5:
                        result = true;
                        break;
                    default:
                        result = false;
                        break;
                }
            }
            return result;
        }

        private void textBox_PinLEDCount_TextChanged(object sender, EventArgs e)
        {
            ServiceFunc.TextBoxValidation(sender as TextBox, TypesNumeric.t_uint16);
        }

        private void textBox_CyclogrammName_TextChanged(object sender, EventArgs e)
        {
            CyclogrammControlValidation();
            (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        }

        private void textBox_InputCyclogramm_TextChanged(object sender, EventArgs e)
        {
            CyclogrammControlValidation();
        }

        /// <summary>
        /// Проверяет правильность введенной информации в контролы клиента, менят цвет контрола и активирует деактивирует кнопки
        /// в зависимости от правильности и полноты информации
        /// </summary>
        private void ClientSettingValidation()
        {
            byte CurrentNumber = 0;
            string CurrentName = "";

            bool Valid = true;
            if (!ServiceFunc.ColoringTextBox(textBox_ClientNumber, ServiceFunc.TextBoxValidation(textBox_ClientNumber, TypesNumeric.t_byte)))
            {
                Valid = false;
            }
            else
            {
                CurrentNumber = byte.Parse(textBox_ClientNumber.Text);
            }
            if (!ServiceFunc.ColoringTextBox(textBox_ClientName, ServiceFunc.TextBoxValidation(textBox_ClientName, TypesString.EnglishRusianDigitUnder)))
            {
                Valid = false;
            }
            else
            {
                CurrentName = textBox_ClientName.Text;
            }
            if (!ServiceFunc.ColoringTextBox(textBox_ClientSSID, ServiceFunc.TextBoxValidation(textBox_ClientSSID, TypesString.EnglishDigitUnderDefis)))
            {
                Valid = false;
            }
            if (!ServiceFunc.ColoringTextBox(textBox_ClientPasswordWifi, ServiceFunc.TextBoxValidation(textBox_ClientPasswordWifi, TypesString.AllWithoutSpace)))
            {
                Valid = false;
            }
            if (!ServiceFunc.ColoringTextBox(textBox_ClientUDPPort, ServiceFunc.TextBoxValidation(textBox_ClientUDPPort, TypesNumeric.t_uint16)))
            {
                Valid = false;
            }
            if (!ServiceFunc.ColoringTextBox(textBox_ClientLEDCount, ServiceFunc.TextBoxValidation(textBox_ClientLEDCount, TypesNumeric.t_uint16)))
            {
                Valid = false;
            }

            button_AddClient.Enabled = Valid;
            button_SaveClient.Enabled = Valid;

            if (CurrentNumber == 0)
            {
                ServiceFunc.ColoringTextBox(textBox_ClientNumber, false);
                button_AddClient.Enabled = false;
                button_SaveClient.Enabled = false;
            }
            else
            {
                if (project.ClientList.Exists(x => x.Number == CurrentNumber))
                {
                    ServiceFunc.ColoringTextBox(textBox_ClientNumber, false);
                    button_AddClient.Enabled = false;
                }
                else
                {
                    button_SaveClient.Enabled = false;
                }
            }
            if (CurrentName == "")
            {
                ServiceFunc.ColoringTextBox(textBox_ClientName, false);
                button_AddClient.Enabled = false;
                button_SaveClient.Enabled = false;
            }
            else
            {
                if (project.ClientList.Exists(x => x.Name == CurrentName))
                {
                    ServiceFunc.ColoringTextBox(textBox_ClientName, false);
                    button_AddClient.Enabled = false;
                    if (project.ClientList.Exists(x => x.Number == CurrentNumber))
                    {
                        button_SaveClient.Enabled = true;
                    }
                    else
                    {
                        button_SaveClient.Enabled = false;
                    }
                }
                if (project.DeletedClientList != null && project.DeletedClientList.Exists(x => x.Name == CurrentName))
                {
                    ServiceFunc.ColoringTextBox(textBox_ClientName, false);
                    button_SaveClient.Enabled = false;
                }
            }
            if (project.ClientList.Count > 0)
            {
                Client CurrentClient = (comboBox_Client.SelectedItem as Client);
                CurrentClient.PinListIsLock = false;
                panel_PinList.Enabled = true;
                if ((project.ClientList.Count > 0) && (comboBox_Client.Items.Count > 0) && (!CurrentClient.PinListIsLock))
                {
                    panel_PinList.Enabled = true;
                }
                ClientSaveImage.Visible = !CurrentClient.Saved;
                if (CurrentClient.PinList.Count > 0)
                {
                    int TmpSum = CurrentClient.PinList.Sum(x => x.LEDCount);
                    textBox_LEDCountCheck.Text = TmpSum.ToString() + "/" + CurrentClient.LEDCount;
                    if (TmpSum == int.Parse(CurrentClient.LEDCount))
                    {
                        textBox_LEDCountCheck.BackColor = ServiceFunc.AcceptColor;
                        groupBox_Cyclogramm.Enabled = true;
                        CyclogrammControlValidation();
                    }
                    else
                    {
                        textBox_LEDCountCheck.BackColor = ServiceFunc.RefuseColor;
                        groupBox_Cyclogramm.Enabled = true; //необходимо False для проверки по пинам
                        CyclogrammControlValidation();
                    }
                }
                else
                {
                    if (CurrentClient.LEDCount == "")
                    {
                        textBox_LEDCountCheck.Text = "0/0";
                    }
                    textBox_LEDCountCheck.Text = "0/" + CurrentClient.LEDCount;
                    textBox_LEDCountCheck.BackColor = ServiceFunc.RefuseColor;
                    groupBox_Cyclogramm.Enabled = true; //необходимо False для проверки по пинам
                    CyclogrammControlValidation();
                }

                button_DeleteClient.Enabled = true;
            }
            else
            {
                button_DeleteClient.Enabled = false;
            }

            if (comboBox_RemovableDrive.Items.Count > 0)
            {
                button_LoadToSD.Enabled = project.Saved;
            }
            else
            {
                button_LoadToSD.Enabled = false;
                GetRemovableDrives();
            }

        }

        /// <summary>
        /// Проверяет правильность заполнения контролов циклограмм
        /// </summary>
        private void CyclogrammControlValidation()
        {
            bool Valid = true;

            if (comboBox_Client.Items.Count > 0)
            {
                if ((comboBox_Client.SelectedItem as Client).Cyclogramm != null)
                {
                    label_CyclogrammStatus.Text = "Связанная циклограмма: \"Data.cyc\" Размер: \"" + (comboBox_Client.SelectedItem as Client).Cyclogramm.FileSize.ToString() + " Кб\"";
                    label_CyclogrammStatus.ForeColor = Color.DarkGreen;
                }
                else
                {
                    label_CyclogrammStatus.Text = "Нет связанной циклограммы";
                    label_CyclogrammStatus.ForeColor = Color.DarkRed;
                }
            }

            if (!ServiceFunc.ColoringTextBox(textBox_InputCyclogramm, File.Exists(textBox_InputCyclogramm.Text)))
            {
                Valid = false;
            }

            button_AddCyclogramm.Enabled = Valid;
        }

        #endregion

        #region Service Form Methods

        /// <summary>
        /// Действие при старте проекта (Открытии или Создании)
        /// </summary>
        private void OnProjectStart()
        {
            GetRemovableDrives();
            try
            {
                Directory.Delete(project.GetAbsoluteTEMPPath(), true);
            }
            catch (Exception)
            {
            }
            ServiceFunc.GetIPAdressList(comboBox_IP);
            if (project.timer == null)
            {
                project.timer = new System.Windows.Forms.Timer();
            }

            project.timer.Interval = 100;
            project.timer.Tick += Timer_Tick;
            project.timer.Enabled = true;

            project.OnActiveThreadsChange += Project_OnActiveThreadsChange;
            project.OnChangeClientList += LoadDataSourceClient;
            project.OnSave += Project_OnSave;
            project.Server.OnSendNumberPlate += Server_OnSendNumberPlate;
            project.Server.ServerIPAdress = (comboBox_IP.SelectedItem as IPAddress);
            project.Server.OnStatusChange += ShowServerStatus;
            project.Server.OnServerIPChange += ShowServerIP;
            project.Server.StartReceiving();
            SetClientsEvents();
            LoadDataSourceClient();
            LoadControlValues();
            ShowServerStatus();
            panel_Status.Visible = true;
            tabControl1.Enabled = true;
            Project_OnSave();
        }

        /// <summary>
        /// Получает список дисков из системы и добавляет съемные диски в раскрывающийся список
        /// </summary>
        private void GetRemovableDrives()
        {
            comboBox_RemovableDrive.Items.Clear();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType == DriveType.Removable)
                {
                    comboBox_RemovableDrive.Items.Add(d);
                    comboBox_RemovableDrive.SelectedIndex = 0;
                }
            }
        }

        private void SetClientsEvents()
        {
            if (project != null)
            {
                if (project.ClientList != null)
                {
                    foreach (Client client in project.ClientList)
                    {
                        client.OnChange -= Client_OnChange;
                        client.OnChange += Client_OnChange;
                        client.OnChangeStatus -= Client_OnChangeStatus;
                        client.OnChangeStatus += Client_OnChangeStatus;
                    }
                }
            }
        }

        private void Client_OnChangeStatus()
        {
            dataGridView_Clients.Refresh();
        }

        private void Client_OnChange()
        {
            LoadDataSourceClient();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (project != null)
            {
                if (project.ClientList != null)
                {

                    for (int i = 0; i < project.ClientList.Count; i++)
                    {
                        if (project.ClientList[i].OnlineTime < 5000000)
                        {
                            project.ClientList[i].OnlineTime += (uint)(project.timer.Interval);
                        }
                    }
                }
            }
        }

        private void Server_OnSendNumberPlate(byte plateNumber, IPEndPoint ip, ClientState state)
        {
            try
            {
                BeginInvoke(
                    new Action(delegate ()
                        {
                            Client client = project.ClientList.FirstOrDefault(x => x.Number == plateNumber);
                            if (client != null)
                            {
                                if (
                                // не ожидается ответа от клиента о воспроизведнии
                                !client.WaitPlayingStatus 
                                // если не онлайн
                                && !client.OnlineStatus 
                                // состояние клиента - ожидание воспроизведения
                                && state == ClientState.Wait 
                                // статус плеера - вопсроизведение
                                && rlcPlayer1.PlaybackStateStr == PlaybackState.Playing)
                                {
                                    client.Send_PlayFrom_12(rlcPlayer1.CurrentTime);
                                    client.WaitPlayingStatus = true;
                                }
                                if (state == ClientState.Play || state == ClientState.Pause)
                                {
                                    client.WaitPlayingStatus = false;
                                }

                                client.OnlineTime = 0; // Сбрасывание времени ожидания клиента, если время меньше определенного числа то клиент онлайн

                                if (client.IPAdress != ip.Address)
                                {
                                    client.IPAdress = ip.Address; // Установка IP адреса клиента, если указанный в клиенте не совпадает с адресом в пакете
                                }
                            }
                        }
                    )
                );
            }
            catch (Exception)
            {
            }
        }

        private void Project_OnSave()
        {
            ProjectSaveImage.Visible = !project.Saved;
        }

        /// <summary>
        /// Если есть активные потоки конвертирования циклограмм, блокирует некоторые елементы формы
        /// </summary>
        private void Project_OnActiveThreadsChange(bool HaveTreads)
        {
            if (!HaveTreads)
            {
                ClientSettingValidation();
                comboBox_Client.Enabled = true;
            }
            else
            {
                button_AddClient.Enabled = false;
                button_DeleteClient.Enabled = false;
                button_SaveClient.Enabled = false;
                comboBox_Client.Enabled = false;
            }
        }

        /// <summary>
        /// Отображает статус сервера на форме в строке информации
        /// </summary>
        private void ShowServerStatus()
        {
            if (project != null)
            {
                if (project.Server.IsRun)
                {
                    label_ServerStatus.Text = "Запущен (" + project.Server.UDPPort.ToString() + ")";
                    label_ServerStatus.ForeColor = Color.Green;
                }
                else
                {
                    label_ServerStatus.Text = "Не запущен";
                    label_ServerStatus.ForeColor = Color.Red;
                }
                ShowServerIP();
            }
        }

        /// <summary>
        /// Отображает текущий IP адрес сервера используемый для передачи данных
        /// </summary>
        private void ShowServerIP()
        {
            bool visible = false;
            if (project != null)
            {
                if (project.Server != null)
                {
                    if (project.Server.ServerIPAdress != null)
                    {
                        label_ServerIP.Text = project.Server.ServerIPAdress.ToString();
                        visible = true;
                    }
                }
            }
            label_ServerIP.Visible = visible;
        }

        /// <summary>
        /// Загружает значения в контролы из текущего проекта
        /// </summary>
        private void LoadControlValues()
        {
            label_CurrentKey.Text = project.Key.ToString();
            textBox_localPort.Text = project.Server.UDPPort.ToString();
            LoadClientControlValues();
        }

        /// <summary>
        /// Загружает значения в контролы настроек клиента из текущего проекта
        /// </summary>
        private void LoadClientControlValues()
        {
            if (project.ClientList.Count > 0)
            {
                textBox_ClientNumber.Text = (comboBox_Client.SelectedItem as Client).Number.ToString();
                textBox_ClientName.Text = (comboBox_Client.SelectedItem as Client).Name;
                textBox_ClientSSID.Text = (comboBox_Client.SelectedItem as Client).WifiSSID;
                textBox_ClientPasswordWifi.Text = (comboBox_Client.SelectedItem as Client).WifiPassword;
                textBox_ClientUDPPort.Text = (comboBox_Client.SelectedItem as Client).UDPPort.ToString();
                textBox_ClientLEDCount.Text = (comboBox_Client.SelectedItem as Client).LEDCount;
            }
            ClientSettingValidation();
        }

        /// <summary>
        /// Загружает данные из списка клиентов на элементы отображения
        /// </summary>
        private void LoadDataSourceClient()
        {
            int TmpIndex = 0;
            if (comboBox_Client.SelectedIndex > 0)
            {
                TmpIndex = comboBox_Client.SelectedIndex;
            }
            BindingSource source = new BindingSource();
            source.DataSource = project.ClientList;
            comboBox_Client.DataSource = source;
            dataGridView_Clients.DataSource = source;
            dataGridView_Clients.Refresh();
            if (project.ClientList.Count > 0)
            {
                if (TmpIndex >= project.ClientList.Count)
                {
                    comboBox_Client.SelectedIndex = project.ClientList.Count - 1;
                }
                else
                {
                    comboBox_Client.SelectedIndex = TmpIndex;
                }
                LoadClientControlValues();
            }

        }

        /// <summary>
        /// Загружает список пинов текущего клиента на форму
        /// </summary>
        private void LoadPinList()
        {
            if ((project.ClientList.Count > 0) && (comboBox_Client.Items.Count > 0))
            {
                (comboBox_Client.SelectedItem as Client).OnChangePinList += () =>
                {
                    BindingSource source = new BindingSource();
                    source.DataSource = (comboBox_Client.SelectedItem as Client).PinList;
                    dataGridView_PinList.DataSource = source;
                    ClientSettingValidation();
                };

                BindingSource source2 = new BindingSource();
                source2.DataSource = (comboBox_Client.SelectedItem as Client).PinList;
                dataGridView_PinList.DataSource = source2;
            }
        }

        /// <summary>
        /// Очищает елементы формы и данные использемого проекта
        /// </summary>
        private void ClearProject()
        {
            textBox_ClientNumber.Text = "";
            textBox_ClientName.Text = "";
            textBox_ClientSSID.Text = "";
            textBox_ClientPasswordWifi.Text = "";
            textBox_ClientUDPPort.Text = "";
            textBox_ClientLEDCount.Text = "";
            textBox_InputCyclogramm.Text = "";
            textBox_LEDCountCheck.Text = "";
            textBox_localPort.Text = "";
            textBox_PinLEDCount.Text = "";
            textBox_PinNumber.Text = "";
            label_CyclogrammStatus.Text = "";
            rlcPlayer1.Reset();

            if (project != null)
            {
                if (project.Server != null)
                {
                    if (project.Server.IsRun)
                    {
                        project.Server.StopReceiving();
                    }
                }
            }
        }

        /// <summary>
        /// Очищает контролы пинов и циклограмм
        /// </summary>
        private void ClearPinAndCyclogrammTextBox()
        {
            textBox_PinNumber.Text = "";
            textBox_PinLEDCount.Text = "";
            textBox_InputCyclogramm.Text = "";
        }

        #endregion

        #region DragDrop methods

        private void CyclogramDragDrop(object sender, DragEventArgs e)
        {
            string[] objects = (string[])e.Data.GetData(DataFormats.FileDrop);
            textBox_InputCyclogramm.Text = objects[0];
            (sender as Control).DragDrop -= CyclogramDragDrop;
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move))
            {
                string[] objects = null;
                objects = (string[])e.Data.GetData(DataFormats.FileDrop);
                if ((objects.Length == 1) & (string.Equals(Path.GetExtension(objects[0]), ".csv", StringComparison.InvariantCultureIgnoreCase)))
                {
                    e.Effect = DragDropEffects.Move;
                    (sender as Control).DragDrop -= CyclogramDragDrop;
                    (sender as Control).DragDrop += CyclogramDragDrop;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        #endregion

        private void comboBox_AudioOutputs_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!(rlcPlayer1.PlaybackStateStr == PlaybackState.Stopped))
            {
                if (MessageBox.Show("В данный момент производится воспроизведение, прервать воспроизведение и сменить устройство?", "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    button_Stop.PerformClick();
                    rlcPlayer1.Device = (comboBox_AudioOutputs.SelectedItem as MMDevice);
                }
                else
                {
                    comboBox_AudioOutputs.SelectedItem = rlcPlayer1.Device;
                }
            }
        }

        private void ComboBoxAudioOutputs_Fill()
        {
            var selectedObject = comboBox_AudioOutputs.SelectedItem;
            string deviceId = "";
            MMDevice selectedDevice;
            try
            {
                selectedDevice = selectedObject as MMDevice;
                if (selectedDevice.DeviceState == DeviceState.Active)
                {
                    deviceId = selectedDevice.DeviceID;
                }
            }
            catch (Exception)
            {
            }

            rlcPlayer1.ReloadDevices();
            var listDevices = rlcPlayer1._devices.Where(x => x.DeviceState == DeviceState.Active).ToList();
            var currentItem = listDevices.FirstOrDefault(x => x.DeviceID == deviceId);            

            comboBox_AudioOutputs.DataSource = listDevices;

            if (currentItem != null)
            {
                comboBox_AudioOutputs.SelectedItem = currentItem;
            }
            comboBox_AudioOutputs.DisplayMember = "FriendlyName";
            comboBox_AudioOutputs.ValueMember = "DeviceID";
        }

        private void RemoteLEDControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (project != null)
            {
                string message = "";

                message += "Все не сохраненные данные будут утеряны...\n";
                message += "ЕСЛИ ЗАПУЩЕН ПРОЕКТ, ВСЕ КЛИЕНТЫ БУДУТ ОСТАНОВЛЕНЫ!!! \n";
                message += "Продолжить?\n";

                if (MessageBox.Show(message, "Закрытие программы", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (project.Server != null)
                    {
                        if (project.Server.IsRun)
                        {
                            project.Server.Send_StopAll_2();
                            project.Server.StopReceiving();
                            while (project.Server.IsRun) { }
                        }
                    }
                }

            }
        }

        private void maskedTextBox_SetTime_TextChanged(object sender, EventArgs e)
        {
            if ((sender as MaskedTextBox).Text.Length == 5 && !(sender as MaskedTextBox).Text.Contains(" "))
            {
                int minutes;
                int seconds;
                if (int.TryParse((sender as MaskedTextBox).Text.Substring(0, 2), out minutes) &&
                    int.TryParse((sender as MaskedTextBox).Text.Substring(3, 2), out seconds))
                {
                    if (seconds > 59)
                    {
                        (sender as MaskedTextBox).Text = (sender as MaskedTextBox).Text.Substring(0, 3) + "59";
                        seconds = 59;
                    }
                    (sender as MaskedTextBox).ForeColor = Color.Green;
                    button_PlayFrom.Enabled = true;
                    return;
                }
            }

            (sender as MaskedTextBox).ForeColor = Color.Red;
            button_PlayFrom.Enabled = false;

        }

        private void maskedTextBox_SetTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && button_PlayFrom.Enabled)
            {
                button_PlayFrom.PerformClick();
            }
        }

        private void TempOpen(string path)
        {
            XMLSaver xml = new XMLSaver();
            xml.Fields = new Project(1);
            try
            {
                xml.ReadXml(path);
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка открытия файла, возможно файл не предназначен для этой программы.");
                return;
            }
            string FileName = Path.GetFileNameWithoutExtension(path);
            string FolderPath = Path.GetDirectoryName(path);
            string FolderName = Path.GetFileName(FolderPath);

            if (FolderName != FileName)
            {
                if (MessageBox.Show("Проект должен находиться в папке проекта с тем же названием, будет создана папка " +
                    FileName + ", и в нее перемещен файл проекта и приложенные файлы. Продолжить открытие?",
                    "Предупреждение", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (Directory.Exists(FolderPath + "\\" + FileName))
                    {
                        MessageBox.Show("Папка \"" + FileName + "\" уже существует. Невозможно продолжить", "Ошибка сохранения", MessageBoxButtons.OK);
                        return;
                    }
                    else
                    {
                        Directory.CreateDirectory(FolderPath + "\\" + FileName);
                        string FilePath = FolderPath + "\\" + FileName + "\\" + FileName + ".xml";
                        File.Move(path, FilePath);
                        Directory.Move(FolderPath + "\\" + xml.Fields.ClientsFolderName, FolderPath + "\\" + FileName + "\\" + xml.Fields.ClientsFolderName);
                        xml = new XMLSaver();
                        xml.Fields = new Project(1);
                        try
                        {
                            xml.ReadXml(FilePath);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Ошибка открытия файла, возможно файл не предназначен для этой программы.");
                            return;
                        }

                        ClearProject();
                        project = xml.Fields;
                        project.RuningThreadsList = new List<Thread>();
                        project.AbsoluteFilePath = FilePath;
                        if (project.Server != null)
                        {
                            if (project.Server.ProjectKey != project.Key)
                            {
                                project.Server.ProjectKey = project.Key;
                            }
                        }
                        project.Saved = true;
                        ToolStripMenuItem_SaveProject.Enabled = true;
                        ToolStripMenuItem_SaveAsProject.Enabled = true;
                        OnProjectStart();
                        return;
                    }

                }
                else
                {
                    return;
                }

            }

            xml = new XMLSaver();
            xml.Fields = new Project(1);
            try
            {
                xml.ReadXml(path);
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка открытия файла, возможно файл не предназначен для этой программы.");
                return;
            }

            ClearProject();
            project = xml.Fields;
            project.RuningThreadsList = new List<Thread>();
            project.AbsoluteFilePath = path;
            if (project.Server != null)
            {
                if (project.Server.ProjectKey != project.Key)
                {
                    project.Server.ProjectKey = project.Key;
                }
            }
            project.Saved = true;
            ToolStripMenuItem_SaveProject.Enabled = true;
            ToolStripMenuItem_SaveAsProject.Enabled = true;
            OnProjectStart();
        }

        private void richTextBox1_DoubleClick(object sender, EventArgs e)
        {
            (sender as RichTextBox).Clear();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (comboBox_RemovableDrive.Items.Count == 0)
            {
                GetRemovableDrives();
                return;
            }
            else
            {
                if ((comboBox_RemovableDrive.SelectedItem as DriveInfo).IsReady)
                {
                    List<FileInfo> ListFiles = new List<FileInfo>();
                    List<FileInfo> ErrorFiles = new List<FileInfo>();
                    List<FileInfo> NotExistFiles = new List<FileInfo>();
                    Client tmpClient = (comboBox_Client.SelectedItem as Client);

                    // Файл настроек клиента
                    FileInfo SetupFile = new FileInfo(tmpClient.Parent.AbsoluteFolderPath + tmpClient.RelativePath + "\\" + tmpClient.Parent.ClientsConfigFileName);
                    FileInfo DestSetupFile = new FileInfo((comboBox_RemovableDrive.SelectedItem as DriveInfo).RootDirectory + SetupFile.Name);
                    if (!SetupFile.Exists)
                    {
                        NotExistFiles.Add(SetupFile);
                    }
                    if (ServiceFunc.FileNotExistsOrHaveAccess(DestSetupFile))
                    {
                        ListFiles.Add(DestSetupFile);
                    }
                    else
                    {
                        ErrorFiles.Add(DestSetupFile);
                    }
                    // Файл циклограммы

                    FileInfo CyclogrammFile = new FileInfo(tmpClient.Parent.AbsoluteFolderPath + tmpClient.RelativePath + "\\Data.cyc");
                    FileInfo DestCyclogrammFile = new FileInfo((comboBox_RemovableDrive.SelectedItem as DriveInfo).RootDirectory + "Data.cyc");

                    if (!CyclogrammFile.Exists)
                    {
                        NotExistFiles.Add(CyclogrammFile);
                    }

                    if (ServiceFunc.FileNotExistsOrHaveAccess(DestCyclogrammFile))
                    {
                        ListFiles.Add(DestCyclogrammFile);
                    }
                    else
                    {
                        ErrorFiles.Add(DestCyclogrammFile);
                    }


                    if (ErrorFiles.Count > 0 || NotExistFiles.Count > 0)
                    {
                        string error = "";
                        if (NotExistFiles.Count > 0)
                        {
                            error += "Следущих необходимых файлов не существует:\n";
                            foreach (FileInfo file in NotExistFiles)
                            {
                                error += "(" + file.FullName + ")\n";
                            }
                            error += "\n";
                        }

                        if (ErrorFiles.Count > 0)
                        {
                            error += "Ошибка замены следующих файлов на съемном носителе (нет доступа):\n";
                            foreach (FileInfo file in ErrorFiles)
                            {
                                error += "(" + file.FullName + ")\n";
                            }
                            error += "\n";
                        }
                        MessageBox.Show(error + "Перенос данных на съемный носитель невозможен.", "Ошибка переноса данных");
                        return;
                    }

                    // Вызывает стандартный диалог копирования файлов в каталоге
                    FileSystem.CopyDirectory(
                        tmpClient.Parent.AbsoluteFolderPath + tmpClient.RelativePath,
                        (comboBox_RemovableDrive.SelectedItem as DriveInfo).RootDirectory.FullName,
                        UIOption.AllDialogs,
                        UICancelOption.DoNothing);
                }
                else
                {
                    GetRemovableDrives();
                    return;
                }
            }
        }

        private void button_TurnOffServer_Click(object sender, EventArgs e)
        {
            if (project != null)
            {
                if (project.Server.IsRun)
                {
                    project.Server.StopReceiving();
                    ResetPlayer();
                }
                else
                {
                    try
                    {
                        project.Server.StartReceiving();
                        ResetPlayer();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ошибка старта сервера. Возможно порт занят. Попробуйте использовать другой порт");
                    }
                }
            }
        }

        private void ResetPlayer()
        {
            rlcPlayer1.Stop();
            rlcPlayer1.CurrentTime = TimeSpan.Zero;
        }
        private void button_Play_Click(object sender, EventArgs e)
        {
            if (rlcPlayer1.TotalTime == TimeSpan.Zero)
            {
                return;
            }
            if (project.Server.IsRun)
            {
                if (rlcPlayer1.PlaybackStateStr == PlaybackState.Stopped || rlcPlayer1.PlaybackStateStr == PlaybackState.Paused)
                {
                    rlcPlayer1.Play();
                    project.Server.Send_PlayAll_1();
                }
            }
            else
            {
                rlcPlayer1.Play();
            }
        }

        private void button_Pause_Click(object sender, EventArgs e)
        {
            if (rlcPlayer1.TotalTime == TimeSpan.Zero)
            {
                return;
            }
            if (project.Server.IsRun)
            {
                if (rlcPlayer1.PlaybackStateStr == PlaybackState.Playing)
                {
                    project.Server.Send_PauseAll_6();
                    Thread.Sleep(2);
                    rlcPlayer1.Pause();
                }
            }
            else
            {
                rlcPlayer1.Pause();
            }
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            if (rlcPlayer1.TotalTime == TimeSpan.Zero)
            {
                return;
            }
            if (project.Server.IsRun)
            {
                if (rlcPlayer1.PlaybackStateStr == PlaybackState.Playing || rlcPlayer1.PlaybackStateStr == PlaybackState.Paused)
                {
                    rlcPlayer1.Stop();
                    rlcPlayer1.CurrentTime = TimeSpan.Zero;
                    project.Server.Send_StopAll_2();
                }
            }
            else
            {
                rlcPlayer1.Stop();
                rlcPlayer1.CurrentTime = TimeSpan.Zero;
            }
        }

        private void button_Open_Click(object sender, EventArgs e)
        {
            //открыть
            OpenFileDialog od = new OpenFileDialog();
            od.InitialDirectory = Environment.CurrentDirectory;
            od.Filter = CodecFactory.SupportedFilesFilterEn;
            od.Multiselect = false;
            if (od.ShowDialog() == DialogResult.OK)
            {
                rlcPlayer1.InitializePlayer(od.FileName);
            }
        }

        private void button_PlayFrom_Click(object sender, EventArgs e)
        {
            int minutes;
            int seconds;
            if (int.TryParse(maskedTextBox_SetTime.Text.Substring(0, 2), out minutes) &&
                int.TryParse(maskedTextBox_SetTime.Text.Substring(3, 2), out seconds))
            {
                double min = TimeSpan.FromMinutes((double)minutes).TotalMilliseconds;
                double sec = TimeSpan.FromSeconds((double)seconds).TotalMilliseconds;
                rlcPlayer1.CurrentTime = TimeSpan.FromMilliseconds(min + sec);
                if (project.Server.IsRun)
                {
                    if (rlcPlayer1.PlaybackStateStr == PlaybackState.Stopped || rlcPlayer1.PlaybackStateStr == PlaybackState.Paused)
                    {                        
                        project.Server.Send_PlayFromAll_7(rlcPlayer1.CurrentTime);
                        Thread.Sleep(2);
                        rlcPlayer1.Play();
                    }
                }
                else
                {
                    rlcPlayer1.Play();
                }                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetRemovableDrives();
            ClientSettingValidation();
        }

        private void button_CopyClient_Click(object sender, EventArgs e)
        {
            if (comboBox_Client.Items.Count > 0)
            {
                Client SelectedClient = (comboBox_Client.SelectedItem as Client);

                ClientValues cv = new ClientValues();
                for (int i = 1; i < 255; i++)
                {
                    if (!project.ClientList.Exists(x => x.Number == i))
                    {
                        cv.Number = i.ToString();
                        break;
                    }
                }
                for (int i = 1; i < int.MaxValue; i++)
                {
                    if (!project.ClientList.Exists(x => x.Name == (SelectedClient.Name + i.ToString())))
                    {
                        cv.Name = SelectedClient.Name + i.ToString();
                        break;
                    }
                }
                cv.WifiSSID = SelectedClient.WifiSSID;
                cv.WifiPassword = SelectedClient.WifiPassword;
                cv.UDPPort = SelectedClient.UDPPort;
                cv.LEDCount = SelectedClient.LEDCount;
                cv.RelativeFolderPath = "\\" + project.ClientsFolderName + "\\" + cv.Name;
                int index = project.AddClient(cv);
                if (index > 0)
                {
                    foreach (Pin SourcePin in SelectedClient.PinList)
                    {
                        project.ClientList[index].AddPin(SourcePin.PinNumber.ToString(), SourcePin.LEDCount.ToString());
                    }
                }
            }
        }

        private void volumeSlider2_VolumeChanged(object sender, EventArgs e)
        {
            rlcPlayer1.Volume = (sender as VolumeSlider.VolumeSlider).Volume;
        }

        private void rlcPlayer1_OnMouseSetTime()
        {
            if (rlcPlayer1.CurrentTime.Minutes >= 99)
            {
                return;
            }
            if (project.Server.IsRun)
            {
                maskedTextBox_SetTime.Text = String.Format("{0,2}:{1,2}", rlcPlayer1.CurrentTime.Minutes.ToString("D2"), rlcPlayer1.CurrentTime.Seconds.ToString("D2"));
                project.Server.Send_PlayFromAll_7(rlcPlayer1.CurrentTime);
                rlcPlayer1.Play();
            }
            else
            {
                rlcPlayer1.Play();
            }
        }

        private void rlcPlayer1_OnInitializedPlayer(string FilePath)
        {
            FileInfo tmpFile = new FileInfo(FilePath);
            if (tmpFile.Exists)
            {
                if (project != null)
                {
                    if (project.BindedAudioFile != null)
                    {
                        if (project.BindedAudioFile.FullName != tmpFile.FullName)
                        {
                            project.BindedAudioFile = tmpFile;
                        }
                    }
                    else
                    {
                        project.BindedAudioFile = tmpFile;
                    }                   
                }
            }
        }

        private void comboBox_AudioOutputs_DropDown(object sender, EventArgs e)
        {
            ComboBoxAudioOutputs_Fill();
        }

        private void buttonPlayerRestart_Click(object sender, EventArgs e)
        {
            string openedFile = rlcPlayer1.CurrentAudioFile;
            if(MessageBox.Show("Будет остановлено воспроизведение и перезагружен плеер!", "Внимание!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                rlcPlayer1.Reset();
                if (!String.IsNullOrEmpty(openedFile))
                {
                    rlcPlayer1.InitializePlayer(openedFile);
                }
            }
            
        }
    }
}