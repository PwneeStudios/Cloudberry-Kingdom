using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class HighScorePanel : CkBaseMenu
    {
        public ScoreList MyScoreList;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            if (Panels != null)
                for (int i = 0; i < Panels.Length; i++)
                    Panels[i].Release();
            Panels = null;
        }

        public HighScorePanel(ScoreList Scores)
        {
            Constructor(Scores);
        }

        bool Instant = true;
        public void Constructor(ScoreList Scores)
        {
            if (Instant)
                NoDelays();
            else
                Fast();
                

            MyScoreList = Scores;
            Create();
            SlideIn();
        }


        class HighScoreReturnToCallerLambda : Lambda
        {
            HighScorePanel hsp;
            public HighScoreReturnToCallerLambda(HighScorePanel hsp)
            {
                this.hsp = hsp;
            }

            public void Apply()
            {
                hsp.ReturnToCaller();
            }
        }

        static string[] TextureName = { "score_screen_grey", "score_screen_grey", "score_screen_grey" };
        HighScorePanel[] Panels;

        public HighScorePanel(params ScoreList[] Scores) { MultiInit(false, Scores); }
        public HighScorePanel(bool Instant, params ScoreList[] Scores) { MultiInit(Instant, Scores); }
        public void MultiInit(bool Instant, params ScoreList[] Scores)
        {
            this.Instant = Instant;

            OnOutsideClick = new HighScoreReturnToCallerLambda(this);
            CheckForOutsideClick = true;

            Constructor(Scores[0]);

            Panels = new HighScorePanel[Scores.Length];
            
            Panels[0] = this;
            Panels[0].Backdrop.TextureName = "score_screen_grey";

            for (int i = 1; i < Scores.Length; i++)
            {
                Panels[i] = new HighScorePanel(Scores[i]);
                Panels[i].Backdrop.TextureName = "score_screen_grey";
            }

            for (int i = 0; i < Scores.Length; i++)
                Panels[i].MakeSwapText();
        }

        void SwapPanels()
        {
            for (int i = 0; i < Panels.Length - 1; i++)
                Tools.Swap(ref Panels[i].MyPile, ref Panels[i+1].MyPile);

            MyPile.Jiggle(true, 8, .3f);
        }

        public override void Init()
        {
            base.Init();

            Core.DrawLayer = Level.AfterPostDrawLayer;
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public static Vector4 ScoreColor = new Color(255, 255, 255).ToVector4();
        public static Vector4 CurrentScoreColor = new Color(22, 22, 222).ToVector4();
        public QuadClass Backdrop;
        void Create()
        {
            MyPile = new DrawPile();

            // Make the backdrop
            Backdrop = new QuadClass("Backplate_1500x900", 500, true);
            Backdrop.Degrees = 90;
            Backdrop.TextureName = "Score\\Score_Screen_grey";

            MyPile.Add(Backdrop, "Backdrop");
            Backdrop.Pos = new Vector2(22.2233f, 10.55567f);

            // 'High Score' text
            EzText Text = new EzText(MyScoreList.GetHeader(), Resources.Font_Grobold42_2, 1450, false, true, .6f);
            Text.Scale = .8f;
            Text.MyFloatColor = new Color(255, 255, 255).ToVector4();
            Text.OutlineColor = new Color(0, 0, 0).ToVector4();
            Text.Pos = new Vector2(-675.6388f, 585);
            MyPile.Add(Text, "Header");
            Text.Shadow = false;
            Text.ShadowColor = new Color(.36f, .36f, .36f, .86f);
            Text.ShadowOffset = new Vector2(-24, -20);

            // Scores
            int DesiredLength = 35;
            float y_spacing = 120;
            Vector2 pos = new Vector2(-973, 322);
            foreach (ScoreEntry score in MyScoreList.Scores)
            {
                Text = new EzText(MyScoreList.ScoreString(score, DesiredLength), Resources.Font_Grobold42);
                SetHeaderProperties(Text);
                Text.Scale *= .55f;

                if (score.Date == ScoreDatabase.MostRecentScoreDate)
                    Text.MyFloatColor = Color.LimeGreen.ToVector4();
                else
                    Text.MyFloatColor = ScoreColor;
                Text.Pos = pos;
                pos.Y -= y_spacing;

                MyPile.Add(Text);
            }

            SetPos();
        }

        void SetPos()
        {
            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-675.6388f, 585f); _t.Scale = 0.8f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-102.7767f, -253.3332f); _q.Size = new Vector2(1930.665f, 1206.665f); }

            MyPile.Pos = new Vector2(161.1108f, 227.7778f);

        }

        EzText SwapText;
        void MakeSwapText()
        {
#if PC_VERSION
            //SwapText = new EzText(ButtonString.Enter(200), Resources.Font_Grobold42_2, 1450, false, true, .6f);
            //SwapText.Pos = new Vector2(-1169.281f, 602.9366f);
#else
            SwapText = new EzText(ButtonString.Go(130), Resources.Font_Grobold42_2, 1450, false, true, .6f);
            SwapText.Pos = new Vector2(-1014.837f, 597.3811f);
#endif

            if (SwapText != null)
            {
                SwapText.Scale = .8f;
                SwapText.MyFloatColor = new Color(255, 255, 255).ToVector4();
                SwapText.OutlineColor = new Color(0, 0, 0).ToVector4();

                MyPile.Add(SwapText);
                SwapText.ShadowColor = new Color(.36f, .36f, .36f, .86f);
                SwapText.ShadowOffset = new Vector2(-24, -20);
            }
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.MyFloatColor = new Color(255, 254, 252).ToVector4();

            text.OutlineColor = new Color(0, 0, 0).ToVector4();
            text.Scale *= 1.48f;

            text.Shadow = false;
        }

        void MakeMenu()
        {
            MyMenu = new Menu(false);

            MyMenu.Control = -1;

            MyMenu.OnB = null;


            MenuItem item;
            FontScale *= .89f * 1.16f;

            item = new MenuItem(new EzText(Localization.Words.PlayAgain, ItemFont));
            item.Go = new Action_PlayAgainProxy1(this);
            AddItem(item);

            item = new MenuItem(new EzText(Localization.Words.HighScores, ItemFont));
            item.Go = null;
            AddItem(item);

            item = new MenuItem(new EzText(Localization.Words.Done, ItemFont));
            item.Go = new Action_DoneProxy1(this);
            AddItem(item);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MyText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();
        }

        class HighScorePanelEndGameHelper : Lambda
        {
            HighScorePanel hsp;
            bool param;

            public HighScorePanelEndGameHelper(HighScorePanel hsp, bool param)
            {
                this.hsp = hsp;
                this.param = param;
            }

            public void Apply()
            {
                hsp.MyGame.EndGame.Apply(param);
            }
        }

        class Action_DoneProxy1 : Lambda_1<MenuItem>
        {
            HighScorePanel hsp;

            public Action_DoneProxy1(HighScorePanel hsp)
            {
                this.hsp = hsp;
            }

            public void Apply(MenuItem dummy)
            {
                hsp.Action_Done();
            }
        }

        void Action_Done()
        {
            SlideOut(PresetPos.Top, 13);
            Active = false;
            
            MyGame.WaitThenDo(36, new HighScorePanelEndGameHelper(this, false));
            return;
        }

        class Action_PlayAgainProxy1 : Lambda_1<MenuItem>
        {
            HighScorePanel hsp;

            public Action_PlayAgainProxy1(HighScorePanel hsp)
            {
                this.hsp = hsp;
            }

            public void Apply(MenuItem dummy)
            {
                hsp.Action_PlayAgain();
            }
        }

        void Action_PlayAgain()
        {
            SlideOut(PresetPos.Top, 13);
            Active = false;

            MyGame.WaitThenDo(36, new HighScorePanelEndGameHelper(this, true));
            return;
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            if (ButtonCheck.State(ControllerButtons.A, -1).Pressed ||
                ButtonCheck.State(ControllerButtons.X, -1).Pressed)
                SwapPanels();

            if (//ButtonCheck.State(ControllerButtons.A, -1).Pressed ||
                ButtonCheck.State(ControllerButtons.B, -1).Pressed)
                ReturnToCaller();
        }

        public override bool HitTest(Vector2 pos)
        {
            return Backdrop.HitTest(pos, new Vector2(-50));
        }
    }
}