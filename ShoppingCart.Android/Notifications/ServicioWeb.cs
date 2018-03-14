using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using System.Threading.Tasks;
using DevAzt.FormsX.Droid.Storage.SQLite.LiteConnection;
using ShoppingCart.API.Result;
using ShoppingCart.ORM;
using DevAzt.FormsX.Net.HttpClient;

namespace ShoppingCart.Droid.Notifications
{
    public class ServicioWeb
    {
        private Context _context;
        private Service _service;

        public ServicioWeb(Service service)
        {
            _context = service.ApplicationContext;
            _service = service;
        }

        public async Task GetNotifications()
        {
            Usuario userlast = null;
            try
            {
                Connection connection = new Connection();
                var db = connection.GetDataBase();
                userlast = db.Usuario.LastOrDefault();
            }
            catch
            {

            }
            if (userlast != null)
            {
                var client = new RestForms();
                NotificationsResult result = null;
                string url = "";
                if (userlast.IdPerfil == 1)
                {
                    url = $"{App.BaseUrl}/Notification/GetBouchers";
                }
                else if (userlast.IdPerfil == 4)
                {
                    url = $"{App.BaseUrl}/Notification/GetVerificados";
                }
                else if (userlast.IdPerfil == 2)
                {
                    url = $"{App.BaseUrl}/Notification/GetGuias/{userlast.IdUsuario}";
                }
                result = await client.Post<NotificationsResult>(url, new Dictionary<string, object>());
                ProcessNotificationsResult(result);
            }
        }


        private void ProcessNotificationsResult(NotificationsResult result)
        {
            if (result != null)
            {
                if (result.Code == 100)
                {
                    foreach (var ticket in result.Tickets)
                    {
                        int idnotification = 0;
                        int.TryParse(ticket.IdTicket, out idnotification);
                        Notification.Builder builder = new Notification.Builder(_context)
                            .SetContentTitle(App.AppName)
                            .SetContentText(ticket.Message)
                            .SetSmallIcon(Resource.Drawable.icon);
                        Notification notification = builder.Build();
                        NotificationManager notificationManager = _context.GetSystemService(Context.NotificationService) as NotificationManager;
                        notificationManager.Notify(App.AppName + "|" + idnotification, idnotification, notification);
                    }
                }
            }
        }
    }
}