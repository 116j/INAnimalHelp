using SQLite;

namespace INAnimalHelp.Models.Models
{
    [Table("Notes")]
    public class Note
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string Icon { get; set; }
        public int ImageId { get; set; }
        public string Info { get; set; }
        public string Data { get; set; }
    }
}
