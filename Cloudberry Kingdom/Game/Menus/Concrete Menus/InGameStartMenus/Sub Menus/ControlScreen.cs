using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CloudberryKingdom
{
    public class ControlScreen : CkBaseMenu
    {
        QuadClass BackgroundQuad;
      
        public ControlScreen(int Control) : base(false)
        {
            EnableBounce();

            this.Control = Control;

            Constructor();
        }

#if PC_VERSION
        QuadClass MakeQuad(Keys key)
        {
            var quad = new QuadClass(ButtonString.KeyToTexture(key), 90);
            MyPile.Add(quad);
            quad.Quad.SetColor(CustomControlsMenu.SecondaryKeyColor);
            return quad;
        }
#endif
        public override void Init()
        {
 	        base.Init();

            PauseGame = true;

            SlideInFrom = SlideOutTo = PresetPos.Left;

            //ReturnToCallerDelay = SlideLength = 0;
            SlideLength = 23;
            DestinationScale *= 1.02f;

            MyPile = new DrawPile();
            EnsureFancy();

            QuadClass Backdrop;
            if (UseBounce)
                Backdrop = new QuadClass("Arcade_BoxLeft", 1500, true);
            else
                Backdrop = new QuadClass("Backplate_1230x740", 1500, true);
            Backdrop.Name = "Backdrop";
            MyPile.Add(Backdrop);

            ReturnToCallerDelay = 7;

            EzText text;

#if PC_VERSION
            text = new EzText(Localization.Words.QuickSpawn, Resources.Font_Grobold42);
            MyPile.Add(text, "quickspawn");
            text.MyFloatColor = ColorHelper.Gray(.955f);

            text = new EzText(Localization.Words.PowerUpMenu, Resources.Font_Grobold42);
            MyPile.Add(text, "powerups");
            text.MyFloatColor = ColorHelper.Gray(.955f);

            text = new EzText(Localization.Words.Menu, Resources.Font_Grobold42);
            MyPile.Add(text, "menu");
            text.MyFloatColor = CampaignHelper.DifficultyColor[1].ToVector4();

            text = new EzText(Localization.Words.Accept, Resources.Font_Grobold42);
            MyPile.Add(text, "accept");
            text.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            text.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;

            text = new EzText(Localization.Words.Back, Resources.Font_Grobold42);
            MyPile.Add(text, "back");
            text.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
            text.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;

            text = new EzText("b", Resources.Font_Grobold42);
            text.SubstituteText("<");
            MyPile.Add(text, "split");

            QuadClass q;

            q = new QuadClass("Enter_Key"); q.ScaleXToMatchRatio(130);
            MyPile.Add(q, "enter");

            q = new QuadClass("Esc_Key"); q.ScaleXToMatchRatio(130);
            MyPile.Add(q, "esc");

            q = new QuadClass("Backspace_Key"); q.ScaleXToMatchRatio(130);
            MyPile.Add(q, "backspace");

            q = new QuadClass("Space_Key"); q.ScaleXToMatchRatio(130);
            MyPile.Add(q, "space");

            SetPos();
#endif
        }

#if PC_VERSION
        void SetPos()
        {
            EzText _t;
            _t = MyPile.FindEzText("quickspawn"); if (_t != null) { _t.Pos = new Vector2(-288.0965f, 435.3178f); _t.Scale = 1.06f; }
            _t = MyPile.FindEzText("powerups"); if (_t != null) { _t.Pos = new Vector2(-267.0644f, 133.7302f); _t.Scale = 1.06f; }
            _t = MyPile.FindEzText("menu"); if (_t != null) { _t.Pos = new Vector2(-280.1582f, 731.7462f); _t.Scale = 1.06f; }
            _t = MyPile.FindEzText("accept"); if (_t != null) { _t.Pos = new Vector2(-286.109f, -156.3493f); _t.Scale = 1.06f; }
            _t = MyPile.FindEzText("back"); if (_t != null) { _t.Pos = new Vector2(-264.2847f, -432.5391f); _t.Scale = 1.06f; }
            _t = MyPile.FindEzText("split"); if (_t != null) { _t.Pos = new Vector2(-536.5085f, 14.28584f); _t.Scale = 1.46f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(1500f, 902.2556f); }
            _q = MyPile.FindQuad("enter"); if (_q != null) { _q.Pos = new Vector2(-771.4287f, -234.9209f); _q.Size = new Vector2(271.0638f, 130f); }
            _q = MyPile.FindQuad("esc"); if (_q != null) { _q.Pos = new Vector2(-638.8887f, 520.2384f); _q.Size = new Vector2(138.2979f, 130f); }
            _q = MyPile.FindQuad("backspace"); if (_q != null) { _q.Pos = new Vector2(-773.8103f, -603.5712f); _q.Size = new Vector2(271.0638f, 130f); }
            _q = MyPile.FindQuad("space"); if (_q != null) { _q.Pos = new Vector2(-768.6523f, 205.9521f); _q.Size = new Vector2(271.0638f, 130f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }
#endif

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            if (ButtonCheck.State(ControllerButtons.A, -1).Pressed ||
                ButtonCheck.State(ControllerButtons.B, -1).Pressed)
            {
                Active = false;
                ReturnToCaller();
            }
        }
    }
}