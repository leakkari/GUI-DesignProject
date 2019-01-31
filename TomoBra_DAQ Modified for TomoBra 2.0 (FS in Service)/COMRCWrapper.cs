using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Picoscope
{
    /// <summary>
    /// Authors:        Evgeny Kirshin, Nicolas Vendeville
    /// Created:        May 13, 2013
    /// Last Updated:   May 23, 2013
    /// Summary:        This class allows the user to create a COMRCWrapper object and to connect to the Picoscope.
    ///                 Used for taking Time-Domain measurements. 
    ///                 Uses Agilent PS9201A.
    ///                 
    /// </summary>
    public class COMRCWrapper
    {
        PicoScope9000.COMRC PS9000;

        public COMRCWrapper()
        {
            PS9000 = new PicoScope9000.COMRC();
        }

        public String ExecCommand(String Command)
        {   // Executes a command, returns string result or raises and exception
            String Result = PS9000.ExecCommand(Command);
            if (Result == null)
            {
                return "";
            }
            else if (Result == "ERROR")
            {
                throw new Exception("PicoScope9000.COMRC command error (" + Command + ")");
            }
            else
            {
                return Result;
            }
        }

        /// <summary>
        /// Executes all the commands regardless if there was an error in one of them
        /// The return value indicates if all the commands have executed correctly (==1)
        /// or at least one failed (==0)
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        public uint ExecCommands(List<string> commands)
        {
            uint result = 1;
            foreach (string c in commands)
            {
                if (ExecCommand(c) == "ERROR")
                {
                    result = 0;
                }
            }
            return result;
        }

        /// <summary>
        ///  Commands to PicoScope to collect data
        /// </summary>
        public void RunSingle()
        {
            ExecCommand("*StopSingle Single");
            bool acq_progress = true;
            ExecCommand("Header Off"); // Switching off headers in results
            Thread current = Thread.CurrentThread;
            while (acq_progress)
            {
                Thread.Sleep(10);
                string result_s = ExecCommand("*StopSingle?");
                if (result_s == "STOP") acq_progress = false;
            }
        }

        /// <summary>
        ///  Transfers data to the program from the COM server
        /// </summary>
        /// <param name="Channel"></param>
        /// <returns></returns>
        public double[] GetWaveForm(uint Channel)
        {
            // Request the collected data
            // 1. Set the source channel
            ExecCommand("Wfm:Source " + ((Channel == 1) ? "Ch1" : "Ch2"));
            // 2. Get the data in string
            string data_s = ExecCommand("Wfm:Data?");
            // 3. Parse the string into the array of doubles
            double[] data_d = new double[Constants.sig_len];
            string[] entries_s = data_s.Split(',');

            uint i = 0;
            bool result = true;
            double value;
            foreach (string c in entries_s)
            {
                result = result & double.TryParse(c, out value);
                data_d[i++] = value;
            }
            return data_d;
        }

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

        public class Constants
        {
            public const uint sig_len = 4096;
            public const uint nSensors = 16;
        }
    }

}