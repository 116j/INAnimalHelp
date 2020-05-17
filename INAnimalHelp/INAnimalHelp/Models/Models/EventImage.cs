using SQLite;

namespace INAnimalHelp.Models.Models
{
    [Table("EventImages")]
    public class EventImage
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int EventId { get; set; }
        public string ImageUrl { get; set; }
        public int ImageId { get; set; }
    }
}
