using INAnimalHelp.Models;
using INAnimalHelp.Models.Models;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventPage : ContentPage
    {
        //модель для обновления страницы
        readonly RefreshEventPageModel model;

        public EventPage(Event @event)
        {
            BindingContext = model = new RefreshEventPageModel() { Event = new EventModel() { Event = @event } };
            InitializeComponent();
            if (App.CurrentOrganization != null && @event.OrganizationId == App.CurrentOrganization.Id)
            {
                //добавление кнопок редактирования и удаления для организации
                ToolbarItem re = new ToolbarItem()
                {
                    Text = "Remove",
                    IconImageSource = "delete"
                };
                ToolbarItem ed = new ToolbarItem()
                {
                    Text = "Edit",
                    IconImageSource = "edit.png"
                };
                re.Clicked += async (s, e) =>
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.None)
                    {
                        App.Database.DeleteEventAsync(@event);
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't delete this event now.");
                    }
                };

                ed.Clicked += async (s, e) =>
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.None)
                    {
                        await Navigation.PushAsync(new EventCreation(@event));
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't edit this event now.");
                    }
                };

                ToolbarItems.Add(re);
                ToolbarItems.Add(ed);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //устанавливаем контент вручную, так как все хранится в модели
            name.Text = model.Event.Event.Name;
            info.Text = model.Event.Event.Info;
            adress.Text = model.Event.Event.Adress;
            start.Text = model.Event.Event.StartDay;
            end.Text = model.Event.Event.EndDay;
            type.Text = model.Event.Event.EventType;
            orgname.Text = model.Event.Event.OrganizationName;
            ImgCarouselView.ItemsSource = model.Event.Images = (await App.Database.GetEventImages(model.Event.Event)).ConvertAll(i =>
            new EventImage() { ImageUrl = Path.Combine(App.ASPDatabase.storage, i.ImageUrl) });
            row.Height = model.Event.Height = model.Event.Images.Count != 0 ? 230 : 0;
            imgStack.IsVisible = model.ImageVisibility = model.Event.Images.Count != 0;
        }
    }
}