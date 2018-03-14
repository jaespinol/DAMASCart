using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System.Threading;

namespace ShoppingCart.Droid.Notifications
{
    [Service]
    public class BackgroundService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();

        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (MainLooper == null)
            {
                Looper.Prepare();
            }

            Thread t = new Thread((obj) =>
            {
                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    VerifyNotifications();
                }
            });
            t.Start();
            return StartCommandResult.Sticky;
        }

        private async void VerifyNotifications()
        {
            try
            {
                ServicioWeb web = new ServicioWeb(this);
                await web.GetNotifications();
            }
            catch
            {

            }
        }
    }
}