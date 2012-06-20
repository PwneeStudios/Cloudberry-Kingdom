using System;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class Floater_Spin : _CircleDeath
    {
        public class SpinTileInfo
        {
            public TextureOrAnim Sprite = Fireball.EmitterTexture;
            public TextureOrAnim BaseSprite = "Joint";
            public TextureOrAnim ChainSprite = "Chain_Tile";
            public float ChainWidth = 44, ChainRepeatWidth = 63;
            public Vector2 Size = new Vector2(320), Shift = Vector2.Zero;
            public Vector2 BaseSize = new Vector2(50, 50);
            public float Radius = 200;
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
            AutoGenSingleton = Floater_Spin_AutoGen.Instance;
            Core.MyType = ObjectType.Floater_Spin;
            Core.ContinuousEnabled = true;

            DeathType = Bobs.Bob.BobDeathType.Pinky;

            Angle = 0;
            Dir = 1;
            Period = 150;
            Offset = 0;
            PivotPoint = Vector2.Zero;

            base.MakeNew();

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
                Anchor.Set(level.Info.Orbs.BaseSprite);
                Anchor.SetToDefault();
                Anchor.Set(level.Info.Orbs.BaseSprite);
                Anchor.Size = level.Info.Orbs.BaseSize;

                Head.Set(level.Info.Orbs.Sprite);
                Head.Quad.UseGlobalIllumination = false;
                Head.SetToDefault();
                Head.Set(level.Info.Orbs.Sprite);
                Head.Size = level.Info.Orbs.Size;
            }
        }

        public Floater_Spin(bool BoxesOnly)
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
            Vector2 Dir = Tools.AngleToDir(CorrespondingAngle);
            Vector2 Pos = PivotPoint + Length * Dir;

            return Pos;
        }

        public override void PhsxStep()
        {
            if (Core.ParentBlock != null)
                PivotPoint = Core.GetPosFromParentOffset();

            float PhsxCutoff = Length + 750;
            if (Core.MyLevel.BoxesOnly) PhsxCutoff = Length + 100;
            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, PhsxCutoff))
            {
                Core.SkippedPhsx = true;
                Core.WakeUpRequirements = true;
                return;
            }
            Core.SkippedPhsx = false;

            float Step = Tools.Modulo(Core.MyLevel.GetIndependentPhsxStep() + Offset, Period);
            float t = Dir * (float)Step / (float)Period;

            Vector2 Pos = GetPos(t);
            Angle = CorrespondingAngle;

            Core.Data.Position = Pos;

            base.PhsxStep();
            Circle.Radius = Info.Orbs.Radius;
        }


        bool OffScreen = false;
        public override void Draw()
        {
            if (Core.SkippedPhsx) return;

            if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
            {
                if (Core.MyLevel.MainCamera.OnScreen(PivotPoint, Length + Info.Orbs.Radius + 600))
                    OffScreen = false;
                else
                {
                    OffScreen = true;
                    return;
                }
            }
            else
                if (OffScreen) return;

            if (Tools.DrawGraphics)
            {
                if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
                {
                    Anchor.Pos = PivotPoint;
                    Anchor.Draw();
                }
                else if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer2)
                {
                    Tools.QDrawer.DrawLine(PivotPoint, Core.Data.Position,
                                new Color(255, 255, 255, 215),
                                Info.Orbs.ChainWidth,
                                Info.Orbs.ChainSprite.MyTexture, Tools.EffectWad.EffectList[0], Info.Orbs.ChainRepeatWidth, 0, 0f);
                }
                else if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer3)
                {
                    Head.Pos = Pos;
                    Head.Draw();
                }
            }

            if (Tools.DrawBoxes)
            {
                if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
                {
                }
                else if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer2)
                {
                    Tools.QDrawer.DrawLine(PivotPoint, Core.Data.Position,
                                new Color(255, 255, 255, 215),
                                20);
                }
                else if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer3)
                {
                    Circle.Draw(new Color(50, 50, 255, 220));
                }
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

            Floater_Spin FloaterA = A as Floater_Spin;
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
