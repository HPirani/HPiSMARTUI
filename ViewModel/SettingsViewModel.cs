/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUI                                               **  
** Description:                                                                  **
** SettingItems ViewModel                                                        **
**                                                                               **
** Created in sat 1403/03/026 15:40 PM By Hosein Pirani                          **
**                                                                               **
** Modified In wed 1403/05/17 16:00 PM To 20:05 by me.                           **
** :                                                                             **
** TODO: Complete Bindings.                                                      **
** TODO:  Create Event To Command Behavior For Writing SettingItems To ECU!!!    **
** TODO: Add Validator.                                                          **
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
using HPISMARTUI.Model;
using HPISMARTUI.Services;
using HPISMARTUI;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HPISMARTUI.Templates;
using Android.Util;

namespace HPISMARTUI.ViewModel
{
  //  [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class SettingsViewModel : ObservableObject
    {  
        public ISettingsService _SettingsService { get; set; }
        
        //Xaml Bindings.
#region XamlBindings
        //DisplayName And Descriptions.
        public string UI_MinServoAngleName => "Minimum Servo Angle";
        public  string UI_MinServoDescription => "Set Minimum Angle Of IDle Adjuster's Servo Angle.";
        public  string UI_MaxServoAngleName => "Maximum Servo Angle";
        public string UI_MaxServoDescription => "Set Maximum Angle Of IDle Adjuster's Servo Angle.";
        public string UI_MinIdlerpmName => "Minimum ENGINE Idle RPM";
        public string UI_MinIdlerpmDescription => "Minimum RPM in Normal State.Servo Angle Referenced From This SetValue.";
        public string UI_BlinkersIntervalName => "Blinkers Delay";
        public string UI_BlinkersIntervalDescription => "Set Blinkers Toggle Frequency in MilliSeconds.";
        public string UI_HeadBlinkIntervalName => "HeadLight Blink Delay";
        public string UI_HeadBlinkIntervalDescription => "Set HeadLight Blinker's Toggle Frequency in MilliSeconds.";
        public string UI_CurrentHornModeName => "Default Active Horn";
        public string UI_CurrentHornModeDescription => "Select Default Mode For Normal Horn Mode.";
        public string UI_HornKeyDebounceName => "Horn Toggle Debounce Time";
        public string UI_HornDebounceDescription => "Set Software Debounce Time For Handling Horn Key. Don't Change it until Horn mode Toggles Wrongly.";
        public string UI_rpmReadIntervalName => "Engine RPM Reading Interval";
        public string UI_rpmReadIntervalDescription => "Set Interval Of Engine's RPM Reading(Referesh Rate) in MilliSeconds.";
        //App
        public string UI_GPSUpdateIntervalName => "GPS Update Interval";
        public string UI_GPSUpdateIntervalDescription => "Set the Bike's Speed and Acceleration Update Interval in MilliSeconds.";
        public string UI_GPSLocationAccuracyName => "GPS Location Accuracy";
        public string UI_GPSLocationAccuracyDescription => "Select GPS Data Accuracy.";
        public string UI_GPSLocationRequestIntervalName => "GPS LocationRequest Interval";
        public string UI_GPSLocationRequestIntervalDescription => "Set GPSLocation Request Interval in MilliSeconds.Bellow 1 Second May Not Work.";
        public string UI_TimerResetIntervalName => "GPS Speed ResetTimer Interval";
        public string UI_TimerResetIntervalDescription => "Set InterVal Of Reset(DisplaySpeed) Timer When No LocationUpdate Were Comed. etc when Bike Stops or weak GPS Signal. ";
        //Minimum & Maximum Values.
        //ECU
        public int Ux_MinimumAllowedMinServoAngle => 0;
        public int Ux_MaximumAllowedMinServoAngle => 359;
        public int Ux_MinimumAllowedMaxServoAngle => 1;
        public int Ux_MaximumAllowedMaxServoAngle => 360;
        public int Ux_MinimumAllowedMinIdleRPM => 1000;
        public int Ux_MaximumAllowedMinIdleRPM => 2500;
        public int Ux_MinimumAllowedBlinkersInterval => 30;
        public int Ux_MaximumAllowedBlinkersInterval => 2000;
        public int Ux_MinimumAllowedHeadBlinkInterval => 30;
        public int Ux_MaximumAllowedHeadBlinkInterval => 2000;
        public int Ux_MinimumAllowedHornDebounceTime => 10;
        public int Ux_MaximumAllowedHornDebounceTime => 5000;
        public int Ux_MinimumAllowedRpmReadInterval => 10;
        public int Ux_MaximumAllowedRpmReadInterval => 2000;
        //App
        public int Ux_MinimumAllowedGPSInterval => 100;
        public int Ux_MaximumAllowedGPSInterval => 60000;
        public int Ux_MinimumAllowedGPSRequestInterval => 100;
        public int Ux_MaximumAllowedGPSRequestInterval => 5000;
        public int Ux_MinimumAllowedTimerReset => 1;
        public int Ux_MaximumAllowedTimerReset => 60;

        //Values.
        [ObservableProperty]
        private int currentHornMode;//Picker SetValue. 
        [ObservableProperty]
        private int minServoAngle;//Minimum Servo Angle.
        [ObservableProperty]
        private int maxServoAngle;//Maximum Servo Angle.
        [ObservableProperty]
        private int minIdleRPM;//Minimum idleRPM.
        [ObservableProperty]
        private int blinkersInterval;//Blinkers Interval.
        [ObservableProperty]
        private int headBlinkInterval;//HeadLight Blinker Interval.
        [ObservableProperty]
        private int hornDebounceDelay;//Software Debounce For Horn Key.
        [ObservableProperty]
        private int rpmReadInterval;
        ///////////
        //App
        [ObservableProperty]
        private int gPSUpdateInterval;
        [ObservableProperty]
        private int gpsLocationAccuracy;
        [ObservableProperty]
        private int gpsRequestInterval;
        [ObservableProperty]
        private int timerResetInterval;


#endregion
   
        public SettingsViewModel(ISettingsService settingsService)
        {
           
            _SettingsService = settingsService;
            //ECU
            MinServoAngle     = _SettingsService.MinimumServoAngle;
            MaxServoAngle     = _SettingsService.MaximumServoAngle;
            BlinkersInterval  = _SettingsService.BlinkersInterval;
            HeadBlinkInterval = _SettingsService.HeadBlinkInterval;
            HornDebounceDelay = _SettingsService.HornKeyDebounceDelay;
            RpmReadInterval   = _SettingsService.RPMreadingInterval;
            //Load Default or Pre-Defined Horn Mode
            CurrentHornMode = _SettingsService.CurrentHornMode;
            /*CurrentHornMode   = _SettingsService.CurrentHornMode switch //TODO: FIX Me.!!!!
              {
                 0 => "0" ,
                 1 => "1",
                 2 => "2",
                 _ => "0"
              };*/
            //App
            GPSUpdateInterval   = _SettingsService.GPSUpdateInterval;
            GpsLocationAccuracy = _SettingsService.GPSLocationAccuracy;
            GpsRequestInterval  = _SettingsService.GPSLocationRequestInterval; 
            TimerResetInterval  = _SettingsService.TimerResetInterval;
        }


        [RelayCommand]
     public void UpdateSettings()
        {
           

            //ECU
            Log.Debug("UpdateSettings", $"CurrentHornMode: {CurrentHornMode}");
                    _SettingsService.MinimumServoAngle    = MinServoAngle;
                    _SettingsService.MaximumServoAngle    = MaxServoAngle;
                    _SettingsService.BlinkersInterval     = BlinkersInterval;
                    _SettingsService.HeadBlinkInterval    = HeadBlinkInterval;
                    _SettingsService.HornKeyDebounceDelay = HornDebounceDelay;
                    _SettingsService.RPMreadingInterval   = RpmReadInterval;
                    _SettingsService.CurrentHornMode      = CurrentHornMode;
                    /*_SettingsService.CurrentHornMode    = CurrentHornMode switch//TODO: FIX Me.!!!!
                    {
                        "0" => 0,
                        "1" => 1,
                        "2" => 2,
                        _ => 0
                    };*/
            //App
                _SettingsService.GPSUpdateInterval          = GPSUpdateInterval;
                _SettingsService.GPSLocationAccuracy        = GpsLocationAccuracy;
                _SettingsService.GPSLocationRequestInterval = GpsRequestInterval;
                _SettingsService.TimerResetInterval         = TimerResetInterval;
        } 









        static async void DisplayMessage(string message, string title = "Settings",  string ok = "OK")
        {
            await Shell.Current.DisplayAlert(title,message,ok);
        }

       /* protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
        {

        }*/

    }
}
