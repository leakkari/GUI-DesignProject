using System;

namespace VNA
{
    /// <summary>
    /// Authors:        Evgeny Kirshin, Nicolas Vendeville
    /// Created:        May 13, 2013
    /// Last Updated:   August 5, 2013
    /// Summary:        This class allows the user to create a VNA object and to connect to the VNA.
    ///                 Uses Agilent HP8722ES Vector Network Analyzer.
    /// </summary>
    public class CIVNA
    {
        BLXLServersLib.Agt_AnalogChannels eChannel;
        BLXLServersLib.Agt_AnalogChannels ActiveChannel;
        public AgtServer8714Lib.AgtServer8714 VNA;
        private bool state = false;                     //indicates if device is connected
        private string outputMessage = null;            //message for main form log

        private bool connectVNA(string Addr)
        {
            VNA = new AgtServer8714Lib.AgtServer8714();

            try
            {
                VNA.Connect(Addr);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// allow user to connect to VNA / takes VNA address / outputs bool to indicate if connection is successful 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool connect(String address)
        { //Connect to HP8722ES
            string szInstrumentModel;
            string szInstrumentVersion;
            string szConnName;
                                                                                            //Original Code below:
            bool ConnState = connectVNA(address);                                           //bool ConnState = IVNA.Connect(tbx_VNA_HP_add.Text);
            if (!ConnState)
            {
                outputMessage = "Failed to connect to VNA.";                                //LogMsg("Failed to connect to the VNA");
                state = false;
                return state;
            }


            if (VNA.ConnectedStatus)                                                   //if (IVNA.VNA.ConnectedStatus) 
            {   // If successfully connected
                szInstrumentModel = VNA.Utilities.InstrumentModel;                     //szInstrumentModel = IVNA.VNA.Utilities.InstrumentModel;
                szInstrumentVersion = VNA.Utilities.InstrumentFirmwareVersion;         //szInstrumentVersion = IVNA.VNA.Utilities.InstrumentFirmwareVersion;
                szConnName = VNA.ConnectionName;                                       //szConnName = IVNA.VNA.ConnectionName;

                if (SupportedModel())                                                  //if (IVNA.SupportedModel())
                {
                    // The following is an example of how to send SCPI commands to the instrument 
                    string szReply = "";     // Variable to hold string returned from instrument
                    object result = szReply;

                    VNA.Output("*idn?");                                               //IVNA.VNA.Output("*idn?");
                    VNA.Enter(ref result);                                             //IVNA.VNA.Enter(ref result);
                    if (result is string)
                        szReply = (string)result;
                    else
                        throw new Exception("unexpected result type returned");

                    outputMessage = "Connected to " + szReply;                              // LogMsg("Connected to " + szReply);
                                                                                            // LogMsg(szInstrumentModel + ", version " + szInstrumentVersion);  
                    outputMessage = outputMessage + "\n" + szInstrumentModel + ", version " + szInstrumentVersion;           

                    // The following two if statements test the ability to get S1P and S2P data
                    if (VNA.TestS1PObtainable())     // S1P data is obtainable              // if (IVNA.VNA.TestS1PObtainable())   
                        outputMessage = outputMessage + "\n" + "S11 obtainable";            // LogMsg("S11 obtainable");

                    if (VNA.TestS2PObtainable())     // S2P data is obtainable
                        outputMessage = outputMessage + "\n" + "S21 obtainable";            //LogMsg("S21 obtainable");
                    state = true;                                                           //IVNA.state = true;
                    return state;
                }
                else
                {
                    outputMessage = "Instrument " + szInstrumentModel + " at address " + szConnName + " is not a supported instrument.";
                                                                                            //LogMsg("Instrument " + szInstrumentModel + " at address " + szConnName + " is not a supported instrument.");
                }
            }
            state = false;
            return state;
        }

        /// <summary>
        /// allow user to obtain output message
        /// </summary>
        /// <returns></returns>
        public string getOutputMessage()
        {
            return outputMessage;
        }

        /// <summary>
        /// Allow user to see if the VNA is connected. Returns true if ON.
        /// </summary>
        /// <returns></returns>
        public bool getConnectionStatus()
        {
            return state;
        }

        /// <summary>
        /// allow user to disconnect VNA
        /// </summary>
        public void disconnect()
        {
            state = false;
        }
    
        private bool SupportedModel()
        {
            // The following demonstrates how to ensure that the connected instrument is an IntuiLink VNA supported instrument
            dynamic vInstrList;         // variable to hold supported instrument list
            String szModel;             // variable to hold Model string
            int i;                      // counter variable

            // Get the currently connected model and the supported models list
            vInstrList = VNA.Utilities.SupportedModels();
            szModel = VNA.Utilities.InstrumentModel;

            // Search the list and exit true if the model matches a model in the list
            for (i = 0; i < vInstrList.Length; i++)
                if (szModel.ToUpper() == vInstrList[i].ToUpper())
                    return true;
            return false;
        }

        public bool SaveSxPData(String FileName, String DeviceDescription, String eType)
        {
            dynamic freq;       // these variants hold the SxP data
            dynamic s11data = null, s12data = null, s13data = null;
            dynamic s21data = null, s22data = null, s23data = null;
            dynamic s31data = null, s32data = null, s33data = null;

            double fstart, fstop, fstep, fcurrent;      // variables for frequency data
            int fpoints;                // variable for sweep points
            String ss;                  // variable to hold the string data
            String sformat;             // variable to hold the string data format
            int i;                      // counter variable
            double dImpedance;          // variable to hold characteristic impedance value
            String sImpedance;          // variable to hold string converted impedance value
            bool bFreqValid;            // flag denoting if frequency is valid
            bool bValidBounds = false;  // flag denoting if data bounds are valid

            // This subroutine captures the SxP data and writes it to the file with the appropriate
            // given filename in a formatted fashion. Since all channels need to be synchronized in
            // order to get SxP data, any channel can be passed as a parameter to those functions
            // that require one.

            fstart = VNA.Measure.SenseFrequencyStart((BLXLServersLib._ANALOGCHANNELS)BLXLServersLib.Agt_AnalogChannels.Agt_AnalogChannel_1);
            fstop = VNA.Measure.SenseFrequencyStop((BLXLServersLib._ANALOGCHANNELS)BLXLServersLib.Agt_AnalogChannels.Agt_AnalogChannel_1);
            fpoints = VNA.Measure.SenseSweepPoints((BLXLServersLib._ANALOGCHANNELS)BLXLServersLib.Agt_AnalogChannels.Agt_AnalogChannel_1);

            switch (eType)
            {
                case "S1P":
                    VNA.GetS1PData(out freq, out s11data);
                    break;
                case "S2P":
                    VNA.GetS2PData(out freq, out s11data, out s21data, out s12data, out s22data);
                    break;
                default:
                    return false;
            }

            if (freq == null)
                bFreqValid = false;
            else
                bFreqValid = true;

            dImpedance = VNA.Measure.SenseZ0((BLXLServersLib._ANALOGCHANNELS)BLXLServersLib.Agt_AnalogChannels.Agt_AnalogChannel_1);

            //  Create a file of the appropriate filename
            System.IO.StreamWriter fil = new System.IO.StreamWriter(FileName);

            // Write header information to the file
            fil.WriteLine("! " + "Device Description:   " + DeviceDescription);
            fil.WriteLine("! Source: Agilent IntuiLink VNA");
            fil.WriteLine("! " + "Instrument Manufacturer:   " + VNA.Utilities.InstrumentManufacturer);
            fil.WriteLine("! " + "Instrument Model:          " + VNA.Utilities.InstrumentModel);
            fil.WriteLine("! " + "Instrument Serial Number:  " + VNA.Utilities.InstrumentSerialNumber);
            fil.WriteLine("! " + "Instrument Firmware Rev:   " + VNA.Utilities.InstrumentFirmwareVersion);

            switch (eType)
            {
                case "S1P":
                    fil.WriteLine("! " + "Frequency Re{S11} Im{S11}");
                    bValidBounds = true;
                    break;
                case "S2P":
                    fil.WriteLine("! " + "Frequency Re{S11} Im{S11}" + ((s21data != null) ? " Re{S21} Im{S21}" : "") + ((s12data != null) ? " Re{S12} Im{S12}" : "") + ((s22data != null) ? "Re{S22} Im{S22}" : ""));
                    bValidBounds = true;
                    break;
                case "S3P":
                    fil.WriteLine("! " + "Frequency Re{S11} Im{S11} Re{S12} Im{S12} Re{S13} Im{S13} Re{S21} Im{S21} Re{S22} Im{S22} Re{S23} Im{S23} Re{S31} Im{S31} Re{S32} Im{S32} Re{S33} Im{S33}");
                    bValidBounds = true;
                    break;
            }

            sImpedance = dImpedance.ToString("0.00");
            fil.WriteLine("# HZ  S  RI R " + sImpedance);

            if (bValidBounds)
            {
                fcurrent = fstart;
                fstep = (fstop - fstart) / (fpoints - 1);

                sformat = "  0.000000E+00; -0.000000E+00;  0.0         ";

                for (i = 0; i < s11data.Length; i += 2)
                {
                    if (bFreqValid)
                        fcurrent = freq[i / 2];

                    ss = fcurrent.ToString(" 0##########") + " ";
                    ss = ss + s11data[i].ToString(sformat) + " ";
                    ss = ss + s11data[i + 1].ToString(sformat) + " ";

                    if (eType == "S2P")
                    {
                        if (s21data != null)
                        {
                            ss = ss + s21data[i].ToString(sformat) + " ";
                            ss = ss + s21data[i + 1].ToString(sformat) + " ";
                        }
                        if (s12data != null)
                        {
                            ss = ss + s12data[i].ToString(sformat) + " ";
                            ss = ss + s12data[i + 1].ToString(sformat) + " ";
                        }
                        if (s22data != null)
                        {
                            ss = ss + s22data[i].ToString(sformat) + " ";
                            ss = ss + s22data[i + 1].ToString(sformat);
                        }
                    }
                    if (eType == "S3P")
                    {
                        ss = ss + s12data[i].ToString(sformat) + " ";
                        ss = ss + s12data[i + 1].ToString(sformat) + " ";
                        ss = ss + s13data[i].ToString(sformat) + " ";
                        ss = ss + s13data[i + 1].ToString(sformat) + " ";
                        ss = ss + s21data[i].ToString(sformat) + " ";
                        ss = ss + s21data[i + 1].ToString(sformat) + " ";
                        ss = ss + s22data[i].ToString(sformat) + " ";
                        ss = ss + s22data[i + 1].ToString(sformat);
                        ss = ss + s23data[i].ToString(sformat) + " ";
                        ss = ss + s23data[i + 1].ToString(sformat);
                        ss = ss + s31data[i].ToString(sformat) + " ";
                        ss = ss + s31data[i + 1].ToString(sformat) + " ";
                        ss = ss + s32data[i].ToString(sformat) + " ";
                        ss = ss + s32data[i + 1].ToString(sformat);
                        ss = ss + s33data[i].ToString(sformat) + " ";
                        ss = ss + s33data[i + 1].ToString(sformat);
                    }
                    fil.WriteLine(ss);
                }
                if (!bFreqValid)
                    fcurrent = fcurrent + fstep;
                else
                    fil.WriteLine("! Different number of points were read from each channel");
            }
            // Close the file
            fil.Close();
            return true;
        }

    }

    public class VNAPkg
    {
        public double[] S11;
        public double[] S21;
        public double[] S12;
        public double[] S22;
        public double[] freq;
        public uint PkgNum;
        public uint iTX;
        public uint iRX;
        //public AcqMode Mode;
        public bool LastPkg = false;
    }

}