namespace TomoBra_DAQ
{
    partial class form_main
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
            this.ms_mainMenu = new System.Windows.Forms.MenuStrip();
            this.tsmi_file = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_file_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_help_view = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_graph_Baseline = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_graph_Signal = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_graph_Response = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_graph_MemDiff = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_graph_Display = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_graph_Display_Default = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_graph_Display_Dual = new System.Windows.Forms.ToolStripMenuItem();
            this.ofd_PS_browse = new System.Windows.Forms.OpenFileDialog();
            this.lv_msgLog = new System.Windows.Forms.ListView();
            this.ch_time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_event = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ss_main = new System.Windows.Forms.StatusStrip();
            this.tssl_connectionStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_device = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_statusSM = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_SM = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_TX = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_TXnum = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_RX = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_RXnum = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_running = new System.Windows.Forms.ToolStripStatusLabel();
            this.pb_status = new System.Windows.Forms.ToolStripProgressBar();
            this.bgw_TD_realTime = new System.ComponentModel.BackgroundWorker();
            this.bgw_TD_collectAuto = new System.ComponentModel.BackgroundWorker();
            this.sc_main = new System.Windows.Forms.SplitContainer();
            this.tbctrl_main = new System.Windows.Forms.TabControl();
            this.tbp_TD = new System.Windows.Forms.TabPage();
            this.sc_TD = new System.Windows.Forms.SplitContainer();
            this.gb_TD_attributes = new System.Windows.Forms.GroupBox();
            this.lb_TD_CurAntA = new System.Windows.Forms.Label();
            this.lb_TD_CurAntA1 = new System.Windows.Forms.Label();
            this.lb_TD_MaxDelay = new System.Windows.Forms.Label();
            this.lb_TD_AlDisc = new System.Windows.Forms.Label();
            this.lb_TD_MaxDelay1 = new System.Windows.Forms.Label();
            this.lb_TD_AlDisc1 = new System.Windows.Forms.Label();
            this.lb_TD_sigCollected = new System.Windows.Forms.Label();
            this.lb_TD_sigCollected1 = new System.Windows.Forms.Label();
            this.lb_TD_alignment = new System.Windows.Forms.Label();
            this.lb_TD_align1 = new System.Windows.Forms.Label();
            this.lb_TD_WfmNum = new System.Windows.Forms.Label();
            this.lb_TD_acqnum = new System.Windows.Forms.Label();
            this.gb_TD_Suffix = new System.Windows.Forms.GroupBox();
            this.nud_TD_counter = new System.Windows.Forms.NumericUpDown();
            this.lb_TD_counter = new System.Windows.Forms.Label();
            this.cb_TD_CPP3 = new System.Windows.Forms.CheckBox();
            this.cb_TD_CPP2 = new System.Windows.Forms.CheckBox();
            this.cb_TD_CPP1 = new System.Windows.Forms.CheckBox();
            this.tbx_TD_Suff3 = new System.Windows.Forms.TextBox();
            this.tbx_TD_Suff2 = new System.Windows.Forms.TextBox();
            this.tbx_TD_Suff1 = new System.Windows.Forms.TextBox();
            this.rb_TD_Suff3 = new System.Windows.Forms.RadioButton();
            this.rb_TD_Suff2 = new System.Windows.Forms.RadioButton();
            this.rb_TD_Suff1 = new System.Windows.Forms.RadioButton();
            this.gb_TD_SamplingRate = new System.Windows.Forms.GroupBox();
            this.rb_TD_200GHz = new System.Windows.Forms.RadioButton();
            this.rb_TD_80GHz = new System.Windows.Forms.RadioButton();
            this.gb_TD_Filtering = new System.Windows.Forms.GroupBox();
            this.rb_TD_FltNone = new System.Windows.Forms.RadioButton();
            this.rb_TD_FltBP = new System.Windows.Forms.RadioButton();
            this.rb_TD_FltLP = new System.Windows.Forms.RadioButton();
            this.gb_TD_fullArray = new System.Windows.Forms.GroupBox();
            this.lb_TD_numsigcol = new System.Windows.Forms.Label();
            this.nud_TD_NCollect = new System.Windows.Forms.NumericUpDown();
            this.bn_TD_fullArray = new System.Windows.Forms.Button();
            this.nud_TD_StartCaseNum = new System.Windows.Forms.NumericUpDown();
            this.cb_TD_IncrementalSave = new System.Windows.Forms.CheckBox();
            this.bn_TD_Collect = new System.Windows.Forms.Button();
            this.lb_TD_start = new System.Windows.Forms.Label();
            this.lb_TD_cases = new System.Windows.Forms.Label();
            this.bn_TD_MemSig = new System.Windows.Forms.Button();
            this.nud_TD_NumCases = new System.Windows.Forms.NumericUpDown();
            this.bn_TD_realTime = new System.Windows.Forms.Button();
            this.bn_TD_single = new System.Windows.Forms.Button();
            this.gb_TD_save = new System.Windows.Forms.GroupBox();
            this.lb_TD_fileNameOptions = new System.Windows.Forms.Label();
            this.tbx_TD_folder = new System.Windows.Forms.TextBox();
            this.bn_TD_SelFolderA = new System.Windows.Forms.Button();
            this.lb_TD_Folder = new System.Windows.Forms.Label();
            this.lb_TD_fileName = new System.Windows.Forms.Label();
            this.tbx_TD_fileName = new System.Windows.Forms.TextBox();
            this.gb_TD_Averaging = new System.Windows.Forms.GroupBox();
            this.lb_TD_storeplot = new System.Windows.Forms.Label();
            this.lb_TD_saving = new System.Windows.Forms.Label();
            this.rb_TD_NoAvg = new System.Windows.Forms.RadioButton();
            this.gb_TD_collecting = new System.Windows.Forms.GroupBox();
            this.rb_TD_CollectSig = new System.Windows.Forms.RadioButton();
            this.cb_TD_Store = new System.Windows.Forms.CheckBox();
            this.rb_TD_CollectBL = new System.Windows.Forms.RadioButton();
            this.rb_TD_SWAvg = new System.Windows.Forms.RadioButton();
            this.nud_TD_NAvgSW = new System.Windows.Forms.NumericUpDown();
            this.cb_TD_NoAvg = new System.Windows.Forms.CheckBox();
            this.nud_TD_NAvgHW = new System.Windows.Forms.NumericUpDown();
            this.rb_TD_HWAvg = new System.Windows.Forms.RadioButton();
            this.cb_TD_SWAvg = new System.Windows.Forms.CheckBox();
            this.cb_TD_HWAvg = new System.Windows.Forms.CheckBox();
            this.gb_TD_Calibration = new System.Windows.Forms.GroupBox();
            this.rb_TD_CalNone = new System.Windows.Forms.RadioButton();
            this.rb_TD_CalRef = new System.Windows.Forms.RadioButton();
            this.rb_TD_CalCorr = new System.Windows.Forms.RadioButton();
            this.legend_TD = new NationalInstruments.UI.WindowsForms.Legend();
            this.legend_TD_item1 = new NationalInstruments.UI.LegendItem();
            this.Baseline = new NationalInstruments.UI.ScatterPlot();
            this.sg_TD_Xaxis = new NationalInstruments.UI.XAxis();
            this.sg_TD_Yaxis_sig = new NationalInstruments.UI.YAxis();
            this.legend_TD_item2 = new NationalInstruments.UI.LegendItem();
            this.Signal = new NationalInstruments.UI.ScatterPlot();
            this.legend_TD_item3 = new NationalInstruments.UI.LegendItem();
            this.Difference = new NationalInstruments.UI.ScatterPlot();
            this.sg_TD_Yaxis_diff = new NationalInstruments.UI.YAxis();
            this.legend_TD_item4 = new NationalInstruments.UI.LegendItem();
            this.SavedDifference = new NationalInstruments.UI.ScatterPlot();
            this.sg_TD_graph = new NationalInstruments.UI.WindowsForms.ScatterGraph();
            this.xyCursor1 = new NationalInstruments.UI.XYCursor();
            this.tbp_PS9201A = new System.Windows.Forms.TabPage();
            this.gb_PS_picoscope = new System.Windows.Forms.GroupBox();
            this.bn_PS_connect = new System.Windows.Forms.Button();
            this.lb_PS = new System.Windows.Forms.Label();
            this.tbx_PS_conFile = new System.Windows.Forms.TextBox();
            this.bn_PS_browseConFile = new System.Windows.Forms.Button();
            this.bn_PS_configure = new System.Windows.Forms.Button();
            this.tbp_SM = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.gb_SM_TXA = new System.Windows.Forms.GroupBox();
            this.nud_SM_TXA_del = new System.Windows.Forms.NumericUpDown();
            this.bn_SM_TXA_loop = new System.Windows.Forms.Button();
            this.lb_SM_TXA_loop = new System.Windows.Forms.Label();
            this.nud_SM_TXA_num = new System.Windows.Forms.NumericUpDown();
            this.bn_SM_TXA_set = new System.Windows.Forms.Button();
            this.lb_SM_TXA = new System.Windows.Forms.Label();
            this.gb_SM_RXA = new System.Windows.Forms.GroupBox();
            this.nud_SM_RXA_del = new System.Windows.Forms.NumericUpDown();
            this.bn_SM_RXA_loop = new System.Windows.Forms.Button();
            this.lb_SM_RXA_loop = new System.Windows.Forms.Label();
            this.nud_SM_RXA_num = new System.Windows.Forms.NumericUpDown();
            this.bn_SM_RXA_set = new System.Windows.Forms.Button();
            this.lb_SM_RXA = new System.Windows.Forms.Label();
            this.bn_SM_Power = new System.Windows.Forms.Button();
            this.tbp_service = new System.Windows.Forms.TabPage();
            this.tbx_serv_folder = new System.Windows.Forms.TextBox();
            this.Full_Scan_gb_Service = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.Full_Scan_Delay_Service = new System.Windows.Forms.NumericUpDown();
            this.Cancel_Full_Scan_Service = new System.Windows.Forms.Button();
            this.Pause_Or_Resume_Full_Scan_Service = new System.Windows.Forms.Button();
            this.Initialize_Full_Scan_Service = new System.Windows.Forms.Button();
            this.lb_serv_folder = new System.Windows.Forms.Label();
            this.lb_serv_fileName2 = new System.Windows.Forms.Label();
            this.tbx_serv_fileName = new System.Windows.Forms.TextBox();
            this.xyCursor3 = new NationalInstruments.UI.XYCursor();
            this.xyCursor4 = new NationalInstruments.UI.XYCursor();
            this.Manual_SW5_set = new System.Windows.Forms.Button();
            this.Manual_SW5_nud = new System.Windows.Forms.NumericUpDown();
            this.lb_SM_pos3 = new System.Windows.Forms.Label();
            this.fbd_TD = new System.Windows.Forms.FolderBrowserDialog();
            this.fbd_load = new System.Windows.Forms.FolderBrowserDialog();
            this.ofd_loadTD = new System.Windows.Forms.OpenFileDialog();
            this.bgw_load_FD_BL = new System.ComponentModel.BackgroundWorker();
            this.bgw_load_FD_Sig = new System.ComponentModel.BackgroundWorker();
            this.ms_mainMenu.SuspendLayout();
            this.ss_main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sc_main)).BeginInit();
            this.sc_main.Panel1.SuspendLayout();
            this.sc_main.Panel2.SuspendLayout();
            this.sc_main.SuspendLayout();
            this.tbctrl_main.SuspendLayout();
            this.tbp_TD.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sc_TD)).BeginInit();
            this.sc_TD.Panel1.SuspendLayout();
            this.sc_TD.Panel2.SuspendLayout();
            this.sc_TD.SuspendLayout();
            this.gb_TD_attributes.SuspendLayout();
            this.gb_TD_Suffix.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TD_counter)).BeginInit();
            this.gb_TD_SamplingRate.SuspendLayout();
            this.gb_TD_Filtering.SuspendLayout();
            this.gb_TD_fullArray.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TD_NCollect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TD_StartCaseNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TD_NumCases)).BeginInit();
            this.gb_TD_save.SuspendLayout();
            this.gb_TD_Averaging.SuspendLayout();
            this.gb_TD_collecting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TD_NAvgSW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TD_NAvgHW)).BeginInit();
            this.gb_TD_Calibration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.legend_TD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sg_TD_graph)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xyCursor1)).BeginInit();
            this.tbp_PS9201A.SuspendLayout();
            this.gb_PS_picoscope.SuspendLayout();
            this.tbp_SM.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gb_SM_TXA.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SM_TXA_del)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SM_TXA_num)).BeginInit();
            this.gb_SM_RXA.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SM_RXA_del)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SM_RXA_num)).BeginInit();
            this.Full_Scan_gb_Service.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Full_Scan_Delay_Service)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xyCursor3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xyCursor4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Manual_SW5_nud)).BeginInit();
            this.SuspendLayout();
            // 
            // ms_mainMenu
            // 
            this.ms_mainMenu.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ms_mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_file});
            this.ms_mainMenu.Location = new System.Drawing.Point(0, 0);
            this.ms_mainMenu.Name = "ms_mainMenu";
            this.ms_mainMenu.Size = new System.Drawing.Size(1209, 24);
            this.ms_mainMenu.TabIndex = 0;
            this.ms_mainMenu.Text = "menuStrip1";
            // 
            // tsmi_file
            // 
            this.tsmi_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_file_exit});
            this.tsmi_file.Name = "tsmi_file";
            this.tsmi_file.Size = new System.Drawing.Size(37, 20);
            this.tsmi_file.Text = "File";
            // 
            // tsmi_file_exit
            // 
            this.tsmi_file_exit.Name = "tsmi_file_exit";
            this.tsmi_file_exit.Size = new System.Drawing.Size(92, 22);
            this.tsmi_file_exit.Text = "Exit";
            this.tsmi_file_exit.Click += new System.EventHandler(this.tsmi_file_exit_Click);
            // 
            // tsmi_help_view
            // 
            this.tsmi_help_view.Name = "tsmi_help_view";
            this.tsmi_help_view.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmi_graph_Baseline
            // 
            this.tsmi_graph_Baseline.Name = "tsmi_graph_Baseline";
            this.tsmi_graph_Baseline.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmi_graph_Signal
            // 
            this.tsmi_graph_Signal.Name = "tsmi_graph_Signal";
            this.tsmi_graph_Signal.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmi_graph_Response
            // 
            this.tsmi_graph_Response.Name = "tsmi_graph_Response";
            this.tsmi_graph_Response.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmi_graph_MemDiff
            // 
            this.tsmi_graph_MemDiff.Name = "tsmi_graph_MemDiff";
            this.tsmi_graph_MemDiff.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmi_graph_Display
            // 
            this.tsmi_graph_Display.Name = "tsmi_graph_Display";
            this.tsmi_graph_Display.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmi_graph_Display_Default
            // 
            this.tsmi_graph_Display_Default.Name = "tsmi_graph_Display_Default";
            this.tsmi_graph_Display_Default.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmi_graph_Display_Dual
            // 
            this.tsmi_graph_Display_Dual.Name = "tsmi_graph_Display_Dual";
            this.tsmi_graph_Display_Dual.Size = new System.Drawing.Size(32, 19);
            // 
            // ofd_PS_browse
            // 
            this.ofd_PS_browse.FileName = "ofd_PS_browse";
            // 
            // lv_msgLog
            // 
            this.lv_msgLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ch_time,
            this.ch_event});
            this.lv_msgLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lv_msgLog.GridLines = true;
            this.lv_msgLog.Location = new System.Drawing.Point(0, 0);
            this.lv_msgLog.Name = "lv_msgLog";
            this.lv_msgLog.Size = new System.Drawing.Size(1207, 94);
            this.lv_msgLog.TabIndex = 2;
            this.lv_msgLog.UseCompatibleStateImageBehavior = false;
            this.lv_msgLog.View = System.Windows.Forms.View.Details;
            // 
            // ch_time
            // 
            this.ch_time.Text = "Time";
            this.ch_time.Width = 138;
            // 
            // ch_event
            // 
            this.ch_event.Text = "Event";
            this.ch_event.Width = 1109;
            // 
            // ss_main
            // 
            this.ss_main.BackColor = System.Drawing.SystemColors.Menu;
            this.ss_main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssl_connectionStatus,
            this.tssl_device,
            this.tssl_statusSM,
            this.tssl_SM,
            this.tssl_TX,
            this.tssl_TXnum,
            this.tssl_RX,
            this.tssl_RXnum,
            this.tssl_running,
            this.pb_status});
            this.ss_main.Location = new System.Drawing.Point(0, 720);
            this.ss_main.Name = "ss_main";
            this.ss_main.Size = new System.Drawing.Size(1209, 22);
            this.ss_main.TabIndex = 4;
            this.ss_main.Text = "statusStrip1";
            // 
            // tssl_connectionStatus
            // 
            this.tssl_connectionStatus.AutoSize = false;
            this.tssl_connectionStatus.BackColor = System.Drawing.SystemColors.Menu;
            this.tssl_connectionStatus.ForeColor = System.Drawing.SystemColors.Desktop;
            this.tssl_connectionStatus.Name = "tssl_connectionStatus";
            this.tssl_connectionStatus.Size = new System.Drawing.Size(107, 17);
            this.tssl_connectionStatus.Text = "Connection Status:";
            // 
            // tssl_device
            // 
            this.tssl_device.AutoSize = false;
            this.tssl_device.BackColor = System.Drawing.SystemColors.Menu;
            this.tssl_device.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tssl_device.ForeColor = System.Drawing.Color.Black;
            this.tssl_device.Name = "tssl_device";
            this.tssl_device.Size = new System.Drawing.Size(90, 17);
            this.tssl_device.Tag = "";
            this.tssl_device.Text = "Device";
            // 
            // tssl_statusSM
            // 
            this.tssl_statusSM.AutoSize = false;
            this.tssl_statusSM.BackColor = System.Drawing.SystemColors.Menu;
            this.tssl_statusSM.ForeColor = System.Drawing.SystemColors.Desktop;
            this.tssl_statusSM.Name = "tssl_statusSM";
            this.tssl_statusSM.Size = new System.Drawing.Size(62, 17);
            this.tssl_statusSM.Text = "SM Status:";
            // 
            // tssl_SM
            // 
            this.tssl_SM.AutoSize = false;
            this.tssl_SM.BackColor = System.Drawing.SystemColors.Menu;
            this.tssl_SM.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tssl_SM.Name = "tssl_SM";
            this.tssl_SM.Size = new System.Drawing.Size(62, 17);
            this.tssl_SM.Text = "ON/OFF";
            // 
            // tssl_TX
            // 
            this.tssl_TX.AutoSize = false;
            this.tssl_TX.BackColor = System.Drawing.SystemColors.Menu;
            this.tssl_TX.ForeColor = System.Drawing.SystemColors.Desktop;
            this.tssl_TX.Name = "tssl_TX";
            this.tssl_TX.Size = new System.Drawing.Size(27, 17);
            this.tssl_TX.Text = "TX: ";
            // 
            // tssl_TXnum
            // 
            this.tssl_TXnum.AutoSize = false;
            this.tssl_TXnum.BackColor = System.Drawing.SystemColors.Menu;
            this.tssl_TXnum.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tssl_TXnum.Name = "tssl_TXnum";
            this.tssl_TXnum.Size = new System.Drawing.Size(30, 17);
            this.tssl_TXnum.Text = "##";
            // 
            // tssl_RX
            // 
            this.tssl_RX.AutoSize = false;
            this.tssl_RX.BackColor = System.Drawing.SystemColors.Menu;
            this.tssl_RX.ForeColor = System.Drawing.SystemColors.Desktop;
            this.tssl_RX.Name = "tssl_RX";
            this.tssl_RX.Size = new System.Drawing.Size(27, 17);
            this.tssl_RX.Text = "RX: ";
            // 
            // tssl_RXnum
            // 
            this.tssl_RXnum.AutoSize = false;
            this.tssl_RXnum.BackColor = System.Drawing.SystemColors.Menu;
            this.tssl_RXnum.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tssl_RXnum.Name = "tssl_RXnum";
            this.tssl_RXnum.Size = new System.Drawing.Size(30, 17);
            this.tssl_RXnum.Text = "##";
            // 
            // tssl_running
            // 
            this.tssl_running.AutoSize = false;
            this.tssl_running.BackColor = System.Drawing.SystemColors.Menu;
            this.tssl_running.Name = "tssl_running";
            this.tssl_running.Size = new System.Drawing.Size(68, 17);
            this.tssl_running.Text = "Running TD";
            // 
            // pb_status
            // 
            this.pb_status.AutoSize = false;
            this.pb_status.Name = "pb_status";
            this.pb_status.Size = new System.Drawing.Size(100, 16);
            this.pb_status.MouseHover += new System.EventHandler(this.pb_status_MouseHover);
            // 
            // bgw_TD_realTime
            // 
            this.bgw_TD_realTime.WorkerReportsProgress = true;
            this.bgw_TD_realTime.WorkerSupportsCancellation = true;
            this.bgw_TD_realTime.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_realTime_DoWork);
            this.bgw_TD_realTime.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_TD_realTime_ProgressChanged);
            this.bgw_TD_realTime.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_TD_realTime_RunWorkerCompleted);
            // 
            // bgw_TD_collectAuto
            // 
            this.bgw_TD_collectAuto.WorkerReportsProgress = true;
            this.bgw_TD_collectAuto.WorkerSupportsCancellation = true;
            this.bgw_TD_collectAuto.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_TD_collectAuto_DoWork);
            this.bgw_TD_collectAuto.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_TD_collectAuto_ProgressChanged);
            this.bgw_TD_collectAuto.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_TD_collectAuto_RunWorkerCompleted);
            // 
            // sc_main
            // 
            this.sc_main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sc_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sc_main.Location = new System.Drawing.Point(0, 24);
            this.sc_main.Name = "sc_main";
            this.sc_main.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // sc_main.Panel1
            // 
            this.sc_main.Panel1.Controls.Add(this.tbctrl_main);
            // 
            // sc_main.Panel2
            // 
            this.sc_main.Panel2.Controls.Add(this.lv_msgLog);
            this.sc_main.Size = new System.Drawing.Size(1209, 696);
            this.sc_main.SplitterDistance = 596;
            this.sc_main.TabIndex = 5;
            // 
            // tbctrl_main
            // 
            this.tbctrl_main.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tbctrl_main.Controls.Add(this.tbp_TD);
            this.tbctrl_main.Controls.Add(this.tbp_PS9201A);
            this.tbctrl_main.Controls.Add(this.tbp_SM);
            this.tbctrl_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbctrl_main.Location = new System.Drawing.Point(0, 0);
            this.tbctrl_main.Name = "tbctrl_main";
            this.tbctrl_main.SelectedIndex = 0;
            this.tbctrl_main.Size = new System.Drawing.Size(1207, 594);
            this.tbctrl_main.TabIndex = 1;
            // 
            // tbp_TD
            // 
            this.tbp_TD.Controls.Add(this.sc_TD);
            this.tbp_TD.Location = new System.Drawing.Point(4, 25);
            this.tbp_TD.Name = "tbp_TD";
            this.tbp_TD.Padding = new System.Windows.Forms.Padding(3);
            this.tbp_TD.Size = new System.Drawing.Size(1199, 565);
            this.tbp_TD.TabIndex = 0;
            this.tbp_TD.Text = "Time Domain";
            this.tbp_TD.UseVisualStyleBackColor = true;
            // 
            // sc_TD
            // 
            this.sc_TD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sc_TD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sc_TD.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.sc_TD.Location = new System.Drawing.Point(3, 3);
            this.sc_TD.Name = "sc_TD";
            // 
            // sc_TD.Panel1
            // 
            this.sc_TD.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.sc_TD.Panel1.Controls.Add(this.gb_TD_attributes);
            this.sc_TD.Panel1.Controls.Add(this.gb_TD_Suffix);
            this.sc_TD.Panel1.Controls.Add(this.gb_TD_SamplingRate);
            this.sc_TD.Panel1.Controls.Add(this.gb_TD_Filtering);
            this.sc_TD.Panel1.Controls.Add(this.gb_TD_fullArray);
            this.sc_TD.Panel1.Controls.Add(this.gb_TD_save);
            this.sc_TD.Panel1.Controls.Add(this.gb_TD_Averaging);
            this.sc_TD.Panel1.Controls.Add(this.gb_TD_Calibration);
            // 
            // sc_TD.Panel2
            // 
            this.sc_TD.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.sc_TD.Panel2.Controls.Add(this.legend_TD);
            this.sc_TD.Panel2.Controls.Add(this.sg_TD_graph);
            this.sc_TD.Size = new System.Drawing.Size(1193, 559);
            this.sc_TD.SplitterDistance = 418;
            this.sc_TD.TabIndex = 0;
            // 
            // gb_TD_attributes
            // 
            this.gb_TD_attributes.Controls.Add(this.lb_TD_CurAntA);
            this.gb_TD_attributes.Controls.Add(this.lb_TD_CurAntA1);
            this.gb_TD_attributes.Controls.Add(this.lb_TD_MaxDelay);
            this.gb_TD_attributes.Controls.Add(this.lb_TD_AlDisc);
            this.gb_TD_attributes.Controls.Add(this.lb_TD_MaxDelay1);
            this.gb_TD_attributes.Controls.Add(this.lb_TD_AlDisc1);
            this.gb_TD_attributes.Controls.Add(this.lb_TD_sigCollected);
            this.gb_TD_attributes.Controls.Add(this.lb_TD_sigCollected1);
            this.gb_TD_attributes.Controls.Add(this.lb_TD_alignment);
            this.gb_TD_attributes.Controls.Add(this.lb_TD_align1);
            this.gb_TD_attributes.Controls.Add(this.lb_TD_WfmNum);
            this.gb_TD_attributes.Controls.Add(this.lb_TD_acqnum);
            this.gb_TD_attributes.Location = new System.Drawing.Point(5, 337);
            this.gb_TD_attributes.Name = "gb_TD_attributes";
            this.gb_TD_attributes.Size = new System.Drawing.Size(209, 216);
            this.gb_TD_attributes.TabIndex = 92;
            this.gb_TD_attributes.TabStop = false;
            this.gb_TD_attributes.Text = "Information";
            // 
            // lb_TD_CurAntA
            // 
            this.lb_TD_CurAntA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_TD_CurAntA.Location = new System.Drawing.Point(135, 53);
            this.lb_TD_CurAntA.Name = "lb_TD_CurAntA";
            this.lb_TD_CurAntA.Size = new System.Drawing.Size(68, 17);
            this.lb_TD_CurAntA.TabIndex = 89;
            this.lb_TD_CurAntA.Text = "---";
            // 
            // lb_TD_CurAntA1
            // 
            this.lb_TD_CurAntA1.AutoSize = true;
            this.lb_TD_CurAntA1.Location = new System.Drawing.Point(2, 54);
            this.lb_TD_CurAntA1.Name = "lb_TD_CurAntA1";
            this.lb_TD_CurAntA1.Size = new System.Drawing.Size(74, 13);
            this.lb_TD_CurAntA1.TabIndex = 88;
            this.lb_TD_CurAntA1.Text = "Signal plotted:";
            // 
            // lb_TD_MaxDelay
            // 
            this.lb_TD_MaxDelay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_TD_MaxDelay.Location = new System.Drawing.Point(135, 150);
            this.lb_TD_MaxDelay.Name = "lb_TD_MaxDelay";
            this.lb_TD_MaxDelay.Size = new System.Drawing.Size(68, 15);
            this.lb_TD_MaxDelay.TabIndex = 87;
            this.lb_TD_MaxDelay.Text = "---";
            // 
            // lb_TD_AlDisc
            // 
            this.lb_TD_AlDisc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_TD_AlDisc.Location = new System.Drawing.Point(135, 126);
            this.lb_TD_AlDisc.Name = "lb_TD_AlDisc";
            this.lb_TD_AlDisc.Size = new System.Drawing.Size(68, 15);
            this.lb_TD_AlDisc.TabIndex = 86;
            this.lb_TD_AlDisc.Text = "---";
            // 
            // lb_TD_MaxDelay1
            // 
            this.lb_TD_MaxDelay1.AutoSize = true;
            this.lb_TD_MaxDelay1.Location = new System.Drawing.Point(2, 150);
            this.lb_TD_MaxDelay1.Name = "lb_TD_MaxDelay1";
            this.lb_TD_MaxDelay1.Size = new System.Drawing.Size(99, 13);
            this.lb_TD_MaxDelay1.TabIndex = 85;
            this.lb_TD_MaxDelay1.Text = "Maximum delay, ps:";
            // 
            // lb_TD_AlDisc1
            // 
            this.lb_TD_AlDisc1.AutoSize = true;
            this.lb_TD_AlDisc1.Location = new System.Drawing.Point(3, 128);
            this.lb_TD_AlDisc1.Name = "lb_TD_AlDisc1";
            this.lb_TD_AlDisc1.Size = new System.Drawing.Size(129, 13);
            this.lb_TD_AlDisc1.TabIndex = 84;
            this.lb_TD_AlDisc1.Text = "Aligned signals discarded:";
            // 
            // lb_TD_sigCollected
            // 
            this.lb_TD_sigCollected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_TD_sigCollected.Location = new System.Drawing.Point(135, 78);
            this.lb_TD_sigCollected.Name = "lb_TD_sigCollected";
            this.lb_TD_sigCollected.Size = new System.Drawing.Size(68, 16);
            this.lb_TD_sigCollected.TabIndex = 83;
            this.lb_TD_sigCollected.Text = "---";
            // 
            // lb_TD_sigCollected1
            // 
            this.lb_TD_sigCollected1.AutoSize = true;
            this.lb_TD_sigCollected1.Location = new System.Drawing.Point(3, 81);
            this.lb_TD_sigCollected1.Name = "lb_TD_sigCollected1";
            this.lb_TD_sigCollected1.Size = new System.Drawing.Size(90, 13);
            this.lb_TD_sigCollected1.TabIndex = 82;
            this.lb_TD_sigCollected1.Text = "Signals collected:";
            // 
            // lb_TD_alignment
            // 
            this.lb_TD_alignment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_TD_alignment.Location = new System.Drawing.Point(135, 104);
            this.lb_TD_alignment.Name = "lb_TD_alignment";
            this.lb_TD_alignment.Size = new System.Drawing.Size(68, 16);
            this.lb_TD_alignment.TabIndex = 81;
            this.lb_TD_alignment.Text = "---";
            // 
            // lb_TD_align1
            // 
            this.lb_TD_align1.AutoSize = true;
            this.lb_TD_align1.Location = new System.Drawing.Point(2, 105);
            this.lb_TD_align1.Name = "lb_TD_align1";
            this.lb_TD_align1.Size = new System.Drawing.Size(73, 13);
            this.lb_TD_align1.TabIndex = 80;
            this.lb_TD_align1.Text = "Alignment, ps:";
            // 
            // lb_TD_WfmNum
            // 
            this.lb_TD_WfmNum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_TD_WfmNum.Location = new System.Drawing.Point(135, 30);
            this.lb_TD_WfmNum.Name = "lb_TD_WfmNum";
            this.lb_TD_WfmNum.Size = new System.Drawing.Size(68, 16);
            this.lb_TD_WfmNum.TabIndex = 79;
            this.lb_TD_WfmNum.Text = "---";
            // 
            // lb_TD_acqnum
            // 
            this.lb_TD_acqnum.AutoSize = true;
            this.lb_TD_acqnum.Location = new System.Drawing.Point(2, 30);
            this.lb_TD_acqnum.Name = "lb_TD_acqnum";
            this.lb_TD_acqnum.Size = new System.Drawing.Size(99, 13);
            this.lb_TD_acqnum.TabIndex = 78;
            this.lb_TD_acqnum.Text = "Acquisition number:";
            // 
            // gb_TD_Suffix
            // 
            this.gb_TD_Suffix.Controls.Add(this.nud_TD_counter);
            this.gb_TD_Suffix.Controls.Add(this.lb_TD_counter);
            this.gb_TD_Suffix.Controls.Add(this.cb_TD_CPP3);
            this.gb_TD_Suffix.Controls.Add(this.cb_TD_CPP2);
            this.gb_TD_Suffix.Controls.Add(this.cb_TD_CPP1);
            this.gb_TD_Suffix.Controls.Add(this.tbx_TD_Suff3);
            this.gb_TD_Suffix.Controls.Add(this.tbx_TD_Suff2);
            this.gb_TD_Suffix.Controls.Add(this.tbx_TD_Suff1);
            this.gb_TD_Suffix.Controls.Add(this.rb_TD_Suff3);
            this.gb_TD_Suffix.Controls.Add(this.rb_TD_Suff2);
            this.gb_TD_Suffix.Controls.Add(this.rb_TD_Suff1);
            this.gb_TD_Suffix.Location = new System.Drawing.Point(3, 102);
            this.gb_TD_Suffix.Name = "gb_TD_Suffix";
            this.gb_TD_Suffix.Size = new System.Drawing.Size(393, 65);
            this.gb_TD_Suffix.TabIndex = 21;
            this.gb_TD_Suffix.TabStop = false;
            this.gb_TD_Suffix.Text = "Suffix (replacement for %s%) ";
            // 
            // nud_TD_counter
            // 
            this.nud_TD_counter.Location = new System.Drawing.Point(239, 19);
            this.nud_TD_counter.Name = "nud_TD_counter";
            this.nud_TD_counter.Size = new System.Drawing.Size(38, 20);
            this.nud_TD_counter.TabIndex = 81;
            this.nud_TD_counter.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nud_TD_counter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lb_TD_counter
            // 
            this.lb_TD_counter.AutoSize = true;
            this.lb_TD_counter.Location = new System.Drawing.Point(186, 21);
            this.lb_TD_counter.Name = "lb_TD_counter";
            this.lb_TD_counter.Size = new System.Drawing.Size(47, 13);
            this.lb_TD_counter.TabIndex = 80;
            this.lb_TD_counter.Text = "Counter:";
            this.lb_TD_counter.MouseHover += new System.EventHandler(this.lb_TD_counter_MouseHover);
            // 
            // cb_TD_CPP3
            // 
            this.cb_TD_CPP3.AutoSize = true;
            this.cb_TD_CPP3.Checked = true;
            this.cb_TD_CPP3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TD_CPP3.Location = new System.Drawing.Point(127, 43);
            this.cb_TD_CPP3.Name = "cb_TD_CPP3";
            this.cb_TD_CPP3.Size = new System.Drawing.Size(45, 17);
            this.cb_TD_CPP3.TabIndex = 22;
            this.cb_TD_CPP3.Text = "C++";
            this.cb_TD_CPP3.UseVisualStyleBackColor = true;
            // 
            // cb_TD_CPP2
            // 
            this.cb_TD_CPP2.AutoSize = true;
            this.cb_TD_CPP2.Location = new System.Drawing.Point(71, 44);
            this.cb_TD_CPP2.Name = "cb_TD_CPP2";
            this.cb_TD_CPP2.Size = new System.Drawing.Size(45, 17);
            this.cb_TD_CPP2.TabIndex = 21;
            this.cb_TD_CPP2.Text = "C++";
            this.cb_TD_CPP2.UseVisualStyleBackColor = true;
            // 
            // cb_TD_CPP1
            // 
            this.cb_TD_CPP1.AutoSize = true;
            this.cb_TD_CPP1.Location = new System.Drawing.Point(12, 44);
            this.cb_TD_CPP1.Name = "cb_TD_CPP1";
            this.cb_TD_CPP1.Size = new System.Drawing.Size(45, 17);
            this.cb_TD_CPP1.TabIndex = 20;
            this.cb_TD_CPP1.Text = "C++";
            this.cb_TD_CPP1.UseVisualStyleBackColor = true;
            // 
            // tbx_TD_Suff3
            // 
            this.tbx_TD_Suff3.Location = new System.Drawing.Point(145, 18);
            this.tbx_TD_Suff3.Name = "tbx_TD_Suff3";
            this.tbx_TD_Suff3.Size = new System.Drawing.Size(22, 20);
            this.tbx_TD_Suff3.TabIndex = 19;
            this.tbx_TD_Suff3.Text = "f";
            // 
            // tbx_TD_Suff2
            // 
            this.tbx_TD_Suff2.Location = new System.Drawing.Point(89, 18);
            this.tbx_TD_Suff2.Name = "tbx_TD_Suff2";
            this.tbx_TD_Suff2.Size = new System.Drawing.Size(22, 20);
            this.tbx_TD_Suff2.TabIndex = 18;
            this.tbx_TD_Suff2.Text = "t";
            // 
            // tbx_TD_Suff1
            // 
            this.tbx_TD_Suff1.Location = new System.Drawing.Point(30, 19);
            this.tbx_TD_Suff1.Name = "tbx_TD_Suff1";
            this.tbx_TD_Suff1.Size = new System.Drawing.Size(22, 20);
            this.tbx_TD_Suff1.TabIndex = 17;
            this.tbx_TD_Suff1.Text = "bl";
            // 
            // rb_TD_Suff3
            // 
            this.rb_TD_Suff3.AutoSize = true;
            this.rb_TD_Suff3.Location = new System.Drawing.Point(127, 21);
            this.rb_TD_Suff3.Name = "rb_TD_Suff3";
            this.rb_TD_Suff3.Size = new System.Drawing.Size(14, 13);
            this.rb_TD_Suff3.TabIndex = 2;
            this.rb_TD_Suff3.UseVisualStyleBackColor = true;
            // 
            // rb_TD_Suff2
            // 
            this.rb_TD_Suff2.AutoSize = true;
            this.rb_TD_Suff2.Location = new System.Drawing.Point(71, 21);
            this.rb_TD_Suff2.Name = "rb_TD_Suff2";
            this.rb_TD_Suff2.Size = new System.Drawing.Size(14, 13);
            this.rb_TD_Suff2.TabIndex = 1;
            this.rb_TD_Suff2.UseVisualStyleBackColor = true;
            // 
            // rb_TD_Suff1
            // 
            this.rb_TD_Suff1.AutoSize = true;
            this.rb_TD_Suff1.Checked = true;
            this.rb_TD_Suff1.Location = new System.Drawing.Point(12, 21);
            this.rb_TD_Suff1.Name = "rb_TD_Suff1";
            this.rb_TD_Suff1.Size = new System.Drawing.Size(14, 13);
            this.rb_TD_Suff1.TabIndex = 0;
            this.rb_TD_Suff1.TabStop = true;
            this.rb_TD_Suff1.UseVisualStyleBackColor = true;
            // 
            // gb_TD_SamplingRate
            // 
            this.gb_TD_SamplingRate.Controls.Add(this.rb_TD_200GHz);
            this.gb_TD_SamplingRate.Controls.Add(this.rb_TD_80GHz);
            this.gb_TD_SamplingRate.Location = new System.Drawing.Point(265, 261);
            this.gb_TD_SamplingRate.Name = "gb_TD_SamplingRate";
            this.gb_TD_SamplingRate.Size = new System.Drawing.Size(131, 73);
            this.gb_TD_SamplingRate.TabIndex = 53;
            this.gb_TD_SamplingRate.TabStop = false;
            this.gb_TD_SamplingRate.Text = "Sampling rate";
            // 
            // rb_TD_200GHz
            // 
            this.rb_TD_200GHz.AutoSize = true;
            this.rb_TD_200GHz.Location = new System.Drawing.Point(9, 30);
            this.rb_TD_200GHz.Name = "rb_TD_200GHz";
            this.rb_TD_200GHz.Size = new System.Drawing.Size(67, 17);
            this.rb_TD_200GHz.TabIndex = 1;
            this.rb_TD_200GHz.Text = "200 GHz";
            this.rb_TD_200GHz.UseVisualStyleBackColor = true;
            // 
            // rb_TD_80GHz
            // 
            this.rb_TD_80GHz.AutoSize = true;
            this.rb_TD_80GHz.Checked = true;
            this.rb_TD_80GHz.Location = new System.Drawing.Point(9, 14);
            this.rb_TD_80GHz.Name = "rb_TD_80GHz";
            this.rb_TD_80GHz.Size = new System.Drawing.Size(61, 17);
            this.rb_TD_80GHz.TabIndex = 0;
            this.rb_TD_80GHz.TabStop = true;
            this.rb_TD_80GHz.Text = "80 GHz";
            this.rb_TD_80GHz.UseVisualStyleBackColor = true;
            // 
            // gb_TD_Filtering
            // 
            this.gb_TD_Filtering.Controls.Add(this.rb_TD_FltNone);
            this.gb_TD_Filtering.Controls.Add(this.rb_TD_FltBP);
            this.gb_TD_Filtering.Controls.Add(this.rb_TD_FltLP);
            this.gb_TD_Filtering.Location = new System.Drawing.Point(127, 261);
            this.gb_TD_Filtering.Name = "gb_TD_Filtering";
            this.gb_TD_Filtering.Size = new System.Drawing.Size(137, 73);
            this.gb_TD_Filtering.TabIndex = 52;
            this.gb_TD_Filtering.TabStop = false;
            this.gb_TD_Filtering.Text = "Filtering";
            // 
            // rb_TD_FltNone
            // 
            this.rb_TD_FltNone.AutoSize = true;
            this.rb_TD_FltNone.Location = new System.Drawing.Point(9, 46);
            this.rb_TD_FltNone.Name = "rb_TD_FltNone";
            this.rb_TD_FltNone.Size = new System.Drawing.Size(75, 17);
            this.rb_TD_FltNone.TabIndex = 2;
            this.rb_TD_FltNone.Text = "No filtering";
            this.rb_TD_FltNone.UseVisualStyleBackColor = true;
            // 
            // rb_TD_FltBP
            // 
            this.rb_TD_FltBP.AutoSize = true;
            this.rb_TD_FltBP.Location = new System.Drawing.Point(9, 30);
            this.rb_TD_FltBP.Name = "rb_TD_FltBP";
            this.rb_TD_FltBP.Size = new System.Drawing.Size(103, 17);
            this.rb_TD_FltBP.TabIndex = 1;
            this.rb_TD_FltBP.Text = "BP 2GHz...4Ghz";
            this.rb_TD_FltBP.UseVisualStyleBackColor = true;
            // 
            // rb_TD_FltLP
            // 
            this.rb_TD_FltLP.AutoSize = true;
            this.rb_TD_FltLP.Checked = true;
            this.rb_TD_FltLP.Location = new System.Drawing.Point(9, 14);
            this.rb_TD_FltLP.Name = "rb_TD_FltLP";
            this.rb_TD_FltLP.Size = new System.Drawing.Size(77, 17);
            this.rb_TD_FltLP.TabIndex = 0;
            this.rb_TD_FltLP.TabStop = true;
            this.rb_TD_FltLP.Text = "LP < 4GHz";
            this.rb_TD_FltLP.UseVisualStyleBackColor = true;
            // 
            // gb_TD_fullArray
            // 
            this.gb_TD_fullArray.Controls.Add(this.lb_TD_numsigcol);
            this.gb_TD_fullArray.Controls.Add(this.nud_TD_NCollect);
            this.gb_TD_fullArray.Controls.Add(this.bn_TD_fullArray);
            this.gb_TD_fullArray.Controls.Add(this.nud_TD_StartCaseNum);
            this.gb_TD_fullArray.Controls.Add(this.cb_TD_IncrementalSave);
            this.gb_TD_fullArray.Controls.Add(this.bn_TD_Collect);
            this.gb_TD_fullArray.Controls.Add(this.lb_TD_start);
            this.gb_TD_fullArray.Controls.Add(this.lb_TD_cases);
            this.gb_TD_fullArray.Controls.Add(this.bn_TD_MemSig);
            this.gb_TD_fullArray.Controls.Add(this.nud_TD_NumCases);
            this.gb_TD_fullArray.Controls.Add(this.bn_TD_realTime);
            this.gb_TD_fullArray.Controls.Add(this.bn_TD_single);
            this.gb_TD_fullArray.Location = new System.Drawing.Point(214, 337);
            this.gb_TD_fullArray.Name = "gb_TD_fullArray";
            this.gb_TD_fullArray.Size = new System.Drawing.Size(182, 216);
            this.gb_TD_fullArray.TabIndex = 91;
            this.gb_TD_fullArray.TabStop = false;
            // 
            // lb_TD_numsigcol
            // 
            this.lb_TD_numsigcol.AutoSize = true;
            this.lb_TD_numsigcol.Location = new System.Drawing.Point(3, 161);
            this.lb_TD_numsigcol.Name = "lb_TD_numsigcol";
            this.lb_TD_numsigcol.Size = new System.Drawing.Size(90, 13);
            this.lb_TD_numsigcol.TabIndex = 36;
            this.lb_TD_numsigcol.Text = "Signals to collect:";
            // 
            // nud_TD_NCollect
            // 
            this.nud_TD_NCollect.Location = new System.Drawing.Point(52, 177);
            this.nud_TD_NCollect.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nud_TD_NCollect.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_TD_NCollect.Name = "nud_TD_NCollect";
            this.nud_TD_NCollect.Size = new System.Drawing.Size(41, 20);
            this.nud_TD_NCollect.TabIndex = 37;
            this.nud_TD_NCollect.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // bn_TD_fullArray
            // 
            this.bn_TD_fullArray.Enabled = false;
            this.bn_TD_fullArray.Location = new System.Drawing.Point(99, 32);
            this.bn_TD_fullArray.Name = "bn_TD_fullArray";
            this.bn_TD_fullArray.Size = new System.Drawing.Size(79, 21);
            this.bn_TD_fullArray.TabIndex = 82;
            this.bn_TD_fullArray.Text = "Full Array";
            this.bn_TD_fullArray.UseVisualStyleBackColor = true;
            this.bn_TD_fullArray.Click += new System.EventHandler(this.bn_TD_fullArray_Click);
            this.bn_TD_fullArray.MouseHover += new System.EventHandler(this.bn_TD_fullArray_MouseHover);
            // 
            // nud_TD_StartCaseNum
            // 
            this.nud_TD_StartCaseNum.Location = new System.Drawing.Point(140, 106);
            this.nud_TD_StartCaseNum.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nud_TD_StartCaseNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_TD_StartCaseNum.Name = "nud_TD_StartCaseNum";
            this.nud_TD_StartCaseNum.Size = new System.Drawing.Size(38, 20);
            this.nud_TD_StartCaseNum.TabIndex = 90;
            this.nud_TD_StartCaseNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cb_TD_IncrementalSave
            // 
            this.cb_TD_IncrementalSave.AutoSize = true;
            this.cb_TD_IncrementalSave.Location = new System.Drawing.Point(37, 59);
            this.cb_TD_IncrementalSave.Name = "cb_TD_IncrementalSave";
            this.cb_TD_IncrementalSave.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cb_TD_IncrementalSave.Size = new System.Drawing.Size(141, 17);
            this.cb_TD_IncrementalSave.TabIndex = 83;
            this.cb_TD_IncrementalSave.Text = "Incremental case collect";
            this.cb_TD_IncrementalSave.UseVisualStyleBackColor = true;
            // 
            // bn_TD_Collect
            // 
            this.bn_TD_Collect.Enabled = false;
            this.bn_TD_Collect.Location = new System.Drawing.Point(99, 174);
            this.bn_TD_Collect.Name = "bn_TD_Collect";
            this.bn_TD_Collect.Size = new System.Drawing.Size(79, 23);
            this.bn_TD_Collect.TabIndex = 36;
            this.bn_TD_Collect.Text = "Collect";
            this.bn_TD_Collect.UseVisualStyleBackColor = true;
            this.bn_TD_Collect.Click += new System.EventHandler(this.bn_TD_Collect_Click);
            // 
            // lb_TD_start
            // 
            this.lb_TD_start.AutoSize = true;
            this.lb_TD_start.Location = new System.Drawing.Point(96, 108);
            this.lb_TD_start.Name = "lb_TD_start";
            this.lb_TD_start.Size = new System.Drawing.Size(40, 13);
            this.lb_TD_start.TabIndex = 89;
            this.lb_TD_start.Text = "start #:";
            // 
            // lb_TD_cases
            // 
            this.lb_TD_cases.AutoSize = true;
            this.lb_TD_cases.Location = new System.Drawing.Point(76, 84);
            this.lb_TD_cases.Name = "lb_TD_cases";
            this.lb_TD_cases.Size = new System.Drawing.Size(60, 13);
            this.lb_TD_cases.TabIndex = 87;
            this.lb_TD_cases.Text = "# of cases:";
            // 
            // bn_TD_MemSig
            // 
            this.bn_TD_MemSig.Enabled = false;
            this.bn_TD_MemSig.Location = new System.Drawing.Point(99, 140);
            this.bn_TD_MemSig.Name = "bn_TD_MemSig";
            this.bn_TD_MemSig.Size = new System.Drawing.Size(79, 23);
            this.bn_TD_MemSig.TabIndex = 39;
            this.bn_TD_MemSig.Text = "Mem. Signal";
            this.bn_TD_MemSig.UseVisualStyleBackColor = true;
            this.bn_TD_MemSig.Click += new System.EventHandler(this.bn_TD_MemSig_Click);
            // 
            // nud_TD_NumCases
            // 
            this.nud_TD_NumCases.Location = new System.Drawing.Point(140, 82);
            this.nud_TD_NumCases.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nud_TD_NumCases.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nud_TD_NumCases.Name = "nud_TD_NumCases";
            this.nud_TD_NumCases.Size = new System.Drawing.Size(38, 20);
            this.nud_TD_NumCases.TabIndex = 88;
            this.nud_TD_NumCases.Tag = "# of cases:";
            this.nud_TD_NumCases.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // bn_TD_realTime
            // 
            this.bn_TD_realTime.Enabled = false;
            this.bn_TD_realTime.Location = new System.Drawing.Point(14, 5);
            this.bn_TD_realTime.Name = "bn_TD_realTime";
            this.bn_TD_realTime.Size = new System.Drawing.Size(79, 21);
            this.bn_TD_realTime.TabIndex = 80;
            this.bn_TD_realTime.Text = "Real Time";
            this.bn_TD_realTime.UseVisualStyleBackColor = true;
            this.bn_TD_realTime.Click += new System.EventHandler(this.bn_TD_realTime_Click);
            // 
            // bn_TD_single
            // 
            this.bn_TD_single.Enabled = false;
            this.bn_TD_single.Location = new System.Drawing.Point(99, 5);
            this.bn_TD_single.Name = "bn_TD_single";
            this.bn_TD_single.Size = new System.Drawing.Size(79, 21);
            this.bn_TD_single.TabIndex = 81;
            this.bn_TD_single.Text = "Single";
            this.bn_TD_single.UseVisualStyleBackColor = true;
            this.bn_TD_single.Click += new System.EventHandler(this.bn_TD_single_Click);
            // 
            // gb_TD_save
            // 
            this.gb_TD_save.Controls.Add(this.lb_TD_fileNameOptions);
            this.gb_TD_save.Controls.Add(this.tbx_TD_folder);
            this.gb_TD_save.Controls.Add(this.bn_TD_SelFolderA);
            this.gb_TD_save.Controls.Add(this.lb_TD_Folder);
            this.gb_TD_save.Controls.Add(this.lb_TD_fileName);
            this.gb_TD_save.Controls.Add(this.tbx_TD_fileName);
            this.gb_TD_save.Location = new System.Drawing.Point(5, 3);
            this.gb_TD_save.Name = "gb_TD_save";
            this.gb_TD_save.Size = new System.Drawing.Size(391, 164);
            this.gb_TD_save.TabIndex = 79;
            this.gb_TD_save.TabStop = false;
            this.gb_TD_save.Text = "Save Options";
            // 
            // lb_TD_fileNameOptions
            // 
            this.lb_TD_fileNameOptions.AutoSize = true;
            this.lb_TD_fileNameOptions.Location = new System.Drawing.Point(21, 77);
            this.lb_TD_fileNameOptions.Name = "lb_TD_fileNameOptions";
            this.lb_TD_fileNameOptions.Size = new System.Drawing.Size(358, 13);
            this.lb_TD_fileNameOptions.TabIndex = 82;
            this.lb_TD_fileNameOptions.Text = "e.g. %TX% is transmitter, %RX% is receiver // %c% is counter, %s% is suffix";
            // 
            // tbx_TD_folder
            // 
            this.tbx_TD_folder.Location = new System.Drawing.Point(43, 19);
            this.tbx_TD_folder.Name = "tbx_TD_folder";
            this.tbx_TD_folder.Size = new System.Drawing.Size(269, 20);
            this.tbx_TD_folder.TabIndex = 60;
            this.tbx_TD_folder.Text = "C:\\Users\\Experiments\\Desktop\\Temporary";
            // 
            // bn_TD_SelFolderA
            // 
            this.bn_TD_SelFolderA.Location = new System.Drawing.Point(318, 18);
            this.bn_TD_SelFolderA.Name = "bn_TD_SelFolderA";
            this.bn_TD_SelFolderA.Size = new System.Drawing.Size(37, 23);
            this.bn_TD_SelFolderA.TabIndex = 61;
            this.bn_TD_SelFolderA.Text = "...";
            this.bn_TD_SelFolderA.UseVisualStyleBackColor = true;
            this.bn_TD_SelFolderA.Click += new System.EventHandler(this.bn_TD_SelFolderA_Click);
            // 
            // lb_TD_Folder
            // 
            this.lb_TD_Folder.AutoSize = true;
            this.lb_TD_Folder.Location = new System.Drawing.Point(1, 23);
            this.lb_TD_Folder.Name = "lb_TD_Folder";
            this.lb_TD_Folder.Size = new System.Drawing.Size(39, 13);
            this.lb_TD_Folder.TabIndex = 59;
            this.lb_TD_Folder.Text = "Folder:";
            // 
            // lb_TD_fileName
            // 
            this.lb_TD_fileName.AutoSize = true;
            this.lb_TD_fileName.Location = new System.Drawing.Point(6, 57);
            this.lb_TD_fileName.Name = "lb_TD_fileName";
            this.lb_TD_fileName.Size = new System.Drawing.Size(87, 13);
            this.lb_TD_fileName.TabIndex = 62;
            this.lb_TD_fileName.Text = "File name format:";
            // 
            // tbx_TD_fileName
            // 
            this.tbx_TD_fileName.Location = new System.Drawing.Point(99, 54);
            this.tbx_TD_fileName.Name = "tbx_TD_fileName";
            this.tbx_TD_fileName.Size = new System.Drawing.Size(213, 20);
            this.tbx_TD_fileName.TabIndex = 63;
            this.tbx_TD_fileName.Text = "sig_%TX%_%RX%.txt";
            // 
            // gb_TD_Averaging
            // 
            this.gb_TD_Averaging.Controls.Add(this.lb_TD_storeplot);
            this.gb_TD_Averaging.Controls.Add(this.lb_TD_saving);
            this.gb_TD_Averaging.Controls.Add(this.rb_TD_NoAvg);
            this.gb_TD_Averaging.Controls.Add(this.gb_TD_collecting);
            this.gb_TD_Averaging.Controls.Add(this.rb_TD_SWAvg);
            this.gb_TD_Averaging.Controls.Add(this.nud_TD_NAvgSW);
            this.gb_TD_Averaging.Controls.Add(this.cb_TD_NoAvg);
            this.gb_TD_Averaging.Controls.Add(this.nud_TD_NAvgHW);
            this.gb_TD_Averaging.Controls.Add(this.rb_TD_HWAvg);
            this.gb_TD_Averaging.Controls.Add(this.cb_TD_SWAvg);
            this.gb_TD_Averaging.Controls.Add(this.cb_TD_HWAvg);
            this.gb_TD_Averaging.Location = new System.Drawing.Point(5, 169);
            this.gb_TD_Averaging.Name = "gb_TD_Averaging";
            this.gb_TD_Averaging.Size = new System.Drawing.Size(391, 92);
            this.gb_TD_Averaging.TabIndex = 54;
            this.gb_TD_Averaging.TabStop = false;
            this.gb_TD_Averaging.Text = "Averaging ";
            // 
            // lb_TD_storeplot
            // 
            this.lb_TD_storeplot.AutoSize = true;
            this.lb_TD_storeplot.Location = new System.Drawing.Point(10, 16);
            this.lb_TD_storeplot.Name = "lb_TD_storeplot";
            this.lb_TD_storeplot.Size = new System.Drawing.Size(61, 13);
            this.lb_TD_storeplot.TabIndex = 85;
            this.lb_TD_storeplot.Text = "Store / Plot";
            // 
            // lb_TD_saving
            // 
            this.lb_TD_saving.AutoSize = true;
            this.lb_TD_saving.Location = new System.Drawing.Point(143, 16);
            this.lb_TD_saving.Name = "lb_TD_saving";
            this.lb_TD_saving.Size = new System.Drawing.Size(86, 13);
            this.lb_TD_saving.TabIndex = 84;
            this.lb_TD_saving.Text = "Save to text file?";
            this.lb_TD_saving.MouseHover += new System.EventHandler(this.lb_TD_saving_MouseHover);
            // 
            // rb_TD_NoAvg
            // 
            this.rb_TD_NoAvg.AutoSize = true;
            this.rb_TD_NoAvg.Location = new System.Drawing.Point(13, 66);
            this.rb_TD_NoAvg.Name = "rb_TD_NoAvg";
            this.rb_TD_NoAvg.Size = new System.Drawing.Size(107, 17);
            this.rb_TD_NoAvg.TabIndex = 2;
            this.rb_TD_NoAvg.TabStop = true;
            this.rb_TD_NoAvg.Text = "Single acquisition";
            this.rb_TD_NoAvg.UseVisualStyleBackColor = true;
            // 
            // gb_TD_collecting
            // 
            this.gb_TD_collecting.Controls.Add(this.rb_TD_CollectSig);
            this.gb_TD_collecting.Controls.Add(this.cb_TD_Store);
            this.gb_TD_collecting.Controls.Add(this.rb_TD_CollectBL);
            this.gb_TD_collecting.Location = new System.Drawing.Point(259, 0);
            this.gb_TD_collecting.Name = "gb_TD_collecting";
            this.gb_TD_collecting.Size = new System.Drawing.Size(132, 92);
            this.gb_TD_collecting.TabIndex = 83;
            this.gb_TD_collecting.TabStop = false;
            this.gb_TD_collecting.Text = "Collecting: ";
            // 
            // rb_TD_CollectSig
            // 
            this.rb_TD_CollectSig.AutoSize = true;
            this.rb_TD_CollectSig.Location = new System.Drawing.Point(21, 35);
            this.rb_TD_CollectSig.Name = "rb_TD_CollectSig";
            this.rb_TD_CollectSig.Size = new System.Drawing.Size(59, 17);
            this.rb_TD_CollectSig.TabIndex = 1;
            this.rb_TD_CollectSig.Text = "Signals";
            this.rb_TD_CollectSig.UseVisualStyleBackColor = true;
            // 
            // cb_TD_Store
            // 
            this.cb_TD_Store.AutoSize = true;
            this.cb_TD_Store.Location = new System.Drawing.Point(21, 62);
            this.cb_TD_Store.Name = "cb_TD_Store";
            this.cb_TD_Store.Size = new System.Drawing.Size(104, 17);
            this.cb_TD_Store.TabIndex = 84;
            this.cb_TD_Store.Text = "Store/Plot signal";
            this.cb_TD_Store.UseVisualStyleBackColor = true;
            this.cb_TD_Store.MouseHover += new System.EventHandler(this.cb_TD_storeSig_MouseHover);
            // 
            // rb_TD_CollectBL
            // 
            this.rb_TD_CollectBL.AutoSize = true;
            this.rb_TD_CollectBL.Checked = true;
            this.rb_TD_CollectBL.Location = new System.Drawing.Point(21, 19);
            this.rb_TD_CollectBL.Name = "rb_TD_CollectBL";
            this.rb_TD_CollectBL.Size = new System.Drawing.Size(65, 17);
            this.rb_TD_CollectBL.TabIndex = 0;
            this.rb_TD_CollectBL.TabStop = true;
            this.rb_TD_CollectBL.Text = "Baseline";
            this.rb_TD_CollectBL.UseVisualStyleBackColor = true;
            // 
            // rb_TD_SWAvg
            // 
            this.rb_TD_SWAvg.AutoSize = true;
            this.rb_TD_SWAvg.Location = new System.Drawing.Point(13, 49);
            this.rb_TD_SWAvg.Name = "rb_TD_SWAvg";
            this.rb_TD_SWAvg.Size = new System.Drawing.Size(117, 17);
            this.rb_TD_SWAvg.TabIndex = 1;
            this.rb_TD_SWAvg.TabStop = true;
            this.rb_TD_SWAvg.Text = "Software averaging";
            this.rb_TD_SWAvg.UseVisualStyleBackColor = true;
            // 
            // nud_TD_NAvgSW
            // 
            this.nud_TD_NAvgSW.Location = new System.Drawing.Point(215, 52);
            this.nud_TD_NAvgSW.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nud_TD_NAvgSW.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nud_TD_NAvgSW.Name = "nud_TD_NAvgSW";
            this.nud_TD_NAvgSW.Size = new System.Drawing.Size(41, 20);
            this.nud_TD_NAvgSW.TabIndex = 11;
            this.nud_TD_NAvgSW.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // cb_TD_NoAvg
            // 
            this.cb_TD_NoAvg.AutoSize = true;
            this.cb_TD_NoAvg.Location = new System.Drawing.Point(146, 69);
            this.cb_TD_NoAvg.Name = "cb_TD_NoAvg";
            this.cb_TD_NoAvg.Size = new System.Drawing.Size(55, 17);
            this.cb_TD_NoAvg.TabIndex = 10;
            this.cb_TD_NoAvg.Text = "Single";
            this.cb_TD_NoAvg.UseVisualStyleBackColor = true;
            // 
            // nud_TD_NAvgHW
            // 
            this.nud_TD_NAvgHW.Location = new System.Drawing.Point(215, 33);
            this.nud_TD_NAvgHW.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nud_TD_NAvgHW.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nud_TD_NAvgHW.Name = "nud_TD_NAvgHW";
            this.nud_TD_NAvgHW.Size = new System.Drawing.Size(41, 20);
            this.nud_TD_NAvgHW.TabIndex = 8;
            this.nud_TD_NAvgHW.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // rb_TD_HWAvg
            // 
            this.rb_TD_HWAvg.AutoSize = true;
            this.rb_TD_HWAvg.Checked = true;
            this.rb_TD_HWAvg.Location = new System.Drawing.Point(13, 32);
            this.rb_TD_HWAvg.Name = "rb_TD_HWAvg";
            this.rb_TD_HWAvg.Size = new System.Drawing.Size(127, 17);
            this.rb_TD_HWAvg.TabIndex = 0;
            this.rb_TD_HWAvg.TabStop = true;
            this.rb_TD_HWAvg.Text = "PicoScope averaging";
            this.rb_TD_HWAvg.UseVisualStyleBackColor = true;
            // 
            // cb_TD_SWAvg
            // 
            this.cb_TD_SWAvg.AutoSize = true;
            this.cb_TD_SWAvg.Location = new System.Drawing.Point(146, 52);
            this.cb_TD_SWAvg.Name = "cb_TD_SWAvg";
            this.cb_TD_SWAvg.Size = new System.Drawing.Size(75, 17);
            this.cb_TD_SWAvg.TabIndex = 9;
            this.cb_TD_SWAvg.Text = "SWAvg = ";
            this.cb_TD_SWAvg.UseVisualStyleBackColor = true;
            // 
            // cb_TD_HWAvg
            // 
            this.cb_TD_HWAvg.AutoSize = true;
            this.cb_TD_HWAvg.Checked = true;
            this.cb_TD_HWAvg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TD_HWAvg.Location = new System.Drawing.Point(146, 35);
            this.cb_TD_HWAvg.Name = "cb_TD_HWAvg";
            this.cb_TD_HWAvg.Size = new System.Drawing.Size(76, 17);
            this.cb_TD_HWAvg.TabIndex = 0;
            this.cb_TD_HWAvg.Text = "HWAvg = ";
            this.cb_TD_HWAvg.UseVisualStyleBackColor = true;
            // 
            // gb_TD_Calibration
            // 
            this.gb_TD_Calibration.BackColor = System.Drawing.SystemColors.Control;
            this.gb_TD_Calibration.Controls.Add(this.rb_TD_CalNone);
            this.gb_TD_Calibration.Controls.Add(this.rb_TD_CalRef);
            this.gb_TD_Calibration.Controls.Add(this.rb_TD_CalCorr);
            this.gb_TD_Calibration.Location = new System.Drawing.Point(5, 261);
            this.gb_TD_Calibration.Name = "gb_TD_Calibration";
            this.gb_TD_Calibration.Size = new System.Drawing.Size(121, 73);
            this.gb_TD_Calibration.TabIndex = 51;
            this.gb_TD_Calibration.TabStop = false;
            this.gb_TD_Calibration.Text = "Calibration ";
            // 
            // rb_TD_CalNone
            // 
            this.rb_TD_CalNone.AutoSize = true;
            this.rb_TD_CalNone.Location = new System.Drawing.Point(9, 46);
            this.rb_TD_CalNone.Name = "rb_TD_CalNone";
            this.rb_TD_CalNone.Size = new System.Drawing.Size(87, 17);
            this.rb_TD_CalNone.TabIndex = 2;
            this.rb_TD_CalNone.Text = "No alignment";
            this.rb_TD_CalNone.UseVisualStyleBackColor = true;
            // 
            // rb_TD_CalRef
            // 
            this.rb_TD_CalRef.AutoSize = true;
            this.rb_TD_CalRef.Location = new System.Drawing.Point(9, 30);
            this.rb_TD_CalRef.Name = "rb_TD_CalRef";
            this.rb_TD_CalRef.Size = new System.Drawing.Size(75, 17);
            this.rb_TD_CalRef.TabIndex = 1;
            this.rb_TD_CalRef.Text = "Reference";
            this.rb_TD_CalRef.UseVisualStyleBackColor = true;
            // 
            // rb_TD_CalCorr
            // 
            this.rb_TD_CalCorr.AutoSize = true;
            this.rb_TD_CalCorr.Checked = true;
            this.rb_TD_CalCorr.Location = new System.Drawing.Point(9, 14);
            this.rb_TD_CalCorr.Name = "rb_TD_CalCorr";
            this.rb_TD_CalCorr.Size = new System.Drawing.Size(103, 17);
            this.rb_TD_CalCorr.TabIndex = 0;
            this.rb_TD_CalCorr.TabStop = true;
            this.rb_TD_CalCorr.Text = "Cross-correlation";
            this.rb_TD_CalCorr.UseVisualStyleBackColor = true;
            // 
            // legend_TD
            // 
            this.legend_TD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.legend_TD.Border = NationalInstruments.UI.Border.SolidBlack;
            this.legend_TD.ItemLayoutMode = NationalInstruments.UI.LegendItemLayoutMode.LeftToRight;
            this.legend_TD.Items.AddRange(new NationalInstruments.UI.LegendItem[] {
            this.legend_TD_item1,
            this.legend_TD_item2,
            this.legend_TD_item3,
            this.legend_TD_item4});
            this.legend_TD.Location = new System.Drawing.Point(47, 518);
            this.legend_TD.Name = "legend_TD";
            this.legend_TD.Size = new System.Drawing.Size(561, 36);
            this.legend_TD.TabIndex = 11;
            // 
            // legend_TD_item1
            // 
            this.legend_TD_item1.Source = this.Baseline;
            this.legend_TD_item1.Text = "Baseline";
            // 
            // Baseline
            // 
            this.Baseline.LineColor = System.Drawing.Color.White;
            this.Baseline.LineColorPrecedence = NationalInstruments.UI.ColorPrecedence.UserDefinedColor;
            this.Baseline.XAxis = this.sg_TD_Xaxis;
            this.Baseline.YAxis = this.sg_TD_Yaxis_sig;
            // 
            // sg_TD_Xaxis
            // 
            this.sg_TD_Xaxis.Caption = "Time, ns";
            this.sg_TD_Xaxis.MajorDivisions.GridLineStyle = NationalInstruments.UI.LineStyle.Dot;
            this.sg_TD_Xaxis.MajorDivisions.GridVisible = true;
            this.sg_TD_Xaxis.Mode = NationalInstruments.UI.AxisMode.Fixed;
            this.sg_TD_Xaxis.Range = new NationalInstruments.UI.Range(0D, 50D);
            // 
            // sg_TD_Yaxis_sig
            // 
            this.sg_TD_Yaxis_sig.BaseLineColor = System.Drawing.SystemColors.ControlLightLight;
            this.sg_TD_Yaxis_sig.BaseLineVisible = true;
            this.sg_TD_Yaxis_sig.Caption = "Amplitude of recorded signals {GREEN, WHITE}, V";
            this.sg_TD_Yaxis_sig.MajorDivisions.GridColor = System.Drawing.Color.Silver;
            this.sg_TD_Yaxis_sig.MajorDivisions.GridLineStyle = NationalInstruments.UI.LineStyle.Dot;
            this.sg_TD_Yaxis_sig.MajorDivisions.GridVisible = true;
            this.sg_TD_Yaxis_sig.Mode = NationalInstruments.UI.AxisMode.Fixed;
            this.sg_TD_Yaxis_sig.Range = new NationalInstruments.UI.Range(-0.25D, 0.25D);
            // 
            // legend_TD_item2
            // 
            this.legend_TD_item2.Source = this.Signal;
            this.legend_TD_item2.Text = "Signal with tumour response";
            // 
            // Signal
            // 
            this.Signal.ToolTipsEnabled = true;
            this.Signal.XAxis = this.sg_TD_Xaxis;
            this.Signal.YAxis = this.sg_TD_Yaxis_sig;
            // 
            // legend_TD_item3
            // 
            this.legend_TD_item3.Source = this.Difference;
            this.legend_TD_item3.Text = "Tumour response";
            // 
            // Difference
            // 
            this.Difference.LineColor = System.Drawing.Color.Red;
            this.Difference.LineColorPrecedence = NationalInstruments.UI.ColorPrecedence.UserDefinedColor;
            this.Difference.XAxis = this.sg_TD_Xaxis;
            this.Difference.YAxis = this.sg_TD_Yaxis_diff;
            // 
            // sg_TD_Yaxis_diff
            // 
            this.sg_TD_Yaxis_diff.Caption = "Amplitude of difference signals {RED, BLUE}, V";
            this.sg_TD_Yaxis_diff.CaptionPosition = NationalInstruments.UI.YAxisPosition.Right;
            this.sg_TD_Yaxis_diff.MajorDivisions.GridLineStyle = NationalInstruments.UI.LineStyle.Dot;
            this.sg_TD_Yaxis_diff.MajorDivisions.GridVisible = true;
            this.sg_TD_Yaxis_diff.Mode = NationalInstruments.UI.AxisMode.Fixed;
            this.sg_TD_Yaxis_diff.Position = NationalInstruments.UI.YAxisPosition.Right;
            this.sg_TD_Yaxis_diff.Range = new NationalInstruments.UI.Range(-0.05D, 0.05D);
            // 
            // legend_TD_item4
            // 
            this.legend_TD_item4.Source = this.SavedDifference;
            this.legend_TD_item4.Text = "Memorized difference";
            // 
            // SavedDifference
            // 
            this.SavedDifference.XAxis = this.sg_TD_Xaxis;
            this.SavedDifference.YAxis = this.sg_TD_Yaxis_diff;
            // 
            // sg_TD_graph
            // 
            this.sg_TD_graph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sg_TD_graph.Border = NationalInstruments.UI.Border.None;
            this.sg_TD_graph.Cursors.AddRange(new NationalInstruments.UI.XYCursor[] {
            this.xyCursor1});
            this.sg_TD_graph.ImmediateUpdates = true;
            this.sg_TD_graph.InteractionMode = ((NationalInstruments.UI.GraphInteractionModes)((((((((NationalInstruments.UI.GraphInteractionModes.ZoomX | NationalInstruments.UI.GraphInteractionModes.ZoomY)
                        | NationalInstruments.UI.GraphInteractionModes.ZoomAroundPoint)
                        | NationalInstruments.UI.GraphInteractionModes.PanX)
                        | NationalInstruments.UI.GraphInteractionModes.PanY)
                        | NationalInstruments.UI.GraphInteractionModes.DragCursor)
                        | NationalInstruments.UI.GraphInteractionModes.DragAnnotationCaption)
                        | NationalInstruments.UI.GraphInteractionModes.EditRange)));
            this.sg_TD_graph.Location = new System.Drawing.Point(0, 0);
            this.sg_TD_graph.Name = "sg_TD_graph";
            this.sg_TD_graph.Plots.AddRange(new NationalInstruments.UI.ScatterPlot[] {
            this.Signal,
            this.Baseline,
            this.Difference,
            this.SavedDifference});
            this.sg_TD_graph.Size = new System.Drawing.Size(769, 526);
            this.sg_TD_graph.TabIndex = 10;
            this.sg_TD_graph.UseColorGenerator = true;
            this.sg_TD_graph.XAxes.AddRange(new NationalInstruments.UI.XAxis[] {
            this.sg_TD_Xaxis});
            this.sg_TD_graph.YAxes.AddRange(new NationalInstruments.UI.YAxis[] {
            this.sg_TD_Yaxis_sig,
            this.sg_TD_Yaxis_diff});
            this.sg_TD_graph.ZoomAnimation = false;
            this.sg_TD_graph.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.sg_TD_graph_MouseDoubleClick);
            // 
            // xyCursor1
            // 
            this.xyCursor1.Color = System.Drawing.Color.Goldenrod;
            this.xyCursor1.HorizontalCrosshairLength = 5F;
            this.xyCursor1.LabelVisible = true;
            this.xyCursor1.LineStyle = NationalInstruments.UI.LineStyle.Dash;
            this.xyCursor1.Plot = this.SavedDifference;
            this.xyCursor1.PointSize = new System.Drawing.Size(5, 5);
            this.xyCursor1.VerticalCrosshairLength = 5F;
            this.xyCursor1.XPosition = 0D;
            this.xyCursor1.YPosition = 0D;
            // 
            // tbp_PS9201A
            // 
            this.tbp_PS9201A.Controls.Add(this.gb_PS_picoscope);
            this.tbp_PS9201A.Location = new System.Drawing.Point(4, 25);
            this.tbp_PS9201A.Name = "tbp_PS9201A";
            this.tbp_PS9201A.Padding = new System.Windows.Forms.Padding(3);
            this.tbp_PS9201A.Size = new System.Drawing.Size(1199, 565);
            this.tbp_PS9201A.TabIndex = 2;
            this.tbp_PS9201A.Text = "PS9201A";
            this.tbp_PS9201A.UseVisualStyleBackColor = true;
            // 
            // gb_PS_picoscope
            // 
            this.gb_PS_picoscope.Controls.Add(this.bn_PS_connect);
            this.gb_PS_picoscope.Controls.Add(this.lb_PS);
            this.gb_PS_picoscope.Controls.Add(this.tbx_PS_conFile);
            this.gb_PS_picoscope.Controls.Add(this.bn_PS_browseConFile);
            this.gb_PS_picoscope.Controls.Add(this.bn_PS_configure);
            this.gb_PS_picoscope.Location = new System.Drawing.Point(44, 30);
            this.gb_PS_picoscope.Name = "gb_PS_picoscope";
            this.gb_PS_picoscope.Size = new System.Drawing.Size(441, 478);
            this.gb_PS_picoscope.TabIndex = 31;
            this.gb_PS_picoscope.TabStop = false;
            this.gb_PS_picoscope.Text = "PS9201A";
            // 
            // bn_PS_connect
            // 
            this.bn_PS_connect.Location = new System.Drawing.Point(25, 30);
            this.bn_PS_connect.Name = "bn_PS_connect";
            this.bn_PS_connect.Size = new System.Drawing.Size(75, 23);
            this.bn_PS_connect.TabIndex = 4;
            this.bn_PS_connect.Text = "Connect";
            this.bn_PS_connect.UseVisualStyleBackColor = true;
            this.bn_PS_connect.Click += new System.EventHandler(this.bn_PS_connect_Click);
            // 
            // lb_PS
            // 
            this.lb_PS.AutoSize = true;
            this.lb_PS.Location = new System.Drawing.Point(22, 67);
            this.lb_PS.Name = "lb_PS";
            this.lb_PS.Size = new System.Drawing.Size(88, 13);
            this.lb_PS.TabIndex = 11;
            this.lb_PS.Text = "Configuration file:";
            // 
            // tbx_PS_conFile
            // 
            this.tbx_PS_conFile.Location = new System.Drawing.Point(114, 63);
            this.tbx_PS_conFile.Name = "tbx_PS_conFile";
            this.tbx_PS_conFile.Size = new System.Drawing.Size(253, 20);
            this.tbx_PS_conFile.TabIndex = 12;
            this.tbx_PS_conFile.Text = "C:\\Users\\Experiments\\Documents\\Karim\\Switching Matrix\\Taylor - Copy\\Configuration" +
                " Files\\config_16avg.txt";
            // 
            // bn_PS_browseConFile
            // 
            this.bn_PS_browseConFile.Location = new System.Drawing.Point(369, 62);
            this.bn_PS_browseConFile.Name = "bn_PS_browseConFile";
            this.bn_PS_browseConFile.Size = new System.Drawing.Size(37, 23);
            this.bn_PS_browseConFile.TabIndex = 13;
            this.bn_PS_browseConFile.Text = "...";
            this.bn_PS_browseConFile.UseVisualStyleBackColor = true;
            this.bn_PS_browseConFile.Click += new System.EventHandler(this.bn_PS_browseConFile_Click);
            // 
            // bn_PS_configure
            // 
            this.bn_PS_configure.Location = new System.Drawing.Point(318, 30);
            this.bn_PS_configure.Name = "bn_PS_configure";
            this.bn_PS_configure.Size = new System.Drawing.Size(86, 23);
            this.bn_PS_configure.TabIndex = 14;
            this.bn_PS_configure.Text = "Configure";
            this.bn_PS_configure.UseVisualStyleBackColor = true;
            this.bn_PS_configure.Click += new System.EventHandler(this.bn_PS_configure_Click);
            // 
            // tbp_SM
            // 
            this.tbp_SM.Controls.Add(this.groupBox3);
            this.tbp_SM.Controls.Add(this.bn_SM_Power);
            this.tbp_SM.Location = new System.Drawing.Point(4, 25);
            this.tbp_SM.Name = "tbp_SM";
            this.tbp_SM.Padding = new System.Windows.Forms.Padding(3);
            this.tbp_SM.Size = new System.Drawing.Size(1199, 565);
            this.tbp_SM.TabIndex = 4;
            this.tbp_SM.Text = "Switching Matrix";
            this.tbp_SM.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.gb_SM_TXA);
            this.groupBox3.Controls.Add(this.gb_SM_RXA);
            this.groupBox3.Location = new System.Drawing.Point(128, 38);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(334, 162);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "TomoBra Control";
            // 
            // gb_SM_TXA
            // 
            this.gb_SM_TXA.Controls.Add(this.nud_SM_TXA_del);
            this.gb_SM_TXA.Controls.Add(this.bn_SM_TXA_loop);
            this.gb_SM_TXA.Controls.Add(this.lb_SM_TXA_loop);
            this.gb_SM_TXA.Controls.Add(this.nud_SM_TXA_num);
            this.gb_SM_TXA.Controls.Add(this.bn_SM_TXA_set);
            this.gb_SM_TXA.Controls.Add(this.lb_SM_TXA);
            this.gb_SM_TXA.Location = new System.Drawing.Point(23, 18);
            this.gb_SM_TXA.Name = "gb_SM_TXA";
            this.gb_SM_TXA.Size = new System.Drawing.Size(145, 129);
            this.gb_SM_TXA.TabIndex = 30;
            this.gb_SM_TXA.TabStop = false;
            this.gb_SM_TXA.Text = "TX Antenna";
            // 
            // nud_SM_TXA_del
            // 
            this.nud_SM_TXA_del.Location = new System.Drawing.Point(90, 69);
            this.nud_SM_TXA_del.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nud_SM_TXA_del.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nud_SM_TXA_del.Name = "nud_SM_TXA_del";
            this.nud_SM_TXA_del.Size = new System.Drawing.Size(45, 20);
            this.nud_SM_TXA_del.TabIndex = 9;
            this.nud_SM_TXA_del.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // bn_SM_TXA_loop
            // 
            this.bn_SM_TXA_loop.Location = new System.Drawing.Point(12, 94);
            this.bn_SM_TXA_loop.Name = "bn_SM_TXA_loop";
            this.bn_SM_TXA_loop.Size = new System.Drawing.Size(125, 23);
            this.bn_SM_TXA_loop.TabIndex = 7;
            this.bn_SM_TXA_loop.Text = "Loop";
            this.bn_SM_TXA_loop.UseVisualStyleBackColor = true;
            this.bn_SM_TXA_loop.Click += new System.EventHandler(this.bn_SM_TXA_loop_Click);
            // 
            // lb_SM_TXA_loop
            // 
            this.lb_SM_TXA_loop.AutoSize = true;
            this.lb_SM_TXA_loop.Location = new System.Drawing.Point(9, 71);
            this.lb_SM_TXA_loop.Name = "lb_SM_TXA_loop";
            this.lb_SM_TXA_loop.Size = new System.Drawing.Size(84, 13);
            this.lb_SM_TXA_loop.TabIndex = 8;
            this.lb_SM_TXA_loop.Text = "Loop delay (ms):";
            // 
            // nud_SM_TXA_num
            // 
            this.nud_SM_TXA_num.Location = new System.Drawing.Point(81, 18);
            this.nud_SM_TXA_num.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nud_SM_TXA_num.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_SM_TXA_num.Name = "nud_SM_TXA_num";
            this.nud_SM_TXA_num.Size = new System.Drawing.Size(45, 20);
            this.nud_SM_TXA_num.TabIndex = 3;
            this.nud_SM_TXA_num.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // bn_SM_TXA_set
            // 
            this.bn_SM_TXA_set.Location = new System.Drawing.Point(10, 45);
            this.bn_SM_TXA_set.Name = "bn_SM_TXA_set";
            this.bn_SM_TXA_set.Size = new System.Drawing.Size(94, 23);
            this.bn_SM_TXA_set.TabIndex = 1;
            this.bn_SM_TXA_set.Text = "Set";
            this.bn_SM_TXA_set.UseVisualStyleBackColor = true;
            this.bn_SM_TXA_set.Click += new System.EventHandler(this.bn_SM_TXA_set_Click);
            // 
            // lb_SM_TXA
            // 
            this.lb_SM_TXA.AutoSize = true;
            this.lb_SM_TXA.Location = new System.Drawing.Point(7, 20);
            this.lb_SM_TXA.Name = "lb_SM_TXA";
            this.lb_SM_TXA.Size = new System.Drawing.Size(60, 13);
            this.lb_SM_TXA.TabIndex = 2;
            this.lb_SM_TXA.Text = "Antenna #:";
            // 
            // gb_SM_RXA
            // 
            this.gb_SM_RXA.Controls.Add(this.nud_SM_RXA_del);
            this.gb_SM_RXA.Controls.Add(this.bn_SM_RXA_loop);
            this.gb_SM_RXA.Controls.Add(this.lb_SM_RXA_loop);
            this.gb_SM_RXA.Controls.Add(this.nud_SM_RXA_num);
            this.gb_SM_RXA.Controls.Add(this.bn_SM_RXA_set);
            this.gb_SM_RXA.Controls.Add(this.lb_SM_RXA);
            this.gb_SM_RXA.Location = new System.Drawing.Point(166, 18);
            this.gb_SM_RXA.Name = "gb_SM_RXA";
            this.gb_SM_RXA.Size = new System.Drawing.Size(143, 129);
            this.gb_SM_RXA.TabIndex = 29;
            this.gb_SM_RXA.TabStop = false;
            this.gb_SM_RXA.Text = "RX Antenna";
            // 
            // nud_SM_RXA_del
            // 
            this.nud_SM_RXA_del.Location = new System.Drawing.Point(88, 69);
            this.nud_SM_RXA_del.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nud_SM_RXA_del.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nud_SM_RXA_del.Name = "nud_SM_RXA_del";
            this.nud_SM_RXA_del.Size = new System.Drawing.Size(45, 20);
            this.nud_SM_RXA_del.TabIndex = 9;
            this.nud_SM_RXA_del.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // bn_SM_RXA_loop
            // 
            this.bn_SM_RXA_loop.Location = new System.Drawing.Point(10, 94);
            this.bn_SM_RXA_loop.Name = "bn_SM_RXA_loop";
            this.bn_SM_RXA_loop.Size = new System.Drawing.Size(125, 23);
            this.bn_SM_RXA_loop.TabIndex = 7;
            this.bn_SM_RXA_loop.Text = "Loop";
            this.bn_SM_RXA_loop.UseVisualStyleBackColor = true;
            this.bn_SM_RXA_loop.Click += new System.EventHandler(this.bn_SM_RXA_loop_Click);
            // 
            // lb_SM_RXA_loop
            // 
            this.lb_SM_RXA_loop.AutoSize = true;
            this.lb_SM_RXA_loop.Location = new System.Drawing.Point(7, 71);
            this.lb_SM_RXA_loop.Name = "lb_SM_RXA_loop";
            this.lb_SM_RXA_loop.Size = new System.Drawing.Size(84, 13);
            this.lb_SM_RXA_loop.TabIndex = 8;
            this.lb_SM_RXA_loop.Text = "Loop delay (ms):";
            // 
            // nud_SM_RXA_num
            // 
            this.nud_SM_RXA_num.Location = new System.Drawing.Point(81, 18);
            this.nud_SM_RXA_num.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nud_SM_RXA_num.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_SM_RXA_num.Name = "nud_SM_RXA_num";
            this.nud_SM_RXA_num.Size = new System.Drawing.Size(45, 20);
            this.nud_SM_RXA_num.TabIndex = 3;
            this.nud_SM_RXA_num.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // bn_SM_RXA_set
            // 
            this.bn_SM_RXA_set.Location = new System.Drawing.Point(10, 45);
            this.bn_SM_RXA_set.Name = "bn_SM_RXA_set";
            this.bn_SM_RXA_set.Size = new System.Drawing.Size(94, 23);
            this.bn_SM_RXA_set.TabIndex = 1;
            this.bn_SM_RXA_set.Text = "Set";
            this.bn_SM_RXA_set.UseVisualStyleBackColor = true;
            this.bn_SM_RXA_set.Click += new System.EventHandler(this.bn_SM_RXA_set_Click);
            // 
            // lb_SM_RXA
            // 
            this.lb_SM_RXA.AutoSize = true;
            this.lb_SM_RXA.Location = new System.Drawing.Point(7, 20);
            this.lb_SM_RXA.Name = "lb_SM_RXA";
            this.lb_SM_RXA.Size = new System.Drawing.Size(60, 13);
            this.lb_SM_RXA.TabIndex = 2;
            this.lb_SM_RXA.Text = "Antenna #:";
            // 
            // bn_SM_Power
            // 
            this.bn_SM_Power.Location = new System.Drawing.Point(17, 38);
            this.bn_SM_Power.Name = "bn_SM_Power";
            this.bn_SM_Power.Size = new System.Drawing.Size(93, 23);
            this.bn_SM_Power.TabIndex = 17;
            this.bn_SM_Power.Tag = "0";
            this.bn_SM_Power.Text = "Power ON";
            this.bn_SM_Power.UseVisualStyleBackColor = true;
            this.bn_SM_Power.Click += new System.EventHandler(this.bn_SM_Power_Click);
            // 
            // tbp_service
            // 
            this.tbp_service.Location = new System.Drawing.Point(0, 0);
            this.tbp_service.Name = "tbp_service";
            this.tbp_service.Size = new System.Drawing.Size(200, 100);
            this.tbp_service.TabIndex = 0;
            // 
            // tbx_serv_folder
            // 
            this.tbx_serv_folder.Location = new System.Drawing.Point(0, 0);
            this.tbx_serv_folder.Name = "tbx_serv_folder";
            this.tbx_serv_folder.Size = new System.Drawing.Size(100, 20);
            this.tbx_serv_folder.TabIndex = 0;
            // 
            // Full_Scan_gb_Service
            // 
            this.Full_Scan_gb_Service.Controls.Add(this.label12);
            this.Full_Scan_gb_Service.Controls.Add(this.Full_Scan_Delay_Service);
            this.Full_Scan_gb_Service.Controls.Add(this.Cancel_Full_Scan_Service);
            this.Full_Scan_gb_Service.Controls.Add(this.Pause_Or_Resume_Full_Scan_Service);
            this.Full_Scan_gb_Service.Controls.Add(this.Initialize_Full_Scan_Service);
            this.Full_Scan_gb_Service.Location = new System.Drawing.Point(465, 42);
            this.Full_Scan_gb_Service.Name = "Full_Scan_gb_Service";
            this.Full_Scan_gb_Service.Size = new System.Drawing.Size(142, 129);
            this.Full_Scan_gb_Service.TabIndex = 38;
            this.Full_Scan_gb_Service.TabStop = false;
            this.Full_Scan_gb_Service.Text = "Full Scan (Doesn\'t Work)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 18);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 13);
            this.label12.TabIndex = 36;
            this.label12.Text = "Delay (ms):";
            // 
            // Full_Scan_Delay_Service
            // 
            this.Full_Scan_Delay_Service.Location = new System.Drawing.Point(75, 15);
            this.Full_Scan_Delay_Service.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.Full_Scan_Delay_Service.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.Full_Scan_Delay_Service.Name = "Full_Scan_Delay_Service";
            this.Full_Scan_Delay_Service.Size = new System.Drawing.Size(45, 20);
            this.Full_Scan_Delay_Service.TabIndex = 36;
            this.Full_Scan_Delay_Service.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // Cancel_Full_Scan_Service
            // 
            this.Cancel_Full_Scan_Service.Location = new System.Drawing.Point(0, 0);
            this.Cancel_Full_Scan_Service.Name = "Cancel_Full_Scan_Service";
            this.Cancel_Full_Scan_Service.Size = new System.Drawing.Size(75, 23);
            this.Cancel_Full_Scan_Service.TabIndex = 37;
            // 
            // Pause_Or_Resume_Full_Scan_Service
            // 
            this.Pause_Or_Resume_Full_Scan_Service.Location = new System.Drawing.Point(0, 0);
            this.Pause_Or_Resume_Full_Scan_Service.Name = "Pause_Or_Resume_Full_Scan_Service";
            this.Pause_Or_Resume_Full_Scan_Service.Size = new System.Drawing.Size(75, 23);
            this.Pause_Or_Resume_Full_Scan_Service.TabIndex = 38;
            // 
            // Initialize_Full_Scan_Service
            // 
            this.Initialize_Full_Scan_Service.Location = new System.Drawing.Point(0, 0);
            this.Initialize_Full_Scan_Service.Name = "Initialize_Full_Scan_Service";
            this.Initialize_Full_Scan_Service.Size = new System.Drawing.Size(75, 23);
            this.Initialize_Full_Scan_Service.TabIndex = 39;
            // 
            // lb_serv_folder
            // 
            this.lb_serv_folder.Location = new System.Drawing.Point(0, 0);
            this.lb_serv_folder.Name = "lb_serv_folder";
            this.lb_serv_folder.Size = new System.Drawing.Size(100, 23);
            this.lb_serv_folder.TabIndex = 0;
            // 
            // lb_serv_fileName2
            // 
            this.lb_serv_fileName2.Location = new System.Drawing.Point(0, 0);
            this.lb_serv_fileName2.Name = "lb_serv_fileName2";
            this.lb_serv_fileName2.Size = new System.Drawing.Size(100, 23);
            this.lb_serv_fileName2.TabIndex = 0;
            // 
            // tbx_serv_fileName
            // 
            this.tbx_serv_fileName.Location = new System.Drawing.Point(0, 0);
            this.tbx_serv_fileName.Name = "tbx_serv_fileName";
            this.tbx_serv_fileName.Size = new System.Drawing.Size(100, 20);
            this.tbx_serv_fileName.TabIndex = 0;
            // 
            // xyCursor3
            // 
            this.xyCursor3.Color = System.Drawing.Color.Goldenrod;
            this.xyCursor3.HorizontalCrosshairLength = 5F;
            this.xyCursor3.LabelVisible = true;
            this.xyCursor3.LineStyle = NationalInstruments.UI.LineStyle.Dash;
            this.xyCursor3.PointSize = new System.Drawing.Size(5, 5);
            this.xyCursor3.VerticalCrosshairLength = 5F;
            this.xyCursor3.XPosition = 0D;
            this.xyCursor3.YPosition = 0D;
            // 
            // xyCursor4
            // 
            this.xyCursor4.Color = System.Drawing.Color.Goldenrod;
            this.xyCursor4.HorizontalCrosshairLength = 5F;
            this.xyCursor4.LabelVisible = true;
            this.xyCursor4.LineStyle = NationalInstruments.UI.LineStyle.Dash;
            this.xyCursor4.PointSize = new System.Drawing.Size(5, 5);
            this.xyCursor4.VerticalCrosshairLength = 5F;
            this.xyCursor4.XPosition = 0D;
            this.xyCursor4.YPosition = 0D;
            // 
            // Manual_SW5_set
            // 
            this.Manual_SW5_set.Location = new System.Drawing.Point(0, 0);
            this.Manual_SW5_set.Name = "Manual_SW5_set";
            this.Manual_SW5_set.Size = new System.Drawing.Size(75, 23);
            this.Manual_SW5_set.TabIndex = 0;
            // 
            // Manual_SW5_nud
            // 
            this.Manual_SW5_nud.Location = new System.Drawing.Point(59, 18);
            this.Manual_SW5_nud.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.Manual_SW5_nud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Manual_SW5_nud.Name = "Manual_SW5_nud";
            this.Manual_SW5_nud.Size = new System.Drawing.Size(49, 20);
            this.Manual_SW5_nud.TabIndex = 1;
            this.Manual_SW5_nud.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lb_SM_pos3
            // 
            this.lb_SM_pos3.AutoSize = true;
            this.lb_SM_pos3.Location = new System.Drawing.Point(6, 20);
            this.lb_SM_pos3.Name = "lb_SM_pos3";
            this.lb_SM_pos3.Size = new System.Drawing.Size(47, 13);
            this.lb_SM_pos3.TabIndex = 0;
            this.lb_SM_pos3.Text = "Position:";
            // 
            // ofd_loadTD
            // 
            this.ofd_loadTD.FileName = "ofd_loadTD";
            // 
            // form_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1209, 742);
            this.Controls.Add(this.sc_main);
            this.Controls.Add(this.ss_main);
            this.Controls.Add(this.ms_mainMenu);
            this.HelpButton = true;
            this.MainMenuStrip = this.ms_mainMenu;
            this.Name = "form_main";
            this.Text = "TomoBra Data Acquisition";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.form_main_FormClosed);
            this.SizeChanged += new System.EventHandler(this.form_main_SizeChanged);
            this.ms_mainMenu.ResumeLayout(false);
            this.ms_mainMenu.PerformLayout();
            this.ss_main.ResumeLayout(false);
            this.ss_main.PerformLayout();
            this.sc_main.Panel1.ResumeLayout(false);
            this.sc_main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sc_main)).EndInit();
            this.sc_main.ResumeLayout(false);
            this.tbctrl_main.ResumeLayout(false);
            this.tbp_TD.ResumeLayout(false);
            this.sc_TD.Panel1.ResumeLayout(false);
            this.sc_TD.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sc_TD)).EndInit();
            this.sc_TD.ResumeLayout(false);
            this.gb_TD_attributes.ResumeLayout(false);
            this.gb_TD_attributes.PerformLayout();
            this.gb_TD_Suffix.ResumeLayout(false);
            this.gb_TD_Suffix.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TD_counter)).EndInit();
            this.gb_TD_SamplingRate.ResumeLayout(false);
            this.gb_TD_SamplingRate.PerformLayout();
            this.gb_TD_Filtering.ResumeLayout(false);
            this.gb_TD_Filtering.PerformLayout();
            this.gb_TD_fullArray.ResumeLayout(false);
            this.gb_TD_fullArray.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TD_NCollect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TD_StartCaseNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TD_NumCases)).EndInit();
            this.gb_TD_save.ResumeLayout(false);
            this.gb_TD_save.PerformLayout();
            this.gb_TD_Averaging.ResumeLayout(false);
            this.gb_TD_Averaging.PerformLayout();
            this.gb_TD_collecting.ResumeLayout(false);
            this.gb_TD_collecting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TD_NAvgSW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TD_NAvgHW)).EndInit();
            this.gb_TD_Calibration.ResumeLayout(false);
            this.gb_TD_Calibration.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.legend_TD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sg_TD_graph)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xyCursor1)).EndInit();
            this.tbp_PS9201A.ResumeLayout(false);
            this.gb_PS_picoscope.ResumeLayout(false);
            this.gb_PS_picoscope.PerformLayout();
            this.tbp_SM.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.gb_SM_TXA.ResumeLayout(false);
            this.gb_SM_TXA.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SM_TXA_del)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SM_TXA_num)).EndInit();
            this.gb_SM_RXA.ResumeLayout(false);
            this.gb_SM_RXA.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SM_RXA_del)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SM_RXA_num)).EndInit();
            this.Full_Scan_gb_Service.ResumeLayout(false);
            this.Full_Scan_gb_Service.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Full_Scan_Delay_Service)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xyCursor3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xyCursor4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Manual_SW5_nud)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip ms_mainMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmi_file;
        private System.Windows.Forms.ToolStripMenuItem tsmi_file_exit;
        private System.Windows.Forms.OpenFileDialog ofd_PS_browse;
        private System.Windows.Forms.StatusStrip ss_main;
        private System.Windows.Forms.ToolStripStatusLabel tssl_device;
        private System.Windows.Forms.ToolStripStatusLabel tssl_SM;
        private System.Windows.Forms.ToolStripStatusLabel tssl_TXnum;
        private System.Windows.Forms.ToolStripProgressBar pb_status;
        private System.Windows.Forms.ToolStripStatusLabel tssl_RXnum;
        private System.Windows.Forms.ToolStripStatusLabel tssl_connectionStatus;
        private System.Windows.Forms.ToolStripStatusLabel tssl_statusSM;
        private System.Windows.Forms.ToolStripStatusLabel tssl_RX;
        private System.Windows.Forms.ToolStripStatusLabel tssl_TX;
        private System.ComponentModel.BackgroundWorker bgw_TD_realTime;
        private System.ComponentModel.BackgroundWorker bgw_TD_collectAuto;
        private System.Windows.Forms.ToolStripStatusLabel tssl_running;
        private System.Windows.Forms.ListView lv_msgLog;
        private System.Windows.Forms.SplitContainer sc_main;
        private System.Windows.Forms.ColumnHeader ch_time;
        private System.Windows.Forms.ColumnHeader ch_event;
        private System.Windows.Forms.ToolStripMenuItem tsmi_graph_Baseline;
        private System.Windows.Forms.ToolStripMenuItem tsmi_graph_Signal;
        private System.Windows.Forms.ToolStripMenuItem tsmi_graph_Response;
        private System.Windows.Forms.ToolStripMenuItem tsmi_graph_MemDiff;
        private System.Windows.Forms.ToolStripMenuItem tsmi_graph_Display;
        private System.Windows.Forms.ToolStripMenuItem tsmi_graph_Display_Default;
        private System.Windows.Forms.ToolStripMenuItem tsmi_graph_Display_Dual;
        private System.Windows.Forms.FolderBrowserDialog fbd_TD;
        private System.Windows.Forms.FolderBrowserDialog fbd_load;
        private System.Windows.Forms.OpenFileDialog ofd_loadTD;
        private System.ComponentModel.BackgroundWorker bgw_load_FD_BL;
        private System.ComponentModel.BackgroundWorker bgw_load_FD_Sig;
        private System.Windows.Forms.ToolStripMenuItem tsmi_help_view;
        private System.Windows.Forms.TabControl tbctrl_main;
        private System.Windows.Forms.TabPage tbp_TD;
        private System.Windows.Forms.SplitContainer sc_TD;
        private System.Windows.Forms.GroupBox gb_TD_attributes;
        private System.Windows.Forms.Label lb_TD_CurAntA;
        private System.Windows.Forms.Label lb_TD_CurAntA1;
        private System.Windows.Forms.Label lb_TD_MaxDelay;
        private System.Windows.Forms.Label lb_TD_AlDisc;
        private System.Windows.Forms.Label lb_TD_MaxDelay1;
        private System.Windows.Forms.Label lb_TD_AlDisc1;
        private System.Windows.Forms.Label lb_TD_sigCollected;
        private System.Windows.Forms.Label lb_TD_sigCollected1;
        private System.Windows.Forms.Label lb_TD_alignment;
        private System.Windows.Forms.Label lb_TD_align1;
        private System.Windows.Forms.Label lb_TD_WfmNum;
        private System.Windows.Forms.Label lb_TD_acqnum;
        private System.Windows.Forms.GroupBox gb_TD_Suffix;
        private System.Windows.Forms.NumericUpDown nud_TD_counter;
        private System.Windows.Forms.Label lb_TD_counter;
        private System.Windows.Forms.CheckBox cb_TD_CPP3;
        private System.Windows.Forms.CheckBox cb_TD_CPP2;
        private System.Windows.Forms.CheckBox cb_TD_CPP1;
        private System.Windows.Forms.TextBox tbx_TD_Suff3;
        private System.Windows.Forms.TextBox tbx_TD_Suff2;
        private System.Windows.Forms.TextBox tbx_TD_Suff1;
        private System.Windows.Forms.RadioButton rb_TD_Suff3;
        private System.Windows.Forms.RadioButton rb_TD_Suff2;
        private System.Windows.Forms.RadioButton rb_TD_Suff1;
        private System.Windows.Forms.GroupBox gb_TD_SamplingRate;
        private System.Windows.Forms.RadioButton rb_TD_200GHz;
        private System.Windows.Forms.RadioButton rb_TD_80GHz;
        private System.Windows.Forms.GroupBox gb_TD_Filtering;
        private System.Windows.Forms.RadioButton rb_TD_FltNone;
        private System.Windows.Forms.RadioButton rb_TD_FltBP;
        private System.Windows.Forms.RadioButton rb_TD_FltLP;
        private System.Windows.Forms.GroupBox gb_TD_fullArray;
        private System.Windows.Forms.Label lb_TD_numsigcol;
        private System.Windows.Forms.NumericUpDown nud_TD_NCollect;
        private System.Windows.Forms.Button bn_TD_fullArray;
        private System.Windows.Forms.NumericUpDown nud_TD_StartCaseNum;
        private System.Windows.Forms.CheckBox cb_TD_IncrementalSave;
        private System.Windows.Forms.Button bn_TD_Collect;
        private System.Windows.Forms.Label lb_TD_start;
        private System.Windows.Forms.Label lb_TD_cases;
        private System.Windows.Forms.Button bn_TD_MemSig;
        private System.Windows.Forms.NumericUpDown nud_TD_NumCases;
        private System.Windows.Forms.Button bn_TD_realTime;
        private System.Windows.Forms.Button bn_TD_single;
        private System.Windows.Forms.GroupBox gb_TD_save;
        private System.Windows.Forms.Label lb_TD_fileNameOptions;
        private System.Windows.Forms.TextBox tbx_TD_folder;
        private System.Windows.Forms.Button bn_TD_SelFolderA;
        private System.Windows.Forms.Label lb_TD_Folder;
        private System.Windows.Forms.Label lb_TD_fileName;
        private System.Windows.Forms.TextBox tbx_TD_fileName;
        private System.Windows.Forms.GroupBox gb_TD_Averaging;
        private System.Windows.Forms.Label lb_TD_storeplot;
        private System.Windows.Forms.Label lb_TD_saving;
        private System.Windows.Forms.RadioButton rb_TD_NoAvg;
        private System.Windows.Forms.GroupBox gb_TD_collecting;
        private System.Windows.Forms.RadioButton rb_TD_CollectSig;
        private System.Windows.Forms.CheckBox cb_TD_Store;
        private System.Windows.Forms.RadioButton rb_TD_CollectBL;
        private System.Windows.Forms.RadioButton rb_TD_SWAvg;
        private System.Windows.Forms.NumericUpDown nud_TD_NAvgSW;
        private System.Windows.Forms.CheckBox cb_TD_NoAvg;
        private System.Windows.Forms.NumericUpDown nud_TD_NAvgHW;
        private System.Windows.Forms.RadioButton rb_TD_HWAvg;
        private System.Windows.Forms.CheckBox cb_TD_SWAvg;
        private System.Windows.Forms.CheckBox cb_TD_HWAvg;
        private System.Windows.Forms.GroupBox gb_TD_Calibration;
        private System.Windows.Forms.RadioButton rb_TD_CalNone;
        private System.Windows.Forms.RadioButton rb_TD_CalRef;
        private System.Windows.Forms.RadioButton rb_TD_CalCorr;
        private NationalInstruments.UI.WindowsForms.Legend legend_TD;
        private NationalInstruments.UI.LegendItem legend_TD_item1;
        private NationalInstruments.UI.ScatterPlot Baseline;
        private NationalInstruments.UI.XAxis sg_TD_Xaxis;
        private NationalInstruments.UI.YAxis sg_TD_Yaxis_sig;
        private NationalInstruments.UI.LegendItem legend_TD_item2;
        private NationalInstruments.UI.ScatterPlot Signal;
        private NationalInstruments.UI.LegendItem legend_TD_item3;
        private NationalInstruments.UI.ScatterPlot Difference;
        private NationalInstruments.UI.YAxis sg_TD_Yaxis_diff;
        private NationalInstruments.UI.LegendItem legend_TD_item4;
        private NationalInstruments.UI.ScatterPlot SavedDifference;
        private NationalInstruments.UI.WindowsForms.ScatterGraph sg_TD_graph;
        private NationalInstruments.UI.XYCursor xyCursor1;
        private NationalInstruments.UI.XYCursor xyCursor4;
        private System.Windows.Forms.TabPage tbp_PS9201A;
        private System.Windows.Forms.GroupBox gb_PS_picoscope;
        private System.Windows.Forms.Button bn_PS_connect;
        private System.Windows.Forms.Label lb_PS;
        private System.Windows.Forms.TextBox tbx_PS_conFile;
        private System.Windows.Forms.Button bn_PS_browseConFile;
        private System.Windows.Forms.Button bn_PS_configure;
        private System.Windows.Forms.TabPage tbp_SM;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox gb_SM_TXA;
        private System.Windows.Forms.NumericUpDown nud_SM_TXA_del;
        private System.Windows.Forms.Button bn_SM_TXA_loop;
        private System.Windows.Forms.Label lb_SM_TXA_loop;
        private System.Windows.Forms.NumericUpDown nud_SM_TXA_num;
        private System.Windows.Forms.Button bn_SM_TXA_set;
        private System.Windows.Forms.Label lb_SM_TXA;
        private System.Windows.Forms.GroupBox gb_SM_RXA;
        private System.Windows.Forms.NumericUpDown nud_SM_RXA_del;
        private System.Windows.Forms.Button bn_SM_RXA_loop;
        private System.Windows.Forms.Label lb_SM_RXA_loop;
        private System.Windows.Forms.NumericUpDown nud_SM_RXA_num;
        private System.Windows.Forms.Button bn_SM_RXA_set;
        private System.Windows.Forms.Label lb_SM_RXA;
        private System.Windows.Forms.Button Manual_SW5_set;
        private System.Windows.Forms.NumericUpDown Manual_SW5_nud;
        private System.Windows.Forms.Label lb_SM_pos3;
        private System.Windows.Forms.Button bn_SM_Power;
        private System.Windows.Forms.TabPage tbp_service;
        private System.Windows.Forms.TextBox tbx_serv_folder;
        private System.Windows.Forms.GroupBox Full_Scan_gb_Service;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown Full_Scan_Delay_Service;
        private System.Windows.Forms.Label lb_serv_folder;
        private System.Windows.Forms.Label lb_serv_fileName2;
        private System.Windows.Forms.TextBox tbx_serv_fileName;
        private NationalInstruments.UI.XYCursor xyCursor3;
        private System.Windows.Forms.Button Cancel_Full_Scan_Service;
        private System.Windows.Forms.Button Pause_Or_Resume_Full_Scan_Service;
        private System.Windows.Forms.Button Initialize_Full_Scan_Service;
    }
}

