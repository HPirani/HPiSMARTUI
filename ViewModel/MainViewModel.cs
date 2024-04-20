/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiUIpro.                                                **  
** Description:                                                                  **
** Serial Input | Output pre-defined Commands.                                   **
** Used For Communicate   Between UI and MicroController.                        **
** Created in sat 1403/01/025 18:40 PM By Hosein Pirani                          **
**  Modified In fri 1403/01/31 16:00 PM To 19:05 by hosein pirani                **
**  :SerialInCommands,EngineState.                                               **
**                                                                               **
** TODO:Complete Siren Player.                                        **
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

//using Microsoft.Maui.Controls;


namespace HPISMARTUI.ViewModel
{
    public partial class MainViewModel : ObservableObject//, IQueryAttributable
    {
        [ObservableProperty]
        ENGINEstate estate = new ENGINEstate();

        public Serial_OUTCommands serial_out_Command = new();
        Serial_InCommands serial_In_Commands = new();
        [ObservableProperty]
        public String backgroundSource = new("bg_b.png");













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
        //

        //Emergency 
        //SirenPlayer
        readonly IAudioManager audioManager;

        System.Timers.Timer timerEmergency;
        [ObservableProperty]
        private int temergencyInterval=15;//
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
        string sendData = "Hello!";
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
            timerEmergency = new System.Timers.Timer(TimeSpan.FromSeconds(TemergencyInterval));
            timerEmergency.Elapsed += timerEmergency_Elapsed;
            timerEmergency.Enabled = false;


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

        //XamlCommands
        [RelayCommand]
        public void SmallLight()
        {


        }
        //

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
        public void Send()
        {

            byte[] send = SerialPortHelper.GetBytes(SendData, EncodingSend);
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
                DisplayFuelLevel  = SubString_and_ToInt(input, ":");
            } else if (is_BatteryLevel)
            {
                DisplayBatteryLevel = SubString_and_ToInt(input, ":");
            } else if (is_Temp)
            {
                DisplayEngineTemp =  SubString_and_ToInt(input, ":");
            } else
            {
                await Shell.Current.DisplayAlert(input, "Failed To Parse Numerical Commands.", "OK");
            }


        }

        private int SubString_and_ToInt(String str,String SubValue)
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

        private void SetEngineStateValue( String  field, bool value)
        {
            switch(field)
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
                case nameof( Estate.IsRightTurn_Enabled):
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

        private void timerEmergency_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Call Emergency!
            timerEmergency.Stop();
            EmergencyMessageSent = true;
        }

        private  async Task doEmergencyState(bool Emergency)
        {
            if (Emergency)
            {
                timerEmergency.Enabled = true;
                if (Estate.IsMeSirenSource_Enabled)
                {
                    PlaySirenSound();
                }

            }else
            {
                
                if (timerEmergency.Enabled)
                {
                    
                    timerEmergency.Enabled = false;

                }
                if (EmergencyMessageSent)
                {
                    EmergencyMessageSent= false;
                    //Send ForgiveMe Message :( 
                }
            }

        }


        private async Task PlaySirenSound()
        {
            var SirenPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("ukelele.mp3"));

            SirenPlayer.Play();

        }
        private void StopSirenSound()
        {
        }

        void Function2()
        {
            Log.Debug("Function2", "HelloFromFunc2");
            Estate.IsSmallLight_Enabled = false;
            SmallLightclr = Estate.SmallLightColor;
        }


    }
}
