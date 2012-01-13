using System.IO;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class FireballEmitter : IObject
    {
        public void TextDraw() { }
        public void Release()
        {
            Core.Release();
        }

        public int Period, Offset;

        public int FireballType;
        public bool FireOnScreen, AlwaysOn;
        public PhsxData EmitData;

        SimpleQuad MyQuad;
        BasePoint Base, HoldBase;

        public bool DrawEmitter;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public void MakeNew()
        {
            Core.Init();
            Core.MyType = ObjectType.FireballEmitter;
            Core.DrawLayer = 8;

            Core.GenData.LimitGeneralDensity = true;

            FireballType = 0;
            FireOnScreen = true;

            Range = 50;

            if (!Core.BoxesOnly)
            {
                MyQuad.Init();
                MyQuad.MyEffect = Tools.BasicEffect;
                MyQuad.MyTexture = Fireball.EmitterTexture;

                Base.e1 = new Vector2(142, 0) * 1.2f;
                Base.e2 = new Vector2(0, 127) * 1.2f;

                HoldBase = Base;
            }
        }

        public FireballEmitter(bool BoxesOnly)
        {
            CoreData = new ObjectData();
            
            Core.BoxesOnly = BoxesOnly;

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void Update()
        {
            if (Core.BoxesOnly) return;

            Base.Origin = Core.Data.Position;
            //Tools.PointxAxisTo(ref Base.e1, ref Base.e2, EmitData.Velocity);

            MyQuad.Update(ref Base);
        }

        /// <summary>
        /// How far the emitter can be from the edge of the screen and still be active.
        /// </summary>
        public float Range;

        public void PhsxStep()
        {
            int Step;
            if (!DrawEmitter)
            {
                Step = Core.GetPhsxStep() % Period - Offset;
                if (Step != 0) return;
            }

            Core.PosFromParentOffset();

            if (!AlwaysOn)
            {
                Vector2 RangePadding = new Vector2(Range + 850, Range + 850);
                if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, RangePadding))
                {
                    Core.SkippedPhsx = true;
                    return;
                }

                //if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, Range))
                //{
                //    Core.SkippedPhsx = false;
                //    return;
                //}
            }
            Core.SkippedPhsx = false;

            //if (!AlwaysOn)
            //if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + Range ||
            //    Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - Range ||
            //    Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + Range ||
            //    Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - Range)
            //{
            //    Core.SkippedPhsx = true;
            //    return;
            //}
            //Core.SkippedPhsx = false;


            if (!FireOnScreen)
                if (Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, 200))
                    return;

            //if (!FireOnScreen)
            //    if (Core.Data.Position.X < Core.MyLevel.MainCamera.TR.X + 200 &&
            //        Core.Data.Position.X > Core.MyLevel.MainCamera.BL.X - 200 &&
            //        Core.Data.Position.Y < Core.MyLevel.MainCamera.TR.Y + 200 &&
            //        Core.Data.Position.Y > Core.MyLevel.MainCamera.BL.Y - 200)
            //        return;

            Step = Core.GetPhsxStep() % Period - Offset;
            if (Step == 0)
            {
                Fireball NewFireball = (Fireball)Core.Recycle.GetObject(ObjectType.Fireball, Core.BoxesOnly);
                EmitData.Position = Core.Data.Position + EmitData.Velocity * .5f;
                NewFireball.Initialize(FireballType, EmitData);

                if (AlwaysOn) NewFireball.StartLife = NewFireball.Life = 200;

                NewFireball.Core.Tag = Core.Tag;

                NewFireball.Parent = this;
                NewFireball.CreationTimeStamp = Core.MyLevel.GetPhsxStep();

                Core.MyLevel.AddObject(NewFireball);
            }

            Step = (Core.GetPhsxStep() % Period - Offset + Period) % Period;
            if (!Core.BoxesOnly)
            {
                float start = .96f;
                float start_shake = .7f;
                float relax = .155f;
                float SuckAmp = 17;
                float OutAmp = 30;
                float ColorAmp = .32f;
                
                //float start = .965f;
                //float start_shake = .7f;
                //float relax = .13f;
                //float SuckAmp = 23;
                //float OutAmp = 40;
                //float ColorAmp = .62f;

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


                    //float Amp = 12;
                    //if (Step % 8 < 4) { Base.e1.X += Amp; Base.e2.Y -= Amp; }
                    //else { Base.e1.X -= Amp; Base.e2.Y += Amp; }
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

                //ParticleEffects.Flame(Core.MyLevel, Core.Data.Position, Core.MyLevel.GetPhsxStep(), 1, 20, false);//t + .12f);
            }
        }

        public void PhsxStep2() { }

        public void Draw()
        {
            if (!DrawEmitter) return;
            if (Core.BoxesOnly) return;

            if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 150 || Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 150)
                return;
            if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 150 || Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 150)
                return;

            if (Tools.DrawGraphics)
            {
                Update();
                Tools.QDrawer.DrawQuad(MyQuad);
            }
        }

        public void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            Update();
        }

        public void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public void Interact(Bob bob) { }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            FireballEmitter EmitterA = A as FireballEmitter;

            FireOnScreen = EmitterA.FireOnScreen;

            Period = EmitterA.Period;
            Offset = EmitterA.Offset;

            FireballType = EmitterA.FireballType;
            FireOnScreen = EmitterA.FireOnScreen;
            AlwaysOn = EmitterA.AlwaysOn;

            Range = EmitterA.Range;

            EmitData = EmitterA.EmitData;

            DrawEmitter = EmitterA.DrawEmitter;
        }

        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnUsed() { }
public void OnMarkedForDeletion() { }
public void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
//StubStubStubEnd6
    }
}