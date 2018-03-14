using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevAzt.FormsX.Device.Sensors
{
    public interface IGeoPosition
    {
        /// <summary>
        /// Evento que será lanzado para obtener la posición actual del GPS
        /// </summary>
        event EventHandler<Position> Position;
        
        /// <summary>
        /// Determina si hay acceso al GPS
        /// </summary>
        bool IsGPSStarted { get; set; }

        /// <summary>
        /// Este método permitira iniciar el GPS en cualquier dispositivo
        /// </summary>
        void Init();

        /// <summary>
        /// Este método permitira detener la busqueda del GPS en cualquier dispositivo
        /// </summary>
        void Stop();

        /// <summary>
        /// Este método permitirá acceder a los permisos de uso para el GPS
        /// </summary>
        /// <returns></returns>
        Task<bool> PermitsGranted();

        /// <summary>
        /// Esta propiedad permitirá acceder a la ultima posición conocida del dispositivo en caso de que no se encuentre una posición
        /// </summary>
        Position LastPosition { get;  set;}
    }
}
