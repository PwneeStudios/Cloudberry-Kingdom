using System;
using Microsoft.Xna.Framework;

using Drawing;
using System.IO;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.FireSpinners
{
    public class FireSpinner : _LineDeath
    {
        public class FireSpinnerTileInfo : TileInfoBase
        {
            //public SpriteInfo Flame = new SpriteInfo(Tools.Texture("small flame"), new Vector2(72, 72), Vector2.Zero, new Color(255, 140, 140));
            public SpriteInfo Flame = new SpriteInfo(Tools.Texture("small flame"), new Vector2(72, 72), Vector2.Zero, Color.White);
            public SpriteInfo Base = new SpriteInfo(null, new Vector2(72, 72), Vector2.Zero, Color.White);

            public float SegmentSpacing = 53;
            public float SpaceFromBase = 0;

            public bool Rotate = true;
            public float RotateStep = .22f * .82f;
            
            // Placement info
            public float TopOffset = -80, BottomOffset = 80;
        }

        /// <summary>
        /// If true then the individual flames comprising the firespinners have an offset
        /// that varies between different firespinners (to prevent player 'vertigo')
        /// </summary>
        static bool RandomMiniOrientation = true;

        public QuadClass MyQuad, MyBaseQuad;

        public int Offset, Period;

        public float Radius;
        public float Angle;

        /// <summary>
        /// Angle of the individual flames comprising the firespinner
        /// </summary>
        private float MiniAngle;
        /// <summary>
        /// Offset angle of the individual flames comprising the firespinner
        /// </summary>
        private float MiniAngle_Offset;

        public int Orientation;
        
        public Vector2 dir;

        public override void MakeNew()
        {
            base.MakeNew();

            AutoGenSingleton = FireSpinner_AutoGen.Instance;
            Core.MyType = ObjectType.FireSpinner;
            DeathType = Bobs.Bob.BobDeathType.FireSpinner;
            Core.DrawLayer = 3;

            PhsxCutoff_Playing = new Vector2(1000, 1000);
            PhsxCutoff_BoxesOnly = new Vector2(-100, 400);

            Core.GenData.NoBlockOverlap = true;
            Core.GenData.LimitGeneralDensity = true;

            Core.WakeUpRequirements = true;
        }

        public override void Init(Vector2 pos, Level level)
        {
            base.Init(pos, level);

            if (!Core.BoxesOnly)
            {
                MyQuad.Set(Info.Spinners.Flame);
                MyQuad.Quad.UseGlobalIllumination = false;

                MyBaseQuad.Set(Info.Spinners.Base);
            }

            int CurPhsxStep = level.CurPhsxStep;
            Angle = 2 * (float)Math.PI * (CurPhsxStep + Offset) / (float)Period;
            MiniAngle = CurPhsxStep * .22f;
            if (RandomMiniOrientation)
                MiniAngle_Offset = MyLevel.Rnd.RndFloat(0, 100);

            dir = new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
            MyLine.Target.p2 = dir * Radius + Core.Data.Position;
            MyLine.Target.p1 = Core.Data.Position;

            MyLine.Current = MyLine.Target;

            MyLine.SkipEdge = true;
        }

        public FireSpinner(bool BoxesOnly)
        {
            if (!BoxesOnly)
            {
                MyQuad = new QuadClass();
                MyBaseQuad = new QuadClass();
            }

            Construct(BoxesOnly);
        }

        protected override void ActivePhsxStep()
        {
            if (Core.WakeUpRequirements)
            {
                SetTarget(Core.GetIndependentPhsxStep() - 1);
                Core.WakeUpRequirements = false;
            }

            MiniAngle = Orientation * (Core.MyLevel.IndependentPhsxStep + MiniAngle_Offset) * Info.Spinners.RotateStep;
            SetTarget(Core.GetIndependentPhsxStep());
        }

        void SetCurrent(float Step)
        {
            Vector2 p1 = Vector2.Zero, p2 = Vector2.Zero;
            GetLine(Step, ref p1, ref p2);
            MyLine.SetCurrent(p1, p2);
        }
        void SetTarget(float Step)
        {
            Vector2 p1 = Vector2.Zero, p2 = Vector2.Zero;
            GetLine(Step, ref p1, ref p2);
            MyLine.SetTarget(p1, p2);
        }
        void GetLine(float Step, ref Vector2 p1, ref Vector2 p2)
        {
            Angle = Orientation * 2 * (float)Math.PI * (Step + Offset) / (float)Period;
            dir = new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));

            p1 = Core.Data.Position;
            p2 = Core.Data.Position + dir * Radius;
        }

        protected override void DrawGraphics()
        {
            // Draw base
            if (MyBaseQuad.Quad._MyTexture != null)
            {
                MyBaseQuad.Pos = Pos;
                MyBaseQuad.Draw();
            }

            // Draw flames
            float BallLength = Info.Spinners.SegmentSpacing;
            Vector2 dif = MyLine.Current.p2 - MyLine.Current.p1;
            float CurRadius = dif.Length();
            dif /= CurRadius;
            int Balls = (int)(CurRadius / BallLength + .5f);
            BallLength = (CurRadius - 10) / Balls;

            Vector2 minidir;
            if (Info.Spinners.Rotate)
                minidir = new Vector2((float)Math.Cos(MiniAngle), (float)Math.Sin(MiniAngle));
            else
                minidir = new Vector2(1, 0);
            MyQuad.PointxAxisTo(minidir);

            Vector2 start = MyLine.Current.p1 + dif * Info.Spinners.SpaceFromBase;
            Vector2 end = MyLine.Current.p2;
            
            /*
            // Vanilla animation
            bool HoldPlaying = MyQuad.Quad.Playing;
            for (int i = 0; i < Balls; i++)
            {
                float t = (float)i / (float)(Balls - 1);
                MyQuad.Pos = (1 - t) * start + t * end;

                if (HoldPlaying && i > 0)
                {
                    MyQuad.Quad.Playing = false;
                    MyQuad.Quad.NextKeyFrame();
                }

                MyQuad.Draw();
            }
            MyQuad.Quad.Playing = HoldPlaying;
             * */

            // Tweening animation
            bool HoldPlaying = MyQuad.Quad.Playing;
            MyQuad.Quad.Playing = false;
            for (int i = 0; i < Balls; i++)
            {
                float t = (float)i / (float)(Balls - 1);
                MyQuad.Pos = (1 - t) * start + t * end;

                if (HoldPlaying && i == 0)
                {
                    MyQuad.Quad.UpdateTextureAnim();
                }

                MyQuad.Alpha = 1f;
                MyQuad.Draw();

                if (HoldPlaying && i > 0)
                {
                    MyQuad.Quad.Playing = false;
                    MyQuad.Quad.NextKeyFrame();
                }

                MyQuad.Alpha = MyQuad.Quad.t - (int)MyQuad.Quad.t;
                MyQuad.Draw();
            }
            MyQuad.Quad.Playing = HoldPlaying;
        }

        public override void Move(Vector2 shift)
        {
            base.Move(shift);
        }

        public override void Interact(Bob bob)
        {
            if (!Core.SkippedPhsx)
            if (Phsx.AABoxAndLineCollisionTest(bob.Box2, ref MyLine))
            {
                if (Core.MyLevel.PlayMode == 0)
                {
                    bob.Die(Bob.BobDeathType.FireSpinner, this);
                }

                if (Core.MyLevel.PlayMode == 1)
                {
                    bool col = Phsx.AABoxAndLineCollisionTest_Tiered(ref MyLine, Core, bob, FireSpinner_AutoGen.Instance);

                    if (col)
                        Core.Recycle.CollectObject(this);
                }
            }
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);
            Core.WakeUpRequirements = true;

            FireSpinner SpinnerA = A as FireSpinner;
            Init(SpinnerA.Pos, SpinnerA.MyLevel);

            Radius = SpinnerA.Radius;

            Offset = SpinnerA.Offset;
            Period = SpinnerA.Period;
            Orientation = SpinnerA.Orientation;

            Angle = SpinnerA.Angle;

            MyLine.SkipEdge = SpinnerA.MyLine.SkipEdge;
        }
    }
}
