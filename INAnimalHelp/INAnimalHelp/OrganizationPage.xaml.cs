using INAnimalHelp.Models;
using INAnimalHelp.Models.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrganizationPage : ContentPage
    {
        List<User> subs;
        readonly RefreshOrganizationProfileModel model;
        /// <summary>
        /// Конструктор страницы организации
        /// </summary>
        /// <param name="organization">организация</param>
        public OrganizationPage(Organization organization)
        {
            InitializeComponent();
            //если пользователь не зарегестрирован, то создание записи недоступно
            new_note.IsEnabled = App.IsAuthenticated;
            //переход на страницы мероприятий и информации
            infolabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => Info())
            });
            eventslabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => Events())
            });
            Title = organization.Name;
            //если пользователь - организация, то подписка недоступна
            subStack.IsVisible = App.CurrentOrganization == null;

            BindingContext = model = new RefreshOrganizationProfileModel() { Organization = organization };
            subscribe.Text = model.SubText;
        }
        /// <summary>
        /// Конструктор профиля организации
        /// </summary>
        public OrganizationPage()
        {

            InitializeComponent();
            //переход на страницы мероприятий и информации
            infolabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => Info())
            });
            eventslabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => Events())
            });
            //настройки
            ToolbarItem item = new ToolbarItem() { IconImageSource = "settings.png" };
            item.Clicked += Settings;
            ToolbarItems.Add(item);

            Title = App.CurrentOrganization.Name;
            subStack.IsVisible = false;
            new_note.IsEnabled = true;

            BindingContext = model = new RefreshOrganizationProfileModel() { Organization = App.CurrentOrganization };
        }
        /// <summary>
        /// Переход на страницу создания записи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void NewNote(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                await Navigation.PushAsync(new NoteCreation(model.Organization));
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't create note now.");
            }
        }
        /// <summary>
        /// Переход на страницу настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Settings(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OrganizationCreation(true, false, App.CurrentOrganization));
        }
        /// <summary>
        /// Переход на страницу информации организации
        /// </summary>
        private async void Info()
        {
            try
            {
                await Navigation.PushAsync(new InformationPage(model.Organization));
            }
            catch (Exception ex) { await DisplayAlert("", ex.Message, "OK"); }
        }
        /// <summary>
        /// Переход на страницу списка мероприятий организации
        /// </summary>
        private async void Events()
        {
            if (App.CurrentOrganization == null)
            {
                await Navigation.PushAsync(new EventList(model.Organization));
            }
            else
            {
                await Navigation.PushAsync(new EventList(App.CurrentOrganization));
            }
        }
        /// <summary>
        /// Подписка или отписка от сраницы организации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Subscribe(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                foreach (User sub in subs)
                {
                    //если пользователь подписан, то отписывается
                    if (App.CurrentUser.Id == sub.Id)
                    {
                        subscribe.Text = model.SubText = "SUBSCRIBE";
                        App.Database.UnsubscribeAsync(model.Organization.Id);
                        subs.Remove(App.CurrentUser);
                        return;
                    }
                }

                subscribe.Text = model.SubText = "UNSUBSCRIBE";
                App.Database.SubscribeAsync(model.Organization.Id);
                subs.Add(App.CurrentUser);
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't subscribe to organization now.");
            }
        }

        /// <summary>
        /// Настройки записи организации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void NoteSettings(object sender, EventArgs e)
        {
            //получение информации о записи
            StackLayout stack = (((sender as ImageButton).Parent as StackLayout).Parent as StackLayout).Children[1] as StackLayout;
            Label label = stack.Children[0] as Label;
            Label data = stack.Children[1] as Label;
            stack = stack.Parent as StackLayout;
            Label about = ((stack.Parent as Grid).Children[2] as StackLayout).Children[0] as Label;
            NoteModel note = model.Notes.Where(i => i.Note.Info == about.Text && i.Note.UserName == label.Text && i.Note.Data == data.Text).FirstOrDefault();
            //если текущий пользователь создал запись
            if (App.CurrentUser != null && label.Text == App.CurrentUser.Login)
            {
                string result = await DisplayActionSheet(null, "Cancel", null, "Delete note", "Edit note");
                if (result == "Delete note")
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.None)
                    {
                        App.Database.DeleteNoteAsync(note.Note);
                        model.Notes.Remove(note);
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't delete this note now.");
                    }
                }
                else if (result == "Edit note")
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.None)
                    {
                        note.Images = await App.Database.GetNoteImages(note.Note);
                        await Navigation.PushAsync(new NoteCreation(note, model.Organization));
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't edit this note now.");
                    }
                }
            }
            //если это страница организации пользователя, но не он создал запись
            else if (App.CurrentOrganization != null && model.Organization.Name != App.CurrentOrganization.Name)
            {
                string result = await DisplayActionSheet(null, "Cancel", null, "Delete note");
                if (result == "Delete note")
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.None)
                    {
                        App.Database.DeleteNoteAsync(note.Note);
                        model.Notes.Remove(note);
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't delete this note now.");
                    }
                }
            }
            //если это страница организации пользователя, и он создал запись
            else if (App.CurrentOrganization != null && label.Text == App.CurrentOrganization.Name)
            {
                string result = await DisplayActionSheet(null, "Cancel", null, "Delete note", "Edit note");
                if (result == "Delete note")
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.None)
                    {
                        App.Database.DeleteNoteAsync(note.Note);
                        model.Notes.Remove(note);
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't delete this note now.");
                    }
                }
                else if (result == "Edit note")
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.None)
                    {
                        note.Images = await App.Database.GetNoteImages(note.Note);
                        await Navigation.PushAsync(new NoteCreation(note, model.Organization));
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show("Internet connection is unavailable. You can't edit this note now.");
                    }
                }
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //если организация была удалена, то переходит на страницу аутентификации
            if (App.IsAuthenticated &&
                App.CurrentOrganization == null &&
                App.CurrentUser == null &&
                (await App.ASPDatabase.GetOrganization(App.CurrentOrganization.Id)) == null)
            {
                App.CurrentOrganization = null;
                App.IsAuthenticated = false;
                await Navigation.PushModalAsync(new AppShell(false));
                App.Current.MainPage = new AppShell(false);
                (Application.Current as App).SaveJSON();
            }
            else
            {
                //записи организации
                List<Note> note = await App.Database.GetOrganizationNotes(model.Organization);
                model.Notes = new ObservableCollection<NoteModel>();

                for (int i = note.Count - 1; i >= 0; i--)
                {
                    List<NoteImage> images = (await App.Database.GetNoteImages(note[i])).ConvertAll(ii => new NoteImage() { ImageUrl = Path.Combine(App.ASPDatabase.storage, ii.ImageUrl) });
                    model.Notes.Add(new NoteModel()
                    {
                        Note = note[i],
                        Images = images,
                        HasImages = images.Count != 0,
                        Height = images.Count != 0 ? 330 : 0,
                        Icon = note[i].ImageId == 0 ? note[i].Icon : Path.Combine(App.ASPDatabase.storage, note[i].Icon)
                    });
                }
                //если пользователь авторизирован, то подписка доступна
                subscribe.IsEnabled = App.CurrentOrganization == null && App.CurrentUser != null;
                if (App.CurrentUser != null)
                {
                    //подписчики организации
                    subs = await App.Database.GetSubscribers(model.Organization);
                    foreach (User sub in subs)
                    {
                        //если пользователь подписан
                        if (App.CurrentUser.Id == sub.Id)
                        {
                            subscribe.Text = model.SubText = "UNSUBSCRIBE";
                        }
                    }
                }
                //информация организации
                name.Text = model.Organization.Name;
                image.Source = model.Organization.ImageId == 0 ? model.Organization.ImageUrl : Path.Combine(App.ASPDatabase.storage, model.Organization.ImageUrl);
                wall.ItemsSource = model.Notes;
                wall.BindingContext = model.Organization;
            }
        }
    }
}