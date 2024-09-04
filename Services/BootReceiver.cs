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
using Android.Util;
using Android.Widget;

namespace HPISMARTUI.Services
{
    [BroadcastReceiver(Label = "BootReceiver", DirectBootAware = true, Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted }, Priority = (int)IntentFilterPriority.HighPriority)]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Shell.Current.DisplayAlert("BootReceiver","Received","OK");
            var launch_intent = Platform.CurrentActivity?.PackageManager?.GetLaunchIntentForPackage(Platform.CurrentActivity.PackageName);
            if (launch_intent != null)
            {
                Log.Debug("BootReceiver", "Starting App");
                launch_intent.AddFlags(ActivityFlags.ReorderToFront);
                launch_intent.AddFlags(ActivityFlags.NewTask);
                launch_intent.AddFlags(ActivityFlags.ResetTaskIfNeeded);
                Platform.CurrentActivity?.StartActivity(launch_intent);
            }
        }

    }
}
