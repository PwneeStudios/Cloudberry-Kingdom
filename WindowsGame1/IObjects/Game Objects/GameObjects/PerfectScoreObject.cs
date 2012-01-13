using Microsoft.Xna.Framework;
using System;

namespace CloudberryKingdom
{
    /// <summary>
    /// This object tracks a player's action.
    /// When a level is finished without dying and with every coin grabbed, a bonus is given.
    /// Doing this multiple times increases the bonus.
    /// </summary>
    public class PerfectScoreObject : GameObject
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
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

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
        public void OnCoinGrab(IObject obj)
        {
            if (!Eligible) return;

            // Give the bonus?
            if (PlayerManager.GetLevelCoins() == MyGame.MyLevel.NumCoins)
            {
                Obtained = true;
                BonusCount++;

                PlayerData player = obj.Core.InteractingPlayer;

                if (player != null)
                    player.LevelStats.Score += BonusValue();

                // Remove last coin score text
                foreach (GameObject gameobj in MyGame.MyGameObjects)
                    if (gameobj.Core.AddedTimeStamp == MyGame.MyLevel.CurPhsxStep)
                        gameobj.Release();

                // Add the visual effect
                Effect(obj.Core.Data.Position);

                // Increase the bonus
                NextBonus += BonusIncrement;
            }
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
            TextFloat text = new TextFloat("perfect!\n+" + BonusValue().ToString(), pos + new Vector2(21, 22.5f));
            text.MyText.MyFloatColor = MyGame.MyLevel.MyTileSetInfo.CoinScoreColor.ToVector4();
            text.MyText.Scale *= 1.3f;
            MyGame.AddGameObject(text);

            //ParticleEffects.CoinExplosion(MyGame.MyLevel, pos);
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
            NextBonus = BaseBonus;
            BonusCount = 0;
        }

        public static bool GlobalObtained;
        public static int GlobalBonus;
        public bool Global = false;
        public PerfectScoreObject(bool Global)
        {
            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;

            BaseBonus = 1000;
            BonusIncrement = 1000;
            MaxBonus = 8000;

            // Global modifer, keep track of multiplier across levels/games
            if (Global)
            {
                this.Global = true;
                Obtained = GlobalObtained;
                NextBonus = GlobalBonus;
            }
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            Count++;
        }
    }
}