﻿using Android.App;
using Android.OS;
using Android.Runtime;

using HPISMARTUI.ViewModel;

namespace HPISMARTUI
    {
    [Application]
    public class MainApplication : MauiApplication
        {
        public static readonly string ChannelId = "SMSTestServiceChannel";
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
            {
          
             
            }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
        public override void OnCreate()
        {
            base.OnCreate();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
#pragma warning disable CA1416
                var serviceChannel =
                    new NotificationChannel(ChannelId,
                        "SMSTestServiceChannel",
                    NotificationImportance.High);

                if (GetSystemService(NotificationService)
                    is NotificationManager manager)
                {
                    manager.CreateNotificationChannel(serviceChannel);
                }
#pragma warning restore CA1416
            }
        }

    }
    }