using System;
using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom.Obstacles
{
    public class SpikeyGuy : _CircleDeath
    {
        public class SpikeyGuyTileInfo : TileInfoBase
        {
            public SpriteInfo Ball = new SpriteInfo("EmitterTexture", new Vector2(320), Vector2.Zero, Color.White);
            public SpriteInfo Base = new SpriteInfo("Joint", new Vector2(50, -1), Vector2.Zero, Color.White);

            public LineSpriteInfo Chain = new LineSpriteInfo("Chain_Tile", 44, 63);

            public float Radius = 200;

            public bool Rotate = false;
            public float RotateOffset = 0, RotateSpeed = 0;
        }

        public float Angle, MaxAngle, Length;
        public int Period, Offset;
        public Vector2 PivotPoint;

        public int Dir;

        QuadClass Anchor, Head;

        public override void OnAttachedToBlock()
        {
            base.OnAttachedToBlock();

            if (Core.ParentBlock is NormalBlock)
                return;
            else
            {
                Core.DrawLayer = Core.ParentBlock.Core.DrawLayer;
                Core.DrawLayer2 = Core.ParentBlock.Core.DrawLayer + 1;
                Core.DrawLayer3 = Core.ParentBlock.Core.DrawLayer + 2;
            }
        }

        public override void MakeNew()
        {
            base.MakeNew();

            AutoGenSingleton = SpikeyGuy_AutoGen.Instance;
            Core.MyType = ObjectType.SpikeyGuy;
            DeathType = Bobs.Bob.BobDeathType.SpikeyGuy;

            Core.ContinuousEnabled = true;
            
            Angle = 0;
            Dir = 1;
            Period = 150;
            Offset = 0;
            PivotPoint = Vector2.Zero;

            Core.DrawLayer = 4;
            Core.DrawLayer2 = 5;
            Core.DrawLayer3 = 6;
        }

        public override void Init(Vector2 pos, Level level)
        {
            base.Init(pos, level);

            PivotPoint = pos;

            if (!Core.BoxesOnly)
            {
                Head.Set(level.Info.SpikeyGuys.Ball);
                Anchor.Set(level.Info.SpikeyGuys.Base);
            }
        }

        public SpikeyGuy(bool BoxesOnly)
        {
            base.Construct(BoxesOnly);

            if (!Core.BoxesOnly)
            {
                Anchor = new QuadClass();
                Head = new QuadClass();
            }
        }

        public float MinY()
        {
            return PivotPoint.Y - Length;
        }

        /// <summary>
        /// Get's the specified position of the floater at time t
        /// </summary>
        /// <param name="t">The parametric time variable, t = (Step + Offset) / Period</param>
        /// <returns></returns>
        float CorrespondingAngle;
        Vector2 GetPos(float t)
        {
            CorrespondingAngle = (float)(2 * Math.PI * t);
            Vector2 Dir = CoreMath.AngleToDir(CorrespondingAngle);
            Vector2 Pos = PivotPoint + Length * Dir;

            return Pos;
        }

        public override void PhsxStep()
        {
            if (Core.ParentBlock != null)
                PivotPoint = Core.GetPosFromParentOffset();

            float PhsxCutoff = Length + 1800;
            if (Core.MyLevel.BoxesOnly) PhsxCutoff = Length + 100;
            
            if (!Core.MyLevel.MainCamera.OnScreen(PivotPoint, PhsxCutoff))
            {
                Core.SkippedPhsx = true;
                Core.WakeUpRequirements = true;
                return;
            }
            Core.SkippedPhsx = false;

            float Step = CoreMath.Modulo(Core.MyLevel.GetIndependentPhsxStep() + Offset, Period);
            float t = Dir * (float)Step / (float)Period;

            Pos = GetPos(t);
            Angle = CorrespondingAngle;

            Radius = Info.SpikeyGuys.Radius;

            ActivePhsxStep();
        }


        bool OffScreen = false;
        protected override void DrawGraphics()
        {
            if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
            {
                if (Core.MyLevel.MainCamera.OnScreen(PivotPoint, Length + Info.SpikeyGuys.Radius + 600))
                    OffScreen = false;
                else
                {
                    OffScreen = true;
                    return;
                }
            }
            else
                if (OffScreen) return;

            if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
            {
                Anchor.Pos = PivotPoint;
                Anchor.Draw();
            }
            else if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer2)
            {
                Tools.QDrawer.DrawLine(PivotPoint, Pos, Info.SpikeyGuys.Chain);
            }
            else if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer3)
            {
                if (Info.SpikeyGuys.Rotate)
                    Head.PointxAxisTo(CorrespondingAngle + Info.SpikeyGuys.RotateOffset + Info.SpikeyGuys.RotateSpeed * MyLevel.IndependentPhsxStep);
                else
                    Head.PointxAxisTo(Info.SpikeyGuys.RotateOffset);

                Head.Pos = Pos;
                Head.Draw();
            }
        }

        protected override void DrawBoxes()
        {
            if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
            {
            }
            else if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer2)
            {
				Tools.QDrawer.DrawLine(PivotPoint, Core.Data.Position, new Color(190, 90, 255, 100), 25);

				//Tools.QDrawer.DrawLine(PivotPoint, Core.Data.Position, new Color(255, 255, 255, 215), 20);
            }
            else if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer3)
            {
				Circle.Draw(new Color(190, 90, 255, 255));

				//Circle.Draw(new Color(50, 50, 255, 220));
            }
        }

        public override void Move(Vector2 shift)
        {
            PivotPoint += shift;

            base.Move(shift);
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            SpikeyGuy FloaterA = A as SpikeyGuy;
            Init(FloaterA.Pos, FloaterA.MyLevel);

            Angle = FloaterA.Angle;
            Dir = FloaterA.Dir;
            Period = FloaterA.Period;
            Offset = FloaterA.Offset;
            PivotPoint = FloaterA.PivotPoint;
            Length = FloaterA.Length;

            Core.WakeUpRequirements = true;
        }
    }
}
