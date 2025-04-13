using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4.projektmunka.Models
{
    public class Game
    {
        public int GameID { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }

        public int DeveloperID { get; set; }
        public virtual Developer Developer { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Platform> Platforms { get; set; }
    }
}
