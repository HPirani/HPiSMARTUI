/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUi                                               **  
** Description:                                                                  **
**                                                                               **
**                                                                               **
** Created in sat 1403/02/025 18:40 PM By Hosein Pirani                          **
**                                                                               **
** Modified In sat 1403/05/17 07:40 PM To 20:05 by me.                           **
** : Settings Added, Emergency Improved, Major Fixes.                            **
** TODO: Test All Methods.                                                       **
** TODO:                                                                         **
** ..                                                                            **
** ...                                                                           **
** And  LOT OF CODE @_@                                                          **
** .....                                                                         **  
** ........                                                                      **
** ...........                                                                   **
** ...............                                                               **
 *********************************************************************************/





using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

#region Constructor
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
        ////////////////////////////////////////////
        //Bike Speed Retrived From GPS;
        //Send To UI via WeakReferenceMessenger.Or Directly Via Xaml Binding.
        [ObservableProperty]
        private static double bikeSpeed  = 999.999d;

        
        // private double speed_MetersPerHours;

        public AndroidLocationManager(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            GPSLocationRequestAccuracy = _settingsService.GPSLocationAccuracy;
            GPSRequestinterval = _settingsService.GPSLocationRequestInterval;
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
                    } else
                    {
                        Log.Debug("ALocationCommandParser", "Im Just Listening...");
                    }
                    
                    break;
                case LCommand.STOP_LISTENING:
                    StopListening();
                    break;
                case LCommand.GET_LAST_LOCATION:
                        await GetDeviceLocation();
                    break;

                default:
                    break;
            }
            Log.Debug("ALocationManager", "Messenger: Received Location Message: " + command.ToString());
        }

#endregion


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
                    Log.Debug("ALocationManager", "get_CurrentLocation requested.");

                    cts = new CancellationTokenSource();
                    var location = await Geolocation.GetLocationAsync(request, cts.Token);
                    if (location is null)
                    {
                        
                        Log.Debug("ALocationManager", $"Can't Get Location With {SingleRequestAccuracy}.");
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
                        Log.Debug("ALocationManager", $"Latitude:  {location.Latitude}");
                        Log.Debug("ALocationManager", $"Longitude: {location.Longitude}");
                        CurrentLocation = FormatLocation(location);
                        break;
                        
                    }
                }
                 
            }
            catch (Exception ex)
            {
                CurrentLocation = FormatLocation(null, ex);
                Log.Debug("ALocationManager ", "get_CurrentLocation exeption: " + ex.Message);
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

                 
              Log.Debug("ALocationListener", success ? "Started listening for foreground location updates" : "Couldn't start listening");
               
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
            RawLocation = e.Location;
            CurrentLocation = FormatLocation(e.Location);
            //Send RawLocation To MainViewModel.
            Log.Debug("ALocationListener", CurrentLocation);
            //If Settings Were Changed.
            if ((_settingsService.GPSLocationRequestInterval != GPSRequestinterval) || (_settingsService.GPSLocationAccuracy != GPSLocationRequestAccuracy ))
            {
                Log.Debug("ALocationListener", $"Settings Changed : Accuracy {(GeolocationAccuracy)GPSLocationRequestAccuracy} , Interval {GPSRequestinterval} To : newAccuracy: {(GeolocationAccuracy)_settingsService.GPSLocationAccuracy} , newInterval: {_settingsService.GPSLocationRequestInterval}");
                StopListening();
                Task.Delay(GPSRequestinterval);//Wait For Stop Listening. TODO: TEST IT.
                GPSRequestinterval = _settingsService.GPSLocationRequestInterval;
                GPSLocationRequestAccuracy = _settingsService.GPSLocationAccuracy;
                StartListening();
            }
        }

     public void StopListening()
        {
            try
            {
                if (cts != null && !cts.IsCancellationRequested)
                    cts.Cancel();
                Geolocation.LocationChanged -= Geolocation_LocationChanged;
                Geolocation.StopListeningForeground();
                Log.Debug("ALocationListener", "Stopped listening for foreground location updates");
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
                Log.Debug("ALocationManager: ", $"null: {ex.Message}");
                return "Error";
            }
            Log.Debug("ALocationManager: ", "formatingData...");
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
            stringBuilder.Append((location.IsFromMockProvider ? "Yes." : "No." ));
      
       
         return stringBuilder.ToString();
        }


    }
}
