using System.Collections.Generic;

namespace Model.Teams
{
    public class Section
    {
        public string ActivityTitle { get; set; }
        public string ActivitySubtitle { get; set; }
        public string ActivityText { get; set; }
        public string ActivityImage { get; set; }
        public string Title { get; set; }
        public IEnumerable<Fact> Facts { get; set; }
        public IEnumerable<Picture> Images { get; set; }

        public Section()
        {
            Facts = new List<Fact>();
            Images = new List<Picture>();
        }
    }
}
