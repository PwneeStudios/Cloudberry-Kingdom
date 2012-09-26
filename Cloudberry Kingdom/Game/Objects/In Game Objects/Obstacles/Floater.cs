using System;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class Floater : Floater_Core, IBound
    {
        public class SpikeyGuyTileInfo : TileInfoBase
        {
            public SpriteInfo Ball = new SpriteInfo(null, Vector2.One);

            public float Radius = 120;

            public LineSpriteInfo Chain = new LineSpriteInfo("chain_tile", 44, 63, 0, new Color(255, 255, 255, 210).ToVector4(), .2f);
        }

        public QuadClass MyQuad;

        public float Angle, MaxAngle, Length;
        public int Period, Offset;
        public Vector2 PivotPoint;

        public override void OnAttachedToBlock()
        {
            base.OnAttachedToBlock();

            Core.DrawLayer2 = Core.ParentBlock.Core.DrawLayer + 1;
        }

        public override void MakeNew()
        {
            AutoGenSingleton = Floater_AutoGen.Instance;
            Core.MyType = ObjectType.Floater;
            Core.ContinuousEnabled = true;

            Angle = 0;
            MaxAngle = 0;
            Period = 150;
            Offset = 0;
            PivotPoint = Vector2.Zero;

            AddAngle = 0;
            PivotLocationType = PivotLocationTypes.TopBottom;

            base.MakeNew();

            SetLayers();
        }

        public override void Init(Vector2 pos, Level level)
        {
            base.Init();

            Core.Data.Position = Core.StartData.Position = PivotPoint = pos;

            SpikeyGuyTileInfo info = level.Info.SpikeyGuys;

            if (!level.BoxesOnly)
            {
                if (info.Ball.Sprite == null)
                    MyQuad.Show = false;
                else
                {
                    MyQuad.Set(info.Ball);
                }
            }

            SetLayers();
        }

        void SetLayers()
        {
            Core.DrawLayer = 7;
            Core.DrawLayer2 = 8;
        }

        public Floater(bool BoxesOnly)
        {
            base.Construct(BoxesOnly);

            if (!BoxesOnly)
            {
                MyQuad = new QuadClass();
            }
        }

        public float MinY()
        {
            return PivotPoint.Y - Length;
        }

        public float AddAngle;

        public enum PivotLocationTypes { TopBottom, LeftRight };
        public PivotLocationTypes PivotLocationType;

        /// <summary>
        /// Get's the specified position of the floater at time t
        /// </summary>
        /// <param name="t">The parametric time variable, t = (Step + Offset) / Period</param>
        /// <returns></returns>
        float CorrespondingAngle;
        public override Vector2 GetPos(float t)
        {
            CorrespondingAngle = MaxAngle * (float)Math.Cos(2 * Math.PI * t);
            Vector2 Dir = new Vector2((float)Math.Cos(AddAngle + CorrespondingAngle - Math.PI / 2),
                                      (float)Math.Sin(AddAngle + CorrespondingAngle - Math.PI / 2));
            Vector2 Pos = PivotPoint + Length * Dir;

            return Pos;
        }

        public override void PhsxStep()
        {
            if (PivotLocationType == PivotLocationTypes.TopBottom
                    && (PivotPoint.X > Core.MyLevel.MainCamera.TR.X + Length ||
                        PivotPoint.X < Core.MyLevel.MainCamera.BL.X - Length)
                ||
                PivotLocationType == PivotLocationTypes.LeftRight
                    && (PivotPoint.Y > Core.MyLevel.MainCamera.TR.Y + Length ||
                        PivotPoint.Y < Core.MyLevel.MainCamera.BL.Y - Length))
            {
                float PhsxCutoff = 200;
                if (Core.MyLevel.BoxesOnly) PhsxCutoff = -100;
                if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, PhsxCutoff))
                {
                    Core.SkippedPhsx = true;
                    Core.WakeUpRequirements = true;
                    return;
                }
            }
            Core.SkippedPhsx = false;

            Radius = Info.SpikeyGuys.Radius;

            //int Step = CoreMath.Modulo(Core.MyLevel.GetPhsxStep() + Offset, Period);
            float Step = CoreMath.Modulo(Core.MyLevel.IndependentPhsxStep + Offset, (float)Period);
            float t = (float)Step / (float)Period;
             
            Vector2 Pos = GetPos(t);
            Angle = CorrespondingAngle;

            Core.Data.Position = Pos;
            Tools.PointyAxisTo(ref MyObject.Base, PivotPoint - Core.Data.Position);

            base.PhsxStep();
        }

        bool OffScreen = false;
        public override void Draw()
        {
            if (Core.SkippedPhsx) return;

            if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
            {
                if (PivotLocationType == PivotLocationTypes.TopBottom)
                {
                    if (PivotPoint.X > Core.MyLevel.MainCamera.TR.X || PivotPoint.X < Core.MyLevel.MainCamera.BL.X)
                    {
                        Vector2 BL = Circle.BL - new Vector2(170, 170);
                        if (BL.X > Core.MyLevel.MainCamera.TR.X || BL.Y > Core.MyLevel.MainCamera.TR.Y)
                        {
                            OffScreen = true;
                            return;
                        }
                        Vector2 TR = Circle.TR + new Vector2(170, 170);
                        if (TR.X < Core.MyLevel.MainCamera.BL.X || TR.Y < Core.MyLevel.MainCamera.BL.Y)
                        {
                            OffScreen = true;
                            return;
                        }
                    }
                }
                else
                {
                    if (PivotPoint.Y > Core.MyLevel.MainCamera.TR.Y || PivotPoint.Y < Core.MyLevel.MainCamera.BL.Y)
                    {
                        Vector2 BL = Circle.BL - new Vector2(170, 170);
                        if (BL.Y > Core.MyLevel.MainCamera.TR.Y || BL.Y > Core.MyLevel.MainCamera.TR.Y)
                        {
                            OffScreen = true;
                            return;
                        }
                        Vector2 TR = Circle.TR + new Vector2(170, 170);
                        if (TR.Y < Core.MyLevel.MainCamera.BL.Y || TR.Y < Core.MyLevel.MainCamera.BL.Y)
                        {
                            OffScreen = true;
                            return;
                        }
                    }
                }

                OffScreen = false;
            }
            else
                if (OffScreen) return;

            if (Tools.DrawGraphics)
            {
                if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
                {
                    Tools.QDrawer.DrawLine(Core.Data.Position, PivotPoint, Info.SpikeyGuys.Chain);
                }
                else if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer2)
                {
                    if (MyObject == null || (MyQuad != null && MyQuad.Show))
                    {
                        MyQuad.PointxAxisTo(Angle);
                        MyQuad.Pos = Pos;

                        MyQuad.Draw();
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
                Tools.QDrawer.DrawLine(PivotPoint, Core.Data.Position, new Color(255, 255, 255, 215), 20);
                //Circle.Draw(new Color(50, 50, 255, 220));

                Circle.Draw(Color.LightSlateGray);
            }
        }

        public void CalculateLength()
        {
            Length = (Core.StartData.Position - PivotPoint).Length();
        }

        public override void Move(Vector2 shift)
        {
            Core.StartData.Position += shift;
            Core.Data.Position += shift;
            PivotPoint += shift;

            //Box.Move(shift);
            Circle.Move(shift);

            MyObject.Base.Origin += shift;
            MyObject.Update();
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Floater FloaterA = A as Floater;
            Init(FloaterA.Pos, FloaterA.MyLevel);

            Angle = FloaterA.Angle;
            MaxAngle = FloaterA.MaxAngle;
            Period = FloaterA.Period;
            Offset = FloaterA.Offset;
            PivotPoint = FloaterA.PivotPoint;
            Length = FloaterA.Length;

            AddAngle = FloaterA.AddAngle;
            PivotLocationType = FloaterA.PivotLocationType;

            Core.WakeUpRequirements = true;
            UpdateObject();
        }
    }
}
