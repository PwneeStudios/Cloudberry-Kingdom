using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class WorldMapGui : StartMenuBase
    {
        public WorldMapGui() : base()
        {
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(Frames);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, Frames);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MySelectedText.Shadow = item.MyText.Shadow = false;
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        EzText Text, MyScoreText;
        QuadClass TextBack;
        public override void Init()
        {
 	        base.Init();

            MyPile = new DrawPile();
            EnsureFancy();

            //TextBack = new QuadClass();
            //TextBack.TextureName = "backplate_1300x660";
            //MyPile.Add(TextBack, "Backplate");
        }

        void SetPos()
        {
            EzText _t;
            _t = MyPile.FindEzText("Title"); if (_t != null) { _t.Pos = new Vector2(-59.97433f, -640.0692f); }
            _t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(-1262.311f, 953.8732f); }
            MyPile.Pos = new Vector2(0f, 0f);
        }

        public void SetTitle(string Title)
        {
            if (Text != null) { MyPile.Remove(Text); Text.Release(); }
            Text = new EzText(Title, Tools.Font_Grobold42_2, true, false);
            Text.Scale *= .8f;
            MyPile.Add(Text, "Title");

            SetPos();
        }

        public void SetScore(int score)
        {
            if (MyScoreText != null) { MyPile.Remove(MyScoreText); MyScoreText.Release(); }

            if (score <= 0) return;

            MyScoreText = new EzText(string.Format("High score\n  {0}", score), Tools.Font_Grobold42_2, 1000, true, false, .6f);
            MyScoreText.Scale *= .55f;
            MyPile.Add(MyScoreText, "Score");

            SetPos();
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}