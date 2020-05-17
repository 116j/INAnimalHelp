using INAnimalHelp.Models;
using System;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();

            BindingContext = App.CurrentUser;

            if (App.MyUserProfiles.Any(u => u.Id == App.CurrentUser.Id))
            {
                addprofile.Text = "Remove profile from my profile's list";
            }
        }
        /// <summary>
        /// Выбор картинки пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void SelectImage(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                Plugin.Media.Abstractions.MediaFile photo = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });
                if (photo == null)
                {
                    return;
                }
                //сохранение картинки на сервере
                Models.Models.Image file = await App.ASPDatabase.AddImage(photo.Path);
                App.CurrentUser.ImageUrl = file.ImageUrl;
                App.CurrentUser.ImageId = file.Id;
                image.Text = "Image set";
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't select new image now.");
            }
        }
        /// <summary>
        /// Выход из аккаунта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoginOut(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                //Выход в зависимости от типа авторизации
                switch (App.CurrentUser.AuthType)
                {
                    case "vk":
                        DependencyService.Get<IVkService>().Logout();
                        break;
                    default:
                        DependencyService.Get<IFacebookService>().Logout();
                        break;
                }
                //освобождение ресурсов для возвращения на страницу аутентификации
                App.IsAuthenticated = false;
                App.CurrentUser = null;
                //сохранение статуса пользователя
                (Application.Current as App).SaveJSON();
                App.Current.MainPage = new AppShell(false);
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't login out now.");
            }
        }
        /// <summary>
        /// Добавление или удаления аккаунта из списка аккаунтов пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AddToList(object sender, EventArgs e)
        {
            if (App.MyUserProfiles.Any(u => u.Id == App.CurrentUser.Id))
            {
                App.MyUserProfiles.Remove(App.MyUserProfiles.Where(u => u.Id == App.CurrentUser.Id).FirstOrDefault());
                addprofile.Text = "Add profile to my profile's list";
            }
            else
            {
                App.MyUserProfiles.Add(App.CurrentUser);
                addprofile.Text = "Remove profile from my profile's list";
            }
        }
        /// <summary>
        /// Удаление аккаунта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void Delete(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                //Убедиться, что пользователь не нажал на кнопку случайно
                bool result = await DisplayAlert("", "Are you sure you want to delete your profile?", "Yes", "No");
                if (result)
                {
                    //если не случайно, освобождаем все ресурсы
                    if (App.MyUserProfiles.Any(u => u.Id == App.CurrentUser.Id))
                    {
                        App.MyUserProfiles.Remove(App.MyUserProfiles.Where(u => u.Id == App.CurrentUser.Id).FirstOrDefault());
                    }

                    App.Database.DeleteUserAsync(App.CurrentUser);
                    LoginOut(sender, e);
                }
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't delete your profile now.");
            }
        }
        /// <summary>
        /// Сохранение изменений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void Save_Changes(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                App.CurrentUser.Login = login.Text;
                (Application.Current as App).SaveJSON();
                App.CurrentUser = await App.Database.SaveUserAsync(App.CurrentUser);
                App.Current.MainPage = new AppShell(false);
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't save changes now.");
            }
        }
    }
}