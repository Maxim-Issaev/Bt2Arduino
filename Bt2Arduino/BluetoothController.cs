using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Java.Util;

namespace Bt2Arduino
{
    
    public class BluetoothController
    {
        private static UUID mDeviceUUID;
        public List<BluetoothDevice> BluetoothDevices;
        private BluetoothAdapter bluetoothAdapter;
        private BluetoothSocket socket;
        public ConncetionSate conncetionState;

        private OutputStreamInvoker outStream = null;
        private InputStreamInvoker inStream = null;

        public BluetoothController()
        {
            mDeviceUUID = Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            BluetoothDevices = new List<BluetoothDevice>();
            ICollection<BluetoothDevice> devices = bluetoothAdapter.BondedDevices;
            foreach (var device in devices)
            {
                BluetoothDevices.Add(device);
            }
            conncetionState = ConncetionSate.failed;

        }
        public async Task ConnectAsync(int TargetId)
        {
            socket = BluetoothDevices[TargetId].CreateRfcommSocketToServiceRecord(mDeviceUUID);
            try
            {
                await socket.ConnectAsync();
                if (socket != null && socket.IsConnected)
                {
                    outStream = (OutputStreamInvoker)socket.OutputStream;
                    conncetionState = ConncetionSate.sucsesful;
                }
                else
                    conncetionState = ConncetionSate.failed;


            }
            catch (Java.IO.IOException)
            {

                conncetionState = ConncetionSate.failed;
            }
        }
        public async Task<bool> WriteAsync(string message)
        {
            try
            {
                uint messageLength = (uint)message.Length;
                byte[] countBuffer = BitConverter.GetBytes(messageLength);
                byte[] buffer = Encoding.UTF8.GetBytes(message);

                await outStream.WriteAsync(countBuffer, 0, countBuffer.Length);
                await outStream.WriteAsync(buffer, 0, buffer.Length);
                return true;
            }
            catch (System.NullReferenceException)
            { return false; }
                
        }
        
    }
    public enum ConncetionSate
    {
        failed,
        sucsesful
    }

}