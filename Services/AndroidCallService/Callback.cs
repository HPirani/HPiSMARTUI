/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUi                                               **  
** Description:                                                                  **
**                                                                               **
**                                                                               **
** Created in sat 1403/06/14 09:40 PM By Hosein Pirani                           **
**                                                                               **
** Modified In Wed 1403/05/31 09:45 PM To  10:15 by me.                          **
** : First Implementation                                                        **
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
using System.Text;
using System.Threading.Tasks;

using Android.Runtime;
using Android.Telecom;

namespace HPISMARTUI.Services.AndroidCallService
    {
    internal class Callback : Call.Callback 
        {
        OngoingCall ongoingCall;
        public Callback(OngoingCall ongoing)
            {
            this.ongoingCall = ongoing;
            }
        public override void OnStateChanged(Call call,  CallState state) 
            {

            base.OnStateChanged(call, state);
            //  Timber.d(call.ToString());
            ongoingCall.state.OnNext(state);

            }
        
        
        }
    }
