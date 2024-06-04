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
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Core.Primitives;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Kotlin.Properties;
using Plugin.Maui.ScreenBrightness;

namespace HPISMARTUI.View;


//[INotifyPropertyChanged]
public partial class SplashPage : ContentPage 
{
    

    SplashViewModel vm;
    public SplashPage(SplashViewModel viewModel/*, IScreenBrightness screenBrightness*/)
    {
        InitializeComponent();
        BindingContext = vm = viewModel;
        // SplashVideoPlayer.PropertyChanged += OnVideoPlayerEvent;
       // screenBrightness.Brightness = 100;
        ScreenBrightness.Default.Brightness = 100;
    }

    // void OnVideoPlayerEvent(object? sender, PropertyChangedEventArgs e)
    // {
    //     if(e.PropertyName == MediaElement.CurrentStateProperty)
    //  }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async void OnAppearing()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        base.OnAppearing();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        SplashVideoPlayer.Stop();
        SplashVideoPlayer.Handler?.DisconnectHandler();
        Shell.Current.Navigation.RemovePage(this);
       

    }

}
