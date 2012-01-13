using System;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class ChallengeMenuItem : MenuItem
    {
        public Challenge MyChallenge;

        public int Stars = 0;
        public static int MaxStars = 5;

        static QuadClass StarEmpty, StarFull;
        static float StarSize = 93;
        static float Spacing = StarSize;

        public float RightEdge = 0, OffsetY = -105;

        public ChallengeMenuItem() { }

        public void CalcStars()
        {
            Stars = 0;
            if (MyChallenge.ID != Guid.Empty)
                foreach (PlayerData data in PlayerManager.Players)
                    Stars = Math.Max(Stars, data.ChallengeStars[MyChallenge.ID]);
        }

        static void InitStar()
        {
            if (StarEmpty != null) return;

            // Empty star
            StarEmpty = new QuadClass();
            StarEmpty.SetToDefault();

            StarEmpty.TextureName = "star_empty";

            StarEmpty.ScaleXToMatchRatio();
            StarEmpty.Scale(StarSize);

            // Full star
            StarFull = new QuadClass();
            StarEmpty.Clone(StarFull);

            StarFull.TextureName = "star";
        }

        public ChallengeMenuItem(EzText Text)
        {
            Init(Text, Text.Clone());
        }
        public ChallengeMenuItem(EzText Text, EzText SelectedText)
        {
            Init(Text, SelectedText);
        }

        protected override void Init(EzText Text, EzText SelectedText)
        {
            base.Init(Text, SelectedText);

            InitStar();
        }

        public override void Draw(bool Text, Camera cam, bool Selected)
        {
            base.Draw(Text, cam, Selected);

            if (Text || MyDrawLayer != MyMenu.CurDrawLayer || !OnScreen)
                return;

            for (int i = 0; i < MaxStars; i++)
            {
                QuadClass star = MaxStars - i <= Stars ? StarFull : StarEmpty;

                star.Pos = new Vector2(RightEdge - i * Spacing + PosOffset.X, MyText.Pos.Y + OffsetY - 7f * i);
                star.Draw();
            }
        }
    }
}