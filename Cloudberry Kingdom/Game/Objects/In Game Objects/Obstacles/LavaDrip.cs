using System;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Obstacles
{
    public class LavaDrip : _BoxDeath
    {
        public class LavaDripTileInfo : TileInfoBase
        {
            public LineSpriteInfo Line = new LineSpriteInfo("Lava_Drip_1", "Lava_Drip_2", "Lava_Drip_3", 440);
            public Vector2 BoxSize = new Vector2(118, 1300);
        }

        public int Offset, DownT, WaitT, PeakT;

        public Vector2 Start, End;

        public bool Exposed;

        public override void MakeNew()
        {
            base.MakeNew();

            AutoGenSingleton = LavaDrip_AutoGen.Instance;
            CoreData.MyType = ObjectType.LavaDrip;
            DeathType = Bobs.BobDeathType.LavaFlow;
            CoreData.DrawLayer = 8;

            PhsxCutoff_Playing = new Vector2(200, 4000);
            PhsxCutoff_BoxesOnly = new Vector2(-150, 4000);

            CoreData.GenData.NoBlockOverlap = false;
            CoreData.GenData.LimitGeneralDensity = false;

            CoreData.WakeUpRequirements = true;
        }

        public override void Init(Vector2 pos, Level level)
        {
            LavaDripTileInfo info = level.Info.LavaDrips;

            BoxSize.X = info.BoxSize.X * level.Info.ScaleAll * level.Info.ScaleAllObjects;

            Start = new Vector2(pos.X, level.MyCamera.TR.Y + BoxSize.Y);
            End = new Vector2(pos.X, level.MyCamera.BL.Y - BoxSize.Y);

            base.Init(pos, level);
        }

        public LavaDrip(bool BoxesOnly)
        {
            Construct(BoxesOnly);
        }

        public int Period;
        public void SetPeriod(float speed)
        {
            DownT = (int)(230 * (BoxSize.Y + 1500) / (1300f + 1500));
            PeakT = 65;
            WaitT = 340 - DownT - PeakT;

            DownT = (int)(DownT / speed);
            PeakT = (int)(PeakT / speed);
            WaitT = (int)(WaitT / speed);

            Period = DownT + PeakT + WaitT;
        }

        protected override void ActivePhsxStep()
        {
            base.ActivePhsxStep();
            AnimStep();
        }

        public void AnimStep() { AnimStep(CoreData.SkippedPhsx); }
        public void AnimStep(bool Skip)
        {
            if (Skip) return;

            var KeyFrames_Peak = Tools.FloatArrayToVectorArray_y(new float[] { 0, .5f, .8f, .9f, .95f, .975f, 1f });
            var KeyFrames_Down = Tools.FloatArrayToVectorArray_y(new float[] { .065f, 1f });

            Exposed = true;

            float t = (float)CoreMath.Modulo(CoreData.GetIndependentPhsxStep() + Offset, DownT + WaitT + PeakT);
            
            float s = 0;
            if (t < PeakT) s = CoreMath.FancyLerp(t / PeakT, KeyFrames_Peak).Y * KeyFrames_Down[0].Y;
            else if (t < PeakT + DownT) s = CoreMath.FancyLerp((t - PeakT) / (float)DownT, KeyFrames_Down).Y;
            else s = 1f;

            CoreData.Data.Position = Vector2.Lerp(Start, End, s);
        }

        protected override void DrawGraphics()
        {
            Tools.QDrawer.DrawLine(new Vector2(CoreData.Data.Position.X, Box.Current.BL.Y), new Vector2(CoreData.Data.Position.X, Box.Current.TR.Y), Info.LavaDrips.Line);
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
            CoreData.Clone(A.CoreData);
            LavaDrip LavaDripA = A as LavaDrip;

            BoxSize = LavaDripA.BoxSize;
            Init(A.CoreData.StartData.Position, A.MyLevel);

            Offset = LavaDripA.Offset;
            DownT = LavaDripA.DownT;
            PeakT = LavaDripA.PeakT;
            WaitT = LavaDripA.WaitT;
            Period = LavaDripA.Period;

            Exposed = LavaDripA.Exposed;

            CoreData.WakeUpRequirements = true;
        }
    }
}
