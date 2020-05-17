using System.Collections.Generic;

namespace INAnimalHelp.Models.Models
{
    public class EventModel
    {
        public Event Event { get; set; }
        public List<EventImage> Images { get; set; }
        public int Height { get; set; }
    }
}
