using System;
using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom.Obstacles
{
    public class SpikeyLine : _CircleDeath, IBound
    {
        public class SpikeyLineTileInfo : TileInfoBase
        {
            public SpriteInfo Ball = new SpriteInfo(null, Vector2.One);

            public float Radius = 120;

            public bool Rotate = false;
            public float RotateOffset = 0, RotateSpeed = 0;
        }

        public int Period, Offset;
        public Vector2 p1, p2;

        QuadClass Head;

        public override void MakeNew()
        {
            base.MakeNew();

            AutoGenSingleton = Boulder_AutoGen.Instance;
            CoreData.MyType = ObjectType.SpikeyLine;
            DeathType = Bobs.BobDeathType.FallingSpike;

            CoreData.ContinuousEnabled = true;

            Period = 150;
            Offset = 0;
            p1 = new Vector2(0, 1300);
            p2 = new Vector2(0, -1300);

            CoreData.DrawLayer = 2;
            CoreData.DrawLayer2 = 6;
        }

        public override void Init(Vector2 pos, Level level)
        {
            base.Init(pos, level);

            if (!CoreData.BoxesOnly)
            {
                Head.Set(level.Info.SpikeyLines.Ball);
            }
        }

        public SpikeyLine(bool BoxesOnly)
        {
            base.Construct(BoxesOnly);

            if (!CoreData.BoxesOnly)
            {
                Head = new QuadClass();
            }
        }

        /// <summary>
        /// Get's the specified position of the floater at time t
        /// </summary>
        /// <param name="t">The parametric time variable, t = (Step + Offset) / Period</param>
        /// <returns></returns>
        public Vector2 GetPos(float t)
        {
            Vector2 pos = Vector2.Lerp(p1, p2, t);
            return pos;
        }

        public override void PhsxStep()
        {
            float PhsxCutoff = 360;
            if (!CoreData.MyLevel.MainCamera.OnScreen(p1, PhsxCutoff) && !CoreData.MyLevel.MainCamera.OnScreen(p2, PhsxCutoff))
            {
                CoreData.SkippedPhsx = true;
                CoreData.WakeUpRequirements = true;
                return;
            }
            CoreData.SkippedPhsx = false;

            float Step = CoreMath.Modulo(CoreData.MyLevel.GetIndependentPhsxStep() + Offset, (float)Period);
            float t = (float)Step / (float)Period;

            Vector2 v = GetPos(t);

            CoreData.Data.Position = v;
            //CoreMath.PointxAxisToAngle(ref MyObject.Base, Core.MyLevel.CurPhsxStep / 20f);

            Radius = Info.SpikeyLines.Radius;

            ActivePhsxStep();
        }


        bool OffScreen = false;
        protected override void DrawGraphics()
        {
            if (CoreData.SkippedPhsx) return;

            if (CoreData.MyLevel.CurrentDrawLayer == CoreData.DrawLayer)
            {
                if (CoreData.MyLevel.MainCamera.OnScreen(CoreData.Data.Position, 200))
                    OffScreen = false;
                else
                {
                    OffScreen = true;
                    return;
                }
            }
            else
                if (OffScreen) return;

            if (Info.SpikeyLines.Rotate)
                Head.PointxAxisTo(Info.SpikeyLines.RotateOffset + Info.SpikeyLines.RotateSpeed * MyLevel.IndependentPhsxStep);
            else
                Head.PointxAxisTo(Info.SpikeyLines.RotateOffset);

            Head.Pos = CoreData.Data.Position;
            Head.Draw();
        }

        public override void Move(Vector2 shift)
        {
            p1 += shift;
            p2 += shift;

            base.Move(shift);            
        }

        public Vector2 TR_Bound()
        {
            Vector2 TR = GetPos(0);
            float step = .2f;
            float t = step;
            while (t <= 1)
            {
                TR = Vector2Extension.Max(TR, GetPos(t));
                t += step;
            }

            return TR;
        }

        public Vector2 BL_Bound()
        {
            Vector2 BL = GetPos(0);
            float step = .2f;
            float t = step;
            while (t <= 1)
            {
                BL = Vector2Extension.Min(BL, GetPos(t));
                t += step;
            }

            return BL;
        }

        public void MoveToBounded(Vector2 shift)
        {
            Move(shift);
        }

        public override void Clone(ObjectBase A)
        {
            CoreData.Clone(A.CoreData);

            SpikeyLine FloaterA = A as SpikeyLine;
            Init(FloaterA.CoreData.Data.Position, FloaterA.MyLevel);

            Period = FloaterA.Period;
            Offset = FloaterA.Offset;
            p1 = FloaterA.p1;
            p2 = FloaterA.p2;

            CoreData.WakeUpRequirements = true;
        }
    }
}
        