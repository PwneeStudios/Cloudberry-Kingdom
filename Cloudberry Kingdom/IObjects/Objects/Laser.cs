﻿using System;
using Microsoft.Xna.Framework;

using Drawing;
using System.IO;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public enum LaserState { Off, Warn, On };
    public class Laser : _LineDeath, IBound
    {
        public class LaserTileInfo : _TileInfo
        {
            public LaserTileInfo()
            {
                Sprite = "Laser";
            }
        }

        LaserState MyState;
        float StateChange;

        public bool AlwaysOn, AlwaysOff;
        public int Offset, Period, Duration, WarnDuration;

        public Vector2 p1, p2;

        public override void MakeNew()
        {
            AutoGenSingleton = Laser_AutoGen.Instance;

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

        public void Init(int CurPhsxStep)
        {
            MyLine.Target.p2 = p2;
            MyLine.Target.p1 = p1;

            MyLine.Current = MyLine.Target;

            MyLine.SkipEdge = true;
        }

        public void SetLine(Vector2 p, float degrees)
        {
            SetLine(p + Tools.DegreesToDir(degrees) * 3800, p - Tools.DegreesToDir(degrees) * 3800);
        }

        /// <summary>
        /// Set the laser end points and calculate it's width (for optimization purposes).
        /// </summary>
        public void SetLine(Vector2 p1, Vector2 p2)
        {
            this.p1 = p1;
            this.p2 = p2;

            Core.Data.Position = (p1 + p2) / 2;

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

        public override void PhsxStep()
        {
            float PhsxCutoff = 500;
            if (Core.MyLevel.BoxesOnly) PhsxCutoff = -100;
            if (!Core.MyLevel.MainCamera.OnScreen(BL, TR, PhsxCutoff))
            {
                Core.SkippedPhsx = true;
                Core.WakeUpRequirements = true;
                return;
            }
            Core.SkippedPhsx = false;

            if (Core.WakeUpRequirements)
            {
                MyLine.SetCurrent(p1, p2);
                Core.WakeUpRequirements = false;
            }

            if (AlwaysOn)
            {
                MyState = LaserState.On;
                float TargetState = Tools.PeriodicCentered(.86f, 1f, 70, Core.MyLevel.CurPhsxStep);
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
                //int Step = Tools.Modulo(Core.MyLevel.GetPhsxStep() + Offset, Period);
                float Step = Tools.Modulo(Core.MyLevel.GetIndependentPhsxStep() + Offset, Period);
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
        public override void Draw()
        {
            float Radius = Math.Abs(p2.X - p1.X);
            if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 150 + Radius || Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 150 + Radius)
                return;
            if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 150 - Radius || Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 150 - Radius)
                return;

            Vector4 Full, Half;
            Full = new Vector4(1, 1f, 1f, .95f);
            Half = new Vector4(1, .5f, .5f, .4f);

            if (AlwaysOff)
            {
                Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2,
                            new Color((1 - StateChange) * new Vector4(1, 1f, 1f, 0f) + StateChange * Full),
                            (1 - StateChange) * 120 + StateChange * 200 * SmallerWidth,
                            Info.Lasers.Sprite.MyTexture, Tools.EffectWad.EffectList[0], 60, 1, false);
            }
            else
            {
                if (MyState == LaserState.Warn)
                {
                    Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2,
                                new Color((1 - StateChange) * Full + StateChange * Half),
                                (1 - StateChange) * 170 + StateChange * 60,
                                Info.Lasers.Sprite.MyTexture, Tools.EffectWad.EffectList[0], 60, 1, false);
                }
                if (MyState == LaserState.On)
                {
                    Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2,
                                new Color((1 - StateChange) * new Vector4(1, 1f, 1f, 0f) + StateChange * Full),
                                (1 - StateChange) * 200 + StateChange * 200 * SmallerWidth,
                                Info.Lasers.Sprite.MyTexture, Tools.EffectWad.EffectList[0], 60, 1, false);
                }
                if (MyState == LaserState.Off && StateChange < .95f)
                {
                    Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2,
                                new Color((1 - StateChange) * Half + StateChange * new Vector4(1, 1f, 1f, 0f)),
                                (1 - StateChange) * 60 + StateChange * 40,
                                Info.Lasers.Sprite.MyTexture, Tools.EffectWad.EffectList[0], 60, 1, false);
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

            SetLine(LaserA.p1, LaserA.p2);

            AlwaysOn = LaserA.AlwaysOn;
            AlwaysOff = LaserA.AlwaysOff;
            SmallerWidth = LaserA.SmallerWidth;

            Offset = LaserA.Offset;
            Period = LaserA.Period;
            WarnDuration = LaserA.WarnDuration;
            Duration = LaserA.Duration;

            MyLine.SkipEdge = LaserA.MyLine.SkipEdge;

            Init(Core.GetPhsxStep());
        }
    }
}