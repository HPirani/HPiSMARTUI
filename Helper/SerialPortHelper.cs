using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Hardware.Usb;
using Android.Nfc;
using Android.Runtime;
using Android.Util;
//using CommunityToolkit.Mvvm.ComponentModel;
using Hoho.Android.UsbSerial.Driver;
using Hoho.Android.UsbSerial.Extensions;
using Hoho.Android.UsbSerial.Util;
using HPISMARTUI.Model;
using Org.Xmlpull.V1.Sax2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace HPISMARTUI.Helper
{

    public class SerialPortHelper
    {
        const int WRITE_WAIT_MILLIS = 1000;
        static Context context => Android.App.Application.Context;
        static UsbManager usbManager => (UsbManager)context.GetSystemService(Context.UsbService);
        static System.Timers.Timer timerData;
        static List<byte> listByteCache = new();
        static int interval = 50;
        static Subject<byte[]> dataSubject = new Subject<byte[]>();
        private static UsbSerialPort _port;
        private static SerialInputOutputManager serialIoManager;
        //Public
        public static IObservable<byte[]> WhenDataReceived() => dataSubject;
        //
        //Public Variables.
        public static bool IsDeviceOpen = false;//Flag For Current Serial State.

        public static bool UsbFeatureSupported()
        {
            bool r = context.PackageManager.HasSystemFeature("android.hardware.usb.host");
            return r;
        }
        public static List<UsbDeviceInfo> GetUsbDevices()
        {
            Log.Info("tag", "GetUsbDevices in Helper");
            var drivers = UsbSerialProber.GetDefaultProber().FindAllDrivers(usbManager);
            List<UsbDeviceInfo> r = new List<UsbDeviceInfo>();
            foreach (var item in usbManager.DeviceList.Values)
            {
                IUsbSerialDriver usbSerialDriver = drivers.Where(x => x.Device.DeviceId == item.DeviceId).FirstOrDefault();
                r.Add(new UsbDeviceInfo()
                {

                    Device = item,
                    Driver = usbSerialDriver,
                    DriverName = usbSerialDriver == null ? "" : usbSerialDriver.GetType().Name.Replace("SerialDriver", "")

                }); ;

                Log.Info("tag", "foreach in Helper" + r.FirstOrDefault());
            }
            return r;
        }
        public static async Task<string> RequestPermissionAsync(UsbDeviceInfo usbDeviceInfo)
        {
            Log.Info("RequestPermissionAsync", "requesting....");
            if (usbDeviceInfo.Driver == null)
            {
                Log.Info("RequestPermissionAsync", "No driver");
                return "No driver";
            }
            if (!await usbManager.RequestPermissionAsync(usbDeviceInfo.Device, context))
            {
                Log.Info("RequestPermissionAsync", "Request permission failed");
                return "Request permission failed";
            }
            Log.Info("RequestPermissionAsync", "null");
            return "";
        }

        public static string Open(UsbDeviceInfo usbDeviceInfo, SerialOption option)
        {
            timerData?.Stop();
            timerData = new System.Timers.Timer(interval);
            timerData.Enabled = false;
            timerData.AutoReset = false;
            timerData.Elapsed += TimerData_Elapsed; ;
            UsbDeviceConnection connection = usbManager.OpenDevice(usbDeviceInfo.Device);
            if (connection == null)
            {
                //   Log.Info("Open in helper", "Open failed");
                return "Connection falut";
            }
            _port = usbDeviceInfo.Driver.Ports.FirstOrDefault();
            if (_port == null)
            {
                //  Log.Info("Open in helper", "No port");
                return "No port";
            }
            serialIoManager = new SerialInputOutputManager(_port)
            {
                BaudRate = option.baudRate,
                DataBits = option.dataBits,
                Parity = option.Parity,
                StopBits = option.StopBits
            };
            serialIoManager.DataReceived += SerialIoManager_DataReceived;
            try
            {
                serialIoManager.Open(usbManager);
                Log.Info("serialIoManager", "Opening...");
                IsDeviceOpen = true;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
            return "";
        }

        public static void SetOption(SerialOption option)
        {
            if (serialIoManager != null)
            {
                _port.SetParameters(option.baudRate, option.dataBits, option.StopBits, option.Parity);
            }
        }

        private static void TimerData_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            cacheDataSend();
        }

        private static void SerialIoManager_DataReceived(object sender, SerialDataReceivedArgs e)
        {
            Log.Info("tag", "SerialIoManager_DataReceived");
            if (interval == 0)
            {
                dataSubject.OnNext(e.Data);
            }
            else
            {
                listByteCache.AddRange(e.Data);
                if (listByteCache.Count > 4098)
                {
                    timerData.Stop();
                    cacheDataSend();
                }
                else
                {
                    timerData.Stop();
                    timerData.Start();
                }

            }
        }
        static void cacheDataSend()
        {
            byte[] bytes = listByteCache.ToArray();
            listByteCache.Clear();
            dataSubject.OnNext(bytes);
        }
        public static void Close()
        {
            IsDeviceOpen = false;
            _port?.Close();
        }
        public  static string Write(byte[] data)
        {
            try
            {
                if (serialIoManager.IsOpen)
                {
                   _port.Write(data, WRITE_WAIT_MILLIS);
                    return "";
                }
                else
                {
                    Log.Info("Write", "Serial port is not open");
                    return "Serial port is not open";
                }
            }
            catch (Exception ex)
            {
                Log.Info("Write", "error:" + ex.Message);
                return "error:" + ex.Message;
            }
        }
        public static void IntervalChange(int interval)
        {
            if (timerData != null)
            {
                timerData.Interval = interval;
            }
        }

        public static bool CheckError(string error, string title = "fault", bool showDialog = true)
        {
            if (string.IsNullOrEmpty(error))
            {
                return true;
            }
            if (showDialog)
            {
                Shell.Current.DisplayAlert(title, error, "OK");
            }
            return false;
        }
        public static string GetData(byte[] data, string encodingName)
        {
            if (encodingName.ToUpper() == "HEX")
            {
                return ByteToHex(data);
            }
            else
            {
                return Encoding.GetEncoding(encodingName).GetString(data);
            }
        }

        public static byte[] GetBytes(string data, string encodingName)
        {
            if (encodingName.ToUpper() == "HEX")
            {
                return HexToByte(data);
            }
            else
            {
                return Encoding.GetEncoding(encodingName).GetBytes(data);
            }
        }

        public static byte[] HexToByte(string msg)
        {
            msg = msg.Replace(" ", "");
            if (msg.Length % 2 != 0)
            {
                msg = "0" + msg;
            }
            byte[] comBuffer = new byte[msg.Length / 2];
            for (int i = 0; i < msg.Length; i += 2)
            {
                comBuffer[i / 2] = Convert.ToByte(msg.Substring(i, 2), 16);
            }
            return comBuffer;
        }
        public static string ByteToHex(byte[] comByte)
        {
            StringBuilder sb = new StringBuilder();
            if (comByte != null)
            {
                for (int i = 0; i < comByte.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(comByte[i].ToString("X2"));
                }
            }
            return sb.ToString();
        }
        public static void WhenUsbDeviceDetached(Action<UsbDevice> action)
        {
            Log.Info("WhenUsbDeviceDetached", "deatached");
            var detachedReceiver = new UsbDeviceDetachedReceiver(action);
            context.RegisterReceiver(detachedReceiver, new IntentFilter(UsbManager.ActionUsbDeviceDetached));
        }
        public static void WhenUsbDeviceAttached(Action<UsbDevice> action)
        {
            Log.Info("WhenUsbDeviceAttached", "atached");
            var attachedReceiver = new UsbDeviceAttachedReceiver(action);
            context.RegisterReceiver(attachedReceiver, new IntentFilter(UsbManager.ActionUsbDeviceAttached));
        }
    }
    class UsbDeviceDetachedReceiver
           : BroadcastReceiver
    {
        readonly string TAG = typeof(UsbDeviceDetachedReceiver).Name;
        readonly Action<UsbDevice> action;

        public UsbDeviceDetachedReceiver(Action<UsbDevice> action)
        {
            this.action = action;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
            Log.Info(TAG, "USB device detached: " + device.DeviceName);
            action(device);
        }
    }

    class UsbDeviceAttachedReceiver
        : BroadcastReceiver
    {
        readonly string TAG = typeof(UsbDeviceAttachedReceiver).Name;
        readonly Action<UsbDevice> action;

        public UsbDeviceAttachedReceiver(Action<UsbDevice> action)
        {
            this.action = action;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var device = intent.GetParcelableExtra(UsbManager.ExtraDevice) as UsbDevice;
            Log.Info(TAG, "USB device attached: " + device.DeviceName);
            action(device);
        }
    }
}
