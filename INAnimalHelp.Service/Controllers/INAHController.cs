using INAnimalHelp.Models.Models;
using INAnimalHelp.Service.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace INAnimalHelp.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class INAHController : ControllerBase
    {
        readonly INAnimalHelpDB db;

        public INAHController(INAnimalHelpDB db)
        {
            this.db = db;
        }
        //методы возвращения списка объектов
        [HttpGet]
        [Route("users")]
        public List<User> GetUser()
        {
            return db.Users.ToList();
        }

        [HttpGet]
        [Route("organizations")]
        public List<Organization> GetOrganization()
        {
            return db.Organizations.ToList();
        }

        [HttpGet]
        [Route("userOrganizations")]
        public List<UserOrganization> GetDependence()
        {
            return db.UserOrganizations.ToList();
        }

        [HttpGet]
        [Route("events")]
        public List<Event> GetEvent()
        {
            return db.Events.ToList();
        }

        [HttpGet]
        [Route("notes")]
        public List<Note> GetNote()
        {
            return db.Notes.ToList();
        }

        [HttpGet]
        [Route("noteImages")]
        public List<NoteImage> GetNoteImage()
        {
            return db.NoteImages.ToList();
        }

        [HttpGet]
        [Route("eventImages")]
        public List<EventImage> GetEventImage()
        {
            return db.EventImages.ToList();
        }
        //методы возвращения одного объекта
        [HttpGet]
        [Route("users/{id}")]
        public User GetUser(int id)
        {
            return db.Users.Find(id);
        }
        [HttpGet]
        [Route("organizations/{id}")]
        public Organization GetOrganization(int id)
        {
            return db.Organizations.Find(id);
        }
        [HttpGet]
        [Route("userOrganizations/{id}")]
        public UserOrganization GetDependence(int id)
        {
            return db.UserOrganizations.Find(id);
        }
        [HttpGet]
        [Route("events/{id}")]
        public Event GetEvent(int id)
        {
            return db.Events.Find(id);
        }
        [HttpGet]
        [Route("notes/{id}")]
        public Note GetNote(int id)
        {
            return db.Notes.Find(id);
        }
        [HttpGet]
        [Route("noteImages/{id}")]
        public NoteImage GetNoteImage(int id)
        {
            return db.NoteImages.Find(id);
        }
        [HttpGet]
        [Route("eventImages/{id}")]
        public EventImage GetEventImage(int id)
        {
            return db.EventImages.Find(id);
        }
        /// <summary>
        /// Отправка файла картинки с сервера на устройство пользователя
        /// в бинарном виде
        /// </summary>
        /// <param name="id">инфификатор картинки</param>
        /// <returns></returns>
        [HttpGet]
        [Route("images/{id}")]
        public IActionResult GetImage(int id)
        {
            //находит картину по индификатору
            Image image = db.Images.Find(id);
            string path = Path.Combine("wwwroot/images", image.ImageUrl);
            //если такой картинки не существует, то выходит
            if (image == null || !System.IO.File.Exists(path))
            {
                return NotFound();
            }
            //создает массив байтов из пути
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            //создает поток в памяти из массива байтов
            MemoryStream stream = new MemoryStream(bytes);
            //возвращает файл с данной картинкой
            return File(stream, "application/octet-stream", image.ImageUrl);
        }
        //методы обновления объекта
        [HttpPut]
        [Route("users")]
        public ActionResult Put(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return Ok(user);

        }
        [HttpPut]
        [Route("organizations")]
        public IActionResult Put(Organization organization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(organization).State = EntityState.Modified;

            db.SaveChanges();

            return Ok(organization);
        }
        [HttpPut]
        [Route("userOrganizations")]
        public IActionResult Put(UserOrganization dependence)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(dependence).State = EntityState.Modified;
            db.SaveChanges();

            return Ok(dependence);
        }
        [HttpPut]
        [Route("events")]
        public IActionResult Put(Event ev)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(ev).State = EntityState.Modified;
            db.SaveChanges();


            return Ok(ev);
        }
        [HttpPut]
        [Route("notes")]
        public IActionResult Put(Note note)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(note).State = EntityState.Modified;

            db.SaveChanges();


            return Ok(note);
        }
        [HttpPut]
        [Route("noteImages")]
        public IActionResult Put(NoteImage image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(image).State = EntityState.Modified;
            db.SaveChanges();


            return Ok(image);
        }
        [HttpPut]
        [Route("eventImages")]
        public IActionResult Put(EventImage image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(image).State = EntityState.Modified;
            db.SaveChanges();

            return Ok(image);
        }
        [HttpPost]
        [Route("users")]
        public IActionResult Post(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Users.Add(user);
            db.SaveChanges();

            return Ok(user.Id);
        }
        //методы добавления объекта
        [HttpPost]
        [Route("organizations")]
        public IActionResult Post(Organization organization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Organizations.Add(organization);
            db.SaveChanges();

            return Ok(organization.Id);
        }

        [HttpPost]
        [Route("userOrganizations")]
        public IActionResult Post(UserOrganization dependence)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.UserOrganizations.Add(dependence);
            db.SaveChanges();

            return Ok(dependence.Id);
        }

        [HttpPost]
        [Route("notes")]
        public IActionResult Post(Note note)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Notes.Add(note);
            db.SaveChanges();

            return Ok(note.Id);
        }
        [HttpPost]
        [Route("events")]
        public IActionResult Post(Event ev)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Events.Add(ev);
            db.SaveChanges();

            return Ok(ev.Id);
        }
        [HttpPost]
        [Route("noteImages")]
        public IActionResult Post(NoteImage image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.NoteImages.Add(image);
            db.SaveChanges();

            return Ok(image.Id);
        }
        [HttpPost]
        [Route("eventImages")]
        public IActionResult Post(EventImage image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.EventImages.Add(image);
            db.SaveChanges();

            return Ok(image.Id);
        }
        /// <summary>
        /// Длбавление картинки на сервер
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("images")]
        public async Task<ActionResult<Image>> Post(IFormFile file)
        {
            if (file != null)
            {
                //создает директорию, если ее еще не скществует
                string path = "wwwroot/images";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //создает путь для этой директории
                path = Path.Combine(path, file.FileName);
                //удаляет файл, если он уже существует
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                //сохраняет картинку на сервере
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                //создает данные в бд
                Image image = new Image { ImageUrl = file.FileName };
                db.Images.Add(image);
                db.SaveChanges();
                return Ok(image);
            }
            return BadRequest();
        }
        //методы удаления объекта
        [HttpDelete]
        [Route("users/{id}")]
        public IActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }
        [HttpDelete]
        [Route("organizations/{id}")]
        public IActionResult DeleteOrganization(int id)
        {
            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return NotFound();
            }

            db.Organizations.Remove(organization);
            db.SaveChanges();

            return Ok(organization);
        }
        [HttpDelete]
        [Route("userOrganizations/{id}")]
        public IActionResult DeleteDependence(int id)
        {
            UserOrganization dependence = db.UserOrganizations.Find(id);
            if (dependence == null)
            {
                return NotFound();
            }

            db.UserOrganizations.Remove(dependence);
            db.SaveChanges();

            return Ok(dependence);
        }
        [HttpDelete]
        [Route("notes/{id}")]
        public IActionResult DeleteNote(int id)
        {
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return NotFound();
            }

            db.Notes.Remove(note);
            db.SaveChanges();

            return Ok(note);
        }
        [HttpDelete]
        [Route("events/{id}")]
        public IActionResult DeleteEvent(int id)
        {
            Event ev = db.Events.Find(id);
            if (ev == null)
            {
                return NotFound();
            }

            db.Events.Remove(ev);
            db.SaveChanges();

            return Ok(ev);
        }
        [HttpDelete]
        [Route("noteImages/{id}")]
        public IActionResult DeleteNoteImage(int id)
        {
            NoteImage image = db.NoteImages.Find(id);
            if (image == null)
            {
                return NotFound();
            }

            db.NoteImages.Remove(image);
            db.SaveChanges();

            return Ok(image);
        }
        [HttpDelete]
        [Route("eventImages/{id}")]
        public IActionResult DeleteEventImage(int id)
        {
            EventImage image = db.EventImages.Find(id);
            if (image == null)
            {
                return NotFound();
            }

            db.EventImages.Remove(image);
            db.SaveChanges();

            return Ok(image);
        }

        [HttpDelete]
        [Route("images/{id}")]
        public IActionResult DeleteImage(int id)
        {
            Image image = db.Images.Find(id);
            if (db.Images.Where(i => i.ImageUrl == image.ImageUrl).ToList().Count == 1)
            {
                System.IO.File.Delete(image.ImageUrl);
            }

            if (image == null)
            {
                return NotFound();
            }

            db.Images.Remove(image);
            db.SaveChanges();
            return Ok(image);
        }
    }
}