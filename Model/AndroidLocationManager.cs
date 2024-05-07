using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Util;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using HPISMARTUI.Messages;
using HPISMARTUI.ViewModel;

using Javax.Xml.Transform;



namespace HPISMARTUI.Model
{
    
    public partial class AndroidLocationManager : ObservableObject
    {
        private enum LResult
        {
            Success,
            FailedDueError,
            Failed
        }


        string notAvailable = "not available";
        CancellationTokenSource cts;
        [ObservableProperty]
        public string retrivedLocation = "Waiting For Call...";
        [ObservableProperty]
        private string currentLocation;
        [ObservableProperty]
        int accuracy = (int)GeolocationAccuracy.Default;
        [ObservableProperty]
        public string listeningLocation;
        [ObservableProperty]
        public string listeningLocationStatus;
        ////////////////////////////////////////////
        //Bike Speed Retrived From GPS;
        //Send To UI via WeakReferenceMessenger.Or Directly Via Xaml Binding.
        [ObservableProperty]
        private static double bikeSpeed = 999.999;

        private double speed_MetersPerMinute;
       // private double speed_MetersPerHours;

        private async Task<LResult> Get_LastLocation()
        {
            // if (IsBusy)
            //    return;

            //  IsBusy = true;
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
            //    if (location == null)
              //  {
                //    CurrentLocation = "Chached Failed.";
                   // return LResult.Failed;
            //    }
                CurrentLocation = FormatLocation(location);
                
                //   RetrivedLocation = LastLocation;
            }
            catch (Exception ex)
            {
                CurrentLocation = FormatLocation(null, ex);
                return LResult.FailedDueError;
                //RetrivedLocation = LastLocation;
            }
            //   IsBusy = false;
            return LResult.Success;

        }

        async Task<LResult> Get_CurrentLocation()
        {
            // if (IsBusy)
            //    return;

            //  IsBusy = true;
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Default);
                Log.Debug("get_CurrentLocation: ", "requested");
               // DeviceLocation = "requested";
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);
                Log.Debug("Latitude: ", location.Latitude.ToString());
                Log.Debug("Longitude: ", location.Longitude.ToString());
                //  LastLocation = "Latitude: " + location.Latitude.ToString() + "Longitude: " + location.Longitude.ToString();
               // if (location == null)
               // {
              //      return LResult.Failed;
               // }
                CurrentLocation = FormatLocation(location);
                //DeviceLocation = CurrentLocation;
            }
            catch (Exception ex)
            {
                CurrentLocation = FormatLocation(null, ex);
                Log.Debug("get_CurrentLocation: ", "exeption: " + ex.Message);
                return LResult.FailedDueError;
               // DeviceLocation = CurrentLocation;
            }
            finally
            {
                cts.Dispose();
                cts = null;
            }
            return LResult.Success;
            // IsBusy = false;
        }


        async void GetDeviceLocation()
        {
            var actualLoc = await Get_CurrentLocation();
            if (actualLoc != LResult.Success)
            {
                await Get_LastLocation();
            }
          
            
            WeakReferenceMessenger.Default.Send(new Messages.DeviceLocationMessage(CurrentLocation));

        }



        string FormatLocation(Location location, Exception ex = null)
        {
            if (location == null)
            {
                Log.Debug("FormatLocation: ", "null!");
                return $"Unable to detect location. Exception: {ex?.Message ?? string.Empty}";
            }
            Log.Debug("FormatLocation: ", "formatingData...");
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Latitude: ");
            stringBuilder.Append(location.Latitude.ToString());
            stringBuilder.Append(" , Longitude: ");
            stringBuilder.Append(location.Longitude.ToString());
            stringBuilder.Append(" , Accuracy: ");
            stringBuilder.Append(location.Accuracy.ToString());
            stringBuilder.Append(" , Altitude: ");
            stringBuilder.Append((location.Altitude.HasValue ? location.Altitude.Value.ToString() : notAvailable));
            stringBuilder.Append(" , AltitudeRef: ");
            stringBuilder.Append(location.AltitudeReferenceSystem.ToString());
            stringBuilder.Append(" , VerticalAccuracy: ");
            stringBuilder.Append((location.VerticalAccuracy.HasValue ? location.VerticalAccuracy.Value.ToString() : notAvailable));
            stringBuilder.Append(" , Course: ");
            stringBuilder.Append((location.Course.HasValue ? location.Course.Value.ToString() : notAvailable));
            stringBuilder.Append(" , Speed: ");
            stringBuilder.Append((location.Speed.HasValue ? location.Speed.Value.ToString() : notAvailable));
            stringBuilder.Append(" , Date: ");
            //stringBuilder.Append(location.Timestamp.Date.ToString());
           // stringBuilder.Append(",");
            stringBuilder.Append(location.Timestamp.LocalDateTime.ToString());
            stringBuilder.Append(" , IsMock: ");
            stringBuilder.Append((location.IsFromMockProvider ? "Yes." : "No." ));
            // Speed
            if (location.Speed.HasValue)
            {

                speed_MetersPerMinute = location.Speed.Value * 60.0f;
               // speed_MetersPerHours = speed_MetersPerMinute * 60.0f;
                //BikeSpeed = speed_MetersPerHours  / 1000.0f; // divide to 1000 (Meters Per KM)
                BikeSpeed = speed_MetersPerMinute * 0.06f; // Equal With: speed_MetersPerMinute * 60 / 1000
            }
/*            return
                $"Latitude: {location.Latitude}\n" +
                $"Longitude: {location.Longitude}\n" +
                $"HorizontalAccuracy: {location.Accuracy}\n" +
                $"Altitude: {(location.Altitude.HasValue ? location.Altitude.Value.ToString() : notAvailable)}\n" +
                $"AltitudeRefSys: {location.AltitudeReferenceSystem.ToString()}\n" +
                $"VerticalAccuracy: {(location.VerticalAccuracy.HasValue ? location.VerticalAccuracy.Value.ToString() : notAvailable)}\n" +
                $"Heading: {(location.Course.HasValue ? location.Course.Value.ToString() : notAvailable)}\n" +
                $"Speed: {(location.Speed.HasValue ? location.Speed.Value.ToString() : notAvailable)}\n" +
                $"Date (UTC): {location.Timestamp:d}\n" +
                $"Time (UTC): {location.Timestamp:T}\n" +
                $"Mocking Provider: {location.IsFromMockProvider}";*/
         return stringBuilder.ToString();
        }


    }
}
