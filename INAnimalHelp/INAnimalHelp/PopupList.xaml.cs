using INAnimalHelp.Models;
using INAnimalHelp.Models.Models;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupList : PopupPage
    {
        public PopupList(string authType, string userType, List<User> users = null, List<Organization> orgs = null)
        {
            //выход при нажатии на задний фон
            BackgroundClicked += async (s, e) =>
            {
                await Navigation.PopAllPopupAsync();
            };
            InitializeComponent();
            switch (userType)
            {
                case "user":
                    //аккаунты пользователя
                    users.ForEach(u => u.ImageUrl = u.ImageId == 0 ? u.ImageUrl : Path.Combine(App.ASPDatabase.storage, u.ImageUrl));
                    list.ItemsSource = users;
                    switch (authType)
                    {
                        case "vk":
                            //инициализация комманды
                            SignIn = async () =>
                            {
                                //выход из аккаунтов пользователя
                                await Navigation.PopAllPopupAsync();

                                LoginResult loginResult = await Login("vk");
                                switch (loginResult.LoginState)
                                {
                                    case LoginState.Success:

                                        App.CurrentUser = new User()
                                        {
                                            ImageUrl = loginResult.ImageUrl,
                                            UserId = loginResult.UserId,
                                            Login = loginResult.FirstName,
                                            AuthType = "vk"
                                        };
                                        App.IsAuthenticated = true;
                                        //если аккаунт существует
                                        if (await App.Database.FindUser(App.CurrentUser.UserId, "vk"))
                                        {
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
                            };
                            break;
                        case "fb":
                            //инициализация комманды
                            SignIn = async () =>
                            {
                                //выход из аккаунтов пользователя
                                await Navigation.PopAllPopupAsync();

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
                                        //если аккаунт существует
                                        if (await App.Database.FindUser(App.CurrentUser.UserId, "fb"))
                                        {
                                            App.IsAuthenticated = true;

                                            App.CurrentUser = await App.Database.GetUserById(App.CurrentUser.UserId, "fb");
                                        }
                                        else
                                        {
                                            //переход на страницу создания организации
                                            App.CurrentUser = await App.Database.SaveUserAsync(App.CurrentUser);
                                        }
                                        (Application.Current as App).SaveJSON();

                                        App.Current.MainPage = new AppShell(false);

                                        break;
                                    case LoginState.Failed:
                                        await DisplayAlert("Authentication error", "Failed: " + loginResult.ErrorString, "OK");
                                        break;
                                }
                            };
                            break;
                    }
                    break;
                case "org":
                    //аккаунты пользователя
                    orgs.ForEach(o => o.ImageUrl = o.ImageId == 0 ? o.ImageUrl : Path.Combine(App.ASPDatabase.storage, o.ImageUrl));
                    list.ItemsSource = orgs;
                    switch (authType)
                    {
                        case "vk":
                            //инициализация комманды
                            SignIn = async () =>
                            {
                                //выход из аккаунтов пользователя
                                await Navigation.PopAllPopupAsync();

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
                                        //если аккаунт существует
                                        if (await App.Database.FindOrganization(App.CurrentOrganization.OrganizationId, "vk"))
                                        {
                                            App.IsAuthenticated = true;

                                            App.CurrentOrganization = await App.Database.GetOrganizationById(App.CurrentOrganization.OrganizationId, "vk");
                                            (Application.Current as App).SaveJSON();

                                            App.Current.MainPage = new AppShell(false);
                                        }
                                        else
                                        {
                                            //создание нового аккаунта
                                            await Navigation.PushAsync(new OrganizationCreation(false, false, new Organization()));
                                        }
                                        break;
                                    case LoginState.Failed:
                                        await DisplayAlert("Authentication error", "Failed: " + loginResult.ErrorString, "OK");
                                        break;
                                }
                            };
                            break;
                        case "fb":
                            //инициализация комманды
                            SignIn = async () =>
                            {
                                //выход из аккаунтов пользователя
                                await Navigation.PopAllPopupAsync();

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
                                        //если аккаунт существует
                                        if (await App.Database.FindOrganization(App.CurrentOrganization.OrganizationId, "fb"))
                                        {
                                            App.IsAuthenticated = true;

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
                            };
                            break;
                    }
                    break;
            }
            //команда входа в другой аккаунт
            label.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(SignIn)
            });
        }
        /// <summary>
        /// Переходит на профиль из списка профилей пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void GoToAccount(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Organization)
            {
                App.CurrentOrganization = await App.Database.GetOrganizationAsync((e.Item as Organization).Id);
            }
            else if (e.Item is User)
            {
                App.CurrentUser = await App.Database.GetUserAsync((e.Item as User).Id);
            }
            App.IsAuthenticated = true;
            await Navigation.PopAllPopupAsync();
            App.Current.MainPage = new AppShell(false);
            (Application.Current as App).SaveJSON();
        }

        readonly Action SignIn;
        /// <summary>
        /// Аутентификация
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