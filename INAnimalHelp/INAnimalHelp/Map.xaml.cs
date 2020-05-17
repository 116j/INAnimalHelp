using INAnimalHelp.Models;
using INAnimalHelp.Models.Models;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Map : ContentPage
    {
        List<Organization> organizations = new List<Organization>();
        bool isFilter = false;

        public Map(string organization, string animal)
        {
            InitializeComponent();
            SetPins(organization, animal);
        }

        async void SetPins(string organization, string animal)
        {
            //Запрос на включение геолокации, если она недоступна
            if (!CrossGeolocator.Current.IsGeolocationEnabled)
            {
                bool myAction = await DisplayAlert("Location", "Please Turn On Location", "OK", "CANCEL");
                if (myAction)
                {
                    DependencyService.Get<ILocSettings>().OpenSettings();
                }
                else
                {
                    await DisplayAlert("Alert", "User Denied Permission", "OK");
                }
            }
            //если геолокация доступна, то переход на ее место
            if (CrossGeolocator.Current.IsGeolocationEnabled)
            {
                IGeolocator locator = CrossGeolocator.Current;
                Plugin.Geolocator.Abstractions.Position position = await locator.GetPositionAsync();
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude),
                                                             Distance.FromMiles(2)));
            }
            //фильтрация по заданным параметрам
            organizations = await App.Database.GetOrganizationsAsync();
            List<Organization> orgs;
            if (organization != null && animal != null)
            {
                orgs = organizations.Where(o => o.AnimalType == animal && o.OrganizationType == organization).ToList();
            }
            else if (organization != null)
            {
                orgs = organizations.Where(o => o.OrganizationType == organization).ToList();
            }
            else if (animal != null)
            {
                orgs = organizations.Where(o => o.AnimalType == animal).ToList();
            }
            else
            {
                orgs = organizations;
            }

            SetPins(orgs);
        }

        async void SetPins(List<Organization> orgs)
        {

            if (organizations.Count > orgs.Count)
            {
                isFilter = true;
            }

            //установка маркеров организаций
            foreach (Organization org in orgs)
            {
                if (org.Adress != null)
                {
                    Pin pin = new Pin
                    {
                        Label = org.Name,
                        Address = org.Adress,
                        Type = PinType.Place,
                        Position = new Xamarin.Forms.Maps.Position(double.Parse(org.Lattitude), double.Parse(org.Longitude))
                    };
                    pin.InfoWindowClicked += async (s, e) =>
                    {
                        e.HideInfoWindow = true;
                        await Navigation.PushAsync(new OrganizationPage(org));
                    };
                    map.Pins.Add(pin);
                }
            }
            //установка маркеров мероприятий организаций пользователя
            if (App.CurrentUser != null && !isFilter)
            {
                List<Event> events = await App.Database.GetUserEvents();
                foreach (Event ev in events)
                {

                    if (ev.Adress != null)
                    {
                        Pin pin = new Pin
                        {
                            Label = ev.Name,
                            Type = PinType.Place,
                            Address = ev.Adress,
                            Position = new Xamarin.Forms.Maps.Position(double.Parse(ev.Lattitude), double.Parse(ev.Longitude))
                        };

                        pin.InfoWindowClicked += async (s, e) =>
                        {
                            e.HideInfoWindow = true;
                            await Navigation.PushAsync(new EventPage(ev));
                        };
                        map.Pins.Add(pin);
                    }
                }

            }
            //установка маркеров мероприятий организации
            else if (App.CurrentOrganization != null && !isFilter)
            {
                List<Event> events = await App.Database.GetOrganizationEvents(App.CurrentOrganization);
                foreach (Event ev in events)
                {

                    if (ev.Adress != null)
                    {
                        Pin pin = new Pin
                        {
                            Label = ev.Name,
                            Type = PinType.Place,
                            Address = ev.Adress,
                            Position = new Xamarin.Forms.Maps.Position(double.Parse(ev.Lattitude), double.Parse(ev.Longitude))
                        };
                        pin.InfoWindowClicked += async (s, e) =>
                        {
                            e.HideInfoWindow = true;
                            await Navigation.PushAsync(new EventPage(ev));
                        };
                        map.Pins.Add(pin);
                    }
                }
            }
        }
    }
}