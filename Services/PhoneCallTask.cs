/********************************************************************************\\
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
**                   this code is part of HPiSMARTUi                             **
** Description:                                                                  **
** PhoneCall Task.                                                               **
**                                                                               **
** Created in mon 1403/04/025 12:40 PM By Hosein Pirani                          **
**                                                                               **
** Modified In Mon 1403/04/25 15:00 PM To 20:05 by me.                           **
** : initial                                                                     **
** TODO:                                                                         **
** TODO:                                                                         **
** ..                                                                            **
** ...                                                                           **
** And  LOT OF CODE @_@                                                          **
** .....                                                                         **  
** ........                                                                      **
** ...........                                                                   **
** ...............                                                               **
\\********************************************************************************/





using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidPlatform = Microsoft.Maui.ApplicationModel.Platform;
using Android.OS;
using Android.Telephony;
using HPISMARTUI.Abstractions;
using Android.Content;


namespace HPISMARTUI.Services
{
    public class PhoneCallTask : IPhoneCallTask
    {
        public string DefaultCountryIso{ get; set;} = "IR";
        public bool AutoDial { get; set; } = false;

        public PhoneCallTask()
        {
            
        }

#region IPhoneCallTask Members

        public bool CanMakePhoneCall
        {
            get
            {
                var packageManager = AndroidPlatform.AppContext.PackageManager;
                var dialIntent = ResolveDialIntent("0000000000");

                return null != dialIntent.ResolveActivity(packageManager);
            }
        }

        public void MakePhoneCall(string number, string name = null, string CountryISO = "IR", bool AutoDial = true)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("number");

            if (CanMakePhoneCall)
            {
                string phoneNumber = number;
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    phoneNumber = PhoneNumberUtils.FormatNumber(number,DefaultCountryIso);
                    
                } else
                {
                    if (!string.IsNullOrEmpty(DefaultCountryIso))
                        phoneNumber = PhoneNumberUtils.FormatNumber(number, DefaultCountryIso);
                }

                var dialIntent = ResolveDialIntent(phoneNumber);
               // StartNewActivity(dialIntent);
                //AndroidPlatform.AppContext.StartActivity(dialIntent);
                dialIntent.SetFlags(ActivityFlags.ClearTop);
                dialIntent.SetFlags(ActivityFlags.NewTask);

                Android.App.Application.Context.StartActivity(dialIntent);
            }
        }
#endregion



#region Methods

        private Intent ResolveDialIntent(string phoneNumber)
        {
            string dialIntent = AutoDial ? Intent.ActionCall : Intent.ActionDial;

           Android.Net.Uri telUri = Android.Net.Uri.Parse("tel:" + phoneNumber);
            return new Intent(dialIntent, telUri);
        }



        #endregion
    }
}

