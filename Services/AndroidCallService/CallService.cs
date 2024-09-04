/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUi                                               **  
** Description:                                                                  **
**                                                                               **
**                                                                               **
** Created in wed 1403/06/14 9:23 PM By Hosein Pirani                            **
**                                                                               **
** Modified In Wed 1403/06/14 09:25 PM To  10:00by me.                           **
** : First Implementation.                                                       **
** TODO:  Complete Methods                                                       **
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

using Android.Telecom;

namespace HPISMARTUI.Services.AndroidCallService
    {
    internal class CallService : InCallService
        {
        OngoingCall ongoingCall;
        public CallService(OngoingCall _ongoingCall) 
            {
            ongoingCall = _ongoingCall;
            }
      public  override void  OnCallAdded(Call call)
            {
            ongoingCall.call = call;
        //CallActivity.start(this, call)
    }

       public override void OnCallRemoved(Call call)
            {
            ongoingCall.call = null;
        }

        }
    }
