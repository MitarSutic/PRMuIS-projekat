using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public enum Status
    {
        Aktivna,
        Unistena,
        GAME_OVER
    }
    [Serializable]
    public class Prepreka
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Oblik { get; set; }
        public Status Aktivna { get; set; }

    }
}
