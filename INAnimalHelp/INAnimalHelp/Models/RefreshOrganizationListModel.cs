using INAnimalHelp.Models.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace INAnimalHelp.Models
{
    public class RefreshOrganizationListModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> _isRefreshing;
        public bool IsRefreshing => _isRefreshing.Value;
        public ReactiveCommand<EventArgs, Unit> RefreshCommand { get; set; }

        public ObservableCollection<Organization> Organizations;
        public string organization;
        public string animal;
        public bool isUser = false;

        public RefreshOrganizationListModel()
        {
            RefreshCommand = ReactiveCommand.CreateFromTask<EventArgs>(async args => await RefreshItemsAsync(), outputScheduler: RxApp.MainThreadScheduler);

            _isRefreshing =
                this.WhenAnyObservable(x => x.RefreshCommand.IsExecuting)
                    .StartWith(false)
                    .DistinctUntilChanged()
                    .ToProperty(this, nameof(IsRefreshing), scheduler: RxApp.MainThreadScheduler);
        }


        async Task RefreshItemsAsync()
        {
            await Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(3));
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                List<Organization> orgs = await App.ASPDatabase.GetOrganizations();

                if (isUser)
                {
                    List<UserOrganization> deps = (await App.ASPDatabase.GetDependences()).Where(d => d.UserId == App.CurrentUser.Id).ToList();
                    deps.ForEach(d => App.Database.SubscribeAsync(d));
                    orgs.ForEach(o =>
                    {
                        if (!deps.Any(d => d.OrganizationId == o.Id))
                        {
                            orgs.Remove(o);
                        }
                    });
                }
                else
                {
                    if (animal == null && organization != null)
                    {
                        orgs = orgs.Where(item => item.OrganizationType == organization).ToList();
                    }

                    else if (organization == null && animal != null)
                    {
                        orgs = orgs.Where(item => item.AnimalType == animal).ToList();

                    }
                    else if (animal != null && organization != null)
                    {
                        orgs = orgs.Where((item) => item.AnimalType == animal && item.OrganizationType == organization).ToList();
                    }
                }
                orgs.ForEach(o => o.ImageUrl = o.ImageId == 0 ? o.ImageUrl : Path.Combine(App.ASPDatabase.storage, o.ImageUrl));
                Organizations = new ObservableCollection<Organization>(orgs);
            }
            else
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. The page can't be updated at this time.");
        }
    }
}
