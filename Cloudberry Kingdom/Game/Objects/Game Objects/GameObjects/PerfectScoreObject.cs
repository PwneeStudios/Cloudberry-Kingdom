using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    /// <summary>
    /// This object tracks a player's action.
    /// When a level is finished without dying and with every coin grabbed, a bonus is given.
    /// Doing this multiple times increases the bonus.
    /// </summary>
    public class PerfectScoreObject : GUI_Panel
    {
        /// <summary>
        /// Whether the players are eligible for the bonus.
        /// </summary>
        bool Eligible = false;

        /// <summary>
        /// Whether the bonus has been obtained this level.
        /// </summary>
        bool Obtained
        {
            get { return _Obtained; }
            set
            {
                _Obtained = value;
                if (Global) GlobalObtained = _Obtained;
            }
        }
        bool _Obtained = false;

        public override void OnAdd()
        {
            base.OnAdd();

            // Remove other perfect score objects
            foreach (GameObject obj in MyGame.MyGameObjects)
            {
                if (obj == this) continue;
                if (obj is PerfectScoreObject) obj.Release();
            }

            // If the player didn't get the bonus last level, reset the multiplier
            if (!Obtained)
                ResetMultiplier();
            Obtained = false;

            Eligible = true;

            MyGame.OnCoinGrab += OnCoinGrab;
            MyGame.OnLevelRetry += OnLevelRetry;

            OnAdd_GUI();
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

			Dots = null;

            MyGame.OnCoinGrab -= OnCoinGrab;
            MyGame.OnLevelRetry -= OnLevelRetry;
        }

        /// <summary>
        /// The base value for the perfect bonus.
        /// </summary>
        public int BaseBonus
        {
            get { return _BaseBonus; }
            set
            {
                _BaseBonus = value;
                if (NextBonus < BaseBonus)
                    NextBonus = BaseBonus;
            }
        }
        public int _BaseBonus;

        /// <summary>
        /// How much the value of the bonus increases each time it is obtained.
        /// </summary>
        public int BonusIncrement;

        /// <summary>
        /// The largest the bonus can be
        /// </summary>
        public int MaxBonus;

        /// <summary>
        /// The value the next perfect bonus is worth.
        /// </summary>
        int NextBonus
        {
            get { return _NextBonus; }
            set
            {
                _NextBonus = Math.Min(value, MaxBonus);
                if (Global) GlobalBonus = _NextBonus;
            }
        }
        int _NextBonus;


        /// <summary>
        /// How many times a bonus has been gotten in a row
        /// </summary>
        int BonusCount = 0;

        /// <summary>
        /// Every time a coin is grabbed check to see if it was the last coin on the level.
        /// </summary>
        public void OnCoinGrab(ObjectBase obj)
        {
            if (!Eligible) return;

            // Give the bonus?
            if (PlayerManager.GetLevelCoins() == MyGame.MyLevel.NumCoins)
            {
                Obtained = true;
                BonusCount++;

                PlayerData player = obj.Core.InteractingPlayer;

				if (player != null)
				{
					if (Campaign)
					{
						player.CampaignCoins += BonusValue();
					}
					else
					{
						player.LevelStats.Score += BonusValue();
					}
				}

                // Remove last coin score text
                MyGame.RemoveLastCoinText();

                // Add the visual effect
                Effect(obj.Core.Data.Position);

                // Increase the bonus
                MyGame.WaitThenDo(2, TextEffect);
                //NextBonus += BonusIncrement;
            }

            UpdateScoreText();
        }

        /// <summary>
        /// The value of the current bonus, factoring in score multipliers the current game might have.
        /// </summary>
        int BonusValue()
        {
            return (int)(NextBonus * MyGame.ScoreMultiplier);
        }

        /// <summary>
        /// The effect that is created when a player gets the bonus.
        /// </summary>
        void Effect(Vector2 pos)
        {
            // Berries
            if (BonusCount % 5 == 0)
            {
                Tools.CurGameData.AddGameObject(new Cheer());
            }

            pos += new Vector2(110, 70);

            // Text float
            TextFloat text = new TextFloat(Localization.Words.Perfect, pos + new Vector2(21, 76.5f));
			string s = BonusValue().ToString();
			if (Campaign) s = "+" + s;
            TextFloat text2 = new TextFloat(s, pos + new Vector2(21, -93.5f));

            text.MyText.Scale *= 1.5f;
            MyGame.AddGameObject(text);

            text2.MyText.Scale *= 1.5f;
            MyGame.AddGameObject(text2);

            ParticleEffects.CoinDie_Perfect(MyGame.MyLevel, pos);
            ParticleEffects.CoinDie_Spritely(MyGame.MyLevel, pos);

            //Vector2 TextPos = MyGame.CamPos + MyPile.Pos;
            ////ParticleEffects.CoinDie_Perfect(MyGame.MyLevel, TextPos + new Vector2(-260, 0));
            ////ParticleEffects.CoinDie_Perfect(MyGame.MyLevel, TextPos + new Vector2(60, 0));
            //ParticleEffects.CoinDie_Spritely(MyGame.MyLevel, TextPos + new Vector2(-260, 0));
            //ParticleEffects.CoinDie_Spritely(MyGame.MyLevel, TextPos + new Vector2(-160, 0));
            //ParticleEffects.CoinDie_Spritely(MyGame.MyLevel, TextPos + new Vector2(-60, 0));
            //ParticleEffects.CoinDie_Spritely(MyGame.MyLevel, TextPos + new Vector2(40, 0));

            //MyGame.OnCompleteLevel += new Action<Levels.Level>(MyGame_OnCompleteLevel);
            //MyGame.WaitThenDo(1, TextEffect);
            //TextEffect();

            var sound = Tools.SoundWad.FindByName("PerfectSound");
            if (sound != null) sound.Play();
        }

        void MyGame_OnCompleteLevel(Levels.Level obj)
        {
            TextEffect();
        }

        void TextEffect()
        {
            NextBonus += BonusIncrement;
            UpdateScoreText();

            if (MyGame.ScoreMultiplier <= 2)
            {
                Text.MyFloatColor = .77f * Color.White.ToVector4() + .23f * new Color(228, 0, 69).ToVector4();
                Text.Scale = 1.18f;
            }
            else
            {
                Text.MyFloatColor = .6f * Color.White.ToVector4() + .4f * new Color(228, 0, 69).ToVector4();
                Text.Scale = 1.115f;
            }
        }

        /// <summary>
        /// If true the player can not get the bonus on this level once they have died once.
        /// </summary>
        public bool IneligibleOnDeath = false;

        int Count = 0;
        /// <summary>
        /// When the players die reset the multiplier
        /// </summary>
        public void OnLevelRetry()
        {
            if (Count == 0) return;
            if (MyGame.FreeReset) return;

            if (IneligibleOnDeath)
                Eligible = false;

            ResetMultiplier();
        }

        void ResetMultiplier()
        {
            bool LostStreak = BonusCount > 0;

            NextBonus = BaseBonus;
            BonusCount = 0;

            if (LostStreak)
            {
                Text.MyFloatColor = .8f * Color.Black.ToVector4() + .2f * new Color(228, 0, 69).ToVector4();
                Text.Scale = 1.185f;
                Text.Angle = -1;
            }

            UpdateScoreText();
        }

        public static bool GlobalObtained;
        public static int GlobalBonus;
        public bool Global = false;
        public bool ShowMultiplier = true;
		public bool Campaign;
        public PerfectScoreObject(bool Global, bool ShowMultiplier, bool Campaign)
        {
			this.Campaign = Campaign;

            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;

			if (Campaign)
			{
				BaseBonus = 10;
				BonusIncrement = 10;
				MaxBonus = 10;
			}
			else
			{
				BaseBonus = 1000;
				BonusIncrement = 1000;
				MaxBonus = 8000;
			}

            // Global modifer, keep track of multiplier across levels/games
            if (Global)
            {
                this.Global = true;
                Obtained = GlobalObtained;
                NextBonus = GlobalBonus;
            }

            this.ShowMultiplier = ShowMultiplier;

            Init_GUI();
        }

        public float Multiplier
        {
            get
            {
                if (MyGame == null) return NextBonus / BaseBonus;
                else return MyGame.ScoreMultiplier * NextBonus / BaseBonus;
            }
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            Count++;
        }


        /// <summary>
        /// Gui code.
        /// </summary>
        #region GUI_Code
        /// <summary>
        /// Return a string representation of the score
        /// </summary>
        public override string ToString()
        {
            //return "Streak x" + string.Format("{0:0.0}", Multiplier);
            //return "Perfect +" + ((int)(Multiplier * 1000)).ToString();
            //return "Next Bonus x " + string.Format("{0:0.0}", Multiplier);
            //return "Perfect x " + string.Format("{0:0}", Multiplier);
            //return "Bonus x " + string.Format("{0:0}", Multiplier);
            return "x " + string.Format("{0:0}", Multiplier);
        }

        bool AddedOnce = false;
        void OnAdd_GUI()
        {
            base.OnAdd();

            if (!AddedOnce)
            {
                SlideIn(0);
                Show();
            }

            AddedOnce = true;

            if (MyLevel.NumCoins == 0) Obtained = true;
        }

        EzText Text;
		List<QuadClass> Dots;
		EzTexture Full, Empty;
        void UpdateScoreText()
        {
            float Hold = Multiplier;
            Text.SubstituteText(ToString());

            if (Hold != Multiplier)
                MyPile.BubbleUp(false);

			// Update dots
			int NumDots = NextBonus / BaseBonus;
			Dots[0].Size = new Vector2(19.9642f, 21.49991f) * CoreMath.Periodic(.96f, 1.03f, 1f, Tools.t);
			for (int i = 0; i < 8; i++)
			{
				Dots[i].Quad.MyTexture = i < NumDots ? Full : Empty;
				Dots[i].Pos = Dots[0].Pos + i * new Vector2(34, 0);
				Dots[i].Size = Dots[0].Size;
			}
        }

        void Init_GUI()
        {
            MyPile = new DrawPile();
            EnsureFancy();

            MyPile.Pos = new Vector2(1350, -800);

            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;

            MyPile.FancyPos.UpdateWithGame = true;

            EzFont font;
            float scale;
            Color c, o;

            font = Resources.Font_Grobold42;
            //scale = .666f;
            scale = .5f;
            //c = new Color(228, 0, 69);
            c = new Color(228, 0, 69);
            o = Color.White;

            Text = new EzText(ToString(), font, 950, false, true);
            Text.Name = "Text";
            Text.Scale = scale;
            Text.Pos = new Vector2(0, 0);
            Text.MyFloatColor = c.ToVector4();
            Text.OutlineColor = o.ToVector4();

            Text.RightJustify = true;

            MyPile.Add(Text);

			Full = Tools.Texture("Dot_Full");
			Empty = Tools.Texture("Dot_Empty");
			Dots = new List<QuadClass>();
			for (int i = 0; i < 8; i++)
			{
				var dot = new QuadClass("White");
				dot.Size = new Vector2(13.54166f, 14.58333f);

				MyPile.Add(dot, "Dot" + i.ToString());
				Dots.Add(dot);
			}

			QuadClass _q;
			_q = MyPile.FindQuad("Dot0"); if (_q != null) { _q.Pos = new Vector2(-302.7778f, -91.66668f); _q.Size = new Vector2(19.2969f, 20.78128f); }

            SetPos();
        }

        void SetPos()
        {
			EzText _t;
			_t = MyPile.FindEzText("Text"); if (_t != null) { _t.Pos = new Vector2(0f, 0f); _t.Scale = 0.8000005f; }
			MyPile.Pos = new Vector2(1569.445f, -772.2226f);
		}

        public override void Release()
        {
            base.Release();
        }

        protected override void MyDraw()
        {
			if (Campaign) return;

			//Tools.Warning();
			//UpdateScoreText();

            Text.MyFloatColor += .05f * (new Color(228, 0, 69).ToVector4() - Text.MyFloatColor);
            Text.Scale += .05f * (.8f - Text.Scale);
            Text.Angle += .2f * (0 - Text.Angle);


            //Text.Scale = 1.285f; // Jordan likes
            //Text.Scale = 0.8f; // TJ is a faggot

            if (!Core.Show || Core.MyLevel.SuppressCheckpoints || !ShowMultiplier) return;

            base.MyDraw();
        }
        #endregion
    }
}