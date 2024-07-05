/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUI.                                              **  
** Description:                                                                  **
** SettingItems Service                                                             **
**                                                                               **
** Created in sat 1403/03/026 14:40 PM By Hosein Pirani                          **
**                                                                               **
** Modified In sun 1403/03/28 20:00 PM To 21:30 by me.                           **
** : Write To ECU Added                                                          **
** TODO: Complete and test Methods.                                             **
** TODO: Get Default SettingItems From ECU.                                          **
** ..                                                                            **
** ...                                                                           **
** And  LOT OF CODE @_@                                                          **
** .....                                                                         **  
** ........                                                                      **
** ...........                                                                   **
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

namespace HPISMARTUI.Services
{

    public class SettingsService : ISettingsService
    {
        private const int DefaultMinServoAngle = 0;
        private const int DefaultMaxServoAngle = 30;
        private const int DefaultMinIdleRPM = 150;
        private const int DefaultBlinkersInterval = 250;
        private const int DefaultHeadBlinkFrequency = 150;
        private const int DefaultHornMode = 0;
        private const int DefaultHornDebounceDelay = 200;


        // All get{}; Should Retrived From ECU!.
        public int MinimumServoAngle
        {
            get => /*DefaultMinServoAngle;*/ Preferences.Get(nameof(MinimumServoAngle), DefaultMinServoAngle);
            set
            {
                Log.Debug("MinimumServoAngle", "writing");
                Preferences.Set(nameof(MinimumServoAngle), value);
                WriteSettingsToECU(Serial_OutCommands.OutSerial_SetMinServoAngle_cmd, value);
            }
        }

        public int MaximumServoAngle
        {
            get =>/*DefaultMaxServoAngle;*/ Preferences.Get(nameof(MaximumServoAngle), DefaultMaxServoAngle);
            set
            {
                Log.Debug("MaximumServoAngle", "writing");
                Preferences.Set(nameof(MaximumServoAngle), value);
                WriteSettingsToECU(Serial_OutCommands.OutSerial_SetMaxServoAngle_cmd, value);
            }
        }
        public int MinIdleRPM
        {
            get => /*DefaultMinIdleRPM;*/ Preferences.Get(nameof(MinIdleRPM), DefaultMinIdleRPM);
            set
            { 
                Preferences.Set(nameof(MinIdleRPM), value);
                Log.Debug("MinIdleRPM", "writing");
                WriteSettingsToECU(Serial_OutCommands.OutSerial_SetMinIdleRPM_cmd, value);
            }
        }

        public int BlinkersInterval
        {
            get => /*DefaultBlinkersInterval;*/ Preferences.Get(nameof(BlinkersInterval), DefaultBlinkersInterval);
            set 
            {
                Log.Debug("BlinkersInterval", "writing");
                Preferences.Set(nameof(BlinkersInterval), value);
                  WriteSettingsToECU(Serial_OutCommands.OutSerial_SetBlinkInterval_cmd, value); 
            }
        }
        
        public int HeadBlinkInterval
        {
            get => /*DefaultHeadBlinkFrequency;*/ Preferences.Get(nameof(HeadBlinkInterval), DefaultHeadBlinkFrequency); 
            set  
            {
                Preferences.Set(nameof(HeadBlinkInterval), value);
                WriteSettingsToECU(Serial_OutCommands.OutSerial_SetHeadBlinkFreq_cmd, value);
            }
        }

        public int CurrentHornMode
        {
            get => /*DefaultHornMode;*/ Preferences.Get(nameof(CurrentHornMode),DefaultHornMode);
            set
            { 
                Preferences.Set(nameof(CurrentHornMode), value);
                WriteSettingsToECU(Serial_OutCommands.OutSerial_SetHornMode_cmd, value); 
            }
        }
        public int HornKeyDebounceDelay
        {
            get => /*DefaultHornDebounceDelay;*/ Preferences.Get(nameof(HornKeyDebounceDelay),DefaultHornDebounceDelay); 
            set {
                Preferences.Set(nameof(HornKeyDebounceDelay), value);
                  WriteSettingsToECU(Serial_OutCommands.OutSerial_SetHornKeyDebounceDelay_cmd, value);
            }
        }

        private  void WriteSettingsToECU(string key, int value)
        {
            


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(key);
            stringBuilder.Append(value);

             Task.Delay(TimeSpan.FromMilliseconds(10));
            WeakReferenceMessenger.Default.Send(new Messages.WriteSettingToECUMessage(stringBuilder.ToString()));

        }

    }
}
