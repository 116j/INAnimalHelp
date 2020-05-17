using Android.App;
using Android.Content;
using Android.Widget;
using INAnimalHelp.Droid;
using INAnimalHelp.Models;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(MediaService))]
namespace INAnimalHelp.Droid
{
    public class MediaService : Java.Lang.Object, IMediaService
    {
        public static int OPENGALLERYCODE = 100;
        /// <summary>
        /// Открытие галереи
        /// </summary>
        public void OpenGallery()
        {
            try
            {
                Intent imageIntent = new Intent(Intent.ActionPick);
                imageIntent.SetType("image/*");
                imageIntent.PutExtra(Intent.ExtraAllowMultiple, true);
                imageIntent.SetAction(Intent.ActionGetContent);
                ((Activity)Xamarin.Forms.Forms.Context).StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), OPENGALLERYCODE);
                Toast.MakeText(global::Android.App.Application.Context, "Tap and hold to select multiple photos.", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Toast.MakeText(global::Android.App.Application.Context, "Error. Can not continue, try again.", ToastLength.Long).Show();
            }
        }
    }
}