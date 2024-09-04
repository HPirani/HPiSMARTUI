using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Hoho.Android.UsbSerial.Driver;
using System.Threading.Tasks;
using Android.Hardware.Usb;

namespace HPISMARTUI.Model
{
    

    public class UsbDeviceInfo
    {
        public UsbDevice Device
        {
            get; set;
        }
        public IUsbSerialDriver Driver
        {
            get; set;
        }
        public string DriverName
        {
            get; set;
        }
    }

   // [ObservableObject]
    public partial class SerialOption
    {
       // [ObservableProperty]
        public int baudRate = 115200;
       // [ObservableProperty]
        public int dataBits = 8;
      //  [ObservableProperty]
        string stopBitsName = StopBits.One.ToString();
        public StopBits StopBits => Enum.Parse<StopBits>(stopBitsName);
     //   [ObservableProperty]
        string parityName = Parity.None.ToString();
        public Parity Parity => Enum.Parse<Parity>(parityName);
    }



}
