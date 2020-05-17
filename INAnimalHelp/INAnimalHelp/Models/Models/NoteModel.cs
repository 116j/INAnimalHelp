using System.Collections.Generic;

namespace INAnimalHelp.Models.Models
{
    public class NoteModel
    {
        public Note Note { get; set; }
        public List<NoteImage> Images { get; set; }
        public string Icon { get; set; }
        public bool HasImages { get; set; }
        public int Height { get; set; }
        public int Size { get; set; } = 40;
    }
}
