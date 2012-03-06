using System.IO;

using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public partial class BlockHead : Boss, IObject
    {
        public void TextDraw() { }

        public void Release()
        {
            Core.Release();
        }

        ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        ObjectClass MyObject;

        public void MakeNew()
        {
            Core.Init();
            Core.DrawLayer = 5;

            //Core.ResetOnlyOnReset = true;
            Core.RemoveOnReset = true;
        }
        
        public BlockHead()
        {
            CoreData = new ObjectData();

            //EzReader reader = new EzReader(Path.Combine(Globals.ContentDirectory, "Objects\\BlockHead.smo"));
            EzReader reader = new EzReader(Path.Combine(Globals.ContentDirectory, "Objects\\Stickman.smo"));
            MyObject = new ObjectClass();
            MyObject.ReadFile(reader);
            MyObject.FinishLoading();
            //MyObject.Scale(new Vector2(700, 700));
            MyObject.Scale(new Vector2(2050, 2050));
            reader.Dispose();

            Vector2 size = MyObject.BoxList[0].Size();
            float ratio = size.Y / size.X;
            MyObject.FinishLoading(Tools.QDrawer, Tools.Device, Tools.TextureWad, Tools.EffectWad, Tools.Device.PresentationParameters, 400, (int)(400 * ratio), false);

            MyObject.InsideColor = Color.White;

            MakeNew();
        }

        void Shake()
        {
            Core.MyLevel.MainCamera.StartShake(1, 79);
        }

        public enum Mode { Boss, Wanted };
        public Mode MyMode;
        public void Init(Vector2 center, Mode mode)
        {
            MyMode = mode;

            Tools.MoveTo(this, center);
            Core.StartData.Position = center;

            cam = Core.MyLevel.MainCamera;

            switch (MyMode)
            {
                case Mode.Boss: Init_BossMode(); return;
                case Mode.Wanted: Init_WantedMode(); return;
            }
        }

        void Init_WantedMode()
        {
            //Vector2 shift = new Vector2(0, -200 - cam.GetHeight() / 2);
            Vector2 shift = new Vector2(0, -710);
            Move(shift);

            // Title
            InitTitle();

            // Idle wanted animation
            Wanted_Idle = Make_Wanted_Idle();

            // Stickman, from above
            //Stickman_FromAbove = Make_Stickman_FromAbove(Wanted_Idle);

            //CurState = Stickman_FromAbove;
            CurState = Wanted_Idle;
            CurState.Begin();
            PhsxStep();
        }

        void InitTitle()
        {
            //TitlePos = new Vector2(425, 350);
            TitlePos = new Vector2(425, -100);

            Title = new EzText("BF Stickman", Tools.Font_Dylan42, 2000, true, true);
            Title.FancyPos = new FancyVector2();
            Title.FancyPos.RelVal = TitlePos + Core.StartData.Position;// +new Vector2(0, 700);
            Title.MyFloatColor = new Color(252, 131, 0).ToVector4();
            Title.OutlineColor = new Color(255, 255, 255).ToVector4();

            Title.Shadow = true;
            Title.ShadowOffset = new Vector2(10.5f, 10.5f);
            Title.ShadowColor = new Color(30, 30, 30);

            //Title.FancyPos.LerpTo(TitlePos, 60);
            Title.Scale = .75f;
        }

        void Init_BossMode()
        {
            //CurState = Stickman_FromAbove;
            CurState.Begin();
            PhsxStep();
        }

        public void Draw()
        {
            if (!Core.Active) return;

            MyObject.PlayUpdate(.08f);
            MyObject.Update(null);
            //MyObject.Draw(true);
            MyObject.ContainedDraw();
        }

        public override void PhsxStep()
        {
            if (!Core.Active) return;

            base.PhsxStep();

            MyObject.PreDraw(Tools.Device, Tools.EffectWad);
        }

        public void PhsxStep2() { }
        public void Reset(bool BoxesOnly)
        {
        }

        public void Clone(IObject A) { }
        public void Interact(Bob bob) { }
        public void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            Core.StartData.Position += shift;

            if (Title != null) Title.Pos += shift;

            MyObject.MoveTo(Core.Data.Position);
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
public bool PreDecision(Bob bob) { return false; }
//StubStubStubEnd7
    }
}