using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4.projektmunka.Models
{
    public class Developer
    {
        public int DeveloperID { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public virtual ICollection<Game> Games { get; set; }
    }
}
