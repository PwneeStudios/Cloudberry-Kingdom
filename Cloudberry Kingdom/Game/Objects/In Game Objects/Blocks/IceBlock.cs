using System;
using System.IO;
using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class IceBlock : BlockBase
    {
        QuadClass Ice;
        SimpleQuad MyQuad;
        BasePoint Base, HoldBase;

        public int Period, Offset;
        public PhsxData EmitData;

        public float MyAnimSpeed;

        public override void MakeNew()
        {
            MyAnimSpeed = .36f;

            Core.Init();
            BlockCore.MyType = ObjectType.IceBlock;

            BlockCore.Layer = .35f;
            Core.DrawLayer = 4;

            if (!Core.BoxesOnly)
            {
                MyQuad.Init();
                MyQuad.MyEffect = Tools.BasicEffect;
                MyQuad.MyTexture = Fireball.EmitterTexture;

                MyQuad.SetColor(new Vector4(.8f, .8f, 1f, .8f));

                //float s = Box.Current.Size.X * 142f / 115f;
                Base.e1 = new Vector2(142, 0);
                Base.e2 = new Vector2(0, 142);

                HoldBase = Base;

                //Ice.Size = new Vector2(115);
            }
        }

        public override void Release()
        {
            base.Release();

            Ice = null;
        }

        public IceBlock(bool BoxesOnly)
        {
            if (!BoxesOnly)
            {
                Ice = new QuadClass();
                Ice.TextureName = "IceBlock";
            }

            MyBox = new AABox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void Init(Vector2 center, Vector2 size)
        {
            Active = true;

            //size = new Vector2(115);
            MyBox.Initialize(center, size);

            Ice.ScaleYToMatchRatio(size.X);
            float s = size.X * 142f / 115f;
            Base.e1 = new Vector2(s, 0);
            Base.e2 = new Vector2(0, s);

            HoldBase = Base;


            Core.StartData.Position = Core.Data.Position = center;

            Update();
        }

        public override void Hit(Bob bob) { }
        public override void LandedOn(Bob bob)
        {
        }
        public override void HitHeadOn(Bob bob) { } public override void SideHit(Bob bob) { } 

        public override void Reset(bool BoxesOnly)
        {
            BlockCore.BoxesOnly = BoxesOnly;

            Active = true;

            BlockCore.Data = BlockCore.StartData;

            MyBox.Current.Center = BlockCore.StartData.Position;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();
        }
        

        public override void PhsxStep()
        {
            Active = Core.Active = true;
            if (!Core.Held)
            {
                if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y)
                    Active = Core.Active = false;
                if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y)
                    Active = Core.Active = false;
            }

            Update();

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);

            BlockCore.StoodOn = false;

            EmitterPhsx();
        }

        void EmitterPhsx()
        {
            if (!Active) return;

            int Step = Core.GetPhsxStep() % Period - Offset;
            //if (Step == 0)
            //{
            //    Fireball NewFireball = (Fireball)Core.Recycle.GetObject(ObjectType.Fireball, Core.BoxesOnly);
            //    EmitData.Position = Core.Data.Position + EmitData.Velocity * .5f;
            //    NewFireball.Initialize(0, EmitData);

            //    NewFireball.Core.Tag = Core.Tag;

            //    NewFireball.Parent = this;
            //    NewFireball.CreationTimeStamp = Core.MyLevel.GetPhsxStep();

            //    Core.MyLevel.AddObject(NewFireball);
            //}

            Step = (Core.GetPhsxStep() % Period - Offset + Period) % Period;
            if (!Core.BoxesOnly)
            {
                float start = .96f;
                float start_shake = .7f;
                float relax = .155f;
                float SuckAmp = 17;
                float OutAmp = 30;
                float ColorAmp = .32f;
                
                // Expand right before emitting
                if (Step > start * Period)
                {
                    Base.e1 = HoldBase.e1;
                    Base.e2 = HoldBase.e2;

                    float t = (Step - start * Period) / ((1 - start) * Period);
                    Base.e1.X = HoldBase.e1.X + (OutAmp + SuckAmp) * t - SuckAmp;
                    Base.e2.Y = HoldBase.e2.Y + (OutAmp + SuckAmp) * t - SuckAmp;
                }
                // Shake before emitting
                else if (Step > start_shake * Period)
                {
                    float t = (Step - start_shake * Period) / ((start - start_shake) * Period);

                    float LessColor = ColorAmp * t;
                    MyQuad.SetColor(new Color(1f, 1 - LessColor, 1 - LessColor, 1f));

                    Base.e1 = HoldBase.e1;
                    Base.e2 = HoldBase.e2;

                    Base.e1.X = HoldBase.e1.X - SuckAmp * t;
                    Base.e2.Y = HoldBase.e2.Y - SuckAmp * t;
                }
                else if (Step < relax * Period)
                {
                    Base.e1 = HoldBase.e1;
                    Base.e2 = HoldBase.e2;

                    float t = 1f - (Step - 0 * Period) / ((relax - 0) * Period);
                    Base.e1.X = HoldBase.e1.X + OutAmp * t;
                    Base.e2.Y = HoldBase.e2.Y + OutAmp * t;

                    float LessColor = ColorAmp * t;
                    MyQuad.SetColor(new Color(1f, 1 - LessColor, 1 - LessColor, 1f));
                }
                else
                {
                    MyQuad.SetColor(Color.White);

                    Base.e1 = HoldBase.e1;
                    Base.e2 = HoldBase.e2;
                }
            }
        }


        public override void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }


        public void Update()
        {
            if (BlockCore.BoxesOnly) return;

            Base.Origin = Core.Data.Position;
            MyQuad.Update(ref Base);

            Ice.Pos = Core.Data.Position;
        }

        public override void Extend(Side side, float pos)
        {
            switch (side)
            {
                case Side.Left:
                    MyBox.Target.BL.X = pos;
                    break;
                case Side.Right:
                    MyBox.Target.TR.X = pos;
                    break;
                case Side.Top:
                    MyBox.Target.TR.Y = pos;
                    break;
                case Side.Bottom:
                    MyBox.Target.BL.Y = pos;
                    break;
            }

            MyBox.Target.FromBounds();
            MyBox.SwapToCurrent();

            Update();

            BlockCore.StartData.Position = MyBox.Current.Center;
        }

        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }

        public override void Draw()
        {
            if (Core.BoxesOnly) return;

            if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 150 || Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 150)
                return;
            if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 150 || Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 150)
                return;

            if (Tools.DrawGraphics)
            {
                Update();
                Ice.Draw();
                Tools.QDrawer.DrawQuad(ref MyQuad);
            }
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            IceBlock BlockA = A as IceBlock;

            Period = BlockA.Period;
            Offset = BlockA.Offset;
            EmitData = BlockA.EmitData;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);
        }

        public override void Write(BinaryWriter writer)
        {
            BlockCore.Write(writer);
        }
        public override void Read(BinaryReader reader) { Core.Read(reader); }

    }
}
