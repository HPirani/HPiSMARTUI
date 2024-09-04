/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUi                                               **  
** Description:                                                                  **
**                                                                               **
**                                                                               **
** Created in sat 1403/02/25 6:40 PM By Hosein Pirani                            **
**                                                                               **
** Modified In Wed 1403/05/31 02:45 PM To  7:15 by me.                           **
** :                            Minor Fixes.                                     **
** TODO: Test All Methods.                                                       **
** TODO:                                                                         **
** ..                                                                            **
** ...                                                                           **
** And CODE                                                                      **
** ..... More Code                                                               **  
** ........ Code                                                                 **
** ...........  #_#                                                              **
** ...............                                                               **
 *********************************************************************************/



 






using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

using AndroidX.Core.Content;

namespace HPISMARTUI.Services
{
   // [Service] 
    [BroadcastReceiver(Name = "ir.hpi.hpismartui.SendStatusReceiver", Enabled = true, Exported = true /*,DirectBootAware = true*/)]
    [IntentFilter(new[] { "ir.hpi.hpismartui.message_sent_action" })]
    public class SendStatusReceiver : BroadcastReceiver
    {

    public static  String MESSAGE_SENT_ACTION =
            "ir.hpi.hpismartui.message_sent_action";

        // Defined by platform, but no constant provided. See docs for SmsManager.sendTextMessage.
        // "android.provider.Telephony.SMS_DELIVER"
        private static  String EXTRA_ERROR_CODE = "errorCode";
    private static  String EXTRA_NO_DEFAULT = "noDefault";


    public override void OnReceive(Context context, Intent intent)
    {
             int resultCode = GetResultExtras(true).GetInt(EXTRA_NO_DEFAULT);
            Log.Debug("OnReceive", intent.Action);

/*            var serviceIntent = new Intent(context, typeof(SmsManagerTestService));
            ContextCompat.StartForegroundService(context,serviceIntent);*/

/*            var persistIntent = new Intent(context, typeof(PersistentService));
            context.StartService(persistIntent);*/

            if ( intent.Action == MESSAGE_SENT_ACTION)
        {
                
                int errorCode = intent.GetIntExtra(EXTRA_ERROR_CODE, -1);
            bool userCancel = intent.GetBooleanExtra(EXTRA_NO_DEFAULT, false);
                Log.Debug("SendStatusReceiver", "ActionEqual But... Error is: {0} , bool is: {1}",errorCode,userCancel);
                if (userCancel)
            {
                Log.Debug("SendStatusReceiver", "SMS not sent, user cancelled.");
            } else
            {
                Log.Debug( "SMS result" ,  "error code: " + errorCode);
            }
        }
    }
}

}
