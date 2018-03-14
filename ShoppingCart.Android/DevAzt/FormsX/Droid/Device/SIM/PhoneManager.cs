using Android.Content;
using DevAzt.FormsX.Device.SIM;
using Android.Telephony;
using Xamarin.Forms;

[assembly: Dependency(typeof(DevAzt.FormsX.Droid.Device.Sensors.PhoneManager))]
namespace DevAzt.FormsX.Droid.Device.Sensors
{
    public class PhoneManager : IPhoneManager
    {
        public string LineNumber {
            get
            {
                TelephonyManager tMgr = (TelephonyManager)Android.App.Application.Context.GetSystemService(Context.TelephonyService);
                return tMgr.Line1Number;
            }
        }
    }
}