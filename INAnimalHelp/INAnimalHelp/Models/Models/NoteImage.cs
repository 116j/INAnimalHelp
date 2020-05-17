using SQLite;

namespace INAnimalHelp.Models.Models
{
    [Table("NoteImages")]
    public class NoteImage
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string ImageUrl { get; set; }
        public int ImageId { get; set; }
    }
}
