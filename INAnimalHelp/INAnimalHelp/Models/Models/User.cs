using SQLite;

namespace INAnimalHelp.Models.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string AuthType { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public int ImageId { get; set; }
        public string UserId { get; set; }
        public string Login { get; set; }
    }
}
