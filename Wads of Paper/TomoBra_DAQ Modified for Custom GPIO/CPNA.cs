using System;
using Agilent.CommandExpert.ScpiNet.AgPNA_A_09_50_13;

namespace PNA
{
    /// <summary>
    /// Authors:        Evgeny Kirshin, Nicolas Vendeville
    /// Created:        May 13, 2013
    /// Last Updated:   July 12, 2013
    /// Summary:        This class allows the user to create a PNA object and to connect to the PNA.
    ///                 Uses Agilent N5242A PNA-X Microwave Analyzer.
    /// </summary>
    public class PNApkg
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

    public class CPNA
    {
        public AgPNA n5242a = null;
        private bool state = false; //indicates if device is connected
        private string outputMessage = null;

        private void GetData(out double[] freq, out double[] S11, out double[] S21, out double[] S12, out double[] S22)
        {
            int opc;
            n5242a.SCPI.INITiate.CONTinuous.Command(null, false);
            n5242a.SCPI.INITiate.IMMediate.Command(null);
            n5242a.SCPI.OPC.Query(out opc); // wait for pending operations to complete

            //n5242a.SCPI.INITiate.CONTinuous.Command(null, false);   // Stop continuous acquisition
            //n5242a.SCPI.INITiate.IMMediate.Command(null);           // Initiate single acquisition

            // S11
            n5242a.SCPI.CALCulate.PARameter.SELect.Command(null, "S11_1G_6G_10K");
            n5242a.SCPI.CALCulate.DATA.QueryBlockReal64(null, "SDATA", out S11);
            n5242a.SCPI.SENSe.X.VALues.QueryBlockReal64(null, out freq);

            // S22
            n5242a.SCPI.CALCulate.PARameter.SELect.Command(null, "S22_1G_6G_10K");
            n5242a.SCPI.CALCulate.DATA.QueryBlockReal64(null, "SDATA", out S22);
            //n5242a.SCPI.SENSe.X.VALues.QueryBlockReal64(null, out freq);

            // S21
            n5242a.SCPI.CALCulate.PARameter.SELect.Command(null, "S21_1G_6G_10K");
            n5242a.SCPI.CALCulate.DATA.QueryBlockReal64(null, "SDATA", out S21);

            // S12
            n5242a.SCPI.CALCulate.PARameter.SELect.Command(null, "S12_1G_6G_10K");
            n5242a.SCPI.CALCulate.DATA.QueryBlockReal64(null, "SDATA", out S12);
        }

       
      
        /// <summary>
        /// allow user to see if the PNA is connected. Returns true if ON.
        /// </summary>
         /// <returns></returns>
        public bool getConnectionStatus()
        {
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
        /// allow user to disconnect PNA
        /// </summary>
        public void disconnect()
        {
            n5242a = null;
            state = false;
        }

        /// <summary>
        /// Attempt to connect to PNA.
        /// Program takes an address "address" as input. 
        /// outputs a string "output" to the main program, informing whether the PNA is connected
        /// the program also updates bool state
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool connect(string address)
        {

            try
            {
                n5242a = new AgPNA(address); //n5242a = new AgPNA(tbx_VNA_Ag_add.Text);
            }
            catch (Exception ex)
            {
                state = false; //return false;
                outputMessage = "Error connecting to PNA.";
                return false; //LogMsg("Error connecting VNA!");
            }

            if (n5242a == null)
            {
                outputMessage = "Error connecting to PNA";
                return false;
            }

            n5242a.SCPI.IDN.Query(out outputMessage);   // n5242a.SCPI.IDN.Query(out IDN);
            // MsgLog(IDN);

            state = true;
            return true; //return true;

        }

        /// <summary>
        /// Attempt to connect (if not connected) and configure the PNA and outputs a message to main program
        /// </summary>
        /// <param name="address"></param>
        /// <param name="BWOutside"></param>
        /// <param name="BWMain"></param>
        public void configure(string address, double BWOutside, double BWMain)
        {
            // Connect to VNA (if not connected)
            if (n5242a == null)
                if (!connect(address))
                {
                    outputMessage = "Cannot connect to PNA.";
                    return;
                }

            n5242a.Transport.DefaultTimeout.Set(20000);
            n5242a.SCPI.CLS.Command();
            n5242a.SCPI.SYSTem.PRESet.Command();
            int opc;
            n5242a.SCPI.OPC.Query(out opc); // wait for pending operations to complete.


            // These four commands are replaced with the block that sends segment table and sets segmented sweep type
            //n5242a.SCPI.SENSe.BANDwidth.RESolution.Command(1, 10000);
            //n5242a.SCPI.SENSe.FREQuency.STARt.Command(null, 1, "GHZ");
            //n5242a.SCPI.SENSe.FREQuency.STOP.Command(null, 6, "GHZ");
            //n5242a.SCPI.SENSe.SWEep.POINts.Command(null, 4096);

            // Segmented sweep
            n5242a.SCPI.FORMat.DATA.Command("ASCii", null);
            n5242a.SCPI.SENSe.SEGMent.LIST.CommandAsciiReal(1u, null, "SSTOP", 4, new double[] {    1, 128, 1e6, 1e9, (double)BWOutside, 0.02, 8,
                                                                                                    1, 4096, 1e9, 6e9, (double)BWMain, 0.02, 8, 
                                                                                                    1, 1024, 6e9, 12e9, (double)BWOutside, 0.02, 8, 
                                                                                                    1, 1024, 12e9, 26e9, (double)BWOutside, 0.02, 8});
            n5242a.SCPI.SENSe.SEGMent.BANDwidth.RESolution.CONTrol.Command(1u, null, true);
            n5242a.SCPI.SENSe.SWEep.TYPE.Command(1u, "SEGMent");
            //n5242a.SCPI.DISPlay.WINDow.TABLe.Command(1u, "SEGMent");

            n5242a.SCPI.CALCulate.PARameter.DELete.ALL.Command(null);

            n5242a.SCPI.FORMat.DATA.Command("REAL", 64);
            n5242a.SCPI.FORMat.BORDer.Command("SWAPped");

            // S11 - window 1, trace 1
            n5242a.SCPI.CALCulate.PARameter.DEFine.EXTended.Command(null, "S11_1G_6G_10K", "S11");
            n5242a.SCPI.DISPlay.WINDow.TRACe.FEED.Command(1, 1, "S11_1G_6G_10K");

            // S22 - window 1, trace 2
            n5242a.SCPI.CALCulate.PARameter.DEFine.EXTended.Command(null, "S22_1G_6G_10K", "S22");
            //n5242a.SCPI.DISPlay.WINDow.STATe.Command(1u, true);
            n5242a.SCPI.DISPlay.WINDow.TRACe.FEED.Command(1, 2, "S22_1G_6G_10K");

            // S21 - window 2, trace 1
            n5242a.SCPI.CALCulate.PARameter.DEFine.EXTended.Command(null, "S21_1G_6G_10K", "S21");
            n5242a.SCPI.DISPlay.WINDow.STATe.Command(2, true);
            n5242a.SCPI.DISPlay.WINDow.TRACe.FEED.Command(2, 1, "S21_1G_6G_10K");
            //n5242a.SCPI.TRIGger.SEQuence.SOURce.Command("MANual");

            // S12 - window 2, trace 2
            n5242a.SCPI.CALCulate.PARameter.DEFine.EXTended.Command(null, "S12_1G_6G_10K", "S12");
            //n5242a.SCPI.DISPlay.WINDow.STATe.Command(2u, true);
            n5242a.SCPI.DISPlay.WINDow.TRACe.FEED.Command(2, 2, "S12_1G_6G_10K");
            //n5242a.SCPI.TRIGger.SEQuence.SOURce.Command("MANual");

            n5242a.SCPI.TRIGger.SEQuence.SOURce.Command("IMMediate");

            outputMessage = "PNA Configured. Outside BW: " + BWOutside.ToString() + " Main BW: " + BWMain.ToString();
        }
    }


}