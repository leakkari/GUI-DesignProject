using System;
using CMCU;
using HIDInterface;

namespace SwitchingMatrix
{
    /// <summary>
    /// Author:         Karim El Hallaoui
    /// Created:        May 8, 2017
    /// Last Updated:   May 8, 2017
    /// Summary:        This class allows the user to create a Switching Matrix object and to connect to the Switching Matrix.
    ///                 This allows control of the antenna array.
    /// </summary>
    public class SWMatrix
    {
        MCU mcu;

        public bool MCUx1 = true;       // true when MCU is connected, false otherwise.
        public int curr_TX;             // Range of Values [1,16] or = -1 (AKA unknown state)
        public int curr_RX;             // Range of Values [1,16] or = -1 (AKA unknown state)

        public SWMatrix()
        {
            mcu = new MCU();
            curr_RX = -1;
            curr_TX = -1;
        }

        /// Connect the Switching Matrix
        public bool Connect()
        {
            if (MCUx1) return (mcu.Connect(0x5711));
            else return mcu.Connect(0x5710);
        }

        /// Disconnect the Switching Matrix
        public void Disconnect()
        {
            mcu.Disconnect();
        }

        // Changes which antenna is transmitting (TX) to antenna AntNum.
        // (Antennas numbered starting from 1...16)
        public void SetTXAntenna(int AntNum)
        {
            byte[] ReportOut = new byte[2];
            ReportOut[0] = 1;                       // ReportOut[0]=1 means Tx
            ReportOut[1] = (byte)AntNum;            // The antenna we want to activate
            mcu.SendReport(ReportOut);              // Send the report
        }

        // Changes which antenna is receiving (RX) to antenna AntNum.
        // (Antennas numbered starting from 1...16)
        public void SetRXAntenna(int AntNum)
        {
            byte[] ReportOut = new byte[2];
            ReportOut[0] = 2;                       // ReportOut[0]=2 means Rx
            ReportOut[1] = (byte)AntNum;            // The antenna we want to activate
            mcu.SendReport(ReportOut);              // Send the report
        }
    }
}