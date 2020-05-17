using INAnimalHelp.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace INAnimalHelp.Service.Models
{
    public partial class INAnimalHelpDB : DbContext
    {
        public INAnimalHelpDB(DbContextOptions<INAnimalHelpDB> options)
            : base(options)
        {
        }

        public virtual DbSet<EventImage> EventImages { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<NoteImage> NoteImages { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<UserOrganization> UserOrganizations { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
