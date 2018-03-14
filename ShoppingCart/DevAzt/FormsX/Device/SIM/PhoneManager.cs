using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DevAzt.FormsX.Device.SIM
{
    public class PhoneManager : IPhoneManager
    {
        private IPhoneManager _service;

        public PhoneManager(IPhoneManager service)
        {
            _service = service;
        }

        public string LineNumber
        {
            get
            {
                return _service.LineNumber;
            }
        }

        public static PhoneManager Instance
        {
            get
            {
                var phonemanager = DependencyService.Get<IPhoneManager>();
                if (phonemanager == null) throw new NullReferenceException("La dependencia de servicio es null");
                return new PhoneManager(phonemanager);
            }
        }

    }
}
