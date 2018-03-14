using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DevAzt.FormsX.Device.Sensors
{
    public class GeoPosition
    {
        private IGeoPosition igeoposition;

        public event EventHandler<Position> PositionChange;

        public GeoPosition(IGeoPosition igeoposition)
        {
            this.igeoposition = igeoposition;
            igeoposition.Position += Igeoposition_Position;
        }

        private void Igeoposition_Position(object sender, Position e)
        {
            if (PositionChange != null)
            {
                PositionChange.Invoke(this, e);
            }
        }

        public static GeoPosition Instance
        {
            get
            {
                var igeoposition = DependencyService.Get<IGeoPosition>();
                if (igeoposition == null) throw new NullReferenceException("La dependencia de servicio es null");
                return new GeoPosition(igeoposition);
            }
        }

        public Position LastPosition
        {
            get
            {
                if (igeoposition != null)
                {
                    return igeoposition.LastPosition;
                }
                return null;
            }
        }

        public bool IsGPSStarted
        {
            get
            {
                if (igeoposition != null)
                {
                    return igeoposition.IsGPSStarted;
                }
                return false;
            }
        }

        public bool StartGPS()
        {
            if (igeoposition != null)
            {
                igeoposition.Init();
                return igeoposition.IsGPSStarted;
            }
            return false;
        }

        public async Task<bool> PermitsGranted()
        {
            if (igeoposition != null)
            {
                return await igeoposition.PermitsGranted();
            }
            return false;
        }

        public bool Stop()
        {
            if (igeoposition != null)
            {
                igeoposition.Stop();
                return igeoposition.IsGPSStarted;
            }
            return false;
        }
    }
}
