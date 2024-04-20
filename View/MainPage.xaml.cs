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
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Hardware.Usb;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Hoho.Android.UsbSerial.Extensions;
using Hoho.Android.UsbSerial.Util;
using Org.Xmlpull.V1.Sax2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Android.OS;
using static Android.Renderscripts.ScriptGroup;
using HPISMARTUI.Model;
using HPISMARTUI.Helper;
using HPISMARTUI.ViewModel;
using System.Collections;
using Microsoft.Extensions.FileProviders;
using Android.Content.Res;

namespace HPISMARTUI.View
{

    public partial class MainPage : ContentPage
        {
        MainViewModel vm;
        
     
        

        public IEnumerable<string> ListAssets(string subfolder)
        {
            AssetManager assets = Platform.AppContext.Assets;
            string[] files = assets.List(subfolder);
            return files;
        }


        public MainPage( MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = vm = viewModel;
        
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!SerialPortHelper.UsbFeatureSupported())
            {
                await Shell.Current.DisplayAlert("Error", "USB OTG Not Supported", "ok");
                System.Environment.Exit(0);
                return;
            }
            vm.GetUsbDevices();

            // Random Background Image.Changes EVERY DAY.


        }


    }
    }