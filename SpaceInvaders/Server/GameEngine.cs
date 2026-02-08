using System;
using System.Collections.Generic;
using System.Linq;
using Common.Models;
using Common.Net;

namespace Server
{
    class GameEngine
    {
        public GameState State { get; private set; }
        public List<Igrac> Igraci { get; private set; }

        private Random rnd = new Random();
        private int obstacleTick = 0;

        public GameEngine(List<Igrac> igraci)
        {
            Igraci = igraci;

            State = new GameState();
            State.Igraci = igraci;
        }

        // ===== INPUT PO IGRACU =====
        public void HandleInput(InputCommand cmd, Igrac igrac)
        {
            if (cmd == null || igrac == null)
                return;

            int oldX = igrac.X;

            switch (cmd.Type)
            {
                case InputType.LEFT:
                    igrac.X--;
                    break;

                case InputType.RIGHT:
                    igrac.X++;
                    break;

                case InputType.SHOOT:
                    State.Projektili.Add(new Projektil
                    {
                        X = igrac.X,
                        Y = igrac.Y - 1,
                        Posiljaoc = igrac.Id.ToString()
                    });
                    break;
            }

            // ===== UDAR U ZID (NEMA EXCEPTIONA) =====
            igrac.X = Math.Max(0, Math.Min(39, igrac.X));
            igrac.Y = Math.Max(0, Math.Min(20, igrac.Y));

            // ===== SPRECAVANJE PREKLAPANJA =====
            foreach (var other in Igraci)
            {
                if (other != igrac && other.X == igrac.X && other.Y == igrac.Y)
                {
                    if (igrac.X > oldX)
                        igrac.X = Math.Min(39, oldX + 2); // desno 2
                    else
                        igrac.X = Math.Max(0, oldX - 2);  // levo 2
                }
            }

        }

        // ===== UPDATE IGRE =====
        public void Update()
        {
            obstacleTick++;

            // prepreke se pomeraju sporije
            if (obstacleTick % 3 == 0)
            {
                GeneratePrepreke();
                MovePrepreke();
            }

            MoveProjektili();
            DetectCollisions();
        }

        // ===== PREPREKE =====
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

                    // svaki igrac gubi zivot
                    foreach (var igrac in Igraci)
                    {
                        if (igrac.BrojZivota > 0)
                            igrac.BrojZivota--;
                    }
                }
            }
        }

        // ===== PROJEKTILI =====
        private void MoveProjektili()
        {
            foreach (var m in State.Projektili.ToList())
            {
                m.Y--;

                if (m.Y < 0)
                    State.Projektili.Remove(m);
            }
        }

        // ===== SUDARI =====
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

                    // poeni idu igracu koji je pucao
                    var shooter = Igraci
                        .FirstOrDefault(i => i.Id.ToString() == m.Posiljaoc);

                    if (shooter != null)
                        shooter.BrojPoena++;
                }
            }
        }
    }
}
