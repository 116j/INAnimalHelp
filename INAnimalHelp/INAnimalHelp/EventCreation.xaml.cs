using INAnimalHelp.Models;
using INAnimalHelp.Models.Models;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventCreation : ContentPage
    {
        readonly string[] types = { "Charitable event", "Master class", "Lecture", "Open day", "Exhibition-distribution", "Metting/Picket" };
        readonly EventModel @event = new EventModel() { Images = new List<EventImage>() };
        ObservableCollection<EventImage> Images = new ObservableCollection<EventImage>();
        private bool isSaved;

        public EventCreation()
        {
            InitializeComponent();
            @event.Event = new Event() { AdressSet = false };
            Device.BeginInvokeOnMainThread(async () => await AskForPermissions());
        }

        public EventCreation(Event @event)
        {
            InitializeComponent();
            Device.BeginInvokeOnMainThread(async () => await AskForPermissions());
            this.@event.Event = @event;
            isSaved = true;
        }
        /// <summary>
        /// Удаление картинки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DeleteImage(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                App.Database.DeleteEventImage(@event.Images[ImgCarouselView.Position].Id);
                @event.Images.Remove(@event.Images[ImgCarouselView.Position]);
                Images.Remove(Images[ImgCarouselView.Position]);
                ImgCarouselView.ItemsSource = Images;
                if (Images.Count == 0)
                {
                    row.Height = 0;
                    row1.Height = 70;
                    row2.Height = 70;
                    row3.Height = 70;
                    row4.Height = 70;
                    row5.Height = 70;
                }
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't delete this image now.");
            }
        }
        /// <summary>
        /// Разрешение на доступ к галереи
        /// </summary>
        /// <returns></returns>
        private async Task<bool> AskForPermissions()
        {
            try
            {
                await CrossMedia.Current.Initialize();
                //доступ к галереи и фото
                Plugin.Permissions.Abstractions.PermissionStatus storagePermissions = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                Plugin.Permissions.Abstractions.PermissionStatus photoPermissions = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);
                if (storagePermissions != Plugin.Permissions.Abstractions.PermissionStatus.Granted || photoPermissions != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    //пользователь должен дать свое соглашение
                    Dictionary<Permission, Plugin.Permissions.Abstractions.PermissionStatus> results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage, Permission.Photos });
                    storagePermissions = results[Permission.Storage];
                    photoPermissions = results[Permission.Photos];
                }

                if (storagePermissions != Plugin.Permissions.Abstractions.PermissionStatus.Granted || photoPermissions != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    //разрешение нужно настраивать вручную
                    await DisplayAlert("Permissions Denied!", "Please go to your app settings and enable permissions.", "Ok");
                    return false;
                }
                else
                {
                    //разрешение получено
                    return true;
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("error. permissions not set.");
                return false;
            }
        }
        /// <summary>
        /// Выбор картинок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SelectImagesButton_Clicked(object sender, EventArgs e)
        {
            //Проверка доступа
            Plugin.Permissions.Abstractions.PermissionStatus storagePermissions = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            Plugin.Permissions.Abstractions.PermissionStatus photoPermissions = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);
            if (storagePermissions == Plugin.Permissions.Abstractions.PermissionStatus.Granted && photoPermissions == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                //открытие галереи
                DependencyService.Get<IMediaService>().OpenGallery();
                //подписка на результат выбора изображений
                MessagingCenter.Unsubscribe<Xamarin.Forms.Application, List<string>>(Xamarin.Forms.Application.Current, "ImagesSelectedAndroid");
                MessagingCenter.Subscribe<Xamarin.Forms.Application, List<string>>(Xamarin.Forms.Application.Current, "ImagesSelectedAndroid", async (s, images) =>
                {
                    if (images.Count > 0 && Connectivity.NetworkAccess != NetworkAccess.None)
                    {
                        //начало загрузки картинок
                        spinner.IsVisible = true;
                        spinner.IsRunning = true;
                        //сохранение мероприятия для получения его ID
                        @event.Event = await App.Database.SaveEventAsync(@event.Event);
                        //сохранение картинок и получение их пути на данном устройстве
                        List<EventImage> imgs = (await App.Database.SaveEventImages(images, @event.Event)).ConvertAll(i =>
                         new EventImage() { ImageUrl = Path.Combine(App.ASPDatabase.storage, i.ImageUrl) });
                        @event.Images.AddRange(imgs);
                        Images = new ObservableCollection<EventImage>(@event.Images);
                        //конец загрузи
                        spinner.IsRunning = false;
                        spinner.IsVisible = false;
                        //изменение разметки(особождение места)
                        row.Height = 270;
                        row1.Height = 70;
                        row2.Height = 70;
                        row3.Height = 70;
                        row4.Height = 70;
                        row5.Height = 70;
                        //обновление 
                        ImgCarouselView.ItemsSource = Images;
                        BindingContext = @event;
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't add images now.");
                    }
                });
            }
        }
        /// <summary>
        /// Выбор даты начала мероприятия 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetStartDate(object sender, EventArgs e)
        {
            @event.Event.StartDay = startDate.Date.ToShortDateString();
            @event.Event.StartD = startDate.Date.Day;
            @event.Event.StartM = startDate.Date.Month;
            @event.Event.StartY = startDate.Date.Year;
        }
        /// <summary>
        /// Выбор даты конца мероприятия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SetEndDate(object sender, EventArgs e)
        {
            if (@event.Event.StartDay != null)
            {
                //если дата начала больше даты конца
                if (startDate.Date > endDate.Date)
                {
                    await DisplayAlert("", "The start date can't be greater than the end date", "OK");
                    return;
                }
            }

            @event.Event.EndDay = endDate.Date.ToShortDateString();
            @event.Event.EndD = endDate.Date.Day;
            @event.Event.EndM = endDate.Date.Month;
            @event.Event.EndY = endDate.Date.Year;
        }
        /// <summary>
        /// выбор типа мероприятия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetEventType(object sender, EventArgs e)
        {
            if (type.SelectedItem != null)
                @event.Event.EventType = type.SelectedItem.ToString();
        }
        /// <summary>
        /// Выбор адреса мероприятия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SetEventAdress(object sender, EventArgs e)
        {
            //сохранение информации
            @event.Event.Name = name.Text;
            @event.Event.Info = info.Text;

            await Navigation.PushAsync(new AdressSearcher(@event.Event));
        }
        /// <summary>
        /// Сохранение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void SaveEvent(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                //если начальная дата не установлена, то устанавливается текущая
                if (@event.Event.StartDay == null)
                {
                    @event.Event.StartDay = DateTime.Now.Date.ToShortDateString();
                    @event.Event.StartD = DateTime.Now.Date.Day;
                    @event.Event.StartM = DateTime.Now.Date.Month;
                    @event.Event.StartY = DateTime.Now.Date.Year;
                }
                //если начальная дата больше конечной даты, то выходим
                if (@event.Event.EndDay != null && startDate.Date > endDate.Date)
                {
                    @event.Event.StartDay = null;
                    @event.Event.EndDay = null;
                    await DisplayAlert("", "The start date can't be greater than the end date", "OK");
                    return;
                }
                //если конечная дата не установлена, то устанавливается текущая
                else if (@event.Event.EndDay == null)
                {
                    @event.Event.EndDay = DateTime.Now.Date.ToShortDateString();
                    @event.Event.EndD = DateTime.Now.Date.Day;
                    @event.Event.EndM = DateTime.Now.Date.Month;
                    @event.Event.EndY = DateTime.Now.Date.Year;
                }
                //сохранение данных
                @event.Event.Info = info.Text;
                @event.Event.Name = name.Text;
                @event.Event.OrganizationId = App.CurrentOrganization.Id;
                @event.Event.OrganizationName = App.CurrentOrganization.Name;
                isSaved = true;
                await App.Database.SaveEventAsync(@event.Event);
                await Navigation.PopAsync();
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't save this event now.");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            type.ItemsSource = types;

            //если не новове мероприятие, то пытаемся получить его картинки с путем на данном устройстве
            if (@event.Event.Id != 0)
            {
                @event.Images = (await App.Database.GetEventImages(@event.Event)).ConvertAll(i =>
            new EventImage() { ImageUrl = Path.Combine(App.ASPDatabase.storage, i.ImageUrl), Id = i.Id });
            }
            //если есть картинки, то устанавливаем разметку для них
            if (@event.Images.Count != 0)
            {
                Images = new ObservableCollection<EventImage>(@event.Images);
                row1.Height = 70;
                row2.Height = 70;
                row3.Height = 70;
                row4.Height = 70;
                row5.Height = 70;
                ImgCarouselView.ItemsSource = Images;
            }
            row.Height = @event.Images.Count != 0 ? 270 : 0;
            //если не новое мероприятие, устанавливаем дату
            if (@event.Event.AdressSet || isSaved)
            {
                if (@event.Event.StartDay != null)
                {
                    startDate.Date = new DateTime(@event.Event.StartY, @event.Event.StartM, @event.Event.StartD);
                }
                if (@event.Event.EndDay != null)
                {
                    endDate.Date = new DateTime(@event.Event.EndY, @event.Event.EndM, @event.Event.EndD);
                }
                adress.Text = @event.Event.Adress;
            }

            BindingContext = @event.Event;
        }
        /// <summary>
        /// Удаление мероприятия, если оно не было сохранено до конца
        /// </summary>
        internal void Popped()
        {
            if (!isSaved && @event.Event != null && @event.Event.Id != 0)
                App.Database.DeleteEventAsync(@event.Event);
        }
    }
}