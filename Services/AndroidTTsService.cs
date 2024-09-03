/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUi                                               **  
** Description: Android Text To Speech Service.                                  **
**                                                                               **
**                                                                               **
** Created in sat 1403/05/31 3:57 PM By Hosein Pirani                            **
**                                                                               **
** Modified In Wed 1403/05/31 04:00 PM To 19:15 by me.                           **
** : Intial Implementation.                                                      **
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;
using System.Collections.ObjectModel;
using Android.Util;
using HPISMARTUI.ViewModel;

namespace HPISMARTUI.Services
{
   public partial class AndroidTTsService : ObservableObject
    {
#pragma warning disable CS8632 

#region Options
        MainViewModel vm;
        readonly ITextToSpeech textToSpeech;
        readonly ISpeechToText speechToText;
        CancellationTokenSource cts = new();
        [ObservableProperty]
        private string recognitionText;

#endregion

#region VoiceCommands
        Dictionary<string, Action>  VoiceCommands;
        private string Logchache;


#endregion
        //
#region Distructor

        public AndroidTTsService(MainViewModel viewModel,ITextToSpeech textToSpeech, ISpeechToText speechToText)
        {
            vm = viewModel;
            this.speechToText = speechToText;
            this.textToSpeech = textToSpeech;
            RegisterVoiceCommands();
        }

        ~AndroidTTsService()
        {
          
        }


#endregion
//

#region TextToSpeech
//
        public async Task SpeakNowDefaultSettingsAsync(string what)
        {
            
            await textToSpeech.SpeakAsync(what, cancelToken: cts.Token);

            // This method will block until utterance finishes.
        }

        // Cancel speech if a cancellation token exists & hasn't been already requested.
        public void CancelSpeech()
        {
            if (cts?.IsCancellationRequested ?? true)
                return;

            cts.Cancel();
        }
//
#endregion
 //
        //------------------------------------------------
#region SpeechToText
        //Speech To Text Service.

        public  async Task StartListening()
        {
            var isGranted = await SpeechToText.RequestPermissions(cts.Token);
            if (!isGranted)
            {
               // await Toast.Make("Permission not granted").Show(CancellationToken.None);
                return;
            }

            speechToText.RecognitionResultUpdated += OnRecognitionTextUpdated;
            speechToText.RecognitionResultCompleted += OnRecognitionTextCompleted;
            await SpeechToText.StartListenAsync(CultureInfo.CurrentCulture, CancellationToken.None);
        }

       public async Task StopListening()
        {
            await SpeechToText.StopListenAsync(CancellationToken.None);
            speechToText.RecognitionResultUpdated -= OnRecognitionTextUpdated;
            speechToText.RecognitionResultCompleted -= OnRecognitionTextCompleted;
        }


        private void OnRecognitionTextUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs args)

        {
            RecognitionText += args.RecognitionResult;
        }

        private void OnRecognitionTextCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs args)
        {

            RecognitionText = args.RecognitionResult;
            
            TryParseInput(RecognitionText);
        }

        private void RegisterVoiceCommands()
        {
            VoiceCommands = new()
            {
                {"StartEngine",()=> DisplayMessage("StartEngine")},
                {"Start Engine",()=> Ld("Start Engine")},
                {"Force Start Engine",()=> Ld("Force Start Engine")},
                {"Start Now",()=> Ld("Start Now")},
                {"StartNow",()=> Ld("StartNow")},


            };

        }
        private void TryParseInput(string input)
        {            
            
                Log.Debug("Received", "${input}");
                
                if (VoiceCommands.TryGetValue(input, out Action value))
                {
                    value.Invoke();
                } else
                {
                    Log.Debug("Try", "NotEqual");
                }
            

        }

#endregion

#region TTsSpecific

        private async void DisplayMessage(string message,string title = nameof(AndroidTTsService),string ok = "OK")
        {
            await Shell.Current.DisplayAlert(message, title, ok);
        }

        private async void Ld(string message, string title = nameof(AndroidTTsService), char type = 'd')
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
           /*
            //using var stream = File.OpenWrite("HPiSmartUILog.txt");
            var docsDirectory = Android.OS.Environment.ExternalStorageDirectory;
            System.IO.File.WriteAllText($"{docsDirectory.AbsoluteFile.Path}/HPlog.txt", string.IsNullOrEmpty(Logchache) ? $"vm:{EmergencyLocation}" : $"{Logchache} \r\n vm:{EmergencyLocation}" );
            var a = System.IO.File.OpenRead($"{docsDirectory.AbsoluteFile.Path}/HPlog.txt");
            using StreamReader reader_1 = new(a);
             Logchache = await reader_1.ReadToEndAsync();
           */

        }
#endregion

#pragma warning restore CS8632 
    }
}
