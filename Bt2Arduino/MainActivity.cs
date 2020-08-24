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
using System.Threading;

namespace Bt2Arduino
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private int SelectedID=0;
        private TextView TextView;

        private Switch Switch1;

        private Button ConnectButton;
        private Spinner Spinner;

        private Button OnOfButton;
        private Button LightPlusButton;
        private Button ModeMinusButton;
        private Button AutoButton;
        private Button ModePlusButton;
        private Button LightMinusButton;
        private Button MusicButton;

        public static BluetoothController bluetoothController;
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
                    SelectedID = names.Count-1;
                }
                ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, names);
                Spinner.Adapter = adapter;
                Spinner.SetSelection(SelectedID);
            }
            ConnectButton.Click += ConnectButton_ClickAsync;

            OnOfButton = FindViewById<Button>(Resource.Id.OnOfButton);
            OnOfButton.Click += OnOfButton_Click;
            LightPlusButton = FindViewById<Button>(Resource.Id.LightPlusButton);
            LightPlusButton.Click += LightPlusButton_Click;
            ModeMinusButton = FindViewById<Button>(Resource.Id.ModeMinusButton);
            ModeMinusButton.Click += ModeMinusButton_Click;
            AutoButton = FindViewById<Button>(Resource.Id.AutoButton);
            AutoButton.Click += AutoButton_Click;
            ModePlusButton = FindViewById<Button>(Resource.Id.ModePlusButton);
            ModePlusButton.Click += ModePlusButton_Click;
            LightMinusButton = FindViewById<Button>(Resource.Id.LightMinusButton);
            LightMinusButton.Click += LightMinusButton_Click;
            MusicButton = FindViewById<Button>(Resource.Id.MusicButton);
            MusicButton.Click += MusicButton_Click;

            Switch1 = FindViewById<Switch>(Resource.Id.switch1);
            Switch1.CheckedChange += Switch1_CheckedChange;
            TextView = FindViewById<TextView>(Resource.Id.OutText);

        }

        private async void Switch1_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (Switch1.Checked==true)
            {
                TextView.Text = "Состояние: Ожидает ввода";
                await bluetoothController.Listen(TextView);
            }
            else
            {
             await   bluetoothController.ListenStopAsync();
                if (bluetoothController.conncetionState==ConncetionSate.sucsesful)
                {
                    TextView.Text = "Состояние: подключено";
                }
            }
        }

        private async void MusicButton_Click(object sender, EventArgs e)
        {
            await TryWriteAsync("7");
        }

        private async void LightMinusButton_Click(object sender, EventArgs e)
        {
            await TryWriteAsync("6");
        }

        private async void ModePlusButton_Click(object sender, EventArgs e)
        {
            await TryWriteAsync("4");
        }

        private async void AutoButton_Click(object sender, EventArgs e)
        {
            await TryWriteAsync("2");
        }

        private async void ModeMinusButton_Click(object sender, EventArgs e)
        {
            await TryWriteAsync("3");
        }

        private async void LightPlusButton_Click(object sender, EventArgs e)
        {
            await TryWriteAsync("5");
        }

        private async void OnOfButton_Click(object sender, EventArgs e)
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
                    TextView.Text = "Состояние: подключено";

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