/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiUIpro.                                                **  
** Description:                                                                  **
** Serial Input | Output pre-defined Commands.                                   **
** Used For Communicate   Between UI and MicroController.                        **
** Created in sat 1403/01/025 18:40 PM By Hosein Pirani                          **
**  Modified In sat 1403/03/18 14:00 PM To 22:04 by hosein pirani                **
**  :Xaml Style... Some Methods...                                               **
**                                                                               **
** TODO:Complete Siren Player.                                                   **
** TODO: Complete Serial Functions in TryParse()                                 **
** Serial functions                                                              **
** Remove Live Charts and install LiveCharts.MAUI!!!!                              
** Event Handler For Them,State File writer ,GPS Speedometer,locator And sender  **
** And  LOT OF CODE @_@                                                          **
** ...                                                                           **  
**                                                                               **
**                                                                               **
**                                                                               **
 *********************************************************************************/

#if NET7_0
#warning Please Upgrade This Project To .Net8 For GPS Listener And More Features!
#endif




using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Core;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using Kotlin.Jvm.Functions;
using Hoho.Android.UsbSerial.Driver;
using Android.Hardware.Usb;
using CommunityToolkit.Mvvm.Input;
using HPISMARTUI.Helper;
using HPISMARTUI.Model;
using HPISMARTUI.View;
using HPISMARTUI.Messages;
using Android.OS;
using Android.Content;
using Android.Util;
using Java.Util.Logging;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Intrinsics.Arm;
using Android.App;
using static Android.Renderscripts.ScriptGroup;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Plugin.Maui.Audio;
using Android.Telephony;
using Android.Provider;
using Android.Text;
using HPISMARTUI.Services;
using Android.Content.PM;
using AndroidPlatform = Microsoft.Maui.ApplicationModel.Platform;
using AndroidX.Core.Content;
using Android.Runtime;
using Android.Views;
using Java.Interop;
using System.Reflection.Metadata;
using Tts = Android.Speech.Tts;
using IntelliJ.Lang.Annotations;
using System.Windows.Input;
using System.Reflection;
using static Android.Provider.ContactsContract.CommonDataKinds;
using static Android.Telephony.CarrierConfigManager;
using Android.Content.Res;
using Java.Sql;
using MauiPageFullScreen;
using Xamarin.KotlinX.Coroutines;
using Java.Util;
using Plugin.Maui.ScreenBrightness;
using Android.Telecom;
using System.Globalization;
//using Microsoft.Maui.Controls;


namespace HPISMARTUI.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainViewModel : ObservableObject//, IQueryAttributable
    {
#region GlobalOptions;
        public IScreenBrightness _screenBrightness;

        [ObservableProperty]
        ENGINEstate estate = new();
        //readonly AndroidLocationManager aLocationManager = new();

        //
        //Main Background
        readonly List<string> BackgroundImages = [];
        [ObservableProperty]
        public string backgroundSource = new("bg_b.png");
        //
        private IAudioPlayer SirenPlayer;
        // //////////////////////////////////////////////
        // SMS
        private static int REQUEST_PERMISSION_READ_STATE => 1;
        private static int REQUEST_GET_SMS_SUB_ID => 2;
        private static ComponentName SETTINGS_SUB_PICK_ACTIVITY => new(
            "com.android.settings", "com.android.settings.sim.SimDialogActivity");
        // Can't import PERFORM_IMS_SINGLE_REGISTRATION const directly beause it's a @SystemApi
        private static string PERFORM_IMS_SINGLE_REGISTRATION =>
            "android.permission.PERFORM_IMS_SINGLE_REGISTRATION";
        private static string DIALOG_TYPE_KEY => "dialog_type";
        public static string RESULT_SUB_ID => "result_sub_id";
        private static readonly int SMS_PICK = 2;
        public static  int SmessageId = 0;
        public int m_sMessageId = SmessageId;
        public bool mIsReadPhoneStateGranted = true;
        public string mPhoneNumber = "+989379223570";
        //
        //DateTime
        
        [ObservableProperty]
        PersianCalendar persiaCalendar;
        System.Timers.Timer TimerNow;

        //GPS
        private readonly int GpsUpdate_TimerInterval = 3; //TODO: Set Currect SetValue. 
        readonly System.Timers.Timer TimerGps;
#endregion

#region XamlBindings
        //Xaml Bindings
        [ObservableProperty]
        private float brightness;
        [ObservableProperty]
        private string persianDateNow;
        [ObservableProperty]
        private string timeNow;
        [ObservableProperty]
        private string timeMinuteNow;
        [ObservableProperty]
        private double bikeSpeed=0.0d;
        [ObservableProperty]
        private double bikeAcceleration=0.0d;
        [ObservableProperty]
        private string displayBatteryInfo ="12.2V 99%"; //Display Battery Voltage And Level.
        [ObservableProperty]
        private int displayFuelLevel=0; // Display Current FuelLevel.
        [ObservableProperty]
        private float displayEngineTemp=0.0f; //Store Engine Temperature
        [ObservableProperty]
        private bool displayOverTemp = false;//Engine Is Over Temp?
        [ObservableProperty]
        private int displayEngineRPM=0;
        [ObservableProperty]
        private byte autoStartDelay = 3;//Delay Between Command And AutoStart.
        [ObservableProperty]
        private int headblinkDelay = 150; //Headlight Blink Frequency. 
        //SMS

        //GPS Location
        [ObservableProperty]
        private string deviceLocation;
        private  Location RawLocation;
       
#endregion

        //Emergency 
        //SirenPlayer
#region EmergencyOptions
        readonly IAudioManager audioManager;
        private readonly List<string> OwnerNumbers = [];
        readonly System.Timers.Timer TimerEmergency;
        [ObservableProperty]
        private int temergencyInterval = 15;//
        private bool EmergencyMessageSent;//flag
        [ObservableProperty]
        private string forgiveMessage = "Sorry, EveryThing Is OK. No Emergency^_^";
#endregion

#region SerialOptions
        //////////////
        /// <summary>
        /// Serial
        /// </summary>
        
        bool IsSerialPortOpen = false;

        static string EncodingSend_Serial => "UTF-8";
        static string EncodingReceive_Serial => "UTF-8";
        //USB
        //public UsbDeviceInfo DeviceInfo { get; set; }
        //Serial Options(BAUD Rate etc...)
        readonly SerialOption serialOption = new();
        //USB
        public ObservableCollection<UsbDeviceInfo> UsbDevices { get; } = [];
        //Receive Buffer
        public string receiveSerialdata = "";
        //Send Buffer
        [ObservableProperty]
        string srialDataForSend = "Hello!";//Just For Test With SendEntryValue Command.
        //Input Serial Parser
        private readonly Dictionary<string, Action> Serial_actions;
        #endregion



        public MainViewModel(IScreenBrightness screenBrightness)
        {
            _screenBrightness = screenBrightness;
            Brightness = _screenBrightness.Brightness;
            // _screenBrightness.Brightness = 100.0f;

            //Emergency Call To Owner.
            TimerEmergency = new System.Timers.Timer(TimeSpan.FromSeconds(TemergencyInterval));
            TimerEmergency.Elapsed += TimerEmergency_Elapsed;
            TimerEmergency.Enabled = false;
            //GPS Timer For Continous Location Update.
            TimerGps = new System.Timers.Timer(TimeSpan.FromSeconds(GpsUpdate_TimerInterval));
            TimerGps.Elapsed += TimerGps_Elapsed;
            TimerGps.Enabled = false;
            //DateTimeTimer
            TimerNow = new(TimeSpan.FromSeconds(1));
            TimerNow.Elapsed += TimerNowElapsed;
            TimerNow.Enabled = true;
            TimerNow.AutoReset = true;

            //SMS Emergency Recipients.
            OwnerNumbers.Add("+989358152831");
            OwnerNumbers.Add("+989379223570");
            OwnerNumbers.Add("+989012795933");
            OwnerNumbers.Add("+989013913356");
            OwnerNumbers.Add("+989963042714");
            OwnerNumbers.Add("+989382532699");

            //ParseSerialCommands();
            // Register Serial Actions
            Serial_actions = new Dictionary<string, Action>
            {
                {Serial_InCommands.InSerial_AlarmSilenced_cmd, async ()=>await DoEmergencyState(false) },
                {Serial_InCommands.InSerial_ALarmSourceIsMicro_cmd,()=>SetEngineStateValue(nameof(Estate.IsMeSirenSource_Enabled),false) },//Not Implemented.
                {Serial_InCommands.InSerial_HeadLightIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsHeadLight_Enabled),true) },
                {Serial_InCommands.InSerial_AlarmSourceIsUI_cmd,()=>SetEngineStateValue(nameof(Estate.IsMeSirenSource_Enabled),true) },//Not Implemented.
                {Serial_InCommands.InSerial_AllBlinkersIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsMultiblink_Enabled),false) },
                {Serial_InCommands.InSerial_AllBlinkersIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsMultiblink_Enabled),true) },
                {Serial_InCommands.InSerial_HeadBlinkIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsHeadBlink_Enabled),false)},
                {Serial_InCommands.InSerial_HeadBlinkIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsHeadBlink_Enabled),true)},
                {Serial_InCommands.InSerial_BlinkDanceIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsBlinkDance_Enabled),false)},
                {Serial_InCommands.InSerial_BlinkDanceIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsBlinkDance_Enabled),true)},
                {Serial_InCommands.InSerial_ENGINEisOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsENGINE_ON),false)},
                {Serial_InCommands.InSerial_ENGINEisON_cmd,()=>SetEngineStateValue( nameof(Estate.IsENGINE_ON),true)},
                {Serial_InCommands.InSerial_HeadLightIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsHeadLight_Enabled),false)},
                {Serial_InCommands.InSerial_LeftTurnIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsLeftTurn_Enabled),false)},
                {Serial_InCommands.InSerial_LeftTurnIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsLeftTurn_Enabled),true)},
                {Serial_InCommands.InSerial_PoliceLightsIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsPOliceLight_Enabled),false)},
                {Serial_InCommands.InSerial_PoliceLightsIsOn_cmd,()=>SetEngineStateValue( nameof(Estate.IsPOliceLight_Enabled),true)},
                {Serial_InCommands.InSerial_RightTurnIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsRightTurn_Enabled),false)},
                {Serial_InCommands.InSerial_RightTurnIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsRightTurn_Enabled),true)},
                {Serial_InCommands.InSerial_ShakeDetected_cmd,async ()=>await DoEmergencyState(true)},//Not Implemented.
              //  {serial_In_Commands.InSerial_SirenIsOFF_cmd,()=>Function1()},//Not Implemented.
               // {serial_In_Commands.InSerial_SirenIsOn_cmd,()=>Function1()}, //Not Implemented.
                //Startup message.
                {Serial_InCommands.InSerial_STARTUP_cmd,()=>Function2() }//Not Implemented.


            };

            /////////////
            SerialPortHelper.WhenUsbDeviceAttached((usbDevice) =>
            {

                Dispatcher.GetForCurrentThread().Dispatch(async () => { await GetUsbDevices(); });
            });

            SerialPortHelper.WhenUsbDeviceDetached((usbDevice) =>
            {
                Dispatcher.GetForCurrentThread().Dispatch(async () => { await GetUsbDevices(); });

            });

            SerialPortHelper.WhenDataReceived().Subscribe(data =>
            {
                receiveSerialdata = SerialPortHelper.GetData(data, EncodingReceive_Serial);
                TryParseInput(receiveSerialdata);
            });


            //Messenger
            //Send StartListening To LocationManager.
            WeakReferenceMessenger.Default.Send(new ALocationManagerCommunications("StartListening"));

            WeakReferenceMessenger.Default.Register<MainViewModel, Messages.DeviceLocationMessage>
                (this, (recipient, message) =>
            {
                recipient.DeviceLocation = message.Value;
                Log.Debug("Messenger", "Received Location Message!");

            });

            WeakReferenceMessenger.Default.Register<MainViewModel, RawDeviceLocationMessage>(this, (recipient, message) =>
            {
                RawLocation = message.Value;
                if (RawLocation != null)
                {
                    if (RawLocation.Speed.HasValue)
                    {

                      var  speed_MetersPerMinute = RawLocation.Speed.Value * 60.0d;
                        // speed_MetersPerHours = speed_MetersPerMinute * 60.0f;
                        //BikeSpeed = speed_MetersPerHours  / 1000.0f; // divide to 1000 (Meters Per KM)
                        BikeSpeed = speed_MetersPerMinute * 0.06d; // Equal With: speed_MetersPerMinute * 60 / 1000
                       // BikeAcceleration = RawLocation.
                    } else
                    {
                        BikeSpeed = 0.0d;
                    }

                }
            });

            WeakReferenceMessenger.Default.Register<MainViewModel, ALocationManagerToMainViewModel>(
                this, (recipient, message) =>
                {
                    DisplayMessage(message.Value);
                });


                

            //Write SettingItems To ECU
            WeakReferenceMessenger.Default.Register<MainViewModel, Messages.WriteSettingToECUMessage>(
                this, async (recipient, message) =>
                {
                    //  await  Shell.Current.DisplayAlert("WritingSettingsToECU", message.Value, "OK");

                    await Task.Delay(TimeSpan.FromMilliseconds(1));

                    SendSerialData(message.Value);
                    await Task.Delay(TimeSpan.FromMilliseconds(1));

                });

            //Chart

        }

        // //////////////////////SMS Methods
#region SMS

        private void SendOutgoingSms(String SMSmessage, String PhoneNumber = "+989012795933")
        {
            String phoneNumber = mPhoneNumber;
            if (String.IsNullOrEmpty(phoneNumber))
            {
                Log.Debug("SendOutgoingSms", "Couldn't get phone number from view! Ignoring request...");
                return;
            }
            if (mIsReadPhoneStateGranted)
            {

                //SmsManager m = SmsManager.Default;
#pragma warning disable CA1422 // Validate platform compatibility
                SmsManager.Default.SendTextMessage(PhoneNumber, null, SMSmessage,
                        PendingIntent.GetBroadcast(AndroidPlatform.CurrentActivity, SmessageId, GetSendStatusIntent(), 0),
                        null);
#pragma warning restore CA1422 // Validate platform compatibility
                Log.Debug("SendOutgoingSms", "SmsManager called");
                SmessageId++;
                
            }
        }

        private void SendOutgoingSmsService(String SMSmessage,String PhoneNumber = "+989012795933")
        {
            Log.Debug("SendOutgoingSmsService", "begin");
            String phoneNumber = mPhoneNumber;
            if (TextUtils.IsEmpty(phoneNumber))
            {
                Log.Debug("SendOutgoingSmsService", "Couldn't get phone number from view! Ignoring request...");
                return;
            }
            if (mIsReadPhoneStateGranted)
            {
                Intent sendSmsIntent = new(nameof(SmsManagerTestService));
                sendSmsIntent.PutExtra(SmsManagerTestService.EXTRA_SEND_TEXT, SMSmessage);
                sendSmsIntent.PutExtra(SmsManagerTestService.EXTRA_SEND_NUMBER, PhoneNumber);
                sendSmsIntent.PutExtra(SmsManagerTestService.EXTRA_SEND_INTENT,
                        PendingIntent.GetBroadcast(AndroidPlatform.CurrentActivity.BaseContext, SmessageId, GetSendStatusIntent(), 0));
                sendSmsIntent.SetComponent(new ComponentName(AndroidPlatform.CurrentActivity.BaseContext, nameof(SmsManagerTestService)));
                AndroidPlatform.CurrentActivity.BaseContext.StartService(sendSmsIntent);
                SmessageId++;
                Log.Debug("SendOutgoingSmsService", "called");
            }
        }

        private void GetSubIdForResult()
        {
            // ask the user for a default SMS SIM.
            Intent intent = new();
            intent.SetComponent(SETTINGS_SUB_PICK_ACTIVITY);
            intent.PutExtra(DIALOG_TYPE_KEY, SMS_PICK);
            try
            {
                AndroidPlatform.CurrentActivity.StartActivity(intent, null);
            }
            catch (ActivityNotFoundException anfe)
            {
                // If SettingItems is not installed, only log the error as we do not want to break
                // legacy applications.
                
                Log.Debug("GetSubIdForResult", $"Unable to launch SettingItems application: {anfe.Message}");
            }
        }

        private void SetPersistentServiceComponentEnabled()
        {
            var serviceIntent = new Intent(AndroidPlatform.CurrentActivity.BaseContext, typeof(PersistentService));
            AndroidPlatform.CurrentActivity.BaseContext.StartService(serviceIntent);
            AndroidPlatform.CurrentActivity.PackageManager.SetComponentEnabledSetting(
                    new ComponentName(AndroidPlatform.CurrentActivity.BaseContext, nameof(PersistentService)),
                        ComponentEnabledState.Enabled,
                        ComponentEnableOption.DontKillApp);
        }

        private void SetPersistentServiceComponentDisabled()
        {
            AndroidPlatform.CurrentActivity.PackageManager.SetComponentEnabledSetting(
            new ComponentName(AndroidPlatform.CurrentActivity.BaseContext, nameof(PersistentService)),
                ComponentEnabledState.Disabled,
                ComponentEnableOption.DontKillApp);
        }

        private void CheckSingleRegPermission()
        {
            if (AndroidPlatform.CurrentActivity.CheckSelfPermission(PERFORM_IMS_SINGLE_REGISTRATION)
                    == Permission.Granted)
            {
                Log.Debug("CheckSingleRegPermission", "Single Reg permission granted");
            } else
            {

                Log.Debug("CheckSingleRegPermission", "Single Reg permission NOT granted");
            }

        }

        public Intent GetSendStatusIntent()
        {
            // Encode requestId in intent data
            Log.Debug("GetSendStatusIntent", "Called.");
            return new Intent(SendStatusReceiver.MESSAGE_SENT_ACTION, null, AndroidPlatform.CurrentActivity.BaseContext,
                    typeof(SendStatusReceiver));
        }

        /*        public  String getPhoneNumber()
                {
                    Log.Debug("getPhoneNumber", "getting...");
                    String result = "0";

        #pragma warning disable CA1416 // Validate platform compatibility
                    int defaultSmsSub = SubscriptionManager.DefaultSmsSubscriptionId;
        #pragma warning restore CA1416 // Validate platform compatibility
                    String line1Number = "000";
                    if (mIsReadPhoneStateGranted)
                    {

                         // TelephonyManager  tm = AndroidPlatform.CurrentActivity.BaseContext.GetSystemService(nameof(TelephonyManager)).JavaCast<TelephonyManager>();
                         // var tm = global::Java.Lang.Object.GetObject<Android.Telephony.TelephonyManager>(0, jnihandle, JniHandleOwnership.DoNotTransfer)!;
                        TelephonyManager tm = (TelephonyManager)AndroidPlatform.CurrentActivity.GetSystemService(Context.TelephonyService);
                        if (tm != null)
                        {

        #pragma warning disable CA1416 // Validate platform compatibility
                            tm = tm.CreateForSubscriptionId(defaultSmsSub);
        #pragma warning restore CA1416 // Validate platform compatibility

        #pragma warning disable CA1422 // Validate platform compatibility
                             line1Number = tm.Line1Number;
        #pragma warning restore CA1422 // Validate platform compatibility

                            if (!TextUtils.IsEmpty(line1Number))
                            {
                                return line1Number;
                            }
                            Log.Debug("getPhoneNumber", "tm notNull but LineNo is Null.");
                        }
                    } else
                    {
                        Log.Debug("getPhoneNumber", "Couldn't resolve line 1 due to permissions error.");
                    }
                    MyphoneNumber = line1Number;
                    return result;
                }*/

        /// <summary>
        /// Send Device's Current GeoLocation Info To owner Number(s),
        /// Or If Message Sent Wrongly(means There Was No Actual Emergency, Just Rider Was'nt Silenced Alarm Before Timer_Emergency() TimeOut),
        /// Send Exuse Me Message!
        /// </summary>
        /// <param name="forgive"></param>
        private async void SendEmergencySMS(bool forgive = false)
        
        {


                foreach (var num in OwnerNumbers)
                {
                    SendOutgoingSms((forgive ? ForgiveMessage : DeviceLocation), num);
                   await Task.Delay(1000);

            }
                return;
            
            
        }
        #endregion

#region XamlCommands

        [RelayCommand]
        private async Task GotoSettingsPageAsync()
        {

            await Shell.Current.GoToAsync(nameof(SettingsPage), true);

        }


        [RelayCommand]
        private async Task LoadBackgroundImage()
        {
            DateTime date = DateTime.Today;

            String PrevChange = await SecureStorage.Default.GetAsync("LastDate");

            // var equal =  String.Equals(PrevChange, date.DayOfYear.ToString());

            if ((PrevChange is null) || !(String.Equals(PrevChange, date.DayOfYear.ToString())))
            {
                await SecureStorage.Default.SetAsync("LastDate", date.DayOfYear.ToString());

                BackgroundImages.Add("bg_a.png");
                BackgroundImages.Add("bg_b.png");
                BackgroundImages.Add("bg_c.png");
                BackgroundImages.Add("bg_d.png");
                BackgroundImages.Add("bg_e.png");
                BackgroundImages.Add("bg_f.png");
                BackgroundImages.Add("bg_g.png");
                BackgroundImages.Add("bg_h.png");
                BackgroundImages.Add("bg_i.png");
                BackgroundImages.Add("bg_j.png");
                BackgroundImages.Add("bg_k.png");
                BackgroundImages.Add("bg_l.png");
                BackgroundImages.Add("bg_m.png");
                BackgroundImages.TrimExcess();
                var random = new System.Random();
                var retrivedeRandom = random.Next(BackgroundImages.Count);
                Log.Debug("Random", retrivedeRandom.ToString());
                try
                {
                    if (String.Equals(BackgroundSource, BackgroundImages[retrivedeRandom]))
                    {
                        retrivedeRandom = random.Next(BackgroundImages.Count);

                    }
                }
                finally
                {
                    BackgroundSource = BackgroundImages[retrivedeRandom];

                    // AndroidPlatform.CurrentActivity.RequestedOrientation = ScreenOrientation.Landscape;
                }
            }

        }


        [RelayCommand]
        public void GetLastLocation()
        {
            //WeakReferenceMessenger.Default.Send(new Messages.EngineState_HeadLightMessage(true));
            // await aLocationManager.GetLastLocation();
            //  Get_LastLocation();
            //   await Task.Delay(TimeSpan.FromSeconds(5));
             WeakReferenceMessenger.Default.Send(new Messages.ALocationManagerCommunications("GetLastLocation"));

            
        }

        //XamlCommands.
        //Sent To MCU.

        /// <summary>
        /// Send UI Commands To MCU
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task SendCommandToENGINEAsync(String command)
        {
            if (!String.IsNullOrEmpty(command))
            {

                string action;//???

                //Process AutoStart and Or HeadLight Blinker.
                if ((string.Equals(command, "AutoStart")) || (string.Equals(command, "ToggleHeadBlink")))
                {
                    StringBuilder stringBuilder = new();
                    if (string.Equals(command, "AutoStart"))
                    {
                        stringBuilder.Clear();
                        stringBuilder.Append("ASE:");
                        stringBuilder.Append(AutoStartDelay);
                        action = stringBuilder.ToString();
                    } else
                    {
                        if (Estate.IsHeadBlink_Enabled)
                        {
                            action = Serial_OutCommands.OutSerial_HeadBlinkOFF_cmd;
                        } else
                        {

                            stringBuilder.Clear();
                            stringBuilder.Append("HBO:");
                            stringBuilder.Append(HeadblinkDelay);
                            action = stringBuilder.ToString();
                        }
                    }
                    
                    
                } else
                {
                    // Procces Touch Commands
                    action = command switch 
                    { //Turn off if HeadLight Is On.
                        "ToggleHeadLight" => Estate.IsHeadLight_Enabled ? Serial_OutCommands.OutSerial_HeadLightOFF_cmd : Serial_OutCommands.OutSerial_HeadLightON_cmd,
                        "ToggleLeftTurn" => Estate.IsLeftTurn_Enabled ? Serial_OutCommands.OutSerial_LeftBlinkOFF_cmd : Serial_OutCommands.OutSerial_LeftBlinkON_cmd,
                        "ToggleRightTurn" => Estate.IsRightTurn_Enabled ? Serial_OutCommands.OutSerial_RightBlinkOFF_cmd : Serial_OutCommands.OutSerial_RightBlinkON_cmd,
                        "ToggleMultiblinker" => Estate.IsMultiblink_Enabled ? Serial_OutCommands.OutSerial_MultiBlinkOFF_cmd : Serial_OutCommands.OutSerial_MultiBlinkON_cmd,
                        "ToggleBlinkDance" => Estate.IsBlinkDance_Enabled ? Serial_OutCommands.OutSerial_BlinkerDanceOFF_cmd : Serial_OutCommands.OutSerial_BlinkerDanceON_cmd,
                        "TogglePoliceLights" => Estate.IsPOliceLight_Enabled ? Serial_OutCommands.OutSerial_PoliceLightOFF_cmd : Serial_OutCommands.OutSerial_PoliceLightON_cmd,
                        "ForceAutoStart" => Estate.IsENGINE_ON ? "" : "ASE:100",

                        _ => throw new NotImplementedException(),

                    };
                    
                }

              await  Shell.Current.DisplayAlert("AutoStart Result", action, "OK");
                SendSerialData(action);
            }
        }
        #endregion

    /////
    //Serial Commands
#region SerialCommunication
        //[RelayCommand]
        public async Task GetUsbDevices()
        {
            UsbDevices.Clear();
            var list = SerialPortHelper.GetUsbDevices();
            foreach (var item in list)
            {
                UsbDevices.Add(item);
                Log.Debug("Add", "Adding { nameof(item)}");
                //fix VirtualView cannot be null here
                await Task.Delay(50);
            }


            if (UsbDevices.Count > 0)
            {
              await  OpenSerialPort();
            } else
            {
                CloseSerialPort();
            }


        }


        //[RelayCommand]
        public async Task OpenSerialPort()
        {
            if (!IsSerialPortOpen)
            {
                string r = await SerialPortHelper.RequestPermissionAsync(UsbDevices.FirstOrDefault());
                if (SerialPortHelper.CheckError(r, showDialog: false))
                {
                    r = SerialPortHelper.Open(UsbDevices.FirstOrDefault(), serialOption);
                    if (SerialPortHelper.CheckError(r, showDialog: false))
                    {
                        //Opened Successfully
                        Log.Info("OpenSerialPort", "Opened Successfully");
                        IsSerialPortOpen = true;
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        // Send();

                    } else
                    {
                        Log.Info("OpenSerialPort", "OPen Error");
                    }
                } else
                {
                    Log.Info("OPEN", "ERROR Request : {r}");
                }
            }
        }
      //  [RelayCommand]
        public void CloseSerialPort()
        {
            try
            {
                SerialPortHelper.Close();
                //  CycleToSend = false;
                IsSerialPortOpen = false;
            }
            catch (Exception)
            {
            }

        }

        /// <summary>
        /// Send Data To (USB) Serial Port
        /// </summary>
        /// <param name="Data"> String Data To Be Sended</param>
      //  [RelayCommand]
        public void SendSerialData(String Data)
        {
            if (IsSerialPortOpen)
            {
                byte[] send = SerialPortHelper.GetBytes(Data, EncodingSend_Serial);
                if (send.Length == 0)
                {
                    return;
                }
                string r = SerialPortHelper.Write(send);
                if (SerialPortHelper.CheckError(r))
                {
                    if (EncodingSend_Serial == "HEX")
                    {
                        //   AddLog(new SerialLog(SendData.ToUpper(), true));
                    } else
                    {
                        //  AddLog(new SerialLog(SendData, true));
                         Shell.Current.DisplayAlert("SendSerialData:", SrialDataForSend, "Ok");
                    }
                } else
                {
                    //   AddLog(new SerialLog(r, true));
                     Shell.Current.DisplayAlert("SendSerialData Error!", r, "Ok");
                }
            }

        }

        private void TryParseInput(string input)
        {            
            if(IsSerialPortOpen)
            {
                Log.Debug("Received", "${input}");
                Estate.IsSmallLight_Enabled = true;
                if (Serial_actions.TryGetValue(input, out Action value))
                {
                    value.Invoke();
                } else
                {
                    Log.Debug("Try", "NotEqual");
                    //Numerical Commands 
                    ParseNumericalCommands(input);
                }
            }

        }



        private async void ParseNumericalCommands(string input)
        {
            var is_RPM = input.Contains("RPM:");
            var is_fuel = input.Contains("FuL:");
            var is_BatteryLevel = input.Contains("VBT:");
            var is_Temp = input.Contains("TMP:");

            if (is_RPM)
            {
                DisplayEngineRPM = SubString_and_ToInt(input, ":");


            } else if (is_fuel)
            {
                DisplayFuelLevel = SubString_and_ToInt(input, ":");
            } else if (is_BatteryLevel)
            {
                //Format BatteryLevel
                var BatteryLevel = SubString_and_ToInt(input, ":");
                 DisplayBatteryInfo = Utilities.FormatBatteryInfo(BatteryLevel);

            } else if (is_Temp)
            {
                DisplayEngineTemp = SubString_and_ToInt(input, ":");
                DisplayOverTemp = DisplayEngineTemp >= 200.0f;

            } else
            {
                await Shell.Current.DisplayAlert(input, "Failed To Parse Numerical Commands.", "OK");
            }


        }

        /// <summary>
        /// Retrives Number From Serial Data after given character(Subvalue).
        /// </summary>
        /// <param name="str">String To Be Prosseced</param>
        /// <param name="SubValue">String Before Number</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static int SubString_and_ToInt(String str, String SubValue)
        {
            if ((String.IsNullOrEmpty(str)) || (String.IsNullOrEmpty(SubValue))) return 0;
            int position = str.IndexOf(SubValue);
            var substr = str.Substring(position + 1);
            int result;
            try
            {
                //  var result = Int32.Parse(substr);
                result = Convert.ToInt32(substr);
            }

            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return result;

        }

        private void SetEngineStateValue(String field, bool value)
        {
            WeakReferenceMessenger.Default.Send(new Messages.EngineState_HeadLightMessage(true));

            switch (field)
            {
                //case nameof(Estate.Is_Silenced):
                // Estate.Is_Silenced = value;
                // break;
                case nameof(Estate.IsHeadLight_Enabled):
                    Estate.IsHeadLight_Enabled = value;
                    break;
                case nameof(Estate.IsMultiblink_Enabled):
                    Estate.IsMultiblink_Enabled = value;
                    break;
                case nameof(Estate.IsHeadBlink_Enabled):
                    Estate.IsHeadBlink_Enabled = value;
                    break;
                case nameof(Estate.IsBlinkDance_Enabled):
                    Estate.IsBlinkDance_Enabled = value;
                    break;
                case nameof(Estate.IsENGINE_ON):
                    Estate.IsENGINE_ON = value;
                    
                    break;
                case nameof(Estate.IsLeftTurn_Enabled):
                    Estate.IsLeftTurn_Enabled = value;
                    break;
                case nameof(Estate.IsRightTurn_Enabled):
                    Estate.IsRightTurn_Enabled = value;
                    break;
                case nameof(Estate.IsPOliceLight_Enabled):
                    Estate.IsPOliceLight_Enabled = value;
                    break;
                // case nameof(Estate.IsShake_Detected):
                //   Estate.IsShake_Detected = value;
                //   break;
                case nameof(Estate.IsMeSirenSource_Enabled):
                    Estate.IsMeSirenSource_Enabled = value;

                    break;
            }



        }
        #endregion

#region Timers
        /*        void ParseSerialCommands()
                {
                    Serial_actions = new Dictionary<string, Action>
                    {
                        {Serial_InCommands.InSerial_AlarmSilenced_cmd, async ()=>await DoEmergencyState(false) },
                        {Serial_InCommands.InSerial_ALarmSourceIsMicro_cmd,()=>SetEngineStateValue(nameof(Estate.IsMeSirenSource_Enabled),false) },//Not Implemented.
                        {Serial_InCommands.InSerial_HeadLightIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsHeadLight_Enabled),true) },
                        {Serial_InCommands.InSerial_AlarmSourceIsUI_cmd,()=>SetEngineStateValue(nameof(Estate.IsMeSirenSource_Enabled),true) },//Not Implemented.
                        {Serial_InCommands.InSerial_AllBlinkersIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsMultiblink_Enabled),false) },
                        {Serial_InCommands.InSerial_AllBlinkersIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsMultiblink_Enabled),true) },
                        {Serial_InCommands.InSerial_HeadBlinkIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsHeadBlink_Enabled),false)},
                        {Serial_InCommands.InSerial_HeadBlinkIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsHeadBlink_Enabled),true)},
                        {Serial_InCommands.InSerial_BlinkDanceIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsBlinkDance_Enabled),false)},
                        {Serial_InCommands.InSerial_BlinkDanceIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsBlinkDance_Enabled),true)},
                        {Serial_InCommands.InSerial_ENGINEisOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsENGINE_ON),false)},
                        {Serial_InCommands.InSerial_ENGINEisON_cmd,()=>SetEngineStateValue( nameof(Estate.IsENGINE_ON),true)},
                        {Serial_InCommands.InSerial_HeadLightIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsHeadLight_Enabled),false)},
                        {Serial_InCommands.InSerial_LeftTurnIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsLeftTurn_Enabled),false)},
                        {Serial_InCommands.InSerial_LeftTurnIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsLeftTurn_Enabled),true)},
                        {Serial_InCommands.InSerial_PoliceLightsIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsPOliceLight_Enabled),false)},
                        {Serial_InCommands.InSerial_PoliceLightsIsOn_cmd,()=>SetEngineStateValue( nameof(Estate.IsPOliceLight_Enabled),true)},
                        {Serial_InCommands.InSerial_RightTurnIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsRightTurn_Enabled),false)},
                        {Serial_InCommands.InSerial_RightTurnIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsRightTurn_Enabled),true)},
                        {Serial_InCommands.InSerial_ShakeDetected_cmd,async ()=>await DoEmergencyState(true)},//Not Implemented.
                      //  {serial_In_Commands.InSerial_SirenIsOFF_cmd,()=>Function1()},//Not Implemented.
                       // {serial_In_Commands.InSerial_SirenIsOn_cmd,()=>Function1()}, //Not Implemented.
                        //Startup message.
                        {Serial_InCommands.InSerial_STARTUP_cmd,()=>Function2() }//Not Implemented.


                    };
                }*/
        //TIMERS
        //
        /// <summary>
        /// Timer For Emergency Call.
        /// Used for Send Last Location Of Device(== Bike) To Owners Number Via SMS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerEmergency_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Call Emergency!
            TimerEmergency.Stop();
            SendEmergencySMS();
       EmergencyMessageSent = true; 
        }
       
        /// <summary>
        /// Timer For GPS Location Updates. The Location will  Update Permanently For More Security.
        /// </summary>
        private void TimerGps_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new Messages.ALocationManagerCommunications("GetLastLocation"));

        }
        private void TimerNowElapsed(object sender,System.Timers.ElapsedEventArgs e)
        {
            
            StringBuilder stringBuilder = new StringBuilder();
            PersiaCalendar = new();
            stringBuilder.Append(PersiaCalendar.GetYear(DateTime.Now));
            stringBuilder.Append('/');
            stringBuilder.Append(PersiaCalendar.GetMonth(DateTime.Now));
            stringBuilder.Append('/');
            stringBuilder.Append(PersiaCalendar.GetDayOfMonth(DateTime.Now));
            PersianDateNow = stringBuilder.ToString();
            stringBuilder.Clear();
            stringBuilder.Append(DateTime.Now.Hour);
            stringBuilder.Append('\n');
            stringBuilder.Append(DateTime.Now.Minute);
            TimeNow = stringBuilder.ToString();
        }

#endregion

#region EmergencyAndSiren
        /// <summary>
        /// 
        /// Emergency State
        /// </summary>
        /// <param name="Emergency"></param>
        /// <returns></returns>
        /// TODO: Add Emergency ShutDown Mode.
        private async Task DoEmergencyState(bool Emergency)
        {
            if (Emergency)
            {
                TimerEmergency.Enabled = true;
                if (Estate.IsMeSirenSource_Enabled)
                {
                   await DoSirenSound(true);
                }

            }else
            {
                
                if (TimerEmergency.Enabled)
                {
                    await DoSirenSound(false);
                    
                    TimerEmergency.Enabled = false;

                }
                if (EmergencyMessageSent)
                {
                    EmergencyMessageSent= false;
                    //Send ForgiveMe Message :( 
                    SendEmergencySMS(true);
                }
            }

        }
#region Brightness
/*        public void SetBrightness(float brightness)
        {
            Android.Views.Window window = AndroidPlatform.CurrentActivity.Window;
         //   var attributesWindow = new WindowManagerLayoutParams();

          // attributesWindow.CopyFrom(window.Attributes);
            window.Attributes.ScreenBrightness = brightness;
           // attributesWindow.ScreenBrightness = brightness;
           
         //   window.Attributes = attributesWindow;
        }*/
        #endregion
        private async Task DoSirenSound(bool play)
        {
            if (play)
            {
                SirenPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("Alarm.mp3"));                
                SirenPlayer.Play();
            } else
            {
                if (SirenPlayer is not null)
                {
                    SirenPlayer.Stop();
                    SirenPlayer.Dispose();
                }
            }
        }

#endregion

        void Function2()
        {
            Log.Debug("Function2", "HelloFromFunc2");
            Estate.IsSmallLight_Enabled = false;
            
        }


        //Location

        static async void DisplayMessage(string message, string title = "Note", string ok = "OK")
        {
            await Shell.Current.DisplayAlert(title, message, ok);
        }

        protected virtual async void OnAppearing()
        {
            await Shell.Current.DisplayPromptAsync(nameof(OnAppearing), "OnAppearing", "OK");
            Controls.ToggleFullScreenStatus();
        }

        public virtual async void OnDisappearing()
        {
            await Shell.Current.DisplayPromptAsync(nameof(OnDisappearing), "OnDisappearing", "OK");
            WeakReferenceMessenger.Default.Send(new Messages.ALocationManagerCommunications("StopListening"));

        }








    }



}
//TODO: Move It To Another File!!
namespace HPISMARTUI.Messages// CommunityToolkit.Mvvm.Messaging.Messages
{

    public class EngineState_HeadLightMessage(bool state) : ValueChangedMessage<bool>(state)
    {
    }

    public class DeviceLocationMessage(String value) : ValueChangedMessage<String>(value)
    {
    }
    public class RawDeviceLocationMessage(Location value) : ValueChangedMessage<Location>(value)
    {
    }
    public class ALocationManagerCommunications(String value) : ValueChangedMessage<String>(value)
    {
    }
    public class ALocationManagerToMainViewModel(String value) : ValueChangedMessage<String>(value)
    {
    }
    public class WriteSettingToECUMessage(String value) : ValueChangedMessage<String>(value)
    {
    }

}
