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

