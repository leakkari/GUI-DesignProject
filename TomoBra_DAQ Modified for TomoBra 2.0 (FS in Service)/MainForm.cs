using NationalInstruments.Analysis;
using NationalInstruments.Analysis.Conversion;
using NationalInstruments.Analysis.Dsp;
using NationalInstruments.Analysis.Dsp.Filters;
using NationalInstruments.Analysis.Math;
using NationalInstruments.Analysis.Monitoring;
using NationalInstruments.Analysis.SignalGeneration;
using NationalInstruments.Analysis.SpectralMeasurements;
using NationalInstruments;
using NationalInstruments.NetworkVariable;
using NationalInstruments.NetworkVariable.WindowsForms;
using NationalInstruments.Tdms;
using NationalInstruments.UI;
using NationalInstruments.UI.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using HIDInterface;
using SwitchingMatrix;
using Picoscope;
//using TimeDomainForm;


namespace TomoBra_DAQ
{
/// <summary>
    /// Updated:        Karim El Hallaoui (17 April 2017)
    /// Created:        Evgeny Kirshin, Nicolas Vendeville (May 13, 2013)
    /// Summary:        Main form for the TomoBra app.
    ///                 The software allows the user to connect to measurement devices and collect Time-Domain (TD) and Frequency-Domain (FD) signals using microwaves.
    ///                 These signals can processed, saved to textfile, and visualized using the graph.
    ///                 The program UI is organized into 7 tabs and the code relevant to these sections are grouped in regions below.
    /// </summary>
    public partial class form_main : Form
    {
        #region Class Constants
        SWMatrix SWMatrix = new SWMatrix();
        COMRCWrapper COMRCW;

        double Ts = 0.0125;  // Sampling interval in ns. By default = 12.5ps => Fs = 80GHz
        double[] Time_scale;
        bool Running_TD = false;
        bool Running_FD = false;
        bool Picoscope_ON = false;
        bool SM_ON = false;
        int num_of_signals = 0;
        TimeSpan time_start;
        uint CurrentCaseNum = 1; // Incremental Collect


        double[] Wfm_Ch1;
        double[] Wfm_Ch2;

        double[] Wfm_SigDiff;
        double[] Wfm_SigDiff_f;
        double[] Wfm_SigDiff_Saved;


        double[] Wfm_SigAl;

        int[] sig_limits = { 0, 1000 };

        #region Constants - Filtering Coefficients

        double[] fir_coeffs_LP_80; // Low-pass filter coefficients
        double[] fir_coeffs_HP_80; // Band-pass filter coefficients
        double[] fir_coeffs_LP_200; // Low-pass filter coefficients
        double[] fir_coeffs_HP_200; // Band-pass filter coefficients

        #endregion // Constants - Filtering Coefficients

        // Variables to collect statistical information (many waveforms)
        double[][] CCh1;        // multiple waveforms recorded at channel1
        double[][] CCh2;        // multiple waveforms recorded at channel2
        double[][] CDiff_f;     // filtered difference signals. This is to check if the alignment and filtering works properly
        int nSigCollected;      // number of signals already stored in CCh1, CCh2, CDiff_f
        bool bCollecting = false;   // indicates if signals collected are recorded or not

        #region Constants - TD Waveform Arrays

        // For semi-automatic and automatic acquisition modes [ RX , TX]
        double[,][] BaselineM = new double[Constants.nSensors, Constants.nSensors][];   // Three-dimensional array to store the multi-static waveforms for baseline
        double[,][] BaselineM_r = new double[Constants.nSensors, Constants.nSensors][];   // Three-dimensional array to store the corresponding references of the above
        double[,][] SignalM = new double[Constants.nSensors, Constants.nSensors][];     // Three-dimensional array to store the multi-static waveforms for signals
        double[,][] SignalM_r = new double[Constants.nSensors, Constants.nSensors][];     // Three-dimensional array to store the corresponding references of the above
        bool[,] SigMaskAv = new bool[Constants.nSensors, Constants.nSensors];       // Mask of collected (available) signals
        bool[,] BLMaskAv = new bool[Constants.nSensors, Constants.nSensors];       // Mask of collected (available) baselines
        double[] Wfm_Single;    // Single acquisition (only for SW averaging or single acquisition): for baaseline single waveform
        double[] Wfm_Single_r;  // Reference signal of Wfm_Single  
        double[] WfmAcc = new double[Constants.sig_len];        // Accumulator for averaging multiple acquisition (for SW mode)

        #endregion // Constants - TD Waveform Arrays

        #region Constants - Tooltips

        // TD tab
        ToolTip lb_TD_counter_TT = new ToolTip();
        ToolTip cb_TD_storeSig_TT = new ToolTip();
        ToolTip bn_TD_fullArray_TT = new ToolTip();
        ToolTip lb_TD_saving_TT = new ToolTip();

        // FD tab
        ToolTip lb_FD_count_TT = new ToolTip();
        ToolTip bn_FD_fullArray_TT = new ToolTip();


        ToolTip pb_status_TT = new ToolTip();

        #endregion // Constants - Tooltips

        List<uint> RXAntennas = new List<uint>();             // Set of antennas to collect signals from
        List<uint> TXAntennas = new List<uint>();             // Set of antennas to use as transmitters
        bool bCollectHWAvg;                                 // true - collect waveform with hardware averaging (PicoScope)
        bool bCollectSWAvg;                                 // true - collect waveform with software averaging (this software)
        bool bCollectNoAvg;                                 // true - collect waveform without averaging 
        uint NAvgHW;                                        // Number of averages for hardware averaging collection
        uint NAvgSW;                                        // Number of averages for software averaging collection

        // Statistics
        int Align_discarded = 0;                            // Number of discarded aligned signals (due to overly large delay)
        double MaxDelay = 0;

        Queue<WfmPkg> _queue = new Queue<WfmPkg>();
        object _lock = new object();

        // Multiple case collection
        bool Stopping = false;

        public class Constants
        {
            public const uint sig_len = 4096;
            public const uint nSensors = 16;
        }

        #endregion // Constants

        public form_main()
        {
            InitializeComponent();                  // Initializes the userform elements      
            MessageLog("Starting new session...");  
            StatusBar_Initialization();
            SM_enable_groupboxes(false);            // disable functions in SM tab (user can't use functions until PicoScope is connected)
            UI_TD_EnableCommands();                 // Disable TD Functions at start up
            bn_PS_connect.Enabled = true;

            Time_scale = new double[Constants.sig_len];
            for (int i = 0; i < Constants.sig_len; i++)
            {
                Time_scale[i] = i * Ts;
                WfmAcc[i] = 0;
            }

            // Load filter coefficients
            fir_coeffs_LP_80 = TimeDomainForm.LoadArray("fir_lp_80G_4G_6G_1_80db.txt");
            fir_coeffs_HP_80 = TimeDomainForm.LoadArray("fir_hp_80G_1G_2G_80db_1db.txt");
            fir_coeffs_LP_200 = TimeDomainForm.LoadArray("fir_lp_200G_4G_6G_1_80db.txt");
            fir_coeffs_HP_200 = TimeDomainForm.LoadArray("fir_hp_200G_1G_2G_80db_1db.txt");

            for (int i = 0; i < Constants.nSensors; i++)
            {
                for (int j = 0; j < Constants.nSensors; j++)
                {
                    SignalM[i, j] = new double[Constants.sig_len];
                    BaselineM[i, j] = new double[Constants.sig_len];
                    SignalM_r[i, j] = new double[Constants.sig_len];
                    BaselineM_r[i, j] = new double[Constants.sig_len];
                    BLMaskAv[i, j] = false;
                    SigMaskAv[i, j] = false;  
                }
            }
        }

        #region Switching Matrix

        private void bn_SM_Power_Click(object sender, EventArgs e)
        {
            // Turn ON main power
            if (!SWMatrix.Connect()) 
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            
            if (int.Parse((string)bn_SM_Power.Tag) == 0) SM_setInterface(true);
            else SM_setInterface(false);
        }

        /// <summary>
        /// Turns Power Button on/off, updates status bar labels (SM, TX/RX), and enables switching matrix functions.
        /// </summary>
        private void SM_setInterface(bool ON)
        {
            if (ON)
            {
                SM_ON = true;
                bn_SM_Power.Text = "Power OFF";
                bn_SM_Power.Tag = "1";
                StatusBar_SM(SM_ON);
                SM_enable_groupboxes(true);
                MessageLog("Switching Matrix ON");
                StatusBar_TX();
                StatusBar_RX();
            }
            else
            {
                SM_ON = false;
                bn_SM_Power.Text = "Power ON";
                bn_SM_Power.Tag = "0";
                StatusBar_SM(SM_ON);
                SM_enable_groupboxes(false);
                MessageLog("Switching Matrix OFF");
                StatusBar_TX();
                StatusBar_RX();
            }
        }

        /// <summary>
        /// Enables SM groupboxes
        /// </summary>
        private void SM_enable_groupboxes(bool ON)
        {
            if (ON && SM_ON)
            {
                gb_SM_RXA.Enabled = true;
                Full_Scan_gb_Service.Enabled = true;
                gb_SM_TXA.Enabled = true;
                // Until 'Initialize' is clicked
                Pause_Or_Resume_Full_Scan_Service.Enabled = false;
                Cancel_Full_Scan_Service.Enabled = false;
            }
            else
            {
                gb_SM_RXA.Enabled = false;
                Full_Scan_gb_Service.Enabled = false;
                gb_SM_TXA.Enabled = false;
            }
        }

        private void bn_SM_RXA_set_Click(object sender, EventArgs e)
        {
            // Set RX antenna
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            SWMatrix.SetRXAntenna((int)nud_SM_RXA_num.Value);
            Console.WriteLine("Tx {0}", nud_SM_TXA_num.Value);
            Console.WriteLine("Rx {0}", nud_SM_RXA_num.Value);
            StatusBar_RX();
        }

        private void bn_SM_RXA_loop_Click(object sender, EventArgs e)
        { // RX Antenna - loop
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            //RX has priority, ignores TX value, loop from RXA 1 to 16
            for (int i = 1; i <= 16; i++)
            {
                SWMatrix.SetRXAntenna((byte)i);
                StatusBar_RX();
                System.Threading.Thread.Sleep((int)nud_SM_RXA_del.Value);
            }

        }

        private void bn_SM_TXA_set_Click(object sender, EventArgs e)
        {
            // Set TX antenna
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            SWMatrix.SetTXAntenna((int)nud_SM_TXA_num.Value);
            Console.WriteLine("Tx {0}", nud_SM_TXA_num.Value);
            Console.WriteLine("Rx {0}", nud_SM_RXA_num.Value);
            StatusBar_TX();
        }

        private void bn_SM_TXA_loop_Click(object sender, EventArgs e)
        { // TX Antenna - loop
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            //RX has priority over TX, must set RX to antenna 16 before entering loop
            SWMatrix.SetRXAntenna((byte)16);
            StatusBar_RX();

            for (int i = 1; i <= 16; i++)
            {
                if (i == 16)
                {
                    SWMatrix.SetRXAntenna((byte)1);
                    StatusBar_RX();
                }

                SWMatrix.SetTXAntenna((byte)i);
                tssl_TXnum.Text = "" + i; // set TX label
                System.Threading.Thread.Sleep((int)nud_SM_TXA_del.Value);
                StatusBar_TX();
            }
        }

        #endregion // Switching Matrix

        #region Picoscope

        private void bn_PS_connect_Click(object sender, EventArgs e)
        {  // Button: Connect/Disconnect to/from PicoScope
            if ((string)bn_PS_connect.Tag == "1")
                PSDisconnect();
            else
            {
                PSConnect();
            }
        }

        private void bn_PS_configure_Click(object sender, EventArgs e)
        {
            if (!Picoscope_ON)
            {
                PSConnect();
                Thread.Sleep(100); // This is to allow PS time to connect before configuring to avoid errors
            }

            // Load text commands from file and execute them in sequence
            if (!File.Exists(tbx_PS_conFile.Text))
            {
                MessageBox.Show("Error: Configuration File Does Not Exist!");
                MessageLog("Error: Configuration File Does Not Exist!");
                return;
            }
            // Load commands from file
            List<string> lines = new List<string>();
            // Use StreamReader for disposing.
            using (StreamReader r = new StreamReader(tbx_PS_conFile.Text))
            {
                // Use while != null pattern for loop
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            uint Result = COMRCW.ExecCommands(lines);
            MessageLog("Picoscope Configured");
            bn_PS_configure.Text = "Re-configure";
            bn_PS_configure.Tag = "1";
        }

        private void bn_PS_browseConFile_Click(object sender, EventArgs e)
        {
            if (ofd_PS_browse.ShowDialog() == DialogResult.OK)
            {
                tbx_PS_conFile.Text = ofd_PS_browse.FileName;
            }
        }

        // Connect to PicoScope COM server
        private void PSConnect()
        {
            if (COMRCW == null) COMRCW = new COMRCWrapper();
            Picoscope_ON = true;
            UI_TD_EnableCommands();
            bn_PS_connect.Tag = "1";
            bn_PS_connect.Text = "Disconnect";
            StatusBar_Device();
            MessageLog("Connected to Picoscope");
        }

        /// <summary>
        /// Disconnect from PicoScope COM server
        /// </summary>
        private void PSDisconnect()
        {
            COMRCW = null;
            GC.Collect(); // Force GC to release the COM object

            bn_PS_connect.Tag = "0";
            bn_PS_connect.Text = "Connect";
            StatusBar_Device();
            MessageLog("Disconnected from Picoscope");
            Picoscope_ON = false;
            UI_TD_EnableCommands();
            StatusBar_Device();
            bn_PS_configure.Text = "Configure";
            bn_PS_configure.Tag = "0";
        }

        #endregion

        #region Status Bar

        // Initializes the status bar on program startup.
        private void StatusBar_Initialization()
        {
            //Status bar initialization commands
            StatusBar_Device();
            StatusBar_SM(false);
            StatusBar_BGW();
            StatusBar_TX();
            StatusBar_RX();
            StatusBar_resize_progressbar();
        }

        // Checks which device is currently in use. i.e. Picoscope, VNA, PNA, or not connected.
        private void StatusBar_Device()
        {
            if (Picoscope_ON)
            {
                tssl_device.Text = "PS9201A";
                tssl_device.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            }
            else
            {
                tssl_device.Text = "Not Connected";
                tssl_device.BackColor = System.Drawing.SystemColors.Menu;
            }
        }

        // Updates the TX Label. 
        private void StatusBar_TX()
        {
            int TX = SWMatrix.curr_TX;
            if (TX == -1)
                tssl_TXnum.Text = "N.A.";
            else
            {
                TX++;
                tssl_TXnum.Text = TX.ToString();
            }
        }

        // Updates the RX Label.
        private void StatusBar_RX()
        {
            int RX = SWMatrix.curr_RX;
            if (RX == -1)
                tssl_RXnum.Text = "N.A.";
            else
            {
                RX++;
                tssl_RXnum.Text = RX.ToString();
            }
        }

        // Changes SM status label on the status strip. 
        private void StatusBar_SM(bool ON)
        {
            if (ON)
            {
                tssl_SM.Text = "ON";
                tssl_SM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            }
            else
            {
                tssl_SM.Text = "OFF";
                tssl_SM.BackColor = System.Drawing.SystemColors.Menu;
                StatusBar_TX();
                StatusBar_RX();
            }
        }

        // Displays to the status bar if the program is taking 1. FD or  2. TD measurements or 3. Idle
        private void StatusBar_BGW()
        {
            if (Running_TD)
            {
                tssl_running.Text = "Running TD";
                tssl_running.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            }
            else

                if (Running_FD)
                {
                    tssl_running.Text = "Running FD";
                    tssl_running.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
                }

                else
                {
                    tssl_running.Text = "Idle";
                    tssl_running.BackColor = System.Drawing.SystemColors.Menu;
                }

        }

        // Resizes the progress bar to fill up remaining space
        private void StatusBar_resize_progressbar()
        {
            int pbSize = ss_main.Width - tssl_device.Width - tssl_RXnum.Width - tssl_SM.Width - tssl_TXnum.Width - tssl_RX.Width - tssl_TX.Width - tssl_statusSM.Width
                - tssl_connectionStatus.Width - tssl_running.Width - 25;
            pb_status.Width = pbSize;
        }

        private void form_main_SizeChanged(object sender, EventArgs e)
        {
            StatusBar_resize_progressbar();
        }

        #endregion

        #region Time domain

        #region TD Constants
        public class WfmPkg
        {
            public double[] Ch1;
            public double[] Ch2;
            public uint PkgNum;
            public uint iTX;
            public uint iRX;
            public AcqMode Mode;
            public bool LastPkg = false;
        }

        public enum AcqMode { amSingle, amAvgHW, amAvgSW };

        #endregion // TD Constants

        #region TD Real Time

        private void bn_TD_realTime_Click(object sender, EventArgs e)
        {
            // Check if RX / TX are set
            if (SWMatrix.curr_RX == -1 || SWMatrix.curr_TX == -1)
                MessageBox.Show("Must set RX and TX before using real time analysis");
            else
                if (SWMatrix.curr_RX == SWMatrix.curr_TX)
                    MessageBox.Show("TX and RX values cannot be the same.");
                else
                {
                    if (!Running_TD)
                    {
                        Running_TD = true;  // Start collection
                        StatusBar_BGW();    // Update status bar

                        COMRCW.ExecCommand("Header Off"); // Switching off headers in results

                        // Set acquisition mode
                        if (rb_TD_HWAvg.Checked)
                            SetAcqMode((uint)nud_TD_NAvgHW.Value);
                        else
                            SetAcqMode(1); // For single and SW

                        UI_TD_EnableCommands(); // Disable other TD functions
                        bn_TD_realTime.Enabled = true; // Then enable real time
                        bn_TD_realTime.Text = "Stop";

                        bgw_TD_realTime.RunWorkerAsync();

                    }
                    else
                    {   // Request to stop process
                        bgw_TD_realTime.CancelAsync();
                        // Disable the Run/Stop button.
                        bn_TD_realTime.Enabled = false;
                        bn_TD_realTime.Text = "Real Time";
                    }
                }

        }

        private void bgw_realTime_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation to the Result property of the DoWorkEventArgs
            // object. This is will be available to the RunWorkerCompleted eventhandler.
            COMRCW.ExecCommand("Header Off"); // Switching off headers in results
            uint PkgNum = 0;
            // Continuously gather data and post it to the main thread
            // until stop command received from main thread
            while (!e.Cancel)
            {
                WfmPkg pkg = new WfmPkg();

                if (rb_TD_SWAvg.Checked) // If doing SW averaging
                {
                    Double Delay;
                    double[] Wfm_ref;

                    // Start data acquisition
                    for (int i = 0; i < NAvgSW; i++)
                    {
                        // Get Waveforms
                        COMRCW.RunSingle();

                        Wfm_ref = COMRCW.GetWaveForm(1);

                        if (i == 0)
                        {
                            Wfm_Single = COMRCW.GetWaveForm(1);
                            Wfm_Single_r = COMRCW.GetWaveForm(2);
                            Wfm_SigAl = Wfm_Single;
                        }
                        else
                            Delay = align_signals_corr(Wfm_Single, array_sub_mean(Wfm_ref), 10, 50, out Wfm_SigAl);

                        // Accumulate the waveform
                        for (int j = 0; j < Constants.sig_len; j++)
                            WfmAcc[j] = WfmAcc[j] + Wfm_SigAl[j];

                        // Average at last value
                        if (i == (NAvgSW - 1))
                        {
                            for (int j = 0; j < Constants.sig_len; j++)
                                WfmAcc[j] = WfmAcc[j] / NAvgSW;
                        }
                    }

                    pkg.Ch1 = WfmAcc;
                    pkg.Ch2 = Wfm_Single_r;
                    pkg.PkgNum = PkgNum++;
                }

                else // HW Averaging / Single Acquisition
                {
                    // Acquire data
                    COMRCW.RunSingle();
                    // Get the waveform from channel 1
                    pkg.Ch1 = COMRCW.GetWaveForm(1);
                    // Get the waveform from channel 2
                    pkg.Ch2 = COMRCW.GetWaveForm(2);
                    pkg.PkgNum = PkgNum++;
                }

                worker.ReportProgress(100, pkg);

                // Check if stop command is pending
                if (worker.CancellationPending)
                    e.Cancel = true;
            }
        }

        private void bgw_TD_realTime_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // New waveforms arrived
            WfmPkg pkg = (WfmPkg)e.UserState;
            Wfm_Ch1 = pkg.Ch1;
            Wfm_Ch2 = pkg.Ch2;
            // Update the data
            /*scatterGraph1.Plots[1].PlotXY(Time_scale, Wfm_Ch1);
            scatterGraph1.Plots[2].PlotXY(Time_scale, Wfm_Ch2);*/

            lb_TD_WfmNum.Text = pkg.PkgNum.ToString();


          
            if (BLMaskAv[SWMatrix.curr_RX, SWMatrix.curr_TX]) 
            {   // Time-align and subtract the baseline from the acquired signal
                double[] Wfm_RefSig;
                double Delay;
                if (rb_TD_CalCorr.Checked)
                    Delay = align_signals_corr(BaselineM[SWMatrix.curr_RX, SWMatrix.curr_TX], array_sub_mean(Wfm_Ch1), 10, 50, out Wfm_SigAl);
                else if (rb_TD_CalRef.Checked)
                {
                    Delay = align_signals(BaselineM_r[SWMatrix.curr_RX, SWMatrix.curr_TX], Wfm_Ch1, array_sub_mean(Wfm_Ch2), out Wfm_SigAl, out Wfm_RefSig, 1000, sig_limits);
                }
                else
                {
                    Wfm_SigAl = Wfm_Ch1;
                    Delay = 0;
                }

                Wfm_SigDiff = array_sub(Wfm_SigAl, BaselineM[SWMatrix.curr_RX, SWMatrix.curr_TX]);
                if (rb_TD_FltLP.Checked)
                {
                    if (rb_TD_80GHz.Checked)
                        Wfm_SigDiff_f = FIR_zf(fir_coeffs_LP_80, Wfm_SigDiff);
                    else Wfm_SigDiff_f = FIR_zf(fir_coeffs_LP_200, Wfm_SigDiff);
                }
                else if (rb_TD_FltBP.Checked)
                {
                    if (rb_TD_80GHz.Checked)
                    {
                        Wfm_SigDiff_f = FIR_zf(fir_coeffs_LP_80, Wfm_SigDiff);
                        Wfm_SigDiff_f = FIR_zf(fir_coeffs_HP_80, Wfm_SigDiff_f);
                    }
                    else
                    {
                        Wfm_SigDiff_f = FIR_zf(fir_coeffs_LP_200, Wfm_SigDiff);
                        Wfm_SigDiff_f = FIR_zf(fir_coeffs_HP_200, Wfm_SigDiff_f);
                    }
                }
                else Wfm_SigDiff_f = Wfm_SigDiff;


                // Plot the obtained signal and the difference
                if (tsmi_graph_Signal.Checked)
                {
                    sg_TD_graph.Plots[0].PlotXY(Time_scale, Wfm_SigAl);
                }
                else
                {
                    sg_TD_graph.Plots[0].ClearData();
                    sg_TD_graph.Plots[1].ClearData();
                }
                sg_TD_graph.Plots[2].PlotXY(Time_scale, Wfm_SigDiff);
                lb_TD_alignment.Text = (Delay * Ts * 1000).ToString();
                // If collecting data
                if (bCollecting)
                {
                    lb_TD_sigCollected.Text = nSigCollected.ToString();
                    // Save data to files if finished collection
                    if (++nSigCollected == nud_TD_NCollect.Value)
                    {
                        MessageLog("Finished collecting " + nud_TD_NCollect.Value + " waveforms.");
                        StopCollecting();
                    }
                }
            }
            else
            {
                sg_TD_graph.Plots[0].PlotXY(Time_scale, Wfm_Ch1);
            }
            lb_TD_WfmNum.Text = pkg.PkgNum.ToString();

        }

        private void bgw_TD_realTime_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Set interface to idle state
            Running_TD = false;
            UI_TD_EnableCommands();
            StatusBar_BGW();
        }

        #endregion // TD Real Time

        #region TD Single

        private void bn_TD_single_Click(object sender, EventArgs e)
        {
            // If storing to global array, ensure that RX / TX are set
            if (cb_TD_Store.Checked && ((SWMatrix.curr_RX == -1) || (SWMatrix.curr_TX == -1)))
            {
                if ((SWMatrix.curr_RX == -1) || (SWMatrix.curr_TX == -1))
                {
                    MessageBox.Show("Please set RX / TX Antennas in order to store to global array");
                    return;
                }

                else
                {
                    if (SWMatrix.curr_RX == SWMatrix.curr_TX)
                        MessageBox.Show("TX and RX values cannot be the same.");
                    return;
                }
            }
            else
                TD_singleSave();
        }

        /// <summary>
        /// Acquires a single wave form and 1. saves to txt 2. stores to global array 3. plots to TD graph
        /// </summary>
        private void TD_singleSave()
        {
            // Deactivate button
            bn_TD_single.BackColor = Color.FromName("Red");
            bn_TD_single.Enabled = false;
            bn_TD_single.Text = "Saving...";

            // Compose the file name
            String FName;
            FName = fileNameTD();

            // Check if file exists
            if (File.Exists(FName))
            {
                switch (MessageBox.Show("Overwrite?",
                            "Warning: file exists!",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        break;
                    case DialogResult.No:
                    case DialogResult.Cancel:
                        return;
                }
            }

            // Find out which signals to save to text and get filtering values
            Set_Averaging();

            // Output message to user
            if (cb_TD_Store.Checked)
                TD_Log_Filter_Display();

            // Get current RX / TX
            int RX = SWMatrix.curr_RX;
            int TX = SWMatrix.curr_TX;

            // Gets single-acquisition waveforms from both channels
            // Construct time-scale
            // Get current time period for channel 1
            COMRCW.ExecCommand("Header Off"); // Switching off headers in results

            // Hardware Averaging
            if (cb_TD_HWAvg.Checked || rb_TD_HWAvg.Checked)
            {
                // Set acquisition mode
                SetAcqMode((uint)nud_TD_NAvgHW.Value);

                // Start data acquisition
                COMRCW.RunSingle();
                //Ts = Ts / 400;

                // Get waveforms
                // channel 1
                Wfm_Ch1 = COMRCW.GetWaveForm(1);
                // channel 2
                Wfm_Ch2 = COMRCW.GetWaveForm(2);

                // Save to text file
                if (bCollectHWAvg)
                {
                    String FileNameHW;
                    FileNameHW = tbx_TD_fileName.Text.Replace("%TX%", "A" + (TX + 1));
                    FileNameHW = tbx_TD_folder.Text + "\\" + FileNameHW.Replace("%RX%", "A" + (RX + 1));
                    FileNameHW = FileNameHW.Replace(".txt", "_hw" + NAvgHW.ToString() + ".txt");
                    TimeDomainForm.StoreArrays(Wfm_Ch1, Wfm_Ch2, FileNameHW);
                }

                // Filter / Process / Plot
                if (cb_TD_Store.Checked && rb_TD_HWAvg.Checked)
                {
                    // Save to global array
                    if (rb_TD_CollectBL.Checked) SaveBaseline(Wfm_Ch1, Wfm_Ch2, (uint)RX, (uint)TX);
                    else SaveSignal(Wfm_Ch1, Wfm_Ch2, (uint)RX, (uint)TX);

                    // Process and plot signal
                    ProcessSignals((uint)RX, (uint)TX);
                }
            }

            // Software Averaging
            if (cb_TD_SWAvg.Checked || rb_TD_SWAvg.Checked)
            {
                // Set acquisition mode
                SetAcqMode(1);

                Double Delay;
                double[] Wfm_ref;


                // Start data acquisition
                for (int i = 0; i < NAvgSW; i++)
                {
                    // Get Waveforms
                    COMRCW.RunSingle();

                    Wfm_ref = COMRCW.GetWaveForm(1);

                    if (i == 0)
                    {
                        Wfm_Single = COMRCW.GetWaveForm(1);
                        Wfm_Single_r = COMRCW.GetWaveForm(2);
                        Wfm_SigAl = Wfm_Single;
                    }
                    else
                    {
                        // Align
                        Delay = align_signals_corr(Wfm_Single, array_sub_mean(Wfm_ref), 10, 50, out Wfm_SigAl);
                    }

                    // Accumulate the waveform
                    for (int j = 0; j < Constants.sig_len; j++)
                        WfmAcc[j] = WfmAcc[j] + Wfm_SigAl[j];

                    // Average at last value
                    if (i == (NAvgSW - 1))
                    {
                        for (int j = 0; j < Constants.sig_len; j++)
                            WfmAcc[j] = WfmAcc[j] / NAvgSW;
                    }
                }

                // Save to text file
                if (bCollectSWAvg)
                {
                    String FileNameSW;
                    FileNameSW = tbx_TD_fileName.Text.Replace("%TX%", "A" + (TX + 1).ToString());
                    FileNameSW = tbx_TD_folder.Text + "\\" + FileNameSW.Replace("%RX%", "A" + (RX + 1).ToString());
                    FileNameSW = FileNameSW.Replace(".txt", "_sw" + NAvgSW.ToString() + ".txt");
                    TimeDomainForm.StoreArrays(WfmAcc, Wfm_Single_r, FileNameSW);
                }

                    // Filter / Process / Plot
                if (cb_TD_Store.Checked && rb_TD_SWAvg.Checked)
                {
                    // Store to Arrays
                    if (rb_TD_CollectBL.Checked)
                        SaveBaseline(WfmAcc, Wfm_Single_r, (uint)RX, (uint)TX);
                    else
                        SaveSignal(WfmAcc, Wfm_Single_r, (uint)RX, (uint)TX);

                    // Process/plot signal
                    ProcessSignals((uint)RX, (uint)TX);
                }
            }

            // Single / No Averaging
            if (cb_TD_NoAvg.Checked || rb_TD_NoAvg.Checked)
            {
                // Set acquisition mode
                SetAcqMode(1);

                // Start data acquisition
                COMRCW.RunSingle();

                // Get waveforms
                Wfm_Single = COMRCW.GetWaveForm(1);
                Wfm_Single_r = COMRCW.GetWaveForm(2);

                // Save to text file 
                if (bCollectNoAvg)
                { 
                    String FileNameSingle;
                    FileNameSingle = tbx_TD_fileName.Text.Replace("%TX%", "A" + (TX + 1).ToString());
                    FileNameSingle = tbx_TD_folder.Text + "\\" + FileNameSingle.Replace("%RX%", "A" + (RX + 1).ToString());
                    FileNameSingle = FileNameSingle.Replace(".txt", "_s.txt");
                    TimeDomainForm.StoreArrays(Wfm_Single, Wfm_Single_r, FileNameSingle);
                }

                // Filter / Process / Plot
                if (cb_TD_Store.Checked && rb_TD_NoAvg.Checked)
                {
                    // Store to global array
                    if (rb_TD_CollectBL.Checked) SaveBaseline(Wfm_Single, Wfm_Single_r, (uint)RX, (uint)TX);
                    else SaveSignal(Wfm_Single, Wfm_Single_r, (uint)RX, (uint)TX);

                    // process/plot signal
                    ProcessSignals((uint)RX, (uint)TX);
                }
            }

            // Increment counter if needed
            if (rb_TD_Suff1.Checked)
            {
                if (cb_TD_CPP1.Checked)
                    nud_TD_counter.Value++;
            }
            else if (rb_TD_Suff2.Checked)
            {
                if (cb_TD_CPP2.Checked)
                    nud_TD_counter.Value++;
            }
            else
            {
                if (cb_TD_CPP3.Checked)
                    nud_TD_counter.Value++;
            }

            // Reset Button
            bn_TD_single.BackColor = Color.FromName("Control");
            bn_TD_single.Enabled = true;
            bn_TD_single.Text = "Single";

        }

        #endregion // TD Single

        #region TD Full Array

        // Collect all signals automatically.
        private void bn_TD_fullArray_Click(object sender, EventArgs e)
        {

            if (Running_TD)
            {
                // Disable the Run/Stop button.
                bn_TD_fullArray.Enabled = false;

                MessageLog("Stopping full array acquisition...");

                // Request to stop process
                Stopping = true;
                bgw_TD_collectAuto.CancelAsync();


                SM_setInterface(false);
                StatusBar_TX();
                StatusBar_RX();
                return;
            }

            else Stopping = false;

            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC2", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                MessageLog("No connection with the MCU!");
                return;
            }
            else SM_setInterface(true);

            // Check if user has picked a folder destination
            if (tbx_TD_folder.Text == "")
                if (fbd_TD.ShowDialog() == DialogResult.OK)
                    tbx_TD_folder.Text = fbd_TD.SelectedPath;

            String SigType;
            if (rb_TD_CollectBL.Checked)
                SigType = "Baseline";
            else
                SigType = "Signal";

            MessageLog("Acquiring Full Array Signal...");
            Set_Averaging();

            // Set the interface to running 
            TD_fullArray_Set();

            if (cb_TD_IncrementalSave.Checked)
            {   // Multiple cases collection
                MessageLog("TD Full Array: Collecting " + nud_TD_NumCases.Value.ToString() + " " + SigType + "s");
                TD_Log_Filter_Display();
                CurrentCaseNum = (uint)nud_TD_StartCaseNum.Value;
                string Folder = (rb_TD_CollectBL.Checked) ? "\\Baseline" + CurrentCaseNum.ToString() : "\\Tumor" + CurrentCaseNum.ToString();
                CollectCase(Folder);
            }
            else
            {
                MessageLog("TD Full Array: Collecting " + SigType);
                TD_Log_Filter_Display();
                CollectCase("");
            }
        }

        private void CollectCase(string Folder)
        {
            // Output message for multiple case collection
            if (cb_TD_IncrementalSave.Checked)
            {
                if (rb_TD_CollectBL.Checked)
                    MessageLog("Collecting Baseline " + CurrentCaseNum);
                else
                    MessageLog("Collecting Tumour " + CurrentCaseNum);
            }

            // Determine start time (for time remaining estimate)
            time_start = System.DateTime.Now.TimeOfDay;

            // Change tbx folder
            tbx_TD_folder.Text = tbx_TD_folder.Text + Folder;
            string SaveFolder = tbx_TD_folder.Text;

            // Prepare variables before running the background thread
            pb_status.Value = 0;
            Align_discarded = 0;
            MaxDelay = 0;
            lb_TD_AlDisc.Text = "0";
            lb_TD_MaxDelay.Text = "0";

            // Fill RXAntennas array (all antennas)
            RXAntennas.Clear();
            for (int i = 0; i < Constants.nSensors; i++)
                RXAntennas.Add((uint)i);

            // Fill TXAntennas array (all antennas)
            TXAntennas.Clear();
            for (int i = 0; i < Constants.nSensors; i++)
                TXAntennas.Add((uint)i);

            // Check if the first file of the collected series exists: in this case ask if we want to replace it
            // We are checking any of the modes in which signals are needed to collect (if either HW or SW avg. or single acq. exists - ask)
            String FName = tbx_TD_fileName.Text.Replace("%TX%", "A" + (TXAntennas[0] + 1).ToString());
            FName = SaveFolder + "\\" + FName.Replace("%RX%", "A" + (RXAntennas[0] + 1).ToString());
            String FNameSingle = FName.Replace(".txt", "_s.txt");
            String FNameSWAvg = FName.Replace(".txt", "_sw" + NAvgSW.ToString() + ".txt");
            String FNameHWAvg = FName.Replace(".txt", "_hw" + NAvgHW.ToString() + ".txt");

            // Create sub-folder if needed 
            if (!System.IO.File.Exists(SaveFolder))
                System.IO.Directory.CreateDirectory(SaveFolder);

            if (File.Exists(FNameSingle) || File.Exists(FNameSWAvg) || File.Exists(FNameHWAvg))
            {
                switch (MessageBox.Show("Overwrite?", "Warning: file exists!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        // "Yes" processing
                        break;
                    case DialogResult.No:
                    // "No" processing
                    case DialogResult.Cancel:
                        // "Cancel" processing
                        return;
                }
            }

            // Start the background worker
            bgw_TD_collectAuto.RunWorkerAsync();
        }

        /// <summary>
        /// Resets interface (full array button and status bar) (used after stopping or completing full array TD acquisition)
        /// </summary>
        private void TD_fullArray_Reset()
        {
            // reset progress bar
            pb_status.Value = 0;

            // Set interface to idle state
            bn_TD_fullArray.Text = "Full Array";
            bn_TD_fullArray.BackColor = Color.FromName("Control");
            Running_TD = false;
            UI_TD_EnableCommands();
            StatusBar_BGW();
        }

        /// <summary>
        /// Set interface to running (full array button changes to "Stop" status bar updates to running, TD functions disabled)
        /// </summary>
        private void TD_fullArray_Set()
        {
            // Set interface to running
            Running_TD = true;
            StatusBar_BGW();

            // Set interface
            UI_TD_EnableCommands();

            // Set button labels
            bn_TD_fullArray.Text = "Stop";
            bn_TD_fullArray.BackColor = Color.FromName("Red");
            bn_TD_fullArray.Enabled = true;
        }

        #region Full Array - Background Worker

        private void bgw_TD_collectAuto_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.

            COMRCW.ExecCommand("Header Off"); // Switching off headers in results

            int nTX = TXAntennas.Count;
            int nRX = RXAntennas.Count;

            // Prepare for scanning:
            Thread.Sleep(1000); // 5-second power-ON delay

            // Stop data acquisition on PicoScope 
            COMRCW.ExecCommand("*StopSingle Stop");

            // 3. For each antenna in TXAntennas
            for (int iTX = 0; iTX < nTX; iTX++)
            {
                // Choose needed TX antenna
                SWMatrix.SetTXAntenna((int)TXAntennas[iTX] + 1);

                // For each antenna in RXAntennas
                for (int iRX = 0; iRX < nRX; iRX++)
                {
                    // Skip cases when TXAntenna == RXAntenna
                    if (TXAntennas[iTX] == RXAntennas[iRX]) continue;

                    // Choose needed RX antenna
                    SWMatrix.SetRXAntenna((int)RXAntennas[iRX] + 1);

                    // Delay: allow some time to switch to the new RX antenna and output pulses
                    Thread.Sleep(50);

                    // Collect the signals using one of the chosen modes
                    if (bCollectHWAvg)
                    {   // Collect hardware-averaged signals
                        // Set PicoScope mode to multiple-averaging mode
                        // Set acquisition mode
                        SetAcqMode(NAvgHW);

                        // Collect waveforms from both channels and send them in a package to the main thread
                        WfmPkg pkg = new WfmPkg();
                        // Start data acquisition
                        COMRCW.RunSingle();

                        // Transfer the waveforms
                        pkg.Ch1 = COMRCW.GetWaveForm(1);
                        pkg.Ch2 = COMRCW.GetWaveForm(2);
                        pkg.PkgNum = 1;
                        pkg.iRX = RXAntennas[iRX];
                        pkg.iTX = TXAntennas[iTX];
                        pkg.Mode = AcqMode.amAvgHW;

                        lock (_lock)
                        {
                            // Add new packet to the queue
                            _queue.Enqueue(pkg);
                        }
                        // Send package to the main thread for processing
                        worker.ReportProgress((int)Math.Floor((double)(iTX * nRX + iRX + 1) / (nRX * nTX) * 100), null);
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;   // stop data acquisition if requested
                        }
                    }
                    if (bCollectSWAvg || bCollectNoAvg)
                    {   // Collect software-averaged signals
                        SetAcqMode(1);
                        for (int i = 0; i < NAvgSW; i++)
                        {   // Collect waveforms from both channels and send them in a package to the main thread
                            WfmPkg pkg = new WfmPkg();
                            // Start data acquisition
                            COMRCW.RunSingle();

                            // Transfer the waveforms
                            pkg.Ch1 = COMRCW.GetWaveForm(1);
                            pkg.Ch2 = COMRCW.GetWaveForm(2);
                            pkg.PkgNum = (uint)i + 1;
                            pkg.iRX = RXAntennas[iRX];
                            pkg.iTX = TXAntennas[iTX];
                            pkg.Mode = AcqMode.amAvgSW;

                            lock (_lock)
                            {
                                // Add new packet to the queue
                                _queue.Enqueue(pkg);
                            }
                            // Send package to the main thread for processing
                            worker.ReportProgress((int)Math.Floor((double)(iTX * nRX + iRX * NAvgSW + i + 1) / (nRX * nTX * NAvgSW) * 100), null);
                            if (worker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;   // stop data acquisition if requested
                            }
                        }
                    }
                }
            }
        }

        private void bgw_TD_collectAuto_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            WfmPkg pkg = null;
            pb_status.Value = e.ProgressPercentage;

            lock (_lock)
            {
                if (_queue.Count > 0)
                    pkg = _queue.Dequeue();
            }

            if (pkg.LastPkg)
            {   // End the thread and leave
                bgw_TD_collectAuto.CancelAsync();
                return;
            }

            Wfm_Ch1 = pkg.Ch1;
            Wfm_Ch2 = pkg.Ch2;

            // If HWAvg packet & the collection is needed
            if (pkg.Mode == AcqMode.amAvgHW)
            {
                Hardware_Averaging_FullArray(pkg);
            }

            // If SWAvg packet & the collection is needed
            else if (pkg.Mode == AcqMode.amAvgSW && (bCollectSWAvg || bCollectNoAvg))
            {

                double Delay;
                if (pkg.PkgNum == 1)
                {   // If first packet in series - store as Single acquisition (will be also used as base reference for alignement)
                    No_Averaging_FullArray(pkg);
                }
                else
                {
                    // Time-align w.r.t. first single acquisition using max. cross-correlation
                    Delay = align_signals_corr(Wfm_Single, array_sub_mean(Wfm_Ch1), 10, 50, out Wfm_SigAl);
                }

                // Accumulate the waveform
                for (int i = 0; i < Constants.sig_len; i++)
                    WfmAcc[i] = WfmAcc[i] + Wfm_SigAl[i];

                if (pkg.PkgNum == NAvgSW)
                {   // If last packet - average and save to file
                    for (int i = 0; i < Constants.sig_len; i++)
                        WfmAcc[i] = WfmAcc[i] / NAvgSW;
                    // Save to file and/or store signal
                    Software_Averaging_FullArray(pkg);
                }
            }
        }

        private void bgw_TD_collectAuto_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Check if stopping
            if (Stopping == true)
            {
                MessageLog("TD Full Array Acquistion Stopped.");
                if (fbd_TD.SelectedPath != "")
                    tbx_TD_folder.Text = fbd_TD.SelectedPath;
                TD_fullArray_Reset();
            }
            else
                // Check if collecting multiple signals
                if (cb_TD_IncrementalSave.Checked)
                {
                    // Check if collection is finished
                    if (++CurrentCaseNum <= (nud_TD_NumCases.Value + nud_TD_StartCaseNum.Value - 1))
                    {
                        string Folder = (rb_TD_CollectBL.Checked) ? "\\Baseline" + CurrentCaseNum.ToString() :
                            "\\Tumor" + CurrentCaseNum.ToString();

                        tbx_TD_folder.Text = tbx_TD_folder.Text.Replace(@"\Baseline" + (CurrentCaseNum - 1), "");
                        tbx_TD_folder.Text = tbx_TD_folder.Text.Replace(@"\Tumor" + (CurrentCaseNum - 1), "");
                        // Estimate time remaining
                        time_remaining_TD(CurrentCaseNum);
                        // Continue with collection
                        CollectCase(Folder);
                    }

                    else // Case when finished full array (with incremental)
                    {
                        CurrentCaseNum = 0;
                        MessageLog("Finished incremental full-array acquisition");
                        tbx_TD_folder.Text = fbd_TD.SelectedPath;
                        TD_fullArray_Reset();
                    }

                }

                else // Case when finished full array (no incremental)
                {
                    MessageLog("Finished collecting TD Full Array Signal");
                    TD_fullArray_Reset();
                }
        }

        #endregion // Full Array - Background Worker

        #region Full Array - Averaging

        /// Full Array Averaging Method will save HW avg to txt AND/OR store to global array + plot
        /// <para> If (cb_TD_HWAvg.checked) >> Saves to txt file  </para>
        /// <para> If (rb_TD_HWAvg.checked) >>  1. Stores to global array AND 2. Calls on ProcessSignals() </para>
        private void Hardware_Averaging_FullArray(WfmPkg pkg)
        {
            if (bCollectHWAvg)
            {
                // Save to file
                String FName;
                FName = tbx_TD_fileName.Text.Replace("%TX%", "A" + (pkg.iTX + 1).ToString());
                FName = tbx_TD_folder.Text + "\\" + FName.Replace("%RX%", "A" + (pkg.iRX + 1).ToString());
                String FNameHWAvg = FName.Replace(".txt", "_hw" + NAvgHW.ToString() + ".txt");
                TimeDomainForm.StoreArrays(Wfm_Ch1, Wfm_Ch2, FNameHWAvg);
            }

            // Store to global array
            if (rb_TD_HWAvg.Checked)
            {
                if (rb_TD_CollectBL.Checked)
                {
                    SaveBaseline(Wfm_Ch1, Wfm_Ch2, pkg.iRX, pkg.iTX);
                }
                else
                {
                    SaveSignal(Wfm_Ch1, Wfm_Ch2, pkg.iRX, pkg.iTX);
                }
                ProcessSignals(pkg.iRX, pkg.iTX);
                lb_TD_CurAntA.Text = "A" + (pkg.iTX + 1).ToString() + " -> A" + (pkg.iRX + 1).ToString();


                // Update Status Bar
                int TX = (int)pkg.iTX + 1;
                int RX = (int)pkg.iRX + 1;
                tssl_TXnum.Text = "" + TX;
                tssl_RXnum.Text = "" + RX;

            }
        }

        /// Full Array Averaging Method will save SW avg to txt AND/OR store to global array + plot
        /// <para> If (cb_TD_SWAvg.checked) >> Saves to txt file  </para>
        /// <para> If (rb_TD_SWAvg.checked) >> 1. Stores to global array AND 2. Calls on ProcessSignals() </para>
        private void Software_Averaging_FullArray(WfmPkg pkg)
        {
            // save to text file
            if (bCollectSWAvg)
            {
                String FName;
                FName = tbx_TD_fileName.Text.Replace("%TX%", "A" + (pkg.iTX + 1).ToString());
                FName = tbx_TD_folder.Text + "\\" + FName.Replace("%RX%", "A" + (pkg.iRX + 1).ToString());
                String FNameSWAvg = FName.Replace(".txt", "_sw" + NAvgSW.ToString() + ".txt");
                TimeDomainForm.StoreArrays(WfmAcc, Wfm_Single_r, FNameSWAvg);
            }

            // Store to global array
            if (rb_TD_SWAvg.Checked)
            {
                if (rb_TD_CollectBL.Checked)
                {
                    SaveBaseline(WfmAcc, Wfm_Single_r, pkg.iRX, pkg.iTX);
                }
                else
                {
                    SaveSignal(WfmAcc, Wfm_Single_r, pkg.iRX, pkg.iTX);
                }
                ProcessSignals(pkg.iRX, pkg.iTX);
                lb_TD_CurAntA.Text = "A" + (pkg.iTX + 1).ToString() + " -> A" + (pkg.iRX + 1).ToString();

                int TX = (int)pkg.iTX;
                TX++;
                int RX = (int)pkg.iRX;
                RX++;
                tssl_TXnum.Text = "" + TX;
                tssl_RXnum.Text = "" + RX;

            }
        }

        // Full Array Averaging Method will save Single Acquisition to txt AND/OR store to global array + plot
        // If (cb_TD_NoAvg.checked) >> Saves to txt file   </para>
        // If (rb_TD_NoAvg.checked) >>  1. Stores to global array AND 2. Calls on ProcessSignals() </para>
        private void No_Averaging_FullArray(WfmPkg pkg)
        {

            Wfm_Single = pkg.Ch1;
            Wfm_Single_r = pkg.Ch2;

            if (bCollectNoAvg)
            {   // Save to file 
                String FName;
                FName = tbx_TD_fileName.Text.Replace("%TX%", "A" + (pkg.iTX + 1).ToString());
                FName = tbx_TD_folder.Text + "\\" + FName.Replace("%RX%", "A" + (pkg.iRX + 1).ToString());
                String FNameSingle = FName.Replace(".txt", "_s.txt");
                TimeDomainForm.StoreArrays(Wfm_Single, Wfm_Single_r, FNameSingle);
            }

            // Store to global array
            if (rb_TD_NoAvg.Checked)
            {
                if (rb_TD_CollectBL.Checked)
                {
                    SaveBaseline(Wfm_Single, Wfm_Single_r, pkg.iRX, pkg.iTX);
                }
                else
                {
                    SaveSignal(Wfm_Single, Wfm_Single_r, pkg.iRX, pkg.iTX);
                }
                ProcessSignals(pkg.iRX, pkg.iTX);
                lb_TD_CurAntA.Text = "A" + (pkg.iTX + 1).ToString() + " -> A" + (pkg.iRX + 1).ToString();

                int TX = (int)pkg.iTX;
                TX++;
                int RX = (int)pkg.iRX;
                RX++;
                tssl_TXnum.Text = "" + TX;
                tssl_RXnum.Text = "" + RX;


            }

            Wfm_SigAl = Wfm_Single;
        }
        #endregion

        #endregion // Full Array

        /// <summary>
        /// Enables / Disables TD related functions. Note: function disables fullarray and realtime when running
        /// </summary>
        private void UI_TD_EnableCommands()
        {
            // Disable all commands if the picoscope is off
            if (!Picoscope_ON)
            {
                bn_TD_Collect.Enabled = false;
                bn_TD_MemSig.Enabled = false;
                bn_TD_realTime.Enabled = false;
                bn_TD_single.Enabled = false;
                bn_TD_fullArray.Enabled = false;

                gb_TD_Averaging.Enabled = false;
                gb_TD_Calibration.Enabled = false;
                gb_TD_Filtering.Enabled = false;
                gb_TD_SamplingRate.Enabled = false;
            }
            else
            {
                // If software is running, disable filtering/calibration/processing and PS config / exec command / save amp files
                // Enable gb_TD_CollectSignals
                if (Running_TD)
                {
                    bn_TD_fullArray.Enabled = false;
                    bn_TD_realTime.Enabled = false;
                    bn_TD_single.Enabled = false;
                    bn_PS_configure.Enabled = false;

                    gb_TD_Averaging.Enabled = false;
                    gb_TD_Calibration.Enabled = false;
                    gb_TD_SamplingRate.Enabled = false;
                    gb_TD_SamplingRate.Enabled = false;


                }
                // Else enable everything
                else
                {
                    bn_TD_Collect.Enabled = true;
                    bn_TD_MemSig.Enabled = true;
                    bn_TD_realTime.Enabled = true;
                    bn_TD_single.Enabled = true;
                    bn_TD_fullArray.Enabled = true;
                    bn_PS_configure.Enabled = true;

                    gb_TD_Averaging.Enabled = true;
                    gb_TD_Calibration.Enabled = true;
                    gb_TD_Filtering.Enabled = true;
                    gb_TD_SamplingRate.Enabled = true;
                }
            }


        }

        public double[] array_sub(double[] a, double[] b)
        {
            double[] a_b = new double[a.Length];
            for (uint i = 0; i < a.Length; i++)
                a_b[i] = a[i] - b[i];
            return a_b;
        }

        public double[] array_sub_mean(double[] a)
        {
            double[] a_s = new double[a.Length];
            double mn = 0;
            for (uint i = 0; i < a.Length; i++)
                mn += a[i];
            mn = mn / a.Length;
            for (uint i = 0; i < a.Length; i++)
                a_s[i] = a[i] - mn;
            return a_s;
        }

        public double align_signals_corr(double[] signal1, double[] signal2, uint interp_coeff, uint win_width, out double[] aligned_signal2)
        {
            // Find abs. max of signal1
            int mx_i;
            double sm_v = max(abs(signal1.Skip((int)win_width).Take(signal1.Length - (int)win_width * 2).ToArray()), out mx_i);
            mx_i = mx_i + (int)win_width;

            // Take out the windows from signals 1 and 2
            double[] sig1_w = signal1.Skip(mx_i - (int)win_width).Take((int)win_width * 2).ToArray();
            double[] sig2_w = signal2.Skip(mx_i - (int)win_width).Take((int)win_width * 2).ToArray();

            // Interpolate the cuts
            double[] scale_t = new double[sig1_w.Length];
            for (uint i = 0; i < sig1_w.Length; i++)
                scale_t[i] = i;
            double[] scale_s = new double[sig1_w.Length * interp_coeff];
            for (uint i = 0; i < scale_s.Length; i++)
                scale_s[i] = ((double)i) / (double)interp_coeff;

            double[] sig1_w_i = interp1_lin(scale_t, sig1_w, scale_s);
            double[] sig2_w_i = interp1_lin(scale_t, sig2_w, scale_s);

            // Find the maximum of the cross-correlation between these two cuts
            double[] ccoeff;
            xcorr(sig1_w_i, sig2_w_i, out ccoeff);
            sm_v = max(ccoeff, out mx_i);
            // mx_i == length(sig1_w_i)-1 means no shift
            double del = (double)(mx_i - sig1_w_i.Length + 1) / (double)interp_coeff;

            // Interpolate signal2 
            double[] scale_i = new double[signal2.Length];
            double[] scale_n = new double[signal2.Length];
            for (uint i = 0; i < signal2.Length; i++)
            {
                scale_i[i] = i;
                scale_n[i] = i - del;
            }
            aligned_signal2 = interp1_lin(scale_i, signal2, scale_n);

            return del;
        }

        private void xcorr(double[] signal1, double[] signal2, out double[] ccoeff)
        {   //returns cross-correlation coefficients between signal1 and signal2 for each of the lags in samples [-length(signal2)+1 length(signal1)-1]
            ccoeff = new double[signal1.Length + signal2.Length - 1];
            for (int l = -signal2.Length + 1; l < signal1.Length; l++)
            {
                ccoeff[signal2.Length + l - 1] = 0;
                for (int i = 0; i < signal2.Length; i++)
                {
                    if ((l + i >= 0) && (l + i < signal1.Length))
                    {
                        ccoeff[signal2.Length + l - 1] = ccoeff[signal2.Length + l - 1] + signal2[i] * signal1[l + i];
                    }
                }
            }
        }

        public double max(double[] array, out int max_index)
        {
            double mx = array[0];
            max_index = 0;
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] > mx)
                {
                    mx = array[i];
                    max_index = i;
                }
            }
            return mx;
        }

        public double min(double[] array, out int min_index)
        {
            double mn = array[0];
            min_index = 0;
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] < mn)
                {
                    mn = array[i];
                    min_index = i;
                }
            }
            return mn;
        }

        public double[] abs(double[] array)
        {
            double[] abs_array = new double[array.Length];
            for (uint i = 0; i < array.Length; i++)
            {
                abs_array[i] = Math.Abs(array[i]);
            }
            return abs_array;
        }

        public double[] interp1_lin(double[] x, double[] y, double[] new_x)
        {
            double[] new_y = new double[new_x.Length];
            for (uint i = 0; i < new_y.Length; i++)
            {
                new_y[i] = interp_1x(x, y, new_x[i]);
            }
            return new_y;
        }

        public double interp_1x(double[] x, double[] y, double new_x)
        {
            if (new_x < x[0]) return y[0];
            if (new_x > x[x.Length - 1]) return y[x.Length - 1];
            double df = Math.Abs(x[0] - new_x);
            double ndf = 0;
            uint ndi = 0;
            uint bl, bh;
            // Find the boundaries (assume array x is monotonic)
            for (uint i = 1; i < x.Length; i++)
            {
                ndf = Math.Abs(x[i] - new_x);
                if (ndf < df)
                {
                    df = ndf;
                    ndi = i;
                }
            }
            // Examine the difference
            if (x[ndi] == new_x)
            {
                return y[ndi];
            }
            else if ((x[ndi] - new_x) < 0)
            {   // new_x is higher -> ndi is lower boundary
                bl = ndi;
                bh = ndi + 1;
            }
            else
            {   // new_x is lower -> ndi is higher boundary
                bh = ndi;
                bl = ndi - 1;
            }
            // Calculate the interpolated value
            return y[bl] * (new_x - x[bh]) / (x[bl] - x[bh]) + y[bh] * (new_x - x[bl]) / (x[bh] - x[bl]);
        }

        private void bn_TD_MemSig_Click(object sender, EventArgs e)
        {

            if (rb_TD_FltLP.Checked || rb_TD_FltBP.Checked)
                Wfm_SigDiff_Saved = Wfm_SigDiff_f;
            else
                Wfm_SigDiff_Saved = Wfm_SigDiff;
            if (tsmi_graph_MemDiff.Checked)
            {
                if (Wfm_SigDiff_Saved != null)
                    sg_TD_graph.Plots[3].PlotXY(Time_scale, Wfm_SigDiff_Saved);
            }
        }

        private void bn_TD_Collect_Click(object sender, EventArgs e)
        {
            // Check if signal collection is already in progress
            if (bCollecting)
            {   // If yes: stop and save the data
                StopCollecting();
                bn_TD_Collect.Text = "Start";
                MessageLog("Stopping waveform collection.");
            }
            else
            {
                // If no:
                //      prepare arrays to acquire needed number of signals (allocate memory)
                //      set the flag to collect data
                CCh1 = new double[(int)nud_TD_NCollect.Value][];
                CCh2 = new double[(int)nud_TD_NCollect.Value][];
                CDiff_f = new double[(int)nud_TD_NCollect.Value][];
                for (int i = 0; i < CCh1.Length; i++)
                {
                    CCh1[i] = new double[Constants.sig_len];
                    CCh2[i] = new double[Constants.sig_len];
                    CDiff_f[i] = new double[Constants.sig_len];
                }
                bn_TD_Collect.Text = "Stop";
                MessageLog("Collecting " + nud_TD_NCollect.Value.ToString() + " waveforms.");
            }
        }

        // Stop data collection (Reset the counter and the flag)
        private void StopCollecting()
        {
            bCollecting = false;
            nSigCollected = 0;
            bn_TD_Collect.Text = "Collect";
            if (nSigCollected > 0)
            {   //      if # of collected signals already > 0, then save the arrays in files
                int Findex = GetNextFileIndex("Ch1_", "txt");
                Store2DArray(CCh1, "Ch1_" + Findex.ToString() + ".txt", nSigCollected);
                Store2DArray(CCh2, "Ch2_" + Findex.ToString() + ".txt", nSigCollected);
                Store2DArray(CDiff_f, "Diff_" + Findex.ToString() + ".txt", nSigCollected);
            }
        }

        // Stores 2D array into a text file: first dimension corresponds to rows, second dimension - to columns
        private void Store2DArray(double[][] Array, String filename, int signals2save)
        {
            int nRows = Array[1].Length;
            int nCols = signals2save;
            string[] lines = new string[nRows];
            for (uint i = 0; i < nRows; i++)
                for (uint j = 0; j < nCols; j++)
                    lines[i] = lines[i] + "\t" + Array[j][i].ToString();
            System.IO.File.WriteAllLines(filename, lines);
        }

        private int GetNextFileIndex(String Prefix, String Extension)
        {
            int FileNumber = 0;
            while (File.Exists(Prefix + (++FileNumber).ToString() + "." + Extension)) ;
            return FileNumber;
        }

        private void bn_TD_SelFolderA_Click(object sender, EventArgs e)
        {
            if (fbd_TD.ShowDialog() == DialogResult.OK)
            {
                tbx_TD_folder.Text = fbd_TD.SelectedPath;
            }

        }

        // Composes the file name for TD signals
        private String fileNameTD()
        {
            // Compose the file name
            String Suffix;
            String FName;

            // Check if user has picked a folder destination
            if (tbx_TD_folder.Text == "")
                if (fbd_TD.ShowDialog() == DialogResult.OK)
                {
                    tbx_TD_folder.Text = fbd_TD.SelectedPath;
                }

            if (rb_TD_Suff1.Checked)
                Suffix = tbx_TD_Suff1.Text;
            else if (rb_TD_Suff2.Checked)
                Suffix = tbx_TD_Suff2.Text;
            else Suffix = tbx_TD_Suff3.Text;

            // Increment counter if needed
            if (rb_TD_Suff1.Checked)
            {
                if (cb_TD_CPP1.Checked)
                    nud_TD_counter.Value++;
            }
            else if (rb_TD_Suff2.Checked)
            {
                if (cb_TD_CPP2.Checked)
                    nud_TD_counter.Value++;
            }
            else
            {
                if (cb_TD_CPP3.Checked)
                    nud_TD_counter.Value++;
            }

            FName = tbx_TD_fileName.Text.Replace("%s%", Suffix);
            FName = FName.Replace("%c%", nud_TD_counter.Value.ToString());
            FName = FName.Replace("%TX%", "A" + tssl_TXnum.Text);
            FName = tbx_TD_folder.Text + "\\" + FName.Replace("%RX%", "A" + tssl_RXnum.Text);
            return FName;
        }

        // Informs user which signal (HW-avg / SW-avg / Single) will be displayed to graph (based on which rb is selected).
        private void TD_Log_Filter_Display()
        {
            String Display;
            if (rb_TD_HWAvg.Checked)
                Display = "Plotting HW-averaged signal: NAvg = " + nud_TD_NAvgHW.Value;
            else if (rb_TD_SWAvg.Checked)
                Display = "Plotting SW-averaged signal: NAvg = " + nud_TD_NAvgSW.Value;
            else
                Display = "Plotting single-acquisition.";
            MessageLog(Display);
        }

        // Sets bool values and int values for bCollectHWAvg / SWAvg / NoAvg so that the signal can be saved a to txt file.
        private void Set_Averaging()
        {
            bCollectHWAvg = false;
            bCollectSWAvg = false;
            bCollectNoAvg = false;

            if (cb_TD_HWAvg.Checked)
            {
                NAvgHW = (uint)nud_TD_NAvgHW.Value;
                bCollectHWAvg = true;
                MessageLog("- Hardware averaging: " + nud_TD_NAvgHW.Value);
            }

            if (cb_TD_SWAvg.Checked)
            {
                NAvgSW = (uint)nud_TD_NAvgSW.Value;
                bCollectSWAvg = true;
                MessageLog("- Software averaging: " + nud_TD_NAvgSW.Value);
            }

            if (cb_TD_NoAvg.Checked)
            {
                bCollectNoAvg = true;
                if (!bCollectSWAvg) NAvgSW = 1; // This is to simplify the code for background worker
                MessageLog("- Single Acquisition");
            }
        }

        // Stores waveform wfm in global array BaselineM
        private void SaveBaseline(double[] wfm, double[] rf, uint i_RX, uint i_TX)
        {
            for (uint i = 0; i < Constants.sig_len; i++)
            {
                BaselineM[i_RX, i_TX][i] = wfm[i];
                BaselineM_r[i_RX, i_TX][i] = rf[i];
            }
            BLMaskAv[i_RX, i_TX] = true;
        }

        // Stores waveform wfm in global array SignalM
        private void SaveSignal(double[] wfm, double[] rf, uint i_RX, uint i_TX)
        {
            for (uint i = 0; i < Constants.sig_len; i++)
            {
                SignalM[i_RX, i_TX][i] = wfm[i];
                SignalM_r[i_RX, i_TX][i] = rf[i];
            }
            SigMaskAv[i_RX, i_TX] = true;
        }

        // Filters and calibrates the signals before displaying them to the TD graph.
        bool ProcessSignals(uint iRX, uint iTX)
        {
            double[] Signal_f = null;
            double[] Baseline_f = null;
            sg_TD_graph.ClearData();
            if (SigMaskAv[iRX, iTX])
            {   // If signal exists
                if (rb_TD_FltLP.Checked)
                {
                    if (rb_TD_80GHz.Checked)
                        Signal_f = FIR_zf(fir_coeffs_LP_80, SignalM[iRX, iTX]);
                    else Signal_f = FIR_zf(fir_coeffs_LP_200, SignalM[iRX, iTX]);
                }
                else if (rb_TD_FltBP.Checked)
                {
                    if (rb_TD_80GHz.Checked)
                    {
                        Signal_f = FIR_zf(fir_coeffs_LP_80, SignalM[iRX, iTX]);
                        Signal_f = FIR_zf(fir_coeffs_HP_80, Signal_f);
                    }
                    else
                    {
                        Signal_f = FIR_zf(fir_coeffs_LP_200, SignalM[iRX, iTX]);
                        Signal_f = FIR_zf(fir_coeffs_HP_200, Signal_f);
                    }
                }
                else Signal_f = SignalM[iRX, iTX];

                // Subtract mean
                Signal_f = array_sub_mean(Signal_f);
                // Plot
                sg_TD_graph.Plots[0].PlotXY(Time_scale, Signal_f);
            }

            if (BLMaskAv[iRX, iTX])
            {   // If baseline exists
                if (rb_TD_FltLP.Checked)
                {
                    if (rb_TD_80GHz.Checked)
                        Baseline_f = FIR_zf(fir_coeffs_LP_80, BaselineM[iRX, iTX]);
                    else Baseline_f = FIR_zf(fir_coeffs_LP_200, BaselineM[iRX, iTX]);
                }
                else if (rb_TD_FltBP.Checked)
                {
                    if (rb_TD_80GHz.Checked)
                    {
                        Baseline_f = FIR_zf(fir_coeffs_LP_80, BaselineM[iRX, iTX]);
                        Baseline_f = FIR_zf(fir_coeffs_HP_80, Baseline_f);
                    }
                    else
                    {
                        Baseline_f = FIR_zf(fir_coeffs_LP_200, BaselineM[iRX, iTX]);
                        Baseline_f = FIR_zf(fir_coeffs_HP_200, Baseline_f);
                    }
                }
                else Baseline_f = BaselineM[iRX, iTX];

                // Subtract mean
                Baseline_f = array_sub_mean(Baseline_f);
                // Plot
                sg_TD_graph.Plots[1].PlotXY(Time_scale, Baseline_f);
            }

            if (SigMaskAv[iRX, iTX] && BLMaskAv[iRX, iTX])
            {   // If both exist
                // Calibrate (+ display alignments in ps)
                double Delay = 0;
                double[] Wfm_RefSig;
                if (rb_TD_CalCorr.Checked)
                {   // Align using max. cross-correlation
                    Delay = align_signals_corr(Baseline_f, Signal_f, 10, 50, out Wfm_SigAl);
                }
                else if (rb_TD_CalRef.Checked)
                {   // Align using max. cross-correlation
                    Delay = align_signals(BaselineM_r[iRX, iTX], Signal_f, SignalM_r[iRX, iTX], out Wfm_SigAl, out Wfm_RefSig, 1000, sig_limits);
                }
                else Wfm_SigAl = Signal_f;
                if (Math.Abs(Delay * Ts) > 0.1)
                {   // If delay is over 100ps - discard the aligned signal (probably, erroneous alignment) 
                    Align_discarded++;
                    lb_TD_AlDisc.Text = Align_discarded.ToString();
                    Wfm_SigAl = Signal_f;   // Keep non-aligned signal
                }
                else if (Delay > 0)
                {
                    if (Delay > MaxDelay)
                    {   // Update the max. delay
                        MaxDelay = Delay;
                        lb_TD_MaxDelay.Text = MaxDelay.ToString();
                    }
                }
                Wfm_SigDiff = array_sub(Wfm_SigAl, Baseline_f);
                // Plot difference
                sg_TD_graph.Plots[2].PlotXY(Time_scale, Wfm_SigDiff);
                // Display alignment
                lb_TD_alignment.Text = (Delay * Ts * 1000).ToString();
                return true;
            }
            return false;
        }

        private static double[] FIR_zf(double[] b, double[] x)
        {
            int M = b.Length;
            int n = x.Length;
            //y[n]=b0x[n]+b1x[n-1]+....bmx[n-M]
            var y = new double[n];
            for (int yi = 0; yi < n; yi++)
            {
                double t = 0.0;
                for (int bi = M - 1; bi >= 0; bi--)
                {
                    if ((yi - bi + (M / 2 - 1) >= n) || (yi - bi + (M / 2 - 1) < 0)) continue;

                    t += b[bi] * x[yi - bi + (M / 2 - 1)];
                }
                y[yi] = t;
            }
            return y;
        }

        public double align_signals(double[] base_ref, double[] signal, double[] reference, out double[] aligned_signal, out double[] aligned_reference, uint interp_mul, int[] ref_lim)
        {
            // Determine the reference point
            double[] base_ref_a = base_ref.Skip(ref_lim[0]).Take(ref_lim[1] - ref_lim[0]).ToArray();
            int r_strt_p;
            double r_strt_pv = min(abs(base_ref_a), out r_strt_p);
            r_strt_p = r_strt_p + ref_lim[0];

            // Find the intersection point with zero knowing two surrounding points
            int bl;
            int bh;

            if (base_ref[r_strt_p] > 0 && base_ref[r_strt_p + 1] <= 0)
            {
                bl = r_strt_p;
                bh = r_strt_p + 1;
            }
            else
            {
                bl = r_strt_p - 1;
                bh = r_strt_p;
            }

            double pm_t = base_ref[bl] * (bh - bl) / (base_ref[bl] - base_ref[bh]) + bl;

            double[] ref_a = reference.Skip(ref_lim[0]).Take(ref_lim[1] - ref_lim[0]).ToArray();
            double sm_v = min(abs(ref_a), out bl);
            bl = bl + ref_lim[0];
            bh = bl + 1;
            double sm_t = reference[bl] * (bh - bl) / (reference[bl] - reference[bh]) + bl;

            double lg_s = pm_t - sm_t;

            // Create the original and shifted scales
            double[] scale_t = new double[base_ref.Length];
            double[] scale_s = new double[base_ref.Length];
            for (uint i = 0; i < base_ref.Length; i++)
            {
                scale_t[i] = i;
                scale_s[i] = ((double)i) - lg_s;
            }

            // Interpolate the reference and the signal
            aligned_reference = interp1_lin(scale_t, reference, scale_s);
            aligned_signal = interp1_lin(scale_t, signal, scale_s);

            return lg_s;
        }

        /// <summary>
        /// Allows user to copy the TD graph to clipboard by double-clicking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sg_TD_graph_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            sg_TD_graph.ToClipboard();
            MessageLog("Time Domain Graph copied to clipboard");
        }

        #endregion

        #region Menu Bar Functions

        #region Exit

        private void form_main_FormClosed(object sender, FormClosedEventArgs e)
        {
            formClosed();
        }

        /// <summary>
        /// Ends background threads if the user has closed the form and disconnects devices
        /// </summary>
        private void formClosed()
        {
            // Stop background workers
            if (Running_TD)
            {
                bgw_TD_realTime.CancelAsync();
                bgw_TD_collectAuto.CancelAsync();
            }

            if (bgw_load_FD_BL.IsBusy)
                bgw_load_FD_BL.CancelAsync();

            if (bgw_load_FD_Sig.IsBusy)
                bgw_load_FD_Sig.CancelAsync();

            // Disconnect Picoscope
            if (COMRCW != null)
            {
                COMRCW = null;
                GC.Collect(); // Force GC to release the COM object
            }

            // Disconnect Switching Matrix
            if (SWMatrix != null)
            {
                SWMatrix.Disconnect();
                SWMatrix = null;
            }

            // Update log
            MessageLog("Closing");

        }

        private void tsmi_file_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        #endregion

        #region Load

        private void tsmi_load_TD_base_Click(object sender, EventArgs e)
        {
            // Load baseline - automatic scan
            String fileName;
            String folder;
            double[,] mtr;

            // Get folder path + file name
            if (ofd_loadTD.ShowDialog() == DialogResult.OK)
            {
                folder = ofd_loadTD.FileName.ToString();
                // Remove file name from folder textbox
                folder = folder.Remove(folder.IndexOf(@"\sig_A"));

                fileName = ofd_loadTD.FileName.ToString();
            }
            else
                return;

            // Add correct suffix to file name
            tbx_TD_fileName.Text = tbx_TD_fileName.Text.Replace(".txt", findTDSuffix(fileName));

            for (int i = 0; i < Constants.nSensors; i++)
                for (int j = 0; j < Constants.nSensors; j++)
                {
                    if (i == j)
                    {
                        BLMaskAv[i, j] = false;
                        continue;
                    }
                    // Compose file name
                    fileName = tbx_TD_fileName.Text.Replace("%RX%", "A" + (i + 1).ToString());
                    fileName = folder + "\\" + fileName.Replace("%TX%", "A" + (j + 1).ToString());
                    if (!File.Exists(fileName))
                    {
                        MessageBox.Show("Error: " + fileName + " does not exist. (Check that file name format is correct).");
                        MessageLog("Unable to load TD baseline.");
                        // remove suffix from file name and replace with .txt
                        tbx_TD_fileName.Text = tbx_TD_fileName.Text.Replace(findTDSuffix(tbx_TD_fileName.Text), ".txt");
                        return;
                    }
                    mtr = LoadMatrix(fileName);
                    for (int k = 0; k < Constants.sig_len; k++)
                    {
                        BaselineM[i, j][k] = mtr[k, 0];
                        BaselineM_r[i, j][k] = mtr[k, 1];
                    }
                    BLMaskAv[i, j] = true;
                }

            // remove suffix from file name and replace with .txt
            tbx_TD_fileName.Text = tbx_TD_fileName.Text.Replace(findTDSuffix(tbx_TD_fileName.Text), ".txt");
            MessageLog("TD Baseline loaded to main array.");
            //lb_Visual_TD_BL.BackColor = System.Drawing.Color.LimeGreen;
            //tsmi_load_TD_base.Checked = true; 
            
        }

        /// <summary>
        /// Finds the TD file name suffix (e.g. _hw##.txt / _sw##.txt / _s.txt) 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private String findTDSuffix(String filename)
        {
            int index;
            String suffix = null;
            if (filename.Contains("_hw"))
            {
                index = filename.IndexOf("_hw");
                suffix = filename.Substring(index);
            }
            else
                if (filename.Contains("_sw"))
                {
                    index = filename.IndexOf("_sw");
                    suffix = filename.Substring(index);
                }
                else
                {
                    index = filename.IndexOf("_s");
                    suffix = filename.Substring(index);
                }
            return suffix;
        }

        private void tsmi_load_TD_sig_Click(object sender, EventArgs e)
        {
            // Load signals - automatic scan
            String fileName;
            String folder;
            double[,] mtr;

            // Get folder path + file name
            if (ofd_loadTD.ShowDialog() == DialogResult.OK)
            {
                folder = ofd_loadTD.FileName.ToString();
                // Remove file name from folder textbox
                folder = folder.Remove(folder.IndexOf(@"\sig_A"));

                fileName = ofd_loadTD.FileName.ToString();
            }
            else
                return;

            // Add correct suffix to file name
            tbx_TD_fileName.Text = tbx_TD_fileName.Text.Replace(".txt", findTDSuffix(fileName));

            for (int i = 0; i < Constants.nSensors; i++)
                for (int j = 0; j < Constants.nSensors; j++)
                {
                    if (i == j)
                    {
                        SigMaskAv[i, j] = false;
                        continue;
                    }

                    fileName = tbx_TD_fileName.Text.Replace("%RX%", "A" + (i + 1).ToString());
                    fileName = folder + "\\" + fileName.Replace("%TX%", "A" + (j + 1).ToString());
                    if (!File.Exists(fileName))
                    {
                        MessageBox.Show("Error: " + fileName + " does not exist. (Check that file name format is correct).");
                        MessageLog("Unable to load TD baseline.");
                        // remove suffix from file name and replace with .txt
                        tbx_TD_fileName.Text = tbx_TD_fileName.Text.Replace(findTDSuffix(tbx_TD_fileName.Text), ".txt");
                        return;
                    }

                    mtr = LoadMatrix(fileName);
                    for (int k = 0; k < Constants.sig_len; k++)
                    {
                        SignalM[i, j][k] = mtr[k, 0];
                        SignalM_r[i, j][k] = mtr[k, 1];
                    }
                    SigMaskAv[i, j] = true;
                }


            // remove suffix from file name and replace with .txt
            tbx_TD_fileName.Text = tbx_TD_fileName.Text.Replace(findTDSuffix(tbx_TD_fileName.Text), ".txt");
            MessageLog("TD Signal loaded to main array.");
            //lb_Visual_TD_Signal.BackColor = System.Drawing.Color.LimeGreen;
            //tsmi_load_TD_sig.Checked = true; 
        }

        /// <summary>
        /// Load a single TD baseline to array. If the antennae are unknown the
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi_load_TD_base_single_Click(object sender, EventArgs e)
        {
            String fileName;
            String path;
            double[,] mtr;
            int TX = 0;
            int RX = 0;

            // Get folder path + file name
            if (ofd_loadTD.ShowDialog() == DialogResult.OK)
            {
                path = ofd_loadTD.FileName.ToString();
            }
            else
                return;

            // Get file name without the path
            fileName = path.Substring(path.IndexOf("sig_A"));

            // Remove sig_A
            fileName = fileName.Remove(0, 5);
            // Get TX
            TX = Convert.ToInt32(fileName.Substring(0, fileName.IndexOf("_")));

            // Remove _A
            fileName = fileName.Remove(0, fileName.IndexOf("A") + 1);

            // Get RX #
            RX = Convert.ToInt32(fileName.Substring(0, fileName.IndexOf("_")));

            // Load values to matrix
            mtr = LoadMatrix(path);

            // if unknown value AKA RX / TX == 0
            // Load to temporary array
            if (RX == 0 || TX == 0)
            {
                RX = 1;
                TX = 1;

                MessageLog("Unknown baseline loaded to RX = 1 / TX = 1");

            }

            // load to global array

            RX--;
            TX--;
            for (int k = 0; k < Constants.sig_len; k++)
            {
                BaselineM[RX, TX][k] = mtr[k, 0];
                BaselineM_r[RX, TX][k] = mtr[k, 1];
            }
            BLMaskAv[RX, TX] = true;

            MessageLog("Single TD Baseline (TX/RX) A" + TX + " A" + RX + " loaded to main array");



        }

        private void tsmi_load_TD_sig_single_Click(object sender, EventArgs e)
        {
            String fileName;
            String path;
            double[,] mtr;
            int TX = 0;
            int RX = 0;

            // Get folder path + file name
            if (ofd_loadTD.ShowDialog() == DialogResult.OK)
            {
                path = ofd_loadTD.FileName.ToString();
            }
            else
                return;

            // Get file name without the path
            fileName = path.Substring(path.IndexOf("sig_A"));

            // Remove sig_A
            fileName = fileName.Remove(0, 5);
            // Get TX
            TX = Convert.ToInt32(fileName.Substring(0, fileName.IndexOf("_")));

            // Remove _A
            fileName = fileName.Remove(0, fileName.IndexOf("A") + 1);

            // Get RX #
            RX = Convert.ToInt32(fileName.Substring(0, fileName.IndexOf("_")));

            // Load values to matrix
            mtr = LoadMatrix(path);

            // if unknown value AKA RX / TX == 0
            // Set RX / TX to 1 1 
            if (RX == 0 || TX == 0)
            {
                RX = 1;
                TX = 1;

                MessageLog("Unknown signal loaded to RX = 1 / TX = 1");
            }

            // load to global array
            RX--;
            TX--;
            for (int k = 0; k < Constants.sig_len; k++)
            {
                SignalM[RX, TX][k] = mtr[k, 0];
                SignalM_r[RX, TX][k] = mtr[k, 1];
            }
            SigMaskAv[RX, TX] = true;

            MessageLog("Single TD Signal (TX/RX) A" + TX + " A" + RX + " loaded to main array");


        }

        private void tsmi_load_FD_base_Click(object sender, EventArgs e)
        {
            if (bgw_load_FD_BL.IsBusy)
            {
                MessageBox.Show("Loading already in progress...");
                return;
            }
            else
                // Get folder path
                if (fbd_load.ShowDialog() == DialogResult.OK)
                {
                    MessageLog("Loading FD Baseline...");
                    bgw_load_FD_BL.RunWorkerAsync();
                }
                else
                    return;
        }

        private void tsmi_load_FD_sig_Click(object sender, EventArgs e)
        {

            if (bgw_load_FD_Sig.IsBusy)
            {
                MessageBox.Show("Loading already in progress...");
                return;
            }
            else
                // Get folder path
                if (fbd_load.ShowDialog() == DialogResult.OK)
                {
                    MessageLog("Loading FD Signal...");
                    bgw_load_FD_Sig.RunWorkerAsync();
                }
                else
                    return;

        }

        /// <summary>
        /// Load TD file to program matrix/array
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private double[,] LoadMatrix(String filename, int HeaderLines = 0)
        {
            // Determine the dimensions
            string[] Lines = File.ReadAllLines(filename);
            char[] delimiters = new char[] { '\t', ' ' };
            string[] parts = Lines[HeaderLines].Split(delimiters,
                     StringSplitOptions.RemoveEmptyEntries);
            uint nDim1 = (uint)Lines.Length;
            uint nDim2 = (uint)parts.Length;
            num_of_signals = (int)nDim1 - HeaderLines;
            double[,] data_d = new double[nDim1, nDim2];
            bool result = true;
            double value = 1;
            for (int i = HeaderLines; i < nDim1; i++)
            {
                parts = Lines[i].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < nDim2; j++)
                {
                    result = result & double.TryParse(parts[j], out value);
                    data_d[i - HeaderLines, j] = value;
                }
            }
            return data_d;
        }

        #endregion // Load

        #endregion // Menu Bar Functions

        #region Message Log

        /// <summary>
        /// Display a message to the "event" column of the message log.
        /// <para> The log automatically registers the date/time of the event and writes to a txt file "log.txt" in the application folder </para>
        /// </summary>
        /// <param name="message"></param>
        private void MessageLog(string message)
        {
            string time = System.DateTime.Now.ToString();
            string line;
            string path = Application.StartupPath.ToString();

            if (message.Equals("Starting new session..."))
            {
                line = "########################## NEW SESSION ##########################" + "\r\n" + time + "   ---   " + message;
            }

            else
                if (message.Equals("Closing"))
                {
                    line = time + "   ---   " + message + "\r\n" + "########################## SESSION END ##########################" + "\r\n" + "\r\n" + "\r\n";
                }
                else
                    line = time + "   ---   " + message;

            lv_msgLog.Items.Add(time).SubItems.Add(message); // write line to program log


            if (File.Exists(path + @"\log.txt")) // write line to text file
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path + @"\log.txt", true))
                {

                    file.WriteLine(line);
                }
            }
            else
            {
                System.IO.File.WriteAllText(path + @"\log.txt", line);
            }

            lv_msgLog.Items[lv_msgLog.Items.Count - 1].EnsureVisible(); // auto scroll to bottom of list view after adding event

        }

        #endregion // Messge Log

        #region Service
        // Set # of signals acquired and averaged by hardware
        private void SetAcqMode(uint NAvg)
        {
            if (NAvg >= 2)
            {
                COMRCW.ExecCommand("Acq:Ch1:Mode AvgMult");
                COMRCW.ExecCommand("Acq:Ch2:Mode AvgMult");
                COMRCW.ExecCommand("Acq:Ch1:NAvg " + NAvg.ToString());
                COMRCW.ExecCommand("Acq:Ch2:NAvg " + NAvg.ToString());
            }
            else
            {
                COMRCW.ExecCommand("Acq:Ch1:Mode Sample");
                COMRCW.ExecCommand("Acq:Ch2:Mode Sample");
            }
        }
        #endregion

        #region Tooltip Displays

        #region Time Domain
        private void lb_TD_counter_MouseHover(object sender, EventArgs e)
        {
            lb_TD_counter_TT.SetToolTip(lb_TD_counter, " (replacement for %c%): ");
        }

        private void cb_TD_storeSig_MouseHover(object sender, EventArgs e)
        {
            cb_TD_storeSig_TT.SetToolTip(cb_TD_Store, "Check to store single acquisition signals to the main array and display to graph.");
        }

        private void bn_TD_fullArray_MouseHover(object sender, EventArgs e)
        {
            bn_TD_fullArray_TT.SetToolTip(bn_TD_fullArray, "Collect all signals automatically.");
        }

        private void lb_TD_saving_MouseHover(object sender, EventArgs e)
        {
            lb_TD_saving_TT.SetToolTip(lb_TD_saving, "Check to save averaged signal to text file");
        }

        #endregion // Time Domain

        /// <summary>
        /// The status bar will display a hint % completion and estimate time remaining.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pb_status_MouseHover(object sender, EventArgs e)
        {
            if (Running_TD || Running_FD) // Display progress of TD / FD measurements if running 
            {
                TimeSpan time_now = System.DateTime.Now.TimeOfDay;
                TimeSpan time_elapsed = time_now.Subtract(time_start);

                double percent_progress = pb_status.Value / 100.00;
                double total;
                double remaining;
                double elapsed;


                String time_remaining;
                String progress_value = "Progress: " + pb_status.Value + "%   ";

                if (percent_progress > 0)
                {
                    elapsed = time_elapsed.TotalSeconds;
                    total = elapsed / percent_progress;
                    remaining = total - elapsed;
                    double minutes = remaining / 60;
                    double seconds = remaining % 60;
                    time_remaining = "Time Remaining: " + (int)minutes + " min. " + (int)seconds + " sec.";
                }
                else
                    time_remaining = "Time Remaining: N/A";

                pb_status_TT.SetToolTip(pb_status.Control, progress_value + time_remaining);
            }

            else // Display generic message or loading progress
                 pb_status_TT.SetToolTip(pb_status.Control, "Progress: " + pb_status.Value + "%   " + "Time Remaining: N/A");
        }

        /// <summary>
        /// Estimates how much time is left for incremental TD full array measurements.
        /// </summary>
        private void time_remaining_TD(uint currentCase)
        {
            TimeSpan time_elapsed = System.DateTime.Now.TimeOfDay.Subtract(time_start);

            int num_cases_left = (int)(nud_TD_NumCases.Value + nud_TD_StartCaseNum.Value - currentCase);
            int elapsed = (int)time_elapsed.TotalSeconds;
            int total_time_remaining = elapsed * num_cases_left;

            MessageLog("Estimated time remaining: " + (total_time_remaining / 60) + " min. " + (total_time_remaining % 60) + " sec.");

        }
        #endregion
    }
}