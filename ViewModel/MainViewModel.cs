/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiUIpro.                                                **  
** Description:                                                                  **
** Serial Input | Output pre-defined Commands.                                   **
** Used For Communicate   Between UI and MicroController.                        **
** Created in sat 1403/01/025 18:40 PM By Hosein Pirani                          **
**  Modified In fri 1403/02/18 16:00 PM To 18:00 by hosein pirani                **
**  :GPS-SMS-Fonts.                                                              **
**                                                                               **
** TODO:Complete Siren Player.                                                   **
** TODO: Complete Serial Functions in TryParse()                                 **
** Serial functions                                                              **
** Event Handler For Them,State File writer ,GPS Speedometer,locator And sender  **
** And  LOT OF CODE @_@                                                          **
** ...                                                                           **  
**                                                                               **
**                                                                               **
**                                                                               **
 *********************************************************************************/






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
//using Microsoft.Maui.Controls;


namespace HPISMARTUI.ViewModel
{
    public partial class MainViewModel : ObservableObject//, IQueryAttributable
    {
        [ObservableProperty]
        ENGINEstate estate = new ENGINEstate();
        AndroidLocationManager aLocationManager = new AndroidLocationManager();
        public Serial_OUTCommands serial_out_Command = new();
        Serial_InCommands serial_In_Commands = new();
        [ObservableProperty]
        public String backgroundSource = new("bg_b.png");
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
        //



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
        //GPS Location
        [ObservableProperty]
        private String deviceLocation;
        private int GpsUpdate_TimerInterval = 3;
        System.Timers.Timer TimerGps;



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
        SerialOption serialOption = new();
        //USB
        public ObservableCollection<UsbDeviceInfo> UsbDevices { get; } = new();
        //Receive Buffer
        public string receivedata = "";
        //Send Buffer
        [ObservableProperty]
        string sendData = "Hello!";//Just For Test With SendEntryValue Command.
        //Input Serial Parser
        private Dictionary<string, Action> _actions;
        private List<String> SerialInCommandList = new();




        /// <summary>
        /// 
        /// <see cref="MainViewModel"/>
        /// 
        /// </summary>
        public MainViewModel()
        {

            //Emergency Call To Owner.
            TimerEmergency = new System.Timers.Timer(TimeSpan.FromSeconds(TemergencyInterval));
            TimerEmergency.Elapsed += timerEmergency_Elapsed;
            TimerEmergency.Enabled = false;
            //GPS Timer For Continous Location Update.
            TimerGps = new System.Timers.Timer(TimeSpan.FromSeconds(GpsUpdate_TimerInterval));
            TimerGps.Elapsed += timerGps_Elapsed;
            TimerGps.Enabled = false;

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

        // //////////////////////SMS Methods


        private void sendOutgoingSms()
        {
            String phoneNumber = mPhoneNumber;
            if (String.IsNullOrEmpty(phoneNumber))
            {
                Log.Debug("sendOutgoingSms", "Couldn't get phone number from view! Ignoring request...");
                return;
            }
            if (mIsReadPhoneStateGranted)
            {

                //SmsManager m = SmsManager.Default;
                SmsManager.Default.SendTextMessage(phoneNumber, null, SmsMessage,
                        PendingIntent.GetBroadcast(AndroidPlatform.CurrentActivity, sMessageId, getSendStatusIntent(), 0),
                        null);
                Log.Debug("sendOutgoingSms", "SmsManager called");
                sMessageId++;
            }
        }

        private void sendOutgoingSmsService()
        {
            Log.Debug("sendOutgoingSmsService", "begin");
            String phoneNumber = mPhoneNumber;
            if (TextUtils.IsEmpty(phoneNumber))
            {
                Log.Debug("sendOutgoingSmsService", "Couldn't get phone number from view! Ignoring request...");
                return;
            }
            if (mIsReadPhoneStateGranted)
            {
                Intent sendSmsIntent = new Intent(nameof(SmsManagerTestService));
                sendSmsIntent.PutExtra(SmsManagerTestService.EXTRA_SEND_TEXT, SmsMessage);
                sendSmsIntent.PutExtra(SmsManagerTestService.EXTRA_SEND_NUMBER, phoneNumber);
                sendSmsIntent.PutExtra(SmsManagerTestService.EXTRA_SEND_INTENT,
                        PendingIntent.GetBroadcast(AndroidPlatform.CurrentActivity.BaseContext, sMessageId, getSendStatusIntent(), 0));
                sendSmsIntent.SetComponent(new ComponentName(AndroidPlatform.CurrentActivity.BaseContext, nameof(SmsManagerTestService)));
                AndroidPlatform.CurrentActivity.BaseContext.StartService(sendSmsIntent);
                sMessageId++;
                Log.Debug("sendOutgoingSmsService", "called");
            }
        }

        private void getSubIdForResult()
        {
            // ask the user for a default SMS SIM.
            Intent intent = new Intent();
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
                Log.Debug("getSubIdForResult", "Unable to launch Settings application.");
            }
        }

        private void setPersistentServiceComponentEnabled()
        {
            var serviceIntent = new Intent(AndroidPlatform.CurrentActivity.BaseContext, typeof(PersistentService));
            AndroidPlatform.CurrentActivity.BaseContext.StartService(serviceIntent);
            AndroidPlatform.CurrentActivity.PackageManager.SetComponentEnabledSetting(
                    new ComponentName(AndroidPlatform.CurrentActivity.BaseContext, nameof(PersistentService)),
                        ComponentEnabledState.Enabled,
                        ComponentEnableOption.DontKillApp);
        }

        private void setPersistentServiceComponentDisabled()
        {
            AndroidPlatform.CurrentActivity.PackageManager.SetComponentEnabledSetting(
            new ComponentName(AndroidPlatform.CurrentActivity.BaseContext, nameof(PersistentService)),
                ComponentEnabledState.Disabled,
                ComponentEnableOption.DontKillApp);
        }

        private void checkSingleRegPermission()
        {
            if (AndroidPlatform.CurrentActivity.CheckSelfPermission(PERFORM_IMS_SINGLE_REGISTRATION)
                    == Permission.Granted)
            {
                Log.Debug("checkSingleRegPermission", "Single Reg permission granted");
            } else
            {

                Log.Debug("checkSingleRegPermission", "Single Reg permission NOT granted");
            }

        }

        public Intent getSendStatusIntent()
        {
            // Encode requestId in intent data
            Log.Debug("getSendStatusIntent", "Called.");
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


        [RelayCommand]
        public async Task SMSAsync()
        {



            //    getPhoneNumber();
            // getSubIdForResult();
            //   await Task.Delay(1000);
            await TextToSpeech.SpeakAsync("Hello everybody!");
            sendOutgoingSms();
            //   await Task.Delay(100);
            //  setPersistentServiceComponentEnabled();
            //   await Task.Delay(100);
            //   sendOutgoingSmsService();
            //    await Task.Delay(100);
            //   checkSingleRegPermission();

            //   SmsManager.Default.SendTextMessage("+989379223570", null, "Hello Xamarin This is My Test SMS", null, null);


        }

        [RelayCommand]
        public async  void GetLastLocation()
        {
            //WeakReferenceMessenger.Default.Send(new Messages.EngineState_HeadLightMessage(true));
           // await aLocationManager.GetLastLocation();
          //  Get_LastLocation();
         //   await Task.Delay(TimeSpan.FromSeconds(5));
            Get_CurrentLocation();
            
            
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
                Open();
            } else
            {
                Close();
            }


        }


        [RelayCommand]
        public async void Open()
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
            // sendOutgoingSms();
            //  sendOutgoingSmsService();
            //   _ = Shell.Current.DisplayAlert("SMS:","Sent" + SmsMessage , "Ok");

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
                {serial_In_Commands.InSerial_AlarmSilenced_cmd, async ()=>await doEmergencyState(false) },
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
                {serial_In_Commands.InSerial_ShakeDetected_cmd,async ()=>await doEmergencyState(true)},//Not Implemented.
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
        private void timerEmergency_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Call Emergency!
            TimerEmergency.Stop();
            EmergencyMessageSent = true;
        }
        //
        /// <summary>
        /// Timer For GPS Location Updates.
        /// </summary>

        /// 
        private  void timerGps_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //
           
            

             //   DeviceLocation = aLocationManager.GetLastLocation().Result;
            
        }

        private  async Task doEmergencyState(bool Emergency)
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

    public class EngineState_HeadLightMessage : ValueChangedMessage<bool>
    {
        public EngineState_HeadLightMessage(bool state) : base(state)
        {
        }
    }

    public class DeviceLocationMessage : ValueChangedMessage<String>
    {
        public DeviceLocationMessage(String value) : base(value)
        {

        }
    }
}
