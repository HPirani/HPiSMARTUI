/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUI                                               **  
** Description:                                                                  **
** SettingItems ViewModel For Adjust ECU Parameters.                                 **
**                                                                               **
** Created in sat 1403/03/026 15:40 PM By Hosein Pirani                          **
**                                                                               **
** Modified In sun 1403/03/26 16:00 PM To 20:05 by me.                           **
** :                                                                             **
** TODO: Complete Bindings.                                                      **
** TODO:  Create Event To Command Behavior For Writing SettingItems To ECU!!!        **
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

namespace HPISMARTUI.ViewModel
{
  //  [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class SettingsViewModel : ObservableObject
    {



        
        public ISettingsService _SettingsService { get; set; }
        


      
        //Xaml Bindings.
        #region XamlStatics
        //DisplayName And Descriptions.
        public  string MinServoAngleName => "Minimum Servo Angle";
        public  string MinServoDescription => "Set Minimum Angle Of IDle Adjuster's Servo Angle.";
        public  string MaxServoAngleName => "Maximum Servo Angle";
        public string MaxServoDescription => "Set Maximum Angle Of IDle Adjuster's Servo Angle.";
        public string MinIdlerpmName => "Minimum ENGINE Idle RPM";
        public string MinIdlerpmDescription => "Minimum RPM in Normal State.Servo Angle Referenced From This SetValue.";
        public string BlinkersIntervalName => "Blinkers Delay";
        public string BlinkersIntervalDescription => "Set Blinkers Toggle Frequency in MilliSeconds.";
        public string HeadBlinkIntervalName => "HeadLight Blink Delay";
        public string HeadBlinkIntervalDescription => "Set HeadLight Blinker's Toggle Frequency in MilliSeconds.";
        public string CurrentHornModeName => "Default Active Horn";
        public string CurrentHornModeDescription => "Select Default Mode For Normal Horn Mode.";
        public string HornKeyDebounceName => "Horn Toggle Debounce Time";
        public string HornDebounceDescription => "Set Software Debounce Time For Handling Horn Key. Don't Change it until Horn mode Toggles Wrongly.";
        //Minimum & Maximum Values.
        public  int MinimumAllowedMinServoAngle => 0;
        public int MaximumAllowedMinServoAngle => 359;
        public int MinimumAllowedMaxServoAngle => 1;
        public int MaximumAllowedMaxServoAngle => 360;
        public int MinimumAllowedMinIdleRPM => 1000;
        public int MaximumAllowedMinIdleRPM => 2500;
        public int MinimumAllowedBlinkersInterval => 30;
        public int MaximumAllowedBlinkersInterval => 2000;
        public int MinimumAllowedHeadBlinkInterval => 30;
        public int MaximumAllowedHeadBlinkInterval => 2000;
        public int MinimumAllowedHornDebounceTime => 10;
        public int MaximumAllowedHornDebounceTime => 5000;
        //

#endregion
        //Values.
        [ObservableProperty]
        
        private string currentHornMode;//Picker SetValue. 
        [ObservableProperty]
      //  [NotifyPropertyChangedFor(nameof(_SettingsService))]
        private int minServoAngle;//Minimum Servo Angle.
        [ObservableProperty]
      //  [NotifyPropertyChangedFor(nameof(_SettingsService))]
        private int maxServoAngle;//Maximum Servo Angle.
        [ObservableProperty]
      //  [NotifyPropertyChangedFor(nameof(_SettingsService))]
        private int minIdleRPM;//Minimum idleRPM.
        [ObservableProperty]
       // [NotifyPropertyChangedFor(nameof(_SettingsService))]
        private int blinkersInterval;//Blinkers Interval.
        [ObservableProperty]
       // [NotifyPropertyChangedFor(nameof(_SettingsService))]
        private int headBlinkInterval;//HeadLight Blinker Interval.
        [ObservableProperty]
       // [NotifyPropertyChangedFor(nameof(_SettingsService))]
        private int hornDebounceDelay;//Software Debounce For Horn Key.




      //  [ObservableProperty]
     //   private ObservableCollection<SettingItems> selectedItems;
        public SettingsViewModel(ISettingsService settingsService)
        {
           
            _SettingsService = settingsService;
            MinServoAngle = _SettingsService.MinimumServoAngle;
            MaxServoAngle = _SettingsService.MaximumServoAngle;
            BlinkersInterval = _SettingsService.BlinkersInterval;
            HeadBlinkInterval = _SettingsService.HeadBlinkInterval;
            HornDebounceDelay = _SettingsService.HornKeyDebounceDelay;
            //Load Default or Pre-Defined Horn Mode
            CurrentHornMode = _SettingsService.CurrentHornMode switch
              {
                 0 => "Normal" ,
                 1 => "OneTwo",
                 2 => "Wedding",
                 _ => "Select One"
              };
        }

        [RelayCommand]
     public void UpdateSettings()
        {
           
                

                    _SettingsService.MinimumServoAngle = MinServoAngle;
                    _SettingsService.MaximumServoAngle = MaxServoAngle;
                    _SettingsService.BlinkersInterval = BlinkersInterval;
                    _SettingsService.HeadBlinkInterval = HeadBlinkInterval;
                    _SettingsService.HornKeyDebounceDelay = HornDebounceDelay;
                    _SettingsService.CurrentHornMode = CurrentHornMode switch
                    {
                        "Normal" => 0,
                        "OneTwo" => 1,
                        "Wedding" => 2,
                        _ => 0
                    };
                
            
        } 









        static async void DisplayMessage(string message, string title = "Note",  string ok = "OK")
        {
            await Shell.Current.DisplayAlert(title,message,ok);
        }

       /* protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
        {

        }*/

    }
}
