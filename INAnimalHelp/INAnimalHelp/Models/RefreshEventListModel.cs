using INAnimalHelp.Models.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace INAnimalHelp.Models
{
    public class RefreshEventListModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> _isRefreshing;
        public bool IsRefreshing => _isRefreshing.Value;
        public ReactiveCommand<EventArgs, Unit> RefreshCommand { get; set; }

        public ObservableCollection<Event> Events;
        public Organization Organization;

        public RefreshEventListModel()
        {
            RefreshCommand = ReactiveCommand.CreateFromTask<EventArgs>(async args => await RefreshItemsAsync(), outputScheduler: RxApp.MainThreadScheduler);

            _isRefreshing =
                this.WhenAnyObservable(x => x.RefreshCommand.IsExecuting)
                    .StartWith(false)
                    .DistinctUntilChanged()
                    .ToProperty(this, nameof(IsRefreshing), scheduler: RxApp.MainThreadScheduler);
        }

        private async Task RefreshItemsAsync()
        {
            await Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(3));
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                Events = new ObservableCollection<Event>();
                if (Organization == null)
                {
                    List<UserOrganization> depends = (await App.ASPDatabase.GetDependences()).Where(d => d.UserId == App.CurrentUser.Id).ToList();
                    List<Event> events = new List<Event>();
                    depends.ForEach(async d => events.AddRange((await App.ASPDatabase.GetEvents()).Where(e => e.OrganizationId == d.OrganizationId).ToList()));
                    Events = new ObservableCollection<Event>(events);
                }
                else
                {
                    Events = new ObservableCollection<Event>((await App.ASPDatabase.GetEvents()).Where(e => e.OrganizationId == Organization.Id).ToList());
                }
            }
            else
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. The page can't be updated at this time.");

        }
    }
}
