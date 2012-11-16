using System;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;






namespace CloudberryKingdom
{
    public class Serpent : _BoxDeath
    {
        public class SerpentTileInfo : TileInfoBase
        {
            public SpriteInfo Serpent = new SpriteInfo("Serpent", new Vector2(200, -1), new Vector2(0, -.875f), Color.White, true);
            public SpriteInfo Fish = new SpriteInfo("Fish_1", new Vector2(35, -1));
            public Vector2 BoxSize = new Vector2(90, 1000);
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
            DeathType = Bob.BobDeathType.None;
            Core.DrawLayer = 8;

            PhsxCutoff_Playing = new Vector2(200, 4000);
            PhsxCutoff_BoxesOnly = new Vector2(-150, 4000);

            Core.GenData.NoBlockOverlap = false;
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
                info.Serpent.Offset = new Vector2(0, -.79f);
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
            WaitT1 = 58;
            UpT = 67;
            DownT = 45;
            WaitT2 = 5;

            int Total = WaitT1 + UpT + DownT + WaitT2;
            WaitT1 = (int)(WaitT1 * Period / (float)Total);
            WaitT2 = (int)(WaitT2 * Period / (float)Total);
            DownT = (int)(DownT * Period / (float)Total);
            UpT = (int)(UpT * Period / (float)Total);

            //WaitT1 = 50;
            //UpT = 67;
            //DownT = 45;

            //WaitT2 = Period - UpT - DownT - WaitT1;
            //if (WaitT2 < 0) WaitT2 = 0;
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

            //if (!Core.BoxesOnly)
            //{
            //    MyQuad.Quad.Playing = true;
            //    if (t < WaitT1 + UpT * .51f)
            //    {
            //        MyQuad.Quad.Playing = false;
            //        MyQuad.Quad.CalcTexture(0, 0);
            //    }
            //    else
            //    {
            //        MyQuad.Quad.Playing = false;
            //        MyQuad.Quad.CalcTexture(0, 1);
            //    }
            //}


            Pos = Vector2.Lerp(Start, End, s) - new Vector2(0, BoxSize.Y);
        }

        protected override void DrawGraphics()
        {
            if (MyFish.Pos.Y > Pos.Y + BoxSize.Y - 50)
                MyFish.Draw();

            MyQuad.Pos = Pos + new Vector2(0, BoxSize.Y);
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
