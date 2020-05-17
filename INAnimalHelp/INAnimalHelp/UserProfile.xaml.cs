using System;
using System.IO;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserProfile : ContentPage
    {
        public UserProfile()
        {
            InitializeComponent();
            BindingContext = App.CurrentUser;
            Title = App.CurrentUser.Login;
        }
        /// <summary>
        /// Переход на страницу организаций пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Organizations_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OrganizationList());
        }
        /// <summary>
        /// Переход на страницу мероприятий организаций пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Events_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EventList());
        }
        /// <summary>
        /// Переход на страницу настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Settings_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings());
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (App.CurrentUser == null || (await App.ASPDatabase.GetUser(App.CurrentUser.Id)) == null)
            {
                App.CurrentUser = null;
                App.IsAuthenticated = false;
                await Navigation.PushModalAsync(new AppShell(false));
                App.Current.MainPage = new AppShell(false);
                (Application.Current as App).SaveJSON();
            }
            else
            {
                name.Text = App.CurrentUser.Login;
                image.Source = App.CurrentUser.ImageId == 0 ? App.CurrentUser.ImageUrl : Path.Combine(App.ASPDatabase.storage, App.CurrentUser.ImageUrl);
            }
        }
    }
}