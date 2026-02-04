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
                    // kasnije projektil
                    break;
            }
        }

        public void Update()
        {
            GeneratePrepreke();
            MovePrepreke();
            MoveProjektili();
            DetectCollisions();
        }

        private void GeneratePrepreke()
        {
            for (int x = 0; x < 40; x++)
            {
                if (rnd.NextDouble() < 0.05)
                {
                    State.Prepreke.Add(new Prepreka
                    {
                        X = x,
                        Y = 0,
                        Oblik = 'O',
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
