using System.Linq;
using System.IO;
using System;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class PopIn : ObjectBase
    {
        /// <summary>
        /// Pop in all the Bobs of a level, using an individual PopIn object per Bob.
        /// </summary>
        public static void PopInAll(Level level, Vector2 pos, Action<Bob> OnPopIn)
        {
            foreach (Bob bob in level.Bobs)
            {
                Tools.MoveTo(bob, pos);
                //bob.Pos = pos;
                pos.X -= 100; pos.Y += 20;
                PopIn pop = new PopIn(true); pop.Init(bob);
                pop.OnPopIn = OnPopIn;
                level.AddObject(pop);
            }
        }    

        public Action<Bob> OnPopIn;

        public override void Release()
        {
            base.Release();

            MyBob = null;
            OnPopIn = null;
        }

        int Step = 0;
        Bob MyBob;

        public PopIn(bool BoxesOnly)
        {
        }

        public override void MakeNew()
        {
            Step = 0;

            Core.ResetOnlyOnReset = true;
        }

        public void Init(Bob bob)
        {
            MakeNew();

            MyBob = bob;

            Core.Data.Position = bob.Core.Data.Position;
            bob.Immortal = true;
            bob.Core.Show = false;
        }

        int InitialDelay = 60;
        bool InitialDelayPast = false;
        const int PopPause = 5;
        static int[] Pops = { 0, 5, 13, 30, PopPause, PopPause, PopPause, PopPause };
        public override void PhsxStep()
        {
            if (!InitialDelayPast)
            {
                Step++;
                if (Step < InitialDelay)
                    return;
                else
                {
                    Step = 0;
                    InitialDelayPast = true;
                }
            }

            if (Tools.IncrementsContainsSum(Pops, Step))
            {
                Core.MyLevel.AddPop(Core.Data.Position + MyLevel.Rnd.RndVector2(160));
                Tools.SoundWad.FindByName("Pop 2").Play();
            }

            if (Step == Pops.Sum() + 13)
            {
                MyBob.Immortal = false;
                MyBob.Core.Show = true;
                MyBob.Core.Data.Velocity = new Vector2(23, 30);

                if (OnPopIn != null) OnPopIn(MyBob); OnPopIn = null;
                MyBob = null;                

                //fart sound
                  //  use this for quick join

                Tools.SoundWad.FindByName("Fart").Play(.5f);
                //for (int i = 0; i < 4; i++)
                //    ParticleEffects.PieceExplosion(Core.MyLevel, Core.Data.Position, 0, 1);
            }
            Step++;
        }
    }
}
