using INAnimalHelp.Models;
using INAnimalHelp.Models.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace INAnimalHelp
{
    public partial class App : Application
    {
        public readonly string statusPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "status.json");
        static DatabaseService database;
        static ASPDatabaseService ASPdatabase;
        public static Organization CurrentOrganization { get; set; }
        public static User CurrentUser { get; set; }
        public static bool IsAuthenticated { get; set; }
        public static List<User> MyUserProfiles { get; set; } = new List<User>();
        public static List<Organization> MyOrganizationProfiles { get; set; } = new List<Organization>();


        public static DatabaseService Database
        {
            get {
                if (database == null)
                {
                    database = new DatabaseService();
                }
                return database;
            }
        }

        public static ASPDatabaseService ASPDatabase
        {
            get {
                if (ASPdatabase == null)
                {
                    ASPdatabase = new ASPDatabaseService();
                }
                return ASPdatabase;
            }
        }

        public App()
        {
            if (File.Exists((Application.Current as App).statusPath))
            {
                try
                {
                    using (StreamReader reader = new StreamReader((Application.Current as App).statusPath))
                    {
                        App.IsAuthenticated = JsonConvert.DeserializeObject<bool>(reader.ReadLine());
                        App.CurrentUser = JsonConvert.DeserializeObject<User>(reader.ReadLine());
                        App.CurrentOrganization = JsonConvert.DeserializeObject<Organization>(reader.ReadLine());
                        App.MyUserProfiles = JsonConvert.DeserializeObject<List<User>>(reader.ReadLine());
                        App.MyOrganizationProfiles = JsonConvert.DeserializeObject<List<Organization>>(reader.ReadLine());
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            RefreshData();
            MainPage = new AppShell(true);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public void SaveJSON()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(statusPath))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(IsAuthenticated));
                    writer.WriteLine(JsonConvert.SerializeObject(CurrentUser));
                    writer.WriteLine(JsonConvert.SerializeObject(CurrentOrganization));
                    writer.WriteLine(JsonConvert.SerializeObject(MyUserProfiles));
                    writer.WriteLine(JsonConvert.SerializeObject(MyOrganizationProfiles));
                }
            }
            catch (System.UnauthorizedAccessException e)
            {
                System.Console.WriteLine(e.Message);
            }
            catch (IOException e)
            {
                System.Console.WriteLine(e.Message);
            }
            catch (System.Security.SecurityException e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        public async void RefreshData()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                List<EventImage> evimages = await App.ASPDatabase.GetEventImages();
                List<NoteImage> nimages = await App.ASPDatabase.GetNoteImages();
                List<Organization> orgs = await App.ASPDatabase.GetOrganizations();
                List<Note> notes = await App.ASPDatabase.GetNotes();
                List<Event> events = await App.ASPDatabase.GetEvents();
                List<UserOrganization> depends = await App.ASPDatabase.GetDependences();
                List<User> users = await App.ASPDatabase.GetUsers();

                orgs.ForEach(async o =>
                {
                    if (!File.Exists(Path.Combine(App.ASPDatabase.storage, o.ImageUrl)) && o.ImageId != 0)
                        await ASPdatabase.SetPath(o.ImageId);

                    (await Database.GetOrganizationNotes(o)).ForEach(n =>
                    {
                        if (!notes.Any(nn => nn.Id == n.Id))
                        {
                            Database.DeleteNoteAsync(n);
                        }
                    });

                    (await Database.GetOrganizationEvents(o)).ForEach(ev =>
                    {
                        if (!events.Any(e => e.Id == ev.Id))
                        {
                            Database.DeleteEventAsync(ev);
                        }
                    });

                    (await Database.GetOrganizationSubscribesAsync(o)).ForEach(d =>
                    {
                        if (!depends.Any(dd => d.Id == dd.Id))
                            Database.UnsubscribeAsync(d.Id);
                    });

                    await Database.SaveOrganizationAsync(o);

                    events.Where(e => e.OrganizationId == o.Id).ToList().ForEach(async e =>
                    {
                        List<EventImage> images = evimages.Where(i => i.EventId == e.Id).ToList();
                        await Database.SaveEventAsync(e);

                        (await Database.GetEventImages(e)).ForEach(i =>
                        {
                            if (!images.Any(ii => ii.Id == i.Id))
                            {
                                Database.DeleteEventImage(i.Id);
                            }
                        });

                        images.ForEach(async i =>
                        {
                            if (!File.Exists(Path.Combine(App.ASPDatabase.storage, i.ImageUrl)) && i.ImageId != 0)
                            {
                                await ASPdatabase.SetPath(i.ImageId);
                            }
                        });
                        Database.SaveEventImages(images);
                    });

                    notes.Where(n => n.OrganizationId == o.Id).ToList().ForEach(async n =>
                    {
                        await Database.SaveNoteAsync(n, o);

                        if (!File.Exists(Path.Combine(App.ASPDatabase.storage, n.Icon)) && n.ImageId != 0)
                        {
                            await ASPdatabase.SetPath(n.ImageId);
                        }

                        List<NoteImage> images = nimages.Where(ii => ii.NoteId == n.Id).ToList();

                        (await Database.GetNoteImages(n)).ForEach(i =>
                        {
                            if (!images.Any(ii => ii.Id == i.Id))
                            {
                                Database.DeleteNoteImage(i.Id);
                            }
                        });

                        images.ForEach(async i =>
                        {
                            if (!File.Exists(Path.Combine(App.ASPDatabase.storage, i.ImageUrl)) && i.ImageId != 0)
                            {
                                await ASPdatabase.SetPath(i.ImageId);
                            }
                        });
                        Database.SaveNoteImages(images);
                    });

                    depends.Where(d => d.OrganizationId == o.Id).ToList().ForEach(d =>
                    {
                        Database.SubscribeAsync(d);
                    });

                });

                users.ForEach(async u =>
                {
                    await Database.SaveUserAsync(u);
                    if (!File.Exists(Path.Combine(App.ASPDatabase.storage, u.ImageUrl)) && u.ImageId != 0)
                    {
                        await ASPdatabase.SetPath(u.ImageId);
                    }
                });

                (await Database.GetUsersAsync()).ForEach(u =>
                {
                    if (!users.Any(uu => uu.Id == u.Id))
                    {
                        Database.DeleteUserAsync(u);
                    }
                });

                (await Database.GetOrganizationsAsync()).ForEach(o =>
                {
                    if (!orgs.Any(oo => oo.Id == o.Id))
                    {
                        Database.DeleteOrganizationAsync(o);
                    }
                });

                for (int i = MyOrganizationProfiles.Count - 1; i >= 0; i--)
                {
                    if (!(await Database.FindOrganization(MyOrganizationProfiles[i].OrganizationId, MyOrganizationProfiles[i].AuthType)))
                    {
                        MyOrganizationProfiles.Remove(MyOrganizationProfiles[i]);
                    }
                }

                for (int i = MyUserProfiles.Count - 1; i >= 0; i--)
                {
                    if (!(await Database.FindUser(MyUserProfiles[i].UserId, MyUserProfiles[i].AuthType)))
                    {
                        MyUserProfiles.Remove(MyUserProfiles[i]);
                    }
                }

                if (CurrentOrganization != null)
                {
                    CurrentOrganization = await Database.GetOrganizationAsync(CurrentOrganization.Id);
                }
                else if (CurrentUser != null)
                {
                    CurrentUser = await Database.GetUserAsync(CurrentUser.Id);
                }
            }
        }
    }
}
