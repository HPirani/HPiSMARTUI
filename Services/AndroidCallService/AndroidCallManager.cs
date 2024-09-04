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
** Modified In Wed 1403/06/14 09:30 PM To  11:25by me.                           **
** :Implementation                                                               **
** TODO:Complete  Methods.                                                       **
** TODO:                                                                         **
** ..                                                                            **
** ...                                                                           **
** And CODE                                                                      **
** ..... More Code                                                               **  
** ........ Code                                                                 **
** ...........  #_#                                                              **
** ...............                                                               **
 *********************************************************************************/








using Android.Content;
using Android.Telecom;

namespace HPISMARTUI.Services.AndroidCallService
    {
    internal class AndroidCallManager
    {
        public Call telecomCall;
        public Callback _callback;
        private static TelecomManager _instance;
        private CallService _CallService;
        Context context;


        public enum DialerCallState
        {

            BLOCKED,
            DISCONNECTED,
            CONFERENCED
        };

        public AndroidCallManager(Call call, TelecomManager instance,Callback callback,CallService callService)

        {
            _CallService = callService;
            telecomCall = call;
            _callback = callback;
            _instance = instance;

            telecomCall.RegisterCallback(_callback);

        }
        ~AndroidCallManager()
            {
            telecomCall.UnregisterCallback(_callback);
            }

#warning This Method will Not Work On Android 11 and above. We should Use RoleManager Instead Of This. 
        private void offerReplacingDefaultDialer()
        {

            if (_instance.DefaultDialerPackage != Android.App.Application.Context.PackageName)
            {
                var intent = new Intent(TelecomManager.ActionChangeDefaultDialer)
                      .PutExtra(TelecomManager.ExtraChangeDefaultDialerPackageName, Android.App.Application.Context.PackageName);
                Android.App.Application.Context.StartActivity(intent);
            }
        }
        




        }
}

