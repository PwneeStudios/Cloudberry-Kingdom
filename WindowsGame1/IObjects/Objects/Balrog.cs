using System.Text;
using Microsoft.Xna.Framework;

using Drawing;
using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Balrogs
{
    public class BalrogFlyupTrigger : ObjectBase
    {
        public bool Safe;

        static int[] Duration = { 0, 30, 10, 20, 10, 3, 2, 3, 2, 3, 2, 10, 0 };
        static EzSound TinkSound;

        public Vector2 FlyupStart;
        public AABox TriggerBox;
        SimpleQuad WarningQuad;
        BasePoint Base;

        int TriggeredTimeStamp;
        int DurationIndex, Count;
        bool Triggered, DrawMark;

        public BalrogFlyupTrigger()
        {
            if (TinkSound == null) TinkSound = Tools.SoundWad.FindByName("!Tink");

            WarningQuad = new SimpleQuad();

            WarningQuad.Init();
            WarningQuad.MyEffect = Tools.BasicEffect;
            WarningQuad.MyTexture = Tools.TextureWad.FindByName("!");
                        
            Base.e1 = new Vector2(540, 0);
            Base.e2 = new Vector2(0, 760);
        }

        public void Init(Vector2 Position)
        {
            Core.Data.Position = Base.Origin = Position;
            TriggerBox = new AABox(Position, new Vector2(600, 3000));

            FlyupStart = Position - new Vector2(0, 2200);
        }
        
        public override void Reset(bool BoxesOnly)
        {
            Triggered = DrawMark = false;
            Count = DurationIndex = 0;
        }

        public void Trigger()
        {
            if (!Triggered)
            {
                TriggeredTimeStamp = Core.MyLevel.GetPhsxStep();

                DurationIndex = 0;
                Count = 0;

                Triggered = true;
                DrawMark = false;
            }
        }

        public override void Interact(Bob bob)
        {
            ColType col = Phsx.CollisionTest(bob.Box, TriggerBox);
            if (col != ColType.NoCol) Trigger();
        }

        public override void PhsxStep()
        {
            if (Triggered)
            {
                Count++;
                if (Count > Duration[DurationIndex])
                {
                    DrawMark = !DrawMark;
                    if (DrawMark && Core.MyLevel.PlayMode == 0) TinkSound.Play();
                    DurationIndex++;
                    Count = 0;

                    if (DurationIndex >= Duration.Length)
                    {
                        Triggered = false;
                    }
                    if (DurationIndex == Duration.Length - 2)
                    {
                     //   Core.MyLevel.TheBalrog.Trigger = this;
                       // Core.MyLevel.TheBalrog.SetToFlyup(FlyupStart);
                    }
                }
            }
        }

        public override void Draw()
        {
            if (Triggered && DrawMark)
            {
                WarningQuad.Update(ref Base);

                if (Tools.DrawGraphics)
                    Tools.QDrawer.DrawQuad(ref WarningQuad);
            }

            if (Tools.DrawBoxes)
                TriggerBox.DrawT(Tools.QDrawer, Color.Blue, 20);
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;

            TriggerBox.Move(shift);

            Base.Origin += shift;
        }
    }

    public class Balrog : ObjectBase
    {
        public BalrogFlyupTrigger Trigger;

        public bool Active;

        public bool BoxesOnly;

        static EzSound WooshSound;
         
        public SimpleObject MyObject;
 
        public AABox Box;

        public override void MakeNew()
        {
            MyObject.Read(0, 0);
            MyObject.Update();
            Box.Initialize(Core.Data.Position, MyObject.Boxes[0].Size() / 2);
        }
    
        public Balrog(Balrog balrog, bool boxesOnly)
        {
            BoxesOnly = boxesOnly;

            MyObject = new SimpleObject(balrog.MyObject, BoxesOnly);

            Core.Data = balrog.Core.Data;

            Box = new AABox();

            MakeNew();
        }

        public Balrog(string file, EzEffectWad EffectWad, EzTextureWad TextureWad)
        {
            WooshSound = Tools.SoundWad.FindByName("Balrog_Woosh");

            ObjectClass SourceObject;
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
            SourceObject = new ObjectClass(Tools.QDrawer, Tools.Device, Tools.Device.PresentationParameters, 100, 100, EffectWad.FindByName("BasicEffect"), TextureWad.FindByName("White"));
            SourceObject.ReadFile(reader, EffectWad, TextureWad);
            reader.Close();
            stream.Close();

            //MyObject.FinishLoading(Tools.QDrawer, Tools.Device, Tools.TextureWad, Tools.EffectWad, Tools.Device.PresentationParameters, Tools.Device.PresentationParameters.BackBufferWidth / 8, Tools.Device.PresentationParameters.BackBufferHeight / 8);

            SourceObject.ConvertForSimple();
            MyObject = new SimpleObject(SourceObject);
            MyObject.Base.e1 *= 755;// *.125f;
            MyObject.Base.e2 *= 755;// *.125f;

            MyObject.Read(0, 0);
            MyObject.Play = true;

            MyObject.Update();

            Box = new AABox(Core.Data.Position, MyObject.Boxes[0].Size() / 2);
        }

        public void Init(bool BoxesOnly, PhsxData StartData)
        {
            Box.SetTarget(Core.Data.Position, Box.Current.Size);
            Box.SwapToCurrent();

            UpdateObject();
        }
        
        public void UpdateObject()
        {
            if (MyObject != null)
            {
                Vector2 NewCenter = Core.Data.Position - (MyObject.Boxes[0].TR.Vertex.xy - MyObject.Base.Origin - Box.Current.Size);

                MyObject.Base.Origin = NewCenter;
                MyObject.Update();

                Core.Data.Position = MyObject.Boxes[0].Center();
            }
        }

        public override void Draw()
        {
            if (!Active) return;

            Vector2 BL = Box.Current.BL - new Vector2(850, 850);
            if (BL.X > Core.MyLevel.MainCamera.TR.X || BL.Y > Core.MyLevel.MainCamera.TR.Y)
                return;
            Vector2 TR = Box.Current.BL + new Vector2(850, 850);
            if (TR.X < Core.MyLevel.MainCamera.BL.X || TR.Y < Core.MyLevel.MainCamera.BL.Y)
                return;

            if (Tools.DrawGraphics)
                MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
            if (Tools.DrawBoxes)
            {
                Box.DrawT(Tools.QDrawer, Color.Blue, 10);
                Box.Draw(Tools.QDrawer, Color.Azure, 10);
            }
        }

        public void SetToFlyup(Vector2 StartPos)
        {
            Active = true;

            if (Core.MyLevel.PlayMode == 0)
            {
                WooshSound.Play();
                foreach (Bob bob in Core.MyLevel.Bobs)
                    Tools.SetVibration(bob.MyPlayerIndex, 1f, 1f, 45);
            }

            MyObject.Read(1, 0);

            Core.Data.Position = StartPos;
            Core.Data.Velocity = new Vector2(0, 55);

            Box.SetTarget(Core.Data.Position, Box.Current.Size);
            Box.SwapToCurrent();
        }

        public override void PhsxStep()
        {
            if (!Active) return;

            Core.Data.Position += Core.Data.Velocity;

            Box.SetTarget(Core.Data.Position, Box.Current.Size);            
        }

        public override void PhsxStep2()
        {
            if (!Active) return;

            Box.SwapToCurrent();

            UpdateObject();
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;

            Box.Move(shift);

            MyObject.Base.Origin += shift;
            MyObject.Update();
        }

        public override void Interact(Bob bob)
        {
            if (Active)
            {
                ColType Col = Phsx.CollisionTest(bob.Box, Box);
                if (Col != ColType.NoCol)
                {
                    //if (Core.MyLevel.PlayMode == 0)
                   //     bob.Die();
                    //else
                       // Trigger.Core.MarkedForDeletion = true;
                }
            }
        }
    }
}
