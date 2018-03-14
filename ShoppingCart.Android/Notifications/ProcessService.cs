
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System.Threading;
using Android.Util;

namespace ShoppingCart.Droid.Notifications
{
    [Service]
    public class ProcessService : Service
    {
        private Handler _handler;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            if (MainLooper == null)
            {
                Looper.Prepare();
            }

            _handler = new Handler(MainLooper);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (MainLooper == null)
            {
                Looper.Prepare();
            }

            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    System.Diagnostics.Debug.WriteLine("Background activity");
                    Log.Verbose("Background", "Application");
                }
            });
            thread.Start();

            return StartCommandResult.Sticky;

        }

    }
}