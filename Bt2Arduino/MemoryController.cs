using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;


namespace Bt2Arduino
{
    public class MemoryController
    {
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private static string TargetAdress;

        public MemoryController(Android.Content.Context context)
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            editor = prefs.Edit();
            TargetAdress = prefs.GetString("TargetAdress","null");
        }
        public string GetTargetAdress()
        {
            return TargetAdress;
        }
        public void SaveTargetAdress(string Adress)
        {
            editor.PutString("TargetAdress", Adress);
            editor.Apply();
        }
    }
}