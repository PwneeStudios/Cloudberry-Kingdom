using System.Linq;
using System.IO;
using System;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class PopIn : IObject
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

        public void TextDraw() { }
        public void Release()
        {
            Core.Release();
            MyBob = null;
            OnPopIn = null;
        }

        int Step = 0;
        Bob MyBob;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public PopIn(bool BoxesOnly)
        {
        }

        public void MakeNew()
        {
            Step = 0;
            CoreData = new ObjectData();

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
        public void PhsxStep()
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
                Core.MyLevel.AddPop(Core.Data.Position + Tools.RndVector2(160));
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

        public void PhsxStep2()
        {
        }

        public void Draw()
        {
        }

        public void Move(Vector2 shift)
        {
        }

        public void Reset(bool BoxesOnly)
        {
        }

        public void Interact(Bob bob)
        {
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);
        }

        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnUsed() { }
public void OnMarkedForDeletion() { }
public void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
//StubStubStubEnd6
    }
}
