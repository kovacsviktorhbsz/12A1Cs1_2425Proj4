using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4.projektmunka.Models
{
    public class GamePlatform
    {
        [Key]
        [Column(Order = 1)]
        public int GameID { get; set; }
        public virtual Game Game { get; set; }

        [Key]
        [Column(Order = 2)]
        public int PlatformID { get; set; }
        public virtual Platform Platform { get; set; }
    }
}
