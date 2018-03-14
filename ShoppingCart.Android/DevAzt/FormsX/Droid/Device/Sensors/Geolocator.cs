using System;
using System.Collections.Generic;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using System.Linq;
using System.Threading.Tasks;

namespace DevAzt.FormsX.Droid.Device.Sensors
{
	public class Geolocator : Java.Lang.Object, ILocationListener
	{
		Location _currentLocation;
		LocationManager _locationManager;
		public string _locationProvider;
        public bool IsGPSStarted { get; set; }

        public Geolocator(Context _context) : base()
		{	
			_locationManager = (LocationManager)_context.GetSystemService(Context.LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
            IsGPSStarted = false;
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);
			if (acceptableLocationProviders.Any())
			{
				_locationProvider = acceptableLocationProviders.First();
				_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
                IsGPSStarted = true;
            }
		}

		public void Stop()
		{
			if (_locationManager != null)
			{
				_locationManager.RemoveUpdates(this);
			}
		}
        
        public event EventHandler<PositionChangedEventArgs> PositionChanged;

		private void OnPositionChanged(Location location)
		{
            if(PositionChanged != null){
                PositionChanged.Invoke(this, new PositionChangedEventArgs(location));
            }
		}

		public void OnLocationChanged(Location location)
		{
			_currentLocation = location;
			// ejecutamos el evento
			if (location != null)
			{
				OnPositionChanged(location);
			}
		}

		public void OnProviderDisabled(string provider)
		{
			
		}

		public void OnProviderEnabled(string provider)
		{

		}

		public void OnStatusChanged(string provider, Availability status, Bundle extras)
		{

		}
	}

	public class PositionChangedEventArgs
	{

		private Location _location;
		public Location Position
		{
			get { return _location; }
		}

		public PositionChangedEventArgs(Location location)
		{
			_location = location;
		}
	}
}