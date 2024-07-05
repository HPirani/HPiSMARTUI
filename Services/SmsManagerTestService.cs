using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Provider;
using Android.Systems;
using Android.Util;
using HPISMARTUI;
using HPISMARTUI.Services;
using Java.Lang;
//using Android.App.IntentService;
//using Android.App.PendingIntent;
using Android.Content;
using Android.Telephony;
using Thread = System.Threading.Thread;
using Object = Java.Lang.Object;
using Android.Test;
using System.Diagnostics.CodeAnalysis;
using Android.Runtime;
using System.Reflection;
using HPISMARTUI.View;
using Java.Util.Concurrent;
using System.Reactive;
using AndroidX.Core.App;




namespace HPISMARTUI.Services
{
    [Service]
    public class SmsManagerTestService : IntentService
    {
        Timer timer = null;
        int myId = (new object()).GetHashCode();
        int BadgeNumber = 0;
       // private readonly IBinder binder = new LocalBinder();
        NotificationCompat.Builder notification;

        private readonly IBinder binder = new LocalBinder();

        public class LocalBinder : Android.OS.Binder
        {
            public SmsManagerTestService GetService()
            {
                return this.GetService();
            }
        }

        /*async*/ void Timer_Elapsed(object state)
        {
            AndroidServiceManager.IsRunning = true;

            //await EnsureHubConnection();
        }

        public override IBinder OnBind(Intent intent)
        {
            return binder;
        }


        public override StartCommandResult OnStartCommand(Intent intent,
    StartCommandFlags flags, int startId)
        {
            Log.Debug("OnStartCommand", " Called");

            var input = intent.GetStringExtra("inputExtra");

            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction("USER_TAPPED_NOTIFIACTION");

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent,
                PendingIntentFlags.Immutable);

            // Increment the BadgeNumber
            BadgeNumber++;

            notification = new NotificationCompat.Builder(this,
                    MainApplication.ChannelId)
                .SetContentText(input)
                .SetSmallIcon(Resource.Drawable.bg_a)
                .SetAutoCancel(false)
                .SetContentTitle("Service Running")
                .SetPriority(NotificationCompat.PriorityDefault)
                .SetContentIntent(pendingIntent);

            notification.SetNumber(BadgeNumber);



            // build and notify
            StartForeground(myId, notification.Build());

            // timer to ensure hub connection
            timer = new Timer(Timer_Elapsed, notification, 0, 10000);

            // You can stop the service from inside the service by calling StopSelf();

            return StartCommandResult.Sticky;
        }

        private static  string LOG_TAG = "smsmanagertestservice";

        private  class SendSmsJob : AsyncTask<Intent, Java.Lang.Void, Java.Lang.Void>
        {
            protected override Java.Lang.Void RunInBackground(params Intent[] @params)
            {
                DoInBackground(this);
                Log.Debug("RunInBackground", "Run in bg Called");
                return null;
            }


            protected override Object DoInBackground(Object[] intents)
    {
                Log.Debug("DoInBackground", " bg Called...");
                Intent intent = (Intent)intents;
        try
        {
                    Log.Debug("DoInBackground", "Sleeping...");
                    Thread.Sleep(5000);
        }
        catch (InterruptedException e)
        {
                    // testing
                    Log.Error("DoInBG...",e.Message);
        }

        string text = intent.GetStringExtra(EXTRA_SEND_TEXT);
        string phoneNumber = intent.GetStringExtra(EXTRA_SEND_NUMBER);
#pragma warning disable CA1422 // Validate platform compatibility
                PendingIntent sendIntent = (PendingIntent)intent.GetParcelableExtra(EXTRA_SEND_INTENT);
#pragma warning restore CA1422 // Validate platform compatibility
                sendSms(phoneNumber, text, sendIntent);
                Log.Debug("DoInBackground", " SendingSMS...");
                return null;
    }

    
        protected override void OnPostExecute(Java.Lang.Void aVoid)
    {
        Log.Debug(LOG_TAG, "SMS sent");
    }


        }

//public static System.String SEND_SMS = "com.android.phone.testapps.smsmanagertestapp.send_sms";
public static string EXTRA_SEND_TEXT = new("text");
public static string EXTRA_SEND_NUMBER = new("number");
public static string EXTRA_SEND_INTENT = new("sendIntent");

public SmsManagerTestService()
{
   //         ("SmsManagerTestService")
}



    protected override void OnHandleIntent(Intent intent)
{
            Log.Debug("OnHandleIntent", " intent.Action");
            switch (intent.Action)
    {
        case "ir.hpi.hpismartui.send_sms":
            {
                new SendSmsJob().Execute(intent);
                break;
            }
    }
}

private static void sendSms(string phoneNumber, string text, PendingIntent sendIntent)
{
#pragma warning disable CA1422 // Validate platform compatibility
            SmsManager m = SmsManager.Default;
#pragma warning restore CA1422 // Validate platform compatibility
            m.SendTextMessage(phoneNumber, null, text, sendIntent, null);
            Log.Debug("sendSmsInManagerService", " SendSerialData.");
        }
}

}
