/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUI.                                              **  
** Description:                                                                  **
** SettingItems Service                                                          **
**                                                                               **
** Created in sat 1403/03/026  2:40 PM By Hosein Pirani                          **
**                                                                               **
** Modified In wed 1403/05/17  6:00 PM To  7:30 PM by me.                        **
** : GPS Options  Added                                                          **
** TODO: Complete and test Methods.                                              **
** TODO: Get Default SettingItems From ECU.                                      **
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

using Android.Util;

using CommunityToolkit.Mvvm.Messaging;

using HPISMARTUI.Model;

using Java.Lang;

using StringBuilder = System.Text.StringBuilder;

namespace HPISMARTUI.Services
{

    public class SettingsService : ISettingsService
    {
        //ECU Settings.
        private const int DefaultMinServoAngle = 0;
        private const int DefaultMaxServoAngle = 30;
        private const int DefaultMinIdleRPM = 150;
        private const int DefaultBlinkersInterval = 250;
        private const int DefaultHeadBlinkFrequency = 150;
        private const int DefaultHornMode = 0;
        private const int DefaultHornDebounceDelay = 200;
        private const int DefaultRpmReadingInterval = 500;
        //App Settings.
        private const double DefaultTrip = 0.0d;
        private const int DefaultGPSUpdateInterval = 250;
        private const int DefaultGPSLocationAccuracy = 0;
        private const int DefaultGPSLocationRequestInterval = 500;
        private const bool DefaultECU_waked = false;
        private const int DefaultTimerResetInterval = 3;
        //App
        public double Trip
        {
            get => Preferences.Get(nameof(Trip), DefaultTrip);
            set
            {
                Log.Debug(nameof(Trip), $"Writing {value} .");
                Preferences.Set(nameof(Trip), value);
            }
        }
        public int TimerResetInterval
        {
            get => Preferences.Get(nameof(TimerResetInterval), DefaultTimerResetInterval);
            set
            {
                Log.Debug(nameof(TimerResetInterval), $"Writing {value} .");
                Preferences.Set(nameof(TimerResetInterval), value);
            }
        }
        public int GPSUpdateInterval
        {
            get => Preferences.Get(nameof(GPSUpdateInterval), DefaultGPSUpdateInterval);
            set
            {
                Log.Debug(nameof(GPSUpdateInterval), $"writing {value} .");
                Preferences.Set(nameof(GPSUpdateInterval), value);
            }
        }
        public int GPSLocationAccuracy
        {
            get => Preferences.Get(nameof(GPSLocationAccuracy), DefaultGPSLocationAccuracy);

            set
            {
                Log.Debug(nameof(GPSLocationAccuracy), $"writing {value} .");
                Preferences.Set(nameof(GPSLocationAccuracy), value);
            }
        }
        public int GPSLocationRequestInterval
        {
            get => Preferences.Get(nameof(GPSLocationRequestInterval), DefaultGPSLocationRequestInterval);
            set
            {
                Log.Debug(nameof(GPSLocationRequestInterval), $"writing {value} .");
                Preferences.Set(nameof(GPSLocationRequestInterval), value);
            }
        }
        public bool IS_ECU_ALive
        {
            get
            {
                // Get ECU Status.
                Task.Run(()=>WeakReferenceMessenger.Default.Send(new Messages.WriteSettingToECUMessage(Serial_OutCommands.OutSerial_AliveReport_cmd)));
                //Wait 500Milliseconds For ECU Response.
                Task.Delay(TimeSpan.FromMilliseconds(500));
                return Preferences.Get(nameof(IS_ECU_ALive), DefaultECU_waked);   
            }
            set
            {
             Preferences.Set(nameof(IS_ECU_ALive), value);
            }
        }
        /*******************************************************************************************************/
        //ECU
        // All get{}; Should Retrived From ECU!.
        public int MinimumServoAngle
        {
            get => /*DefaultMinServoAngle;*/ Preferences.Get(nameof(MinimumServoAngle), DefaultMinServoAngle);
            set
            {
                Log.Debug(nameof(MinimumServoAngle), $"writing {value} .");
                Preferences.Set(nameof(MinimumServoAngle), value);
                WriteSettingsToECU(Serial_OutCommands.OutSerial_SetMinServoAngle_cmd, value);
            }
        }

        public int MaximumServoAngle
        {
            get =>/*DefaultMaxServoAngle;*/ Preferences.Get(nameof(MaximumServoAngle), DefaultMaxServoAngle);
            set
            {
                Log.Debug(nameof(MaximumServoAngle), $"writing {value} .");
                Preferences.Set(nameof(MaximumServoAngle), value);
                WriteSettingsToECU(Serial_OutCommands.OutSerial_SetMaxServoAngle_cmd, value);
            }
        }
        public int MinIdleRPM
        {
            get => /*DefaultMinIdleRPM;*/ Preferences.Get(nameof(MinIdleRPM), DefaultMinIdleRPM);
            set
            {
                Log.Debug(nameof(MinIdleRPM), $"writing {value} .");
                Preferences.Set(nameof(MinIdleRPM), value);
                WriteSettingsToECU(Serial_OutCommands.OutSerial_SetMinIdleRPM_cmd, value);
            }
        }

        public int RPMreadingInterval
        {
            get => Preferences.Get(nameof(RPMreadingInterval), DefaultRpmReadingInterval);
            set
            {
                Log.Debug(nameof(RPMreadingInterval), $"Writing {value} .");
                Preferences.Set(nameof(RPMreadingInterval), value);
                WriteSettingsToECU(Serial_OutCommands.OutSerial_SetrpmReadInterval_cmd, value);
            }
        }

        public int BlinkersInterval
        {
            get => /*DefaultBlinkersInterval;*/ Preferences.Get(nameof(BlinkersInterval), DefaultBlinkersInterval);
            set 
            {
                Log.Debug(nameof(BlinkersInterval), $"writing {value} .");
                Preferences.Set(nameof(BlinkersInterval), value);
                  WriteSettingsToECU(Serial_OutCommands.OutSerial_SetBlinkInterval_cmd, value); 
            }
        }
        
        public int HeadBlinkInterval
        {
            get => /*DefaultHeadBlinkFrequency;*/ Preferences.Get(nameof(HeadBlinkInterval), DefaultHeadBlinkFrequency); 
            set  
            {
                Log.Debug(nameof(HeadBlinkInterval), $"writing {value} .");
                Preferences.Set(nameof(HeadBlinkInterval), value);
                WriteSettingsToECU(Serial_OutCommands.OutSerial_SetHeadBlinkFreq_cmd, value);
            }
        }

        public int CurrentHornMode
        {
            get => /*DefaultHornMode;*/ Preferences.Get(nameof(CurrentHornMode),DefaultHornMode);
            set
            {
                Log.Debug(nameof(CurrentHornMode), $"writing {value} .");
                Preferences.Set(nameof(CurrentHornMode), value);
                WriteSettingsToECU(Serial_OutCommands.OutSerial_SetHornMode_cmd, value); 
            }
        }
        public int HornKeyDebounceDelay
        {
            get => /*DefaultHornDebounceDelay;*/ Preferences.Get(nameof(HornKeyDebounceDelay),DefaultHornDebounceDelay);
            set
            {
                Log.Debug(nameof(HornKeyDebounceDelay), $"writing {value} .");
                Preferences.Set(nameof(HornKeyDebounceDelay), value);
                WriteSettingsToECU(Serial_OutCommands.OutSerial_SetHornKeyDebounceDelay_cmd, value);
            }
        }

        private async  void WriteSettingsToECU(string key, int value)
        {
            


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(key);
            stringBuilder.Append(value);

            await Task.WhenAll( 
                  Task.Delay(TimeSpan.FromMilliseconds(10)),
                  Task.Run(()=> WeakReferenceMessenger.Default.Send(new Messages.WriteSettingToECUMessage(stringBuilder.ToString()) ) ) );

        }

    }
}
