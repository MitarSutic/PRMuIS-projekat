using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [Serializable]
    public class Igrac
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public int BrojPoena { get; set; }
        public int BrojZivota { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

    }
}
