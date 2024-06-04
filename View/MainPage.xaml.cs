/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiUIpro.                                                **  
** Description:                                                                  **
** Main Page Of HPISMARTUI                                                       **
**                                                                               **
** Created in tue 1403/01/014 12:00 PM By Hosein Pirani                          **
**                                                                               **
** Modified In sun 1403/01/55 16:00 PM To 19:05 by hosein pirani                 **
**                                                                               **
** TODO: YOU MUST CONVERT YOUR PROJECT TO MVVM.O_O.                              **
** TODO: Complete Serial Functions                                               **
** Serial functions                                                              **
** Event Handler For Them,State File writer ,GPS Speedometer,locator And sender  **
** And  LOT OF CODE @_@                                                          **
** ...                                                                           **  
**                                                                               **
**                                                                               **
**                                                                               **
 *********************************************************************************/


using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hoho.Android.UsbSerial.Driver;
using System.Collections.ObjectModel;
using Android;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Hardware;
using Android.Hardware.Usb;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Android.Content.PM;
using Android.OS;
using Android.Telephony;
using Android.Text;
using Hoho.Android.UsbSerial.Extensions;
using Hoho.Android.UsbSerial.Util;
using Org.Xmlpull.V1.Sax2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using static Android.Renderscripts.ScriptGroup;
using HPISMARTUI.Model;
using HPISMARTUI.Helper;
using HPISMARTUI.ViewModel;
using System.Collections;
using Microsoft.Extensions.FileProviders;
using HPISMARTUI.Services;
using AndroidPlatform = Microsoft.Maui.ApplicationModel.Platform;

namespace HPISMARTUI.View
{

    public partial class MainPage : ContentPage
        {
        MainViewModel vm;
      //  ContextWrapper ContextWrapper = AndroidPlatform.CurrentActivity;
        
        private static int REQUEST_PERMISSION_READ_STATE = 1;





        public MainPage( MainViewModel viewModel)
        {
            
            InitializeComponent();
            BindingContext = vm = viewModel;
            

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            

            if (AndroidPlatform.CurrentActivity.BaseContext.CheckSelfPermission(Manifest.Permission.ReadPhoneState)
         != Permission.Granted
         || AndroidPlatform.CurrentActivity.BaseContext.CheckSelfPermission(Manifest.Permission.SendSms)
                 != Permission.Granted)
            {
                vm.mIsReadPhoneStateGranted = false;
                AndroidPlatform.CurrentActivity.RequestPermissions(new String[]{Manifest.Permission.ReadPhoneState,
                    Manifest.Permission.SendSms}, REQUEST_PERMISSION_READ_STATE);
            } else
            {
               // await Shell.Current.DisplayAlert("SMS Permission","Granted","OK");

                vm.mIsReadPhoneStateGranted = true;
            }



            if (!SerialPortHelper.UsbFeatureSupported())
            {
                await Shell.Current.DisplayAlert("Error", "USB OTG Not Supported", "ok");
                System.Environment.Exit(0);
                return;
            }
            vm.GetUsbDevices();

            // Random Background Image.Changes EVERY DAY.


        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            AndroidPlatform.CurrentActivity.BaseContext.StopService(new Intent(AndroidPlatform.CurrentActivity.BaseContext, typeof(SmsManagerTestService)));
        }

        protected override  bool OnBackButtonPressed()
        {
            
            var uaction = Shell.Current.DisplayPromptAsync("Exit", "Exit?", "Yes", "No");
            if (uaction.Result.Equals("Yes"))
            {
                //System.Environment.Exit(0);
                App.Current.Quit();
            }
            base.OnBackButtonPressed();
            return true;
        }


    }
    }