
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.CurrentActivity;
using DevAzt.FormsX.Device.Sensors;

[assembly: Dependency(typeof(DevAzt.FormsX.Droid.Device.Sensors.GeoPosition))]
namespace DevAzt.FormsX.Droid.Device.Sensors
{
    public class GeoPosition : IGeoPosition
    {
        public bool IsGPSStarted { get; set; }

        public Position LastPosition
        {
            get; set;
        }

        public event EventHandler<Position> Position;

        private void OnPositionChange(Position currentposition)
        {
            if (Position != null)
            {
                Position.Invoke(this, currentposition);
            }
        }

        Geolocator _geolocator;
        public void Init()
        {
            var context = CrossCurrentActivity.Current.Activity.ApplicationContext;
            _geolocator = new Geolocator(context);
            IsGPSStarted = _geolocator.IsGPSStarted;
            if (IsGPSStarted)
            {
                _geolocator.PositionChanged += Geolocator_PositionChanged;
            }
        }

        private void Geolocator_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            try
            {
                var latitude = e.Position.Latitude;
                var longitude = e.Position.Longitude;
                var accuaracy = e.Position.Accuracy;
                var date = TimeSpan.FromMilliseconds(e.Position.Time);
                Position position = new Position()
                {
                    Success = true,
                    Date = new DateTime(date.Ticks),
                    Accuracy = accuaracy,
                    Latitude = latitude,
                    Longitude = longitude
                };
                this.LastPosition = position;
                OnPositionChange(position);
            }
            catch
            {
                OnPositionChange(new Position { Success = false });
            }
        }

        public void Stop()
        {
            _geolocator.PositionChanged -= Geolocator_PositionChanged;
            _geolocator.Stop();
            IsGPSStarted = false;
        }

        public Task<bool> PermitsGranted()
        {
            Task<bool> mytask = new Task<bool>(() => true);
            mytask.Start();
            return mytask;
        }

        private void WaitTask()
        {

        }
    }
}
