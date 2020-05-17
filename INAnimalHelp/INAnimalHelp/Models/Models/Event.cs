using SQLite;

namespace INAnimalHelp.Models.Models
{
    [Table("Events")]
    public class Event
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string StartDay { get; set; }
        public int StartD { get; set; }
        public int StartM { get; set; }
        public int StartY { get; set; }
        public string EndDay { get; set; }
        public int EndD { get; set; }
        public int EndM { get; set; }
        public int EndY { get; set; }
        public string Info { get; set; }
        public string EventType { get; set; }
        public string Adress { get; set; }
        public string Lattitude { get; set; }
        public string Longitude { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public bool AdressSet { get; set; }
    }
}
