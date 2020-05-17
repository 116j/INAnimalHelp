using INAnimalHelp.Models;
using INAnimalHelp.Models.Models;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventList : ContentPage
    {
        //модель для обновления данных
        readonly RefreshEventListModel model;

        public EventList()
        {
            InitializeComponent();
            BindingContext = model = new RefreshEventListModel();
        }

        public EventList(Organization organization)
        {
            InitializeComponent();

            if (organization == App.CurrentOrganization)
            {
                //добавление возможности добавления мероприятия для организации
                ToolbarItem ad = new ToolbarItem()
                {
                    IconImageSource = ImageSource.FromFile("plus.png")
                };
                ad.Clicked += async (s, e) =>
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.None)
                    {
                        await Navigation.PushAsync(new EventCreation());
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't create new event now.");
                    }
                };

                ToolbarItems.Add(ad);
            }

            BindingContext = model = new RefreshEventListModel() { Organization = organization };
        }

        /// <summary>
        /// Переход на страницу мероприятия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ToEventPage(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new EventPage((Event)list.SelectedItem));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            model.Events = new ObservableCollection<Event>();
            //список мероприятий (пользователя или организации)
            if (model.Organization == null)
            {
                list.ItemsSource = model.Events = new ObservableCollection<Event>(await App.Database.GetUserEvents());
            }
            else
            {
                list.ItemsSource = model.Events = new ObservableCollection<Event>(await App.Database.GetOrganizationEvents(model.Organization));
            }
        }
    }
}