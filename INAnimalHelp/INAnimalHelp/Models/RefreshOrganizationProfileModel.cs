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
    public class RefreshOrganizationProfileModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> _isRefreshing;
        public bool IsRefreshing => _isRefreshing.Value;
        public ReactiveCommand<EventArgs, Unit> RefreshCommand { get; set; }

        public ObservableCollection<NoteModel> Notes = new ObservableCollection<NoteModel>();
        public Organization Organization;
        public string SubText = "SUBSCRIBE";

        public RefreshOrganizationProfileModel()
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
            await Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(10));
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                Organization = await App.ASPDatabase.GetOrganization(Organization.Id);
                await App.Database.SaveOrganizationAsync(Organization);
                Notes = new ObservableCollection<NoteModel>();
                List<Note> note = (await App.ASPDatabase.GetNotes()).Where(n => n.OrganizationId == Organization.Id).ToList();
                for (int i = note.Count - 1; i >= 0; i--)
                {
                    List<NoteImage> images = (await App.ASPDatabase.GetNoteImages()).ConvertAll(ii => new NoteImage() { ImageUrl = Path.Combine(App.ASPDatabase.storage, ii.ImageUrl) });
                    App.Database.SaveNoteImages(images);
                    Notes.Add(new NoteModel()
                    {
                        Note = note[i],
                        Images = images,
                        HasImages = images.Count != 0,
                        Height = images.Count != 0 ? 330 : 0,
                        Icon = note[i].ImageId == 0 ? note[i].Icon : Path.Combine(App.ASPDatabase.storage, note[i].Icon)

                    });
                }
                if (App.CurrentUser != null)
                {
                    List<UserOrganization> subs = (await App.ASPDatabase.GetDependences()).Where(d => d.OrganizationId == Organization.Id).ToList();
                    foreach (UserOrganization sub in subs)
                    {
                        App.Database.SubscribeAsync(sub);
                        if (App.CurrentUser.Id == sub.Id)
                        {
                            SubText = "UNSUBSCRIBE";
                        }
                    }
                }
            }
            else
            {
                DependencyService.Get<IToast>().Show("Internet connection is unavailable. The page can't be updated at this time.");
            }
        }
    }
}
