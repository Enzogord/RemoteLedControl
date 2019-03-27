using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Windows.Forms;
using Core;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using Microsoft.VisualBasic.FileIO;
using Service;

namespace RemoteLEDServer
{
    public partial class RemoteLEDControl : Form
    {
        //public RLCProjectController ProjectController { get; set; }

        //[Obsolete("Временный проброс до исправления всего ссылающегося кода")]
        //RemoteControlProject project => ProjectController.CurrentProject;

        //public RemoteLEDControl()
        //{
        //    InitializeComponent();
        //}

        //private void RemoteLEDControl_Load(object sender, EventArgs e)
        //{
        //    ComboBoxAudioOutputs_Fill();
        //    dataGridView_Clients.AutoGenerateColumns = false;
        //    dataGridView_PinList.AutoGenerateColumns = false;
        //    labelBroadcastAddress.Text = ProjectController.NetworkController.BroadcastIPAddress.ToString();

        //    FIXME тестовые вызовы для дебага
        //    tabControl1.Enabled = true;
        //    OpenProject(ProjectController.CurrentProject, Environment.CurrentDirectory);
        //    rlcPlayer1.InitializePlayer(ProjectController.CurrentProject.AudioFile.FullName);
        //    OnProjectStart();
        //}

        //#region Forms Control Events

        /// <summary>
        ///  Создать проект
        /// </summary>
        //private void ToolStripMenuItem_CreateProject_Click(object sender, EventArgs e)
        //{

        //    Создать
        //    FormCreateProject f = new FormCreateProject();
        //    if (f.ShowDialog() == DialogResult.OK)
        //    {
        //        ClearProject();
        //        ProjectController.CurrentProject = new RemoteControlProject(f.ProjectKey);
        //        project.Saved = false;
        //        ToolStripMenuItem_SaveProject.Enabled = true;
        //        ToolStripMenuItem_SaveAsProject.Enabled = true;
        //        OnProjectStart();
        //    }
        //    else
        //    {
        //        if (project == null)
        //        {
        //            ToolStripMenuItem_SaveProject.Enabled = false;
        //            ToolStripMenuItem_SaveAsProject.Enabled = false;
        //        }
        //    }
        //}

        /// <summary>
        /// Открыть проект
        /// </summary>
        //private void ToolStripMenuItem_OpenProject_Click(object sender, EventArgs e)
        //{
        //    Открыть            
        //    OpenFileDialog od = new OpenFileDialog();
        //    od.InitialDirectory = Environment.CurrentDirectory;
        //    od.Filter = "XML files|*.xml";
        //    od.Multiselect = false;
        //    if (od.ShowDialog() == DialogResult.OK)
        //    {
        //        XMLSaver xml = new XMLSaver();
        //        xml.Fields = new RemoteControlProject(1);
        //        try
        //        {
        //            xml.ReadXml(od.FileName);
        //        }
        //        catch (Exception)
        //        {
        //            MessageBox.Show("Ошибка открытия файла, возможно файл не предназначен для этой программы.");
        //            return;
        //        }

        //        string FileName = Path.GetFileNameWithoutExtension(od.FileName);
        //        string FolderPath = Path.GetDirectoryName(od.FileName);
        //        string FolderName = Path.GetFileName(FolderPath);

        //        if (FolderName != FileName)
        //        {
        //            if (MessageBox.Show("Проект должен находиться в папке проекта с тем же названием, будет создана папка " +
        //                FileName + ", и в нее перемещен файл проекта и приложенные файлы. Продолжить открытие?",
        //                "Предупреждение", MessageBoxButtons.OKCancel) == DialogResult.OK)
        //            {
        //                if (Directory.Exists(FolderPath + "\\" + FileName))
        //                {
        //                    MessageBox.Show("Папка \"" + FileName + "\" уже существует. Невозможно продолжить", "Ошибка сохранения", MessageBoxButtons.OK);
        //                    return;
        //                }
        //                else
        //                {
        //                    Directory.CreateDirectory(FolderPath + "\\" + FileName);
        //                    string FilePath = FolderPath + "\\" + FileName + "\\" + FileName + ".xml";
        //                    File.Move(od.FileName, FilePath);
        //                    Directory.Move(FolderPath + "\\" + xml.Fields.ClientsFolderName, FolderPath + "\\" + FileName + "\\" + xml.Fields.ClientsFolderName);
        //                    xml = new XMLSaver();
        //                    xml.Fields = new RemoteControlProject(1);
        //                    try
        //                    {
        //                        xml.ReadXml(FilePath);
        //                    }
        //                    catch (Exception)
        //                    {
        //                        MessageBox.Show("Ошибка открытия файла, возможно файл не предназначен для этой программы.");
        //                        return;
        //                    }

        //                    ClearProject();
        //                    ProjectController.CurrentProject = xml.Fields;
        //                    project.RuningThreadsList = new List<Thread>();
        //                    project.AbsoluteFilePath = FilePath;
        //                    if (ProjectController.Server != null)
        //                    {
        //                        if (ProjectController.Server.ProjectKey != project.Key)
        //                        {
        //                            ProjectController.Server.ProjectKey = project.Key;
        //                        }
        //                    }
        //                    ProjectController.UDPServer = new UDPServer(project.Key);
        //                    project.Saved = true;
        //                    ToolStripMenuItem_SaveProject.Enabled = true;
        //                    ToolStripMenuItem_SaveAsProject.Enabled = true;
        //                    OnProjectStart();
        //                    return;
        //                }

        //            }
        //            else
        //            {
        //                return;
        //            }

        //        }

        //        xml = new XMLSaver();
        //        xml.Fields = new RemoteControlProject(1);
        //        try
        //        {
        //            xml.ReadXml(od.FileName);
        //        }
        //        catch (Exception)
        //        {
        //            MessageBox.Show("Ошибка открытия файла, возможно файл не предназначен для этой программы.");
        //            return;
        //        }

        //        OpenProject(xml.Fields, od.FileName);
        //    }
        //}

        //public void OpenProject(RemoteControlProject project, string filePath)
        //{
        //    ClearProject();
        //    ProjectController.CurrentProject = project;
        //    project.RuningThreadsList = new List<Thread>();
        //    project.AbsoluteFilePath = filePath;
        //    if(ProjectController.Server != null) {
        //        if(ProjectController.Server.ProjectKey != project.Key) {
        //            ProjectController.Server.ProjectKey = project.Key;
        //        }
        //    }
        //    ProjectController.UDPServer = new UDPServer(project.Key);
        //    project.Saved = true;
        //    ToolStripMenuItem_SaveProject.Enabled = true;
        //    ToolStripMenuItem_SaveAsProject.Enabled = true;
        //    if(project.AudioFile != null) {
        //        if(project.AudioFile.Exists) {
        //            rlcPlayer1.InitializePlayer(project.AudioFile.FullName);
        //        } else {
        //            MessageBox.Show("Сохраненного в проекта файла \"" + project.AudioFile.FullName + "\" не существует на диске. Необходимо вручную добавить аудио файл в плеер.", "Ошибка открытия аудио файла");
        //        }
        //    }
        //    OnProjectStart();
        //}

        /// <summary>
        /// Сохранить проект
        /// </summary>
        //private void ToolStripMenuItem_SaveProject_Click(object sender, EventArgs e)
        //{
        //    Сохранить
        //    if (project == null)
        //    {
        //        return;
        //    }

        //    if (project.AbsoluteFilePath == null)
        //    {
        //        ProjectController.SaveProjectAs();
        //    }
        //    else
        //    {
        //        ProjectController.SaveProject();
        //    }
        //}

        /// <summary>
        /// Сохранить проект как
        /// </summary>
        //private void ToolStripMenuItem_SaveAsProject_Click(object sender, EventArgs e)
        //{
        //    Сохранить как
        //    if (project == null)
        //    {
        //        return;
        //    }
        //    ProjectController.SaveProjectAs();
        //}

        /// <summary>
        /// Сохраняет настройки сервера в проекте
        /// </summary>
        //private void button_SaveServerSetting_Click(object sender, EventArgs e)
        //{ 
        //    Сохранить сервер
        //    /*if (ProjectController != null && ProjectController.Server != null)
        //    {
        //        ProjectController.Server.UDPPort = ushort.Parse(textBox_localPort.Text);
        //        ProjectController.Server.ServerIPAdress = ServerIP;
        //    }*/
        //}

        //private void textBox_localPort_TextChanged(object sender, EventArgs e)
        //{
        //    bool b = ServiceFunc.TextBoxValidation((sender as TextBox), TypesNumeric.t_uint16);
        //    ServiceFunc.ColoringTextBox((sender as TextBox), b);
        //    (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        //    button_SaveServerSetting.Enabled = b;
        //}

        /// <summary>
        /// Событие при отрисовке ячейки датагрида списка клиентов
        /// </summary>
        //private void dataGridView_Clients_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        //{
        //    if (e.RowIndex >= 0)
        //    {
        //        if (((sender as DataGridView).Rows[e.RowIndex].DataBoundItem as RemoteClient).IsOnline)
        //        {
        //            (sender as DataGridView).Rows[e.RowIndex].Cells["Column_Status"].Style.BackColor = ServiceFunc.AcceptColor;
        //            (sender as DataGridView).Rows[e.RowIndex].Cells["Column_Status"].Style.SelectionBackColor = ServiceFunc.AcceptColor;
        //            (sender as DataGridView).Rows[e.RowIndex].Cells["Column_Status"].Style.SelectionForeColor = Color.Black;
        //        }
        //        else
        //        {
        //            (sender as DataGridView).Rows[e.RowIndex].Cells["Column_Status"].Style.BackColor = ServiceFunc.RefuseColor;
        //            (sender as DataGridView).Rows[e.RowIndex].Cells["Column_Status"].Style.SelectionBackColor = ServiceFunc.RefuseColor;
        //            (sender as DataGridView).Rows[e.RowIndex].Cells["Column_Status"].Style.SelectionForeColor = Color.Black;
        //        }
        //    }
        //}

        //private void button_AddClient_Click(object sender, EventArgs e)
        //{
        //    ClientValues TmpClientValues;
        //    TmpClientValues.Number = textBox_ClientNumber.Text;
        //    TmpClientValues.Name = textBox_ClientName.Text;
        //    TmpClientValues.WifiSSID = textBox_ClientSSID.Text;
        //    TmpClientValues.WifiPassword = textBox_ClientPasswordWifi.Text;
        //    TmpClientValues.UDPPort = ushort.Parse(textBox_ClientUDPPort.Text);
        //    TmpClientValues.LEDCount = textBox_ClientLEDCount.Text;
        //    TmpClientValues.RelativeFolderPath = "\\" + project.ClientsFolderName + "\\" + TmpClientValues.Name;
        //    int index = project.AddClient(TmpClientValues);
        //    comboBox_Client.SelectedIndex = index;
        //    LoadClientControlValues();
        //    ClientSettingValidation();
        //}

        //private void button_DeleteClient_Click(object sender, EventArgs e)
        //{
        //    if (comboBox_Client.Items.Count > 0)
        //    {
        //        if (MessageBox.Show("Вся информация о клиенте будет безвозвратно удалена, Вы действительно хотите продолжить?", "Удаление клиента", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //        {
        //            project.DeleteClient(comboBox_Client.SelectedItem as RemoteClient);
        //            ClientSettingValidation();
        //        }
        //    }
        //}

        //private void button_SaveClient_Click(object sender, EventArgs e)
        //{            
        //}

        //private void comboBox_Client_SelectionChangeCommitted(object sender, EventArgs e)
        //{
        //    LoadClientControlValues();
        //    LoadPinList();
        //    ClearPinAndCyclogrammTextBox();
        //}

        //private void comboBox_Client_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    LoadPinList();
        //    ClearPinAndCyclogrammTextBox();
        //}

        //private void button_AddPin_Click(object sender, EventArgs e)
        //{
        //}

        //private void dataGridView_PinList_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Delete)
        //    {
        //        (comboBox_Client.SelectedItem as RemoteClient).DeletePin((dataGridView_PinList.SelectedRows[0].DataBoundItem as Pin));
        //        LoadPinList();
        //        ClientSettingValidation();
        //    }
        //}

        //private void button_AddCyclogram_Click(object sender, EventArgs e)
        //{
        //   /* if (project.AbsoluteFolderPath != null)
        //    {

        //        Cyclogramm TmpCyclogramm = new Cyclogramm();
        //        TmpCyclogramm.Parent = (comboBox_Client.SelectedItem as Client);
        //        TmpCyclogramm.Converted = false;
        //        TmpCyclogramm.Saved = false;

        //        string tempstring = project.GetAbsoluteTEMPPath() + (comboBox_Client.SelectedItem as Client).RelativePath;
        //        if (!Directory.Exists(tempstring))
        //        {
        //            Directory.CreateDirectory(tempstring);
        //        }

        //        string OutputFileName = tempstring + "\\Data.cyc";

        //        FormConvertion progressForm = new FormConvertion();
        //        progressForm.InputFile = textBox_InputCyclogramm.Text;
        //        progressForm.OutputFile = OutputFileName;
        //        switch (progressForm.ShowDialog())
        //        {
        //            case DialogResult.Yes:
        //                FileInfo fi = new FileInfo(OutputFileName);
        //                TmpCyclogramm.FileSize = (int)Math.Ceiling((double)fi.Length / 1024D);
        //                (comboBox_Client.SelectedItem as Client).Cyclogramm = TmpCyclogramm;
        //                (comboBox_Client.SelectedItem as Client).Cyclogramm.Converted = true;
        //                break;
        //            case DialogResult.No:
        //                MessageBox.Show("Ошибка при конвертировании циклограммы, попробуйте снова. Данная циклограмма будет удалена из программы.");
        //                break;
        //            default:
        //                break;
        //        }

        //        ClearPinAndCyclogrammTextBox();
        //        CyclogrammControlValidation();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Необходимо сохранить проект");
        //    }*/
        //}

        //private void button_FindInputCyclogram_Click(object sender, EventArgs e)
        //{
        //    var ofd = new OpenFileDialog();
        //    ofd.Filter = "CSV files|*.csv";

        //    if (ofd.ShowDialog() == DialogResult.OK)
        //    {
        //        textBox_InputCyclogramm.Text = ofd.FileName;
        //    }
        //}        

        //private void FillIPAddresses()
        //{
        //    comboBox_IP.Items.Clear();

        //    comboBox_IP.Items.AddRange(ProjectController.NetworkController.AddressSettings.ToArray());
        //    if(ProjectController.NetworkController.CurrentAddressSetting != null) {
        //        comboBox_IP.SelectedItem = ProjectController.NetworkController.CurrentAddressSetting;
        //    }
        //    labelBroadcastAddress.Text = ProjectController.NetworkController.BroadcastIPAddress.ToString();
        //}

        //private void button_RefreshIPList_Click(object sender, EventArgs e)
        //{
        //    FillIPAddresses();
        //}

        //#endregion

        //#region Client TextBox Validation methods

        //private void textBox_ClientNumber_TextChanged(object sender, EventArgs e)
        //{
        //    ClientSettingValidation();
        //    (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        //}

        //private void textBox_ClientName_TextChanged(object sender, EventArgs e)
        //{
        //    ClientSettingValidation();
        //    (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        //}

        //private void textBox_ClientSSID_TextChanged(object sender, EventArgs e)
        //{
        //    ClientSettingValidation();
        //    (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        //}

        //private void textBox_ClientPassowordWifi_TextChanged(object sender, EventArgs e)
        //{
        //    ClientSettingValidation();
        //    (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        //}

        //private void textBox_ClientUDPPort_TextChanged(object sender, EventArgs e)
        //{
        //    ClientSettingValidation();
        //    (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        //}

        //private void textBox_ClientLEDCount_TextChanged(object sender, EventArgs e)
        //{
        //    ClientSettingValidation();
        //    (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        //}

        //private void textBox_PinNumber_TextChanged(object sender, EventArgs e)
        //{
        //    if (ServiceFunc.TextBoxValidation(sender as TextBox, TypesNumeric.t_byte_with_zero))
        //    {
        //        if (PinValidation((sender as TextBox).Text))
        //        {
        //            ServiceFunc.ColoringTextBox((sender as TextBox), true);
        //            button_AddPin.Enabled = true;
        //        }
        //        else
        //        {
        //            ServiceFunc.ColoringTextBox((sender as TextBox), false);
        //            button_AddPin.Enabled = false;
        //        }
        //    }
        //    else
        //    {
        //        ServiceFunc.ColoringTextBox((sender as TextBox), false);
        //        button_AddPin.Enabled = false;
        //    }
        //}

        //private bool PinValidation(string PinText)
        //{
        //    bool result = false;
        //    byte tmpByte;
        //    if (byte.TryParse(PinText, out tmpByte))
        //    {
        //        switch (tmpByte)
        //        {
        //            case 0:
        //                result = true;
        //                break;
        //            case 2:
        //                result = true;
        //                break;
        //            case 4:
        //                result = true;
        //                break;
        //            case 5:
        //                result = true;
        //                break;
        //            default:
        //                result = false;
        //                break;
        //        }
        //    }
        //    return result;
        //}

        //private void textBox_PinLEDCount_TextChanged(object sender, EventArgs e)
        //{
        //    ServiceFunc.TextBoxValidation(sender as TextBox, TypesNumeric.t_uint16);
        //}

        //private void textBox_CyclogrammName_TextChanged(object sender, EventArgs e)
        //{
        //    CyclogrammControlValidation();
        //    (sender as TextBox).Select((sender as TextBox).TextLength, 0);
        //}

        //private void textBox_InputCyclogramm_TextChanged(object sender, EventArgs e)
        //{
        //    CyclogrammControlValidation();
        //}

        /// <summary>
        /// Проверяет правильность введенной информации в контролы клиента, менят цвет контрола и активирует деактивирует кнопки
        /// в зависимости от правильности и полноты информации
        /// </summary>
        //private void ClientSettingValidation()
        //{
        //    byte CurrentNumber = 0;
        //    string CurrentName = "";

        //    bool Valid = true;
        //    if (!ServiceFunc.ColoringTextBox(textBox_ClientNumber, ServiceFunc.TextBoxValidation(textBox_ClientNumber, TypesNumeric.t_byte)))
        //    {
        //        Valid = false;
        //    }
        //    else
        //    {
        //        CurrentNumber = byte.Parse(textBox_ClientNumber.Text);
        //    }
        //    if (!ServiceFunc.ColoringTextBox(textBox_ClientName, ServiceFunc.TextBoxValidation(textBox_ClientName, TypesString.EnglishRusianDigitUnder)))
        //    {
        //        Valid = false;
        //    }
        //    else
        //    {
        //        CurrentName = textBox_ClientName.Text;
        //    }
        //    if (!ServiceFunc.ColoringTextBox(textBox_ClientSSID, ServiceFunc.TextBoxValidation(textBox_ClientSSID, TypesString.EnglishDigitUnderDefis)))
        //    {
        //        Valid = false;
        //    }
        //    if (!ServiceFunc.ColoringTextBox(textBox_ClientPasswordWifi, ServiceFunc.TextBoxValidation(textBox_ClientPasswordWifi, TypesString.AllWithoutSpace)))
        //    {
        //        Valid = false;
        //    }
        //    if (!ServiceFunc.ColoringTextBox(textBox_ClientUDPPort, ServiceFunc.TextBoxValidation(textBox_ClientUDPPort, TypesNumeric.t_uint16)))
        //    {
        //        Valid = false;
        //    }
        //    if (!ServiceFunc.ColoringTextBox(textBox_ClientLEDCount, ServiceFunc.TextBoxValidation(textBox_ClientLEDCount, TypesNumeric.t_uint16)))
        //    {
        //        Valid = false;
        //    }

        //    button_AddClient.Enabled = Valid;
        //    button_SaveClient.Enabled = Valid;

        //    if (CurrentNumber == 0)
        //    {
        //        ServiceFunc.ColoringTextBox(textBox_ClientNumber, false);
        //        button_AddClient.Enabled = false;
        //        button_SaveClient.Enabled = false;
        //    }
        //    else
        //    {
        //        if (project.ClientList.Exists(x => x.Number == CurrentNumber))
        //        {
        //            ServiceFunc.ColoringTextBox(textBox_ClientNumber, false);
        //            button_AddClient.Enabled = false;
        //        }
        //        else
        //        {
        //            button_SaveClient.Enabled = false;
        //        }
        //    }
        //    if (CurrentName == "")
        //    {
        //        ServiceFunc.ColoringTextBox(textBox_ClientName, false);
        //        button_AddClient.Enabled = false;
        //        button_SaveClient.Enabled = false;
        //    }
        //    else
        //    {
        //        if (project.ClientList.Exists(x => x.Name == CurrentName))
        //        {
        //            ServiceFunc.ColoringTextBox(textBox_ClientName, false);
        //            button_AddClient.Enabled = false;
        //            if (project.ClientList.Exists(x => x.Number == CurrentNumber))
        //            {
        //                button_SaveClient.Enabled = true;
        //            }
        //            else
        //            {
        //                button_SaveClient.Enabled = false;
        //            }
        //        }
        //        if (project.DeletedClientList != null && project.DeletedClientList.Exists(x => x.Name == CurrentName))
        //        {
        //            ServiceFunc.ColoringTextBox(textBox_ClientName, false);
        //            button_SaveClient.Enabled = false;
        //        }
        //    }
        //    /*
        //    if (project.ClientList.Count > 0)
        //    {
        //        Client CurrentClient = (comboBox_Client.SelectedItem as Client);
        //        CurrentClient.PinListIsLock = false;
        //        panel_PinList.Enabled = true;
        //        if ((project.ClientList.Count > 0) && (comboBox_Client.Items.Count > 0) && (!CurrentClient.PinListIsLock))
        //        {
        //            panel_PinList.Enabled = true;
        //        }
        //        ClientSaveImage.Visible = !CurrentClient.Saved;
        //        if (CurrentClient.PinList.Count > 0)
        //        {
        //            int TmpSum = CurrentClient.PinList.Sum(x => x.LEDCount);
        //            textBox_LEDCountCheck.Text = TmpSum.ToString() + "/" + CurrentClient.LEDCount;
        //            if (TmpSum == int.Parse(CurrentClient.LEDCount))
        //            {
        //                textBox_LEDCountCheck.BackColor = ServiceFunc.AcceptColor;
        //                groupBox_Cyclogramm.Enabled = true;
        //                CyclogrammControlValidation();
        //            }
        //            else
        //            {
        //                textBox_LEDCountCheck.BackColor = ServiceFunc.RefuseColor;
        //                groupBox_Cyclogramm.Enabled = true; //необходимо False для проверки по пинам
        //                CyclogrammControlValidation();
        //            }
        //        }
        //        else
        //        {
        //            if (CurrentClient.LEDCount == "")
        //            {
        //                textBox_LEDCountCheck.Text = "0/0";
        //            }
        //            textBox_LEDCountCheck.Text = "0/" + CurrentClient.LEDCount;
        //            textBox_LEDCountCheck.BackColor = ServiceFunc.RefuseColor;
        //            groupBox_Cyclogramm.Enabled = true; //необходимо False для проверки по пинам
        //            CyclogrammControlValidation();
        //        }

        //        button_DeleteClient.Enabled = true;
        //    }
        //    else
        //    {
        //        button_DeleteClient.Enabled = false;
        //    }
        //    */

        //    if (comboBox_RemovableDrive.Items.Count > 0)
        //    {
        //        button_LoadToSD.Enabled = project.Saved;
        //    }
        //    else
        //    {
        //        button_LoadToSD.Enabled = false;
        //        UpdateRemovableDrives();
        //    }

        //}

        /// <summary>
        /// Проверяет правильность заполнения контролов циклограмм
        /// </summary>
        //private void CyclogrammControlValidation()
        //{
        //    bool Valid = true;

        //    if (comboBox_Client.Items.Count > 0)
        //    {
        //        if ((comboBox_Client.SelectedItem as RemoteClient).Cyclogramm != null)
        //        {
        //            label_CyclogrammStatus.Text = "Связанная циклограмма: \"Data.cyc\" Размер: \"" + (comboBox_Client.SelectedItem as RemoteClient).Cyclogramm.FileSize.ToString() + " Кб\"";
        //            label_CyclogrammStatus.ForeColor = Color.DarkGreen;
        //        }
        //        else
        //        {
        //            label_CyclogrammStatus.Text = "Нет связанной циклограммы";
        //            label_CyclogrammStatus.ForeColor = Color.DarkRed;
        //        }
        //    }

        //    if (!ServiceFunc.ColoringTextBox(textBox_InputCyclogramm, File.Exists(textBox_InputCyclogramm.Text)))
        //    {
        //        Valid = false;
        //    }

        //    button_AddCyclogramm.Enabled = Valid;
        //}

        //#endregion

        //#region Service Form Methods

        /// <summary>
        /// Действие при старте проекта (Открытии или Создании)
        /// </summary>
        //private void OnProjectStart()
        //{
        //    UpdateRemovableDrives();
        //    try
        //    {
        //        Directory.Delete(project.GetAbsoluteTEMPPath(), true);
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    FillIPAddresses();

        //    if (project.Timer == null)
        //    {
        //        project.Timer = new System.Windows.Forms.Timer();
        //    }

        //    project.Timer.Interval = 100;
        //    project.Timer.Tick += Timer_Tick;
        //    project.Timer.Enabled = true;

        //    project.OnActiveThreadsChange += Project_OnActiveThreadsChange;
        //    project.OnChangeClientList += LoadDataSourceClient;
        //    project.OnSave += Project_OnSave;
        //    ProjectController.Server.OnSendNumberPlate += Server_OnSendNumberPlate;
        //    ProjectController.Server.OnStatusChange += OnChangeServerStatus;
        //    ProjectController.Server.OnServerIPChange += OnChangeServerIP;
        //    ProjectController.Server.Initialize(
        //        ProjectController.NetworkController.CurrentAddressSetting.IPAddress,
        //        ProjectController.NetworkController.Port, 
        //        ProjectController.NetworkController.BroadcastIPAddress);
        //    ProjectController.Server.StartReceiving();
        //    LoadDataSourceClient();
        //    LoadControlValues();
        //    ShowServerStatus();
        //    panel_Status.Visible = true;
        //    tabControl1.Enabled = true;
        //    Project_OnSave();
        //}

        /// <summary>
        /// Получает список дисков из системы и добавляет съемные диски в раскрывающийся список
        /// </summary>
        //private void UpdateRemovableDrives()
        //{
        //    var drives = ServiceFunctions.GetRemovableDrives();
        //    comboBox_RemovableDrive.Items.Clear();
        //    comboBox_RemovableDrive.Items.AddRange(drives.ToArray());
        //    if(drives.Any()) {
        //        comboBox_RemovableDrive.SelectedIndex = 0;
        //    }
        //}

        //private void Client_OnChangeStatus()
        //{
        //    dataGridView_Clients.Refresh();
        //}

        //private void Client_OnChange()
        //{
        //    LoadDataSourceClient();
        //}

        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    if (project != null)
        //    {
        //        if (project.ClientList != null)
        //        {

        //            for (int i = 0; i < project.ClientList.Count; i++)
        //            {
        //                if (project.ClientList[i].OnlineTime < 5000000)
        //                {
        //                    project.ClientList[i].OnlineTime += (uint)(project.Timer.Interval);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void Server_OnSendNumberPlate(object sender, PlateInfoEventArgs e)
        //{
        //    try
        //    {
        //        BeginInvoke(
        //            new Action(delegate ()
        //                {
        //                    RemoteClient client = project.ClientList.FirstOrDefault(x => x.Number == e.ClientNumber);
        //                    if (client != null)
        //                    {
        //                        if (
        //                         не ожидается ответа от клиента о воспроизведнии
        //                        !client.WaitPlayingStatus 
        //                         если не онлайн
        //                        && !client.IsOnline 
        //                         состояние клиента - ожидание воспроизведения
        //                        && e.ClientState == ClientState.Wait 
        //                         статус плеера - вопсроизведение
        //                        && rlcPlayer1.PlaybackStateStr == PlaybackState.Playing)
        //                        {
        //                            ProjectController.Server.Send_PlayFrom_12(rlcPlayer1.CurrentTime, client.Number, client.IPAddress);
        //                            client.WaitPlayingStatus = true;
        //                        }
        //                        if (e.ClientState == ClientState.Play || e.ClientState == ClientState.Pause)
        //                        {
        //                            client.WaitPlayingStatus = false;
        //                        }

        //                        client.OnlineTime = 0; // Сбрасывание времени ожидания клиента, если время меньше определенного числа то клиент онлайн
                                
        //                        if (client.IPAddress != e.IpEndPoint.Address)
        //                        {
        //                            client.IPAddress = e.IpEndPoint.Address; // Установка IP адреса клиента, если указанный в клиенте не совпадает с адресом в пакете
        //                        }
        //                    }
        //                }
        //            )
        //        );
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        //private void Project_OnSave()
        //{
        //    ProjectSaveImage.Visible = !project.Saved;
        //}

        /// <summary>
        /// Если есть активные потоки конвертирования циклограмм, блокирует некоторые елементы формы
        /// </summary>
        //private void Project_OnActiveThreadsChange(bool HaveTreads)
        //{
        //    if (!HaveTreads)
        //    {
        //        ClientSettingValidation();
        //        comboBox_Client.Enabled = true;
        //    }
        //    else
        //    {
        //        button_AddClient.Enabled = false;
        //        button_DeleteClient.Enabled = false;
        //        button_SaveClient.Enabled = false;
        //        comboBox_Client.Enabled = false;
        //    }
        //}

        //private void OnChangeServerStatus(object sender, EventArgs e)
        //{
        //    ShowServerStatus();
        //}

        /// <summary>
        /// Отображает статус сервера на форме в строке информации
        /// </summary>
        //private void ShowServerStatus()
        //{
        //    if (project != null)
        //    {
        //        if (ProjectController.Server.IsRun)
        //        {
        //            label_ServerStatus.Text = "Запущен (" + ProjectController.NetworkController.Port.ToString() + ")";
        //            label_ServerStatus.ForeColor = Color.Green;
        //        }
        //        else
        //        {
        //            label_ServerStatus.Text = "Не запущен";
        //            label_ServerStatus.ForeColor = Color.Red;
        //        }
        //        ShowServerIP();
        //    }
        //}
        //private void OnChangeServerIP(object sender, EventArgs e)
        //{
        //    ShowServerIP();
        //}
        /// <summary>
        /// Отображает текущий IP адрес сервера используемый для передачи данных
        /// </summary>
        //private void ShowServerIP()
        //{
        //    bool visible = false;
        //    if (project != null)
        //    {
        //        label_ServerIP.Text = ProjectController.NetworkController.CurrentAddressSetting.IPAddress.ToString();
        //        visible = true;
        //    }
        //    label_ServerIP.Visible = visible;
        //}

        /// <summary>
        /// Загружает значения в контролы из текущего проекта
        /// </summary>
        //private void LoadControlValues()
        //{
        //    label_CurrentKey.Text = project.Key.ToString();
        //    textBox_localPort.Text = ProjectController.NetworkController.Port.ToString();
        //    LoadClientControlValues();
        //}

        /// <summary>
        /// Загружает значения в контролы настроек клиента из текущего проекта
        /// </summary>
        //private void LoadClientControlValues()
        //{
        //    if (project.ClientList.Count > 0)
        //    {
        //        textBox_ClientNumber.Text = (comboBox_Client.SelectedItem as RemoteClient).Number.ToString();
        //        textBox_ClientName.Text = (comboBox_Client.SelectedItem as RemoteClient).Name;
        //    }
        //    ClientSettingValidation();
        //}

        /// <summary>
        /// Загружает данные из списка клиентов на элементы отображения
        /// </summary>
        //private void LoadDataSourceClient()
        //{
        //    int TmpIndex = 0;
        //    if (comboBox_Client.SelectedIndex > 0)
        //    {
        //        TmpIndex = comboBox_Client.SelectedIndex;
        //    }
        //    BindingSource source = new BindingSource();
        //    source.DataSource = project.ClientList;
        //    comboBox_Client.DataSource = source;
        //    dataGridView_Clients.DataSource = source;
        //    dataGridView_Clients.Refresh();
        //    if (project.ClientList.Count > 0)
        //    {
        //        if (TmpIndex >= project.ClientList.Count)
        //        {
        //            comboBox_Client.SelectedIndex = project.ClientList.Count - 1;
        //        }
        //        else
        //        {
        //            comboBox_Client.SelectedIndex = TmpIndex;
        //        }
        //        LoadClientControlValues();
        //    }

        //}

        //public string GetPropertyName<TNotifyPropertyType>(Expression<Func<TNotifyPropertyType, object>> typeExpr)
        //{
        //    if(typeExpr == null) {
        //        throw new ArgumentNullException("selectorExpression");
        //    }
        //    MemberExpression body = typeExpr.Body as MemberExpression;
        //    if(body == null) {
        //        throw new ArgumentException("The body must be a member expression");
        //    }
        //    return body.Member.Name;
        //}

        /// <summary>
        /// Загружает список пинов текущего клиента на форму
        /// </summary>
        //private void LoadPinList()
        //{
        //    if ((project.ClientList.Count > 0) && (comboBox_Client.Items.Count > 0))
        //    {
        //        (comboBox_Client.SelectedItem as RemoteClient).PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
        //        {
        //            if(e.PropertyName == GetPropertyName<RemoteClient>(x => x.ObservablePins)) {

        //            }
        //            BindingSource source = new BindingSource();
        //            source.DataSource = (comboBox_Client.SelectedItem as RemoteClient).PinList;
        //            dataGridView_PinList.DataSource = source;
        //            ClientSettingValidation();
        //        };

        //        BindingSource source2 = new BindingSource();
        //        source2.DataSource = (comboBox_Client.SelectedItem as RemoteClient).PinList;
        //        dataGridView_PinList.DataSource = source2;
        //    }
        //}

        /// <summary>
        /// Очищает елементы формы и данные использемого проекта
        /// </summary>
        //private void ClearProject()
        //{
        //    textBox_ClientNumber.Text = "";
        //    textBox_ClientName.Text = "";
        //    textBox_ClientSSID.Text = "";
        //    textBox_ClientPasswordWifi.Text = "";
        //    textBox_ClientUDPPort.Text = "";
        //    textBox_ClientLEDCount.Text = "";
        //    textBox_InputCyclogramm.Text = "";
        //    textBox_LEDCountCheck.Text = "";
        //    textBox_localPort.Text = "";
        //    textBox_PinLEDCount.Text = "";
        //    textBox_PinNumber.Text = "";
        //    label_CyclogrammStatus.Text = "";
        //    rlcPlayer1.Reset();

        //    if (project != null)
        //    {
        //        if (ProjectController.Server != null)
        //        {
        //            if (ProjectController.Server.IsRun)
        //            {
        //                ProjectController.Server.StopReceiving();
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Очищает контролы пинов и циклограмм
        /// </summary>
        //private void ClearPinAndCyclogrammTextBox()
        //{
        //    textBox_PinNumber.Text = "";
        //    textBox_PinLEDCount.Text = "";
        //    textBox_InputCyclogramm.Text = "";
        //}

        //#endregion

        //#region DragDrop methods

        //private void CyclogramDragDrop(object sender, DragEventArgs e)
        //{
        //    string[] objects = (string[])e.Data.GetData(DataFormats.FileDrop);
        //    textBox_InputCyclogramm.Text = objects[0];
        //    (sender as Control).DragDrop -= CyclogramDragDrop;
        //}

        //private void OnDragEnter(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(DataFormats.FileDrop) && ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move))
        //    {
        //        string[] objects = null;
        //        objects = (string[])e.Data.GetData(DataFormats.FileDrop);
        //        if ((objects.Length == 1) & (string.Equals(Path.GetExtension(objects[0]), ".csv", StringComparison.InvariantCultureIgnoreCase)))
        //        {
        //            e.Effect = DragDropEffects.Move;
        //            (sender as Control).DragDrop -= CyclogramDragDrop;
        //            (sender as Control).DragDrop += CyclogramDragDrop;
        //        }
        //        else
        //        {
        //            e.Effect = DragDropEffects.None;
        //        }
        //    }
        //}

        //#endregion

        //private void comboBox_AudioOutputs_SelectionChangeCommitted(object sender, EventArgs e)
        //{
        //    if (!(rlcPlayer1.PlaybackStateStr == PlaybackState.Stopped))
        //    {
        //        if (MessageBox.Show("В данный момент производится воспроизведение, прервать воспроизведение и сменить устройство?", "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //        {
        //            button_Stop.PerformClick();
        //            rlcPlayer1.Device = (comboBox_AudioOutputs.SelectedItem as MMDevice);
        //        }
        //        else
        //        {
        //            comboBox_AudioOutputs.SelectedItem = rlcPlayer1.Device;
        //        }
        //    }
        //}

        //private void ComboBoxAudioOutputs_Fill()
        //{
        //    var selectedObject = comboBox_AudioOutputs.SelectedItem;
        //    string deviceId = "";
        //    MMDevice selectedDevice;
        //    try
        //    {
        //        selectedDevice = selectedObject as MMDevice;
        //        if (selectedDevice.DeviceState == DeviceState.Active)
        //        {
        //            deviceId = selectedDevice.DeviceID;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    rlcPlayer1.ReloadDevices();
        //    var listDevices = rlcPlayer1._devices.Where(x => x.DeviceState == DeviceState.Active).ToList();
        //    var currentItem = listDevices.FirstOrDefault(x => x.DeviceID == deviceId);            

        //    comboBox_AudioOutputs.DataSource = listDevices;

        //    if (currentItem != null)
        //    {
        //        comboBox_AudioOutputs.SelectedItem = currentItem;
        //    }
        //    comboBox_AudioOutputs.DisplayMember = "FriendlyName";
        //    comboBox_AudioOutputs.ValueMember = "DeviceID";
        //}

        //private void RemoteLEDControl_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    if (project != null)
        //    {
        //        string message = "";

        //        message += "Все не сохраненные данные будут утеряны...\n";
        //        message += "ЕСЛИ ЗАПУЩЕН ПРОЕКТ, ВСЕ КЛИЕНТЫ БУДУТ ОСТАНОВЛЕНЫ!!! \n";
        //        message += "Продолжить?\n";

        //        if (MessageBox.Show(message, "Закрытие программы", MessageBoxButtons.YesNo) == DialogResult.No)
        //        {
        //            e.Cancel = true;
        //        }
        //        else
        //        {
        //            if (ProjectController.Server != null)
        //            {
        //                if (ProjectController.Server.IsRun)
        //                {
        //                    ProjectController.Server.Send_StopAll_2();
        //                    ProjectController.Server.StopReceiving();
        //                    while (ProjectController.Server.IsRun) { }
        //                }
        //            }
        //        }

        //    }
        //}

        //private void maskedTextBox_SetTime_TextChanged(object sender, EventArgs e)
        //{
        //    if ((sender as MaskedTextBox).Text.Length == 5 && !(sender as MaskedTextBox).Text.Contains(" "))
        //    {
        //        int minutes;
        //        int seconds;
        //        if (int.TryParse((sender as MaskedTextBox).Text.Substring(0, 2), out minutes) &&
        //            int.TryParse((sender as MaskedTextBox).Text.Substring(3, 2), out seconds))
        //        {
        //            if (seconds > 59)
        //            {
        //                (sender as MaskedTextBox).Text = (sender as MaskedTextBox).Text.Substring(0, 3) + "59";
        //                seconds = 59;
        //            }
        //            (sender as MaskedTextBox).ForeColor = Color.Green;
        //            button_PlayFrom.Enabled = true;
        //            return;
        //        }
        //    }

        //    (sender as MaskedTextBox).ForeColor = Color.Red;
        //    button_PlayFrom.Enabled = false;

        //}

        //private void maskedTextBox_SetTime_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter && button_PlayFrom.Enabled)
        //    {
        //        button_PlayFrom.PerformClick();
        //    }
        //}

        //private void TempOpen(string path)
        //{
        //    XMLSaver xml = new XMLSaver();
        //    xml.Fields = new RemoteControlProject(1);
        //    try
        //    {
        //        xml.ReadXml(path);
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Ошибка открытия файла, возможно файл не предназначен для этой программы.");
        //        return;
        //    }
        //    string FileName = Path.GetFileNameWithoutExtension(path);
        //    string FolderPath = Path.GetDirectoryName(path);
        //    string FolderName = Path.GetFileName(FolderPath);

        //    if (FolderName != FileName)
        //    {
        //        if (MessageBox.Show("Проект должен находиться в папке проекта с тем же названием, будет создана папка " +
        //            FileName + ", и в нее перемещен файл проекта и приложенные файлы. Продолжить открытие?",
        //            "Предупреждение", MessageBoxButtons.OKCancel) == DialogResult.OK)
        //        {
        //            if (Directory.Exists(FolderPath + "\\" + FileName))
        //            {
        //                MessageBox.Show("Папка \"" + FileName + "\" уже существует. Невозможно продолжить", "Ошибка сохранения", MessageBoxButtons.OK);
        //                return;
        //            }
        //            else
        //            {
        //                Directory.CreateDirectory(FolderPath + "\\" + FileName);
        //                string FilePath = FolderPath + "\\" + FileName + "\\" + FileName + ".xml";
        //                File.Move(path, FilePath);
        //                Directory.Move(FolderPath + "\\" + xml.Fields.ClientsFolderName, FolderPath + "\\" + FileName + "\\" + xml.Fields.ClientsFolderName);
        //                xml = new XMLSaver();
        //                xml.Fields = new RemoteControlProject(1);
        //                try
        //                {
        //                    xml.ReadXml(FilePath);
        //                }
        //                catch (Exception)
        //                {
        //                    MessageBox.Show("Ошибка открытия файла, возможно файл не предназначен для этой программы.");
        //                    return;
        //                }

        //                ClearProject();
        //                ProjectController.CurrentProject = xml.Fields;
        //                project.RuningThreadsList = new List<Thread>();
        //                project.AbsoluteFilePath = FilePath;
        //                if (ProjectController.Server != null)
        //                {
        //                    if (ProjectController.Server.ProjectKey != project.Key)
        //                    {
        //                        ProjectController.Server.ProjectKey = project.Key;
        //                    }
        //                }
        //                project.Saved = true;
        //                ToolStripMenuItem_SaveProject.Enabled = true;
        //                ToolStripMenuItem_SaveAsProject.Enabled = true;
        //                OnProjectStart();
        //                return;
        //            }

        //        }
        //        else
        //        {
        //            return;
        //        }

        //    }

        //    xml = new XMLSaver();
        //    xml.Fields = new RemoteControlProject(1);
        //    try
        //    {
        //        xml.ReadXml(path);
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Ошибка открытия файла, возможно файл не предназначен для этой программы.");
        //        return;
        //    }

        //    ClearProject();
        //    ProjectController.CurrentProject = xml.Fields;
        //    project.RuningThreadsList = new List<Thread>();
        //    project.AbsoluteFilePath = path;
        //    if (ProjectController.Server != null)
        //    {
        //        if (ProjectController.Server.ProjectKey != project.Key)
        //        {
        //            ProjectController.Server.ProjectKey = project.Key;
        //        }
        //    }
        //    project.Saved = true;
        //    ToolStripMenuItem_SaveProject.Enabled = true;
        //    ToolStripMenuItem_SaveAsProject.Enabled = true;
        //    OnProjectStart();
        //}

        //private void richTextBox1_DoubleClick(object sender, EventArgs e)
        //{
        //    (sender as RichTextBox).Clear();
        //}

        //private void button2_Click_1(object sender, EventArgs e)
        //{
        //    if (comboBox_RemovableDrive.Items.Count == 0)
        //    {
        //        UpdateRemovableDrives();
        //        return;
        //    }
        //    else
        //    {
        //        if ((comboBox_RemovableDrive.SelectedItem as DriveInfo).IsReady)
        //        {
        //            List<FileInfo> ListFiles = new List<FileInfo>();
        //            List<FileInfo> ErrorFiles = new List<FileInfo>();
        //            List<FileInfo> NotExistFiles = new List<FileInfo>();
        //            RemoteClient tmpClient = (comboBox_Client.SelectedItem as RemoteClient);

        //             Файл настроек клиента
        //            FileInfo SetupFile = new FileInfo(tmpClient.Parent.AbsoluteFolderPath + tmpClient.RelativePath + "\\" + tmpClient.Parent.ClientsConfigFileName);
        //            FileInfo DestSetupFile = new FileInfo((comboBox_RemovableDrive.SelectedItem as DriveInfo).RootDirectory + SetupFile.Name);
        //            if (!SetupFile.Exists)
        //            {
        //                NotExistFiles.Add(SetupFile);
        //            }
        //            if (ServiceFunc.FileNotExistsOrHaveAccess(DestSetupFile))
        //            {
        //                ListFiles.Add(DestSetupFile);
        //            }
        //            else
        //            {
        //                ErrorFiles.Add(DestSetupFile);
        //            }
        //             Файл циклограммы

        //            FileInfo CyclogrammFile = new FileInfo(tmpClient.Parent.AbsoluteFolderPath + tmpClient.RelativePath + "\\Data.cyc");
        //            FileInfo DestCyclogrammFile = new FileInfo((comboBox_RemovableDrive.SelectedItem as DriveInfo).RootDirectory + "Data.cyc");

        //            if (!CyclogrammFile.Exists)
        //            {
        //                NotExistFiles.Add(CyclogrammFile);
        //            }

        //            if (ServiceFunc.FileNotExistsOrHaveAccess(DestCyclogrammFile))
        //            {
        //                ListFiles.Add(DestCyclogrammFile);
        //            }
        //            else
        //            {
        //                ErrorFiles.Add(DestCyclogrammFile);
        //            }


        //            if (ErrorFiles.Count > 0 || NotExistFiles.Count > 0)
        //            {
        //                string error = "";
        //                if (NotExistFiles.Count > 0)
        //                {
        //                    error += "Следущих необходимых файлов не существует:\n";
        //                    foreach (FileInfo file in NotExistFiles)
        //                    {
        //                        error += "(" + file.FullName + ")\n";
        //                    }
        //                    error += "\n";
        //                }

        //                if (ErrorFiles.Count > 0)
        //                {
        //                    error += "Ошибка замены следующих файлов на съемном носителе (нет доступа):\n";
        //                    foreach (FileInfo file in ErrorFiles)
        //                    {
        //                        error += "(" + file.FullName + ")\n";
        //                    }
        //                    error += "\n";
        //                }
        //                MessageBox.Show(error + "Перенос данных на съемный носитель невозможен.", "Ошибка переноса данных");
        //                return;
        //            }

        //             Вызывает стандартный диалог копирования файлов в каталоге
        //            FileSystem.CopyDirectory(
        //                tmpClient.Parent.AbsoluteFolderPath + tmpClient.RelativePath,
        //                (comboBox_RemovableDrive.SelectedItem as DriveInfo).RootDirectory.FullName,
        //                UIOption.AllDialogs,
        //                UICancelOption.DoNothing);
        //        }
        //        else
        //        {
        //            UpdateRemovableDrives();
        //            return;
        //        }
        //    }
        //}

        //private void button_TurnOffServer_Click(object sender, EventArgs e)
        //{
        //    if (ProjectController.Server.IsRun)
        //    {
        //        ProjectController.Server.StopReceiving();
        //        ResetPlayer();
        //    }
        //    else
        //    {
        //        try
        //        {
        //            ProjectController.Server.StartReceiving();
        //            ResetPlayer();
        //        }
        //        catch (Exception)
        //        {
        //            MessageBox.Show("Ошибка старта сервера. Возможно порт занят. Попробуйте использовать другой порт");
        //        }
        //    }
        //}

        //private void ResetPlayer()
        //{
        //    rlcPlayer1.Stop();
        //    rlcPlayer1.CurrentTime = TimeSpan.Zero;
        //}

        //private void button_Play_Click(object sender, EventArgs e)
        //{
        //    if (rlcPlayer1.TotalTime == TimeSpan.Zero)
        //    {
        //        return;
        //    }
        //    if (ProjectController.Server.IsRun)
        //    {
        //        if (rlcPlayer1.PlaybackStateStr == PlaybackState.Stopped || rlcPlayer1.PlaybackStateStr == PlaybackState.Paused)
        //        {
        //            rlcPlayer1.Play();
        //            ProjectController.Server.Send_PlayAll_1();
        //        }
        //    }
        //    else
        //    {
        //        rlcPlayer1.Play();
        //    }
        //}

        //private void button_Pause_Click(object sender, EventArgs e)
        //{
        //    if (rlcPlayer1.TotalTime == TimeSpan.Zero)
        //    {
        //        return;
        //    }
        //    if (ProjectController.Server.IsRun)
        //    {
        //        if (rlcPlayer1.PlaybackStateStr == PlaybackState.Playing)
        //        {
        //            ProjectController.Server.Send_PauseAll_6();
        //            Thread.Sleep(2);
        //            rlcPlayer1.Pause();
        //        }
        //    }
        //    else
        //    {
        //        rlcPlayer1.Pause();
        //    }
        //}

        //private void button_Stop_Click(object sender, EventArgs e)
        //{
        //    if (rlcPlayer1.TotalTime == TimeSpan.Zero)
        //    {
        //        return;
        //    }
        //    if (ProjectController.Server.IsRun)
        //    {
        //        if (rlcPlayer1.PlaybackStateStr == PlaybackState.Playing || rlcPlayer1.PlaybackStateStr == PlaybackState.Paused)
        //        {
        //            rlcPlayer1.Stop();
        //            rlcPlayer1.CurrentTime = TimeSpan.Zero;
        //            ProjectController.Server.Send_StopAll_2();
        //        }
        //    }
        //    else
        //    {
        //        rlcPlayer1.Stop();
        //        rlcPlayer1.CurrentTime = TimeSpan.Zero;
        //    }
        //}

        //private void button_Open_Click(object sender, EventArgs e)
        //{
        //    открыть
        //    OpenFileDialog od = new OpenFileDialog();
        //    od.InitialDirectory = Environment.CurrentDirectory;
        //    od.Filter = CodecFactory.SupportedFilesFilterEn;
        //    od.Multiselect = false;
        //    if (od.ShowDialog() == DialogResult.OK)
        //    {
        //        rlcPlayer1.InitializePlayer(od.FileName);
        //    }
        //}

        //private void button_PlayFrom_Click(object sender, EventArgs e)
        //{
        //    int minutes;
        //    int seconds;
        //    if (int.TryParse(maskedTextBox_SetTime.Text.Substring(0, 2), out minutes) &&
        //        int.TryParse(maskedTextBox_SetTime.Text.Substring(3, 2), out seconds))
        //    {
        //        double min = TimeSpan.FromMinutes((double)minutes).TotalMilliseconds;
        //        double sec = TimeSpan.FromSeconds((double)seconds).TotalMilliseconds;
        //        rlcPlayer1.CurrentTime = TimeSpan.FromMilliseconds(min + sec);
        //        if (ProjectController.Server.IsRun)
        //        {
        //            if (rlcPlayer1.PlaybackStateStr == PlaybackState.Stopped || rlcPlayer1.PlaybackStateStr == PlaybackState.Paused)
        //            {                        
        //                ProjectController.Server.Send_PlayFromAll_7(rlcPlayer1.CurrentTime);
        //                Thread.Sleep(2);
        //                rlcPlayer1.Play();
        //            }
        //        }
        //        else
        //        {
        //            rlcPlayer1.Play();
        //        }                
        //    }
        //}

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    UpdateRemovableDrives();
        //    ClientSettingValidation();
        //}

        //private void button_CopyClient_Click(object sender, EventArgs e)
        //{
        //    if (comboBox_Client.Items.Count > 0)
        //    {
        //        RemoteClient SelectedClient = (comboBox_Client.SelectedItem as RemoteClient);

        //        ClientValues cv = new ClientValues();
        //        for (int i = 1; i < 255; i++)
        //        {
        //            if (!project.ClientList.Exists(x => x.Number == i))
        //            {
        //                cv.Number = i.ToString();
        //                break;
        //            }
        //        }
        //        for (int i = 1; i < int.MaxValue; i++)
        //        {
        //            if (!project.ClientList.Exists(x => x.Name == (SelectedClient.Name + i.ToString())))
        //            {
        //                cv.Name = SelectedClient.Name + i.ToString();
        //                break;
        //            }
        //        }
        //        cv.WifiSSID = SelectedClient.WifiSSID;
        //        cv.WifiPassword = SelectedClient.WifiPassword;
        //        cv.UDPPort = SelectedClient.UDPPort;
        //        cv.LEDCount = SelectedClient.LEDCount;
        //        cv.RelativeFolderPath = "\\" + project.ClientsFolderName + "\\" + cv.Name;
        //        int index = project.AddClient(cv);
        //        if (index > 0)
        //        {
        //            foreach (Pin SourcePin in SelectedClient.PinList)
        //            {
        //                project.ClientList[index].AddPin(SourcePin.PinNumber.ToString(), SourcePin.LEDCount.ToString());
        //            }
        //        }
        //    }
        //}

        //private void volumeSlider2_VolumeChanged(object sender, EventArgs e)
        //{
        //    rlcPlayer1.Volume = (sender as VolumeSlider.VolumeSlider).Volume;
        //}

        //private void rlcPlayer1_OnMouseSetTime()
        //{
        //    if (rlcPlayer1.CurrentTime.Minutes >= 99)
        //    {
        //        return;
        //    }
        //    if (ProjectController.Server.IsRun)
        //    {
        //        maskedTextBox_SetTime.Text = String.Format("{0,2}:{1,2}", rlcPlayer1.CurrentTime.Minutes.ToString("D2"), rlcPlayer1.CurrentTime.Seconds.ToString("D2"));
        //        ProjectController.Server.Send_PlayFromAll_7(rlcPlayer1.CurrentTime);
        //        rlcPlayer1.Play();
        //    }
        //    else
        //    {
        //        rlcPlayer1.Play();
        //    }
        //}

        //private void rlcPlayer1_OnInitializedPlayer(string FilePath)
        //{
        //    FileInfo tmpFile = new FileInfo(FilePath);
        //    if (tmpFile.Exists)
        //    {
        //        if (project != null)
        //        {
        //            if (project.AudioFile != null)
        //            {
        //                if (project.AudioFile.FullName != tmpFile.FullName)
        //                {
        //                    project.AudioFile = tmpFile;
        //                }
        //            }
        //            else
        //            {
        //                project.AudioFile = tmpFile;
        //            }                   
        //        }
        //    }
        //}

        //private void comboBox_AudioOutputs_DropDown(object sender, EventArgs e)
        //{
        //    ComboBoxAudioOutputs_Fill();
        //}

        //private void buttonPlayerRestart_Click(object sender, EventArgs e)
        //{
        //    string openedFile = rlcPlayer1.CurrentAudioFile;
        //    if(MessageBox.Show("Будет остановлено воспроизведение и перезагружен плеер!", "Внимание!", MessageBoxButtons.OKCancel) == DialogResult.OK)
        //    {
        //        rlcPlayer1.Reset();
        //        if (!String.IsNullOrEmpty(openedFile))
        //        {
        //            rlcPlayer1.InitializePlayer(openedFile);
        //        }
        //    }
            
        //}

        //private void comboBox_IP_SelectionChangeCommitted(object sender, EventArgs e)
        //{
        //    if(ProjectController.NetworkController.IsLocked) {
        //        comboBox_IP.SelectedItem = ProjectController.NetworkController.CurrentAddressSetting;
        //    } else {
        //        ProjectController.NetworkController.CurrentAddressSetting = (NetworkAddressSetting)comboBox_IP.SelectedItem;

        //        labelBroadcastAddress.Text = ProjectController.NetworkController.CurrentAddressSetting.IPAddress.ToString();
        //    }
        //}
    }
}