using System.IO;

using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public partial class Balrog : Boss, IObject
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

        public Balrog()
        {
            CoreData = new ObjectData();

            EzReader reader = new EzReader(Path.Combine(Globals.ContentDirectory, "Objects\\Balrog.smo"));
            MyObject = new ObjectClass();
            MyObject.ReadFile(reader);
            MyObject.FinishLoading();
            MyObject.Scale(new Vector2(1700, 1700));
            reader.Dispose();

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

        EzText WantedText1, WantedText2;
        void InitTitle()
        {
            Title = new EzText("Balrog", Tools.Font_Dylan42, 2000, true, true);
            Title.FancyPos = new FancyVector2();
            TitlePos = new Vector2(280, -650);
            Title.FancyPos.RelVal = TitlePos + Core.StartData.Position;// +new Vector2(0, 700);
            Title.MyFloatColor = new Color(252, 131, 0).ToVector4();
            Title.OutlineColor = new Color(255, 255, 255).ToVector4();
            //Title.MyFloatColor = Color.Gray.ToVector4();
            //Title.OutlineColor = Color.Red.ToVector4();

            Title.Shadow = true;
            Title.ShadowOffset = new Vector2(10.5f, 10.5f);
            Title.ShadowColor = new Color(30, 30, 30);

            //Title.BackdropShift = new Vector2(-23, 0);
            //Title.BackdropTemplate = PieceQuad.SpeechBubble;
            //Title.AddBackdrop(.5f * Vector2.One, new Vector2(20, 20));
            //Title.Backdrop.SetColor(Color.DarkGray);
            //Title.Backdrop.SetAlpha(.85f);
            
            //Title.Backdrop.Clone(PieceQuad.SpeechBubble);

            //Title.FancyPos.LerpTo(TitlePos, 60);
            //Title.Scale = .9f;


            WantedText1 = new EzText("Wanted! Dead or alive.\nReward of {pStar,65,?}x50", Tools.Font_Dylan28, 2000, true, true);
            WantedText1.FancyPos = new FancyVector2(Title.FancyPos);
            WantedText1.FancyPos.RelVal = new Vector2(150, -250);
            WantedText1.MyFloatColor = new Color(252, 131, 0).ToVector4();
            WantedText1.OutlineColor = new Color(255, 255, 255).ToVector4();

            WantedText1.Shadow = true;
            WantedText1.ShadowOffset = new Vector2(10.5f, 10.5f);
            WantedText1.ShadowColor = new Color(30, 30, 30);

            //WantedText1.Scale = .9f;
        }

        public override void DrawTitle()
        {
            if (!Core.Active) return;

            base.DrawTitle();

            //WantedText1.Draw(cam);
        }

        void Init_WantedMode()
        {
            Vector2 shift = new Vector2(0, -150);
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
            MyObject.Draw(true);
        }

        public override void PhsxStep()
        {
            if (!Core.Active) return;

            base.PhsxStep();
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
//StubStubStubEnd6
    }
}