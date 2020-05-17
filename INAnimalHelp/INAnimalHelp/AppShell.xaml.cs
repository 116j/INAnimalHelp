
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Xamarin.Forms.TabbedPage
    {
        //первая вкладка
        readonly NavigationPage navigation2;
        //вторая вкладка
        NavigationPage navigation1;
        public AppShell(bool isbegining)
        {
            InitializeComponent();
            //установка меню снизу
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            navigation2 = new NavigationPage(new OrganizationList(isbegining)) { Title = "List", IconImageSource= "list.png" };

            if (!App.IsAuthenticated)
            {
                //если не авторизован, то на страницу аутентификации
                navigation1 = new NavigationPage(new AuthenticationPage());
            }
            else
            {
                //если авторизован, то на страницу профиля
                if (App.CurrentOrganization != null)
                { 
                        navigation1 = new NavigationPage(new OrganizationPage());
                }
                else
                {
                        navigation1 = new NavigationPage(new UserProfile());
                }
            }

            navigation1.Title = "Profile";
            navigation1.IconImageSource = "profile.png";
            Children.Add(navigation2);
            Children.Add(navigation1);
            CurrentPage = navigation1;
            //события закрытия страницы
            navigation2.Popped += (s, e) =>
            {
                if (e.Page is NoteCreation)
                {
                    var page = e.Page as NoteCreation;
                    page.Popped();
                }
                if (e.Page is EventCreation)
                {
                    var page = e.Page as EventCreation;
                    page.Popped();
                }
            };
            navigation1.Popped += (s, e) =>
            {
                if (e.Page is NoteCreation)
                {
                    var page = e.Page as NoteCreation;
                    page.Popped();
                }
                if (e.Page is EventCreation)
                {
                    var page = e.Page as EventCreation;
                    page.Popped();
                }
            };

        }
    }
}