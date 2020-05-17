using Android.Content;
using Android.Locations;
using INAnimalHelp.Droid;
using INAnimalHelp.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(GetLocation))]
namespace INAnimalHelp.Droid
{
    public class GetLocation : ILocSettings
    {
        /// <summary>
        /// Открытие настроек для включения геолокации
        /// </summary>
        public void OpenSettings()
        {
            LocationManager LM = (LocationManager)global::Android.App.Application.Context.GetSystemService(Context.LocationService);

            if (LM.IsProviderEnabled(LocationManager.GpsProvider) == false)
            {
                Intent intent = new Intent(global::Android.Provider.Settings.ActionLocationSourceSettings);
                intent.AddFlags(ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.MultipleTask);
                global::Android.App.Application.Context.StartActivity(intent);
            }
        }
    }
}