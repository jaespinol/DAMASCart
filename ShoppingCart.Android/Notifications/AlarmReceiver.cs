using Android.Content;
using Android.Util;

namespace ShoppingCart.Droid.Notifications
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        private static string _tag
        {
            get
            {
                return "LL24";
            }
        }
        
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Verbose(_tag, "Log de vide para la alarma...");
            Intent background = new Intent(context, typeof(BackgroundService));
            context.StartService(background);
        }
    }
}