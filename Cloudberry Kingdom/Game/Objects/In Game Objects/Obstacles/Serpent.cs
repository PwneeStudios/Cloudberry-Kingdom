using System;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Serpents
{
    public class Serpent : _BoxDeath
    {
        public class SerpentTileInfo : TileInfoBase
        {
            public SpriteInfo Serpent = new SpriteInfo("Serpent", new Vector2(200, -1), new Vector2(0, -.855f), Color.White, true);
            public SpriteInfo Fish = new SpriteInfo("Fish_1", new Vector2(35, -1));
            public Vector2 BoxSize = new Vector2(90, 90);
        }

        public QuadClass MyQuad, MyFish;

        public int Offset, UpT, DownT, WaitT1, WaitT2;

        public Vector2 Start, End;

        public bool Exposed;

        public override void MakeNew()
        {
            base.MakeNew();

            AutoGenSingleton = Serpent_AutoGen.Instance;
            Core.MyType = ObjectType.Serpent;
            DeathType = Bobs.Bob.BobDeathType.None;
            Core.DrawLayer = 9;

            PhsxCutoff_Playing = new Vector2(200, 4000);
            PhsxCutoff_BoxesOnly = new Vector2(-150, 4000);

            Core.GenData.NoBlockOverlap = true;
            Core.GenData.LimitGeneralDensity = false;

            Core.WakeUpRequirements = true;
        }

        public override void Init(Vector2 pos, Level level)
        {
            SerpentTileInfo info = level.Info.Serpents;

            BoxSize = info.BoxSize * level.Info.ScaleAll * level.Info.ScaleAllObjects;

            Start = new Vector2(pos.X, level.MyCamera.BL.Y - 200);
            End = new Vector2(pos.X, level.MyCamera.TR.Y - 600);

            if (!level.BoxesOnly)
            {
                MyQuad.Set(info.Serpent);
                MyFish.Set(info.Fish);
            }

            base.Init(pos, level);
        }

        public Serpent(bool BoxesOnly)
        {
            if (!BoxesOnly)
            {
                MyQuad = new QuadClass();
                MyFish = new QuadClass();
            }

            Construct(BoxesOnly);
        }

        public void SetPeriod(int Period)
        {
            WaitT1 = 50;
            UpT = 67;
            DownT = 45;

            WaitT2 = Period - UpT - DownT - WaitT1;
            if (WaitT2 < 0) WaitT2 = 0;
        }

        protected override void ActivePhsxStep()
        {
            base.ActivePhsxStep();
            AnimStep();
        }

        public void AnimStep() { AnimStep(Core.SkippedPhsx); }
        public void AnimStep(bool Skip)
        {
            if (Skip) return;

            Exposed = true;

            float t = (float)CoreMath.Modulo(Core.GetIndependentPhsxStep() + Offset, UpT + DownT + WaitT1 + WaitT2);

            float s = 0, s_fish = 0;

            // Fish
            if (!Core.BoxesOnly)
            {
                if (t < WaitT1 + UpT)
                    s_fish = CoreMath.ParabolaInterp(t / (WaitT1), new Vector2(1.2f, 1.00f), 0, 2.5f);

                MyFish.Pos = Vector2.Lerp(Start, End, s_fish);
            }

            // Serpent
            if (t < WaitT1)
                s = 0;
            else if (t < UpT + WaitT1)
                s = CoreMath.ParabolaInterp((t - WaitT1) / (float)(UpT), new Vector2(1, 1f), 0, 2.5f);
            else if (t < WaitT2 + UpT + WaitT1)
                s = 1;
            else
                s = CoreMath.ParabolaInterp(1 + (t - WaitT2 - UpT - WaitT1) / (float)DownT, new Vector2(1, 1f), 0);

            Pos = Vector2.Lerp(Start, End, s);
        }

        protected override void DrawGraphics()
        {
            if (MyFish.Pos.Y > Pos.Y - 50)
                MyFish.Draw();

            MyQuad.Pos = Pos;
            MyQuad.Draw();
        }

        public override void Move(Vector2 shift)
        {
            base.Move(shift);
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);
            Init(A.Core.StartData.Position, A.MyLevel);
            
            Serpent SerpentA = A as Serpent;

            Offset = SerpentA.Offset;
            UpT = SerpentA.UpT;
            DownT = SerpentA.DownT;
            WaitT1 = SerpentA.WaitT1;
            WaitT2 = SerpentA.WaitT2;

            Exposed = SerpentA.Exposed;

            Core.WakeUpRequirements = true;
        }
    }
}
