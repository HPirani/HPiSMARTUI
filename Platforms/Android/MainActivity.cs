﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Telephony;
using Android.Text;
using Android.Util;

using HPISMARTUI.Services;
using Android.Widget;
using Android;
using Android.Runtime;
using Microsoft.Maui.Platform;
using Kotlin;
using HPISMARTUI.ViewModel;
using AndroidX.Core.Content;
using Android.Views;
//using var vm =Microsoft.Maui. ;
//using Android.Telephony.Ims;
//using Android.Telephony.Gsm;
namespace HPISMARTUI
{
    [Activity( Theme = "@style/MyTheme", MainLauncher = true, ResizeableActivity = true,ScreenOrientation = ScreenOrientation.Landscape,TurnScreenOn = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : MauiAppCompatActivity
    {
        SendStatusReceiver receiver;
        BroadcastReceiver boot_broadcastReceiver;
        //BootReceiver bootReceiver;
        public MainActivity()
        {
            AndroidServiceManager.MainActivity = this;
            //StartService();
            //  StartForegroundService(typeof (SmsManagerTestService));

        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            receiver = new SendStatusReceiver();
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            //this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            DeviceDisplay.Current.KeepScreenOn = true;
            //  DeviceDisplay.Current.
            // Code omitted for clarity
            boot_broadcastReceiver = new BootReceiver();

            
        }

        protected override void OnResume()
        {
            base.OnResume();
            RegisterReceiver(receiver, new IntentFilter("ir.hpi.hpismartui.message_sent_action"));
            // Code omitted for clarity
            RegisterReceiver(boot_broadcastReceiver, new IntentFilter(Intent.ActionBootCompleted));
        }

        protected override void OnPause()
        {
            UnregisterReceiver(receiver);
            // Code omitted for clarity
            base.OnPause();
        }
    

    //
    // MainViewModel mainViewModel;
    public void StartService()
        {


            var serviceIntent = new Intent(BaseContext,typeof(SmsManagerTestService));
            ContextCompat.StartForegroundService(BaseContext, serviceIntent);
        }

        public void StopService()
        {
            var serviceIntent = new Intent(this, typeof(SmsManagerTestService));
            StopService(serviceIntent);
            
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            // Handle the intent that you received
            ProcessIntent(intent);
            
        }
        private void ProcessIntent(Intent intent)
        {
            // Extract data from the intent and use it
            // For example, you can check for a specific action or extract extras
            if (intent != null)
            {
                // Example: checking for a specific action
                var action = intent.Action;
                if (action == "USER_TAPPED_NOTIFIACTION")
                {
                    // Handle the specific action
                    Log.Debug("MainActivity", "Stopping Service...");
                    StopService();
                }
            }
        }



    }




}
