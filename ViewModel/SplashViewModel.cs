/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiUIpro.                                                **  
** Description:                                                                  **
** Splash Screen ViewModel.                                                      **
**                                                                               **
** Created in sat 1403/02/025 13:40 PM By Hosein Pirani                          **
**                                                                               **
** Modified In sun 1403/03/01 16:00 PM To 20:05 by me.                           **
** : Splash Video Added.                                                         **
** TODO: Find Besmellah Label And Fade it.                                       **
** TODO:  & Fix It's Position                                                    **
** ..                                                                            **
** ...                                                                           **
** And  LOT OF CODE @_@                                                          **
** .....                                                                         **  
** ........                                                                      **
** ...........                                                                   **
** ...............                                                               **
 *********************************************************************************/


using Android.Util;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using HPISMARTUI.View;

using Kotlin.Properties;



namespace HPISMARTUI.ViewModel
{
    public partial  class SplashViewModel : ObservableObject
    {
        Animation besmellahAnimation = new();
        [ObservableProperty]
        private bool  besmellahVisible = false;
        [ObservableProperty]
        private String besmellahStyle = "A";
        [ObservableProperty]
        private String besmellahFont = "faBesmellah1";
        List<String> Fonts = new();
        private readonly String BesmellahStyles = "1234567890AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
        [ObservableProperty]
        private bool videoPlayerVisible = true;
        public SplashViewModel()
        {
            Fonts.Add("faBesmellah1");
            Fonts.Add("fabesmellah2");//phantom-stencil
            Fonts.TrimExcess();
            Random random = new Random();
            var randomIndex = random.Next(0,BesmellahStyles.Length - 1);
            BesmellahStyle = BesmellahStyles.Substring(randomIndex,1);
            //

            var randomIndex2 = 0;
            do
            {

                 randomIndex2 = random.Next(0, 2);
            }while(randomIndex2 > 2);

            BesmellahFont = Fonts[randomIndex2];

        }
        

        [RelayCommand]
       async Task OnMediaFailed()
        {
            Log.Error("Media:", " failed");
            VideoPlayerVisible = false;
            BesmellahVisible = true;
            await Task.Delay(TimeSpan.FromSeconds(1));

         await Shell.Current.GoToAsync(nameof(MainPage), true);
        }
        [RelayCommand]
        async Task OnMediaEnded()
        {
            Log.Info("MediaPlayer", "Media ended.");
            VideoPlayerVisible = false;
            BesmellahVisible = true;
               await Task.Delay(TimeSpan.FromSeconds(1));
             await Shell.Current.GoToAsync(nameof(MainPage), true);
            //SplashPage->Besmellah.FadeTo(0);
          //await  SplashPage.FindByName<Label>("Besmellah").FadeTo(0);
        }



    }
}
