using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4.projektmunka.Models
{
    class Platform
    {
        public int PlatformID { get; set; }
        public string PlatformName { get; set; }
        public string Manufacturer { get; set; }

        public virtual ICollection<Game> Games { get; set; }
    }
}
