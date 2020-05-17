using INAnimalHelp.Models.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace INAnimalHelp.Models
{
    public class RefreshEventPageModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> _isRefreshing;
        public bool IsRefreshing => _isRefreshing.Value;
        public ReactiveCommand<EventArgs, Unit> RefreshCommand { get; set; }

        public EventModel Event;
        public bool ImageVisibility;

        public RefreshEventPageModel()
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
                Event.Event = await App.ASPDatabase.GetEvent(Event.Event.Id);
                await App.Database.SaveEventAsync(Event.Event);
                Event.Images = (await App.ASPDatabase.GetEventImages()).Where(i => i.EventId == Event.Event.Id).ToList().ConvertAll(i =>
                    new EventImage() { ImageUrl = Path.Combine(App.ASPDatabase.storage, i.ImageUrl) });
                App.Database.SaveEventImages(Event.Images);
                Event.Height = Event.Images.Count != 0 ? 230 : 0;
                ImageVisibility = Event.Images.Count != 0;
            }
            else
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. The page can't be updated at this time.");
        }

    }
}
