using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4.projektmunka.Models
{
    class Review
    {
        public int ReviewID { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

        public int GameID { get; set; }
        public virtual Game Game { get; set; }
    }
}
