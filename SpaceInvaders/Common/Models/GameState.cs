using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [Serializable]
    public class GameState
    {
        public Igrac Igrac { get; set; }
        public List<Prepreka> Prepreke { get; set; }
        public List<Projektil> Projektili { get; set; }
        public Status Status { get; set; }
        public List<Igrac> RangLista { get; set; }

        public GameState()
        {
            Prepreke = new List<Prepreka>();
            Projektili = new List<Projektil>();
            RangLista = new List<Igrac>();
        }
        }
    }
