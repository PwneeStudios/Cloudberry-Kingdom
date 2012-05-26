using System;
using Microsoft.Xna.Framework;

using Drawing;
using System.IO;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.FireSpinners
{
    public class FireSpinner : ObjectBase
    {
        /// <summary>
        /// If true then the individual flames comprising the firespinners have an offset
        /// that varies between different firespinners (to prevent player 'vertigo')
        /// </summary>
        static bool RandomMiniOrientation = true;

        public SimpleQuad MyQuad;
        public BasePoint Base;
        public EzTexture AnchorTexture;

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

        public MovingLine MyLine;
        public Vector2 dir;

        void SetColor()
        {
            MyQuad.SetColor(new Color(255, 140, 140));
            MyQuad.MyEffect = Tools.BasicEffect;
            MyQuad.MyTexture = BallTexture;
        }

        static EzTexture BallTexture = null;
        public override void MakeNew()
        {
            Core.Init();
            Core.MyType = ObjectType.FireSpinner;
            Core.ContinuousEnabled = true;
            Core.DrawLayer = 3;

            Core.GenData.NoBlockOverlap = true;
            Core.GenData.LimitGeneralDensity = true;

            if (!Core.BoxesOnly)
            {
                MyQuad.Init();
                MyQuad.UseGlobalIllumination = false;
                MyQuad.MyEffect = Tools.BasicEffect;
                MyQuad.MyTexture = BallTexture;

                Vector2 Size = new Vector2(72);
                Base.e1 = new Vector2(Size.X, 0);
                Base.e2 = new Vector2(0, Size.Y);
            }

            SetColor();

            Core.WakeUpRequirements = true;
        }

        public FireSpinner(bool BoxesOnly)
        {
            if (BallTexture == null)
                BallTexture = Tools.TextureWad.FindByName(InfoWad.GetStr("FireSpinner_Ball_Texture"));

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void Init(int CurPhsxStep)
        {
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

        public override void PhsxStep()
        {
            Core.PosFromParentOffset();

            Vector2 PhsxCutoff = new Vector2(1000, 1000);
            if (Core.MyLevel.BoxesOnly) PhsxCutoff = new Vector2(-100, 400);
            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, PhsxCutoff))
            {
                Core.SkippedPhsx = true;
                Core.WakeUpRequirements = true;
                return;
            }
            Core.SkippedPhsx = false;

            if (Core.WakeUpRequirements)
            {
                SetTarget(Core.GetIndependentPhsxStep() - 1);
                Core.WakeUpRequirements = false;
            }

            //MiniAngle = Orientation * (Core.MyLevel.CurPhsxStep + MiniAngle_Offset) * .22f;
            //SetTarget(Core.GetPhsxStep());
            MiniAngle = Orientation * (Core.MyLevel.IndependentPhsxStep + MiniAngle_Offset) * .22f;
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

            return;
            p1 = Core.Data.Position + dir * Radius;
            p2 = Core.Data.Position + 2 * dir * Radius;

            float Angle2 = -3 * Angle * Orientation;
            //float Angle2 = -2 * Angle * Orientation;
            Vector2 dir2 = new Vector2((float)Math.Cos(Angle2), (float)Math.Sin(Angle2));
            p1 = Core.Data.Position + .75f * dir * Radius;
            p2 = Core.Data.Position + .7f * (dir + dir2) * Radius;
        }

        public override void PhsxStep2()
        {
            if (!Core.SkippedPhsx)
                MyLine.SwapToCurrent();
        }

        public override void Draw()
        {
            if (Core.SkippedPhsx) return;
            
            if ((MyLine.Current.p1.X > Core.MyLevel.MainCamera.TR.X + 150 || MyLine.Current.p1.Y > Core.MyLevel.MainCamera.TR.Y + 150 + Radius) &&
                (MyLine.Current.p2.X > Core.MyLevel.MainCamera.TR.X + 150 || MyLine.Current.p2.Y > Core.MyLevel.MainCamera.TR.Y + 150 + Radius) &&
                (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 150 || Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 150 + Radius))
                return;
            if ((MyLine.Current.p1.X < Core.MyLevel.MainCamera.BL.X - 150 || MyLine.Current.p1.Y < Core.MyLevel.MainCamera.BL.Y - 150 - Radius) &&
                (MyLine.Current.p2.X < Core.MyLevel.MainCamera.BL.X - 150 || MyLine.Current.p2.Y < Core.MyLevel.MainCamera.BL.Y - 150 - Radius) &&
                (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 150 || Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 150 - Radius))
                return;

            if (Tools.DrawGraphics && !Core.BoxesOnly)
            {
                float BallLength = 53;
                float CurRadius = (MyLine.Current.p2 - MyLine.Current.p1).Length();
                int Balls = (int)(CurRadius / BallLength + .5f);
                BallLength = (CurRadius - 10) / Balls;

                Vector2 minidir = new Vector2((float)Math.Cos(MiniAngle), (float)Math.Sin(MiniAngle));
                Tools.PointxAxisTo(ref Base.e1, ref Base.e2, minidir);

                for (int i = 0; i < Balls; i++)
                {
                    //Base.Origin = Core.Data.Position + dir * i * BallLength;
                    float t = (float)i / (float)(Balls - 1);
                    Base.Origin = (1 - t) * MyLine.Current.p1 + t * MyLine.Current.p2;

                    MyQuad.Update(ref Base);

                    Tools.QDrawer.DrawQuad(MyQuad);
                }

                // Draw extra anchor
                if (Math.Abs(Core.Data.Position.X - MyLine.Current.p1.X) > 10 ||
                    Math.Abs(Core.Data.Position.Y - MyLine.Current.p1.Y) > 10)
                {
                    Base.Origin = Core.Data.Position;
                    Base.e1 *= .6f;
                    Base.e2 *= .6f;

                    MyQuad.Update(ref Base);

                    if (AnchorTexture == null)
                        AnchorTexture = Tools.TextureWad.FindByName("Circle");

                    EzTexture Hold = MyQuad.MyTexture;
                    MyQuad.MyTexture = AnchorTexture;

                    Tools.QDrawer.DrawQuad(MyQuad);

                    Base.e1 /= .6f;
                    Base.e2 /= .6f;

                    MyQuad.MyTexture = Hold;
                }
            }

            if (Tools.DrawBoxes)
            {
                //Tools.QDrawer.DrawLine(MyLine.Current.p1, MyLine.Current.p2, Color.AntiqueWhite, 15);
                Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2, Color.Orange, 20);
            }
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            MyLine.Current.p1 += shift;
            MyLine.Current.p2 += shift;
            MyLine.Target.p1 += shift;
            MyLine.Target.p2 += shift;
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;
            
            SetColor();
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

            Radius = SpinnerA.Radius;

            Offset = SpinnerA.Offset;
            Period = SpinnerA.Period;
            Orientation = SpinnerA.Orientation;

            Angle = SpinnerA.Angle;

            MyLine.SkipEdge = SpinnerA.MyLine.SkipEdge;

            Init(Core.GetPhsxStep());
        }
    }
}
