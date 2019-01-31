using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SwitchingMatrix;
using HIDInterface;

namespace CMCU
{
    public class MCU
    {
        HIDDevice device;               // The device that is communicated with
        bool bMCUConnected;             // Is the MCU connected

        // Initialize the microcontroller
        public MCU()
        {
            device = null;
            bMCUConnected = false;
        }

        // Connect the microcontroller
        public bool Connect(ushort PID)
        {
            // Check if the MCU is already connected
            if (bMCUConnected) return true;

            // Get all the connected devices
            HIDDevice.interfaceDetails[] devices = HIDDevice.getConnectedDevices();


            // List all available device paths in the listbox
            if (devices.Length > 0)
            {  
                for (int i = 0; i < devices.Length; i++)
                {
                    // Search for the needed device
                    if (devices[i].VID == 0x0483 && devices[i].PID == PID)
                    {
                        // MCU found => connect it
                        device = new HIDDevice(devices[i].VID, devices[i].PID, (ushort)devices[i].serialNumber, false);
                    }
                }
                if (device != null) bMCUConnected = true;
            }
            return bMCUConnected;
        }

        //close the device to release all handles etc
        public void Disconnect()
        {
            if (device != null)
            {
                device.close();
                device = null;
            }
        }

        // Returns true if the MCU is connected
        public bool IsConnected()
        {
            return bMCUConnected;
        }

        // Used to communicate with the MCU. Can send bytes through USB
        public void SendReport(byte[] Report)
        {
            device.write(Report);
        }


    }
}