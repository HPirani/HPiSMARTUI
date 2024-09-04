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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Service.Carrier;
using Android.Util;

namespace HPISMARTUI.Services
{
    [Service (Name = "ir.hpi.hpismartui.PersistentService", Exported = false)]
    public class PersistentService : CarrierMessagingClientService
    {
/*               private readonly IBinder binder = new LocalBinder();
                public class LocalBinder : Android.OS.Binder
                {
                    public PersistentService GetService()
                    {
                        return this.GetService();
                    }
                }*/



/*        public override bool BindService(Intent service, IServiceConnection conn, [GeneratedEnum] Bind flags)
        {
            return base.BindService(service, conn, flags);
        }*/

        public override void OnCreate()
    {
        base.OnCreate();
        Log.Debug("SmsTestApp", "onCreate");
    }

    
    public override void OnDestroy()
    {
        Log.Debug("SmsTestApp", "onDestroy");
    }



        public override bool OnUnbind(Intent intent)
    {
        Log.Debug("SmsTestApp", "onUnbind");
        return false;
    }
}

}
