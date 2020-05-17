using INAnimalHelp.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InformationPage : ContentPage
    {
        public InformationPage(Organization organization)
        {
            InitializeComponent();
            BindingContext = organization;
            //команды перехода к сайтам или телефону
            site.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => { 
                    try
                    {
                        await Browser.OpenAsync(organization.Site, BrowserLaunchMode.SystemPreferred);
                    }
                    catch(UriFormatException)
                    {
                    }
                })
            }); 
            facebook.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => {
                    try
                    {
                        await Browser.OpenAsync(organization.Facebook, BrowserLaunchMode.SystemPreferred);
                    }
                    catch (UriFormatException)
                    {
                    }
                })
            });
            inst.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => {
                    try
                    {
                        await Browser.OpenAsync(organization.Instagram, BrowserLaunchMode.SystemPreferred);
                    }
                    catch (UriFormatException)
                    {
                    }
                })
            });
            vk.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => {
                    try
                    {
                        await Browser.OpenAsync(organization.Vkontakte, BrowserLaunchMode.SystemPreferred);
                    }
                    catch (UriFormatException)
                    {
                    }
                })
            });
            number.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => PhoneDialer.Open(organization.Number))
            });
        }
    }
}