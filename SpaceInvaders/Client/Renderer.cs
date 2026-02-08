using System;
using Common.Models;

namespace Client
{
    public class Renderer
    {
        public void Draw(GameState state)
        {
            if (state == null || state.Igraci == null || state.Igraci.Count == 0)
                return;

            Console.SetCursorPosition(0, 0);

            // === STATUS BAR ZA SVE IGRACE ===
            foreach (var igrac in state.Igraci)
            {
                Console.WriteLine(
                    $"{igrac.Ime}: Poeni {igrac.BrojPoena} | Zivoti {igrac.BrojZivota}"
                        .PadRight(Console.WindowWidth)
                );
            }

            Console.WriteLine("".PadRight(Console.WindowWidth));

            // === MAPA 21x40 (2 karaktera po polju) ===
            string[,] mapa = new string[21, 40];

            // Prepreke
            foreach (var p in state.Prepreke)
            {
                if (p.Oblik == 'O')
                    mapa[p.Y, p.X] = " O";
                else if (p.Oblik == '#')
                    mapa[p.Y, p.X] = "[]";
            }

            // Projektili
            foreach (var m in state.Projektili)
            {
                mapa[m.Y, m.X] = " ^";
            }

            // Igraci
            int playerIndex = 1;

            foreach (var igrac in state.Igraci)
            {
                // prvi red – igrac
                mapa[igrac.Y, igrac.X] = " A";

                // drugi red – broj igraca
                if (igrac.Y + 1 < 21)
                    mapa[igrac.Y + 1, igrac.X] = " " + playerIndex;

                playerIndex++;
            }


            // Gornji border
            Console.WriteLine("+" + new string('-', 40 * 2) + "+");

            // Crtanje mape
            for (int y = 0; y < 21; y++)
            {
                Console.Write("|");
                for (int x = 0; x < 40; x++)
                {
                    Console.Write(mapa[y, x] ?? "  ");
                }
                Console.WriteLine("|");
            }

            // Donji border
            Console.WriteLine("+" + new string('-', 40 * 2) + "+");
        }
    }
}
