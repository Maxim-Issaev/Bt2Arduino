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

        private bool Listening;

        private OutputStreamInvoker outStream = null;
        private InputStreamInvoker inStream = null;

        private int ReconectId;

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
            ReconectId = TargetId;
            socket = BluetoothDevices[TargetId].CreateRfcommSocketToServiceRecord(mDeviceUUID);
            try
            {
                await socket.ConnectAsync();
                if (socket != null && socket.IsConnected)
                {
                    outStream = (OutputStreamInvoker)socket.OutputStream;
                    conncetionState = ConncetionSate.sucsesful;
                    inStream=(InputStreamInvoker)socket.InputStream;
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
            catch (System.IO.IOException)
            {
                return false;
            }
            catch (Java.IO.IOException)
            {
                return false;
            }
            catch (System.NullReferenceException)
            {
                return false;
            }
        }
        public async Task Listen(TextView textView)
        {
            Listening = true;
            byte[] uintBuffer = new byte[sizeof(uint)]; // This reads the first 4 bytes which form an uint that indicates the length of the string message.
            byte[] textBuffer; // This will contain the string message.

            // Keep listening to the InputStream while connected.
            while (Listening)
            {
                try
                {
                    // This one blocks until it gets 4 bytes.
                    await inStream.ReadAsync(uintBuffer, 0, uintBuffer.Length);
                    uint readLength = BitConverter.ToUInt32(uintBuffer, 0);

                    textBuffer = new byte[readLength];
                    // Here we know for how many bytes we are looking for.
                    await inStream.ReadAsync(textBuffer, 0, (int)readLength);

                    string s = Encoding.UTF8.GetString(textBuffer);
                    textView.Text = "Состояние:"+s;
                }
                catch (Java.IO.IOException e)
                {
                    textView.Text = "Состояние: Ошибка";
                    Listening = false;
                    break;
                }
                catch (System.NullReferenceException)
                {
                    textView.Text = "Состояние: Нет подключения";
                    Listening = false;
                    break;
                }
            }
            textView.Text = "Состояние: Отключено";
        }
        public async Task ListenStopAsync()
        {
            Listening = false;
            socket.Close();
            await ConnectAsync(ReconectId);
        }
        public bool Stop()
        {
            try
            {
                socket.Close();
                return true;
            }
            catch (System.NullReferenceException)
            {
                return false;
            }
        }

    }
    public enum ConncetionSate
    {
        failed,
        sucsesful
    }

}