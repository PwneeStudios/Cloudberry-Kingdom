using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Drawing;

using WindowsGame1.Levels;
using WindowsGame1.Bobs;

namespace WindowsGame1
{
    public enum StarMode { Draw, Text, Red, None, Token };
    public class Stars
    {
        public static EzTexture WhiteStar, RegularStar;
        static bool Initialized = false;

        public int NumStars;
        public QuadClass MyStar;
        public EzText StarText;

        public StarMode MyMode = StarMode.None;

        public Vector2 Pos;

        public Stars()
        {
            if (!Initialized)
            {
                WhiteStar = Tools.TextureWad.FindByName("star_white");
                RegularStar = Tools.TextureWad.FindByName("star");
                Initialized = true;
            }
                        
            MyStar = new QuadClass();
        }

        public void SetStars(int Num)
        {
            NumStars = Num;

            if (MyMode == StarMode.Draw)
            {
                MyStar.Shadow = true;
                MyStar.ShadowColor = new Color(50, 50, 50);
                MyStar.ShadowOffset = new Vector2(7, 7);
            }

            if (MyMode == StarMode.Text || MyMode == StarMode.Red)
            {
                if (MyMode == StarMode.Text)
                {
                    //StarText = new EzText(NumStars.ToString() + " / 85", Tools.MenuBoldPlus, true, true);
                    //StarText = new EzText("12\n    /\n        85", Tools.MenuBoldPlus, true, true);
                    string str = NumStars.ToString() + "\n    /\n        " + "85";
                    if (NumStars < 10) str = " " + str;
                    StarText = new EzText(str, Tools.Font_Dylan28, 1000, true, true, .48f);


                    StarText.MyFloatColor = Color.HotPink.ToVector4();// new Vector4(0.75f, .45f, 0.3f, 1);

                    StarText.Shadow = true;
                    StarText.ShadowColor = new Color(15, 15, 15);
                    StarText.ShadowOffset = new Vector2(11f, 11f);
                }
                else
                {
                    StarText = new EzText("x" + NumStars.ToString(), Tools.Font_Dylan24, false, true);
                    StarText.Shadow = true;
                    StarText.ShadowColor = new Color(25, 25, 25);
                    StarText.ShadowOffset = new Vector2(7.5f, 7.5f);
                }

                MyStar.Shadow = true;
                MyStar.ShadowColor = new Color(50, 50, 50);
                MyStar.ShadowOffset = 1.375f * StarText.ShadowOffset;

                if (MyMode == StarMode.Red)
                    StarText.MyFloatColor = Color.Gold.ToVector4();// new Color(225, 85, 150).ToVector4();// new Vector4(0f, .7f, 0f, 1);
            }

            if (MyMode == StarMode.Token)
            {
                StarText = new EzText("{pBubble3,70,70}x" + NumStars.ToString(), Tools.Font_Dylan24, true, true);
                StarText.Shadow = true;
                StarText.PicShadow = false;
                StarText.ShadowColor = new Color(25, 25, 25);
                StarText.ShadowOffset = new Vector2(7.5f, 7.5f);

                StarText.MyFloatColor = new Color(230, 65, 63).ToVector4();// new Color(225, 85, 150).ToVector4();// new Vector4(0f, .7f, 0f, 1);
            }
        }

        public void MakeNew()
        {
            MyStar.Quad.Init();
            MyStar.Quad.MyEffect = Tools.EffectWad.FindByName("Basic");
            MyStar.Quad.MyTexture = Tools.TextureWad.FindByName("star");

            MyStar.Quad.EnforceTextureRatio();
            MyStar.Base.e1 = new Vector2(57, 0);
            MyStar.Base.e2 = new Vector2(0, 57);

            SetMode(MyMode);
        }

        public void SetMode(StarMode NewMode)
        {
            MyMode = NewMode;

            switch (MyMode)
            {
                case StarMode.Token:
                    break;

                case StarMode.Draw:
                    MyStar.Quad.MyTexture = RegularStar;
                    MyStar.Base.e1 = new Vector2(57, 0);
                    MyStar.Base.e2 = new Vector2(0, 57);
                    break;

                case StarMode.Red:
                    MyStar.Base.e1 = new Vector2(70, 0);
                    MyStar.Base.e2 = new Vector2(0, 70);
                    break;

                case StarMode.Text:
                    MyStar.Base.e1 = new Vector2(140, 0);
                    MyStar.Base.e2 = new Vector2(0, 140);
                    break;
            }
        }

        public void Draw(bool Text)
        {
            switch (MyMode)
            {
                case StarMode.None:
                    return;

                case StarMode.Token:
                    Draw_Token(Text);
                    break;

                case StarMode.Draw:
                    Draw_Draw(Text);
                    break;

                case StarMode.Red:
                case StarMode.Text:
                    Draw_Text(Text);
                    break;
            }
        }

        public void Draw_Token(bool Text)
        {
            if (Text)
            {
                StarText._Pos = Pos + new Vector2(0, 234);
                StarText.Draw(Tools.CurLevel.MainCamera, false);
            }
        }

        public void Draw_Text(bool Text)
        {
            /*
            MyStar.Quad.MyTexture = WhiteStar;
            MyStar.Quad.SetColor(new Color(50,50,50));
            MyStar.Base.Origin = Pos + new Vector2(-52, 234 + 15) - 1.375f * StarText.ShadowOffset;
            MyStar.Draw();

            MyStar.Quad.MyTexture = RegularStar;
            MyStar.Quad.SetColor(Color.White);*/

            if (!Text)
            {
                if (MyMode == StarMode.Text)
                    MyStar.Base.Origin = Pos + new Vector2(0, 234 + 135);
                else
                    MyStar.Base.Origin = Pos + new Vector2(-52, 234 + 40);
                MyStar.Draw();
            }
            else
            {
                if (MyMode == StarMode.Text)
                {
                    StarText._Pos = Pos + new Vector2(-93.5f, 234 + 162.5f);
                    StarText.Draw(Tools.CurLevel.MainCamera, false);
                }
                else
                {
                    StarText._Pos = Pos + new Vector2(0, 234 + 25);
                    StarText.Draw(Tools.CurLevel.MainCamera, false);
                }
            }
        }

        public void Draw_Draw(bool Text)
        {
            if (Text) return;

            float r = 234;
            float step = 32 - 4 * NumStars; //24
            float ang_range = (NumStars - 1) * step * 2f * (float)(Math.PI) / 360f;
            float ang_step = 0;
            if (NumStars > 1) ang_step = ang_range / (NumStars - 1);
            for (int i = 0; i < NumStars; i++)
            {
                float ang = (float)(Math.PI / 2 + i * ang_step - ang_range / 2);
                MyStar.Base.Origin = Pos + r * new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang));

                MyStar.Draw();
            }
        }
    }
}