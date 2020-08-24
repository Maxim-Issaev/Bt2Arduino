using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Bluetooth;
using System.Collections.Generic;
using Android.Views;
using System;
using System.Threading.Tasks;
using Android.Content;

namespace Bt2Arduino
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private int SelectedID=0;
        
        private Button SendnButton;
        private Button ConnectButton;
        private Spinner Spinner;

        public BluetoothController bluetoothController;
        public MemoryController memoryController;
        
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            bluetoothController = new BluetoothController();
            memoryController = new MemoryController(this);

            SetContentView(Resource.Layout.activity_main);
            
            ConnectButton = FindViewById<Button>(Resource.Id.button1);
            Spinner = FindViewById<Spinner>(Resource.Id.spinner1);
            Spinner.ItemSelected += Spinner_ItemSelected1;
            List<string> names = new List<string>();
            foreach (BluetoothDevice device in bluetoothController.BluetoothDevices)
            {
                names.Add(device.Name);
                if (memoryController.GetTargetAdress() == device.Address)
                {
                    SelectedID = names.Count;
                }
            }
            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, names);
            Spinner.Adapter = adapter;
            Spinner.SetSelection(SelectedID-1);
            ConnectButton.Click += ConnectButton_ClickAsync;

            SendnButton = FindViewById<Button>(Resource.Id.button2);
            SendnButton.Click += SendnButton_ClickAsync;
            
        }

        private async void SendnButton_ClickAsync(object sender, EventArgs e)
        {
            await TryWriteAsync("1");
        }

        private async void ConnectButton_ClickAsync(object sender, EventArgs e)
        {
            if (bluetoothController.conncetionState == ConncetionSate.failed)
            {
                await bluetoothController.ConnectAsync(SelectedID);
                if (bluetoothController.conncetionState == ConncetionSate.sucsesful)
                {
                    Context context = Application.Context;
                    string text = "Успешно!";
                    ToastLength duration = ToastLength.Short;
                    var toast = Toast.MakeText(context, text, duration);
                    toast.Show();

                }   
                else
                {
                    Context context = Application.Context;
                    string text = "Не удалось подключиться!";
                    ToastLength duration = ToastLength.Short;
                    var toast = Toast.MakeText(context, text, duration);
                    toast.Show();
                }
            }
        }

        private void Spinner_ItemSelected1(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SelectedID = Convert.ToInt32(e.Id);
            List<string> adresses = new List<string>();
            foreach (BluetoothDevice device in bluetoothController.BluetoothDevices)
            {
                adresses.Add(device.Address);
            }
            memoryController.SaveTargetAdress(adresses[SelectedID]);
        }
        private async Task TryWriteAsync(string message)
        {
            if (await bluetoothController.WriteAsync(message) != true)
            {
                Context context = Application.Context;
                string text = "Ошибка";
                ToastLength duration = ToastLength.Short;
                var toast = Toast.MakeText(context, text, duration);
                toast.Show();
            }
        }
    }
}