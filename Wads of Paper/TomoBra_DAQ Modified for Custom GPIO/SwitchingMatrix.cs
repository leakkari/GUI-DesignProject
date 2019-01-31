using System;
using HIDInterface;
using System.Drawing;
using System.Windows.Forms;

namespace SwitchingMatrix
{
    /// <summary>
    /// Authors:        Evgeny Kirshin, Nicolas Vendeville
    /// Created:        May 13, 2013
    /// Last Updated:   August 5, 2013.
    /// Summary:        This class allows the user to create a Switching Matrix object and to connect to the Switching Matrix.
    ///                 This allows manual / automatic control of the antenna array.
    /// </summary>
    public class CSWMatrix
    {
        CMCU mcu;
        public int curr_TX; // Range of Values [1,16] or = -1 (AKA unknown state)
        public int curr_RX; // Range of Values [1,16] or = -1 (AKA unknown state)

        public CSWMatrix()
        {
            mcu = new CMCU();
            curr_RX = -1;
            curr_TX = -1;

        }

        /// <summary>
        /// Connect the Switching Matrix
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            return mcu.Connect();
        }

        /// <summary>
        /// Disconnect the Switching Matrix
        /// </summary>
        public void Disconnect()
        {
            mcu.Disconnect();
        }

        // GUI of Pin Layout. This replaces SetTXAntenna (above).

        private void bn_SetGPIO_Click(object sender, EventArgs e)
        {
            CheckBox checkbox1 = new CheckBox();
            CheckBox checkbox2 = new CheckBox();
            CheckBox checkbox3 = new CheckBox();
            CheckBox checkbox4 = new CheckBox();
            CheckBox checkbox5 = new CheckBox();
            CheckBox checkbox6 = new CheckBox();
            CheckBox checkbox7 = new CheckBox();
            CheckBox checkbox8 = new CheckBox();
            CheckBox checkbox9 = new CheckBox();
            CheckBox checkbox10 = new CheckBox();
            CheckBox checkbox11 = new CheckBox();
            CheckBox checkbox12 = new CheckBox();
            CheckBox checkbox13 = new CheckBox();
            CheckBox checkbox14 = new CheckBox();
            CheckBox checkbox15 = new CheckBox();
            CheckBox checkbox16 = new CheckBox();
            CheckBox checkbox17 = new CheckBox();
            CheckBox checkbox18 = new CheckBox();
            CheckBox checkbox19 = new CheckBox();
            CheckBox checkbox20 = new CheckBox();
            CheckBox checkbox21 = new CheckBox();
            CheckBox checkbox22 = new CheckBox();
            CheckBox checkbox23 = new CheckBox();
            CheckBox checkbox24 = new CheckBox();
            CheckBox checkbox25 = new CheckBox();
            CheckBox checkbox26 = new CheckBox();
            CheckBox checkbox27 = new CheckBox();
            CheckBox checkbox28 = new CheckBox();
            CheckBox checkbox29 = new CheckBox();
            CheckBox checkbox30 = new CheckBox();
            CheckBox checkbox31 = new CheckBox();
            CheckBox checkbox32 = new CheckBox();
            CheckBox checkbox33 = new CheckBox();
            CheckBox checkbox34 = new CheckBox();
            CheckBox checkbox35 = new CheckBox();
            CheckBox checkbox36 = new CheckBox();
            CheckBox checkbox37 = new CheckBox();
            CheckBox checkbox38 = new CheckBox();
            CheckBox checkbox39 = new CheckBox();
            CheckBox checkbox40 = new CheckBox();
            CheckBox checkbox41 = new CheckBox();
            CheckBox checkbox42 = new CheckBox();
            CheckBox checkbox43 = new CheckBox();
            CheckBox checkbox44 = new CheckBox();
            CheckBox checkbox45 = new CheckBox();
            CheckBox checkbox46 = new CheckBox();
            CheckBox checkbox47 = new CheckBox();
            CheckBox checkbox48 = new CheckBox();
            CheckBox checkbox49 = new CheckBox();
            CheckBox checkbox50 = new CheckBox();
            CheckBox checkbox51 = new CheckBox();
            CheckBox checkbox52 = new CheckBox();
            CheckBox checkbox53 = new CheckBox();
            CheckBox checkbox54 = new CheckBox();
            CheckBox checkbox55 = new CheckBox();
            CheckBox checkbox56 = new CheckBox();
            CheckBox checkbox57 = new CheckBox();
            CheckBox checkbox58 = new CheckBox();
            byte[] GPIO_values = new byte[9];

            if (checkbox1.Checked == true)
            {
                GPIO_values[0] = 0x01;
            }
            if (checkbox2.Checked == true)
            {
                GPIO_values[0] = 0x02;
            }
            if (checkbox3.Checked == true)
            {
                GPIO_values[0] = 0x04;
            }
            if (checkbox4.Checked == true)
            {
                GPIO_values[0] = 0x08;
            }
            if (checkbox5.Checked == true)
            {
                GPIO_values[0] = 0x10;
            }
            if (checkbox6.Checked == true)
            {
                GPIO_values[0] = 0x20;
            }
            if (checkbox7.Checked == true)
            {
                GPIO_values[0] = 0x40;
            }
            if (checkbox8.Checked == true)
            {
                GPIO_values[0] = 0x80;
            }
            if (checkbox9.Checked == true)
            {
                GPIO_values[1] = 0x01;
            }
            if (checkbox10.Checked == true)
            {
                GPIO_values[1] = 0x02;
            }
            if (checkbox11.Checked == true)
            {
                GPIO_values[1] = 0x04;
            }
            if (checkbox12.Checked == true)
            {
                GPIO_values[1] = 0x08;
            }
            if (checkbox13.Checked == true)
            {
                GPIO_values[1] = 0x10;
            }
            if (checkbox14.Checked == true)
            {
                GPIO_values[1] = 0x20;
            }
            if (checkbox15.Checked == true)
            {
                GPIO_values[1] = 0x40;
            }
            if (checkbox16.Checked == true)
            {
                GPIO_values[1] = 0x80;
            }
            if (checkbox17.Checked == true)
            {
                GPIO_values[2] = 0x01;
            }
            if (checkbox18.Checked == true)
            {
                GPIO_values[2] = 0x02;
            }
            if (checkbox19.Checked == true)
            {
                GPIO_values[2] = 0x04;
            }
            if (checkbox20.Checked == true)
            {
                GPIO_values[2] = 0x08;
            }
            if (checkbox21.Checked == true)
            {
                GPIO_values[2] = 0x10;
            }
            if (checkbox22.Checked == true)
            {
                GPIO_values[2] = 0x20;
            }
            if (checkbox23.Checked == true)
            {
                GPIO_values[2] = 0x40;
            }
            if (checkbox24.Checked == true)
            {
                GPIO_values[2] = 0x80;
            }
            if (checkbox25.Checked == true)
            {
                GPIO_values[3] = 0x01;
            }
            if (checkbox26.Checked == true)
            {
                GPIO_values[3] = 0x02;
            }
            if (checkbox27.Checked == true)
            {
                GPIO_values[3] = 0x04;
            }
            if (checkbox28.Checked == true)
            {
                GPIO_values[3] = 0x08;
            }
            if (checkbox29.Checked == true)
            {
                GPIO_values[3] = 0x10;
            }
            if (checkbox30.Checked == true)
            {
                GPIO_values[3] = 0x20;
            }
            if (checkbox31.Checked == true)
            {
                GPIO_values[3] = 0x40;
            }
            if (checkbox32.Checked == true)
            {
                GPIO_values[3] = 0x80;
            }
            if (checkbox33.Checked == true)
            {
                GPIO_values[4] = 0x01;
            }
            if (checkbox34.Checked == true)
            {
                GPIO_values[4] = 0x02;
            }
            if (checkbox35.Checked == true)
            {
                GPIO_values[4] = 0x04;
            }
            if (checkbox36.Checked == true)
            {
                GPIO_values[4] = 0x08;
            }
            if (checkbox37.Checked == true)
            {
                GPIO_values[4] = 0x10;
            }
            if (checkbox38.Checked == true)
            {
                GPIO_values[4] = 0x20;
            }
            if (checkbox39.Checked == true)
            {
                GPIO_values[4] = 0x40;
            }
            if (checkbox40.Checked == true)
            {
                GPIO_values[4] = 0x80;
            }
            if (checkbox41.Checked == true)
            {
                GPIO_values[5] = 0x01;
            }
            if (checkbox42.Checked == true)
            {
                GPIO_values[5] = 0x02;
            }
            if (checkbox43.Checked == true)
            {
                GPIO_values[5] = 0x04;
            }
            if (checkbox44.Checked == true)
            {
                GPIO_values[5] = 0x08;
            }
            if (checkbox45.Checked == true)
            {
                GPIO_values[5] = 0x10;
            }
            if (checkbox46.Checked == true)
            {
                GPIO_values[5] = 0x20;
            }
            if (checkbox47.Checked == true)
            {
                GPIO_values[5] = 0x40;
            }
            if (checkbox48.Checked == true)
            {
                GPIO_values[5] = 0x80;
            }
            if (checkbox49.Checked == true)
            {
                GPIO_values[6] = 0x01;
            }
            if (checkbox50.Checked == true)
            {
                GPIO_values[6] = 0x02;
            }
            if (checkbox51.Checked == true)
            {
                GPIO_values[6] = 0x04;
            }
            if (checkbox52.Checked == true)
            {
                GPIO_values[6] = 0x08;
            }
            if (checkbox53.Checked == true)
            {
                GPIO_values[6] = 0x10;
            }
            if (checkbox54.Checked == true)
            {
                GPIO_values[6] = 0x20;
            }
            if (checkbox55.Checked == true)
            {
                GPIO_values[6] = 0x40;
            }
            if (checkbox56.Checked == true)
            {
                GPIO_values[6] = 0x80;
            }
            if (checkbox57.Checked == true)
            {
                GPIO_values[7] = 0x01;
            }
            if (checkbox58.Checked == true)
            {
                GPIO_values[7] = 0x02;
            }

            SetGPIO(GPIO_values);
        }

        //------------- Low-level functions ----------------
        public void PowerOnOff(byte State)
        {
            curr_RX = -1;
            curr_TX = -1;

            // Prepare the report
            byte[] ReportOut = new byte[2];
            ReportOut[0] = 15;   // Power ON/OFF
            ReportOut[1] = State;   // Power state
            mcu.SendReport(ReportOut);        // Send the report  */   
            
        }

        public void SetGPIO(byte[] word)
        {
            // Prepare the report
            byte[] ReportOut = new byte[10];
            ReportOut[0] = 1;
            ReportOut[1] = word[0];
            ReportOut[2] = word[1];
            ReportOut[3] = word[2];
            ReportOut[4] = word[3];
            ReportOut[5] = word[4];
            ReportOut[6] = word[5];
            ReportOut[7] = word[6];
            ReportOut[8] = word[7];
            ReportOut[9] = word[8];
            mcu.SendReport(ReportOut);
        }

        // Sets the state of PulseGate output
        public void SetGatingOutput(byte State)
        {
            // Prepare the report
            byte[] ReportOut = new byte[2];
            ReportOut[0] = 14;   // Report #14 - PG_CTL 
            ReportOut[1] = State;
            mcu.SendReport(ReportOut);    // Send the report      
        }

        // Sets the state of MUX_EN signal
        public void SetMUX_EN(byte State)
        {
            // Prepare the report
            byte[] ReportOut = new byte[2];
            ReportOut[0] = 17;   // Report #17 - MUX_EN 
            ReportOut[1] = State;
            mcu.SendReport(ReportOut);    // Send the report
            if (State == 0)
            {
                curr_TX = -1;
                curr_RX = -1;
            }
        }

        // Sets the state of SW_EN signal
        public void SetSW_EN(byte State)
        {
            // Prepare the report
            byte[] ReportOut = new byte[2];
            ReportOut[0] = 13;   // Report #13 - SW_EN 
            ReportOut[1] = State;
            mcu.SendReport(ReportOut);    // Send the report
            if (State == 0)
            {
                curr_TX = -1;
                curr_RX = -1;
            }
        }

        // Sets the state of Fans (ON/OFF)
        public void SetFansState(byte State)
        {
            // Prepare the report
            byte[] ReportOut = new byte[2];
            ReportOut[0] = 18;   // Report #18 - Fans
            ReportOut[1] = State;
            mcu.SendReport(ReportOut);    // Send the report      
        }

    }
    public class CMCU
    {
        // Interaction with MCU
        HIDDevice device = null;
        bool bMCUConnected = false;

        public bool Connect()
        {
            if (bMCUConnected) return true;
            //Get the details of all connected USB HID devices
            HIDDevice.interfaceDetails[] devices = HIDDevice.getConnectedDevices();

            if (devices.Length > 0)
            {   // List all available device paths in the listbox
                for (int i = 0; i < devices.Length; i++)
                {
                    // Search for the needed device
                    if (devices[i].VID == 0x0483 && devices[i].PID == 0x5710)
                    {
                        // MCU found => connect
                        device = new HIDDevice(devices[i].VID, devices[i].PID, (ushort)devices[i].serialNumber, false);
                        if (device != null) bMCUConnected = true;
                    }
                }
            }
            return bMCUConnected;
        }

        public void Disconnect()
        {
            //close the device to release all handles etc
            if (device != null)
            {
                device.close();
                device = null;
            }
        }

        public bool IsConnected()
        {
            return bMCUConnected;
        }

        public void SendReport(byte[] Report)
        {
            device.write(Report);
        }

    }
}