using Microsoft.Xna.Framework;

using Drawing;

namespace CloudberryKingdom
{
    public class ScoreText
    {
        FancyVector2 Center;
        public Vector2 Pos
        {
            get { return Center.RelVal; }
            set { Center.RelVal = value; }
        }

        QuadClass Image;
        EzText Text;

        public int Num, OutOf, Target;
        public bool ShowOutOf;

        public ScoreText()
        {
        }

        public enum Units { Coins, Blobs, Attempts };
        string[] UnitTextureNames = { "CoinBlue", "Score\\Blob", "Score\\Stickman" };

        Vector2[] ImageOffset = { new Vector2(0, 0), new Vector2(0, 78), new Vector2(0, 0) };
        Vector2[] TextOffset = { new Vector2(112, 0), new Vector2(65, 0), new Vector2(158, 0) };

        //Vector2[] NumTypePos = { new Vector2(-659, 322), new Vector2(305, 400 - 200), new Vector2(-350, -100) };
        //Vector2[] NumPos = { new Vector2(-547, 322), new Vector2(370, 322 - 200), new Vector2(-192, -100) };

        Units MyUnits;
        public void Init(Units units, FancyVector2 center)
        {
            MyUnits = units;

            Center = new FancyVector2(center);

            if (Image != null) Image.Release();
            Image = new QuadClass(Center);
            Image.Quad.MyTexture = Tools.TextureWad.FindByName(UnitTextureNames[(int)MyUnits]);
            Image.Pos = ImageOffset[(int)MyUnits];

            UpdateText();
            Text.Pos = TextOffset[(int)MyUnits];

            switch (MyUnits)
            {
                case Units.Coins:
                    Image.ScaleToTextureSize(); Image.Scale(.9f);
                    break;

                case Units.Blobs:
                    Image.ScaleToTextureSize(); Image.Scale(.9f);
                    break;

                case Units.Attempts:
                    Image.ScaleToTextureSize(); Image.Scale(.9f);
                    break;
            }
        }

        public void SetVal(int Num) { SetVal(Num, -1); }
        public void SetVal(int Num, int OutOf)
        {
            this.Num = Num;

            if (OutOf >= 0)
            {
                this.OutOf = OutOf;
                ShowOutOf = true;
            }

            UpdateText();
        }

        void UpdateText()
        {
            Vector2 HoldPos = Vector2.Zero;
            if (Text != null) { HoldPos = Text.Pos; Text.Release(); }

            if (ShowOutOf)
                Text = new EzText("x " + Num.ToString() + " / " + OutOf.ToString(), Tools.Font_DylanThick28, false, true);
            else
                Text = new EzText("x " + Num.ToString(), Tools.Font_DylanThick28, false, true);

            Text.FancyPos = new FancyVector2(Center);
            Text.Pos = HoldPos;
        }

        public void DrawImage()
        {
            Center.Update();

            Image.Draw();
        }

        public void DrawText()
        {
            Center.Update();

            Text.Draw(Tools.CurLevel.MainCamera);
        }
    }
}