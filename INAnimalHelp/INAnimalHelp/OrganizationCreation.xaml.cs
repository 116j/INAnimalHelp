using INAnimalHelp.Models;
using INAnimalHelp.Models.Models;
using System;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrganizationCreation : ContentPage
    {
        readonly bool isSetting;
        bool isSaved = false;
        readonly string[] orgPicker = { "Shelter", "Team", "Fund" };
        readonly string[] anPicker = { "Cats", "Dogs", "Horses", "Rodents", "Reptiles", "Farm animals", "Wild animals" };
        //переменная для локального сохранения результатов при переходе на страницу выбора адреса
        readonly Organization organization;

        public OrganizationCreation(bool isSetting, bool isAdressSet, Organization org)
        {
            InitializeComponent();
            this.isSetting = isSetting;
            organization = org;
            //видимость кнопок
            logout.IsVisible = isSetting;
            addprofile.IsVisible = isSetting;
            //название кнопкки
            if (App.MyOrganizationProfiles.Any(o => o.Id == App.CurrentOrganization.Id))
            {
                addprofile.Text = "Remove profile from my profile's list";
            }
            if (isSetting)
            {
                ToolbarItem delete = new ToolbarItem()
                {
                    IconImageSource = "delete.png"
                };
                delete.Clicked += DeleteOrganization;
                ToolbarItems.Add(delete);
            }

            //ресурсы пикеров
            organizationPicker.ItemsSource = orgPicker;
            animalPicker.ItemsSource = anPicker;

            BindingContext = org;
        }
        /// <summary>
        /// Выбор типа животного
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetAnimalType(object sender, EventArgs e)
        {
            App.CurrentOrganization.AnimalType = animalPicker.SelectedItem.ToString();
        }
        /// <summary>
        /// Выбор типа организации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetOrganizationType(object sender, EventArgs e)
        {
            App.CurrentOrganization.OrganizationType = organizationPicker.SelectedItem.ToString();
        }

        /// <summary>
        /// Выбор картинки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void SelectImage(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                Plugin.Media.Abstractions.MediaFile photo = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,

                });
                if (photo == null)
                {
                    return;
                }
                //сохранение картинки на сервере
                Models.Models.Image file = await App.ASPDatabase.AddImage(photo.Path);
                App.CurrentOrganization.ImageId = file.Id;
                App.CurrentOrganization.ImageUrl = file.ImageUrl;
                image.Text = "Image set";
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't select new image now.");
            }
        }

        /// <summary>
        /// Переход на страницу выбора адреса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void ChooseOrganizationAdress(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                //сохранение данных в локальную переменную 
                organization.Name = name.Text;
                organization.About = about.Text;
                organization.Site = site.Text;
                organization.Number = number.Text;
                organization.Email = email.Text;
                organization.Instagram = instagram.Text;
                organization.Vkontakte = vk.Text;
                organization.Facebook = fb.Text;

                await Navigation.PushModalAsync(new AdressSearcher(isSetting, organization));
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't choose adress now.");
            }
        }

        async void SaveOrganization(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                //проверка имени
                if (name.Text == "" || name.Text == null)
                {
                    name.Placeholder = "Name can't be empty!";
                    return;
                }
                //запоминание значений
                App.CurrentOrganization.Name = name.Text;
                App.CurrentOrganization.About = about.Text;
                App.CurrentOrganization.Site = site.Text;
                App.CurrentOrganization.Number = number.Text;
                App.CurrentOrganization.Email = email.Text;
                App.CurrentOrganization.Instagram = instagram.Text;
                App.CurrentOrganization.Vkontakte = vk.Text;
                App.CurrentOrganization.Facebook = fb.Text;
                App.IsAuthenticated = true;

                //сохранение данных локально и на сервере
                App.CurrentOrganization = await App.Database.SaveOrganizationAsync(App.CurrentOrganization);
                (Application.Current as App).SaveJSON();
                isSaved = true;
                App.Current.MainPage = new AppShell(false);
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't save your data now.");
            }
        }
        /// <summary>
        /// Удаление щрганизации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void DeleteOrganization(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                //Убедиться, что пользователь не нажал на кнопку случайно
                bool result = await DisplayAlert("", "Are you sure you want to delete your organization's profile?", "Yes", "No");
                if (result)
                {
                    //если не случайно, освобождаем все ресурсы
                    if (App.MyOrganizationProfiles.Any(o => o.Id == App.CurrentOrganization.Id))
                    {
                        App.MyOrganizationProfiles.Remove(App.MyOrganizationProfiles.Where(o => o.Id == App.CurrentOrganization.Id).FirstOrDefault());
                    }

                    App.Database.DeleteOrganizationAsync(App.CurrentOrganization);
                    LoginOut(sender, e);
                }
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't delete your profile now.");
            }
        }
        /// <summary>
        /// Выход из профиля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoginOut(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                //Выход в зависимости от типа авторизации
                switch (App.CurrentOrganization.AuthType)
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
                App.CurrentOrganization = null;
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
        /// Добавление профиля в личный список аккаунтов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AddToList(object sender, EventArgs e)
        {
            if (App.MyOrganizationProfiles.Any(o => o.Id == App.CurrentOrganization.Id))
            {
                App.MyOrganizationProfiles.Remove(App.MyOrganizationProfiles.Where(o => o.Id == App.CurrentOrganization.Id).FirstOrDefault());
                addprofile.Text = "Add profile to my profile's list";
            }
            else
            {
                App.MyOrganizationProfiles.Add(App.CurrentOrganization);
                addprofile.Text = "Remove profile from my profile's list";
            }
        }
        /// <summary>
        /// Освобождение локальной переменной при выходе, 
        /// если создание организации было отменено и 
        /// она не была сохранена
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (!isSetting && isSaved)
            {
                App.CurrentOrganization = null;
            }
        }
    }
}