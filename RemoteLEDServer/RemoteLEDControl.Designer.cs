namespace RemoteLEDServer
{
    partial class RemoteLEDControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteLEDControl));
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.ToolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_CreateProject = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_OpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_SaveProject = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_SaveAsProject = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_Status = new System.Windows.Forms.FlowLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            this.label_CurrentKey = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label_ServerStatus = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label_ServerIP = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel_Separator = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.groupBox_Cyclogramm = new System.Windows.Forms.GroupBox();
            this.label_CyclogrammStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_InputCyclogramm = new System.Windows.Forms.TextBox();
            this.button_FindInputCyclogramm = new System.Windows.Forms.Button();
            this.button_AddCyclogramm = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_ClientNumber = new System.Windows.Forms.TextBox();
            this.panel_PinList = new System.Windows.Forms.Panel();
            this.textBox_PinNumber = new System.Windows.Forms.TextBox();
            this.textBox_LEDCountCheck = new System.Windows.Forms.TextBox();
            this.dataGridView_PinList = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox_PinLEDCount = new System.Windows.Forms.TextBox();
            this.button_AddPin = new System.Windows.Forms.Button();
            this.textBox_ClientUDPPort = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.textBox_ClientName = new System.Windows.Forms.TextBox();
            this.button_SaveClient = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.textBox_ClientPasswordWifi = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textBox_ClientSSID = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.textBox_ClientLEDCount = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ClientSaveImage = new System.Windows.Forms.PictureBox();
            this.button_CopyClient = new System.Windows.Forms.Button();
            this.button_DeleteClient = new System.Windows.Forms.Button();
            this.label36 = new System.Windows.Forms.Label();
            this.comboBox_Client = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox_RemovableDrive = new System.Windows.Forms.ComboBox();
            this.button_AddClient = new System.Windows.Forms.Button();
            this.button_LoadToSD = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox_Audio = new System.Windows.Forms.GroupBox();
            this.comboBox_AudioOutputs = new System.Windows.Forms.ComboBox();
            this.label_audioDevice = new System.Windows.Forms.Label();
            this.groupBox_server = new System.Windows.Forms.GroupBox();
            this.label40 = new System.Windows.Forms.Label();
            this.button_RefreshIPList = new System.Windows.Forms.Button();
            this.textBox_localPort = new System.Windows.Forms.TextBox();
            this.button_TurnOffServer = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_IP = new System.Windows.Forms.ComboBox();
            this.button_SaveServerSetting = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel_Clients = new System.Windows.Forms.Panel();
            this.dataGridView_Clients = new System.Windows.Forms.DataGridView();
            this.Column_Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel_Player = new System.Windows.Forms.Panel();
            this.rlcPlayer1 = new RLCPlayer.RLCPlayer();
            this.panel_PlayerControls = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.volumeSlider2 = new VolumeSlider.VolumeSlider();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel10 = new System.Windows.Forms.Panel();
            this.maskedTextBox_SetTime = new System.Windows.Forms.MaskedTextBox();
            this.button_Open = new System.Windows.Forms.Button();
            this.button_PlayFrom = new System.Windows.Forms.Button();
            this.button_Play = new System.Windows.Forms.Button();
            this.button_Pause = new System.Windows.Forms.Button();
            this.button_Stop = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ProjectSaveImage = new System.Windows.Forms.PictureBox();
            this.cyclogrammBindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.cyclogrammBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cyclogrammBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.MainMenu.SuspendLayout();
            this.panel_Status.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.groupBox_Cyclogramm.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel_PinList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_PinList)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClientSaveImage)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox_Audio.SuspendLayout();
            this.groupBox_server.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel_Clients.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Clients)).BeginInit();
            this.panel_Player.SuspendLayout();
            this.rlcPlayer1.SuspendLayout();
            this.panel_PlayerControls.SuspendLayout();
            this.panel11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel10.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProjectSaveImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cyclogrammBindingSource2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cyclogrammBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cyclogrammBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.BackColor = System.Drawing.SystemColors.Control;
            this.MainMenu.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemFile});
            this.MainMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.MainMenu.Location = new System.Drawing.Point(1, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.MainMenu.Size = new System.Drawing.Size(987, 24);
            this.MainMenu.TabIndex = 4;
            this.MainMenu.Text = "menuStrip1";
            // 
            // ToolStripMenuItemFile
            // 
            this.ToolStripMenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_CreateProject,
            this.ToolStripMenuItem_OpenProject,
            this.ToolStripMenuItem_SaveProject,
            this.ToolStripMenuItem_SaveAsProject});
            this.ToolStripMenuItemFile.Name = "ToolStripMenuItemFile";
            this.ToolStripMenuItemFile.Size = new System.Drawing.Size(48, 20);
            this.ToolStripMenuItemFile.Text = "Файл";
            // 
            // ToolStripMenuItem_CreateProject
            // 
            this.ToolStripMenuItem_CreateProject.Name = "ToolStripMenuItem_CreateProject";
            this.ToolStripMenuItem_CreateProject.Size = new System.Drawing.Size(156, 22);
            this.ToolStripMenuItem_CreateProject.Text = "Создать";
            this.ToolStripMenuItem_CreateProject.Click += new System.EventHandler(this.ToolStripMenuItem_CreateProject_Click);
            // 
            // ToolStripMenuItem_OpenProject
            // 
            this.ToolStripMenuItem_OpenProject.Name = "ToolStripMenuItem_OpenProject";
            this.ToolStripMenuItem_OpenProject.Size = new System.Drawing.Size(156, 22);
            this.ToolStripMenuItem_OpenProject.Text = "Открыть";
            this.ToolStripMenuItem_OpenProject.Click += new System.EventHandler(this.ToolStripMenuItem_OpenProject_Click);
            // 
            // ToolStripMenuItem_SaveProject
            // 
            this.ToolStripMenuItem_SaveProject.Enabled = false;
            this.ToolStripMenuItem_SaveProject.Name = "ToolStripMenuItem_SaveProject";
            this.ToolStripMenuItem_SaveProject.Size = new System.Drawing.Size(156, 22);
            this.ToolStripMenuItem_SaveProject.Text = "Сохранить";
            this.ToolStripMenuItem_SaveProject.Click += new System.EventHandler(this.ToolStripMenuItem_SaveProject_Click);
            // 
            // ToolStripMenuItem_SaveAsProject
            // 
            this.ToolStripMenuItem_SaveAsProject.Enabled = false;
            this.ToolStripMenuItem_SaveAsProject.Name = "ToolStripMenuItem_SaveAsProject";
            this.ToolStripMenuItem_SaveAsProject.Size = new System.Drawing.Size(156, 22);
            this.ToolStripMenuItem_SaveAsProject.Text = "Сохранить как";
            this.ToolStripMenuItem_SaveAsProject.Click += new System.EventHandler(this.ToolStripMenuItem_SaveAsProject_Click);
            // 
            // panel_Status
            // 
            this.panel_Status.Controls.Add(this.label9);
            this.panel_Status.Controls.Add(this.label_CurrentKey);
            this.panel_Status.Controls.Add(this.label12);
            this.panel_Status.Controls.Add(this.label_ServerStatus);
            this.panel_Status.Controls.Add(this.label13);
            this.panel_Status.Controls.Add(this.label_ServerIP);
            this.panel_Status.Location = new System.Drawing.Point(92, 4);
            this.panel_Status.Name = "panel_Status";
            this.panel_Status.Padding = new System.Windows.Forms.Padding(1, 1, 0, 0);
            this.panel_Status.Size = new System.Drawing.Size(620, 16);
            this.panel_Status.TabIndex = 31;
            this.panel_Status.Visible = false;
            this.panel_Status.WrapContents = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(1, 1);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(93, 14);
            this.label9.TabIndex = 5;
            this.label9.Text = "Ключ проекта: ";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_CurrentKey
            // 
            this.label_CurrentKey.AutoSize = true;
            this.label_CurrentKey.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Bold);
            this.label_CurrentKey.ForeColor = System.Drawing.Color.MediumBlue;
            this.label_CurrentKey.Location = new System.Drawing.Point(94, 1);
            this.label_CurrentKey.Margin = new System.Windows.Forms.Padding(0);
            this.label_CurrentKey.Name = "label_CurrentKey";
            this.label_CurrentKey.Size = new System.Drawing.Size(0, 14);
            this.label_CurrentKey.TabIndex = 5;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(114, 1);
            this.label12.Margin = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 14);
            this.label12.TabIndex = 5;
            this.label12.Text = "Сервер: ";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_ServerStatus
            // 
            this.label_ServerStatus.AutoSize = true;
            this.label_ServerStatus.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Bold);
            this.label_ServerStatus.ForeColor = System.Drawing.Color.Red;
            this.label_ServerStatus.Location = new System.Drawing.Point(167, 1);
            this.label_ServerStatus.Margin = new System.Windows.Forms.Padding(0);
            this.label_ServerStatus.Name = "label_ServerStatus";
            this.label_ServerStatus.Size = new System.Drawing.Size(0, 14);
            this.label_ServerStatus.TabIndex = 5;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(187, 1);
            this.label13.Margin = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(72, 14);
            this.label13.TabIndex = 5;
            this.label13.Text = "IP сервера: ";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_ServerIP
            // 
            this.label_ServerIP.AutoSize = true;
            this.label_ServerIP.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Bold);
            this.label_ServerIP.ForeColor = System.Drawing.Color.MediumBlue;
            this.label_ServerIP.Location = new System.Drawing.Point(259, 1);
            this.label_ServerIP.Margin = new System.Windows.Forms.Padding(0);
            this.label_ServerIP.Name = "label_ServerIP";
            this.label_ServerIP.Size = new System.Drawing.Size(0, 14);
            this.label_ServerIP.TabIndex = 5;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.panel_Separator);
            this.tabPage3.Controls.Add(this.panel1);
            this.tabPage3.Location = new System.Drawing.Point(4, 23);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(1, 2, 2, 2);
            this.tabPage3.Size = new System.Drawing.Size(979, 337);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Настройки клиентов";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel_Separator
            // 
            this.panel_Separator.BackColor = System.Drawing.Color.Transparent;
            this.panel_Separator.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Separator.Location = new System.Drawing.Point(1, 2);
            this.panel_Separator.Name = "panel_Separator";
            this.panel_Separator.Size = new System.Drawing.Size(976, 4);
            this.panel_Separator.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(976, 333);
            this.panel1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 32);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(976, 301);
            this.panel3.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.groupBox_Cyclogramm);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 195);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(6, 0, 6, 6);
            this.panel5.Size = new System.Drawing.Size(976, 106);
            this.panel5.TabIndex = 66;
            // 
            // groupBox_Cyclogramm
            // 
            this.groupBox_Cyclogramm.Controls.Add(this.label_CyclogrammStatus);
            this.groupBox_Cyclogramm.Controls.Add(this.label2);
            this.groupBox_Cyclogramm.Controls.Add(this.textBox_InputCyclogramm);
            this.groupBox_Cyclogramm.Controls.Add(this.button_FindInputCyclogramm);
            this.groupBox_Cyclogramm.Controls.Add(this.button_AddCyclogramm);
            this.groupBox_Cyclogramm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_Cyclogramm.Location = new System.Drawing.Point(6, 0);
            this.groupBox_Cyclogramm.Name = "groupBox_Cyclogramm";
            this.groupBox_Cyclogramm.Size = new System.Drawing.Size(964, 100);
            this.groupBox_Cyclogramm.TabIndex = 64;
            this.groupBox_Cyclogramm.TabStop = false;
            this.groupBox_Cyclogramm.Text = "Циклограмма";
            // 
            // label_CyclogrammStatus
            // 
            this.label_CyclogrammStatus.AutoSize = true;
            this.label_CyclogrammStatus.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_CyclogrammStatus.ForeColor = System.Drawing.Color.DarkGreen;
            this.label_CyclogrammStatus.Location = new System.Drawing.Point(15, 55);
            this.label_CyclogrammStatus.Name = "label_CyclogrammStatus";
            this.label_CyclogrammStatus.Size = new System.Drawing.Size(0, 22);
            this.label_CyclogrammStatus.TabIndex = 66;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 14);
            this.label2.TabIndex = 63;
            this.label2.Text = "Путь до файла CSV";
            // 
            // textBox_InputCyclogramm
            // 
            this.textBox_InputCyclogramm.AllowDrop = true;
            this.textBox_InputCyclogramm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_InputCyclogramm.Location = new System.Drawing.Point(133, 20);
            this.textBox_InputCyclogramm.Name = "textBox_InputCyclogramm";
            this.textBox_InputCyclogramm.ReadOnly = true;
            this.textBox_InputCyclogramm.Size = new System.Drawing.Size(524, 22);
            this.textBox_InputCyclogramm.TabIndex = 55;
            this.textBox_InputCyclogramm.TextChanged += new System.EventHandler(this.textBox_InputCyclogramm_TextChanged);
            this.textBox_InputCyclogramm.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
            // 
            // button_FindInputCyclogramm
            // 
            this.button_FindInputCyclogramm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_FindInputCyclogramm.Location = new System.Drawing.Point(663, 19);
            this.button_FindInputCyclogramm.Name = "button_FindInputCyclogramm";
            this.button_FindInputCyclogramm.Size = new System.Drawing.Size(64, 24);
            this.button_FindInputCyclogramm.TabIndex = 61;
            this.button_FindInputCyclogramm.Text = "Открыть";
            this.button_FindInputCyclogramm.UseVisualStyleBackColor = true;
            this.button_FindInputCyclogramm.Click += new System.EventHandler(this.button_FindInputCyclogram_Click);
            // 
            // button_AddCyclogramm
            // 
            this.button_AddCyclogramm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_AddCyclogramm.Enabled = false;
            this.button_AddCyclogramm.Location = new System.Drawing.Point(733, 19);
            this.button_AddCyclogramm.Name = "button_AddCyclogramm";
            this.button_AddCyclogramm.Size = new System.Drawing.Size(226, 24);
            this.button_AddCyclogramm.TabIndex = 62;
            this.button_AddCyclogramm.Text = "Добавить / Заменить циклограмму";
            this.button_AddCyclogramm.UseVisualStyleBackColor = true;
            this.button_AddCyclogramm.Click += new System.EventHandler(this.button_AddCyclogram_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label15);
            this.panel4.Controls.Add(this.label24);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.textBox_ClientNumber);
            this.panel4.Controls.Add(this.panel_PinList);
            this.panel4.Controls.Add(this.textBox_ClientUDPPort);
            this.panel4.Controls.Add(this.label22);
            this.panel4.Controls.Add(this.textBox_ClientName);
            this.panel4.Controls.Add(this.button_SaveClient);
            this.panel4.Controls.Add(this.label19);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.label25);
            this.panel4.Controls.Add(this.textBox_ClientPasswordWifi);
            this.panel4.Controls.Add(this.label16);
            this.panel4.Controls.Add(this.textBox_ClientSSID);
            this.panel4.Controls.Add(this.label23);
            this.panel4.Controls.Add(this.textBox_ClientLEDCount);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(976, 195);
            this.panel4.TabIndex = 65;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(32, 6);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(91, 20);
            this.label15.TabIndex = 35;
            this.label15.Text = "Номер клиента";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Cambria", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label24.ForeColor = System.Drawing.Color.SteelBlue;
            this.label24.Location = new System.Drawing.Point(198, 113);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(53, 12);
            this.label24.TabIndex = 38;
            this.label24.Text = "1 ... 65 535";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Cambria", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ForeColor = System.Drawing.Color.SteelBlue;
            this.label4.Location = new System.Drawing.Point(198, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 38;
            this.label4.Text = "1 ... 65 535";
            // 
            // textBox_ClientNumber
            // 
            this.textBox_ClientNumber.BackColor = System.Drawing.SystemColors.Window;
            this.textBox_ClientNumber.Location = new System.Drawing.Point(124, 6);
            this.textBox_ClientNumber.Name = "textBox_ClientNumber";
            this.textBox_ClientNumber.Size = new System.Drawing.Size(49, 22);
            this.textBox_ClientNumber.TabIndex = 44;
            this.textBox_ClientNumber.TextChanged += new System.EventHandler(this.textBox_ClientNumber_TextChanged);
            // 
            // panel_PinList
            // 
            this.panel_PinList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_PinList.Controls.Add(this.textBox_PinNumber);
            this.panel_PinList.Controls.Add(this.textBox_LEDCountCheck);
            this.panel_PinList.Controls.Add(this.dataGridView_PinList);
            this.panel_PinList.Controls.Add(this.textBox_PinLEDCount);
            this.panel_PinList.Controls.Add(this.button_AddPin);
            this.panel_PinList.Enabled = false;
            this.panel_PinList.Location = new System.Drawing.Point(297, 4);
            this.panel_PinList.Name = "panel_PinList";
            this.panel_PinList.Size = new System.Drawing.Size(676, 187);
            this.panel_PinList.TabIndex = 57;
            // 
            // textBox_PinNumber
            // 
            this.textBox_PinNumber.Location = new System.Drawing.Point(11, 2);
            this.textBox_PinNumber.Name = "textBox_PinNumber";
            this.textBox_PinNumber.Size = new System.Drawing.Size(59, 22);
            this.textBox_PinNumber.TabIndex = 51;
            this.textBox_PinNumber.TextChanged += new System.EventHandler(this.textBox_PinNumber_TextChanged);
            // 
            // textBox_LEDCountCheck
            // 
            this.textBox_LEDCountCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_LEDCountCheck.Location = new System.Drawing.Point(454, 2);
            this.textBox_LEDCountCheck.Name = "textBox_LEDCountCheck";
            this.textBox_LEDCountCheck.ReadOnly = true;
            this.textBox_LEDCountCheck.Size = new System.Drawing.Size(85, 22);
            this.textBox_LEDCountCheck.TabIndex = 56;
            // 
            // dataGridView_PinList
            // 
            this.dataGridView_PinList.AllowUserToAddRows = false;
            this.dataGridView_PinList.AllowUserToDeleteRows = false;
            this.dataGridView_PinList.AllowUserToResizeColumns = false;
            this.dataGridView_PinList.AllowUserToResizeRows = false;
            this.dataGridView_PinList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_PinList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_PinList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView_PinList.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridView_PinList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightSkyBlue;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Cambria", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.DeepSkyBlue;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_PinList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView_PinList.ColumnHeadersHeight = 18;
            this.dataGridView_PinList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView_PinList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dataGridView_PinList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView_PinList.EnableHeadersVisualStyles = false;
            this.dataGridView_PinList.GridColor = System.Drawing.Color.Black;
            this.dataGridView_PinList.Location = new System.Drawing.Point(10, 26);
            this.dataGridView_PinList.MultiSelect = false;
            this.dataGridView_PinList.Name = "dataGridView_PinList";
            this.dataGridView_PinList.RowHeadersVisible = false;
            this.dataGridView_PinList.RowTemplate.Height = 20;
            this.dataGridView_PinList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_PinList.ShowCellToolTips = false;
            this.dataGridView_PinList.ShowEditingIcon = false;
            this.dataGridView_PinList.Size = new System.Drawing.Size(663, 159);
            this.dataGridView_PinList.TabIndex = 54;
            this.dataGridView_PinList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_PinList_KeyDown);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "PinNumber";
            this.dataGridViewTextBoxColumn1.HeaderText = "Пин";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 60;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "LEDCount";
            this.dataGridViewTextBoxColumn2.HeaderText = "Количество светодиодов";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // textBox_PinLEDCount
            // 
            this.textBox_PinLEDCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_PinLEDCount.Location = new System.Drawing.Point(73, 2);
            this.textBox_PinLEDCount.Name = "textBox_PinLEDCount";
            this.textBox_PinLEDCount.Size = new System.Drawing.Size(378, 22);
            this.textBox_PinLEDCount.TabIndex = 52;
            this.textBox_PinLEDCount.TextChanged += new System.EventHandler(this.textBox_PinLEDCount_TextChanged);
            // 
            // button_AddPin
            // 
            this.button_AddPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_AddPin.Font = new System.Drawing.Font("Cambria", 9F);
            this.button_AddPin.Location = new System.Drawing.Point(541, 1);
            this.button_AddPin.Name = "button_AddPin";
            this.button_AddPin.Size = new System.Drawing.Size(132, 24);
            this.button_AddPin.TabIndex = 53;
            this.button_AddPin.Text = "Добавить/Изменить";
            this.button_AddPin.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button_AddPin.UseVisualStyleBackColor = true;
            this.button_AddPin.Click += new System.EventHandler(this.button_AddPin_Click);
            // 
            // textBox_ClientUDPPort
            // 
            this.textBox_ClientUDPPort.Location = new System.Drawing.Point(124, 109);
            this.textBox_ClientUDPPort.Name = "textBox_ClientUDPPort";
            this.textBox_ClientUDPPort.Size = new System.Drawing.Size(61, 22);
            this.textBox_ClientUDPPort.TabIndex = 49;
            this.textBox_ClientUDPPort.TextChanged += new System.EventHandler(this.textBox_ClientUDPPort_TextChanged);
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(28, 84);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(95, 20);
            this.label22.TabIndex = 40;
            this.label22.Text = "Пароль WiFi сети";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox_ClientName
            // 
            this.textBox_ClientName.BackColor = System.Drawing.SystemColors.Window;
            this.textBox_ClientName.Location = new System.Drawing.Point(124, 31);
            this.textBox_ClientName.Name = "textBox_ClientName";
            this.textBox_ClientName.Size = new System.Drawing.Size(162, 22);
            this.textBox_ClientName.TabIndex = 45;
            this.textBox_ClientName.WordWrap = false;
            this.textBox_ClientName.TextChanged += new System.EventHandler(this.textBox_ClientName_TextChanged);
            // 
            // button_SaveClient
            // 
            this.button_SaveClient.Enabled = false;
            this.button_SaveClient.Location = new System.Drawing.Point(124, 162);
            this.button_SaveClient.Name = "button_SaveClient";
            this.button_SaveClient.Size = new System.Drawing.Size(140, 22);
            this.button_SaveClient.TabIndex = 47;
            this.button_SaveClient.Text = "Сохранить клиент";
            this.button_SaveClient.UseVisualStyleBackColor = true;
            this.button_SaveClient.Click += new System.EventHandler(this.button_SaveClient_Click);
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(8, 134);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(115, 20);
            this.label19.TabIndex = 36;
            this.label19.Text = "Кол-во светодиодов";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(32, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 20);
            this.label6.TabIndex = 34;
            this.label6.Text = "Имя клиента";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(11, 108);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(112, 20);
            this.label25.TabIndex = 42;
            this.label25.Text = "UDP порт клиента";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox_ClientPasswordWifi
            // 
            this.textBox_ClientPasswordWifi.Location = new System.Drawing.Point(124, 84);
            this.textBox_ClientPasswordWifi.Name = "textBox_ClientPasswordWifi";
            this.textBox_ClientPasswordWifi.Size = new System.Drawing.Size(140, 22);
            this.textBox_ClientPasswordWifi.TabIndex = 48;
            this.textBox_ClientPasswordWifi.TextChanged += new System.EventHandler(this.textBox_ClientPassowordWifi_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Cambria", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label16.ForeColor = System.Drawing.Color.SteelBlue;
            this.label16.Location = new System.Drawing.Point(185, 10);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(39, 12);
            this.label16.TabIndex = 39;
            this.label16.Text = "1 ... 255";
            // 
            // textBox_ClientSSID
            // 
            this.textBox_ClientSSID.Location = new System.Drawing.Point(124, 59);
            this.textBox_ClientSSID.Name = "textBox_ClientSSID";
            this.textBox_ClientSSID.Size = new System.Drawing.Size(140, 22);
            this.textBox_ClientSSID.TabIndex = 47;
            this.textBox_ClientSSID.TextChanged += new System.EventHandler(this.textBox_ClientSSID_TextChanged);
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(56, 59);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(67, 20);
            this.label23.TabIndex = 43;
            this.label23.Text = "SSID WiFi";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox_ClientLEDCount
            // 
            this.textBox_ClientLEDCount.Location = new System.Drawing.Point(124, 134);
            this.textBox_ClientLEDCount.Name = "textBox_ClientLEDCount";
            this.textBox_ClientLEDCount.Size = new System.Drawing.Size(61, 22);
            this.textBox_ClientLEDCount.TabIndex = 50;
            this.textBox_ClientLEDCount.TextChanged += new System.EventHandler(this.textBox_ClientLEDCount_TextChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.LightSkyBlue;
            this.panel2.Controls.Add(this.ClientSaveImage);
            this.panel2.Controls.Add(this.button_CopyClient);
            this.panel2.Controls.Add(this.button_DeleteClient);
            this.panel2.Controls.Add(this.label36);
            this.panel2.Controls.Add(this.comboBox_Client);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.comboBox_RemovableDrive);
            this.panel2.Controls.Add(this.button_AddClient);
            this.panel2.Controls.Add(this.button_LoadToSD);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(976, 32);
            this.panel2.TabIndex = 0;
            // 
            // ClientSaveImage
            // 
            this.ClientSaveImage.BackgroundImage = global::RemoteLEDServer.Properties.Resources.save_error;
            this.ClientSaveImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSaveImage.ErrorImage = null;
            this.ClientSaveImage.InitialImage = null;
            this.ClientSaveImage.Location = new System.Drawing.Point(17, 7);
            this.ClientSaveImage.Name = "ClientSaveImage";
            this.ClientSaveImage.Size = new System.Drawing.Size(20, 20);
            this.ClientSaveImage.TabIndex = 59;
            this.ClientSaveImage.TabStop = false;
            this.ClientSaveImage.Visible = false;
            // 
            // button_CopyClient
            // 
            this.button_CopyClient.Location = new System.Drawing.Point(432, 7);
            this.button_CopyClient.Name = "button_CopyClient";
            this.button_CopyClient.Size = new System.Drawing.Size(138, 22);
            this.button_CopyClient.TabIndex = 47;
            this.button_CopyClient.Text = "Скопировать клиент";
            this.button_CopyClient.UseVisualStyleBackColor = true;
            this.button_CopyClient.Click += new System.EventHandler(this.button_CopyClient_Click);
            // 
            // button_DeleteClient
            // 
            this.button_DeleteClient.Location = new System.Drawing.Point(572, 7);
            this.button_DeleteClient.Name = "button_DeleteClient";
            this.button_DeleteClient.Size = new System.Drawing.Size(115, 22);
            this.button_DeleteClient.TabIndex = 47;
            this.button_DeleteClient.Text = "Удалить клиент";
            this.button_DeleteClient.UseVisualStyleBackColor = true;
            this.button_DeleteClient.Click += new System.EventHandler(this.button_DeleteClient_Click);
            // 
            // label36
            // 
            this.label36.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label36.Location = new System.Drawing.Point(37, 7);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(62, 19);
            this.label36.TabIndex = 45;
            this.label36.Text = "Клиент:";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBox_Client
            // 
            this.comboBox_Client.DisplayMember = "Name";
            this.comboBox_Client.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Client.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox_Client.FormattingEnabled = true;
            this.comboBox_Client.Location = new System.Drawing.Point(101, 7);
            this.comboBox_Client.Name = "comboBox_Client";
            this.comboBox_Client.Size = new System.Drawing.Size(200, 22);
            this.comboBox_Client.TabIndex = 44;
            this.comboBox_Client.ValueMember = "Number";
            this.comboBox_Client.SelectedIndexChanged += new System.EventHandler(this.comboBox_Client_SelectedIndexChanged);
            this.comboBox_Client.SelectionChangeCommitted += new System.EventHandler(this.comboBox_Client_SelectionChangeCommitted);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(711, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 22);
            this.button1.TabIndex = 61;
            this.button1.Text = "Обновить диски";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox_RemovableDrive
            // 
            this.comboBox_RemovableDrive.FormattingEnabled = true;
            this.comboBox_RemovableDrive.Location = new System.Drawing.Point(820, 7);
            this.comboBox_RemovableDrive.Name = "comboBox_RemovableDrive";
            this.comboBox_RemovableDrive.Size = new System.Drawing.Size(42, 22);
            this.comboBox_RemovableDrive.TabIndex = 60;
            // 
            // button_AddClient
            // 
            this.button_AddClient.Enabled = false;
            this.button_AddClient.Location = new System.Drawing.Point(307, 7);
            this.button_AddClient.Name = "button_AddClient";
            this.button_AddClient.Size = new System.Drawing.Size(123, 22);
            this.button_AddClient.TabIndex = 46;
            this.button_AddClient.Text = "Добавить клиент";
            this.button_AddClient.UseVisualStyleBackColor = true;
            this.button_AddClient.Click += new System.EventHandler(this.button_AddClient_Click);
            // 
            // button_LoadToSD
            // 
            this.button_LoadToSD.Location = new System.Drawing.Point(864, 7);
            this.button_LoadToSD.Name = "button_LoadToSD";
            this.button_LoadToSD.Size = new System.Drawing.Size(106, 22);
            this.button_LoadToSD.TabIndex = 47;
            this.button_LoadToSD.Text = "Загрузить на SD";
            this.button_LoadToSD.UseVisualStyleBackColor = true;
            this.button_LoadToSD.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox_Audio);
            this.tabPage2.Controls.Add(this.groupBox_server);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(725, 260);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Настройки сервера";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox_Audio
            // 
            this.groupBox_Audio.Controls.Add(this.comboBox_AudioOutputs);
            this.groupBox_Audio.Controls.Add(this.label_audioDevice);
            this.groupBox_Audio.Location = new System.Drawing.Point(352, 6);
            this.groupBox_Audio.Name = "groupBox_Audio";
            this.groupBox_Audio.Size = new System.Drawing.Size(355, 99);
            this.groupBox_Audio.TabIndex = 35;
            this.groupBox_Audio.TabStop = false;
            this.groupBox_Audio.Text = "Аудио";
            // 
            // comboBox_AudioOutputs
            // 
            this.comboBox_AudioOutputs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_AudioOutputs.FormattingEnabled = true;
            this.comboBox_AudioOutputs.Location = new System.Drawing.Point(16, 51);
            this.comboBox_AudioOutputs.Name = "comboBox_AudioOutputs";
            this.comboBox_AudioOutputs.Size = new System.Drawing.Size(320, 22);
            this.comboBox_AudioOutputs.TabIndex = 31;
            this.comboBox_AudioOutputs.SelectionChangeCommitted += new System.EventHandler(this.comboBox_AudioOutputs_SelectionChangeCommitted);
            // 
            // label_audioDevice
            // 
            this.label_audioDevice.Location = new System.Drawing.Point(13, 25);
            this.label_audioDevice.Name = "label_audioDevice";
            this.label_audioDevice.Size = new System.Drawing.Size(190, 18);
            this.label_audioDevice.TabIndex = 10;
            this.label_audioDevice.Text = "Устройство воспроизведения";
            this.label_audioDevice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox_server
            // 
            this.groupBox_server.Controls.Add(this.label40);
            this.groupBox_server.Controls.Add(this.button_RefreshIPList);
            this.groupBox_server.Controls.Add(this.textBox_localPort);
            this.groupBox_server.Controls.Add(this.button_TurnOffServer);
            this.groupBox_server.Controls.Add(this.label3);
            this.groupBox_server.Controls.Add(this.comboBox_IP);
            this.groupBox_server.Controls.Add(this.button_SaveServerSetting);
            this.groupBox_server.Location = new System.Drawing.Point(6, 6);
            this.groupBox_server.Name = "groupBox_server";
            this.groupBox_server.Size = new System.Drawing.Size(340, 160);
            this.groupBox_server.TabIndex = 34;
            this.groupBox_server.TabStop = false;
            this.groupBox_server.Text = "Сервер";
            // 
            // label40
            // 
            this.label40.Location = new System.Drawing.Point(5, 81);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(108, 18);
            this.label40.TabIndex = 29;
            this.label40.Text = "IP адрес сервера";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button_RefreshIPList
            // 
            this.button_RefreshIPList.Location = new System.Drawing.Point(249, 78);
            this.button_RefreshIPList.Name = "button_RefreshIPList";
            this.button_RefreshIPList.Size = new System.Drawing.Size(71, 23);
            this.button_RefreshIPList.TabIndex = 33;
            this.button_RefreshIPList.Text = "Обновить";
            this.button_RefreshIPList.UseVisualStyleBackColor = true;
            this.button_RefreshIPList.Click += new System.EventHandler(this.button_RefreshIPList_Click);
            // 
            // textBox_localPort
            // 
            this.textBox_localPort.Location = new System.Drawing.Point(118, 50);
            this.textBox_localPort.Name = "textBox_localPort";
            this.textBox_localPort.Size = new System.Drawing.Size(81, 22);
            this.textBox_localPort.TabIndex = 7;
            this.textBox_localPort.TextChanged += new System.EventHandler(this.textBox_localPort_TextChanged);
            // 
            // button_TurnOffServer
            // 
            this.button_TurnOffServer.Location = new System.Drawing.Point(117, 17);
            this.button_TurnOffServer.Name = "button_TurnOffServer";
            this.button_TurnOffServer.Size = new System.Drawing.Size(147, 23);
            this.button_TurnOffServer.TabIndex = 32;
            this.button_TurnOffServer.Text = "Вкл/выкл сервер";
            this.button_TurnOffServer.UseVisualStyleBackColor = true;
            this.button_TurnOffServer.Click += new System.EventHandler(this.button_TurnOffServer_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(18, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 18);
            this.label3.TabIndex = 10;
            this.label3.Text = "Порт (1-65535)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBox_IP
            // 
            this.comboBox_IP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_IP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox_IP.FormattingEnabled = true;
            this.comboBox_IP.Location = new System.Drawing.Point(118, 78);
            this.comboBox_IP.Name = "comboBox_IP";
            this.comboBox_IP.Size = new System.Drawing.Size(129, 23);
            this.comboBox_IP.TabIndex = 28;
            // 
            // button_SaveServerSetting
            // 
            this.button_SaveServerSetting.Enabled = false;
            this.button_SaveServerSetting.Location = new System.Drawing.Point(117, 114);
            this.button_SaveServerSetting.Name = "button_SaveServerSetting";
            this.button_SaveServerSetting.Size = new System.Drawing.Size(147, 23);
            this.button_SaveServerSetting.TabIndex = 30;
            this.button_SaveServerSetting.Text = "Сохранить настройки";
            this.button_SaveServerSetting.UseVisualStyleBackColor = true;
            this.button_SaveServerSetting.Click += new System.EventHandler(this.button_SaveServerSetting_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel_Clients);
            this.tabPage1.Controls.Add(this.panel7);
            this.tabPage1.Controls.Add(this.panel_Player);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(979, 337);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Выступление";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel_Clients
            // 
            this.panel_Clients.Controls.Add(this.dataGridView_Clients);
            this.panel_Clients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Clients.Location = new System.Drawing.Point(3, 3);
            this.panel_Clients.Name = "panel_Clients";
            this.panel_Clients.Size = new System.Drawing.Size(973, 234);
            this.panel_Clients.TabIndex = 26;
            // 
            // dataGridView_Clients
            // 
            this.dataGridView_Clients.AllowDrop = true;
            this.dataGridView_Clients.AllowUserToAddRows = false;
            this.dataGridView_Clients.AllowUserToDeleteRows = false;
            this.dataGridView_Clients.AllowUserToResizeColumns = false;
            this.dataGridView_Clients.AllowUserToResizeRows = false;
            this.dataGridView_Clients.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_Clients.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView_Clients.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridView_Clients.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.LightSkyBlue;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Cambria", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.DeepSkyBlue;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_Clients.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView_Clients.ColumnHeadersHeight = 24;
            this.dataGridView_Clients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView_Clients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_Number,
            this.Column_Name,
            this.Column_Status});
            this.dataGridView_Clients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Clients.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView_Clients.EnableHeadersVisualStyles = false;
            this.dataGridView_Clients.GridColor = System.Drawing.Color.Black;
            this.dataGridView_Clients.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_Clients.MultiSelect = false;
            this.dataGridView_Clients.Name = "dataGridView_Clients";
            this.dataGridView_Clients.RowHeadersVisible = false;
            this.dataGridView_Clients.RowTemplate.Height = 20;
            this.dataGridView_Clients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView_Clients.ShowCellToolTips = false;
            this.dataGridView_Clients.ShowEditingIcon = false;
            this.dataGridView_Clients.Size = new System.Drawing.Size(973, 234);
            this.dataGridView_Clients.TabIndex = 22;
            this.dataGridView_Clients.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView_Clients_CellPainting);
            // 
            // Column_Number
            // 
            this.Column_Number.DataPropertyName = "Number";
            this.Column_Number.HeaderText = "Номер";
            this.Column_Number.Name = "Column_Number";
            this.Column_Number.Width = 60;
            // 
            // Column_Name
            // 
            this.Column_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column_Name.DataPropertyName = "Name";
            this.Column_Name.HeaderText = "Название";
            this.Column_Name.Name = "Column_Name";
            // 
            // Column_Status
            // 
            this.Column_Status.DataPropertyName = "StatusString";
            this.Column_Status.HeaderText = "Статус";
            this.Column_Status.Name = "Column_Status";
            this.Column_Status.Width = 70;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.Transparent;
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(3, 237);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(973, 4);
            this.panel7.TabIndex = 24;
            // 
            // panel_Player
            // 
            this.panel_Player.Controls.Add(this.rlcPlayer1);
            this.panel_Player.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_Player.Location = new System.Drawing.Point(3, 241);
            this.panel_Player.Name = "panel_Player";
            this.panel_Player.Size = new System.Drawing.Size(973, 93);
            this.panel_Player.TabIndex = 23;
            // 
            // rlcPlayer1
            // 
            this.rlcPlayer1.AllowDrop = true;
            this.rlcPlayer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rlcPlayer1.Controls.Add(this.panel_PlayerControls);
            this.rlcPlayer1.CurrentTime = System.TimeSpan.Parse("00:00:00");
            this.rlcPlayer1.CurrentTimeString = null;
            this.rlcPlayer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rlcPlayer1.LabelsVertMargin = 2;
            this.rlcPlayer1.Location = new System.Drawing.Point(0, 0);
            this.rlcPlayer1.MinimumSize = new System.Drawing.Size(150, 50);
            this.rlcPlayer1.Name = "rlcPlayer1";
            this.rlcPlayer1.PlayerTrackBackColor = System.Drawing.Color.White;
            this.rlcPlayer1.PlayerTrackBorderColor = System.Drawing.Color.LightGray;
            this.rlcPlayer1.PlayerTrackBorderHoverColor = System.Drawing.Color.Black;
            this.rlcPlayer1.PlayerTrackHeight = 15;
            this.rlcPlayer1.PlayerTrackLeftRightMargin = 10;
            this.rlcPlayer1.PlayerTrackTopBottomMargin = 4;
            this.rlcPlayer1.PlayerTrackTrackColor = System.Drawing.Color.LightSkyBlue;
            this.rlcPlayer1.Size = new System.Drawing.Size(973, 93);
            this.rlcPlayer1.TabIndex = 23;
            this.rlcPlayer1.Volume = 100;
            this.rlcPlayer1.OnMouseSetTime += new RLCPlayer.RLCPlayer.DMouseSetTime(this.rlcPlayer1_OnMouseSetTime);
            this.rlcPlayer1.OnInitializedPlayer += new RLCPlayer.RLCPlayer.DInitializedPlayer(this.rlcPlayer1_OnInitializedPlayer);
            // 
            // panel_PlayerControls
            // 
            this.panel_PlayerControls.Controls.Add(this.panel11);
            this.panel_PlayerControls.Controls.Add(this.panel10);
            this.panel_PlayerControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_PlayerControls.Location = new System.Drawing.Point(0, 63);
            this.panel_PlayerControls.Name = "panel_PlayerControls";
            this.panel_PlayerControls.Size = new System.Drawing.Size(971, 28);
            this.panel_PlayerControls.TabIndex = 6;
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.volumeSlider2);
            this.panel11.Controls.Add(this.pictureBox1);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel11.Location = new System.Drawing.Point(239, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(200, 28);
            this.panel11.TabIndex = 9;
            // 
            // volumeSlider2
            // 
            this.volumeSlider2.FillColor = System.Drawing.Color.LightSkyBlue;
            this.volumeSlider2.Location = new System.Drawing.Point(31, 7);
            this.volumeSlider2.Name = "volumeSlider2";
            this.volumeSlider2.Size = new System.Drawing.Size(112, 16);
            this.volumeSlider2.TabIndex = 7;
            this.volumeSlider2.VolumeChanged += new System.EventHandler(this.volumeSlider2_VolumeChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = global::RemoteLEDServer.Properties.Resources.volume;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(10, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(21, 24);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.maskedTextBox_SetTime);
            this.panel10.Controls.Add(this.button_Open);
            this.panel10.Controls.Add(this.button_PlayFrom);
            this.panel10.Controls.Add(this.button_Play);
            this.panel10.Controls.Add(this.button_Pause);
            this.panel10.Controls.Add(this.button_Stop);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel10.Location = new System.Drawing.Point(0, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(239, 28);
            this.panel10.TabIndex = 8;
            // 
            // maskedTextBox_SetTime
            // 
            this.maskedTextBox_SetTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.maskedTextBox_SetTime.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.maskedTextBox_SetTime.Location = new System.Drawing.Point(155, 3);
            this.maskedTextBox_SetTime.Mask = "00:00";
            this.maskedTextBox_SetTime.Name = "maskedTextBox_SetTime";
            this.maskedTextBox_SetTime.Size = new System.Drawing.Size(37, 22);
            this.maskedTextBox_SetTime.TabIndex = 22;
            this.maskedTextBox_SetTime.ValidatingType = typeof(System.DateTime);
            this.maskedTextBox_SetTime.TextChanged += new System.EventHandler(this.maskedTextBox_SetTime_TextChanged);
            this.maskedTextBox_SetTime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.maskedTextBox_SetTime_KeyDown);
            // 
            // button_Open
            // 
            this.button_Open.BackColor = System.Drawing.Color.Transparent;
            this.button_Open.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_Open.FlatAppearance.BorderSize = 0;
            this.button_Open.Image = ((System.Drawing.Image)(resources.GetObject("button_Open.Image")));
            this.button_Open.Location = new System.Drawing.Point(96, 2);
            this.button_Open.Name = "button_Open";
            this.button_Open.Size = new System.Drawing.Size(27, 24);
            this.button_Open.TabIndex = 6;
            this.button_Open.UseVisualStyleBackColor = false;
            this.button_Open.Click += new System.EventHandler(this.button_Open_Click);
            // 
            // button_PlayFrom
            // 
            this.button_PlayFrom.BackColor = System.Drawing.Color.Transparent;
            this.button_PlayFrom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_PlayFrom.Enabled = false;
            this.button_PlayFrom.FlatAppearance.BorderSize = 0;
            this.button_PlayFrom.Image = global::RemoteLEDServer.Properties.Resources.play_simple;
            this.button_PlayFrom.Location = new System.Drawing.Point(193, 2);
            this.button_PlayFrom.Name = "button_PlayFrom";
            this.button_PlayFrom.Size = new System.Drawing.Size(27, 24);
            this.button_PlayFrom.TabIndex = 6;
            this.button_PlayFrom.UseVisualStyleBackColor = false;
            this.button_PlayFrom.Click += new System.EventHandler(this.button_PlayFrom_Click);
            // 
            // button_Play
            // 
            this.button_Play.BackColor = System.Drawing.Color.Transparent;
            this.button_Play.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_Play.FlatAppearance.BorderSize = 0;
            this.button_Play.Image = global::RemoteLEDServer.Properties.Resources.play_simple;
            this.button_Play.Location = new System.Drawing.Point(12, 2);
            this.button_Play.Name = "button_Play";
            this.button_Play.Size = new System.Drawing.Size(27, 24);
            this.button_Play.TabIndex = 6;
            this.button_Play.UseVisualStyleBackColor = false;
            this.button_Play.Click += new System.EventHandler(this.button_Play_Click);
            // 
            // button_Pause
            // 
            this.button_Pause.BackColor = System.Drawing.Color.Transparent;
            this.button_Pause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_Pause.FlatAppearance.BorderSize = 0;
            this.button_Pause.Image = ((System.Drawing.Image)(resources.GetObject("button_Pause.Image")));
            this.button_Pause.Location = new System.Drawing.Point(40, 2);
            this.button_Pause.Name = "button_Pause";
            this.button_Pause.Size = new System.Drawing.Size(27, 24);
            this.button_Pause.TabIndex = 6;
            this.button_Pause.UseVisualStyleBackColor = false;
            this.button_Pause.Click += new System.EventHandler(this.button_Pause_Click);
            // 
            // button_Stop
            // 
            this.button_Stop.BackColor = System.Drawing.Color.Transparent;
            this.button_Stop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_Stop.FlatAppearance.BorderSize = 0;
            this.button_Stop.Image = ((System.Drawing.Image)(resources.GetObject("button_Stop.Image")));
            this.button_Stop.Location = new System.Drawing.Point(68, 2);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(27, 24);
            this.button_Stop.TabIndex = 6;
            this.button_Stop.UseVisualStyleBackColor = false;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Enabled = false;
            this.tabControl1.Location = new System.Drawing.Point(1, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(987, 364);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label10);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.label5);
            this.tabPage4.Controls.Add(this.label1);
            this.tabPage4.Location = new System.Drawing.Point(4, 23);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(979, 577);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "О программе";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(28, 177);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(109, 15);
            this.label10.TabIndex = 5;
            this.label10.Text = "Copyright ©  2017";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(28, 142);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(114, 15);
            this.label7.TabIndex = 6;
            this.label7.Text = "Skype: enzovadim1";
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.Location = new System.Drawing.Point(27, 57);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(567, 39);
            this.label11.TabIndex = 7;
            this.label11.Text = "Программа для удаленного управления по WiFi сети светодиодными массивами покдлюче" +
    "нными к клиентскому контроллеру Esp8266";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(27, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 15);
            this.label8.TabIndex = 8;
            this.label8.Text = "Для связи:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(28, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(170, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "E-mail: enzogord@gmail.com";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(27, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(304, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "Remote LED Control Server    Version 1.0";
            // 
            // ProjectSaveImage
            // 
            this.ProjectSaveImage.BackgroundImage = global::RemoteLEDServer.Properties.Resources.save_error;
            this.ProjectSaveImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ProjectSaveImage.ErrorImage = null;
            this.ProjectSaveImage.InitialImage = null;
            this.ProjectSaveImage.Location = new System.Drawing.Point(56, 1);
            this.ProjectSaveImage.Name = "ProjectSaveImage";
            this.ProjectSaveImage.Size = new System.Drawing.Size(20, 20);
            this.ProjectSaveImage.TabIndex = 59;
            this.ProjectSaveImage.TabStop = false;
            this.ProjectSaveImage.Visible = false;
            // 
            // cyclogrammBindingSource2
            // 
            this.cyclogrammBindingSource2.DataSource = typeof(Core.Cyclogramm);
            // 
            // cyclogrammBindingSource
            // 
            this.cyclogrammBindingSource.DataSource = typeof(Core.Cyclogramm);
            // 
            // cyclogrammBindingSource1
            // 
            this.cyclogrammBindingSource1.DataSource = typeof(Core.Cyclogramm);
            // 
            // RemoteLEDControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 388);
            this.Controls.Add(this.ProjectSaveImage);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel_Status);
            this.Controls.Add(this.MainMenu);
            this.Font = new System.Drawing.Font("Cambria", 9F);
            this.MinimumSize = new System.Drawing.Size(1004, 350);
            this.Name = "RemoteLEDControl";
            this.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.Text = "Remote LED Control Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RemoteLEDControl_FormClosing);
            this.Load += new System.EventHandler(this.RemoteLEDControl_Load);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.panel_Status.ResumeLayout(false);
            this.panel_Status.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.groupBox_Cyclogramm.ResumeLayout(false);
            this.groupBox_Cyclogramm.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel_PinList.ResumeLayout(false);
            this.panel_PinList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_PinList)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ClientSaveImage)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.groupBox_Audio.ResumeLayout(false);
            this.groupBox_server.ResumeLayout(false);
            this.groupBox_server.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.panel_Clients.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Clients)).EndInit();
            this.panel_Player.ResumeLayout(false);
            this.rlcPlayer1.ResumeLayout(false);
            this.panel_PlayerControls.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProjectSaveImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cyclogrammBindingSource2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cyclogrammBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cyclogrammBindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemFile;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_CreateProject;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_OpenProject;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_SaveProject;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_SaveAsProject;
        private System.Windows.Forms.FlowLayoutPanel panel_Status;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label_CurrentKey;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label_ServerStatus;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label_ServerIP;
        private System.Windows.Forms.PictureBox ProjectSaveImage;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button_AddCyclogramm;
        private System.Windows.Forms.TextBox textBox_InputCyclogramm;
        private System.Windows.Forms.Button button_FindInputCyclogramm;
        private System.Windows.Forms.Panel panel_Separator;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel_PinList;
        private System.Windows.Forms.TextBox textBox_PinNumber;
        private System.Windows.Forms.TextBox textBox_LEDCountCheck;
        private System.Windows.Forms.DataGridView dataGridView_PinList;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.TextBox textBox_PinLEDCount;
        private System.Windows.Forms.Button button_AddPin;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button button_SaveClient;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_ClientPasswordWifi;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox textBox_ClientLEDCount;
        private System.Windows.Forms.TextBox textBox_ClientSSID;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox textBox_ClientName;
        private System.Windows.Forms.TextBox textBox_ClientUDPPort;
        private System.Windows.Forms.TextBox textBox_ClientNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox ClientSaveImage;
        private System.Windows.Forms.Button button_DeleteClient;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.ComboBox comboBox_Client;
        private System.Windows.Forms.Button button_AddClient;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ComboBox comboBox_AudioOutputs;
        private System.Windows.Forms.Button button_SaveServerSetting;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.ComboBox comboBox_IP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_localPort;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dataGridView_Clients;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel_Player;
        private System.Windows.Forms.Panel panel_PlayerControls;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Button button_Open;
        private System.Windows.Forms.Button button_Play;
        private System.Windows.Forms.Button button_Pause;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Panel panel_Clients;
        private System.Windows.Forms.MaskedTextBox maskedTextBox_SetTime;
        private System.Windows.Forms.Button button_PlayFrom;
        private System.Windows.Forms.BindingSource cyclogrammBindingSource;
        private System.Windows.Forms.BindingSource cyclogrammBindingSource1;
        private System.Windows.Forms.BindingSource cyclogrammBindingSource2;
        private System.Windows.Forms.ComboBox comboBox_RemovableDrive;
        private System.Windows.Forms.Button button_LoadToSD;
        private System.Windows.Forms.Button button_TurnOffServer;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox_Audio;
        private System.Windows.Forms.Label label_audioDevice;
        private System.Windows.Forms.GroupBox groupBox_server;
        private System.Windows.Forms.Button button_RefreshIPList;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Status;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox_Cyclogramm;
        private System.Windows.Forms.Button button_CopyClient;
        private System.Windows.Forms.Label label_CyclogrammStatus;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private RLCPlayer.RLCPlayer rlcPlayer1;
        private VolumeSlider.VolumeSlider volumeSlider2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
    }
}

