using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.OS;
using Android.Content;
using Android.Net;
using Android.Support;
using Android.Telecom;
using Android.Text;
using Java.Lang;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Android.Runtime;
using static Android.Telecom.Call;
using Java.Util;
using static HPISMARTUI.Services.AndroidCallManager;
using System.Reflection.Metadata;
using AndroidX.Core.Telephony;
using Android.Telephony;
using Android.Provider;
using Android.Media.Metrics;
using System.Reflection.Emit;
using Android.Util;
using Android.Nfc;
using SkiaSharp;
using Uri = Android.Net.Uri;
using Android.Accounts;
using Android.Bluetooth;
using static HPISMARTUI.Services.SmsManagerTestService;


namespace HPISMARTUI.Services
{
    internal class AndroidCallManager
    {
       public Call telecomCall;
        public Callback _callback;
        private static TelecomManager _instance;
        private InCallService _inCallService;
        Context context;
        

        public enum DialerCallState
        {

            BLOCKED,
            DISCONNECTED,
            CONFERENCED
        };

        public AndroidCallManager(Call call, TelecomManager instance)

        {
            telecomCall = call;
            _instance = instance;
            telecomCall.RegisterCallback(_callback);



        }



   
    }
}

