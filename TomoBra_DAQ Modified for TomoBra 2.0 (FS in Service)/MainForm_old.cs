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
using Agilent.CommandExpert.ScpiNet.AgPNA_A_09_50_13;
using VNA;
using PNA;
using Picoscope;
using HelpForm;


namespace TomoBra_DAQ
{
    /// <summary>
    /// Authors:        Evgeny Kirshin, Nicolas Vendeville
    /// Created:        May 13, 2013
    /// Last Updated:   August 30, 2013
    /// Summary:        Main form for the TomoBra app.
    ///                 The software allows the user to connect to measurement devices and collect Time-Domain (TD) and Frequency-Domain (FD) signals using microwaves.
    ///                 These signals can processed, saved to textfile, and visualized using the graph.
    ///                 The program UI is organized into 7 tabs and the code relevant to these sections are grouped in regions below.
    /// </summary>
    public partial class form_main : Form
    {
        #region Class Constants
        CSWMatrix SWMatrix = new CSWMatrix();
        CIVNA IVNA = new CIVNA();   // VNA HP8722ES
        COMRCWrapper COMRCW;

        double Ts = 0.0125;  // Sampling interval in ns. By default = 12.5ps => Fs = 80GHz
        double[] Time_scale;
        bool Running_TD = false;
        bool Running_FD = false;
        bool Picoscope_ON = false;
        bool FD_BL_isLoaded = false;
        bool FD_Sig_isLoaded = false;
        int num_of_signals = 0;
        TimeSpan time_start;

        // Temporary Single Arrays (for loading / visualization)
        // TD
        double[] single_TD_baseline = new double[Constants.sig_len];
        double[] single_TD_signal = new double[Constants.sig_len];
        double[] single_TD_baseline_r = new double[Constants.sig_len];
        double[] single_TD_signal_r = new double[Constants.sig_len];
        // FD
        double[] single_FD_baseline = new double[Constants.sig_len_FD];
        double[] single_FD_signal = new double[Constants.sig_len_FD];


        uint CurrentCaseNum = 1; // Incremental Collect


        double[] Wfm_Ch1;
        double[] Wfm_Ch2;

        double[] Wfm_SigDiff;
        double[] Wfm_SigDiff_f;
        double[] Wfm_SigDiff_Saved;

        double[] Wfm_f;     // Filtered waveform

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

        #region Constants - FD Waveform Arrays

        // Baseline Waveform Arrays
        // Real
        double[,][] Baseline_Frequency = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Baseline_S11_Real = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Baseline_S21_Real = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Baseline_S12_Real = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Baseline_S22_Real = new double[Constants.nSensors, Constants.nSensors][];
        // Imaginary
        double[,][] Baseline_S11_Imag = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Baseline_S21_Imag = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Baseline_S12_Imag = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Baseline_S22_Imag = new double[Constants.nSensors, Constants.nSensors][];


        //Signal Waveform Arrays
        // Real
        double[,][] Signal_Frequency = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Signal_S11_Real = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Signal_S21_Real = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Signal_S12_Real = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Signal_S22_Real = new double[Constants.nSensors, Constants.nSensors][];
        // Imaginary
        double[,][] Signal_S11_Imag = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Signal_S21_Imag = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Signal_S12_Imag = new double[Constants.nSensors, Constants.nSensors][];
        double[,][] Signal_S22_Imag = new double[Constants.nSensors, Constants.nSensors][];


        #endregion // Constants - FD Waveform Arrays

        //uint[] RXAntennas = new uint[Constants.nSensors+1];   
        //uint[] TXAntennas = new uint[Constants.nSensors+1];   // Array of antennas to use as transmitters
        List<uint> RXAntennas = new List<uint>();             // Set of antennas to collect signals from
        List<uint> TXAntennas = new List<uint>();             // Set of antennas to use as transmitters
        //uint iTX, iRX;                                      // Indices in TXAntennas, RXAntennas respectively (next to collect)
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
        //// VNA vars////
        string FNameVNA;

        Queue<VNAPkg> _queueVNA = new Queue<VNAPkg>();
        object _lockVNA = new object();

        bool[,] SigMask;                                    // 2-D mask, each element shows if corresponding TX-RX signal should be collected (true) or not (false) 
        ///////////////

        // Multiple case collection
        
        bool Stopping = false;

        //// VNA N5242A////
        CPNA PNA = new CPNA();



        ///////////////


        public class Constants
        {
            public const uint sig_len = 4096;
            public const uint nSensors = 16;
            public const uint sig_len_FD = 6273;
        }

        #endregion // Constants

        public form_main()
        {
            InitializeComponent();
            MessageLog("Starting new session...");
            initialize_Picoscope_tbx();
            StatusBar_Initialization();
            SM_enable_groupboxes(false);           // disable functions in SM tab
            UI_TD_EnableCommands(); // Disable TD Functions at start up
            bn_PS_connect.Enabled = true;


            Time_scale = new double[Constants.sig_len];
            for (int i = 0; i < Constants.sig_len; i++)
            {
                Time_scale[i] = i * Ts;
                WfmAcc[i] = 0;
            }
            // Load filter coefficients
            fir_coeffs_LP_80 = LoadArray("fir_lp_80G_4G_6G_1_80db.txt");
            fir_coeffs_HP_80 = LoadArray("fir_hp_80G_1G_2G_80db_1db.txt");
            fir_coeffs_LP_200 = LoadArray("fir_lp_200G_4G_6G_1_80db.txt");
            fir_coeffs_HP_200 = LoadArray("fir_hp_200G_1G_2G_80db_1db.txt");

            // Preallocate the multi-static waveform arrays
            //BaselineM = new double[M][];
            //SignalM = new double[M][][];
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
            initialize_FD_Global_Arrays(Constants.sig_len_FD);
        }

        private double[] LoadArray(String filename)
        {
            double[] data_d = new double[File.ReadAllLines(filename).Length];
            bool result = true;
            double value;
            int i = 0;
            string line;
            //List<string> lines = new List<string>();
            // Use using StreamReader for disposing.
            using (StreamReader r = new StreamReader(filename))
            {
                // Use while != null pattern for loop
                while ((line = r.ReadLine()) != null)
                {
                    result = result & double.TryParse(line, out value);
                    data_d[i++] = value;
                }
            }
            return data_d;
        }

        private void initialize_FD_Global_Arrays(uint length)
        {
            for (int i = 0; i < Constants.nSensors; i++)
            {
                for (int j = 0; j < Constants.nSensors; j++)
                {
                    // FD Baseline Waveform Arrays
                    // Real
                    Baseline_Frequency[i, j] = new double[length];
                    Baseline_S11_Real[i, j] = new double[length];
                    Baseline_S21_Real[i, j] = new double[length];
                    Baseline_S12_Real[i, j] = new double[length];
                    Baseline_S22_Real[i, j] = new double[length];
                    // Imaginary
                    Baseline_S11_Imag[i, j] = new double[length];
                    Baseline_S21_Imag[i, j] = new double[length];
                    Baseline_S12_Imag[i, j] = new double[length];
                    Baseline_S22_Imag[i, j] = new double[length];

                    // FD Signal Waveform Arrays
                    // Real
                    Signal_Frequency[i, j] = new double[length];
                    Signal_S11_Real[i, j] = new double[length];
                    Signal_S21_Real[i, j] = new double[length];
                    Signal_S12_Real[i, j] = new double[length];
                    Signal_S22_Real[i, j] = new double[length];
                    // Imaginary
                    Signal_S11_Imag[i, j] = new double[length];
                    Signal_S21_Imag[i, j] = new double[length];
                    Signal_S12_Imag[i, j] = new double[length];
                    Signal_S22_Imag[i, j] = new double[length];
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
            if (int.Parse((string)bn_SM_Power.Tag) == 0)
            {

                SM_setInterface(true);
            }
            else
                SM_setInterface(false);
        }

        /// <summary>
        /// Turns Power Button on/off, updates status bar labels (SM, TX/RX), and enables switching matrix functions.
        /// </summary>
        private void SM_setInterface(bool ON)
        {
            if (ON)
            {
                SWMatrix.PowerOnOff(1);         // Power ON
                bn_SM_Power.Text = "Power OFF";
                bn_SM_Power.Tag = "1";
                StatusBar_SM(true);
                SM_enable_groupboxes(true);
                MessageLog("Switching Matrix ON");
                StatusBar_TX();
                StatusBar_RX();
            }
            else
            {
                SWMatrix.PowerOnOff(0);         // Power OFF
                bn_SM_Power.Text = "Power ON";
                bn_SM_Power.Tag = "0";
                StatusBar_SM(false);
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
            if (ON)
            {
                gb_SM_MUX.Enabled = true;
                gb_SM_RXA.Enabled = true;
                gb_SM_SW1.Enabled = true;
                gb_SM_SW2.Enabled = true;
                gb_SM_SW3.Enabled = true;
                gb_SM_SW4.Enabled = true;
                gb_SM_SW5.Enabled = true;
                gb_SM_SW6.Enabled = true;
                gb_SM_TXA.Enabled = true;
            }
            else
            {
                gb_SM_MUX.Enabled = false;
                gb_SM_RXA.Enabled = false;
                gb_SM_SW1.Enabled = false;
                gb_SM_SW2.Enabled = false;
                gb_SM_SW3.Enabled = false;
                gb_SM_SW4.Enabled = false;
                gb_SM_SW5.Enabled = false;
                gb_SM_SW6.Enabled = false;
                gb_SM_TXA.Enabled = false;
            }

        }

        private void bn_SM_SW_EN_Click(object sender, EventArgs e)
        {
            // Drive pin "D" of switches 3...10 to HIGH
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (int.Parse((string)bn_SM_SW_EN.Tag) == 0)
            {
                SWMatrix.SetSW_EN(1);   // Switch_EN ON
                bn_SM_SW_EN.Text = "SW_EN OFF";
                bn_SM_SW_EN.Tag = "1";
            }
            else
            {
                SWMatrix.SetSW_EN(0);   // Switch_EN OFF
                bn_SM_SW_EN.Text = "SW_EN ON";
                bn_SM_SW_EN.Tag = "0";
            }
        }

        private void bn_SM_MUX_EN_Click(object sender, EventArgs e)
        {
            // Drive SW_EN pins of all MUXes to HIGH enabling their work
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (int.Parse((string)bn_SM_MUX_EN.Tag) == 0)
            {
                SWMatrix.SetMUX_EN(1); // MUX_EN ON
                bn_SM_MUX_EN.Text = "MUX_EN OFF";
                bn_SM_MUX_EN.Tag = "1";
            }
            else
            {
                SWMatrix.SetMUX_EN(0);   // MUX_EN OFF
                bn_SM_MUX_EN.Text = "MUX_EN ON";
                bn_SM_MUX_EN.Tag = "0";
            }
        }

        private void bn_SM_PG_CTL_Click(object sender, EventArgs e)
        {
            // Toggle state of PG_CTL output (Pulse Gate)
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            if (int.Parse((string)bn_SM_PG_CTL.Tag) == 0)
            {
                SWMatrix.SetGatingOutput(1);   // PG_CTL-> 1
                bn_SM_PG_CTL.Text = "Pulse Gate OFF";
                bn_SM_PG_CTL.Tag = "1";
            }
            else
            {
                SWMatrix.SetGatingOutput(0);   // PG_CTL-> 0
                bn_SM_PG_CTL.Text = "Pulse Gate ON";
                bn_SM_PG_CTL.Tag = "0";
            }
        }

        private void bn_SM_Fans_Click(object sender, EventArgs e)
        {
            // Turn ON fans
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (int.Parse((string)bn_SM_Fans.Tag) == 0)
            {
                SWMatrix.SetFansState(1);   // Fans ON
                bn_SM_Fans.Text = "Fans OFF";
                bn_SM_Fans.Tag = "1";
            }
            else
            {
                SWMatrix.SetFansState(0);   // Fans OFF
                bn_SM_Fans.Text = "Fans ON";
                bn_SM_Fans.Tag = "0";
            }
        }
        private void bn_SW1_set_Click(object sender, EventArgs e)
        {
            // Switch switch #1 in a position
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            SWMatrix.SetSW1Pos((byte)(nud_SM_SW1.Value - 1));
            StatusBar_RX();
        }

        private void bn_SM_SW1Loop_Click(object sender, EventArgs e)
        {
            // Switch #1 - loop
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            for (int i = 0; i < 8; i++)
            {
                SWMatrix.SetSW1Pos((byte)i);
                System.Threading.Thread.Sleep((int)nud_SM_SW1_del.Value);
            }
        }

        private void bn_SW2_set_Click(object sender, EventArgs e)
        {
            // Switch switch #2 in a position
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            SWMatrix.SetSW2Pos((byte)(nud_SM_SW2.Value - 1));
            StatusBar_RX();
        }

        private void bn_SM_SW2Loop_Click(object sender, EventArgs e)
        {
            // Switch #2 - loop
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            byte[] ReportOut = new byte[2];

            for (int i = 0; i < 8; i++)
            {
                SWMatrix.SetSW2Pos((byte)i);
                System.Threading.Thread.Sleep((int)nud_SM_SW2_del.Value);
            }

        }

        private void bn_SW3_set_Click(object sender, EventArgs e)
        {
            // Switch switch #3 in a position
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            SWMatrix.SetSW3Pos((byte)(nud_SM_SW3.Value));
            StatusBar_RX();
        }

        private void bn_SW4_set_Click(object sender, EventArgs e)
        {
            // Switch switch #4 in a position
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            SWMatrix.SetSW4Pos((byte)(nud_SM_SW4.Value - 1));
            StatusBar_TX();
        }

        private void bn_SM_SW4Loop_Click(object sender, EventArgs e)
        {
            // Switch #4 - loop
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            for (int i = 0; i < 8; i++)
            {
                SWMatrix.SetSW4Pos((byte)i);
                System.Threading.Thread.Sleep((int)nud_SM_SW4_del.Value);
            }

        }

        private void bn_SW5_set_Click(object sender, EventArgs e)
        {
            // Switch switch #5 in a position
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            SWMatrix.SetSW5Pos((byte)(nud_SM_SW5.Value - 1));
            StatusBar_TX();
        }

        private void bn_SM_SW5Loop_Click(object sender, EventArgs e)
        {
            // Switch #5 - loop
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            for (int i = 0; i < 8; i++)
            {
                SWMatrix.SetSW5Pos((byte)i);
                System.Threading.Thread.Sleep((int)nud_SM_SW5_del.Value);
            }
        }

        private void bn_SW6_set_Click(object sender, EventArgs e)
        {
            // Switch switch #6 in a position
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            SWMatrix.SetSW6Pos((byte)(nud_SM_SW6.Value));
            StatusBar_TX();
        }

        private void bn_MUX_set_Click(object sender, EventArgs e)
        {
            // Switch MUX16_A 
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            SWMatrix.SetMUX16APos((byte)(nud_SM_MUX16_A.Value - 1));
            StatusBar_RX();
            StatusBar_TX();
        }

        private void bn_SM_RXA_set_Click(object sender, EventArgs e)
        {
            // Set RX antenna
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC3", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            SWMatrix.SetRXAntenna((byte)nud_SM_RXA_num.Value);
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
            for (int i = 0; i < 16; i++)
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

            SWMatrix.SetTXAntenna((byte)nud_SM_TXA_num.Value);
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
            SWMatrix.SetRXAntenna((byte)15);
            StatusBar_RX();

            for (int i = 0; i < 16; i++)
            {
                if (i == 15)
                {
                    //if RX == TX then change RX to antenna 1
                    SWMatrix.SetRXAntenna((byte)0);
                    StatusBar_RX();
                }

                SWMatrix.SetTXAntenna((byte)i);
                tssl_TXnum.Text = "" + i; // set TX label
                System.Threading.Thread.Sleep((int)nud_SM_TXA_del.Value);
                StatusBar_TX();
            }

        }

        
        
        /// <summary>
        /// Disables the switching matrix if the program is currently running measurements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbctrl_main_Selected(object sender, TabControlEventArgs e)
        {
            if (Running_TD || Running_FD)
                Enable_SWMatrix(true);
            else
                Enable_SWMatrix(false);
        }

        /// <summary>
        /// Enables / Disables the Switching Matrix Tab
        /// </summary>
        /// <param name="ON"></param>
        private void Enable_SWMatrix(bool ON)
        {
            if (!ON)
            {
                SM_enable_groupboxes(false);
                SM_enable_buttons(false);
            }
            else
            {
                SM_enable_groupboxes(true);
                SM_enable_buttons(true);
            }
        }

        /// <summary>
        /// Enables / Disables the 5 buttons (power, switch_en, mux_en, pulse gates, fans) on SWM tab
        /// </summary>
        /// <param name="ON"></param>
        private void SM_enable_buttons(bool ON)
        {
            if (ON)
            {
                bn_SM_Fans.Enabled = true;
                bn_SM_Power.Enabled = true;
                bn_SM_SW_EN.Enabled = true;
                bn_SM_MUX_EN.Enabled = true;
                bn_SM_PG_CTL.Enabled = true;
            }
            else // OFF
            {
                bn_SM_Fans.Enabled = false;
                bn_SM_Power.Enabled = false;
                bn_SM_SW_EN.Enabled = false;
                bn_SM_MUX_EN.Enabled = false;
                bn_SM_PG_CTL.Enabled = false;
            }

        }
        
        #endregion // Switching Matrix

        #region Service
        private void bn_serv_saveAmpFiles_Click(object sender, EventArgs e)
        {

            nud_serv_counter.Value = 0;
            String FName = tbx_serv_folder.Text + "\\" + tbx_serv_fileName.Text.Replace("%c%", nud_serv_counter.Value.ToString());
            // Check if file exists
            if (File.Exists(FName))
            {
                switch (MessageBox.Show("Overwrite?",
                            "Warning: file exists!",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        // "Yes" processing
                        break;
                    case DialogResult.No:
                        return;
                    // "No" processing
                    case DialogResult.Cancel:
                        // "Cancel" processing
                        return;
                }
            }

            // Construct time-scale
            // Get current time period for channel 1
            COMRCW.ExecCommand("Header Off"); // Switching off headers in results
            // Set acquisition mode
            SetAcqMode((uint)nud_serv_nAvg.Value);

            bn_serv_saveAmpFiles.BackColor = Color.FromName("Red");
            bn_serv_saveAmpFiles.Enabled = false;
            bn_serv_saveAmpFiles.Text = "Saving...";

            // Start data acquisition
            COMRCW.RunSingle();

            // Get the waveform from channel 1
            Wfm_Ch1 = COMRCW.GetWaveForm(1);

            // Get the waveform from channel 2
            Wfm_Ch2 = COMRCW.GetWaveForm(2);
            // Save to files
            StoreArrays(Wfm_Ch1, Wfm_Ch2, FName);

            bn_serv_saveAmpFiles.BackColor = Color.FromName("Control");
            bn_serv_saveAmpFiles.Enabled = true;
            bn_serv_saveAmpFiles.Text = "Save";

            //increment count
            nud_serv_counter.Value++;

            timer_serv_amp.Enabled = true;

        }



        /// <summary>
        /// Stores 2 (1D) arrays into a text file: in two columns
        /// </summary>
        /// <param name="Array1"></param>
        /// <param name="Array2"></param>
        /// <param name="filename"></param>
        private void StoreArrays(double[] Array1, double[] Array2, String filename)
        {
            int nRows = Array1.Length;
            string[] lines = new string[nRows];
            for (uint i = 0; i < nRows; i++)
                lines[i] = Array1[i].ToString() + "\t" + Array2[i].ToString();
            System.IO.File.WriteAllLines(filename, lines);
        }

        /// <summary>
        /// Set # of signals acquired and averaged by hardware
        /// </summary>
        /// <param name="NAvg"></param>
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

        private void timer_serv_amp_Tick(object sender, EventArgs e)
        {

            // At the 3 min mark,set up the file name
            String FName = tbx_serv_folder.Text + "\\" + tbx_serv_fileName.Text.Replace("%c%", nud_serv_counter.Value.ToString());

            // Write signals to file with selected file name and count

            // Construct time-scale
            // Get current time period for channel 1
            COMRCW.ExecCommand("Header Off"); // Switching off headers in results
            // Set acquisition mode
            SetAcqMode((uint)nud_serv_nAvg.Value);

            bn_serv_saveAmpFiles.BackColor = Color.FromName("Red");
            bn_serv_saveAmpFiles.Enabled = false;
            bn_serv_saveAmpFiles.Text = "Saving...";

            // Start data acquisition
            COMRCW.RunSingle();

            // Get the waveform from channel 1
            Wfm_Ch1 = COMRCW.GetWaveForm(1);

            // Get the waveform from channel 2
            Wfm_Ch2 = COMRCW.GetWaveForm(2);
            // Save to files
            StoreArrays(Wfm_Ch1, Wfm_Ch2, FName);

            bn_serv_saveAmpFiles.BackColor = Color.FromName("Control");
            bn_serv_saveAmpFiles.Enabled = true;
            bn_serv_saveAmpFiles.Text = "Save";

            //once count reaches 20, stop timer
            if (nud_serv_counter.Value >= 20)
            {
                timer_serv_amp.Enabled = false;
            }

            //increment count
            nud_serv_counter.Value++;
        }

        private void bn_serv_selectFold_Click(object sender, EventArgs e)
        {

            if (fbd_FD.ShowDialog() == DialogResult.OK)
            {
                tbx_serv_folder.Text = fbd_FD.SelectedPath;
            }

        }
        #endregion

        #region VNA

        #region CVNA
        private void bn_VNA_HP_connect_Click(object sender, EventArgs e)
        { // Button: Connect/Disconnect to/from VNA
            if ((string)bn_VNA_HP_connect.Tag == "1")
                VNA_HP_Disconnect();
            else VNA_HP_Connect();

        }

        /// <summary>
        /// Connect to HP8722ES
        /// </summary>
        private void VNA_HP_Connect()
        {
            bool VNA_ON = IVNA.connect(tbx_VNA_HP_address.Text);

            if (VNA_ON)
            {
                bn_VNA_HP_connect.Tag = "1";
                bn_VNA_HP_connect.Text = "Disconnect";
                tbx_VNA_HP_address.Enabled = false;
                FD_Enable();
                MessageLog("Connected to HP VNA.");
            }

            MessageLog(IVNA.getOutputMessage());    //output connection message to log
            StatusBar_Device();                     //update status bar
        }

        /// <summary>
        /// Disconnect from HP8722ES
        /// </summary>
        private void VNA_HP_Disconnect()
        {
            bn_VNA_HP_connect.Tag = "0";
            bn_VNA_HP_connect.Text = "Connect";
            tbx_VNA_HP_address.Enabled = true;
            IVNA.disconnect();
            StatusBar_Device();                     //update status bar
            FD_Disable();
            MessageLog("HP VNA disconnected.");
        }

        #endregion

        #region CPNA
        private void bn_VNA_Ag_connect_Click(object sender, EventArgs e)
        { // Button: Connect/Disconnect to/from VNA N5242A
            if ((string)bn_VNA_Ag_connect.Tag == "1")
                VNA_Ag_Disconnect();
            else VNA_Ag_Connect();
        }

        /// <summary>
        /// Disconnect from N5242A
        /// </summary>
        private void VNA_Ag_Disconnect()
        {
            bn_VNA_Ag_connect.Tag = "0";
            bn_VNA_Ag_connect.Text = "Connect";

            FD_Enable();
            bn_VNA_Ag_configure.Tag = 0;
            bn_VNA_Ag_configure.Text = "Configure";

            PNA.disconnect();
            bn_VNA_Ag_configure.Tag = 0;                //Not configured any more
            StatusBar_Device();                             //update status bar
            FD_Disable();
            MessageLog("PNA disconnected.");
        }

        /// <summary>
        /// Connect to N5242A
        /// </summary>
        private void VNA_Ag_Connect()
        {

            // try to connect    
            if (PNA.connect(tbx_VNA_Ag_address.Text))
            {
                // output status message
                MessageLog(PNA.getOutputMessage());
                MessageLog("PNA connected. Configure before starting measurements.");

                //update status bar and labels
                StatusBar_Device();
                bn_VNA_Ag_connect.Tag = "1";
                bn_VNA_Ag_connect.Text = "Disconnect";
            }
            else
                // else display error message
                MessageLog(PNA.getOutputMessage());
        }
        #endregion

        /// <summary>
        /// Enable FD commands
        /// </summary>
        private void FD_Enable()
        {
            bn_FD_single.Enabled = true;
            bn_FD_fullArray.Enabled = true;


        }

        /// <summary>
        /// Disable FD commands
        /// </summary>
        private void FD_Disable()
        {
            bn_FD_single.Enabled = false;
            bn_FD_fullArray.Enabled = false;

        }

        private void bn_VNA_Ag_configure_Click(object sender, EventArgs e)
        {
            // If not connected, try to connect then configure
            if (!PNA.getConnectionStatus())
            {

                if (PNA.connect(tbx_VNA_Ag_address.Text))
                {
                    MessageLog(PNA.getOutputMessage()); // display connect message
                    MessageLog("PNA Connected.");
                    PNA.configure(tbx_VNA_Ag_address.Text, (double)nud_VNA_Ag_BWOutside.Value, (double)nud_VNA_Ag_BWMain.Value);
                    MessageLog(PNA.getOutputMessage()); // display configure message

                    FD_Enable();
                    bn_VNA_Ag_configure.Tag = 1;
                    bn_VNA_Ag_configure.Text = "Re-configure";

                    StatusBar_Device();
                    bn_VNA_Ag_connect.Tag = "1";
                    bn_VNA_Ag_connect.Text = "Disconnect";

                    lb_FD_Ag_BWMain.Text = nud_VNA_Ag_BWMain.Value.ToString();
                    lb_FD_Ag_BWOutside.Text = nud_VNA_Ag_BWOutside.Value.ToString();
                }
                else
                    MessageLog(PNA.getOutputMessage()); // else display error message
            }
            // If already connected, reconfigure...
            else
            {
                PNA.configure(tbx_VNA_Ag_address.Text, (double)nud_VNA_Ag_BWOutside.Value, (double)nud_VNA_Ag_BWMain.Value);
                MessageLog(PNA.getOutputMessage()); // display configure message
                FD_Enable();
                bn_VNA_Ag_configure.Tag = 1;
                bn_VNA_Ag_configure.Text = "Re-configure";

                lb_FD_Ag_BWMain.Text = nud_VNA_Ag_BWMain.Value.ToString();
                lb_FD_Ag_BWOutside.Text = nud_VNA_Ag_BWOutside.Value.ToString();
            }


        }
        #endregion

        #region Picoscope
        private void bn_PS_connect_Click(object sender, EventArgs e)
        {  // Button: Connect/Disconnect to/from VNA
            if ((string)bn_PS_connect.Tag == "1")
                PSDisconnect();
            else PSConnect();

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

        private void bn_PS_execCommand_Click(object sender, EventArgs e)
        {
            try
            {
                String result = COMRCW.ExecCommand(tbx_PS_command.Text);
                lbx_PS_result.Items.Insert(0, result);
            }
            catch (Exception ex)
            {
                lbx_PS_result.Items.Insert(0, ex.Message);
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

        #region Statistical Analysis
        //Statistical Analysis Variables
        int maxWaveforms = 100;
        int numWaveforms = 0;
        int noisyWaveforms = 0;
        int numIntersections = 0;

        double calcTs;
        //[Waveform number, segment of waveform, channel]
        double[, ,] crossLoc_ch;
        double[, ,] average_ch;
        double[, ,] stdDev_ch;

        private void bn_PS_getSquareData_Click(object sender, EventArgs e)
        {
            //This function assumes that two clock signals are being sent to channels 1 and 2
            maxWaveforms = (int)nud_PS_nWave.Value;
            bool risingEdge = cb_PS_risingEdge.Checked;
            numWaveforms = 0;
            noisyWaveforms = 0;
            numIntersections = (int)nud_PS_nInter.Value;

            crossLoc_ch = new double[maxWaveforms, numIntersections, 2];
            average_ch = new double[maxWaveforms, numIntersections + 1, 2];
            stdDev_ch = new double[maxWaveforms, numIntersections + 1, 2];
            double[] MSEPhaseDiff = new double[maxWaveforms];
            double[] outputAligned;

            dgv_PS_waveStats.Rows.Clear();
            pb_status.Maximum = maxWaveforms;
            pb_status.Value = 0;
            tbx_PS_noise.Text = "Noisy Waveforms: " + noisyWaveforms;

            PSConnect();
            // Gets single-acquisition waveforms from both channels
            // Construct time-scale
            // Get current time period for channel 1
            COMRCW.ExecCommand("Header Off"); // Switching off headers in results

            //Calculating the time between samples
            string scale, parseString;
            scale = COMRCW.ExecCommand("TB:ScaleA?");
            parseString = "";
            char unit = ' ';

            for (int i = 0; i < scale.Length; i++)
            {
                if (char.IsDigit(scale[i]))
                    parseString += scale[i];
                else
                {
                    unit = scale[i + 1];
                    break;
                }
            }

            //Converting the scale to time between samples in ns
            calcTs = double.Parse(parseString);
            switch (unit)
            {
                case 'p': calcTs /= 400000;
                    break;
                case 'n': calcTs /= 400;
                    break;
                case 'µ': calcTs *= 2.5;
                    break;
                case 'm': calcTs *= 2500;
                    break;
                default: break;
            }

            // Set acquisition mode
            COMRCW.ExecCommand("Acq:Ch1:Mode Sample");
            COMRCW.ExecCommand("Acq:Ch2:Mode Sample");

            for (int i = 0; i < maxWaveforms; i++)
            {
                bool repeat = false;

                do
                {
                    COMRCW.RunSingle();
                    // Get the waveform from channel 1
                    Wfm_Ch1 = COMRCW.GetWaveForm(1);
                    // Get the waveform from channel 2
                    Wfm_Ch2 = COMRCW.GetWaveForm(2);

                    repeat = statStuff(Wfm_Ch1, 0);
                    repeat = statStuff(Wfm_Ch2, 1) || repeat;
                } while (repeat);   //Repeats when there are noisy signals

                //Only looks for the first specified(rising vs. falling edge) zero crossing
                MSEPhaseDiff[i] = align_MSE(Wfm_Ch1, Wfm_Ch2, risingEdge, 100, out outputAligned);

                numWaveforms++;       //keeping the number of waveforms in a global
                pb_status.Value = numWaveforms;
            }

            double[,] avgCross = new double[numIntersections, 2];
            double[,] avgSegment = new double[numIntersections + 1, 2];
            double[,] avgStdDev = new double[numIntersections + 1, 2];
            double[,] stdDevCross = new double[numIntersections, 2];
            double[,] stdDevSegment = new double[numIntersections + 1, 2];
            double avgMSEPhase = 0;
            double stdDevMSEPhase = 0;
            //Calculate stats across multiple waveforms

            //i = channel, j = segment
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < numIntersections; j++)
                {
                    avgCross[j, i] = 0;
                    avgSegment[j, i] = 0;
                    avgStdDev[j, i] = 0;
                    stdDevCross[j, i] = 0;
                    stdDevSegment[j, i] = 0;

                    for (int k = 0; k < maxWaveforms; k++)
                    {
                        avgCross[j, i] += crossLoc_ch[k, j, i];
                        avgSegment[j, i] += average_ch[k, j, i];
                        avgStdDev[j, i] += stdDev_ch[k, j, i];
                    }
                    avgCross[j, i] /= maxWaveforms;
                    avgSegment[j, i] /= maxWaveforms;
                    avgStdDev[j, i] /= maxWaveforms;

                    for (int k = 0; k < maxWaveforms; k++)
                    {
                        stdDevCross[j, i] += (crossLoc_ch[k, j, i] - avgCross[j, i]) * (crossLoc_ch[k, j, i] - avgCross[j, i]);
                        stdDevSegment[j, i] += (average_ch[k, j, i] - avgSegment[j, i]) * (average_ch[k, j, i] - avgSegment[j, i]);
                    }
                    stdDevCross[j, i] /= (maxWaveforms - 1);
                    stdDevSegment[j, i] /= (maxWaveforms - 1);

                    stdDevCross[j, i] = Math.Sqrt(stdDevCross[j, i]);
                    stdDevSegment[j, i] = Math.Sqrt(stdDevSegment[j, i]);
                }

                //The segment after the last zero crossing
                for (int k = 0; k < maxWaveforms; k++)
                {
                    avgSegment[numIntersections, i] += average_ch[k, numIntersections, i];
                    avgStdDev[numIntersections, i] += stdDev_ch[k, numIntersections, i];

                }
                avgSegment[numIntersections, i] /= maxWaveforms;
                avgStdDev[numIntersections, i] /= maxWaveforms;

                for (int k = 0; k < maxWaveforms; k++)
                {
                    stdDevSegment[numIntersections, i] += (average_ch[k, numIntersections, i] - avgSegment[numIntersections, i])
                                                        * (average_ch[k, numIntersections, i] - avgSegment[numIntersections, i]);
                }
                stdDevSegment[numIntersections, i] /= (maxWaveforms - 1);
                stdDevSegment[numIntersections, i] = Math.Sqrt(stdDevSegment[numIntersections, i]);
            }   //End averages of averages etc.

            //Difference in crossings
            double[,] crossDiff = new double[maxWaveforms, numIntersections];
            double[] crossDiffAvg = new double[numIntersections];
            double[] crossDiffStdDev = new double[numIntersections];
            for (int i = 0; i < numIntersections; i++)
            {
                crossDiffAvg[i] = 0;
                crossDiffStdDev[i] = 0;
                for (int j = 0; j < maxWaveforms; j++)
                {
                    crossDiff[j, i] = crossLoc_ch[j, i, 0] - crossLoc_ch[j, i, 1];
                    crossDiffAvg[i] += crossDiff[j, i];
                }
                crossDiffAvg[i] /= maxWaveforms;

                for (int j = 0; j < maxWaveforms; j++)
                {
                    crossDiffStdDev[i] += (crossDiff[j, i] - crossDiffAvg[i]) * (crossDiff[j, i] - crossDiffAvg[i]);
                }
                crossDiffStdDev[i] /= (maxWaveforms - 1);
                crossDiffStdDev[i] = Math.Sqrt(crossDiffStdDev[i]);
            }

            //MSE phase difference
            avgMSEPhase = average(MSEPhaseDiff);
            stdDevMSEPhase = stdDev(MSEPhaseDiff, avgMSEPhase);

            //output to table

            for (int i = 0; i < numIntersections + 1; i++)
            {
                dgv_PS_waveStats.Rows.Add("Channel 1 Segment " + i + " Averages", string.Format("{0:f4} mV", avgSegment[i, 0] * 1000),
                                    string.Format("{0:f4} mV", stdDevSegment[i, 0] * 1000));
                dgv_PS_waveStats.Rows.Add("Channel 1 Segment " + i + " Std. Dev.", string.Format("{0:f4} mV", avgStdDev[i, 0] * 1000),
                                    "--");
                dgv_PS_waveStats.Rows.Add("Channel 2 Segment " + i + " averages", string.Format("{0:f4} mV", avgSegment[i, 1] * 1000),
                                    string.Format("{0:f4} mV", stdDevSegment[i, 1] * 1000));
                dgv_PS_waveStats.Rows.Add("Channel 2 Segment " + i + " Std. Dev.", string.Format("{0:f4} mV", avgStdDev[i, 1] * 1000),
                                    "--");
            }
            for (int i = 0; i < numIntersections; i++)
            {
                dgv_PS_waveStats.Rows.Add("Channel 1 Zero Crossing " + i, string.Format("{0:f4} ns", avgCross[i, 0]),
                                    string.Format("{0:f4} ps", stdDevCross[i, 0] * 1000));
                dgv_PS_waveStats.Rows.Add("Channel 2 Zero Crossing " + i, string.Format("{0:f4} ns", avgCross[i, 1]),
                                    string.Format("{0:f4} ps", stdDevCross[i, 1] * 1000));

                //If negative, Ch1 crosses sooner than Ch2
                dgv_PS_waveStats.Rows.Add("Difference in Zero Crossing " + i, string.Format("{0:f4} ps", crossDiffAvg[i] * 1000),
                                    string.Format("{0:f4} ps", crossDiffStdDev[i] * 1000));
            }
            dgv_PS_waveStats.Rows.Add("MSE phase difference", string.Format("{0:f4} samples", avgMSEPhase),
                                    string.Format("{0:f4} samples", stdDevMSEPhase));
            dgv_PS_waveStats.Rows.Add("MSE phase difference", string.Format("{0:f4} ps", avgMSEPhase * calcTs * 1000),
                                    string.Format("{0:f4} ps", stdDevMSEPhase * calcTs * 1000));
            dgv_PS_waveStats.AutoResizeColumns();

        }

        private bool statStuff(double[] channelData, int channel)
        {
            //Naive method to analyse clock signals.
            const int margin = 20;

            //Find the zero crossings
            int[] zeroCrossLoc = new int[numIntersections + 2];
            double[] stdDeviation = new double[numIntersections + 1];
            double[] average = new double[numIntersections + 1];

            zeroCrossLoc[0] = -margin;

            int numCross = 0;
            for (int i = 0; i < channelData.Length - 1; i++)
            {
                if (channelData[i] * channelData[i + 1] < 0)
                {
                    numCross++;

                    //Too many crossings --> noisy data
                    if (numCross > numIntersections)
                    {
                        noisyWaveforms++;
                        tbx_PS_noise.Text = "Noisy Waveforms: " + noisyWaveforms;

                        var fileOut = new StreamWriter("Errant_data_ch" + channel + "_" + noisyWaveforms + ".txt");
                        for (int j = 0; j < channelData.Length; j++)
                        {
                            fileOut.WriteLine(channelData[j]);
                        }
                        fileOut.Close();
                        return true;
                    }

                    zeroCrossLoc[numCross] = i;
                }
            }

            zeroCrossLoc[numCross + 1] = channelData.Length + margin;

            //Get the Std. Dev. data for each segment between the zero crossings
            for (int i = 0; i <= numCross; i++)
            {
                //calculate the average for a given portion of data
                average[i] = 0;
                for (int j = zeroCrossLoc[i] + margin; j < zeroCrossLoc[i + 1] - margin; j++)
                {
                    average[i] += channelData[j];
                }
                average[i] /= ((zeroCrossLoc[i + 1] - margin) - (zeroCrossLoc[i] + margin));

                //standard deviation
                stdDeviation[i] = 0;
                for (int j = zeroCrossLoc[i] + margin; j < zeroCrossLoc[i + 1] - margin; j++)
                {
                    stdDeviation[i] += (channelData[j] - average[i]) * (channelData[j] - average[i]);
                }
                stdDeviation[i] /= ((zeroCrossLoc[i + 1] - margin) - (zeroCrossLoc[i] + margin)) - 1;
                stdDeviation[i] = Math.Sqrt(stdDeviation[i]);

                average_ch[numWaveforms, i, channel] = average[i];
                stdDev_ch[numWaveforms, i, channel] = stdDeviation[i];
            }

            //Linear interpolation to find the crossover points
            for (int i = 0; i < numCross; i++)
            {
                double timeDelta = calcTs / Math.Abs(channelData[zeroCrossLoc[i + 1]] - channelData[zeroCrossLoc[i + 1] + 1]);
                crossLoc_ch[numWaveforms, i, channel] = zeroCrossLoc[i + 1] * calcTs + timeDelta * Math.Abs(channelData[zeroCrossLoc[i + 1]]);
            }
            return false;
        }

        private double average(double[] numbers)
        {
            double avg = 0;
            for (int i = 0; i < numbers.Length; i++)
                avg += numbers[i];
            avg /= numbers.Length;
            return avg;
        }

        private double stdDev(double[] numbers, double average)
        {
            double stdDev = 0;
            for (int i = 0; i < numbers.Length; i++)
                stdDev += (numbers[i] - average) * (numbers[i] - average);
            stdDev /= (numbers.Length - 1);
            stdDev = Math.Sqrt(stdDev);
            return stdDev;
        }

        private double align_MSE(double[] sig1, double[] sig2, bool risingEdge, double interpCoeff, out double[] alignedSig)
        {
            //Used for clock signals at 5ns/div or 2ns/div
            //higher interpCoeff --> more interpolation

            const int BASE_WIDTH = 50;
            const int SIG_WIDTH = 20;
            int edgeLoc = 0;

            //Find the first expected zero crossing
            for (int i = BASE_WIDTH - 1; i < sig1.Length - BASE_WIDTH - 1; i++)
            {
                if (sig1[i] * sig1[i + 1] < 0)
                {
                    if ((!risingEdge && sig1[i + BASE_WIDTH] < 0) || (risingEdge && sig1[i + BASE_WIDTH] > 0))
                    {
                        edgeLoc = i;
                        break;
                    }
                }
            }

            //Take portions of the signals around the zero crossing specified
            double[] cutSig1;
            double[] cutSig2;

            linInterp(sig1, edgeLoc - BASE_WIDTH, edgeLoc + BASE_WIDTH, interpCoeff, out cutSig1);
            linInterp(sig2, edgeLoc - SIG_WIDTH, edgeLoc + SIG_WIDTH, interpCoeff, out cutSig2);

            double[] mseValues;

            xmse(cutSig1, cutSig2, out mseValues);

            int minMSE = 0;

            for (int i = 1; i < mseValues.Length; i++)
            {
                if (mseValues[i] < mseValues[minMSE])
                    minMSE = i;
            }

            double phaseDiff = minMSE / interpCoeff - BASE_WIDTH + SIG_WIDTH;
            shiftSignal(sig2, phaseDiff, out alignedSig);
            return phaseDiff;
        }

        private void linInterp(double[] signal, int left, int right, double interpCoeff, out double[] outArray)
        {
            //Expands an array with interpCoeff values for each one in the original
            //An array of 2 elements and interpCoeff 10 would result in an array of 11 elements
            outArray = new double[(int)Math.Floor((right - left) * interpCoeff) + 1];

            for (int i = 0; i < outArray.Length; i++)
            {
                outArray[i] = pointLinInterp(signal[(int)Math.Floor(left + i / interpCoeff)], signal[(int)Math.Ceiling(left + i / interpCoeff)],
                                (left + i / interpCoeff) - (Math.Floor(left + i / interpCoeff)));
            }

        }

        private void xmse(double[] signal, double[] pattern, out double[] msev)
        {

            int numLags = signal.Length - pattern.Length + 1;

            msev = new double[numLags];

            for (int i = 0; i < numLags; i++)
            {
                msev[i] = 0;
                for (int j = 0; j < pattern.Length; j++)
                {
                    msev[i] += (signal[i + j] - pattern[j]) * (signal[i + j] - pattern[j]);
                }
            }
        }

        private void shiftSignal(double[] signal, double phaseDiff, out double[] shiftedSig)
        {
            //Shifts an array by the specified number of indices.  Linear interpolation and extrapolation is used where needed
            shiftedSig = new double[signal.Length];

            for (int i = 0; i < shiftedSig.Length; i++)
            {
                if (i - phaseDiff < 0)
                {
                    //extrapolate on the left
                    shiftedSig[i] = signal[0] + (signal[1] - signal[0]) * (i - phaseDiff);
                }
                else if (i - phaseDiff >= shiftedSig.Length - 1)
                {
                    //extrapolate to the right
                    shiftedSig[i] = signal[signal.Length - 1] + (signal[signal.Length - 1] - signal[signal.Length - 2])
                                    * (i - phaseDiff - (signal.Length - 1));
                }
                else
                {
                    //just interpolate
                    shiftedSig[i] = pointLinInterp(signal[(int)Math.Floor(i - phaseDiff)], signal[(int)Math.Ceiling(i - phaseDiff)],
                                        phaseDiff - Math.Floor(phaseDiff));
                }
            }
        }

        private double pointLinInterp(double point1, double point2, double weight)
        {
            //interpolate a value between two points given the weight of the second.
            if (weight < 0 || weight > 1)
                return (0);
            return (point1 + (point2 - point1) * weight);
        }

        private void bn_PS_getPulseData_Click(object sender, EventArgs e)
        {

            //Used to analyze a pulse on channel 1, and a clock on channel 2
            //align_MSE_pulse has parameters near the top chosen for a picoscope scale of 100ps/div or 50ps/div

            maxWaveforms = (int)nud_PS_nWave.Value;
            bool risingEdge = cb_PS_risingEdge.Checked;

            double[] MSEPhaseDiff = new double[maxWaveforms - 1];
            double[] Ch1PhaseDiff = new double[maxWaveforms - 1];
            double[] Ch2PhaseDiff = new double[maxWaveforms - 1];

            double[] MSEPhaseDiffBig = new double[maxWaveforms - 1];
            double[] Ch1PhaseDiffBig = new double[maxWaveforms - 1];
            double[] Ch2PhaseDiffBig = new double[maxWaveforms - 1];

            double[] outputAligned;

            dgv_PS_waveStats.Rows.Clear();
            pb_status.Maximum = maxWaveforms;
            pb_status.Value = 0;

            PSConnect();
            // Gets single-acquisition waveforms from both channels
            // Construct time-scale
            // Get current time period for channel 1
            COMRCW.ExecCommand("Header Off"); // Switching off headers in results

            //Calculating the time between samples
            string scale, parseString;
            scale = COMRCW.ExecCommand("TB:ScaleA?");
            parseString = "";
            char unit = ' ';

            for (int i = 0; i < scale.Length; i++)
            {
                if (char.IsDigit(scale[i]))
                    parseString += scale[i];
                else
                {
                    unit = scale[i + 1];
                    break;
                }
            }

            //Converting the scale to time between samples in ns
            calcTs = double.Parse(parseString);
            switch (unit)
            {
                case 'p': calcTs /= 400000;
                    break;
                case 'n': calcTs /= 400;
                    break;
                case 'µ': calcTs *= 2.5;
                    break;
                case 'm': calcTs *= 2500;
                    break;
                default: break;
            }

            // Set acquisition mode
            COMRCW.ExecCommand("Acq:Ch1:Mode Sample");
            COMRCW.ExecCommand("Acq:Ch2:Mode Sample");

            //Getting the baseline to compare waveforms against
            COMRCW.RunSingle();
            double[] WfmBase_Ch1 = COMRCW.GetWaveForm(1);
            double[] WfmBase_Ch2 = COMRCW.GetWaveForm(2);

            if (cb_PS_saveWFText.Checked)
            {
                writeSignalToFile(WfmBase_Ch1, "Ch1_Baseline.txt");
                writeSignalToFile(WfmBase_Ch2, "Ch2_Baseline.txt");
            }

            var ch1PhaseOut = new StreamWriter("ch1PhaseOut.txt");
            var ch2PhaseOut = new StreamWriter("ch2PhaseOut.txt");

            var ch1PhaseOutBig = new StreamWriter("ch1PhaseOutBig.txt");
            var ch2PhaseOutBig = new StreamWriter("ch2PhaseOutBig.txt");

            for (int i = 1; i < maxWaveforms; i++)
            {
                COMRCW.RunSingle();
                // Get the waveform from channel 1
                Wfm_Ch1 = COMRCW.GetWaveForm(1);
                // Get the waveform from channel 2
                Wfm_Ch2 = COMRCW.GetWaveForm(2);

                if (cb_PS_saveWFText.Checked)
                {
                    writeSignalToFile(Wfm_Ch1, "Ch1_signal" + i + ".txt");
                    writeSignalToFile(Wfm_Ch2, "Ch2_signal" + i + ".txt");
                }

                Ch1PhaseDiff[i - 1] = align_MSE_pulse(WfmBase_Ch1, Wfm_Ch1, true, 10, false, out outputAligned);
                Ch2PhaseDiff[i - 1] = align_MSE_pulse(WfmBase_Ch2, Wfm_Ch2, false, 10, false, out outputAligned);
                MSEPhaseDiff[i - 1] = Ch1PhaseDiff[i - 1] - Ch2PhaseDiff[i - 1];

                Ch1PhaseDiffBig[i - 1] = align_MSE_pulse(WfmBase_Ch1, Wfm_Ch1, true, 10, true, out outputAligned);
                Ch2PhaseDiffBig[i - 1] = align_MSE_pulse(WfmBase_Ch2, Wfm_Ch2, false, 10, true, out outputAligned);
                MSEPhaseDiffBig[i - 1] = Ch1PhaseDiffBig[i - 1] - Ch2PhaseDiffBig[i - 1];

                ch1PhaseOut.WriteLine(Ch1PhaseDiff[i - 1]);
                ch2PhaseOut.WriteLine(Ch2PhaseDiff[i - 1]);

                ch1PhaseOutBig.WriteLine(Ch1PhaseDiffBig[i - 1]);
                ch2PhaseOutBig.WriteLine(Ch2PhaseDiffBig[i - 1]);

                pb_status.Value = i + 1;
            }
            ch1PhaseOut.Close();
            ch2PhaseOut.Close();
            ch1PhaseOutBig.Close();
            ch2PhaseOutBig.Close();

            double Ch1PhaseAvg = 0, Ch1PhaseStdDev = 0, Ch2PhaseAvg = 0, Ch2PhaseStdDev = 0, phaseDiffAvg = 0, phaseDiffStdDev = 0;
            double Ch1PhaseAvgBig = 0, Ch1PhaseStdDevBig = 0, Ch2PhaseAvgBig = 0, Ch2PhaseStdDevBig = 0,
                    phaseDiffAvgBig = 0, phaseDiffStdDevBig = 0;

            //Average and std. dev.
            Ch1PhaseAvg = average(Ch1PhaseDiff);
            Ch2PhaseAvg = average(Ch2PhaseDiff);
            phaseDiffAvg = average(MSEPhaseDiff);
            Ch1PhaseAvgBig = average(Ch1PhaseDiffBig);
            Ch2PhaseAvgBig = average(Ch2PhaseDiffBig);
            phaseDiffAvgBig = average(MSEPhaseDiffBig);

            Ch1PhaseStdDev = stdDev(Ch1PhaseDiff, Ch1PhaseAvg);
            Ch2PhaseStdDev = stdDev(Ch2PhaseDiff, Ch2PhaseAvg);
            phaseDiffStdDev = stdDev(MSEPhaseDiff, phaseDiffAvg);
            Ch1PhaseStdDevBig = stdDev(Ch1PhaseDiffBig, Ch1PhaseAvgBig);
            Ch2PhaseStdDevBig = stdDev(Ch2PhaseDiffBig, Ch2PhaseAvgBig);
            phaseDiffStdDevBig = stdDev(MSEPhaseDiffBig, phaseDiffAvgBig);


            //output results to the table
            dgv_PS_waveStats.Rows.Add("Pulse phase difference", string.Format("{0:f4} samples", Ch1PhaseAvg),
                                    string.Format("{0:f4} samples", Ch1PhaseStdDev));
            dgv_PS_waveStats.Rows.Add("Clock phase difference", string.Format("{0:f4} samples", Ch2PhaseAvg),
                                    string.Format("{0:f4} samples", Ch2PhaseStdDev));
            dgv_PS_waveStats.Rows.Add("Phase difference between Channels", string.Format("{0:f4} samples", phaseDiffAvg),
                                    string.Format("{0:f4} samples", phaseDiffStdDev));

            dgv_PS_waveStats.Rows.Add("Pulse phase difference", string.Format("{0:f4} ps", Ch1PhaseAvg * 1000 * calcTs),
                                    string.Format("{0:f4} ps", Ch1PhaseStdDev * 1000 * calcTs));
            dgv_PS_waveStats.Rows.Add("Clock phase difference", string.Format("{0:f4} ps", Ch2PhaseAvg * 1000 * calcTs),
                                    string.Format("{0:f4} ps", Ch2PhaseStdDev * 1000 * calcTs));
            dgv_PS_waveStats.Rows.Add("Phase difference between Channels", string.Format("{0:f4} ps", phaseDiffAvg * 1000 * calcTs),
                                    string.Format("{0:f4} ps", phaseDiffStdDev * 1000 * calcTs));

            dgv_PS_waveStats.Rows.Add("Pulse phase difference-Wide MSE", string.Format("{0:f4} samples", Ch1PhaseAvgBig),
                        string.Format("{0:f4} samples", Ch1PhaseStdDevBig));
            dgv_PS_waveStats.Rows.Add("Clock phase difference-Wide MSE", string.Format("{0:f4} samples", Ch2PhaseAvgBig),
                                    string.Format("{0:f4} samples", Ch2PhaseStdDevBig));
            dgv_PS_waveStats.Rows.Add("Phase difference between Channels-Wide MSE", string.Format("{0:f4} samples", phaseDiffAvgBig),
                                    string.Format("{0:f4} samples", phaseDiffStdDevBig));

            dgv_PS_waveStats.Rows.Add("Pulse phase difference-Wide MSE", string.Format("{0:f4} ps", Ch1PhaseAvgBig * 1000 * calcTs),
                                    string.Format("{0:f4} ps", Ch1PhaseStdDevBig * 1000 * calcTs));
            dgv_PS_waveStats.Rows.Add("Clock phase difference-Wide MSE", string.Format("{0:f4} ps", Ch2PhaseAvgBig * 1000 * calcTs),
                                    string.Format("{0:f4} ps", Ch2PhaseStdDevBig * 1000 * calcTs));
            dgv_PS_waveStats.Rows.Add("Phase difference between Channels-Wide MSE", string.Format("{0:f4} ps", phaseDiffAvgBig * 1000 * calcTs),
                                    string.Format("{0:f4} ps", phaseDiffStdDevBig * 1000 * calcTs));
            dgv_PS_waveStats.AutoResizeColumns();
        }

        private void writeSignalToFile(double[] signal, string fileName)
        {
            var fileOut = new StreamWriter(fileName);
            for (int i = 0; i < signal.Length; i++)
            {
                fileOut.WriteLine(signal[i]);
            }
            fileOut.Close();
        }

        private double align_MSE_pulse(double[] sig1, double[] sig2, bool maximum, double interpCoeff, bool largeWidth, out double[] alignedSig)
        {
            //Maximum == true --> analyzing pulse.  Otherwise analyzing a clock signal
            //higher interpCoeff --> more interpolation

            //May need to change these, depending on the scale
            int BASE_WIDTH = 200;
            int SIG_WIDTH = 60;
            if (largeWidth)
            {
                BASE_WIDTH = 250;//100ps/div: 250       50ps/div: 500
                SIG_WIDTH = 150;//100ps/div: 150        50ps/div: 300
            }

            int edgeLoc = 0;

            double[] absSig1 = new double[sig1.Length];

            for (int i = 0; i < sig1.Length; i++)
            {
                absSig1[i] = Math.Abs(sig1[i]);
                if ((absSig1[i] > absSig1[edgeLoc] && maximum) || (absSig1[i] < absSig1[edgeLoc] && !maximum))
                    edgeLoc = i;
            }

            //Take portions of the signals around the point specified
            double[] cutSig1;
            double[] cutSig2;

            linInterp(sig1, edgeLoc - BASE_WIDTH, edgeLoc + BASE_WIDTH, interpCoeff, out cutSig1);
            linInterp(sig2, edgeLoc - SIG_WIDTH, edgeLoc + SIG_WIDTH, interpCoeff, out cutSig2);

            double[] mseValues;

            xmse(cutSig1, cutSig2, out mseValues);

            int minMSE = 0;

            //find the lowest MSE
            for (int i = 1; i < mseValues.Length; i++)
            {
                if (mseValues[i] < mseValues[minMSE])
                    minMSE = i;
            }

            double phaseDiff = minMSE / interpCoeff - BASE_WIDTH + SIG_WIDTH;
            shiftSignal(sig2, phaseDiff, out alignedSig);
            return phaseDiff;
        }

        private void bn_PS_saveWaveform_Click(object sender, EventArgs e)
        {
            //This mode collects signals and directly outputs them to textfiles.

            PSConnect();
            // Gets single-acquisition waveforms from both channels
            // Construct time-scale
            // Get current time period for channel 1
            COMRCW.ExecCommand("Header Off"); // Switching off headers in results

            // Set acquisition mode
            COMRCW.ExecCommand("Acq:Ch1:Mode Sample");
            COMRCW.ExecCommand("Acq:Ch2:Mode Sample");

            int maxWaveforms = (int)Math.Floor(nud_PS_nWave.Value);

            for (int i = 0; i < maxWaveforms; i++)
            {
                COMRCW.RunSingle();
                // Get the waveform from channel 1
                Wfm_Ch1 = COMRCW.GetWaveForm(1);
                // Get the waveform from channel 2
                Wfm_Ch2 = COMRCW.GetWaveForm(2);

                var fileOut = new StreamWriter("Waveform" + (i + 1) + "_ch1" + ".txt");
                var fileOut2 = new StreamWriter("Waveform" + (i + 1) + "_ch2" + ".txt");
                for (int j = 0; j < Wfm_Ch1.Length; j++)
                {
                    fileOut.WriteLine(Wfm_Ch1[j]);
                    fileOut2.WriteLine(Wfm_Ch2[j]);
                }
                fileOut.Close();
                fileOut2.Close();
            }

        }
        #endregion

        /// <summary>
        /// Sets the configure text box in the PS tab to appropriate path at program start up
        /// </summary>
        private void initialize_Picoscope_tbx()
        {
            string path = Application.StartupPath.ToString();
            path = path.Replace(@"\bin\Debug", @"\config.txt");
            tbx_PS_conFile.Text = path;
        }

        #endregion

        #region Status Bar
        /// <summary>
        /// Initializes the status bar on program startup.
        /// </summary>
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

        /// <summary>
        /// Checks which device is currently in use. i.e. Picoscope, VNA, PNA, or not connected.
        /// </summary>
        private void StatusBar_Device()
        {
            if (Picoscope_ON)
            {
                tssl_device.Text = "PS9201A";
                tssl_device.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            }
            else
                if (IVNA.getConnectionStatus())
                {
                    tssl_device.Text = "HP8722ES";
                    tssl_device.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
                }
                else
                    if (PNA.getConnectionStatus())
                    {
                        tssl_device.Text = "N5242A";
                        tssl_device.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
                    }
                    else
                    {
                        tssl_device.Text = "Not Connected";
                        tssl_device.BackColor = System.Drawing.SystemColors.Menu;
                    }
        }

        /// <summary>
        /// Updates the TX Label. 
        /// </summary>
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

        /// <summary>
        /// Updates the RX Label.
        /// </summary>
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

        /// <summary>
        /// Changes SM status label on the status strip. 
        /// </summary>
        /// <param name="state"></param>
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

        /// <summary>
        /// Displays to the status bar if the program is taking 1. FD or  2. TD measurements or 3. Idle
        /// </summary>
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

        /// <summary>
        /// Resizes the progress bar to fill up remaining space
        /// </summary>
        private void StatusBar_resize_progressbar()
        {
            int pbSize = ss_main.Width - tssl_device.Width - tssl_RXnum.Width - tssl_SM.Width - tssl_TXnum.Width - tssl_RX.Width - tssl_TX.Width - tssl_statusSM.Width
                - tssl_connectionStatus.Width - tssl_running.Width - 25;
            pb_status.Width = pbSize;
        }

        private void form_main_SizeChanged(object sender, EventArgs e)
        {
            // Description: calls resizePB() after the main form of the program is resized
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

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            ContinuousAcquisition(worker, e);
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

        void ContinuousAcquisition(BackgroundWorker worker, DoWorkEventArgs e)
        {
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
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion // TD Real Time

        #region TD Single

        private void bn_TD_single_Click(object sender, EventArgs e)
        {
            // If storing to global array, ensure that RX / TX are set
            if (cb_TD_Store.Checked && ((SWMatrix.curr_RX == -1) || (SWMatrix.curr_TX == -1)))
            {
                if ((SWMatrix.curr_RX == -1) || (SWMatrix.curr_TX == -1))
                    MessageBox.Show("Please set RX / TX Antennas in order to store to global array");

                else
                    if (SWMatrix.curr_RX == SWMatrix.curr_TX)
                        MessageBox.Show("TX and RX values cannot be the same.");
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
                        // "Yes" processing
                        break;
                    case DialogResult.No:
                    // "No" processing
                    case DialogResult.Cancel:
                        // "Cancel" processing
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
                    StoreArrays(Wfm_Ch1, Wfm_Ch2, FileNameHW);
                }

                // Filter / Process / Plot
                if (cb_TD_Store.Checked && rb_TD_HWAvg.Checked)
                {
                    // Save to global array
                    if (rb_TD_CollectBL.Checked)
                        SaveBaseline(Wfm_Ch1, Wfm_Ch2, (uint)RX, (uint)TX);
                    else
                        SaveSignal(Wfm_Ch1, Wfm_Ch2, (uint)RX, (uint)TX);

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
                    StoreArrays(WfmAcc, Wfm_Single_r, FileNameSW);
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
                    StoreArrays(Wfm_Single, Wfm_Single_r, FileNameSingle);
                }

                // Filter / Process / Plot
                if (cb_TD_Store.Checked && rb_TD_NoAvg.Checked)
                {
                    // Store to global array
                    if (rb_TD_CollectBL.Checked)
                        SaveBaseline(Wfm_Single, Wfm_Single_r, (uint)RX, (uint)TX);
                    else
                        SaveSignal(Wfm_Single, Wfm_Single_r, (uint)RX, (uint)TX);

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

        /// <summary>
        /// Collect all signals automatically.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            else
                Stopping = false;

            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC2", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                MessageLog("No connection with the MCU!");
                return;
            }
            else
                SM_setInterface(true);

            // Check if user has picked a folder destination
            if (tbx_TD_folder.Text == "")
                if (fbd_TD.ShowDialog() == DialogResult.OK)
                {
                    tbx_TD_folder.Text = fbd_TD.SelectedPath;
                }

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

            AcquireSignalsAuto(worker, e);
        }

        void AcquireSignalsAuto(BackgroundWorker worker, DoWorkEventArgs e)
        {
            COMRCW.ExecCommand("Header Off"); // Switching off headers in results

            int nTX = TXAntennas.Count;
            int nRX = RXAntennas.Count;

            // Prepare for scanning:
            // 0. Power ON, delay
            SWMatrix.PowerOnOff(1);
            Thread.Sleep(5000); // 5-second power-ON delay
            // 1. Enable multiplexors by setting MUX_EN to “1”;
            SWMatrix.SetMUX_EN(1);
            // 2. Drive pins “D” of all multiplexors to “1” (SW_EN -> “1”);
            SWMatrix.SetSW_EN(1);

            // Stop data acquisition on PicoScope 
            COMRCW.ExecCommand("*StopSingle Stop");

            // 3. For each antenna in TXAntennas
            for (int iTX = 0; iTX < nTX; iTX++)
            {
                // Choose needed TX antenna
                SWMatrix.SetTXAntenna(TXAntennas[iTX] + 1);

                // For each antenna in RXAntennas
                for (int iRX = 0; iRX < nRX; iRX++)
                {
                    // Skip cases when TXAntenna == RXAntenna
                    if (TXAntennas[iTX] == RXAntennas[iRX]) continue;

                    // Choose needed RX antenna
                    SWMatrix.SetRXAntenna(RXAntennas[iRX] + 1);

                    // The inner loop (TX and RX antennas selected)

                    // Enable pulses by setting PG_CTL to “1”
                    SWMatrix.SetGatingOutput(1);

                    // Delay: allow some time to switch to the new RX antenna and output pulses
                    Thread.Sleep(100);

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
                        // Disable pulses by setting PG_CTL to “0”
                        //SWMatrix.SetGatingOutput(0);

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
                            // Disable MUX_EN and SW_EN:
                            SWMatrix.SetSW_EN(0);
                            SWMatrix.SetMUX_EN(0);
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
                            // Disable pulses by setting PG_CTL to “0”
                            //SWMatrix.SetGatingOutput(0);     

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
                                // Disable MUX_EN and SW_EN:
                                SWMatrix.SetSW_EN(0);
                                SWMatrix.SetMUX_EN(0);
                                e.Cancel = true;
                                return;   // stop data acquisition if requested
                            }
                        }
                    }
                }
            }

            // Disable MUX_EN and SW_EN:
            SWMatrix.SetSW_EN(0);
            SWMatrix.SetMUX_EN(0);

            // Set TX and RX antennas to #1 (doesn't make sense, but good for initialization) - this is to switch cascading switches to position 0 (no current)
            SWMatrix.SetTXAntenna(0);
            SWMatrix.SetRXAntenna(0);

            // Let the main thread know that we've done
            WfmPkg pkg_end = new WfmPkg();
            pkg_end.LastPkg = true;     // Let main thread know that we have finished the work
            lock (_lock)
            {
                // Add new packet to the queue
                _queue.Enqueue(pkg_end);
            }
            // Send an indication to the main thread that the process is over (null-packet)
            worker.ReportProgress(100, null);

            // wait for main thread to finish the background thread 
            while (true)
            {
                Thread.Sleep(10);
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;   // stop data acquisition if requested
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

        /// <summary>
        /// Full Array Averaging Method will save HW avg to txt AND/OR store to global array + plot
        /// <para> If (cb_TD_HWAvg.checked) >> Saves to txt file  </para>
        /// <para> If (rb_TD_HWAvg.checked) >>  1. Stores to global array AND 2. Calls on ProcessSignals() </para>
        /// </summary>
        /// <param name="pkg"></param>
        private void Hardware_Averaging_FullArray(WfmPkg pkg)
        {
            if (bCollectHWAvg)
            {
                // Save to file
                String FName;
                FName = tbx_TD_fileName.Text.Replace("%TX%", "A" + (pkg.iTX + 1).ToString());
                FName = tbx_TD_folder.Text + "\\" + FName.Replace("%RX%", "A" + (pkg.iRX + 1).ToString());
                String FNameHWAvg = FName.Replace(".txt", "_hw" + NAvgHW.ToString() + ".txt");
                StoreArrays(Wfm_Ch1, Wfm_Ch2, FNameHWAvg);
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

        /// <summary>
        /// Full Array Averaging Method will save SW avg to txt AND/OR store to global array + plot
        /// <para> If (cb_TD_SWAvg.checked) >> Saves to txt file  </para>
        /// <para> If (rb_TD_SWAvg.checked) >> 1. Stores to global array AND 2. Calls on ProcessSignals() </para>
        /// </summary>
        /// <param name="pkg"></param>
        private void Software_Averaging_FullArray(WfmPkg pkg)
        {
            // save to text file
            if (bCollectSWAvg)
            {
                String FName;
                FName = tbx_TD_fileName.Text.Replace("%TX%", "A" + (pkg.iTX + 1).ToString());
                FName = tbx_TD_folder.Text + "\\" + FName.Replace("%RX%", "A" + (pkg.iRX + 1).ToString());
                String FNameSWAvg = FName.Replace(".txt", "_sw" + NAvgSW.ToString() + ".txt");
                StoreArrays(WfmAcc, Wfm_Single_r, FNameSWAvg);
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

        /// <summary>
        /// Full Array Averaging Method will save Single Acquisition to txt AND/OR store to global array + plot
        /// <para> If (cb_TD_NoAvg.checked) >> Saves to txt file   </para>
        /// <para> If (rb_TD_NoAvg.checked) >>  1. Stores to global array AND 2. Calls on ProcessSignals() </para>
        /// </summary>
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
                StoreArrays(Wfm_Single, Wfm_Single_r, FNameSingle);
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
                bn_serv_saveAmpFiles.Enabled = false;

                tsmi_load_TD.Enabled = true;

                gb_TD_Averaging.Enabled = false;
                gb_TD_Analysis.Enabled = false;
                gb_TD_Calibration.Enabled = false;
                gb_TD_Filtering.Enabled = false;
                gb_TD_SamplingRate.Enabled = false;
                gb_TD_CollectSignals.Enabled = false;
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
                    bn_PS_execCommand.Enabled = false;
                    bn_serv_saveAmpFiles.Enabled = false;

                    tsmi_load_TD.Enabled = false;

                    gb_TD_CollectSignals.Enabled = true;
                    gb_TD_Averaging.Enabled = false;
                    gb_TD_Analysis.Enabled = false;
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
                    bn_PS_execCommand.Enabled = true;
                    bn_serv_saveAmpFiles.Enabled = true;

                    tsmi_load_TD.Enabled = true;

                    gb_TD_Averaging.Enabled = true;
                    gb_TD_Analysis.Enabled = true;
                    gb_TD_Calibration.Enabled = true;
                    gb_TD_Filtering.Enabled = true;
                    gb_TD_SamplingRate.Enabled = true;
                    gb_TD_CollectSignals.Enabled = true;

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
            // If the maximum index is less then the window width - cannot align: return high value (much higher than the possible signal length, e.g. 1000000)
            /*if (mx_i <= win_width)
            { 
                aligned_signal2 = null;
                return 1000000;
            }*/
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
            /*
                        if(del == 0) 
                        {
                            aligned_signal2 = signal2;
                            return del;
                        }
                        */
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

        /// <summary>
        /// Stop data collection (Reset the counter and the flag)
        /// </summary>
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

        /// <summary>
        /// Stores 2D array into a text file: first dimension corresponds to rows, second dimension - to columns
        /// </summary>
        /// <param name="Array"></param>
        /// <param name="filename"></param>
        /// <param name="signals2save"></param>
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

        /// <summary>
        /// Composes the file name for TD signals
        /// </summary>
        /// <returns></returns>
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

            FName = tbx_TD_fileName.Text.Replace("%s%", Suffix);
            FName = FName.Replace("%c%", nud_TD_counter.Value.ToString());
            FName = FName.Replace("%TX%", "A" + tssl_TXnum.Text);
            FName = tbx_TD_folder.Text + "\\" + FName.Replace("%RX%", "A" + tssl_RXnum.Text);
            return FName;
        }

        /// <summary>
        /// Informs user which signal (HW-avg / SW-avg / Single) will be displayed to graph (based on which rb is selected).
        /// </summary>
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

        /// <summary>
        /// Sets bool values and int values for bCollectHWAvg / SWAvg / NoAvg so that the signal can be saved a to txt file.
        /// </summary>
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

        /// <summary>
        /// Stores waveform wfm in global array BaselineM
        /// </summary>
        /// <param name="wfm"></param>
        /// <param name="rf"></param>
        /// <param name="i_RX"></param>
        /// <param name="i_TX"></param>
        private void SaveBaseline(double[] wfm, double[] rf, uint i_RX, uint i_TX)
        {
            for (uint i = 0; i < Constants.sig_len; i++)
            {
                BaselineM[i_RX, i_TX][i] = wfm[i];
                BaselineM_r[i_RX, i_TX][i] = rf[i];
            }
            BLMaskAv[i_RX, i_TX] = true;
        }

        /// <summary>
        /// Stores waveform wfm in global array SignalM
        /// </summary>
        /// <param name="wfm"></param>
        /// <param name="rf"></param>
        /// <param name="i_RX"></param>
        /// <param name="i_TX"></param>
        private void SaveSignal(double[] wfm, double[] rf, uint i_RX, uint i_TX)
        {
            for (uint i = 0; i < Constants.sig_len; i++)
            {
                SignalM[i_RX, i_TX][i] = wfm[i];
                SignalM_r[i_RX, i_TX][i] = rf[i];
            }
            SigMaskAv[i_RX, i_TX] = true;
        }

        /// <summary>
        /// Filters and calibrates the signals before displaying them to the TD graph.
        /// </summary>
        /// <param name="iRX"></param>
        /// <param name="iTX"></param>
        /// <returns></returns>
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
            /*
            double[] s_scale_i = new double[N_INT_SAMP * 2];
            for (uint i = 0; i < N_INT_SAMP * 2; i++)
                s_scale_i[i] = i;
            double[] s_scale_n = new double[N_INT_SAMP * 2 * interp_mul];
            for (uint i = 0; i < N_INT_SAMP * 2 * interp_mul; i++)
                s_scale_n[i] = ( (double)i ) / (double)interp_mul;
            double[] base_ref_s = base_ref.Skip(r_strt_p - N_INT_SAMP).Take(N_INT_SAMP * 2).ToArray();

            double[] p_i = interp1_lin(s_scale_i, base_ref_s, s_scale_n);

            int pm_i;
            double pm_v = min(abs(p_i), out pm_i);

            double[] ref_s = reference.Skip(r_strt_p - N_INT_SAMP).Take(N_INT_SAMP * 2).ToArray();
            double[] s_t = interp1_lin(s_scale_i, ref_s, s_scale_n);

            int sm_i;
            double sm_v = min(abs(s_t), out sm_i);
            
            double lg_s = ((double)(pm_i - sm_i)) / (double)interp_mul;
            */
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

            if (Running_FD)
            {
                bgw_FD_CollectAutoVNA.CancelAsync();
                bgw_FD_CollectAutoVNAAg.CancelAsync();
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

            // Disconnect PNA
            if (PNA != null)
                PNA.disconnect();

            // Disconnect PNA
            if (IVNA != null)
                IVNA.disconnect();

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
            lb_Visual_TD_BL.BackColor = System.Drawing.Color.LimeGreen;
            
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
            lb_Visual_TD_Signal.BackColor = System.Drawing.Color.LimeGreen;
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

            // Get TX #
            String txnum = string.Empty;
            for (int i = 0; i <= fileName.IndexOf("_"); i++)
            {       if (char.IsDigit(fileName, i))
                    txnum += fileName[i];
            }
            TX = int.Parse(txnum);

            // Get RX #
            String rxnum = string.Empty;
            for (int i = fileName.IndexOf("_"); i < fileName.Length; i++)
            {
                if (char.IsDigit(fileName, i))
                    rxnum += fileName[i];
            }
            RX = int.Parse(rxnum);

            // Load values to matrix
            mtr = LoadMatrix(path);

            // if unknown value AKA RX / TX == 0
            // Load to temporary array
            if (RX == 0 || TX == 0)
            {
                for (int k = 0; k < Constants.sig_len; k++)
                {
                    single_TD_baseline[k] = mtr[k, 0];
                    single_TD_baseline_r[k] = mtr[k, 1];
                }
                MessageLog(fileName + " loaded to temporary array");
            }

            else // load to global array
            {
                RX--;
                TX--;
                for (int k = 0; k < Constants.sig_len; k++)
                {
                    BaselineM[RX, TX][k] = mtr[k, 0];
                    BaselineM_r[RX, TX][k] = mtr[k, 1];
                }
                BLMaskAv[RX, TX] = true;
                
            MessageLog("Single TD Baseline " + fileName + " loaded to main array");
            }


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

        #region FD Load Backgroundworkers

        private void bgw_load_FD_BL_DoWork(object sender, DoWorkEventArgs e)
        {
            // Load baseline - automatic scan
            String fileName;
            String folder = fbd_load.SelectedPath;
            double[,] mtr;

            for (int i = 0; i < Constants.nSensors; i++)
                for (int j = 0; j < Constants.nSensors; j++)
                {
                    fileName = tbx_FD_fileName.Text.Replace("%RX%", "A" + (i + 1).ToString());
                    fileName = folder + "\\" + fileName.Replace("%TX%", "A" + (j + 1).ToString());
                    if (!File.Exists(fileName))
                    {
                        MessageBox.Show("Error: " + fileName + " does not exist. (Check that file name format is correct).");
                        FD_BL_isLoaded = false;
                        return;
                    }
                    mtr = LoadMatrix(fileName, 1);
                    for (int k = 0; k < Constants.sig_len_FD; k++)
                    {

                        Baseline_Frequency[i, j][k] = mtr[k, 0];

                        Baseline_S11_Real[i, j][k] = mtr[k, 1];
                        Baseline_S11_Imag[i, j][k] = mtr[k, 2];

                        Baseline_S21_Real[i, j][k] = mtr[k, 3];
                        Baseline_S21_Imag[i, j][k] = mtr[k, 4];

                        Baseline_S12_Real[i, j][k] = mtr[k, 5];
                        Baseline_S12_Imag[i, j][k] = mtr[k, 6];

                        Baseline_S22_Real[i, j][k] = mtr[k, 7];
                        Baseline_S22_Imag[i, j][k] = mtr[k, 8];

                    }
                }

            FD_BL_isLoaded = true;
        }

        private void bgw_load_FD_BL_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (FD_BL_isLoaded)
            {
                MessageLog("FD Baseline loaded to main array.");
                lb_Visual_FD_BL.BackColor = System.Drawing.Color.LimeGreen;
                FD_BL_isLoaded = false; // reset flag
            }

            else
            {
                MessageLog("FD Baseline was not properly loaded.");
            }
        }

        private void bgw_load_FD_Sig_DoWork(object sender, DoWorkEventArgs e)
        {

            // Load baseline - automatic scan
            String fileName;
            String folder = fbd_load.SelectedPath;
            double[,] mtr;

            for (int i = 0; i < Constants.nSensors; i++)
                for (int j = 0; j < Constants.nSensors; j++)
                {
                    fileName = tbx_FD_fileName.Text.Replace("%RX%", "A" + (i + 1).ToString());
                    fileName = folder + "\\" + fileName.Replace("%TX%", "A" + (j + 1).ToString());
                    if (!File.Exists(fileName))
                    {
                        MessageBox.Show("Error: " + fileName + " does not exist. (Check that file name format is correct).");
                        FD_Sig_isLoaded = false;
                        return;
                    }
                    mtr = LoadMatrix(fileName, 1);
                    for (int k = 0; k < Constants.sig_len_FD; k++)
                    {

                        Signal_Frequency[i, j][k] = mtr[k, 0];

                        Signal_S11_Real[i, j][k] = mtr[k, 1];
                        Signal_S11_Imag[i, j][k] = mtr[k, 2];

                        Signal_S21_Real[i, j][k] = mtr[k, 3];
                        Signal_S21_Imag[i, j][k] = mtr[k, 4];

                        Signal_S12_Real[i, j][k] = mtr[k, 5];
                        Signal_S12_Imag[i, j][k] = mtr[k, 6];

                        Signal_S22_Real[i, j][k] = mtr[k, 7];
                        Signal_S22_Imag[i, j][k] = mtr[k, 8];

                    }
                    //BLMaskAv[i, j] = true;
                }

            FD_Sig_isLoaded = true;

        }

        private void bgw_load_FD_Sig_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (FD_Sig_isLoaded)
            {
                MessageLog("FD Signal loaded to main array.");
                lb_Visual_FD_Signal.BackColor = System.Drawing.Color.LimeGreen;
                FD_Sig_isLoaded = false; // reset flag
            }

            else
            {
                MessageLog("FD Signal was not properly loaded.");
            }
        }

        #endregion // FD Load Backgroundworkers

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

        #region Graph

        /// <summary>
        /// Displays or hides the baseline or S11 paramater of all tabs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi_graph_Baseline_CheckedChanged(object sender, EventArgs e)
        {
            if (tsmi_graph_Baseline.Checked)
            {
                sg_TD_graph.Plots[0].Visible = true;
                sg_FD_graph.Plots[0].Visible = true;
                sg_Visual_TD.Plots[0].Visible = true;
                sg_Visual_FD.Plots[0].Visible = true;

            }

            else
            {
                sg_TD_graph.Plots[0].Visible = false;
                sg_FD_graph.Plots[0].Visible = false;
                sg_Visual_TD.Plots[0].Visible = false;
                sg_Visual_FD.Plots[0].Visible = false;
            }

        }

        private void tsmi_graph_Signal_CheckedChanged(object sender, EventArgs e)
        {

            if (tsmi_graph_Signal.Checked)
            {
                sg_TD_graph.Plots[1].Visible = true;
                sg_FD_graph.Plots[1].Visible = true;
                sg_Visual_TD.Plots[1].Visible = true;
                sg_Visual_FD.Plots[1].Visible = true;
            }

            else
            {
                sg_TD_graph.Plots[1].Visible = false;
                sg_FD_graph.Plots[1].Visible = false;
                sg_Visual_TD.Plots[1].Visible = false;
                sg_Visual_FD.Plots[1].Visible = false;
            }

        }

        private void tsmi_graph_Response_CheckedChanged(object sender, EventArgs e)
        {
            if (tsmi_graph_Response.Checked)
            {
                sg_TD_graph.Plots[2].Visible = true;
                sg_FD_graph.Plots[2].Visible = true;
                sg_Visual_TD.Plots[2].Visible = true;
                sg_Visual_FD.Plots[2].Visible = true;
            }

            else
            {
                sg_TD_graph.Plots[2].Visible = false;
                sg_FD_graph.Plots[2].Visible = false;
                sg_Visual_TD.Plots[2].Visible = false;
                sg_Visual_FD.Plots[2].Visible = false;
            }

        }

        private void tsmi_graph_MemDiff_CheckedChanged(object sender, EventArgs e)
        {
            if (tsmi_graph_Baseline.Checked)
            {
                sg_TD_graph.Plots[3].Visible = true;
                sg_FD_graph.Plots[3].Visible = true;
                sg_Visual_TD.Plots[3].Visible = true;
                sg_Visual_FD.Plots[3].Visible = true;
            }

            else
            {
                sg_TD_graph.Plots[3].Visible = false;
                sg_FD_graph.Plots[3].Visible = false;
                sg_Visual_TD.Plots[3].Visible = false;
                sg_Visual_FD.Plots[3].Visible = false;
            }
        }

        // Display Functions
        private void tsmi_graph_Display_Default_CheckedChanged(object sender, EventArgs e)
        {
            if (tsmi_graph_Display_Default.Checked)
                tsmi_graph_Display_Dual.Checked = false;

            else
                tsmi_graph_Display_Dual.Checked = true;
        }

        private void tsmi_graph_Display_Dual_CheckedChanged(object sender, EventArgs e)
        {
            if (tsmi_graph_Display_Dual.Checked)
                tsmi_graph_Display_Default.Checked = false;
            else
                tsmi_graph_Display_Default.Checked = true;
        }

        /// <summary>
        /// Allows the user to reset X & Y - axes of the currently selected graph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi_graph_Display_Default_Click(object sender, EventArgs e)
        {
            // if TD tab page selected - reset TD graph
            if (tbctrl_main.SelectedTab == tbp_TD)
            {
                sg_TD_Yaxis_sig.Range = new NationalInstruments.UI.Range(-0.25D, 0.25D);
                sg_TD_Yaxis_diff.Range = new NationalInstruments.UI.Range(-0.5D, 0.5D);
                sg_TD_Xaxis.Range = new NationalInstruments.UI.Range(0D, 50D);
            }
            else // if FD tab page selected - reset FD graph
                if (tbctrl_main.SelectedTab == tbp_FD)
                {
                    sg_FD_Yaxis_S11_S22.Range = new NationalInstruments.UI.Range(-50D, 0.5D);
                    sg_FD_Yaxis_S12_S21.Range = new NationalInstruments.UI.Range(-100D, 0D);
                    sg_FD_Xaxis.Range = new NationalInstruments.UI.Range(1D, 6D);
                }
                else // if Visualizer tab page selected ...
                    if (tbctrl_main.SelectedTab == tbp_Visual)
                    {
                        // if FD graph visible - reset FD graph
                        if (sg_Visual_FD.Visible)
                        {
                            sg_Visual_FD_Yaxis_S11_S22.Range = new NationalInstruments.UI.Range(-50D, 0.5D);
                            sg_Visual_FD_Yaxis_S12_S21.Range = new NationalInstruments.UI.Range(-100D, 0D);
                            sg_Visual_FD_Xaxis.Range = new NationalInstruments.UI.Range(1D, 6D);
                        }
                        else // else reset TD graph
                        {
                            sg_Visual_TD_Yaxis_sig.Range = new NationalInstruments.UI.Range(-0.25D, 0.25D);
                            sg_Visual_TD_Yaxis_diff.Range = new NationalInstruments.UI.Range(-0.5D, 0.5D);
                            sg_Visual_TD_Xaxis.Range = new NationalInstruments.UI.Range(0D, 50D);
                        }


                    }


        }

        /// <summary>
        /// Allows the user to set X & Y - axes of the currenlty selected TD graph.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi_graph_Display_Dual_Click(object sender, EventArgs e)
        {
            // if TD tab page selected - reset TD graph
            if (tbctrl_main.SelectedTab == tbp_TD)
            {
                sg_TD_Yaxis_sig.Range = new NationalInstruments.UI.Range(-0.25D, 0.3D);
                sg_TD_Yaxis_diff.Range = new NationalInstruments.UI.Range(-0.5D, 0.1D);
                sg_TD_Xaxis.Range = new NationalInstruments.UI.Range(0D, 50D);
            }

            else // if Visualizer tab page and TD graph selected - reset TD graph
                if ((tbctrl_main.SelectedTab == tbp_Visual) && (sg_Visual_TD.Visible == true))
                {
                    sg_Visual_TD_Yaxis_sig.Range = new NationalInstruments.UI.Range(-0.25D, 0.3D);
                    sg_Visual_TD_Yaxis_diff.Range = new NationalInstruments.UI.Range(-0.5D, 0.1D);
                    sg_Visual_TD_Xaxis.Range = new NationalInstruments.UI.Range(0D, 50D);
                }
        }

        /// <summary>
        /// Shows appropriate display options (e.g. S Paramaters when viewing FD graph)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi_graph_DropDownOpened(object sender, EventArgs e)
        {
            // Viewing a TD graph
            if (tbctrl_main.SelectedTab == tbp_TD || ((tbctrl_main.SelectedTab == tbp_Visual) && sg_Visual_TD.Visible))
            {
                tsmi_graph_Baseline.Text = "Show Baseline";
                tsmi_graph_Signal.Text = "Show Signal";
                tsmi_graph_MemDiff.Text = "Show Memorized Difference";
                tsmi_graph_Response.Text = "Show Tumour Response";
                tsmi_graph_Display_Dual.Visible = true;
            }
            else // Viewing an FD graph
                if (tbctrl_main.SelectedTab == tbp_FD || ((tbctrl_main.SelectedTab == tbp_Visual) && sg_Visual_FD.Visible))
                {
                    tsmi_graph_Baseline.Text = "Show S11";
                    tsmi_graph_Signal.Text = "Show S22";
                    tsmi_graph_MemDiff.Text = "Show S12";
                    tsmi_graph_Response.Text = "Show S21";
                    tsmi_graph_Display_Dual.Visible = false;
                }
                else // No graph displayed, default text
                {
                    tsmi_graph_Baseline.Text = "Show Baseline / S11";
                    tsmi_graph_Signal.Text = "Show Signal / S22";
                    tsmi_graph_MemDiff.Text = "Show Memorized Difference / S12";
                    tsmi_graph_Response.Text = "Show Tumour Response / S21";
                    tsmi_graph_Display_Dual.Visible = true;
                }


        }
        
        #region Graph - Click
        // Keeps the drop down menu open if an option is selected

        private void tsmi_graph_Baseline_Click(object sender, EventArgs e)
        {
            tsmi_graph.ShowDropDown();
        }

        private void tsmi_graph_Signal_Click(object sender, EventArgs e)
        {
            tsmi_graph.ShowDropDown();
        }

        private void tsmi_graph_Response_Click(object sender, EventArgs e)
        {
            tsmi_graph.ShowDropDown();
        }

        private void tsmi_graph_MemDiff_Click(object sender, EventArgs e)
        {

            tsmi_graph.ShowDropDown();
        }

        #endregion // Graph - Click

        #endregion // Graph

        #region Help

        private void tsmi_help_view_Click(object sender, EventArgs e)
        {
            HelpFormWindow Help = new HelpFormWindow();
            Help.Show();
        }
        #endregion // Help

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

        #region Frequency Domain

        #region FD Single

        private void bn_FD_single_Click(object sender, EventArgs e)
        {
            // Ensure that user has specified RX / TX before storing
            if (cb_FD_storeSig.Checked && (SWMatrix.curr_RX == -1 || SWMatrix.curr_TX == -1))
            {
                MessageBox.Show("Please set RX and TX to known values in order to store to global array.");
                return;
            }

            if (IVNA.getConnectionStatus()) //if connected to HP, run HP single acquisition
                FD_HP_single();
            else
                if (PNA.getConnectionStatus()) // else if connected to Ag, run Ag single acquisition
                    FD_Ag_single();
        }

        /// <summary>
        /// Single FD signal acquisition using HP VNA
        /// </summary>
        private void FD_HP_single()
        {
            // Make sure both S11 and S21 are obtainable (VNA properly configured)
            if (!IVNA.VNA.TestS1PObtainable() || IVNA.VNA.TestS2PObtainable())
            {
                MessageBox.Show("Please make sure both S21 and S11 are obtainable!");
                return;
            }

            // Check if the first file of the collected data exists: in this case ask if we want to replace it
            String FName = fileNameFD();
            FName = FName.Replace(".txt", "_S11.txt");

            if (File.Exists(FName))
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

            // Get and save the data
            IVNA.SaveSxPData(FName, "HP8722E, PSC3", "S1P");
            FName = FName.Replace(".txt", "_S21.txt");
            IVNA.SaveSxPData(FName, "HP8722E, PSC3", "S2P");
            MessageLog("VNA: Single Waveform Acquired.");
        }

        /// <summary>
        /// Single FD signal acquisition using Ag PNA
        /// </summary>
        private void FD_Ag_single()
        {

            //sgAuto.Visible = false;
            //sgSParam.Visible = true;

            // Check if the first file of the collected data exists: in this case ask if we want to replace it

            String FName = fileNameFD(); //.Replace(".txt", "_S11.txt");

            if (File.Exists(FName))
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

            // Get and save the data
            int opc;
            PNA.n5242a.SCPI.INITiate.CONTinuous.Command(null, false);
            PNA.n5242a.SCPI.INITiate.IMMediate.Command(null);
            PNA.n5242a.SCPI.OPC.Query(out opc); // wait for pending operations to complete

            double[] S11;
            double[] S21;
            double[] S12;
            double[] S22;
            double[] freq;

            //n5242a.SCPI.INITiate.CONTinuous.Command(null, false);   // Stop continuous acquisition
            //n5242a.SCPI.INITiate.IMMediate.Command(null);           // Initiate single acquisition

            VNA_N5242A_GetData(out freq, out S11, out S21, out S12, out S22);

            // Store to Global Array (if requested)
            if (cb_FD_storeSig.Checked)
            {
                if (rb_FD_ColBl.Checked)
                    // Store Baseline
                    StoreToFrequencyArray(S11, S21, S12, S22, freq, true, SWMatrix.curr_RX - 1, SWMatrix.curr_TX - 1);
                else
                    // Store Signal
                    StoreToFrequencyArray(S11, S21, S12, S22, freq, false, SWMatrix.curr_RX - 1, SWMatrix.curr_TX - 1);
            }

            // Plot
            Plot_FD_Signal(sg_FD_graph, S11, S22, S21, S12, freq);

            // Save to file
            // Format: [freq]\t[S11,real]\t[S11, imag]\t[S21,real]\t[S21, imag]\t[S12,real]\t[S12, imag]\t[S22,real]\t[S22, imag] 
            string[] lines = new string[freq.Length + 1];
            lines[0] = "PNA-X N5242A. Format (REAL64): [freq]\t[S11,real]\t[S11, imag]\t[S21,real]\t[S21, imag]\t[S12,real]\t[S12, imag]\t[S22,real]\t[S22, imag]";
            for (uint i = 0; i < freq.Length; i++)
                lines[i + 1] = freq[i].ToString() + "\t" + S11[2 * i].ToString() + "\t" + S11[2 * i + 1].ToString() +
                                "\t" + S21[2 * i].ToString() + "\t" + S21[2 * i + 1].ToString() +
                                "\t" + S12[2 * i].ToString() + "\t" + S12[2 * i + 1].ToString() +
                                "\t" + S22[2 * i].ToString() + "\t" + S22[2 * i + 1].ToString();
            System.IO.File.WriteAllLines(FName, lines);
            MessageLog("PNA: Single Waveform Acquired.");
        }

        /// <summary>
        /// Composes the text file name for Frequency Domain
        /// </summary>
        /// <returns></returns>
        private string fileNameFD()
        {
            // Compose the file name
            String Suffix;
            String FName;

            // Check if user has picked a folder destination
            if (tbx_FD_folder.Text == "")
                if (fbd_FD.ShowDialog() == DialogResult.OK)
                {
                    tbx_FD_folder.Text = fbd_FD.SelectedPath;
                }

            if (rb_FD_suff1.Checked)
                Suffix = tbx_FD_suff1.Text;
            else if (rb_FD_suff2.Checked)
                Suffix = tbx_FD_suff2.Text;
            else Suffix = tbx_FD_suff3.Text;

            FName = tbx_FD_fileName.Text.Replace("%s%", Suffix);
            FName = FName.Replace("%c%", nud_FD_counter.Value.ToString());
            FName = FName.Replace("%TX%", "A" + tssl_TXnum.Text);
            FName = tbx_FD_folder.Text + "\\" + FName.Replace("%RX%", "A" + tssl_RXnum.Text);
            return FName;
        }


        #endregion

        #region FD Full Array

        private void bn_FD_fullArray_Click(object sender, EventArgs e)
        {
            // Connect to MCU (switching matrix)
            if (!SWMatrix.Connect())
            {
                MessageBox.Show("No connection with the MCU!", "PSC2", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            else
                // Set SM interface to ON
                SM_setInterface(true);

            // Connect to device
            if (IVNA.getConnectionStatus()) //if connected to HP, run HP single acquisition
            {
                FD_HP_fullArray();
            }
            else
                if (PNA.getConnectionStatus()) // else if connected to Ag, run Ag single acquisition
                {
                    FD_Ag_fullArray();
                }
                else
                    MessageLog("Not connected to VNA / PNA.");
        }

        /// <summary>
        /// Set the interface (status bar and full array button) when starting FD full array
        /// </summary>
        private void FD_fullArray_Set()
        {
            bn_FD_fullArray.Text = "Stop";
            bn_FD_fullArray.BackColor = Color.FromName("Red");
            bn_FD_fullArray.Enabled = true;
            bn_FD_single.Enabled = false;
            Running_FD = true;
            StatusBar_BGW();
        }

        /// <summary>
        /// Reset the interface (if FD stopping or if FD acquisition completed)
        /// </summary>
        private void FD_fullArray_Reset()
        {
            // reset progress bar
            pb_status.Value = 0;

            // Set interface to idle state
            bn_FD_fullArray.Text = "Full Array";
            bn_FD_fullArray.BackColor = Color.FromName("Control");
            bn_FD_fullArray.Enabled = true;
            bn_FD_single.Enabled = true;
            Running_FD = false;
            StatusBar_BGW();
        }

        #region Full Array - VNA

        private void FD_HP_fullArray()
        {
            // See if already running
            if (Running_FD)
            {
                // Request to stop process
                bgw_FD_CollectAutoVNA.CancelAsync();
                // Disable the Run/Stop button.
                bn_FD_fullArray.Enabled = false;
                MessageLog("Stopping full array acquisition");
                SM_setInterface(false);
                return;
            }

            MessageLog("Acquiring Full Array Signal...");

            String SigType;
            if (rb_FD_ColBl.Checked)
                SigType = "baseline";
            else
                SigType = "signal";


            // Set interface
            FD_fullArray_Set();

            if (cb_FD_IncrementalSave.Checked)              // Multiple case collection
            {
                MessageLog("Collecting " + nud_FD_NumCases.Value + " " + SigType + "s. Start #: " + nud_FD_StartCaseNum.Value);
                CurrentCaseNum = (uint)nud_FD_StartCaseNum.Value;
                string Folder = (rb_FD_ColBl.Checked) ? "\\Baseline" + CurrentCaseNum.ToString() : "\\Tumor" + CurrentCaseNum.ToString();
                CollectCase_VNA(Folder);
            }
            else
            {
                MessageLog("Collecting 1 " + SigType);
                CollectCase_VNA("");
            }
        }

        private void CollectCase_VNA(string Folder)
        {
            // Make sure both S11 and S21 are obtainable (VNA properly configured)
            /*if (!IVNA.VNA.TestS1PObtainable() || IVNA.VNA.TestS2PObtainable())
            {
                MessageBox.Show("Please make sure both S21 and S11 are obtainable!");
                return;
            }*/
            // Reset progress bar
            pb_status.Value = 0;

            // Get start time (for progress bar estimate)
            time_start = System.DateTime.Now.TimeOfDay;

            // Output message to user (for incremental case)
            if (cb_FD_IncrementalSave.Checked)
            {
                if (rb_FD_ColBl.Checked)
                    MessageLog("Collecting Baseline " + CurrentCaseNum);
                else
                    MessageLog("Collecting Tumour " + CurrentCaseNum);
            }

            // Update textbox
            tbx_FD_folder.Text = tbx_FD_folder.Text + Folder;
            string SaveFolder = tbx_FD_folder.Text;

            RXAntennas.Clear();                                 // Fill RXAntennas array (all antennas)
            for (int i = 0; i < Constants.nSensors; i++)
                RXAntennas.Add((uint)i);


            TXAntennas.Clear();                                 // Fill TXAntennas array (all antennas)
            for (int i = 0; i < Constants.nSensors; i++)
                TXAntennas.Add((uint)i);

            // Create file name - for testing
            String FName = tbx_FD_fileName.Text.Replace("%TX%", "A" + (TXAntennas[0] + 1).ToString());
            FNameVNA = tbx_FD_folder.Text + "\\" + FName.Replace("%RX%", "A" + (RXAntennas[0] + 1).ToString());

            if (File.Exists(FNameVNA))
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
            // Create file name template - for saving
            FName = tbx_FD_fileName.Text;
            FNameVNA = tbx_FD_folder.Text + "\\" + FName;

            // Start the background worker
            bgw_FD_CollectAutoVNA.RunWorkerAsync();

        }

        #region CIVNA Background Worker

        private void bgw_FD_CollectAutoVNA_DoWork(object sender, DoWorkEventArgs e)
        {

            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            AcquireSignalsAutoVNA(worker, e);
        }

        void AcquireSignalsAutoVNA(BackgroundWorker worker, DoWorkEventArgs e)
        {
            int nTX = TXAntennas.Count;
            int nRX = RXAntennas.Count;

            // Prepare for scanning:
            // 0. Power ON, delay
            SWMatrix.PowerOnOff(1);
            Thread.Sleep(5000); // 5-second power-ON delay
            // 1. Enable multiplexors by setting MUX_EN to “1”;
            SWMatrix.SetMUX_EN(1);
            // 2. Drive pins “D” of all multiplexors to “1” (SW_EN -> “1”);
            SWMatrix.SetSW_EN(1);

            // 3. For each antenna in TXAntennas
            for (int iTX = 0; iTX < nTX; iTX++)
            {
                // Choose needed TX antenna
                SWMatrix.SetTXAntenna(TXAntennas[iTX] + 1);

                // For each antenna in RXAntennas
                for (int iRX = 0; iRX < nRX; iRX++)
                {
                    // Choose needed RX antenna
                    SWMatrix.SetRXAntenna(RXAntennas[iRX] + 1);

                    // Delay: allow some time to switch to the new RX antenna and output pulses
                    Thread.Sleep(3000);

                    // The inner loop (TX and RX antennas selected)
                    String FNameVNA_c = FNameVNA.Replace("%TX%", "A" + (TXAntennas[iTX] + 1).ToString());
                    FNameVNA_c = FNameVNA_c.Replace("%RX%", "A" + (TXAntennas[iRX] + 1).ToString());

                    // Store S11 when TXAntenna == RXAntenna
                    if (TXAntennas[iTX] == RXAntennas[iRX])
                        IVNA.SaveSxPData(FNameVNA_c, "HP8722E, PSC3", "S1P");
                    else
                        IVNA.SaveSxPData(FNameVNA_c, "HP8722E, PSC3", "S2P");

                    WfmPkg pkg = new WfmPkg();
                    // Report progress
                    pkg.PkgNum = 1;
                    pkg.iRX = RXAntennas[iRX];
                    pkg.iTX = TXAntennas[iTX];

                    lock (_lock)
                    {
                        // Add new packet to the queue
                        _queue.Enqueue(pkg);
                    }
                    // Send package to the main thread for processing
                    worker.ReportProgress((int)Math.Floor((double)(iTX * nRX + iRX + 1) / (nRX * nTX) * 100), null);

                    if (worker.CancellationPending)
                    {
                        // Disable MUX_EN and SW_EN:
                        SWMatrix.SetSW_EN(0);
                        SWMatrix.SetMUX_EN(0);
                        e.Cancel = true;
                        return;   // stop data acquisition if requested
                    }
                }
            }

            // Disable MUX_EN and SW_EN:
            SWMatrix.SetSW_EN(0);
            SWMatrix.SetMUX_EN(0);

            // Set TX and RX antennas to #1 (doesn't make sense, but good for initialization) - this is to switch cascading switches to position 0 (no current)
            SWMatrix.SetTXAntenna(0);
            SWMatrix.SetRXAntenna(0);

            // Let the main thread know that we've done
            WfmPkg pkg_end = new WfmPkg();
            pkg_end.LastPkg = true;     // Let main thread know that we have finished the work
            lock (_lock)
            {
                // Add new packet to the queue
                _queue.Enqueue(pkg_end);
            }
            // Send an indication to the main thread that the process is over (null-packet)
            worker.ReportProgress(100, null);

            // wait for main thread to finish the background thread 
            while (true)
            {
                Thread.Sleep(10);
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;   // stop data acquisition if requested
                }
            }
        }

        private void bgw_FD_CollectAutoVNA_ProgressChanged(object sender, ProgressChangedEventArgs e)
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
                bgw_FD_CollectAutoVNA.CancelAsync();
                return;
            }

            lb_FD_CurAntA.Text = "A" + (pkg.iTX + 1).ToString() + " -> A" + (pkg.iRX + 1).ToString();
            //Note** june 26 2013
            //int TX = (int)pkg.iTX + 1;
            //int RX = (int)pkg.iRX + 1;
            //tssl_TXnum.Text = "" + TX;
            //tssl_RXnum.Text = "" + RX;
            StatusBar_TX();
            StatusBar_RX();
        }

        private void bgw_FD_CollectAutoVNA_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Check if stopping
            if (!Stopping)
            {
                // Check if collecting multiple signals
                if (cb_FD_IncrementalSave.Checked)
                {
                    // Check if collection is finished
                    if (++CurrentCaseNum <= (nud_FD_NumCases.Value + nud_FD_StartCaseNum.Value - 1))
                    {
                        string Folder = (rb_FD_ColBl.Checked) ? "\\Baseline" + CurrentCaseNum.ToString() :
                            "\\Tumor" + CurrentCaseNum.ToString();

                        tbx_FD_folder.Text = tbx_FD_folder.Text.Replace(@"\Baseline" + (CurrentCaseNum - 1), "");
                        tbx_FD_folder.Text = tbx_FD_folder.Text.Replace(@"\Tumor" + (CurrentCaseNum - 1), "");
                        // Estimate time remaining
                        time_remaining_FD(CurrentCaseNum);
                        // Continue with collection
                        CollectCase_VNA(Folder);
                    }

                    else // Case when finished full array (with incremental)
                    {
                        CurrentCaseNum = 0;
                        MessageLog("Finished Incremental FD Full-Array Acquisition");
                        tbx_FD_folder.Text = fbd_FD.SelectedPath;
                        FD_fullArray_Reset();
                    }

                }

                else // Case when finished full array (no incremental)
                {
                    MessageLog("Finished collecting TD Full Array Signal");
                    tbx_FD_folder.Text = fbd_FD.SelectedPath;
                    FD_fullArray_Reset();
                }
            }
            else // Case when stopping
            {
                MessageLog("TD Full Array Acquistion Stopped.");
                tbx_FD_folder.Text = fbd_FD.SelectedPath;
                FD_fullArray_Reset();
            }
        }

        #endregion

        #endregion

        #region Full Array - PNA

        private void FD_Ag_fullArray()
        {
            if (Running_FD) // See if already running
            {
                // Request to stop process
                bgw_FD_CollectAutoVNAAg.CancelAsync();
                // Disable the Run/Stop button.
                bn_FD_fullArray.Enabled = false;
                Stopping = true;
                MessageLog("Stopping full array acquisition.");
                SM_setInterface(false);
                return;
            }
            else Stopping = false;


            String SigType;
            if (rb_FD_ColBl.Checked)
                SigType = "baseline";
            else
                SigType = "signal";


            // Set interface
            FD_fullArray_Set();

            if (cb_FD_IncrementalSave.Checked)              // Multiple cases collection
            {
                MessageLog("Collecting " + nud_FD_NumCases.Value + " " + SigType + "s. Start #: " + nud_FD_StartCaseNum.Value);
                CurrentCaseNum = (uint)nud_FD_StartCaseNum.Value;
                string Folder = (rb_FD_ColBl.Checked) ? "\\Baseline" + CurrentCaseNum.ToString() : "\\Tumor" + CurrentCaseNum.ToString();
                CollectCase_PNA(Folder);
            }
            else
            {
                MessageLog("Collecting 1 " + SigType);
                CollectCase_PNA("");
            }
        }

        private void CollectCase_PNA(string Folder)
        {

            // Reset progress bar
            pb_status.Value = 0;

            // Get start time (for progress bar estimate)
            time_start = System.DateTime.Now.TimeOfDay;

            // Output message to user (for incremental case)
            if (cb_FD_IncrementalSave.Checked)
            {
                if (rb_FD_ColBl.Checked)
                    MessageLog("Collecting Baseline " + CurrentCaseNum);
                else
                    MessageLog("Collecting Tumour " + CurrentCaseNum);
            }

            // Update textbox
            tbx_FD_folder.Text = tbx_FD_folder.Text + Folder;
            string SaveFolder = tbx_FD_folder.Text;

            // Fill RXAntennas array (all antennas)
            RXAntennas.Clear();
            for (int i = 0; i < Constants.nSensors; i++)
                RXAntennas.Add((uint)i);

            // Fill TXAntennas array (all antennas)
            TXAntennas.Clear();
            for (int i = 0; i < Constants.nSensors; i++)
                TXAntennas.Add((uint)i);

            // Load signal mask
            SigMask = LoadMatrixBool(@"sig_mask.txt");

            // Create file name - for testing
            String FName = tbx_FD_fileName.Text.Replace("%TX%", "A" + (TXAntennas[0] + 1).ToString());
            FNameVNA = SaveFolder + "\\" + FName.Replace("%RX%", "A" + (RXAntennas[0] + 1).ToString());

            // Create sub-folder if needed 
            if (!System.IO.File.Exists(SaveFolder))
                System.IO.Directory.CreateDirectory(SaveFolder);

            if (File.Exists(FNameVNA))
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
            // Create file name template - for saving
            FName = tbx_FD_fileName.Text;
            FNameVNA = SaveFolder + "\\" + FName;

            // Start the background worker
            bgw_FD_CollectAutoVNAAg.RunWorkerAsync();

        }

        #region CPNA Background Worker

        private void bgw_FD_CollectAutoVNAAg_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            AcquireSignalsAutoVNAAg(worker, e);
        }

        void AcquireSignalsAutoVNAAg(BackgroundWorker worker, DoWorkEventArgs e)
        {
            int nTX = TXAntennas.Count;
            int nRX = RXAntennas.Count;

            double[] S11;
            double[] S12;
            double[] S21;
            double[] S22;
            double[] freq;

            // Prepare for scanning:
            // 0. Power ON, delay
            SWMatrix.PowerOnOff(1);
            Thread.Sleep(5000); // 5-second power-ON delay
            // 1. Enable multiplexors by setting MUX_EN to “1”;
            SWMatrix.SetMUX_EN(1);
            // 2. Drive pins “D” of all multiplexors to “1” (SW_EN -> “1”);
            SWMatrix.SetSW_EN(1);

            // 3. For each antenna in TXAntennas
            for (int iTX = 0; iTX < nTX; iTX++)
            {
                // Choose needed TX antenna
                SWMatrix.SetTXAntenna(TXAntennas[iTX] + 1);

                // For each antenna in RXAntennas
                for (int iRX = 0; iRX < nRX; iRX++)
                {
                    if (!SigMask[iTX, iRX]) continue;
                    // Choose needed RX antenna
                    SWMatrix.SetRXAntenna(RXAntennas[iRX] + 1);

                    // Delay: allow some time to switch to the new RX antenna and output pulses
                    Thread.Sleep(50);

                    // The inner loop (TX and RX antennas selected)


                    // Store S11 when TXAntenna == RXAntenna
                    /*if (TXAntennas[iTX] == RXAntennas[iRX])
                        IVNA.SaveSxPData(FNameVNA_c, "HP8722E, PSC3", "S1P");
                    else
                        IVNA.SaveSxPData(FNameVNA_c, "HP8722E, PSC3", "S2P");*/

                    // Get data
                    VNA_N5242A_GetData(out freq, out S11, out S12, out S21, out S22);

                    VNAPkg pkg = new VNAPkg();
                    // Report progress
                    pkg.PkgNum = 1;
                    pkg.iRX = RXAntennas[iRX];
                    pkg.iTX = TXAntennas[iTX];
                    pkg.S11 = S11;
                    pkg.S21 = S21;
                    pkg.S12 = S12;
                    pkg.S22 = S22;
                    pkg.freq = freq;

                    lock (_lockVNA)
                    {
                        // Add new packet to the queue
                        _queueVNA.Enqueue(pkg);
                    }
                    // Send package to the main thread for processing
                    worker.ReportProgress((int)Math.Floor((double)(iTX * nRX + iRX + 1) / (nRX * nTX) * 100), null);

                    if (worker.CancellationPending)
                    {
                        // Disable MUX_EN and SW_EN:
                        SWMatrix.SetSW_EN(0);
                        SWMatrix.SetMUX_EN(0);
                        e.Cancel = true;
                        return;   // stop data acquisition if requested
                    }
                }
            }

            // Disable MUX_EN and SW_EN:
            SWMatrix.SetSW_EN(0);
            SWMatrix.SetMUX_EN(0);

            // Set TX and RX antennas to #1 (doesn't make sense, but good for initialization) - this is to switch cascading switches to position 0 (no current)
            SWMatrix.SetTXAntenna(0);
            SWMatrix.SetRXAntenna(0);

            // Let the main thread know that we've done
            VNAPkg pkg_end = new VNAPkg();
            pkg_end.LastPkg = true;     // Let main thread know that we have finished the work
            lock (_lockVNA)
            {
                // Add new packet to the queue
                _queueVNA.Enqueue(pkg_end);
            }
            // Send an indication to the main thread that the process is over (null-packet)
            worker.ReportProgress(100, null);

            // wait for main thread to finish the background thread 
            while (true)
            {
                Thread.Sleep(10);
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;   // stop data acquisition if requested
                }
            }
        }

        private void bgw_FD_CollectAutoVNAAg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            VNAPkg pkg = null;
            pb_status.Value = e.ProgressPercentage;

            lock (_lockVNA)
            {
                if (_queueVNA.Count > 0)
                    pkg = _queueVNA.Dequeue();
            }

            if (pkg.LastPkg)
            {   // End the thread and leave
                bgw_FD_CollectAutoVNAAg.CancelAsync();
                return;
            }

            // Update userface labels
            lb_FD_CurAntA.Text = "A" + (pkg.iTX + 1).ToString() + " -> A" + (pkg.iRX + 1).ToString();
            StatusBar_TX();
            StatusBar_RX();

            // Plot
            Plot_FD_Signal(sg_FD_graph, pkg.S11, pkg.S22, pkg.S21, pkg.S12, pkg.freq);

            // Store to Global Array (if requested)
            if (cb_FD_storeSig.Checked)
            {
                if (rb_FD_ColBl.Checked)
                    // Store Baseline
                    StoreToFrequencyArray(pkg.S11, pkg.S22, pkg.S12, pkg.S22, pkg.freq, true, (int)pkg.iRX, (int)pkg.iTX);
                else
                    // Store Signal
                    StoreToFrequencyArray(pkg.S11, pkg.S21, pkg.S12, pkg.S22, pkg.freq, false, (int)pkg.iRX, (int)pkg.iTX);
            }

            // Save to file
            String FNameVNA_c = FNameVNA.Replace("%TX%", "A" + (pkg.iTX + 1).ToString());
            FNameVNA_c = FNameVNA_c.Replace("%RX%", "A" + (pkg.iRX + 1).ToString());
            // Format: [freq]\t[S11,real]\t[S11, imag]\t[S21,real]\t[S21, imag]\t[S12,real]\t[S12, imag]\t[S22,real]\t[S22, imag] 
            string[] lines = new string[pkg.freq.Length + 1];
            lines[0] = "PNA-X N5242A. Format (REAL64): [freq]\t[S11,real]\t[S11, imag]\t[S21,real]\t[S21, imag]\t[S12,real]\t[S12, imag]\t[S22,real]\t[S22, imag]";
            for (uint i = 0; i < pkg.freq.Length; i++)
                lines[i + 1] = pkg.freq[i].ToString() + "\t" + pkg.S11[2 * i].ToString() + "\t" + pkg.S11[2 * i + 1].ToString() +
                                "\t" + pkg.S21[2 * i].ToString() + "\t" + pkg.S21[2 * i + 1].ToString() +
                                "\t" + pkg.S12[2 * i].ToString() + "\t" + pkg.S12[2 * i + 1].ToString() +
                                "\t" + pkg.S22[2 * i].ToString() + "\t" + pkg.S22[2 * i + 1].ToString();
            System.IO.File.WriteAllLines(FNameVNA_c, lines);

        }

        private void bgw_FD_CollectAutoVNAAg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Check if stopping
            if (!Stopping)
            {
                // Check if collecting multiple signals
                if (cb_FD_IncrementalSave.Checked)
                {
                    // Check if collection is finished
                    if (++CurrentCaseNum <= (nud_FD_NumCases.Value + nud_FD_StartCaseNum.Value - 1))
                    {
                        string Folder = (rb_FD_ColBl.Checked) ? "\\Baseline" + CurrentCaseNum.ToString() :
                            "\\Tumor" + CurrentCaseNum.ToString();

                        tbx_FD_folder.Text = tbx_FD_folder.Text.Replace(@"\Baseline" + (CurrentCaseNum - 1), "");
                        tbx_FD_folder.Text = tbx_FD_folder.Text.Replace(@"\Tumor" + (CurrentCaseNum - 1), "");
                        // Estimate time remaining
                        time_remaining_FD(CurrentCaseNum);
                        // Continue with collection
                        CollectCase_PNA(Folder);
                    }

                    else // Case when finished full array (with incremental)
                    {
                        CurrentCaseNum = 0;
                        MessageLog("Finished Incremental FD Full-Array Acquisition");
                        tbx_FD_folder.Text = fbd_FD.SelectedPath;
                        FD_fullArray_Reset();
                    }

                }

                else // Case when finished full array (no incremental)
                {
                    MessageLog("Finished collecting TD Full Array Signal");
                    FD_fullArray_Reset();
                }
            }
            else // Case when stopping
            {
                MessageLog("TD Full Array Acquistion Stopped.");
                if (fbd_FD.SelectedPath != "")
                tbx_FD_folder.Text = fbd_FD.SelectedPath;
                FD_fullArray_Reset();
            }
        }

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Will Split S-parameter arrays into real / imaginary and store to global arrays. 
        /// <para>Stores to baseline if BaseLine == true else stores signal.</para>
        /// </summary>
        /// <param name="S11"></param>
        /// <param name="S21"></param>
        /// <param name="S12"></param>
        /// <param name="S22"></param>
        /// <param name="freq"></param>
        /// <param name="BaseLine"></param>
        /// <param name="iRX"></param>
        /// <param name="iTX"></param>
        private void StoreToFrequencyArray(double[] S11, double[] S21, double[] S12, double[] S22, double[] freq, bool BaseLine, int iRX, int iTX)
        {
            if (BaseLine)
            {
                for (int i = 0; i < freq.Length; i++)
                {
                    Baseline_S11_Real[iRX, iTX][i] = S11[2 * i];
                    Baseline_S11_Imag[iRX, iTX][i] = S11[2 * i + 1];

                    Baseline_S12_Real[iRX, iTX][i] = S12[2 * i];
                    Baseline_S12_Imag[iRX, iTX][i] = S12[2 * i + 1];

                    Baseline_S21_Real[iRX, iTX][i] = S21[2 * i];
                    Baseline_S21_Imag[iRX, iTX][i] = S21[2 * i + 1];

                    Baseline_S22_Real[iRX, iTX][i] = S22[2 * i];
                    Baseline_S22_Imag[iRX, iTX][i] = S22[2 * i + 1];

                    Baseline_Frequency[iRX, iTX][i] = freq[i];

                }
            }
            else // Signal
            {
                for (int i = 0; i < freq.Length; i++)
                {
                    Signal_S11_Real[iRX, iTX][i] = S11[2 * i];
                    Signal_S11_Imag[iRX, iTX][i] = S11[2 * i + 1];

                    Signal_S12_Real[iRX, iTX][i] = S12[2 * i];
                    Signal_S12_Imag[iRX, iTX][i] = S12[2 * i + 1];

                    Signal_S21_Real[iRX, iTX][i] = S21[2 * i];
                    Signal_S21_Imag[iRX, iTX][i] = S21[2 * i + 1];

                    Signal_S22_Real[iRX, iTX][i] = S22[2 * i];
                    Signal_S22_Imag[iRX, iTX][i] = S22[2 * i + 1];

                    Signal_Frequency[iRX, iTX][i] = freq[i];

                }
            }


        }

        private void VNA_N5242A_GetData(out double[] freq, out double[] S11, out double[] S21, out double[] S12, out double[] S22)
        {
            int opc;
            PNA.n5242a.SCPI.INITiate.CONTinuous.Command(null, false);
            PNA.n5242a.SCPI.INITiate.IMMediate.Command(null);
            PNA.n5242a.SCPI.OPC.Query(out opc); // wait for pending operations to complete

            //n5242a.SCPI.INITiate.CONTinuous.Command(null, false);   // Stop continuous acquisition
            //n5242a.SCPI.INITiate.IMMediate.Command(null);           // Initiate single acquisition

            // S11
            PNA.n5242a.SCPI.CALCulate.PARameter.SELect.Command(null, "S11_1G_6G_10K");
            PNA.n5242a.SCPI.CALCulate.DATA.QueryBlockReal64(null, "SDATA", out S11);
            PNA.n5242a.SCPI.SENSe.X.VALues.QueryBlockReal64(null, out freq);

            // S22
            PNA.n5242a.SCPI.CALCulate.PARameter.SELect.Command(null, "S22_1G_6G_10K");
            PNA.n5242a.SCPI.CALCulate.DATA.QueryBlockReal64(null, "SDATA", out S22);
            //n5242a.SCPI.SENSe.X.VALues.QueryBlockReal64(null, out freq);

            // S21
            PNA.n5242a.SCPI.CALCulate.PARameter.SELect.Command(null, "S21_1G_6G_10K");
            PNA.n5242a.SCPI.CALCulate.DATA.QueryBlockReal64(null, "SDATA", out S21);

            // S12
            PNA.n5242a.SCPI.CALCulate.PARameter.SELect.Command(null, "S12_1G_6G_10K");
            PNA.n5242a.SCPI.CALCulate.DATA.QueryBlockReal64(null, "SDATA", out S12);
        }

        private void bn_FD_browse_Click(object sender, EventArgs e)
        {
            if (fbd_FD.ShowDialog() == DialogResult.OK)
            {
                tbx_FD_folder.Text = fbd_FD.SelectedPath;
            }

        }

        /// <summary>
        /// Loads 2D matrix of bool from text file (separated with tabulation or space)
        /// First Dimension
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool[,] LoadMatrixBool(String filename)
        {
            // Determine the dimensions
            string[] Lines = File.ReadAllLines(filename);
            char[] delimiters = new char[] { '\t', ' ' };
            string[] parts = Lines[0].Split(delimiters,
                     StringSplitOptions.RemoveEmptyEntries);
            uint nDim1 = (uint)Lines.Length;
            uint nDim2 = (uint)parts.Length;
            bool[,] data_d = new bool[nDim1, nDim2];
            bool result = true;
            int value;
            for (int i = 0; i < nDim1; i++)
            {
                parts = Lines[i].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < nDim2; j++)
                {
                    result = result & int.TryParse(parts[j], out value);
                    data_d[i, j] = (value == 1) ? true : false;
                }
            }
            return data_d;
        }

        private void bn_FD_MemSig_Click(object sender, EventArgs e)
        {

            if (tsmi_graph_MemDiff.Checked)
                sg_TD_graph.PlotXY(Time_scale, Wfm_SigDiff_Saved);
        }

        /// <summary>
        /// Allows user to copy the FD graph to clipboard by double-clicking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sg_FD_graph_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            sg_FD_graph.ToClipboard();
            MessageLog("Frequency Domain Graph copied to clipboard");
        }

        /// <summary>
        /// Takes 5 arrays (S11/S22/S21/S12/Frequency) and clears and displays to the FD graph specified by the user
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="S11"></param>
        /// <param name="S22"></param>
        /// <param name="S21"></param>
        /// <param name="S12"></param>
        /// <param name="frequency"></param>
        private void Plot_FD_Signal(ScatterGraph graph, double[] S11, double[] S22, double[] S21, double[] S12, double[] frequency)
        {
            double[] S11MagLog = new double[frequency.Length];
            double[] S22MagLog = new double[frequency.Length];
            double[] S21MagLog = new double[frequency.Length];
            double[] S12MagLog = new double[frequency.Length];

            double[] freq_adj = new double[frequency.Length];

            // Output values to graph
            sg_FD_graph.ClearData();
            //xAxisFreq.Range.Minimum = (double)freq[0] / 1.0e9;

            for (int i = 0; i < frequency.Length; i++)
            {
                // S11
                S11MagLog[i] = 20 * Math.Log10(Math.Sqrt(S11[2 * i] * S11[2 * i] + S11[2 * i + 1] * S11[2 * i + 1]));
                // S22
                S22MagLog[i] = 20 * Math.Log10(Math.Sqrt(S22[2 * i] * S22[2 * i] + S22[2 * i + 1] * S22[2 * i + 1]));
                // S21
                S21MagLog[i] = 20 * Math.Log10(Math.Sqrt(S21[2 * i] * S21[2 * i] + S21[2 * i + 1] * S21[2 * i + 1]));
                // S12
                S12MagLog[i] = 20 * Math.Log10(Math.Sqrt(S12[2 * i] * S12[2 * i] + S12[2 * i + 1] * S12[2 * i + 1]));
                freq_adj[i] = frequency[i] / 1.0e9;
            }

            // S11
            graph.Plots[0].PlotXY(freq_adj, S11MagLog);
            // S22
            graph.Plots[1].PlotXY(freq_adj, S22MagLog);
            // S21
            graph.Plots[2].PlotXY(freq_adj, S21MagLog);
            // S12
            graph.Plots[3].PlotXY(freq_adj, S12MagLog);

        }

        #endregion // Frequency Domain

        #region Tooltip Displays

        #region Time Domain
        private void lb_TD_counter_MouseHover(object sender, EventArgs e)
        {
            ToolTip counter = new ToolTip();
            counter.SetToolTip(lb_TD_counter, " (replacement for %c%): ");

        }

        private void cb_TD_storeSig_MouseHover(object sender, EventArgs e)
        {
            ToolTip tip = new ToolTip();
            tip.SetToolTip(cb_TD_Store, "Check to store single acquisition signals to the main array and display to graph.");
        }

        private void bn_TD_fullArray_MouseHover(object sender, EventArgs e)
        {
            ToolTip tip = new ToolTip();
            tip.SetToolTip(bn_TD_fullArray, "Collect all signals automatically.");
        }

        #endregion // Time Domain

        #region Frequency Domain

        private void lb_FD_count_MouseHover(object sender, EventArgs e)
        {
            ToolTip counter = new ToolTip();
            counter.SetToolTip(lb_FD_count, "(replacement for %c%): ");
        }

        private void bn_FD_fullArray_MouseHover(object sender, EventArgs e)
        {
            ToolTip tip = new ToolTip();
            tip.SetToolTip(bn_FD_fullArray, "Collect all signals automatically.");
        }

        #endregion // Frequency Domain

        /// <summary>
        /// The status bar will display a hint % completion and estimate time remaining.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pb_status_MouseHover(object sender, EventArgs e)
        {

            TimeSpan time_now = System.DateTime.Now.TimeOfDay;
            TimeSpan time_elapsed = time_now.Subtract(time_start);

            double percent_progress = pb_status.Value / 100.00;
            double total;
            double remaining;
            double elapsed;

            ToolTip progress = new ToolTip();

            String time_remaining;
            String progress_value = "Progress: 0" + pb_status.Value + "%   ";

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

            progress.SetToolTip(pb_status.Control, progress_value + time_remaining);
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

        /// <summary>
        /// Estimates how much time is left for incremental FD full array measurements.
        /// </summary>
        private void time_remaining_FD(uint currentCase)
        {
            TimeSpan time_elapsed = System.DateTime.Now.TimeOfDay.Subtract(time_start);

            int num_cases_left = (int)(nud_FD_NumCases.Value + nud_FD_StartCaseNum.Value) - (int)(currentCase);
            int elapsed = (int)time_elapsed.TotalSeconds;
            int total_time_remaining = elapsed * num_cases_left;

            MessageLog("Estimated time remaining: " + (total_time_remaining / 60) + " min. " + (total_time_remaining % 60) + " sec.");

        }

        #endregion // Tooltip Displays

        #region Visualizer

        #region Time Domain

        #region TD Radiobutton controls

        private void rb_Visual_Flt_LoPass_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_Flt_LoPass.Checked)
                Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
        }

        private void rb_Visual_Flt_Band_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_Flt_Band.Checked)
                Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
        }

        private void rb_Visual_Flt_None_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_Flt_None.Checked)
                Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
        }

        private void rb_Visual_80GHz_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_80GHz.Checked)
                Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
        }

        private void rb_Visual_200Ghz_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_200Ghz.Checked)
                Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
        }

        private void rb_Visual_xCal_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_xCal.Checked)
                Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
        }

        private void rb_Visual_CalRef_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_CalRef.Checked)
                Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
        }

        private void rb_Visual_noCal_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_noCal.Checked)
                Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
        }

        #endregion

        /// <summary>
        /// Filters and calibrates the signals before displaying them to the TD Visualizer graph.
        /// </summary>
        /// <param name="iRX"></param>
        /// <param name="iTX"></param>
        /// <returns></returns>
        bool Visualize_TD(uint iRX, uint iTX)
        {
            double[] Signal_f = null;
            double[] Baseline_f = null;
            sg_Visual_TD.ClearData();
            if (SigMaskAv[iRX, iTX] && (rb_Visual_Signal.Checked || rb_Visual_TD_Both.Checked))
            {
                // If signal exists
                if (rb_Visual_Flt_LoPass.Checked)
                {
                    if (rb_Visual_80GHz.Checked)
                        Signal_f = FIR_zf(fir_coeffs_LP_80, SignalM[iRX, iTX]);
                    else Signal_f = FIR_zf(fir_coeffs_LP_200, SignalM[iRX, iTX]);
                }
                else if (rb_Visual_Flt_Band.Checked)
                {
                    if (rb_Visual_80GHz.Checked)
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
                sg_Visual_TD.Plots[0].PlotXY(Time_scale, Signal_f);
            }

            if (BLMaskAv[iRX, iTX] && (rb_Visual_Baseline.Checked || rb_Visual_TD_Both.Checked))
            {   // If baseline exists
                if (rb_Visual_Flt_LoPass.Checked)
                {
                    if (rb_Visual_80GHz.Checked)
                        Baseline_f = FIR_zf(fir_coeffs_LP_80, BaselineM[iRX, iTX]);
                    else Baseline_f = FIR_zf(fir_coeffs_LP_200, BaselineM[iRX, iTX]);
                }
                else if (rb_Visual_Flt_Band.Checked)
                {
                    if (rb_Visual_80GHz.Checked)
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
                sg_Visual_TD.Plots[1].PlotXY(Time_scale, Baseline_f);
            }

            if (SigMaskAv[iRX, iTX] && BLMaskAv[iRX, iTX] && rb_Visual_TD_Both.Checked)
            {   // If both exist
                // Calibrate (+ display alignments in ps)
                double Delay = 0;
                double[] Wfm_RefSig;
                if (rb_Visual_xCal.Checked)
                {   // Align using max. cross-correlation
                    Delay = align_signals_corr(Baseline_f, Signal_f, 10, 50, out Wfm_SigAl);
                }
                else if (rb_Visual_CalRef.Checked)
                {   // Align using max. cross-correlation
                    Delay = align_signals(BaselineM_r[iRX, iTX], Signal_f, SignalM_r[iRX, iTX], out Wfm_SigAl, out Wfm_RefSig, 1000, sig_limits);
                }
                else Wfm_SigAl = Signal_f;
                if (Math.Abs(Delay * Ts) > 0.1)
                {   // If delay is over 100ps - discard the aligned signal (probably, erroneous alignment) 
                    Align_discarded++;
                    //lb_TD_AlDisc.Text = Align_discarded.ToString();
                    Wfm_SigAl = Signal_f;   // Keep non-aligned signal
                }
                else if (Delay > 0)
                {
                    if (Delay > MaxDelay)
                    {   // Update the max. delay
                        MaxDelay = Delay;
                        //lb_TD_MaxDelay.Text = MaxDelay.ToString();
                    }
                }
                Wfm_SigDiff = array_sub(Wfm_SigAl, Baseline_f);
                // Plot difference
                sg_Visual_TD.Plots[2].PlotXY(Time_scale, Wfm_SigDiff);
                // Display alignment
                //lb_TD_alignment.Text = (Delay * Ts * 1000).ToString();
                return true;
            }
            return false;
        }

        private void rb_Visual_TD_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_TD.Checked)
                Visualizer_TD_Visible(true);
        }

        #endregion // Time Domain

        #region Frequency Domain

        /// <summary>
        /// Reads stored FD baseline values from global array and outputs to Visualizer Graph
        /// </summary>
        private void Visualize_FD_Baseline()
        {
            int RX = (int)nud_Visual_RX.Value - 1;
            int TX = (int)nud_Visual_TX.Value - 1;

            double[] S11_Real = Baseline_S11_Real[RX, TX];
            double[] S11_Imag = Baseline_S11_Imag[RX, TX];

            double[] S21_Real = Baseline_S21_Real[RX, TX];
            double[] S21_Imag = Baseline_S21_Imag[RX, TX];

            double[] S12_Real = Baseline_S12_Real[RX, TX];
            double[] S12_Imag = Baseline_S12_Imag[RX, TX];

            double[] S22_Real = Baseline_S22_Real[RX, TX];
            double[] S22_Imag = Baseline_S22_Imag[RX, TX];

            double[] freq = Baseline_Frequency[RX, TX];

            double[] S11MagLog = new double[freq.Length];
            double[] S22MagLog = new double[freq.Length];
            double[] S21MagLog = new double[freq.Length];
            double[] S12MagLog = new double[freq.Length];

            double[] freq_adj = new double[freq.Length];

            // Output values to graph
            sg_Visual_FD.ClearData();
            //xAxisFreq.Range.Minimum = (double)freq[0] / 1.0e9;

            for (int i = 0; i < freq.Length; i++)
            {
                // S11
                S11MagLog[i] = 20 * Math.Log10(Math.Sqrt(S11_Real[i] * S11_Real[i] + S11_Imag[i] * S11_Imag[i]));
                // S22
                S22MagLog[i] = 20 * Math.Log10(Math.Sqrt(S22_Real[i] * S22_Real[i] + S22_Imag[i] * S22_Imag[i]));
                // S21
                S21MagLog[i] = 20 * Math.Log10(Math.Sqrt(S21_Real[i] * S21_Real[i] + S21_Imag[i] * S21_Imag[i]));
                // S12
                S12MagLog[i] = 20 * Math.Log10(Math.Sqrt(S12_Real[i] * S12_Real[i] + S12_Imag[i] * S12_Imag[i]));
                freq_adj[i] = freq[i] / 1.0e9;
            }

            // S11
            sg_Visual_FD.Plots[0].PlotXY(freq_adj, S11MagLog);
            // S22
            sg_Visual_FD.Plots[1].PlotXY(freq_adj, S22MagLog);
            // S21
            sg_Visual_FD.Plots[2].PlotXY(freq_adj, S21MagLog);
            // S12
            sg_Visual_FD.Plots[3].PlotXY(freq_adj, S12MagLog);
        }

        /// <summary>
        /// Reads stored FD signal values from global array and outputs to Visualizer Graph
        /// </summary>
        private void Visualize_FD_Signal()
        {
            int RX = (int)nud_Visual_RX.Value - 1;
            int TX = (int)nud_Visual_TX.Value - 1;

            double[] S11_Real = Signal_S11_Real[RX, TX];
            double[] S11_Imag = Signal_S11_Imag[RX, TX];

            double[] S21_Real = Signal_S21_Real[RX, TX];
            double[] S21_Imag = Signal_S21_Imag[RX, TX];

            double[] S12_Real = Signal_S12_Real[RX, TX];
            double[] S12_Imag = Signal_S12_Imag[RX, TX];

            double[] S22_Real = Signal_S22_Real[RX, TX];
            double[] S22_Imag = Signal_S22_Imag[RX, TX];

            double[] freq = Signal_Frequency[RX, TX];

            double[] S11MagLog = new double[freq.Length];
            double[] S22MagLog = new double[freq.Length];
            double[] S21MagLog = new double[freq.Length];
            double[] S12MagLog = new double[freq.Length];

            double[] freq_adj = new double[freq.Length];

            // Output values to graph
            sg_Visual_FD.ClearData();
            //xAxisFreq.Range.Minimum = (double)freq[0] / 1.0e9;

            for (int i = 0; i < freq.Length; i++)
            {
                // S11
                S11MagLog[i] = 20 * Math.Log10(Math.Sqrt(S11_Real[i] * S11_Real[i] + S11_Imag[i] * S11_Imag[i]));
                // S22
                S22MagLog[i] = 20 * Math.Log10(Math.Sqrt(S22_Real[i] * S22_Real[i] + S22_Imag[i] * S22_Imag[i]));
                // S21
                S21MagLog[i] = 20 * Math.Log10(Math.Sqrt(S21_Real[i] * S21_Real[i] + S21_Imag[i] * S21_Imag[i]));
                // S12
                S12MagLog[i] = 20 * Math.Log10(Math.Sqrt(S12_Real[i] * S12_Real[i] + S12_Imag[i] * S12_Imag[i]));
                freq_adj[i] = freq[i] / 1.0e9;
            }

            // S11
            sg_Visual_FD.Plots[0].PlotXY(freq_adj, S11MagLog);
            // S22
            sg_Visual_FD.Plots[1].PlotXY(freq_adj, S22MagLog);
            // S21
            sg_Visual_FD.Plots[2].PlotXY(freq_adj, S21MagLog);
            // S12
            sg_Visual_FD.Plots[3].PlotXY(freq_adj, S12MagLog);
        }

        #endregion // Frequency Domain

        /// <summary>
        /// Will set the Visualizer page to Time Domain functionality if ON (true) else set to Frequency.
        /// </summary>
        /// <param name="ON"></param>
        private void Visualizer_TD_Visible(bool ON)
        {
            if (ON) // TD Mode
            {
                sg_Visual_TD.Visible = true;
                sg_Visual_FD.Visible = false;

                legend_Visual_TD.Visible = true;
                legend_Visual_FD.Visible = false;

                rb_Visual_TD_Both.Enabled = true;
                rb_Visual_TD_Both.Visible = true;

                gb_Visual_Settings.Visible = true;

            }
            else // FD Mode
            {
                sg_Visual_TD.Visible = false;
                sg_Visual_FD.Visible = true;

                legend_Visual_TD.Visible = false;
                legend_Visual_FD.Visible = true;

                rb_Visual_TD_Both.Enabled = false;
                rb_Visual_TD_Both.Visible = false;
                if (rb_Visual_TD_Both.Checked)
                    rb_Visual_Baseline.Checked = true;

                gb_Visual_Settings.Visible = false;
            }

        }

        private void rb_Visual_FD_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_FD.Checked)
                Visualizer_TD_Visible(false);
        }

        private void nud_Visual_TX_ValueChanged(object sender, EventArgs e)
        {
            if (rb_Visual_TD.Checked)
                Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
            else
                if (rb_Visual_Baseline.Checked)
                    Visualize_FD_Baseline();
                else
                    Visualize_FD_Signal();
        }

        private void nud_Visual_RX_ValueChanged(object sender, EventArgs e)
        {
            if (rb_Visual_TD.Checked)
                Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
            else
                if (rb_Visual_Baseline.Checked)
                    Visualize_FD_Baseline();
                else
                    Visualize_FD_Signal();

        }

        private void rb_Visual_Baseline_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_Baseline.Checked)
            {
                if (rb_Visual_TD.Checked)
                {
                    Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
                }

                else
                    Visualize_FD_Baseline();

                tsmi_graph_Baseline.Checked = true;
            }
            
        }

        private void rb_Visual_Signal_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_Signal.Checked)
            {
                if (rb_Visual_TD.Checked)
                    Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
                else
                    Visualize_FD_Signal();

                tsmi_graph_Signal.Checked = true;
            }

        }

        private void rb_Visual_TD_Both_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Visual_TD_Both.Checked)
            {
                Visualize_TD((uint)nud_Visual_RX.Value - 1, (uint)nud_Visual_TX.Value - 1);
                tsmi_graph_Baseline.Checked = true;
                tsmi_graph_Signal.Checked = true;
                tsmi_graph_MemDiff.Checked = true;
                tsmi_graph_Response.Checked = true;

            }
        }

        /// <summary>
        /// Will copy to clipboard when the user double clicks the FD graph.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sg_Visual_FD_DoubleClick(object sender, EventArgs e)
        {
            sg_Visual_FD.ToClipboard();
            MessageLog("FD Graph copied to clipboard");
        }

        /// <summary>
        /// Will copy to clipboard when the user double clicks the TD graph.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sg_Visual_TD_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            sg_Visual_TD.ToClipboard();
            MessageLog("TD Graph copied to clipboard.");
        }

        #endregion // Visualizer

        








    }
    
    

}
