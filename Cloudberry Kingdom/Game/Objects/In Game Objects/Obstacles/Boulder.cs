using System;
using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Obstacles
{
    public class Boulder : _CircleDeath, IBound
    {
        public class BoulderTileInfo : TileInfoBase
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

            CoreData.DrawLayer2 = CoreData.ParentBlock.CoreData.DrawLayer + 1;
        }

        public override void MakeNew()
        {
            base.MakeNew();

            AutoGenSingleton = Boulder_AutoGen.Instance;
            CoreData.MyType = ObjectType.Boulder;
            DeathType = BobDeathType.Boulder;

            CoreData.ContinuousEnabled = true;

            Angle = 0;
            MaxAngle = 0;
            Period = 150;
            Offset = 0;
            PivotPoint = Vector2.Zero;

            AddAngle = 0;
            PivotLocationType = PivotLocationTypes.TopBottom;

            SetLayers();
        }

        public override void Init(Vector2 pos, Level level)
        {
            base.Init(pos, level);

            Radius = 120;

            CoreData.Init();
            CoreData.ContinuousEnabled = true;

            CoreData.GenData.OverlapWidth = 60;

            Circle.Center = CoreData.Data.Position;
            CoreData.Data.Position = CoreData.StartData.Position = PivotPoint = pos;

            BoulderTileInfo info = level.Info.Boulders;

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
            CoreData.DrawLayer = 7;
            CoreData.DrawLayer2 = 8;
        }

        public Boulder(bool BoxesOnly)
        {
            base.Construct(BoxesOnly);

            if (!CoreData.BoxesOnly)
            {
                MyQuad = new QuadClass();
            }
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
        Vector2 GetPos(float t)
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
                    && (PivotPoint.X > CoreData.MyLevel.MainCamera.TR.X + Length ||
                        PivotPoint.X < CoreData.MyLevel.MainCamera.BL.X - Length)
                ||
                PivotLocationType == PivotLocationTypes.LeftRight
                    && (PivotPoint.Y > CoreData.MyLevel.MainCamera.TR.Y + Length ||
                        PivotPoint.Y < CoreData.MyLevel.MainCamera.BL.Y - Length))
            {
                float PhsxCutoff = 200;
                if (CoreData.MyLevel.BoxesOnly) PhsxCutoff = -100;
                if (!CoreData.MyLevel.MainCamera.OnScreen(CoreData.Data.Position, PhsxCutoff))
                {
                    CoreData.SkippedPhsx = true;
                    CoreData.WakeUpRequirements = true;
                    return;
                }
            }
            CoreData.SkippedPhsx = false;

            Radius = Info.Boulders.Radius;

            float Step = CoreMath.Modulo(CoreData.MyLevel.IndependentPhsxStep + Offset, (float)Period);
            float t = (float)Step / (float)Period;
             
            Vector2 Pos = GetPos(t);
            Angle = CorrespondingAngle;

            CoreData.Data.Position = Pos;

            ActivePhsxStep();
        }

        bool OffScreen = false;
        protected override void DrawGraphics()
        {
            if (CoreData.MyLevel.CurrentDrawLayer == CoreData.DrawLayer)
            {
                if (PivotLocationType == PivotLocationTypes.TopBottom)
                {
                    if (PivotPoint.X > CoreData.MyLevel.MainCamera.TR.X || PivotPoint.X < CoreData.MyLevel.MainCamera.BL.X)
                    {
                        Vector2 BL = Circle.BL - new Vector2(170, 170);
                        if (BL.X > CoreData.MyLevel.MainCamera.TR.X || BL.Y > CoreData.MyLevel.MainCamera.TR.Y)
                        {
                            OffScreen = true;
                            return;
                        }
                        Vector2 TR = Circle.TR + new Vector2(170, 170);
                        if (TR.X < CoreData.MyLevel.MainCamera.BL.X || TR.Y < CoreData.MyLevel.MainCamera.BL.Y)
                        {
                            OffScreen = true;
                            return;
                        }
                    }
                }
                else
                {
                    if (PivotPoint.Y > CoreData.MyLevel.MainCamera.TR.Y || PivotPoint.Y < CoreData.MyLevel.MainCamera.BL.Y)
                    {
                        Vector2 BL = Circle.BL - new Vector2(170, 170);
                        if (BL.Y > CoreData.MyLevel.MainCamera.TR.Y || BL.Y > CoreData.MyLevel.MainCamera.TR.Y)
                        {
                            OffScreen = true;
                            return;
                        }
                        Vector2 TR = Circle.TR + new Vector2(170, 170);
                        if (TR.Y < CoreData.MyLevel.MainCamera.BL.Y || TR.Y < CoreData.MyLevel.MainCamera.BL.Y)
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

            if (CoreData.MyLevel.CurrentDrawLayer == CoreData.DrawLayer)
            {
                Tools.QDrawer.DrawLine(CoreData.Data.Position, PivotPoint, Info.Boulders.Chain);
            }
            else if (CoreData.MyLevel.CurrentDrawLayer == CoreData.DrawLayer2)
            {
                if (MyQuad != null && MyQuad.Show)
                {
                    MyQuad.PointxAxisTo(Angle);
                    MyQuad.Pos = CoreData.Data.Position;

                    MyQuad.Draw();
                }
            }
        }

        protected override void DrawBoxes()
        {
			Tools.QDrawer.DrawLine(PivotPoint, CoreData.Data.Position, new Color(119, 136, 153, 100), 20);
			Circle.Draw(new Color(119, 136, 153));

			//Tools.QDrawer.DrawLine(PivotPoint, Core.Data.Position, new Color(255, 255, 255, 215), 20);
			//Circle.Draw(Color.LightSlateGray);
        }

        public void CalculateLength()
        {
            Length = (CoreData.StartData.Position - PivotPoint).Length();
        }

        public override void Move(Vector2 shift)
        {
            PivotPoint += shift;

            base.Move(shift);
        }

        public void MoveToBounded(Vector2 shift)
        {
            Move(shift);
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);

            CoreData.Data.Velocity = Vector2.Zero;
        }

        public override void Clone(ObjectBase A)
        {
            CoreData.Clone(A.CoreData);

            Boulder FloaterA = A as Boulder;
            Init(FloaterA.CoreData.Data.Position, FloaterA.MyLevel);

            Angle = FloaterA.Angle;
            MaxAngle = FloaterA.MaxAngle;
            Period = FloaterA.Period;
            Offset = FloaterA.Offset;
            PivotPoint = FloaterA.PivotPoint;
            Length = FloaterA.Length;

            AddAngle = FloaterA.AddAngle;
            PivotLocationType = FloaterA.PivotLocationType;

            CoreData.WakeUpRequirements = true;
        }
    }
}
