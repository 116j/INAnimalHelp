using INAnimalHelp.Models.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace INAnimalHelp.Models
{
    //настройки базы данных
    public static class TaskExtensions
    {
        public static async void SafeFireAndForget(this Task task,
            bool returnToCallingContext,
            Action<Exception> onException = null)
        {
            try
            {
                await task.ConfigureAwait(returnToCallingContext);
            }
            catch (Exception ex) when (onException != null)
            {
                onException(ex);
            }
        }
    }
    public static class Constant
    {
        public const string DatabaseFilename = "Database.db3";

        public const SQLiteOpenFlags Flags =
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get {
                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }
    }

    public class DatabaseService
    {
        //инициализация бд
        readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constant.DatabasePath, Constant.Flags);
        });
        SQLiteAsyncConnection Database => lazyInitializer.Value;
        //индикаторы существования столов
        bool OrgsInitialized = false;
        bool NotesInitialized = false;
        bool NoteImagesInitialized = false;
        bool EventImagesInitialized = false;
        bool EventsInitialized = false;
        bool UsOrgsInitialized = false;
        bool UsersInitialized = false;

        public DatabaseService()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        //нахождение в бд по id авторизации и типу авторизации
        public async Task<bool> FindUser(string id, string auth)
        {
            return (await Database.Table<User>().ToListAsync()).Any(o => o.UserId == id && o.AuthType == auth);
        }

        public async Task<bool> FindOrganization(string id, string auth)
        {
            return (await Database.Table<Organization>().ToListAsync()).Any(o => o.OrganizationId == id && o.AuthType == auth);
        }


        //получения списка объектов
        public async Task<List<Organization>> GetOrganizationsAsync()
        {
            if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(Organization).Name))
            {
                await Database.CreateTablesAsync(CreateFlags.None, typeof(Organization)).ConfigureAwait(false);
            }
            return await Database.Table<Organization>().ToListAsync();
        }

        public async Task<List<User>> GetUsersAsync()
        {
            if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(User).Name))
            {
                await Database.CreateTablesAsync(CreateFlags.None, typeof(User)).ConfigureAwait(false);
            }
            return await Database.Table<User>().ToListAsync();
        }

        public async Task<List<UserOrganization>> GetOrganizationSubscribesAsync(Organization organization)
        {
            if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(UserOrganization).Name))
            {
                await Database.CreateTablesAsync(CreateFlags.None, typeof(UserOrganization)).ConfigureAwait(false);
            }
            return await Database.Table<UserOrganization>().Where(d => d.OrganizationId == organization.Id).ToListAsync();
        }

        public async Task<List<Event>> GetOrganizationEvents(Organization organization)
        {
            if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(Event).Name))
            {
                await Database.CreateTablesAsync(CreateFlags.None, typeof(Event)).ConfigureAwait(false);
            }
            return await Database.Table<Event>().Where(e => e.OrganizationId == organization.Id).ToListAsync();
        }

        public async Task<List<Event>> GetUserEvents()
        {
            List<Organization> orgs = await GetUserOrganizations();
            List<Event> events = new List<Event>();
            foreach (Organization org in orgs)
            {
                List<Event> evets = await GetOrganizationEvents(org);
                foreach (Event ev in evets)
                {
                    events.Add(ev);
                }
            }
            return events;
        }

        public async Task<List<Note>> GetOrganizationNotes(Organization organization)
        {
            if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(Note).Name))
            {
                await Database.CreateTablesAsync(CreateFlags.None, typeof(Note)).ConfigureAwait(false);
            }
            return await Database.Table<Note>().Where(n => n.OrganizationId == organization.Id).ToListAsync();
        }

        public async Task<List<Organization>> GetUserOrganizations()
        {
            List<UserOrganization> deps = await Database.Table<UserOrganization>().Where(d => d.UserId == App.CurrentUser.Id).ToListAsync();
            List<Organization> orgs = new List<Organization>();
            foreach (UserOrganization dep in deps)
            {
                orgs.Add(await GetOrganizationAsync(dep.OrganizationId));
            }
            return orgs;
        }

        public async Task<List<User>> GetSubscribers(Organization organization)
        {
            List<UserOrganization> deps = await Database.Table<UserOrganization>().Where(d => d.OrganizationId == organization.Id).ToListAsync();
            List<User> users = new List<User>();
            foreach (UserOrganization dep in deps)
            {
                users.Add(await GetUserAsync(dep.UserId));
            }
            return users;
        }

        public async Task<List<NoteImage>> GetNoteImages(Note note)
        {
            if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(NoteImage).Name))
            {
                await Database.CreateTablesAsync(CreateFlags.None, typeof(NoteImage)).ConfigureAwait(false);
            }
            return await Database.Table<NoteImage>().Where(i => i.NoteId == note.Id).ToListAsync();
        }

        public async Task<List<EventImage>> GetEventImages(Event ev)
        {
            if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(EventImage).Name))
            {
                await Database.CreateTablesAsync(CreateFlags.None, typeof(EventImage)).ConfigureAwait(false);
            }
            return await Database.Table<EventImage>().Where(i => i.EventId == ev.Id).ToListAsync();
        }
        //получает один объект по Id
        public Task<Organization> GetOrganizationAsync(int id)
        {
            return Database.Table<Organization>().Where(o => o.Id == id).FirstOrDefaultAsync();
        }

        public Task<User> GetUserAsync(int id)
        {
            return Database.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public Task<Event> GetEventAsync(int id)
        {
            return Database.Table<Event>().Where(e => e.Id == id).FirstOrDefaultAsync();
        }
        //получает юзера по Id и типу авторизации
        public Task<User> GetUserById(string id, string auth)
        {
            return Database.Table<User>().Where(o => o.UserId == id && o.AuthType == auth).FirstOrDefaultAsync();
        }
        //получение объектов по Id
        public Task<Organization> GetOrganizationById(string id, string auth)
        {
            return Database.Table<Organization>().Where(o => o.OrganizationId == id && o.AuthType == auth).FirstOrDefaultAsync();
        }

        //сохранение объектов
        public async Task<Organization> SaveOrganizationAsync(Organization organization)
        {
            if (organization.Id != 0)
            {
                if ((await Database.Table<Organization>().ToListAsync()).Any(o => o.Id == organization.Id))
                {
                    await Database.UpdateAsync(organization);
                    await App.ASPDatabase.UpdateOrganization(organization);
                }
                //если сохранен на сервере, но не в локальной бд
                else
                {
                    await Database.InsertAsync(organization);
                }
            }
            else
            {
                organization = await App.ASPDatabase.AddOrganization(organization);
                await Database.InsertAsync(organization);
            }
            return organization;
        }

        public async Task<User> SaveUserAsync(User user)
        {
            if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(User).Name))
            {
                await Database.CreateTablesAsync(CreateFlags.None, typeof(User)).ConfigureAwait(false);
            }

            if (user.Id != 0)
            {
                if ((await Database.Table<User>().ToListAsync()).Any(o => o.Id == user.Id))
                {
                    await Database.UpdateAsync(user);
                    await App.ASPDatabase.UpdateUser(user);
                }
                //если сохранен на сервере, но не в локальной бд
                else
                {
                    await Database.InsertAsync(user);
                }
            }
            else
            {
                user = await App.ASPDatabase.AddUser(user);
                await Database.InsertAsync(user);
            }
            return user;
        }

        public async Task<List<NoteImage>> SaveNoteImages(List<string> imageUrls, Note note)
        {
            List<NoteImage> images = new List<NoteImage>();
            foreach (string img in imageUrls)
            {
                //сохранение картинки на сервере
                Image file = await App.ASPDatabase.AddImage(img);
                GC.Collect();
                NoteImage image = new NoteImage() { NoteId = note.Id, ImageUrl = file.ImageUrl, ImageId = file.Id };
                image = await App.ASPDatabase.AddNoteImage(image);
                images.Add(image);
                await Database.InsertAsync(image);
            }
            return images;
        }

        public async Task<List<EventImage>> SaveEventImages(List<string> imageUrls, Event ev)
        {
            List<EventImage> images = new List<EventImage>();
            foreach (string img in imageUrls)
            {
                //сохранение картинки на сервере
                Image file = await App.ASPDatabase.AddImage(img);
                GC.Collect();
                EventImage image = new EventImage() { EventId = ev.Id, ImageUrl = file.ImageUrl, ImageId = file.Id };
                image = await App.ASPDatabase.AddEventImage(image);
                images.Add(image);
                await Database.InsertAsync(image);
            }
            return images;
        }

        public async void SaveNoteImages(List<NoteImage> images)
        {
            foreach (NoteImage image in images)
            {
                if ((await Database.Table<NoteImage>().ToListAsync()).Any(i => i.Id == image.Id))
                {
                    await Database.UpdateAsync(image);
                }
                else
                {
                    await Database.InsertAsync(image);
                }
            }
        }

        public async void SaveEventImages(List<EventImage> images)
        {
            foreach (EventImage image in images)
            {
                if ((await Database.Table<EventImage>().ToListAsync()).Any(i => i.Id == image.Id))
                {
                    await Database.UpdateAsync(image);
                }
                else
                {
                    await Database.InsertAsync(image);
                }
            }
        }

        public async Task<Note> SaveNoteAsync(Note note, Organization organization)
        {
            note.OrganizationId = organization.Id;

            if (note.Id == 0)
            {
                note = await App.ASPDatabase.AddNote(note);
                await Database.InsertAsync(note);
            }
            else
            {
                if ((await Database.Table<Note>().ToListAsync()).Any(o => o.Id == note.Id))
                {
                    await App.ASPDatabase.UpdateNote(note);
                    await Database.UpdateAsync(note);
                }
                else
                {
                    await Database.InsertAsync(note);
                }
            }
            return note;
        }

        public async void SubscribeAsync(int id)
        {
            UserOrganization dep = new UserOrganization() { OrganizationId = id, UserId = App.CurrentUser.Id };
            dep = await App.ASPDatabase.AddDependence(dep);
            await Database.InsertAsync(dep);

        }

        public async void SubscribeAsync(UserOrganization dep)
        {
            if (!(await Database.Table<UserOrganization>().ToListAsync()).Any(d => d.Id == dep.Id))
                await Database.InsertAsync(dep);
        }

        public async Task<Event> SaveEventAsync(Event @event)
        {

            if (@event.Id == 0)
            {
                @event.OrganizationId = App.CurrentOrganization.Id;
                @event.OrganizationName = App.CurrentOrganization.Name;
                @event = await App.ASPDatabase.AddEvent(@event);
                await Database.InsertAsync(@event);
            }
            else
            {
                if ((await Database.Table<Event>().ToListAsync()).Any(o => o.Id == @event.Id))
                {
                    await App.ASPDatabase.UpdateEvent(@event);
                    await Database.UpdateAsync(@event);
                }
                else
                {
                    await Database.InsertAsync(@event);
                }

            }
            return @event;
        }


        //удаление объектов
        public async void UnsubscribeAsync(int id)
        {
            await Database.DeleteAsync<UserOrganization>(id);
            await App.ASPDatabase.DeleteDependence(id);
        }

        public async void DeleteNoteImage(int id)
        {
            NoteImage img = await Database.Table<NoteImage>().Where(i => i.Id == id).FirstOrDefaultAsync();
            await Database.DeleteAsync(img);
            await App.ASPDatabase.DeleteNoteImage(id);
            await App.ASPDatabase.DeleteImage(img.ImageId);
        }

        public async void DeleteEventImage(int id)
        {
            EventImage img = await Database.Table<EventImage>().Where(i => i.Id == id).FirstOrDefaultAsync();
            await Database.DeleteAsync(img);
            await App.ASPDatabase.DeleteEventImage(id);
            await App.ASPDatabase.DeleteImage(img.ImageId);
        }

        public async void DeleteUserAsync(User user)
        {
            if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(User).Name))
            {
                await Database.CreateTablesAsync(CreateFlags.None, typeof(User)).ConfigureAwait(false);
            }

            List<UserOrganization> deps = await Database.Table<UserOrganization>().Where(x => x.UserId == user.Id).ToListAsync();
            foreach (UserOrganization item in deps)
            {
                UnsubscribeAsync(item.OrganizationId);
            }
            await Database.DeleteAsync<User>(user.Id);
            await App.ASPDatabase.DeleteUser(user.Id);

        }

        public async void DeleteNoteAsync(Note note)
        {
            List<NoteImage> images = await Database.Table<NoteImage>().Where(x => x.NoteId == note.Id).ToListAsync();
            foreach (NoteImage im in images)
            {
                DeleteNoteImage(im.Id);
            }
            await Database.DeleteAsync<Note>(note.Id);
            await App.ASPDatabase.DeleteNote(note.Id);
        }

        public async void DeleteOrganizationAsync(Organization organization)
        {
            List<UserOrganization> deps = await Database.Table<UserOrganization>().Where(d => d.OrganizationId == organization.Id).ToListAsync();
            List<Event> events = await Database.Table<Event>().Where(e => e.OrganizationId == organization.Id).ToListAsync();
            List<Note> notes = await Database.Table<Note>().Where(n => n.OrganizationId == organization.Id).ToListAsync();
            foreach (Event ev in events)
            {
                DeleteEventAsync(ev);
            }
            foreach (Note note in notes)
            {
                DeleteNoteAsync(note);
            }
            foreach (UserOrganization dep in deps)
            {
                await Database.DeleteAsync(dep.Id);
                await App.ASPDatabase.DeleteDependence(dep.Id);
            }
            await Database.DeleteAsync<Organization>(organization.Id);
            await App.ASPDatabase.DeleteOrganization(organization.Id);
        }

        public async void DeleteEventAsync(Event @event)
        {
            List<EventImage> images = await Database.Table<EventImage>().Where(x => x.EventId == @event.Id).ToListAsync();
            foreach (EventImage im in images)
            {
                DeleteEventImage(im.Id);
            }
            await Database.DeleteAsync<Event>(@event.Id);
            await App.ASPDatabase.DeleteEvent(@event.Id);
        }


        //инициализация столов
        async Task InitializeAsync()
        {
            try
            {
                if (!OrgsInitialized)
                {
                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(Organization).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(Organization)).ConfigureAwait(false);
                    }
                    OrgsInitialized = true;
                }
                if (!UsersInitialized)
                {
                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(User).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(User)).ConfigureAwait(false);
                    }
                    UsersInitialized = true;
                }
                if (!NotesInitialized)
                {
                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(Note).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(Note)).ConfigureAwait(false);
                    }
                    NotesInitialized = true;
                }
                if (!EventsInitialized)
                {
                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(Event).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(Event)).ConfigureAwait(false);
                    }
                    EventsInitialized = true;
                }
                if (!UsOrgsInitialized)
                {
                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(UserOrganization).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(UserOrganization)).ConfigureAwait(false);
                    }
                    UsOrgsInitialized = true;
                }
                if (!EventImagesInitialized)
                {
                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(EventImage).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(EventImage)).ConfigureAwait(false);
                    }
                    EventImagesInitialized = true;
                }
                if (!NoteImagesInitialized)
                {
                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(NoteImage).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(NoteImage)).ConfigureAwait(false);
                    }
                    NoteImagesInitialized = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
