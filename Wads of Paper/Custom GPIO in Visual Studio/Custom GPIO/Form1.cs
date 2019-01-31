using System;
using HIDInterface;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Custom_GPIO
/// <summary>
/// Authors:        Evgeny Kirshin, Nicolas Vendeville
/// Created:        May 13, 2013
/// Last Updated:   August 5, 2013.
/// Summary:        This class allows the user to create a Switching Matrix object and to connect to the Switching Matrix.
///                 This allows manual / automatic control of the antenna array.
/// </summary>
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

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

            // GUI of Pin Layout. This replaces SetTXAntenna.

            public void SetGPIO_Click(int box)
            {
                byte[] GPIO_values = new byte[8];

                if (box == 1)
                {
                    GPIO_values[0] = 0x01;
                }
                if (box == 2)
                {
                    GPIO_values[0] = 0x02;
                    GPIO_values[1] = 0x00;
                    GPIO_values[2] = 0x00;
                    GPIO_values[3] = 0x00;
                    GPIO_values[4] = 0x00;
                    GPIO_values[5] = 0x00;
                    GPIO_values[6] = 0x00;
                    GPIO_values[7] = 0x00;
                }
                if (box == 3)
                {
                    GPIO_values[0] = 0x04;
                }
                if (box == 4)
                {
                    GPIO_values[0] = 0x08;
                }
                if (box == 5)
                {
                    GPIO_values[0] = 0x10;
                }
                if (box == 6)
                {
                    GPIO_values[0] = 0x20;
                }
                if (box == 7)
                {
                    GPIO_values[0] = 0x40;
                }
                if (box == 8)
                {
                    GPIO_values[0] = 0x80;
                }
                if (box == 9)
                {
                    GPIO_values[1] = 0x01;
                }
                if (box == 10)
                {
                    GPIO_values[1] = 0x02;
                }
                if (box == 11)
                {
                    GPIO_values[1] = 0x04;
                }
                if (box == 12)
                {
                    GPIO_values[1] = 0x08;
                }
                if (box == 13)
                {
                    GPIO_values[1] = 0x10;
                }
                if (box == 14)
                {
                    GPIO_values[1] = 0x20;
                }
                if (box == 15)
                {
                    GPIO_values[1] = 0x40;
                }
                if (box == 16)
                {
                    GPIO_values[1] = 0x80;
                }
                if (box == 17)
                {
                    GPIO_values[2] = 0x01;
                }
                if (box == 18)
                {
                    GPIO_values[2] = 0x02;
                }
                if (box == 19)
                {
                    GPIO_values[2] = 0x04;
                }
                if (box == 20)
                {
                    GPIO_values[2] = 0x08;
                }
                if (box == 21)
                {
                    GPIO_values[2] = 0x10;
                }
                if (box == 22)
                {
                    GPIO_values[2] = 0x20;
                }
                if (box == 23)
                {
                    GPIO_values[2] = 0x40;
                }
                if (box == 24)
                {
                    GPIO_values[2] = 0x80;
                }
                if (box == 25)
                {
                    GPIO_values[3] = 0x01;
                }
                if (box == 26)
                {
                    GPIO_values[3] = 0x02;
                }
                if (box == 27)
                {
                    GPIO_values[3] = 0x04;
                }
                if (box == 28)
                {
                    GPIO_values[3] = 0x08;
                }
                if (box == 29)
                {
                    GPIO_values[3] = 0x10;
                }
                if (box == 30)
                {
                    GPIO_values[3] = 0x20;
                }
                if (box == 31)
                {
                    GPIO_values[3] = 0x40;
                }
                if (box == 32)
                {
                    GPIO_values[3] = 0x80;
                }
                if (box == 33)
                {
                    GPIO_values[4] = 0x01;
                }
                if (box == 34)
                {
                    GPIO_values[4] = 0x02;
                }
                if (box == 35)
                {
                    GPIO_values[4] = 0x04;
                }
                if (box == 36)
                {
                    GPIO_values[4] = 0x08;
                }
                if (box == 37)
                {
                    GPIO_values[4] = 0x10;
                }
                if (box == 38)
                {
                    GPIO_values[4] = 0x20;
                }
                if (box == 39)
                {
                    GPIO_values[4] = 0x40;
                }
                if (box == 40)
                {
                    GPIO_values[4] = 0x80;
                }
                if (box == 41)
                {
                    GPIO_values[5] = 0x01;
                }
                if (box == 42)
                {
                    GPIO_values[5] = 0x02;
                }
                if (box == 43)
                {
                    GPIO_values[5] = 0x04;
                }
                if (box == 44)
                {
                    GPIO_values[5] = 0x08;
                }
                if (box == 45)
                {
                    GPIO_values[5] = 0x10;
                }
                if (box == 46)
                {
                    GPIO_values[5] = 0x20;
                }
                if (box == 47)
                {
                    GPIO_values[5] = 0x40;
                }
                if (box == 48)
                {
                    GPIO_values[5] = 0x80;
                }
                if (box == 49)
                {
                    GPIO_values[6] = 0x01;
                }
                if (box == 50)
                {
                    GPIO_values[6] = 0x02;
                }

                SetGPIO(GPIO_values);
            }

            //------------- Low-level functions ----------------

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

                mcu.SendReport(ReportOut);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            CSWMatrix blah = new CSWMatrix();
            blah.SetGPIO_Click(2);
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

