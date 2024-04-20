using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hoho.Android.UsbSerial.Driver;
using Android.Hardware.Usb;
using Android.OS;
using Android.Content;
using Android.Util;

namespace HPISMARTUI
{
    public class SerialPortViewModel : IQueryAttributable
    {

        bool openIng = false;
        public ObservableCollection<UsbDeviceInfo> UsbDevices { get; } = new();
        SerialOption Option = new();
        public string receivedata = "";
        private Dictionary<string, Action> _actions;


       // [ObservableProperty]
        string encodingSend = "UTF-8";
        //[ObservableProperty]
        string encodingReceive = "UTF-8";
        public SerialPortViewModel()
        {
            SerialPortHelper.WhenUsbDeviceAttached((usbDevice) =>
            {
                GetUsbDevices();
            });

            SerialPortHelper.WhenDataReceived().Subscribe(data =>
            {
                //  string text;
                receivedata = SerialPortHelper.GetData(data, encodingReceive);
                AddLog(receivedata);

                TryParseInput(receivedata);


                //  Shell.Current.DisplayAlert("Received Data", text, "Ok");

            });

            SerialPortHelper.WhenUsbDeviceDetached((usbDevice) =>
            {
                GetUsbDevices();
            });



        }
        
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            GetUsbDevices();
        }
      //  [RelayCommand]
        public async Task GetUsbDevices()
        {
            SerialPortHelper.RequestPermissionAsync(UsbDevices.FirstOrDefault());
            UsbDevices.Clear();
            var list = SerialPortHelper.GetUsbDevices();
            foreach (var item in list)
            {
                UsbDevices.Add(item);
                //fix VirtualView cannot be null here
                await Task.Delay(50);
            }


            if (UsbDevices.Count > 0)
            {
                await Open(UsbDevices.FirstOrDefault());
            }

        }
     //   [RelayCommand]
        async Task Open(UsbDeviceInfo usbDeviceInfo)
        {

            SerialPortHelper.Open(usbDeviceInfo, Option);

        }


        private void TryParseInput(string input)
        {

            if (_actions.ContainsKey(input))
            {
                _actions[input].Invoke();


            } else

            {
                AddLog(("not equal"));
            }

        }
        void AddLog(String serialLog)
        {

            Task.Delay(50);
        }



    }



}
