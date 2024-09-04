/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUi                                               **  
** Description:                                                                  **
**                                                                               **
**                                                                               **
** Created in sat 1403/02/25 6:40 PM By Hosein Pirani                            **
**                                                                               **
** Modified In Wed 1403/05/31 02:45 PM To  7:15 by me.                           **
** :                            Minor Fixes.                                     **
** TODO: Test All Methods.                                                       **
** TODO:                                                                         **
** ..                                                                            **
** ...                                                                           **
** And CODE                                                                      **
** ..... More Code                                                               **  
** ........ Code                                                                 **
** ...........  #_#                                                              **
** ...............                                                               **
 *********************************************************************************/





using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Android.App;
using Android.Content.Res;



//using Android.Locations;
using Android.Util;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using HPISMARTUI.Messages;
using HPISMARTUI.ViewModel;

using IntelliJ.Lang.Annotations;

using Javax.Xml.Transform;



namespace HPISMARTUI.Services
{
    
    public partial class AndroidLocationManager : ObservableObject
    {
#region Enums


        private enum LResult
        {
            Success,
            FailedDueError,
            Failed
        }
        public enum LCommand
        {
            START_LISTENING,
            STOP_LISTENING,
            GET_LAST_LOCATION
        }

#endregion

#region GlobalOptions
        readonly ISettingsService _settingsService;
        GeolocationAccuracy SingleRequestAccuracy = GeolocationAccuracy.Best;
        readonly string notAvailable = "not available";
        CancellationTokenSource cts;
        [ObservableProperty]
        private string currentLocation;
       public Location RawLocation = new();//RawLocation For Speed , Acceleration etc.
        [ObservableProperty]
        int gPSLocationRequestAccuracy;
        [ObservableProperty]
        int gPSRequestinterval;
        public bool IsListening => Geolocation.IsListeningForeground;
        public bool IsNotListening => !IsListening;
        System.Timers.Timer TimerReset;//Timer For Check the Location Update.
        [ObservableProperty]
        private int timerResetInterval = 3;//Interval in Seconds.
        public string Logchache
        {
            get;
            private set;
        }

        ////////////////////////////////////////////
        //Bike Speed Retrived From GPS;
        //Send To UI via WeakReferenceMessenger.Or Directly Via Xaml Binding.
        [ObservableProperty]
        private static double bikeSpeed  = 999.999d;

#endregion


#region Constructor_Distructor       

        public AndroidLocationManager(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            GPSLocationRequestAccuracy = _settingsService.GPSLocationAccuracy;
            GPSRequestinterval = _settingsService.GPSLocationRequestInterval;
            TimerResetInterval = _settingsService.TimerResetInterval;
        }
        ~AndroidLocationManager()
        {
            TimerReset.Enabled=false;
            TimerReset?.Dispose();
        }

#endregion

#region PublicMethods

        public async Task ProcessCommand(LCommand command)
        {
            switch (command)
            {
                case LCommand.START_LISTENING:
                    if (IsNotListening)
                    {
                        StartListening();
                        //Create Timer.
                        TimerReset = new System.Timers.Timer(TimeSpan.FromSeconds(TimerResetInterval));
                        TimerReset.Elapsed += OnTimerReset;
                        TimerReset.AutoReset = false;
                        TimerReset.Enabled = false;
                    } else
                    {
                        Ld( "Im Just Listening...","ALocationCommandParser");
                    }
                    
                    break;
                case LCommand.STOP_LISTENING:
                    StopListening();
                    //StopTimer.
                    TimerReset.Enabled = false;
                    break;
                case LCommand.GET_LAST_LOCATION:
                        await GetDeviceLocation();
                    break;

                default:
                    break;
            }
            Ld( $"Messenger: Received Location Message: {command}"  , "ProcessCommand");
        }

        #endregion


#region Methods
        private async Task<LResult> Get_LastLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                CurrentLocation = FormatLocation(location);
            }
            catch (Exception ex)
            {
                CurrentLocation = FormatLocation(null, ex);
                return LResult.FailedDueError;
                
            }
            return LResult.Success;

        }

        async Task<LResult> Get_CurrentLocation()
        {
            try
            {
                
                for (var i = 0; i <= 5; i++)
                {

                    var request = new GeolocationRequest(SingleRequestAccuracy);
                    Ld( "get_CurrentLocation requested.", "Get_CurrentLocation");

                    cts = new CancellationTokenSource();
                    var location = await Geolocation.GetLocationAsync(request, cts.Token);
                    if (location is null)
                    {
                        
                        Ld( $"Can't Get Location With {SingleRequestAccuracy}.","Get_CurrentLocation");
                        SingleRequestAccuracy = SingleRequestAccuracy switch
                        {//Try All Modes
                            GeolocationAccuracy.Default => GeolocationAccuracy.Lowest,
                            GeolocationAccuracy.Lowest => GeolocationAccuracy.Low,
                            GeolocationAccuracy.Low => GeolocationAccuracy.Medium,
                            GeolocationAccuracy.Medium => GeolocationAccuracy.High,
                            GeolocationAccuracy.High => GeolocationAccuracy.Best,
                            GeolocationAccuracy.Best => GeolocationAccuracy.Default,
                            _ => GeolocationAccuracy.Default
                        };
                    } else//Located Successfully So Break.
                    {
                        Ld( $"Latitude:  {location.Latitude}","Get_CurrentLocation");
                        Ld( $"Longitude: {location.Longitude}","Get_CurrentLocation");
                        CurrentLocation = FormatLocation(location);
                        break;
                        
                    }
                }
                 
            }
            catch (Exception ex)
            {
                CurrentLocation = FormatLocation(null, ex);
                Ld( "get_CurrentLocation exeption: " + ex.Message);
                return LResult.FailedDueError;
             
            }
            finally
            {
                cts.Dispose();
                cts = null;
            }
            return LResult.Success;
            
        }

      public  async  Task GetDeviceLocation()
        {
            
            var actualLoc = await Get_CurrentLocation();
            if (actualLoc != LResult.Success)
            {
                await Get_LastLocation();
            }
          
            
            

        }

      public  async void StartListening()
        {
            try
            {
                Geolocation.LocationChanged += Geolocation_LocationChanged;

                var request = new GeolocationListeningRequest((GeolocationAccuracy)GPSLocationRequestAccuracy,TimeSpan.FromMilliseconds(GPSRequestinterval));

                var success = await Geolocation.StartListeningForegroundAsync(request);

                 
              Ld(success ? "Started listening for foreground location updates" : "Couldn't start listening", "ALocationListener");
               
            }
            catch (Exception ex)
            {
                CurrentLocation = FormatLocation(null, ex);
                //throw;
            }

            OnPropertyChanged(nameof(IsListening));
            OnPropertyChanged(nameof(IsNotListening));
           
        }

       /// <summary>
       /// Listen For Location changes.
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        void Geolocation_LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            if (TimerReset.Enabled)
            {
                Ld("Disabling Timer.");
                TimerReset.Enabled = false;
            }else
            {
                Ld("Enabling Timer.");
                TimerReset.Enabled = true;
            }
            RawLocation = e.Location;
            CurrentLocation = FormatLocation(e.Location);
            //Send RawLocation To MainViewModel.
            Ld( CurrentLocation, "GeolocChanged:");
            //If Settings Were Changed.
            if ((_settingsService.GPSLocationRequestInterval != GPSRequestinterval) || (_settingsService.GPSLocationAccuracy != GPSLocationRequestAccuracy ))
            {
                Ld( $"Settings Changed : Accuracy {(GeolocationAccuracy)GPSLocationRequestAccuracy} , Interval {GPSRequestinterval} To : newAccuracy: {(GeolocationAccuracy)_settingsService.GPSLocationAccuracy} , newInterval: {_settingsService.GPSLocationRequestInterval}","ALocationListener");
                StopListening();
                Task.Delay(GPSRequestinterval);//Wait For Stop Listening. TODO: TEST IT.
                GPSRequestinterval = _settingsService.GPSLocationRequestInterval;
                GPSLocationRequestAccuracy = _settingsService.GPSLocationAccuracy;
                TimerResetInterval = _settingsService.TimerResetInterval;
                StartListening();
            }


        }

     public void StopListening()
        {
            //Stop Timer
            TimerReset.Enabled = false;
            
            try
            {
                if (cts != null && !cts.IsCancellationRequested)
                    cts.Cancel();
                Geolocation.LocationChanged -= Geolocation_LocationChanged;
                Geolocation.StopListeningForeground();
                Ld( "Stopped listening for foreground location updates","ALocationListener");
            }
            catch (Exception ex)
            {
                CurrentLocation = FormatLocation(null, ex);
                throw;
            }

            OnPropertyChanged(nameof(IsListening));
            OnPropertyChanged(nameof(IsNotListening));
        }


        string FormatLocation(Location location, Exception ex = null)
        {
            if (location == null)
            {
                Ld( $"null: {ex.Message}");
                return "Error";
            }
            Ld("formatingData...", "ALocationManager:");
            StringBuilder stringBuilder = new();
            stringBuilder.Append("Latitude: ");
            stringBuilder.Append(location.Latitude);
            stringBuilder.Append(" , Longitude: ");
            stringBuilder.Append(location.Longitude);
            stringBuilder.Append(" , Accuracy: ");
            stringBuilder.Append(location.Accuracy);
            stringBuilder.Append(" , Altitude: ");
            stringBuilder.Append((location.Altitude.HasValue ? location.Altitude.Value.ToString() : notAvailable));
            stringBuilder.Append(" , AltitudeRef: ");
            stringBuilder.Append(location.AltitudeReferenceSystem);
            stringBuilder.Append(" , VerticalAccuracy: ");
            stringBuilder.Append((location.VerticalAccuracy.HasValue ? location.VerticalAccuracy.Value.ToString() : notAvailable));
            stringBuilder.Append(" , Course: ");
            stringBuilder.Append((location.Course.HasValue ? location.Course.Value.ToString() : notAvailable));
            stringBuilder.Append(" , Speed: ");

            if (location.Speed.HasValue)
            {
                // speed_MetersPerHours = speed_MetersPerMinute * 60.0f;
                //BikeSpeed = speed_MetersPerHours  / 1000.0f; // divide to 1000 (Meters Per KM)
                BikeSpeed = location.Speed.Value * 3.6d; // Equal With: speed_MetersPerMinute * 60 / 1000
            } else
            {
                BikeSpeed = 0.0f;
            }
            stringBuilder.Append((location.Speed.HasValue ? BikeSpeed.ToString() : notAvailable));
            stringBuilder.Append(" , Date: ");
            stringBuilder.Append(location.Timestamp.LocalDateTime.ToString());
            stringBuilder.Append(" , IsMock: ");
            stringBuilder.Append((location.IsFromMockProvider ? "Yes." : "No."));


            return stringBuilder.ToString();
        }
        #endregion


#region Timers

        //Watchdog For Location Change. If Location were Not Changed More Than 3 Seconds it Meant Bike is Not Moving. So... 
      void OnTimerReset(object sender, System.Timers.ElapsedEventArgs e)
        {
         RawLocation.Speed = 0.0d;
         TimerReset.Enabled = false;
            Ld("TimerResetTimedOut.", "TimerReset@ALocationManager");
        }

#endregion


#region Logger

        public async void Ld(string message, string title = "ALocationManager", char type = 'd')
        {
            switch (type)
            {
                case 'e'://error
                    Log.Error(title, message);
                    break;
                case 'i'://info
                    Log.Info(title, message);
                    break;
                case 'd'://debug
                    Log.Debug(title, message);
                    break;
                case 'w':
                    Log.Warn(title, message);
                    break;
            }

            try
            {

                //using var stream = File.OpenWrite("HPiSmartUILog.txt");
                var docsDirectory = Android.OS.Environment.ExternalStorageDirectory;
                File.WriteAllText($"{docsDirectory.AbsoluteFile.Path}/HPlog.txt", string.IsNullOrEmpty(Logchache) ? $":{CurrentLocation}" : $"{Logchache} \r\n :{CurrentLocation}");


                var a = File.OpenRead($"{docsDirectory.AbsoluteFile.Path}/HPlog.txt");
                using StreamReader reader_1 = new(a);
                Logchache = await reader_1.ReadToEndAsync();
            }catch (Exception ex)
            {
                Log.Error("Ld Error", $"{ex.Message}, {ex.StackTrace}, {ex.InnerException}");
            }

        }

#endregion


    }
}
