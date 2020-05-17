
using Android.App;
using Android.Widget;
using INAnimalHelp.Droid;
using INAnimalHelp.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(MessageService))]
namespace INAnimalHelp.Droid
{
    public class MessageService : IToast
    {
        /// <summary>
        /// Показ уведомлений
        /// </summary>
        /// <param name="message">текст уведомления</param>
        public void Show(string message)
        {
            Toast.MakeText(global::Android.App.Application.Context, message, ToastLength.Long).Show();
        }
    }
}