using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
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
        private enum LResult
        {
            Success,
            FailedDueError,
            Failed
        }

        readonly string notAvailable = "not available";
        CancellationTokenSource cts;
        [ObservableProperty]
        public string retrivedLocation = "Waiting For Call...";
        [ObservableProperty]
        private string currentLocation;
        [ObservableProperty]
        int accuracy = (int)GeolocationAccuracy.Default;
        public bool IsListening => Geolocation.IsListeningForeground;
        public bool IsNotListening => !IsListening;
        ////////////////////////////////////////////
        //Bike Speed Retrived From GPS;
        //Send To UI via WeakReferenceMessenger.Or Directly Via Xaml Binding.
        [ObservableProperty]
        private static double bikeSpeed = 999.999;

        private double speed_MetersPerMinute;
        // private double speed_MetersPerHours;

        public AndroidLocationManager()
        {
            //Messenger
            WeakReferenceMessenger.Default.Register<AndroidLocationManager, Messages.ALocationManagerCommunications>
                (this, async (recipient, message) =>
                {
                    //recipient.DeviceLocation = message.Value;
                    switch (message.Value)
                    {
                        case "StartListening":
                            StartListening();
                            break;
                        case "StopListening":
                            StopListening();
                            break;
                        case "GetLastLocation":
                          await  GetDeviceLocation();
                            break;

                        default: break;
                    }
                    Log.Debug("Messenger", "Received Location Message!");

                });

        }


        private async Task<LResult> Get_LastLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
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
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Default);
                Log.Debug("get_CurrentLocation: ", "requested");
               // DeviceLocation = "requested";
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);
                Log.Debug("Latitude: ", location.Latitude.ToString());
                Log.Debug("Longitude: ", location.Longitude.ToString());

                CurrentLocation = FormatLocation(location);
                
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

      public  async  Task GetDeviceLocation()
        {
            var actualLoc = await Get_CurrentLocation();
            if (actualLoc != LResult.Success)
            {
                await Get_LastLocation();
            }
          
            
            WeakReferenceMessenger.Default.Send(new Messages.DeviceLocationMessage(CurrentLocation));

        }

      public  async void StartListening()
        {
            try
            {
                Geolocation.LocationChanged += Geolocation_LocationChanged;

                var request = new GeolocationListeningRequest((GeolocationAccuracy)Accuracy);

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
            CurrentLocation = FormatLocation(e.Location);
            //Send RawLocation To MainViewModel.
            WeakReferenceMessenger.Default.Send(new RawDeviceLocationMessage(e.Location));
            //Send FormatedLocation To MainViewModel For Emergency Message.
            WeakReferenceMessenger.Default.Send(new Messages.DeviceLocationMessage(CurrentLocation));
        }

     public void StopListening()
        {
            try
            {
                if (cts != null && !cts.IsCancellationRequested)
                    cts.Cancel();

                Geolocation.LocationChanged -= Geolocation_LocationChanged;

                Geolocation.StopListeningForeground();

                CurrentLocation = "Stopped listening for foreground location updates";
            }
            catch (Exception ex)
            {
                CurrentLocation = FormatLocation(null, ex);
            }

            OnPropertyChanged(nameof(IsListening));
            OnPropertyChanged(nameof(IsNotListening));
        }


        string FormatLocation(Location location, Exception ex = null)
        {
            if (location == null)
            {
                WeakReferenceMessenger.Default.Send(new ALocationManagerToMainViewModel($"Unable to detect location. Exception: {ex?.Message ?? string.Empty}"));
                Log.Debug("FormatLocation: ", "null!");
                return $"Unable to detect location. Exception: {ex?.Message ?? string.Empty}";
            }
            Log.Debug("FormatLocation: ", "formatingData...");
            StringBuilder stringBuilder = new StringBuilder();
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
            } else
            {
                BikeSpeed = 0.0f;
            }
/*            return
                $"Latitude: {location.Latitude}\n" +
                $"Longitude: {location.Longitude}\n" +
                $"HorizontalAccuracy: {location.Accuracy}\n" +
                $"Altitude: {(location.Altitude.HasValue ? location.Altitude.SetValue.ToString() : notAvailable)}\n" +
                $"AltitudeRefSys: {location.AltitudeReferenceSystem.ToString()}\n" +
                $"VerticalAccuracy: {(location.VerticalAccuracy.HasValue ? location.VerticalAccuracy.SetValue.ToString() : notAvailable)}\n" +
                $"Heading: {(location.Course.HasValue ? location.Course.SetValue.ToString() : notAvailable)}\n" +
                $"Speed: {(location.Speed.HasValue ? location.Speed.SetValue.ToString() : notAvailable)}\n" +
                $"Date (UTC): {location.Timestamp:d}\n" +
                $"Time (UTC): {location.Timestamp:T}\n" +
                $"Mocking Provider: {location.IsFromMockProvider}";*/
         return stringBuilder.ToString();
        }


    }
}
