using System;
using Microsoft.Xna.Framework;

using CoreEngine;
using System.IO;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Obstacles
{
    public enum LaserState { Off, Warn, On };
    public class Laser : _LineDeath, IBound
    {
		//public static Vector4 Laser_DefaultTint_Full = new Vector4(1.0f, 1.0f, 1.0f, .95f);
		public static Vector4 Laser_DefaultTint_Full = new Vector4(.95f, .95f, .95f, .90f);

        public class LaserTileInfo : TileInfoBase
        {
            public LineSpriteInfo Line = new LineSpriteInfo("Laser", 100, 60, 1, Vector4.One);
            
            public Vector4 Tint_Full = new Vector4(1, 1f, 1f, .95f);
            public Vector4 Tint_Half = new Vector4(1, .5f, .5f, .4f);
            public float Scale = 1f;
        }

        LaserState MyState;
        float StateChange;

        public bool AlwaysOn, AlwaysOff;
        public int Offset, Period, Duration, WarnDuration;

        public override void MakeNew()
        {
            base.MakeNew();

            AutoGenSingleton = Laser_AutoGen.Instance;
            Core.MyType = ObjectType.Laser;
            DeathType = Bobs.Bob.BobDeathType.Laser;

            PhsxCutoff_Playing = new Vector2(500);
            PhsxCutoff_BoxesOnly = new Vector2(-100);

            SmallerWidth = 170f / 200f;

            AlwaysOn = false;
            AlwaysOff = false;
            StateChange = 0;

            Core.Init();
            Core.MyType = ObjectType.Laser;
            Core.DrawLayer = 9;
            Core.ContinuousEnabled = true;

            Core.WakeUpRequirements = true;
        }

        public Laser(bool BoxesOnly)
        {
            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void SetLine(Vector2 p, float degrees)
        {
            SetLine(p + CoreMath.DegreesToDir(degrees) * 3800, p - CoreMath.DegreesToDir(degrees) * 3800);
        }

        /// <summary>
        /// Set the laser end points and calculate it's width (for optimization purposes).
        /// </summary>
        public void SetLine(Vector2 p1, Vector2 p2)
        {
            this.p1 = p1;
            this.p2 = p2;

            Core.Data.Position = (p1 + p2) / 2;

            MyLine.Target.p2 = p2;
            MyLine.Target.p1 = p1;

            MyLine.Current = MyLine.Target;

            MyLine.SkipEdge = true;

            BL = BL_Bound();
            TR = TR_Bound();
        }

        Vector2 BL, TR;
        public Vector2 TR_Bound()
        {
            return Vector2.Max(p1, p2);
        }
        public Vector2 BL_Bound()
        {
            return Vector2.Min(p1, p2);
        }

        protected override void ActivePhsxStep()
        {
            if (Core.WakeUpRequirements)
            {
                MyLine.SetCurrent(p1, p2);
                Core.WakeUpRequirements = false;
            }

            if (AlwaysOn)
            {
                MyState = LaserState.On;
                float TargetState = CoreMath.PeriodicCentered(.86f, 1f, 70, Core.MyLevel.CurPhsxStep);
                StateChange += .02f * Math.Sign(TargetState - StateChange);
                if (StateChange > 1) StateChange = 1;
            }
            else if (AlwaysOff)
            {
                MyState = LaserState.Off;
                float TargetState = 0;
                StateChange += .02f * Math.Sign(TargetState - StateChange);
                if (StateChange < 0) StateChange = 0;
            }
            else
            {
                //int Step = CoreMath.Modulo(Core.MyLevel.GetPhsxStep() + Offset, Period);
                float Step = CoreMath.Modulo(Core.MyLevel.GetIndependentPhsxStep() + Offset, Period);
                if (Step < WarnDuration)
                {
                    MyState = LaserState.Warn;
                    StateChange = Math.Min(1, (WarnDuration - Step) / 7f);
                }
                else if (Step < WarnDuration + Duration)
                {
                    MyState = LaserState.On;
                    StateChange = Math.Min(1, (WarnDuration + Duration - Step) / 4f);
                }
                else
                {
                    MyState = LaserState.Off;
                    StateChange = Math.Min(1, (Period - Step) / 3f);
                }
            }

            MyLine.SetTarget(p1, p2);
        }

        public float SmallerWidth;
        protected override void DrawGraphics()
        {
            var info = Info.Lasers;
            float scale = info.Scale;
            Vector4 Full = info.Tint_Full;
            Vector4 Half = info.Tint_Half;

            if (AlwaysOff)
            {
                Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2, info.Line, 
                            (1 - StateChange) * new Vector4(1, 1f, 1f, 0f) + StateChange * Full,
                            (1 - StateChange) * 120 + StateChange * 200 * SmallerWidth);
            }
            else
            {
                if (MyState == LaserState.Warn)
                {
                    Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2, info.Line,
                                (1 - StateChange) * Full + StateChange * Half,
                                ((1 - StateChange) * 170 + StateChange * 60) * scale);
                }
                if (MyState == LaserState.On)
                {
                    Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2, info.Line,
                                (1 - StateChange) * new Vector4(1, 1f, 1f, 0f) + StateChange * Full,
                                ((1 - StateChange) * 200 + StateChange * 200 * SmallerWidth) * scale, Tools.t * 3.5f);
                }
                if (MyState == LaserState.Off && StateChange < .95f)
                {
                    Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2, info.Line,
                                (1 - StateChange) * Half + StateChange * new Vector4(1, 1f, 1f, 0f),
                                ((1 - StateChange) * 60 + StateChange * 40) * scale);
                }
            }
        }

        protected override void DrawBoxes()
        {
            Vector4 Full, Half;
            Full = new Vector4(1, 1f, 1f, .95f);
            Half = new Vector4(1, .5f, .5f, .4f);

            if (AlwaysOff)
            {
                Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2,
                            new Color((1 - StateChange) * new Vector4(1, 1f, 1f, 0f) + StateChange * Full), 60);
            }
            else
            {
                if (MyState == LaserState.Warn)
                {
                    Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2, 
                                new Color((1 - StateChange) * Full + StateChange * Half), 60);
                }
                if (MyState == LaserState.On)
                {
                    Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2,
                                new Color((1 - StateChange) * new Vector4(1, 1f, 1f, 0f) + StateChange * Full), 60);
                }
                if (MyState == LaserState.Off && StateChange < .95f)
                {
                    Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2,
                                new Color((1 - StateChange) * Half + StateChange * new Vector4(1, 1f, 1f, 0f)), 60);
                }
            }
        }

        public void MoveToBounded(Vector2 shift)
        {
            Move(shift);
        }

        public override void Move(Vector2 shift)
        {
            base.Move(shift);

            MyLine.Current.p1 += shift;
            MyLine.Current.p2 += shift;
            MyLine.Target.p1 += shift;
            MyLine.Target.p2 += shift;

            p1 += shift;
            p2 += shift;
        }

        public override void Interact(Bob bob)
        {
            if (MyState == LaserState.On && !Core.SkippedPhsx)
                base.Interact(bob);
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);
            Core.WakeUpRequirements = true;

            Laser LaserA = A as Laser;
            Init(A.Pos, A.MyLevel);

            SetLine(LaserA.p1, LaserA.p2);

            AlwaysOn = LaserA.AlwaysOn;
            AlwaysOff = LaserA.AlwaysOff;
            SmallerWidth = LaserA.SmallerWidth;

            Offset = LaserA.Offset;
            Period = LaserA.Period;
            WarnDuration = LaserA.WarnDuration;
            Duration = LaserA.Duration;

            MyLine.SkipEdge = LaserA.MyLine.SkipEdge;
        }
    }
}
