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

            if (CoreData.ParentBlock is NormalBlock)
                return;
            else
            {
                CoreData.DrawLayer = CoreData.ParentBlock.CoreData.DrawLayer;
                CoreData.DrawLayer2 = CoreData.ParentBlock.CoreData.DrawLayer + 1;
                CoreData.DrawLayer3 = CoreData.ParentBlock.CoreData.DrawLayer + 2;
            }
        }

        public override void MakeNew()
        {
            base.MakeNew();

            AutoGenSingleton = SpikeyGuy_AutoGen.Instance;
            CoreData.MyType = ObjectType.SpikeyGuy;
            DeathType = Bobs.BobDeathType.SpikeyGuy;

            CoreData.ContinuousEnabled = true;
            
            Angle = 0;
            Dir = 1;
            Period = 150;
            Offset = 0;
            PivotPoint = Vector2.Zero;

            CoreData.DrawLayer = 4;
            CoreData.DrawLayer2 = 5;
            CoreData.DrawLayer3 = 6;
        }

        public override void Init(Vector2 pos, Level level)
        {
            base.Init(pos, level);

            PivotPoint = pos;

            if (!CoreData.BoxesOnly)
            {
                Head.Set(level.Info.SpikeyGuys.Ball);
                Anchor.Set(level.Info.SpikeyGuys.Base);
            }
        }

        public SpikeyGuy(bool BoxesOnly)
        {
            base.Construct(BoxesOnly);

            if (!CoreData.BoxesOnly)
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
            if (CoreData.ParentBlock != null)
                PivotPoint = CoreData.GetPosFromParentOffset();

            float PhsxCutoff = Length + 1800;
            if (CoreData.MyLevel.BoxesOnly) PhsxCutoff = Length + 100;
            
            if (!CoreData.MyLevel.MainCamera.OnScreen(PivotPoint, PhsxCutoff))
            {
                CoreData.SkippedPhsx = true;
                CoreData.WakeUpRequirements = true;
                return;
            }
            CoreData.SkippedPhsx = false;

            float Step = CoreMath.Modulo(CoreData.MyLevel.GetIndependentPhsxStep() + Offset, Period);
            float t = Dir * (float)Step / (float)Period;

            CoreData.Data.Position = GetPos(t);
            Angle = CorrespondingAngle;

            Radius = Info.SpikeyGuys.Radius;

            ActivePhsxStep();
        }


        bool OffScreen = false;
        protected override void DrawGraphics()
        {
            if (CoreData.MyLevel.CurrentDrawLayer == CoreData.DrawLayer)
            {
                if (CoreData.MyLevel.MainCamera.OnScreen(PivotPoint, Length + Info.SpikeyGuys.Radius + 600))
                    OffScreen = false;
                else
                {
                    OffScreen = true;
                    return;
                }
            }
            else
                if (OffScreen) return;

            if (CoreData.MyLevel.CurrentDrawLayer == CoreData.DrawLayer)
            {
                Anchor.Pos = PivotPoint;
                Anchor.Draw();
            }
            else if (CoreData.MyLevel.CurrentDrawLayer == CoreData.DrawLayer2)
            {
                Tools.QDrawer.DrawLine(PivotPoint, CoreData.Data.Position, Info.SpikeyGuys.Chain);
            }
            else if (CoreData.MyLevel.CurrentDrawLayer == CoreData.DrawLayer3)
            {
                if (Info.SpikeyGuys.Rotate)
                    Head.PointxAxisTo(CorrespondingAngle + Info.SpikeyGuys.RotateOffset + Info.SpikeyGuys.RotateSpeed * MyLevel.IndependentPhsxStep);
                else
                    Head.PointxAxisTo(Info.SpikeyGuys.RotateOffset);

                Head.Pos = CoreData.Data.Position;
                Head.Draw();
            }
        }

        protected override void DrawBoxes()
        {
            if (CoreData.MyLevel.CurrentDrawLayer == CoreData.DrawLayer)
            {
            }
            else if (CoreData.MyLevel.CurrentDrawLayer == CoreData.DrawLayer2)
            {
				Tools.QDrawer.DrawLine(PivotPoint, CoreData.Data.Position, new Color(190, 90, 255, 100), 25);

				//Tools.QDrawer.DrawLine(PivotPoint, Core.Data.Position, new Color(255, 255, 255, 215), 20);
            }
            else if (CoreData.MyLevel.CurrentDrawLayer == CoreData.DrawLayer3)
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
            CoreData.Clone(A.CoreData);

            SpikeyGuy FloaterA = A as SpikeyGuy;
            Init(FloaterA.CoreData.Data.Position, FloaterA.MyLevel);

            Angle = FloaterA.Angle;
            Dir = FloaterA.Dir;
            Period = FloaterA.Period;
            Offset = FloaterA.Offset;
            PivotPoint = FloaterA.PivotPoint;
            Length = FloaterA.Length;

            CoreData.WakeUpRequirements = true;
        }
    }
}
