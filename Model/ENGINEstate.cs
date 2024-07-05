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
        private bool _isSmallLight_Enabled = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeadLightColor))]
        private bool isHeadLight_Enabled = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BlinkersColor))]
        private bool isLeftTurn_Enabled = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BlinkersColor))]
        private bool isRightTurn_Enabled = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BlinkersColor))]
        private bool isMultiblink_Enabled = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BlinkersColor))]
        private bool isBlinkDance_Enabled = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BlinkersColor))]
        private bool isHeadBlink_Enabled = false;

        [ObservableProperty]
        private bool isENGINE_ON = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PoliceLightColor))]
        private bool isPOliceLight_Enabled = false;

        [ObservableProperty]
        private bool isMeSirenSource_Enabled = false;
        //
        [ObservableProperty]
        private bool isShake_Detected = false;

        [ObservableProperty]
        private bool is_Silenced = false;//should reset the "ShakeDetected".


       
        public Color SmallLightColor
        {
            get
            {
                Log.Debug("SmallLightColor", "Invoked");
                return IsSmallLight_Enabled ? Colors.White : Colors.Gray;
            }
        }
        public Color HeadLightColor
        {
            get
            {
                return IsSmallLight_Enabled ? Colors.LightBlue : Colors.Gray;
            }
        }
        public Color BlinkersColor
        {
            get
            {
                return IsLeftTurn_Enabled ? Colors.Red : Colors.Gray;
            }
        }
        public Color PoliceLightColor
        {
            get
            {
                return IsPOliceLight_Enabled ? Colors.OrangeRed : Colors.Gray;
            }
        }



       

        

    }




}
