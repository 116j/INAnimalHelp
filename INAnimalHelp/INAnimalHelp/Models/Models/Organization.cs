using SQLite;

namespace INAnimalHelp.Models.Models
{
    [Table("Organizations")]
    public class Organization
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string AuthType { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public int ImageId { get; set; }
        public string OrganizationId { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string AnimalType { get; set; }
        public string OrganizationType { get; set; }
        public string Adress { get; set; }
        public string Lattitude { get; set; }
        public string Longitude { get; set; }
        public string Number { get; set; }
        public string Site { get; set; }
        public string Vkontakte { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
    }
}
