using INAnimalHelp.Models;
using INAnimalHelp.Models.Models;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthenticationPage : ContentPage
    {
        public AuthenticationPage()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Вход в качестве пользователя через Вконтакте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void VKAuthUser(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                //переход к списку аккаунтов пользователя, если он не пуст
                if (App.MyUserProfiles != null)
                {
                    List<User> list = App.MyUserProfiles.Where(x => x.AuthType == "vk").ToList();
                    if (list.Count != 0)
                    {
                        await PopupNavigation.PushAsync(new PopupList("vk", "user", list));
                        return;
                    }
                }

                LoginResult loginResult = await Login("vk");
                switch (loginResult.LoginState)
                {
                    case LoginState.Success:

                        App.CurrentUser = new User()
                        {
                            ImageUrl = loginResult.ImageUrl,
                            UserId = loginResult.UserId,
                            Login = loginResult.FirstName,
                            Email = loginResult.Email,
                            AuthType = "vk"
                        };
                        App.IsAuthenticated = true;
                        if (await App.Database.FindUser(App.CurrentUser.UserId, "vk"))
                        {
                            //если аккаунт существует
                            App.CurrentUser = await App.Database.GetUserById(App.CurrentUser.UserId, "vk");
                        }
                        else
                        {
                            //создание нового аккаунта
                            App.CurrentUser = await App.Database.SaveUserAsync(App.CurrentUser);
                        }
                        (Application.Current as App).SaveJSON();
                        App.Current.MainPage = new AppShell(false);
                        break;
                    case LoginState.Failed:
                        await DisplayAlert("Authentication error", "Failed: " + loginResult.ErrorString, "OK");
                        break;
                }
            }
            else
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't sign up now.");
        }
        /// <summary>
        /// Вход в качестве организации через Вконтакте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void VKAuthOrganization(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                //переход к списку аккаунтов организаций пользователя, если он не пуст
                if (App.MyOrganizationProfiles != null)
                {
                    List<Organization> list = App.MyOrganizationProfiles.Where(x => x.AuthType == "vk").ToList();
                    if (list.Count != 0)
                    {
                        await PopupNavigation.PushAsync(new PopupList("vk", "org", null, list));
                        return;
                    }
                }
                LoginResult loginResult = await Login("vk");
                switch (loginResult.LoginState)
                {
                    case LoginState.Success:

                        App.CurrentOrganization = new Organization()
                        {
                            Email = loginResult.Email,
                            ImageUrl = loginResult.ImageUrl,
                            OrganizationId = loginResult.UserId,
                            AuthType = "vk"
                        };
                        App.IsAuthenticated = true;
                        if (await App.Database.FindOrganization(App.CurrentOrganization.OrganizationId, "vk"))
                        {
                            //если аккаунт существует
                            App.CurrentOrganization = await App.Database.GetOrganizationById(App.CurrentOrganization.OrganizationId, "vk");
                            (Application.Current as App).SaveJSON();
                            App.Current.MainPage = new AppShell(false);
                        }
                        else
                        {
                            //переход на страницу создания организации
                            await Navigation.PushAsync(new OrganizationCreation(false, false, new Organization()));
                        }
                        break;
                    case LoginState.Failed:
                        await DisplayAlert("Authentication error", "Failed: " + loginResult.ErrorString, "OK");
                        break;
                }
            }
            else
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't sign up now.");
        }
        /// <summary>
        /// Вход в качестве пользователя через Facebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FacebookAuthUser(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                //переход к списку аккаунтов пользователя, если он не пуст
                if (App.MyUserProfiles != null)
                {
                    List<User> list = App.MyUserProfiles.Where(x => x.AuthType == "fb").ToList();
                    if (list.Count != 0)
                    {
                        await PopupNavigation.PushAsync(new PopupList("fb", "user", list));
                        return;
                    }
                }
                LoginResult loginResult = await Login("");
                switch (loginResult.LoginState)
                {
                    case LoginState.Success:

                        App.CurrentUser = new User()
                        {
                            Email = loginResult.Email,
                            UserId = loginResult.UserId,
                            Login = loginResult.FirstName,
                            ImageUrl = loginResult.ImageUrl,
                            AuthType = "fb"
                        };
                        App.IsAuthenticated = true;
                        if (await App.Database.FindUser(App.CurrentUser.UserId, "fb"))
                        {
                            //если аккаунт существует
                            App.CurrentUser = await App.Database.GetUserById(App.CurrentUser.UserId, "fb");
                        }
                        else
                        {
                            //создание нового аккаунта
                            App.CurrentUser = await App.Database.SaveUserAsync(App.CurrentUser);
                        }
                        (Application.Current as App).SaveJSON();
                        App.Current.MainPage = new AppShell(false);
                        break;
                    case LoginState.Failed:
                        await DisplayAlert("Authentication error", "Failed: " + loginResult.ErrorString, "OK");
                        break;
                }
            }
            else
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't sign up now.");
        }
        /// <summary>
        /// Вход в качестве организации через Facebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FacebookAuthOrganization(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                //переход к списку аккаунтов организаций пользователя, если он не пуст
                if (App.MyOrganizationProfiles != null)
                {
                    List<Organization> list = App.MyOrganizationProfiles.Where(x => x.AuthType == "fb").ToList();
                    if (list.Count != 0)
                    {
                        await PopupNavigation.PushAsync(new PopupList("fb", "org", null, list));
                        return;
                    }
                }
                LoginResult loginResult = await Login("");
                switch (loginResult.LoginState)
                {
                    case LoginState.Success:
                        App.CurrentOrganization = new Organization()
                        {
                            Email = loginResult.Email,
                            ImageUrl = loginResult.ImageUrl,
                            OrganizationId = loginResult.UserId,
                            AuthType = "fb"
                        };
                        App.IsAuthenticated = true;
                        if (await App.Database.FindOrganization(App.CurrentOrganization.OrganizationId, "fb"))
                        {
                            //если аккаунт существует
                            App.CurrentOrganization = await App.Database.GetOrganizationById(App.CurrentOrganization.OrganizationId, "fb");
                            (Application.Current as App).SaveJSON();

                            App.Current.MainPage = new AppShell(false);
                        }
                        else
                        {
                            //переход на страницу создания организации
                            await Navigation.PushAsync(new OrganizationCreation(false, false, new Organization()));
                        }
                        break;
                    case LoginState.Failed:
                        await DisplayAlert("Authentication error", "Failed: " + loginResult.ErrorString, "OK");
                        break;
                }
            }
            else
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't sign up now.");
        }
        /// <summary>
        /// Аунтефикация
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        Task<LoginResult> Login(string providerName)
        {
            switch (providerName.ToLower())
            {
                case "vk":
                    return DependencyService.Get<IVkService>().Login();
                default:
                    return DependencyService.Get<IFacebookService>().Login();

            }
        }
    }
}