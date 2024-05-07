/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiUIpro.                                                **  
** Description:                                                                  **
** ENGINE States Retrived From Serial                                            **
**                                                                               **
** Created in sat 1403/01/25 17:20 PM By Hosein Pirani                           **
**                                                                               **
** Modified In sun 1402/06/05 12:00 PM To 19:05 by hosein pirani                 **
**                                                                               **
**                                                                               **
** TODO:Complete the Implementation                                              **
**                                                                               **
**                                                                               **
** And  LOT OF CODE @_@                                                          **
** ...                                                                           **  
**                                                                               **
**                                                                               **
**                                                                               **
 *********************************************************************************/


using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.OS;
using Android.Provider;
using Android.Util;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Google.Android.Material.Internal;
using GoogleGson;


using HPISMARTUI.Model;
using HPISMARTUI.Messages;
using Java.Lang;

using Microsoft.Maui.Controls.PlatformConfiguration;



namespace HPISMARTUI.Model
{
    
    public partial class ENGINEstate : ObservableObject
    {
      
        
        [ObservableProperty]
        private int temergencyInterval = 15;
        

        public ENGINEstate()
        {
            WeakReferenceMessenger.Default
 .Register<ENGINEstate, Messages.EngineState_HeadLightMessage>(this,
  (recipient, message) =>
 {


         recipient.IsHeadLight_Enabled = message.Value;
     Log.Debug("Messenger", "Received HeadlightMessage!");
     
 });


        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SmallLightColor))]
        private bool _isSmallLight_Enabled;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeadLightColor))]
        public bool isHeadLight_Enabled;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BlinkersColor))]
        public bool isLeftTurn_Enabled;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BlinkersColor))]
        public bool isRightTurn_Enabled;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BlinkersColor))]
        public bool isMultiblink_Enabled;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BlinkersColor))]
        public bool isBlinkDance_Enabled;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BlinkersColor))]
        public bool isHeadBlink_Enabled;

        [ObservableProperty]
        public bool isENGINE_ON;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PoliceLightColor))]
        public bool isPOliceLight_Enabled;

        [ObservableProperty]
        public bool isMeSirenSource_Enabled;
        //
        [ObservableProperty]
        public bool isShake_Detected;

        [ObservableProperty]
        public bool is_Silenced;//should reset the "ShakeDetected".


       
        public Color SmallLightColor
        {
            get
            {
                Log.Debug("SmallLightColor", "Invoked");
                return IsSmallLight_Enabled ? Colors.Blue : Colors.DarkBlue;
            }
        }
        public Color HeadLightColor
        {
            get
            {
                return IsSmallLight_Enabled ? Colors.Blue : Colors.DarkBlue;
            }
        }
        public Color BlinkersColor
        {
            get
            {
                return IsLeftTurn_Enabled ? Colors.Blue : Colors.DarkBlue;
            }
        }
        public Color PoliceLightColor
        {
            get
            {
                return IsPOliceLight_Enabled ? Colors.Blue : Colors.DarkBlue;
            }
        }

        

       

        

    }




}
