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
    public partial class NoteCreation : ContentPage
    {
        readonly NoteModel note = new NoteModel() { Note = new Note(), Images = new List<NoteImage>() };
        readonly Organization organization;
        bool isSaved = false;
        ObservableCollection<NoteImage> Images;

        public NoteCreation(Organization organization)
        {
            this.organization = organization;
            InitializeComponent();
            Device.BeginInvokeOnMainThread(async () => await AskForPermissions());
            note = new NoteModel() { Note = new Note(), Images = new List<NoteImage>() };
        }

        public NoteCreation(NoteModel note, Organization organization)
        {
            this.organization = organization;
            isSaved = true;
            //размер поля ввода
            if (note.Note.Info != null || note.Note.Info != "")
            {
                note.Size += 23 * note.Note.Info.Length / 38;
            }

            InitializeComponent();
            this.note = note;
            input.Text = note.Note.Info;
            GetImages();
            Device.BeginInvokeOnMainThread(async () => await AskForPermissions());
        }
        /// <summary>
        /// Добавление картинок записи
        /// </summary>
        async void GetImages()
        {
            if (note.Note.Id != 0)
            {
                note.Images = await App.Database.GetNoteImages(note.Note);
            }

            imgstack.IsVisible = note.Images.Count != 0;

            if (note.Images.Count != 0)
            {
                row2.Height = 570;
                note.Images = note.Images.ConvertAll(i => new NoteImage() { ImageUrl = Path.Combine(App.ASPDatabase.storage, i.ImageUrl) });
                ImgCarouselView.ItemsSource = Images = new ObservableCollection<NoteImage>(note.Images);
            }

            BindingContext = note;
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
                App.Database.DeleteNoteImage(note.Images[ImgCarouselView.Position].Id);
                note.Images.Remove(note.Images[ImgCarouselView.Position]);
                Images.Remove(Images[ImgCarouselView.Position]);
                ImgCarouselView.ItemsSource = Images;
                if (Images.Count == 0)
                {
                    row2.Height = 0;
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
                MessagingCenter.Unsubscribe<Application, List<string>>(Application.Current, "ImagesSelectedAndroid");
                MessagingCenter.Subscribe<Application, List<string>>(Application.Current, "ImagesSelectedAndroid", async (s, images) =>
                {
                    if (images.Count > 0 && Connectivity.NetworkAccess != NetworkAccess.None)
                    {
                        imgstack.IsVisible = true;
                        spinner.IsVisible = true;
                        spinner.IsRunning = true;
                        note.Note = await App.Database.SaveNoteAsync(note.Note, organization);
                        List<NoteImage> imgs = (await App.Database.SaveNoteImages(images, note.Note)).
                        ConvertAll(i => new NoteImage() { ImageUrl = Path.Combine(App.ASPDatabase.storage, i.ImageUrl), Id = i.Id });
                        note.Images.AddRange(imgs);
                        Images = new ObservableCollection<NoteImage>(note.Images);
                        row2.Height = 570;
                        ImgCarouselView.ItemsSource = Images;
                        spinner.IsRunning = false;
                        spinner.IsVisible = false;
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't add images now.");
                    }
                });
            }
        }
        /// <summary>
        /// Сохранение 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void SaveNote(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                if (App.CurrentOrganization != null)
                {
                    note.Note.UserName = App.CurrentOrganization.Name;
                    note.Note.UserId = App.CurrentOrganization.Id;
                    note.Note.Icon = App.CurrentOrganization.ImageUrl;
                    note.Note.ImageId = App.CurrentOrganization.ImageId;
                }
                else
                {
                    note.Note.UserName = App.CurrentUser.Login;
                    note.Note.UserId = App.CurrentUser.Id;
                    note.Note.Icon = App.CurrentUser.ImageUrl;
                    note.Note.ImageId = App.CurrentUser.ImageId;
                }

                note.Note.Info = input.Text;
                note.Note.Data = DateTime.Now.ToShortDateString();
                await App.Database.SaveNoteAsync(note.Note, organization);
                isSaved = true;
                await Navigation.PopAsync();
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't save this note now.");
            }
        }
        /// <summary>
        /// Удаление записи, если оноа не была сохранена до конца
        /// </summary>
        internal void Popped()
        {
            if (!isSaved && note.Note != null && note.Note.Id != 0)
            {
                App.Database.DeleteNoteAsync(note.Note);
            }
        }
        /// <summary>
        /// изменение размера поля ввода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoteTextChanged(object sender, TextChangedEventArgs e)
        {
            row1.Height = note.Size + 23 * e.NewTextValue.Length / 38;
            input.HeightRequest = note.Size + 23 * e.NewTextValue.Length / 38;
        }
    }
}