using SQLite;

namespace INAnimalHelp.Models.Models
{
    [Table("UserOrganizations")]
    public class UserOrganization
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
    }
}
