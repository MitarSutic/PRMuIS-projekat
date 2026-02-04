using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace Client
{
    public class Renderer
    {
        public void Draw(GameState state)
        {
            Console.Clear();
            Console.WriteLine("Poeni: " + state.Igrac.BrojPoena);
            Console.WriteLine("Zivoti: " + state.Igrac.BrojZivota);

            char[,] mapa = new char[21, 40];

            foreach(var p in state.Prepreke)
            {
                mapa[p.Y, p.X] = p.Oblik;
            }

            foreach(var m in state.Projektili)
            {
                mapa[m.Y, m.X] = '^';
            }

            mapa[state.Igrac.Y, state.Igrac.X] = 'A';

            for (int y = 0; y <21; y++)
            {
                for(int x = 0; x < 40; x++)
                    Console.Write(mapa[y, x] == '\0' ? ' ' : mapa[y, x]);
                Console.WriteLine();
            }
        }
    }
}
