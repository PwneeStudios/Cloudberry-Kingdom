using System;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class Floater_Spin : Floater_Core, IBound
    {
        static bool Flaming = true;

        public float Angle, MaxAngle, Length;
        public int Period, Offset;
        public Vector2 PivotPoint;

        public int Dir;

        QuadClass Anchor;

        QuadClass Head;

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

            if (!Core.BoxesOnly)
            {
                Anchor = new QuadClass();
                Anchor.SetToDefault();
                Anchor.TextureName = "Joint";
                Anchor.Size = new Vector2(50, 50);

                if (Flaming)
                {
                    Head = new QuadClass();
                    Head.Quad.UseGlobalIllumination = false;
                    Head.SetToDefault();
                    Head.Quad.MyTexture = Fireball.EmitterTexture;
                    Head.Size = new Vector2(Radius + 120);
                }
            }

            base.MakeNew();

            Core.DrawLayer = 4;
            Core.DrawLayer2 = 5;
            Core.DrawLayer3 = 6;
            
            //Core.DrawLayer2 = 6;
            
        }

        public Floater_Spin(bool BoxesOnly)
        {
            base.Construct(BoxesOnly);

            if (!Core.BoxesOnly)
            {
                MyObject.Quads[0].MyTexture = Tools.TextureWad.FindByName("SpikeBlob2");
                MyObject.Quads[1].MyTexture = Tools.TextureWad.FindByName("SpikeyGuy2");
                //MyObject.Quads[0].MyTexture = Tools.TextureWad.FindByName("SpikeBlob2");
                //MyObject.Quads[1].MyTexture = Tools.TextureWad.FindByName("SpikeyGuy3");
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
        public override Vector2 GetPos(float t)
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

            //int Step = Tools.Modulo(Core.MyLevel.GetPhsxStep() + Offset, Period);
            float Step = Tools.Modulo(Core.MyLevel.GetIndependentPhsxStep() + Offset, Period);
            float t = Dir * (float)Step / (float)Period;

            Vector2 Pos = GetPos(t);
            Angle = CorrespondingAngle;

            Core.Data.Position = Pos;
            Tools.PointyAxisTo(ref MyObject.Base, Core.Data.Position - PivotPoint);

            base.PhsxStep();
            Circle.Radius = Radius;
        }
        static float Radius = 200;


        bool OffScreen = false;
        public override void Draw()
        {
            if (Core.SkippedPhsx) return;

            if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
            {
                if (Core.MyLevel.MainCamera.OnScreen(PivotPoint, Length + Radius + 600))
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
                                new Color(255, 255, 255, 215),//140),
                                44,
                                ChainTexture, Tools.EffectWad.EffectList[0], 63, 0, 0f);
                }
                else if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer3)
                {
                    if (Flaming)
                    {
                        Head.Pos = Pos;
                        Head.Draw();
                    }
                    else
                    {
                        MyObject.UpdateQuads();
                        MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
                    }
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

            Angle = FloaterA.Angle;
            Dir = FloaterA.Dir;
            Period = FloaterA.Period;
            Offset = FloaterA.Offset;
            PivotPoint = FloaterA.PivotPoint;
            Length = FloaterA.Length;

            Core.WakeUpRequirements = true;
            UpdateObject();
        }
    }
}
