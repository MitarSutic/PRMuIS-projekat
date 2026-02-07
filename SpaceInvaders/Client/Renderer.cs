using System;
using Common.Models;

namespace Client
{
    public class Renderer
    {
        public void Draw(GameState state)
        {
            // Status bar
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Poeni: {state.Igrac.BrojPoena}".PadRight(Console.WindowWidth));
            Console.WriteLine($"Zivoti: {state.Igrac.BrojZivota}".PadRight(Console.WindowWidth));
            Console.WriteLine("".PadRight(Console.WindowWidth));

            // Logicka mapa 21x40, vizuelno 21x80 (2 karaktera po polju)
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

            // Igrac
            mapa[state.Igrac.Y, state.Igrac.X] = " A";

            // Crtanje mape
            for (int y = 0; y < 21; y++)
            {
                for (int x = 0; x < 40; x++)
                {
                    Console.Write(mapa[y, x] ?? "  ");
                }
                Console.WriteLine();
            }
        }
    }
}
