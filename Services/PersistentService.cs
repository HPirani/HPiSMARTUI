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
