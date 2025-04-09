using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4.projektmunka.Models
{
    class GamePlatform
    {
        public int GameID { get; set; }
        public virtual Game Game { get; set; }

        public int PlatformID { get; set; }
        public virtual Platform Platform { get; set; }
    }
}
