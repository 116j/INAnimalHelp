using Flurl.Http;
using INAnimalHelp.Models.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace INAnimalHelp.Models
{
    public class ASPDatabaseService
    {
        //ссылка на сервер
        const string url = "https://inanimalhelpservice.azurewebsites.net/api/INAH/";
        //путь к загруженным картинкам
        public string storage = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Storage", "Images");
        /// <summary>
        /// Добавление картинки на данное устройство
        /// </summary>
        /// <param name="id">ID картинки</param>
        /// <returns></returns>
        public async Task<string> SetPath(int id)
        {
            GC.Collect();
            return await (url + "images/" + id).DownloadFileAsync(storage);
        }
        //получение списка всех объектов
        public async Task<List<User>> GetUsers()
        {
            var result = (url + "users").GetAsync();
            return await result.ReceiveJson<List<User>>();
        }

        public async Task<List<Organization>> GetOrganizations()
        {
            var result = (url + "organizations").GetAsync();
            return await result.ReceiveJson<List<Organization>>();
        }

        public async Task<List<Event>> GetEvents()
        {
            var result = (url + "events").GetAsync();
            return await result.ReceiveJson<List<Event>>();
        }

        public async Task<List<Note>> GetNotes()
        {
            var result = (url + "notes").GetAsync();
            return await result.ReceiveJson<List<Note>>();
        }

        public async Task<List<UserOrganization>> GetDependences()
        {
            var result = (url + "userOrganizations").GetAsync();
            return await result.ReceiveJson<List<UserOrganization>>();
        }

        public async Task<List<NoteImage>> GetNoteImages()
        {
            var result = (url + "noteImages").GetAsync();
            return await result.ReceiveJson<List<NoteImage>>();
        }

        public async Task<List<EventImage>> GetEventImages()
        {
            var result = (url + "eventImages").GetAsync();
            return await result.ReceiveJson<List<EventImage>>();
        }
        //получение определенного объекта
        public async Task<User> GetUser(int id)
        {
            var result = (url + "users/" + id).GetAsync();
            return await result.ReceiveJson<User>();
        }

        public async Task<Organization> GetOrganization(int id)
        {
            var result = (url + "organizations/" + id).GetAsync();
            return await result.ReceiveJson<Organization>();
        }

        public async Task<Event> GetEvent(int id)
        {
            var result = (url + "events/" + id).GetAsync();
            return await result.ReceiveJson<Event>();
        }

        public async Task<Note> GetNote(int id)
        {
            var result = (url + "notes/" + id).GetAsync();
            return await result.ReceiveJson<Note>();
        }

        public async Task<UserOrganization> GetDependence(int id)
        {
            var result = (url + "userOrganizations/" + id).GetAsync();
            return await result.ReceiveJson<UserOrganization>();
        }

        public async Task<NoteImage> GetNoteImage(int id)
        {
            var result = (url + "noteImages/" + id).GetAsync();
            return await result.ReceiveJson<NoteImage>();
        }

        public async Task<EventImage> GetEventImage(int id)
        {
            var result = (url + "eventImages/" + id).GetAsync();
            return await result.ReceiveJson<EventImage>();
        }
        //добавление объекта в бд
        public async Task<User> AddUser(User user)
        {
            try
            {
                var response = await (url + "users").PostAsync(
               new StringContent(
                   JsonConvert.SerializeObject(user),
                   Encoding.UTF8, "application/json"));

                user.Id = int.Parse(await response.Content.ReadAsStringAsync());
                return user;
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<Organization> AddOrganization(Organization organization)
        {
            try
            {
                var response = await (url + "organizations").PostAsync(
               new StringContent(
                   JsonConvert.SerializeObject(organization),
                   Encoding.UTF8, "application/json"));

                organization.Id = int.Parse(await response.Content.ReadAsStringAsync());
                return organization;
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<UserOrganization> AddDependence(UserOrganization dependence)
        {
            try
            {
                var response = await (url + "userOrganizations").PostAsync(
               new StringContent(
                   JsonConvert.SerializeObject(dependence),
                   Encoding.UTF8, "application/json"));

                dependence.Id = int.Parse(await response.Content.ReadAsStringAsync());
                return dependence;
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<Event> AddEvent(Event ev)
        {
            try
            {
                var response = await (url + "events").PostAsync(
                new StringContent(
                    JsonConvert.SerializeObject(ev),
                    Encoding.UTF8, "application/json"));

                ev.Id = int.Parse(await response.Content.ReadAsStringAsync());
                return ev;
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<Note> AddNote(Note note)
        {
            try
            {
                var response = await (url + "notes").PostAsync(
                new StringContent(
                    JsonConvert.SerializeObject(note),
                    Encoding.UTF8, "application/json"));

                note.Id = int.Parse(await response.Content.ReadAsStringAsync());
                return note;
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<NoteImage> AddNoteImage(NoteImage image)
        {
            try
            {
                var response = await (url + "noteImages").PostAsync(
               new StringContent(
                   JsonConvert.SerializeObject(image),
                   Encoding.UTF8, "application/json"));

                image.Id = int.Parse(await response.Content.ReadAsStringAsync());
                return image;
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<EventImage> AddEventImage(EventImage image)
        {
            try
            {
                var response = await (url + "eventImages").PostAsync(
                new StringContent(
                    JsonConvert.SerializeObject(image),
                    Encoding.UTF8, "application/json"));

                image.Id = int.Parse(await response.Content.ReadAsStringAsync());
                return image;
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<Image> AddImage(string file)
        {
            var response = (url + "images").PostMultipartAsync(mp => mp.AddFile("file", file));
            var jsn = JObject.Parse(await (await response).Content.ReadAsStringAsync());
            var image = new Image() { Id = int.Parse(jsn["id"]?.ToString()), ImageUrl = jsn["imageUrl"]?.ToString() };
            await SetPath(image.Id);
            return image;
        }
        //обновление данных об объектк в бд
        public async Task<User> UpdateUser(User user)
        {
            try
            {
                var response = (url + "users").PutAsync(
               new StringContent(
                   JsonConvert.SerializeObject(user),
                   Encoding.UTF8, "application/json"));
                return await response.ReceiveJson<User>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }
        public async Task<Organization> UpdateOrganization(Organization organization)
        {
            try
            {
                var response = (url + "organizations").PutAsync(
                new StringContent(
                    JsonConvert.SerializeObject(organization),
                    Encoding.UTF8, "application/json"));
                return await response.ReceiveJson<Organization>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }
        public async Task<UserOrganization> UpdateDependence(UserOrganization dependence)
        {
            try
            {
                var response = (url + "userOrganizations").PutAsync(
               new StringContent(
                   JsonConvert.SerializeObject(dependence),
                   Encoding.UTF8, "application/json"));
                return await response.ReceiveJson<UserOrganization>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }
        public async Task<Note> UpdateNote(Note note)
        {
            try
            {
                var response = (url + "notes").PutAsync(
                new StringContent(
                    JsonConvert.SerializeObject(note),
                    Encoding.UTF8, "application/json"));
                return await response.ReceiveJson<Note>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }
        public async Task<Event> UpdateEvent(Event ev)
        {
            try
            {
                var response = (url + "events").PutAsync(
                new StringContent(
                    JsonConvert.SerializeObject(ev),
                    Encoding.UTF8, "application/json"));
                return await response.ReceiveJson<Event>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }
        public async Task<NoteImage> UpdateNoteImage(NoteImage image)
        {
            try
            {
                var response = (url + "noteImages").PutAsync(
                new StringContent(
                    JsonConvert.SerializeObject(image),
                    Encoding.UTF8, "application/json"));
                return await response.ReceiveJson<NoteImage>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }
        public async Task<EventImage> UpdateEventImage(EventImage image)
        {
            try
            {
                var response = (url + "eventImages").PutAsync(
                new StringContent(
                    JsonConvert.SerializeObject(image),
                    Encoding.UTF8, "application/json"));
                return await response.ReceiveJson<EventImage>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }
        //удаление объекта из бд
        public async Task<User> DeleteUser(int id)
        {
            try
            {
                var response = (url + "users/" + id).DeleteAsync();
                if ((await response).StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return await response.ReceiveJson<User>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<Organization> DeleteOrganization(int id)
        {
            try
            {
                var response = (url + "organizations/" + id).DeleteAsync();
                if ((await response).StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return await response.ReceiveJson<Organization>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }

        }

        public async Task<UserOrganization> DeleteDependence(int id)
        {
            try
            {
                var response = (url + "userOrganizations/" + id).DeleteAsync();
                if ((await response).StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return await response.ReceiveJson<UserOrganization>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<Note> DeleteNote(int id)
        {
            try
            {
                var response = (url + "notes/" + id).DeleteAsync();
                if ((await response).StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return await response.ReceiveJson<Note>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<Event> DeleteEvent(int id)
        {
            try
            {
                var response = (url + "events/" + id).DeleteAsync();
                if ((await response).StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return await response.ReceiveJson<Event>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<NoteImage> DeleteNoteImage(int id)
        {
            try
            {
                var response = (url + "noteImages/" + id).DeleteAsync();
                if ((await response).StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return await response.ReceiveJson<NoteImage>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<EventImage> DeleteEventImage(int id)
        {
            try
            {
                var response = (url + "eventImages/" + id).DeleteAsync();
                if ((await response).StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return await response.ReceiveJson<EventImage>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

        public async Task<Image> DeleteImage(int id)
        {
            try
            {
                var response = (url + "images/" + id).DeleteAsync();
                if ((await response).StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return await response.ReceiveJson<Image>();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }
    }
}
