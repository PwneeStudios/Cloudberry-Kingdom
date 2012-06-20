using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CloudberryKingdom
{
    public class ControlScreen : StartMenuBase
    {
        QuadClass BackgroundQuad;
      
        public ControlScreen(int Control) : base(false)
        {
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
            SlideLength = 29;
            DestinationScale *= 1.02f;

            BackgroundQuad = new QuadClass();
            BackgroundQuad.SetToDefault();
            BackgroundQuad.Quad.MyTexture = Tools.TextureWad.FindByName("ControllerScreen");

            MyPile = new DrawPile();
            EnsureFancy();

            BackgroundQuad.FullScreen(Tools.CurLevel.MainCamera);
            MyPile.Add(BackgroundQuad);
            BackgroundQuad.Pos = Vector2.Zero;

            ReturnToCallerDelay = 10;

            EzText text;

#if PC_VERSION
            text = new EzText("quick spawn", Tools.Font_DylanThin42);
            text.Scale = 1.06f;
            MyPile.Add(text);
            text.Pos = new Vector2(-404.7632f, 626.9842f);
            text.MyFloatColor = Tools.Gray(.955f);

            text = new EzText("power ups", Tools.Font_DylanThin42);
            text.Scale = 1.06f;
            MyPile.Add(text);
            text.Pos = new Vector2(-380.9531f, 380.9523f);
            text.MyFloatColor = Tools.Gray(.955f);

            text = new EzText("menu", Tools.Font_DylanThin42);
            text.Scale = 1.06f;
            MyPile.Add(text);
            text.Pos = new Vector2(-396.8247f, 976.1902f);
            text.MyFloatColor = CampaignMenu.DifficultyColor[1].ToVector4();

            text = new EzText("accept", Tools.Font_DylanThin42);
            text.Scale = 1.06f;
            MyPile.Add(text);
            text.Pos = new Vector2(-388.8873f, 182.5395f);
            text.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            text.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;

            text = new EzText("back", Tools.Font_DylanThin42);
            text.Scale = 1.06f;
            MyPile.Add(text);
            text.Pos = new Vector2(-380.9512f, -71.42798f);
            text.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
            text.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;

            text = new EzText("b", Tools.Font_DylanThin42);
            text.SubstituteText("<");
            text.Scale = 1.46f;
            MyPile.Add(text);
            text.Pos = new Vector2(-603.1748f, 325.397f);

            QuadClass q;

            q = new QuadClass("Enter_Key"); q.ScaleXToMatchRatio(130);
            MyPile.Add(q);
            q.Pos = new Vector2(-793.6509f, 87.30127f);

            q = new QuadClass("Esc_Key"); q.ScaleXToMatchRatio(130);
            MyPile.Add(q);
            q.Pos = new Vector2(-666.6665f, 761.9049f);

            q = new QuadClass("Backspace_Key"); q.ScaleXToMatchRatio(130);
            MyPile.Add(q);
            q.Pos = new Vector2(-801.5879f, -261.9048f);

            q = new QuadClass("Space_Key"); q.ScaleXToMatchRatio(130);
            MyPile.Add(q);
            q.Pos = new Vector2(-793.6523f, 436.5077f);
#endif
        }

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