using INAnimalHelp.Models.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace INAnimalHelp.Models
{
    //страничка с интерфейсами и перечислениями
    public interface IToast
    {
        void Show(string message);
    }
    public interface ILocSettings
    {
        void OpenSettings();
    }

    public interface IMediaService
    {
        void OpenGallery();
    }

    public interface ICompressImages
    {
        string CompressImage(string path);
    }

    public interface IPhotoPickerService
    {
        Task<Stream> GetImageAsync();
    }

    public interface IFacebookService
    {
        Task<LoginResult> Login();
        void Logout();
    }

    public interface IVkService
    {
        Task<LoginResult> Login();
        void Logout();
    }

    public enum LoginState
    {
        Failed,
        Canceled,
        Success
    }
}
