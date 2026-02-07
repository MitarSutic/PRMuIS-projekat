using System;
using System.Linq;
using Common.Models;
using Common.Net;

namespace Server
{
    class GameEngine
    {

        public GameState State { get; private set; }
        private Random rnd = new Random();
        private int obstacleTick = 0;

        public GameEngine(Igrac igrac)
        {
            State = new GameState();
            State.Igrac = igrac;
        }

        public void HandleInput(InputCommand cmd)
        {
            if (cmd == null) return;

            switch (cmd.Type)
            {
                case InputType.LEFT:
                    State.Igrac.X--;
                    break;

                case InputType.RIGHT:
                    State.Igrac.X++;
                    break;

                case InputType.SHOOT:
                    State.Projektili.Add(new Projektil
                    {
                        X = State.Igrac.X,
                        Y = State.Igrac.Y - 1
                    });
                    break;

            }
        }

        public void Update()
        {
            obstacleTick++;

            // prepreke se generisu i pomeraju sporije
            if (obstacleTick % 3 == 0)   // promeni 3 → 4 ili 5 ako treba jos sporije
            {
                GeneratePrepreke();
                MovePrepreke();
            }

            // igrac i projektili se azuriraju svaki tick
            MoveProjektili();
            DetectCollisions();
        }


        private void GeneratePrepreke()
        {
            for (int x = 0; x < 40; x++)
            {
                if (rnd.NextDouble() < 0.02)
                {
                    char oblik = rnd.NextDouble() < 0.5 ? 'O' : '#';

                    State.Prepreke.Add(new Prepreka
                    {
                        X = x,
                        Y = 0,
                        Oblik = oblik,
                        Aktivna = Status.Aktivna
                    });
                }
            }
        }


        private void MovePrepreke()
        {
            foreach (var p in State.Prepreke.ToList())
            {
                p.Y++;
                if (p.Y >= 20)
                {
                    State.Prepreke.Remove(p);
                    State.Igrac.BrojZivota--;
                }
            }
        }

        private void MoveProjektili()
        {
            foreach (var m in State.Projektili.ToList())
            {
                m.Y--;

                if (m.Y < 0)
                    State.Projektili.Remove(m);
            }
        }


        private void DetectCollisions()
        {
            foreach (var m in State.Projektili.ToList())
            {
                var hit = State.Prepreke
                    .FirstOrDefault(p => p.X == m.X && p.Y == m.Y);

                if (hit != null)
                {
                    State.Prepreke.Remove(hit);
                    State.Projektili.Remove(m);
                    State.Igrac.BrojPoena++;
                }
            }
        }


    }
}
