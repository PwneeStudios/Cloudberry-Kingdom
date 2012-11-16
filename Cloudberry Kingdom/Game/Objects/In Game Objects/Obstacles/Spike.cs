using System;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class Spike : _BoxDeath
    {
        public class SpikeTileInfo : TileInfoBase
        {
            public SpriteInfo Spike = new SpriteInfo(null);
            public SpriteInfo Base = new SpriteInfo(null);

            public float PeakHeight = .2f;

            public float TopOffset = 2, BottomOffset = 2, SideOffset = 2;

            public Vector2 ObjectSize = new Vector2(575, 535);
        }

        static bool PeakOut = true;

        public SimpleObject MyObject;        

        public QuadClass MyQuad, MyBaseQuad;

        public int Dir;
        public float Angle;

        public int Offset, UpT, DownT, WaitT1, WaitT2;

        public bool Exposed;

        public void SetPeriod(int Period)
        {
            Period -= 7 + 7;

            float Total = 20 + 40;

            //UpT = (int)(7 * Period / Total);
            WaitT1 = (int)(20 * Period / Total);
            //DownT = (int)(7 * Period / Total);
            WaitT2 = (int)(40 * Period / Total);
        }

        public override void MakeNew()
        {
            MyObject.Linear = true;

            UpT = 7;
            WaitT1 = 20;
            DownT = 7;
            WaitT2 = 40;

            SetDir(Prototypes.SpikeObj.Dir);

            Core.Init();
            Core.MyType = ObjectType.Spike;
            Core.ContinuousEnabled = true;
            Core.DrawLayer = 2;

            Core.GenData.NoBlockOverlap = true;
            Core.GenData.LimitGeneralDensity = true;

            Core.WakeUpRequirements = true;
        }

        float SetHeight;
        public override void Init(Vector2 Pos, Level level)
        {
            SpikeTileInfo info = level.Info.Spikes;

            Vector2 size = info.ObjectSize * level.Info.ScaleAll * level.Info.ScaleAllObjects;
            MyObject.Base.e1 = new Vector2(size.X, 0);
            MyObject.Base.e2 = new Vector2(0, size.Y);

            Box.Initialize(Core.Data.Position, Prototypes.SpikeObj.MyObject.Boxes[0].Size() / 2);

            MyObject.Read(0, 0);
            MyObject.Update();
            UpdateObject();

            

            if (!level.BoxesOnly)
            {
                //if (info.Base.Sprite == null)
                //    MyBaseQuad.Show = false;
                //else
                if (info.Base.Sprite != null)
                {
                    MyBaseQuad = new QuadClass();
                    MyBaseQuad.Set(info.Base);
                    MyObject.Quads[1].Hide = true;
                }

                //if (info.Spike.Sprite == null)
                //    MyQuad.Show = false;
                //else
                if (info.Spike.Sprite != null)
                {
                    MyQuad = new QuadClass();
                    MyQuad.Set(info.Spike);
                    SetHeight = MyQuad.Size.Y;
                    MyObject.Quads[1].Hide = true;
                }
            }

            Box.SetTarget(Core.Data.Position, Box.Current.Size);
            Box.SwapToCurrent();
        }

        public Spike(bool BoxesOnly)
        {
            Construct(BoxesOnly);
        }

        public override void Construct(bool BoxesOnly)
        {
            MyObject = new SimpleObject(Prototypes.SpikeObj.MyObject, BoxesOnly);

 	        base.Construct(BoxesOnly);
        }

        public void SetDir(int dir)
        {
            Dir = dir;
            SetAngle(Dir * (float)Math.PI / 2);
        }

        Vector2 unit;
        public void SetAngle(float Ang)
        {
            Angle = Ang;
            Vector2 Direction = new Vector2((float)Math.Cos(Ang), (float)Math.Sin(Ang));
            CoreMath.PointxAxisTo(ref MyObject.Base.e1, ref MyObject.Base.e2, Direction);

            if (MyQuad != null)
                MyQuad.PointxAxisTo(Direction);
            if (MyBaseQuad != null)
            {
                MyBaseQuad.PointxAxisTo(Direction);
                unit = MyBaseQuad.Base.e2;
                unit.Normalize();
            }
        }


        public Spike(string file, EzEffectWad EffectWad, EzTextureWad TextureWad)
        {
            ObjectClass SourceObject;
            Tools.UseInvariantCulture();
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);

            SourceObject = new ObjectClass(Tools.QDrawer, Tools.Device, EffectWad.FindByName("BasicEffect"), TextureWad.FindByName("White"));
            SourceObject.ReadFile(reader, EffectWad, TextureWad);
            reader.Close();
            stream.Close();

            SourceObject.ConvertForSimple();
            MyObject = new SimpleObject(SourceObject);

            MyObject.Quads[1].Animated = false;
            
            MyObject.Read(0, 1);
            MyObject.Play = true;
            MyObject.EnqueueAnimation(0, 0, true);
            MyObject.DequeueTransfers();
            MyObject.Update();


            Core.Data.Position = new Vector2(0, 0);
            Core.Data.Velocity = new Vector2(0, 0);

            Box = new AABox(Core.Data.Position, MyObject.Boxes[0].Size() / 2);
        }

        public override void PhsxStep()
        {
            Core.PosFromParentOffset();

            Vector2 PhsxCutoff = new Vector2(200, 200);
            if (Core.MyLevel.BoxesOnly) PhsxCutoff = new Vector2(-150, 200);
            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, PhsxCutoff))
            {
                Core.SkippedPhsx = true;
                Core.WakeUpRequirements = true;
                return;
            }
            Core.SkippedPhsx = false;

            if (Core.WakeUpRequirements)
            {
                UpdateObject();
            }            

            AnimStep();
            UpdateObject();

            Box.Current.Center = MyObject.Boxes[0].Center();
            Box.Current.Size = CoreMath.Abs(MyObject.Boxes[0].Size()) / 2;
            Box.SetTarget(Box.Current.Center, Box.Current.Size + new Vector2(.0f, .02f));
            
            if (Core.WakeUpRequirements)
            {
                Box.SwapToCurrent();
                Core.WakeUpRequirements = false;
            }
        }

        public override void PhsxStep2()
        {
            if (Core.SkippedPhsx) return;

            Box.SetCurrent(Box.Current.Center, Box.Current.Size);
        }


        public void AnimStep() { AnimStep(Core.SkippedPhsx); }
        public void AnimStep(bool Skip)
        {
            if (Skip) return;

            Exposed = true;

            float PeakHeight;
            if (PeakOut) PeakHeight = Info.Spikes.PeakHeight;
            else PeakHeight = .01f;

            float AnimSpeed = 0;

            float t = (float)CoreMath.Modulo(Core.GetIndependentPhsxStep() + Offset, UpT + DownT + WaitT1 + WaitT2);
            if (t < UpT) MyObject.t = PeakHeight + (1 - PeakHeight) * t / (float)UpT;
            else if (t < UpT + WaitT1) MyObject.t = 1;
            else if (t < UpT + WaitT1 + DownT) MyObject.t = 1 + .9f * (t - UpT - WaitT1) / (float)DownT;
            else MyObject.t = 1.9f +.1f * (t - UpT - WaitT1 - DownT) / (float)WaitT2;

            if (!Core.BoxesOnly)
            {
                // Peak out before showing
                if (PeakOut)
                {
                    float PeakTime = 1.9655f;
                    float MaxPeakTime = 1.98f;
                    if (MyObject.t > PeakTime)
                    {
                        MyObject.t = PeakHeight * Math.Min(1f, (MyObject.t - PeakTime) / (MaxPeakTime - PeakTime));
                    }
                }
                // Shake before showing
                else
                {
                    if (MyObject.t > 1.9655f)
                    {
                        float HoldAngle = Angle;
                        SetAngle(Angle + MyLevel.Rnd.RndFloat(-.385f, .385f));
                        Angle = HoldAngle;
                    }
                    else
                        SetAngle(Angle);
                }
            }

            if (t < UpT / 3) Exposed = false;
            if (t > UpT + WaitT1 + .66f * DownT) Exposed = false;

            MyObject.PlayUpdate(AnimSpeed * 1000f / 60f / 150f);

            var s = MyObject.t;
            if (s > 1) s = 2 - s;
            if (MyQuad != null && MyQuad.Show)
                MyQuad.Base.e2 = unit * SetHeight * s;
        }

        public void UpdateObject()
        {
            if (MyObject != null)
            {
                MyObject.Base.Origin = Core.Data.Position;
                MyObject.Update();
            }
        }

        protected override void DrawGraphics()
        {
            //if (MyBaseQuad.Quad._MyTexture != null)
            if (MyBaseQuad != null)
            {
                MyQuad.Pos = Pos;
                MyQuad.Draw();

                //MyBaseQuad.Quad.MyEffect = Tools.EffectWad.FindByName("Hsl");
                MyBaseQuad.Pos = Pos;
                MyBaseQuad.Draw();
            }
            else
                MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
        }

        protected override void DrawBoxes()
        {
            Box.DrawFilled(Tools.QDrawer, Color.Blue);
        }

        public override void Move(Vector2 shift)
        {
            base.Move(shift);
            
            MyObject.Base.Origin += shift;
            MyObject.Update();
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);
        }

        public override void Interact(Bob bob)
        {
            if (!Core.SkippedPhsx && Exposed)
            {
                bool Col = Phsx.BoxBoxOverlap(bob.Box2, Box);
                if (Col)
                {
                    if (Core.MyLevel.PlayMode == 0)
                        bob.Die(Bob.BobDeathType.Spike, this);

                    if (Core.MyLevel.PlayMode != 0)
                    {
                        bool col = Phsx.BoxBoxOverlap_Tiered(Box, Core, bob, Spike_AutoGen.Instance);

                        if (col)
                            Core.Recycle.CollectObject(this);
                    }
                }
            }
        }

        public void CloneBoxObject(SimpleObject SimpleObjA, SimpleObject SimpleObjB)
        {
            SimpleObjA.Base = SimpleObjB.Base;
            for (int i = 0; i < SimpleObjA.Boxes.Length; i++)
            {
                SimpleObjA.Boxes[i].TR = SimpleObjB.Boxes[i].TR;
                SimpleObjA.Boxes[i].BL = SimpleObjB.Boxes[i].BL;
            }
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);
                        
            Spike SpikeA = A as Spike;
            Init(A.Pos, A.MyLevel);

            SetDir(SpikeA.Dir);

            Angle = SpikeA.Angle;
            Offset = SpikeA.Offset;
            UpT = SpikeA.UpT;
            DownT = SpikeA.DownT;
            WaitT1 = SpikeA.WaitT1;
            WaitT2 = SpikeA.WaitT2;

            Core.WakeUpRequirements = true;
            UpdateObject();

            Exposed = SpikeA.Exposed;
        }
    }
}
