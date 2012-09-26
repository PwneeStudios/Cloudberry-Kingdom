using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_Forest_PressStart : StartMenuBase
    {
        public TitleGameData_Forest Title;
        public StartMenu_Forest_PressStart(TitleGameData_Forest Title)
            : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(0);
            Title.Title.SetPos();
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        EzText Text;
        int DelayToAllowInput;
        public override void Init()
        {
 	        base.Init();

            DelayToAllowInput = 80;
            CallDelay = ReturnToCallerDelay = 0;

            MyPile = new DrawPile();

            EnsureFancy();

            Text = new EzText("Press any key to start", Tools.Font_Grobold42, true);
            Text.MyFloatColor = new Color(213, 200, 227).ToVector4();
            Text.OutlineColor = Color.Black.ToVector4();
            MyPile.Add(Text, "Press");

            SetPos();
        }
        
        void SetPos()
        {
            EzText _t;
            _t = MyPile.FindEzText("Press"); if (_t != null) { _t.Pos = new Vector2(8.332428f, -672.2224f); _t.Scale = 0.8212293f; }
            MyPile.Pos = new Vector2(0f, 0f);
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep(); if (!Active) return;

            if (DelayToAllowInput > 0)
            {
                DelayToAllowInput--;
                return;
            }

            if (ButtonCheck.AnyKey())
            {
                DelayToAllowInput = 10;

                Call(new StartMenu_Forest(Title));
                Hide();
            }
        }

        float t = -.3f;
        protected override void MyDraw()
        {
            if (!Active) return;

            Vector4 c1 = new Color(213, 200, 227).ToVector4();
            Vector4 c2 = new Color(190, 200, 227).ToVector4();
            Text.MyFloatColor = Vector4.Lerp(c1, c2, CoreMath.Periodic(0, 1, 3, Tools.t));

            t += .01f;
            Text.Alpha = t;

            //Text.Pos = new Vector2(-922.2231f, -619.4446f + CoreMath.Periodic(0, 8, 3, Tools.t));
            Text.Scale = .8f * CoreMath.Periodic(.818668f, .831668f, 3, Tools.t);

            base.MyDraw();
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();
        }
    }
}