/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUi                                               **  
** Description:                                                                  **
**                                                                               **
**                                                                               **
** Created in sat 1403/06/14 9:23 PM By Hosein Pirani                            **
**                                                                               **
** Modified In Wed 1403/06/14 09:45 PM To  10:00 by me.                          **
** :  First Implementation.                                                      **
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





using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

using Android.Telecom;

namespace HPISMARTUI.Services.AndroidCallService
    {
    internal class OngoingCall
        {

        private Callback callback;
      public  BehaviorSubject<CallState> state = new(CallState.Disconnected);
       public Call call;//?
        public OngoingCall(Callback _callback)
            {
            callback = _callback;
            
            
            }

        void answer()
            {
            call?.Answer(VideoProfileState.AudioOnly);
            
            }

        void hangup()
            {
            call?.Disconnect();
            }
        }
    }
