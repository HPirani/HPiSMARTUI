/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiUIpro.                                                **  
** Description:                                                                  **
** Serial Input | Output pre-defined Commands.                                   **
** Used For Communicate   Between UI and MicroController.                        **
** Created in sat 1403/01/025 18:40 PM By Hosein Pirani                          **
**  Modified In sat 1403/02/29 13:00 PM To 20:00 by hosein pirani                **
**  :GPS-SMS-Fonts.                                                              **
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
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
//using Microsoft.Maui.Controls;


namespace HPISMARTUI.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainViewModel : ObservableObject//, IQueryAttributable
    {
        [ObservableProperty]
        ENGINEstate estate = new ENGINEstate();
        AndroidLocationManager aLocationManager = new AndroidLocationManager();
        public Serial_OUTCommands serial_out_Command = new();
        Serial_InCommands serial_In_Commands = new();
        //Main Background

        //chart


        //

        List<String> BackgroundImages = new();
        [ObservableProperty]
        public String backgroundSource = new("bg_b.png");
        //
        private IAudioPlayer SirenPlayer;

        // //////////////////////////////////////////////
        // SMS
        private static int REQUEST_PERMISSION_READ_STATE = 1;
        private static int REQUEST_GET_SMS_SUB_ID = 2;
        private static ComponentName SETTINGS_SUB_PICK_ACTIVITY = new ComponentName(
            "com.android.settings", "com.android.settings.sim.SimDialogActivity");
        // Can't import PERFORM_IMS_SINGLE_REGISTRATION const directly beause it's a @SystemApi
        private static String PERFORM_IMS_SINGLE_REGISTRATION =
            "android.permission.PERFORM_IMS_SINGLE_REGISTRATION";
        private static String DIALOG_TYPE_KEY = "dialog_type";
        public static String RESULT_SUB_ID = "result_sub_id";
        private static int SMS_PICK = 2;
        public static int sMessageId = 0;
        public int m_sMessageId = sMessageId;
        public bool mIsReadPhoneStateGranted = true;
        public String mPhoneNumber = "+989379223570";
        //
        //GPS
        private int GpsUpdate_TimerInterval = 3; //TODO: Set Currect Value. 
        System.Timers.Timer TimerGps;



        //TextToSpeech.




        //Xaml Bindings
        [ObservableProperty]
        private Color smallLightclr;
        [ObservableProperty]
        private int displayBatteryLevel;
        [ObservableProperty]
        private int displayFuelLevel;
        [ObservableProperty]
        private int displayEngineTemp;
        [ObservableProperty]
        private int displayEngineRPM;
        //SMS
        [ObservableProperty]
        private String smsMessage = "Hello From HPi!";
        private List<String> OwnerNumbers = new();
        //GPS Location
        [ObservableProperty]
        private String deviceLocation;
/*        [ObservableProperty]
        private double bikeSpeed = 0.0f;*/


        //Emergency 
        //SirenPlayer
        readonly IAudioManager audioManager;

        System.Timers.Timer TimerEmergency;
        [ObservableProperty]
        private int temergencyInterval = 15;//
        private bool EmergencyMessageSent;//flag


        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Serial
        /// </summary>
        [ObservableProperty]
        bool isOpen = false;

        readonly string EncodingSend = "UTF-8";
        readonly string EncodingReceive = "UTF-8";
        //USB
        //public UsbDeviceInfo DeviceInfo { get; set; }
        //Serial Options(BAUD Rate etc...)
        readonly SerialOption serialOption = new();
        //USB
        public ObservableCollection<UsbDeviceInfo> UsbDevices { get; } = new();
        //Receive Buffer
        public string receivedata = "";
        //Send Buffer
        [ObservableProperty]
        string sendData = "Hello!";//Just For Test With SendEntryValue Command.
        //Input Serial Parser
        private Dictionary<string, Action> _actions;
        [ObservableProperty]
        private string forgiveMessage = "Sorry, EveryThing Is OK. No Emergency^_^";

        //private readonly List<String> SerialInCommandList = new();




        /// <summary>
        /// 
        /// <see cref="MainViewModel"/>
        /// 
        /// </summary>
        public MainViewModel()
        {

            //Emergency Call To Owner.
            TimerEmergency = new System.Timers.Timer(TimeSpan.FromSeconds(TemergencyInterval));
            TimerEmergency.Elapsed += TimerEmergency_Elapsed;
            TimerEmergency.Enabled = false;
            //GPS Timer For Continous Location Update.
            TimerGps = new System.Timers.Timer(TimeSpan.FromSeconds(GpsUpdate_TimerInterval));
            TimerGps.Elapsed += TimerGps_Elapsed;
            TimerGps.Enabled = false;

            //SMS Emergency Recipients.
            OwnerNumbers.Add("+989358152831");
            OwnerNumbers.Add("+989379223570");
            OwnerNumbers.Add("+989012795933");
            OwnerNumbers.Add("+989013913356");
            OwnerNumbers.Add("+989963042714");
            OwnerNumbers.Add("+989382532699");

            ParseSerialCommands();

            SerialPortHelper.WhenUsbDeviceAttached((usbDevice) =>
            {
                GetUsbDevices();
            });

            SerialPortHelper.WhenUsbDeviceDetached((usbDevice) =>
            {
                GetUsbDevices();

            });

            SerialPortHelper.WhenDataReceived().Subscribe(data =>
            {
                receivedata = SerialPortHelper.GetData(data, EncodingReceive);
                TryParseInput(receivedata);
            });


            //Messenger
            WeakReferenceMessenger.Default.Register<MainViewModel, Messages.DeviceLocationMessage>
                (this,(recipient, message) =>
            {
                recipient.DeviceLocation = message.Value;
              Log.Debug("Messenger", "Received Location Message!");

           });

            //Chart
     
        }



        /* public void ApplyQueryAttributes(IDictionary<string, object> query)
            {
           GetUsbDevices();
            if (query.ContainsKey("Serial"))
           {
               DeviceInfo = (UsbDeviceInfo)query["Serial"];
                 Open();
             }
          }*/

        [RelayCommand]
        private void FullScreen()
        {
        Controls.ToggleFullScreenStatus();
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
                var random = new Random();
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

                    AndroidPlatform.CurrentActivity.RequestedOrientation = ScreenOrientation.Landscape;
                }
            }
             
        }



        // //////////////////////SMS Methods


        private  void SendOutgoingSms(String SMSmessage, String PhoneNumber = "+989012795933")
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
                SmsManager.Default.SendTextMessage(PhoneNumber, null, SMSmessage,
                        PendingIntent.GetBroadcast(AndroidPlatform.CurrentActivity, sMessageId, GetSendStatusIntent(), 0),
                        null);
                Log.Debug("SendOutgoingSms", "SmsManager called");
                sMessageId++;
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
                Intent sendSmsIntent = new Intent(nameof(SmsManagerTestService));
                sendSmsIntent.PutExtra(SmsManagerTestService.EXTRA_SEND_TEXT, SMSmessage);
                sendSmsIntent.PutExtra(SmsManagerTestService.EXTRA_SEND_NUMBER, PhoneNumber);
                sendSmsIntent.PutExtra(SmsManagerTestService.EXTRA_SEND_INTENT,
                        PendingIntent.GetBroadcast(AndroidPlatform.CurrentActivity.BaseContext, sMessageId, GetSendStatusIntent(), 0));
                sendSmsIntent.SetComponent(new ComponentName(AndroidPlatform.CurrentActivity.BaseContext, nameof(SmsManagerTestService)));
                AndroidPlatform.CurrentActivity.BaseContext.StartService(sendSmsIntent);
                sMessageId++;
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
                // If Settings is not installed, only log the error as we do not want to break
                // legacy applications.
                Log.Debug("GetSubIdForResult", $"Unable to launch Settings application: {anfe.Message}");
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
#region XamlCommands

        


        [RelayCommand]
        public async Task SMSAsync()
        {



            //    getPhoneNumber();
            // getSubIdForResult();
            //   await Task.Delay(1000);
            await TextToSpeech.SpeakAsync("Hello everybody!");
            SendOutgoingSms(SmsMessage,mPhoneNumber);
            //   await Task.Delay(100);
            //  setPersistentServiceComponentEnabled();
            //   await Task.Delay(100);
            //   sendOutgoingSmsService();
            //    await Task.Delay(100);
            //   checkSingleRegPermission();

            //   SmsManager.Default.SendTextMessage("+989379223570", null, "Hello Xamarin This is My Test SMS", null, null);


        }

        [RelayCommand]
        public async  Task GetLastLocation()
        {
            //WeakReferenceMessenger.Default.Send(new Messages.EngineState_HeadLightMessage(true));
            // await aLocationManager.GetLastLocation();
            //  Get_LastLocation();
            //   await Task.Delay(TimeSpan.FromSeconds(5));
            await aLocationManager.GetDeviceLocation();
            
            
        }

        //XamlCommands.
        //Sent To MCU.
        /// <summary>
        /// HeadLight Command.
        /// Process HeadLight Command(ON?OFF?BLINK?).
        /// </summary>
        [RelayCommand]
        public async Task HeadLightAsync()
        {


        }
        //
        /// <summary>
        /// Test
        /// </summary>
        [RelayCommand]
        public void SmallLight()
        {
            Estate.IsSmallLight_Enabled = !Estate.IsSmallLight_Enabled;
        }

        //
        /// <summary>
        /// RightTurn Command
        /// Process Right Blinkers(ON?OFF?).
        /// </summary>
        [RelayCommand]
        public async Task RightBlinkerAsync()
        {
        }
        //
        /// <summary>
        /// MultiBlink Command.
        /// prosses All blinkers(ON?OFF?DANCE?).
        /// </summary>
        [RelayCommand]
        public async Task MultiBlinkAsync()
        {
        }
        //
        /// <summary>
        /// SirenCommands.
        /// Prosses Police lights(ON?OFF?LOUD?SILENTLY?).
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task PolicelightsAsync()
        {

        }
        //
        /// <summary>
        /// SendEntryValueCommand
        /// Just For Test. Will Be Removed.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public void SendEntryValue()
        {
            Send(SendData);
        }
        //
        /// <summary>
        /// Send UI Commands To MCU
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task SendCommandToENGINEAsync(String command)
        {
            if (!String.IsNullOrEmpty(command))
            {
                Send(command);
            }
        }
#endregion

        /////////////////////////////////////////////////////////
        //Serial Commands
        [RelayCommand]
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
              await  Open();
            } else
            {
                Close();
            }


        }


        [RelayCommand]
        public async Task Open()
        {
            if (!IsOpen)
            {
                string r = await SerialPortHelper.RequestPermissionAsync(UsbDevices.FirstOrDefault());
                if (SerialPortHelper.CheckError(r, showDialog: false))
                {
                    r = SerialPortHelper.Open(UsbDevices.FirstOrDefault(), serialOption);
                    if (SerialPortHelper.CheckError(r, showDialog: false))
                    {
                        //Opened Successfully
                        Log.Info("Open", "Opened Successfully");
                        IsOpen = true;
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        // Send();

                    } else
                    {
                        Log.Info("Open", "OPen Error");
                    }
                } else
                {
                    Log.Info("OPEN", "ERROR Request : {r}");
                }
            }
        }
        [RelayCommand]
        public void Close()
        {
            try
            {
                SerialPortHelper.Close();
                //  CycleToSend = false;
                IsOpen = false;
            }
            catch (Exception)
            {
            }

        }

        [RelayCommand]
        public void Send(String Data)
        {


            byte[] send = SerialPortHelper.GetBytes(Data, EncodingSend);
            if (send.Length == 0)
            {
                return;
            }
            string r = SerialPortHelper.Write(send);
            if (SerialPortHelper.CheckError(r))
            {
                if (EncodingSend == "HEX")
                {
                    //   AddLog(new SerialLog(SendData.ToUpper(), true));
                } else
                {
                    //  AddLog(new SerialLog(SendData, true));

                    _ = Shell.Current.DisplayAlert("Send:", SendData, "Ok");



                }

            } else
            {
                //   AddLog(new SerialLog(r, true));
                _ = Shell.Current.DisplayAlert("Send Error!", r, "Ok");
            }

        }

        private void TryParseInput(string input)
        {
            Log.Debug("Received", "${input}");
            Estate.IsSmallLight_Enabled = true;
            if (_actions.TryGetValue(input, out Action value))
            {
                value.Invoke();
            } else
            {
                Log.Debug("Try", "NotEqual");
                //Numerical Commands 
                ParseNumericalCommands(input);
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
                DisplayBatteryLevel = SubString_and_ToInt(input, ":");
            } else if (is_Temp)
            {
                DisplayEngineTemp = SubString_and_ToInt(input, ":");
            } else
            {
                await Shell.Current.DisplayAlert(input, "Failed To Parse Numerical Commands.", "OK");
            }


        }

        private int SubString_and_ToInt(String str, String SubValue)
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


        void ParseSerialCommands()
        {
            _actions = new Dictionary<string, Action>
            {
                {serial_In_Commands.InSerial_AlarmSilenced_cmd, async ()=>await DoEmergencyState(false) },
                {serial_In_Commands.InSerial_ALarmSourceIsMicro_cmd,()=>SetEngineStateValue(nameof(Estate.IsMeSirenSource_Enabled),false) },//Not Implemented.
                {serial_In_Commands.InSerial_HeadLightIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsHeadLight_Enabled),true) },
                {serial_In_Commands.InSerial_AlarmSourceIsUI_cmd,()=>SetEngineStateValue(nameof(Estate.IsMeSirenSource_Enabled),true) },//Not Implemented.
                {serial_In_Commands.InSerial_AllBlinkersIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsMultiblink_Enabled),false) },
                {serial_In_Commands.InSerial_AllBlinkersIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsMultiblink_Enabled),true) },
                {serial_In_Commands.InSerial_HeadBlinkIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsHeadBlink_Enabled),false)},
                {serial_In_Commands.InSerial_HeadBlinkIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsHeadBlink_Enabled),true)},
                {serial_In_Commands.InSerial_BlinkDanceIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsBlinkDance_Enabled),false)},
                {serial_In_Commands.InSerial_BlinkDanceIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsBlinkDance_Enabled),true)},
                {serial_In_Commands.InSerial_ENGINEisOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsENGINE_ON),false)},
                {serial_In_Commands.InSerial_ENGINEisON_cmd,()=>SetEngineStateValue( nameof(Estate.IsENGINE_ON),true)},
                {serial_In_Commands.InSerial_HeadLightIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsHeadLight_Enabled),false)},
                {serial_In_Commands.InSerial_LeftTurnIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsLeftTurn_Enabled),false)},
                {serial_In_Commands.InSerial_LeftTurnIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsLeftTurn_Enabled),true)},
                {serial_In_Commands.InSerial_PoliceLightsIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsPOliceLight_Enabled),false)},
                {serial_In_Commands.InSerial_PoliceLightsIsOn_cmd,()=>SetEngineStateValue( nameof(Estate.IsPOliceLight_Enabled),true)},
                {serial_In_Commands.InSerial_RightTurnIsOFF_cmd,()=>SetEngineStateValue( nameof(Estate.IsRightTurn_Enabled),false)},
                {serial_In_Commands.InSerial_RightTurnIsON_cmd,()=>SetEngineStateValue( nameof(Estate.IsRightTurn_Enabled),true)},
                {serial_In_Commands.InSerial_ShakeDetected_cmd,async ()=>await DoEmergencyState(true)},//Not Implemented.
              //  {serial_In_Commands.InSerial_SirenIsOFF_cmd,()=>Function1()},//Not Implemented.
               // {serial_In_Commands.InSerial_SirenIsOn_cmd,()=>Function1()}, //Not Implemented.
                //Startup message.
                {serial_In_Commands.InSerial_STARTUP_cmd,()=>Function2() }//Not Implemented.


            };
        }
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
        //
        /// <summary>
        /// Timer For GPS Location Updates.
        /// </summary>

        /// 
        private async void TimerGps_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //
           
            

              await aLocationManager.GetDeviceLocation();
            
        }

        private  async Task DoEmergencyState(bool Emergency)
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


        void Function2()
        {
            Log.Debug("Function2", "HelloFromFunc2");
            Estate.IsSmallLight_Enabled = false;
            SmallLightclr = Estate.SmallLightColor;
        }


        //Location











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
}
