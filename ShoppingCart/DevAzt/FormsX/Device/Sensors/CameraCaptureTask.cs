using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevAzt.FormsX.Device.Sensors
{
    public class CameraCaptureTask
    {
        private IMedia _media;

        public string FileName { get; set; }
        public string FolderName { get; set; }

        public CameraCaptureTask()
        {
            FileName = "file" + DateTime.Now.Ticks + ".jpg";
            FolderName = "Photos";
        }

        public CameraCaptureTask(string filename)
        {
            FileName = string.IsNullOrEmpty(filename) ? "file" + DateTime.Now.Ticks + ".jpg" : filename;
            FolderName = "Photos";
        }

        public CameraCaptureTask(string filename, string foldername)
        {
            FileName = string.IsNullOrEmpty(filename) ? "file" + DateTime.Now.Ticks + ".jpg" : filename;
            FolderName = string.IsNullOrEmpty(filename) ? "Photos" : foldername;
        }

        public async Task<PhotoResult> TakePhoto()
        {
            _media = CrossMedia.Current;
            if (await _media.Initialize())
            {
                if (!_media.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    return new PhotoResult { Photo = null, Success = false, Message = "No tienes camara" };
                }
                StoreCameraMediaOptions options = new StoreCameraMediaOptions
                {
                    Directory = FolderName,
                    Name = FileName,
                    CompressionQuality = 20
                };
                MediaFile file = await _media.TakePhotoAsync(options);
                return new PhotoResult { Photo = file, Success = true, Message = "Se tomó la foto" };
            }
            else
            {

            }
            return new PhotoResult { Photo = null, Success = false, Message = "No se pudo inicializar la camara" };
        }
    }

    public class PhotoResult
    {
        public string Message { get; internal set; }
        public MediaFile Photo { get; internal set; }
        public bool Success { get; internal set; }
    }
}
