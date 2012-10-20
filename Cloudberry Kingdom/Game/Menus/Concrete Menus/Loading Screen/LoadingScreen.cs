using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    public interface ILoadingScreen
    {
        void AddHint(string hint, int extra_wait);

        void Start();
        void End();

        void PreDraw();
        void Draw(Camera cam);

        void MakeFake();
    }
 
    public class LoadingScreen : ILoadingScreen
    {
        public static int DefaultMinLoadLength = 85;
        public static int MinLoadLength;

        public bool Fake = false;
        public void MakeFake() { Fake = true; }

        QuadClass BackgroundQuad, BlackQuad;
        ObjectClass CenterObject;
        EzText LoadingText, HintText;

        EzText TextObject;

        bool Fade;
        float FadeAlpha;

        public void AddHint(string hint, int extra_wait)
        {
            MinLoading += extra_wait;

            HintText = new EzText(hint, Tools.Font_Grobold42, 10000, true, true);
            HintText.Scale *= .6125f;
            CkColorHelper._x_x_HappyBlueColor(HintText);
            //HintText.OutlineColor = Color.Purple.ToVector4();
            HintText.Pos = new Vector2(0, -250);
        }

        public LoadingScreen()
        {
            BackgroundQuad = new QuadClass();
            BackgroundQuad.SetToDefault();
            BackgroundQuad.Quad.SetColor(Color.Black);

            BlackQuad = new QuadClass();
            BlackQuad.SetToDefault();
            BlackQuad.Quad.SetColor(new Color(0, 0, 0, 0));
             
            LoadingText = new EzText("Loading...", Tools.Font_Grobold42, true, true);
            LoadingText.FixedToCamera = true;
            LoadingText._Pos = new Vector2(21, -56);

            if (CenterObject != null)
            {
                CenterObject.Release();
                CenterObject = null;
            }
            TextObject = null;

            BobPhsx type;
            if (Tools.WorldMap != null)
                type = Tools.WorldMap.DefaultHeroType;
            else
                type = Tools.CurGameData.DefaultHeroType;
             
            if (type is BobPhsxSpaceship)
            {
                TextObject = new EzText("?", Tools.Font_Grobold42, true, true);
                CkColorHelper._x_x_HappyBlueColor(TextObject);
                TextObject.Scale *= 1.25f;
                TextObject.FixedToCamera = true;
                TextObject._Pos = new Vector2(11, 170);
            }
            else
            {
                CenterObject = new ObjectClass(type.Prototype.PlayerObject, false, false);

                Vector2 size = CenterObject.BoxList[0].Size();
                float ratio = size.Y / size.X;
                int width = Tools.TheGame.Resolution.Bob.X;
                CenterObject.FinishLoading(Tools.QDrawer, Tools.Device, Tools.TextureWad, Tools.EffectWad, Tools.Device.PresentationParameters, width, (int)(width * ratio));//Tools.Device.PresentationParameters.BackBufferWidth / 4, Tools.Device.PresentationParameters.BackBufferHeight / 4);

                //CenterObject.FinishLoading(Tools.QDrawer, Tools.Device, Tools.TextureWad, Tools.EffectWad, Tools.Device.PresentationParameters, 400, 550);
                CenterObject.ParentQuad.Scale(new Vector2(1.35f, 1.35f));
                CenterObject.Read(1, 0);
                CenterObject.EnqueueAnimation(1, 0, true);
                //CenterObject.ParentQuad.Center.Move(new Vector2(0, -126));

                if (type == BobPhsxBox.Instance)
                {
                    foreach (BaseQuad quad in CenterObject.QuadList)
                        if (quad.MyDrawOrder == ObjectDrawOrder.WithOutline)
                            quad.Show = false;
                }

                if (type is BobPhsxSpaceship)
                {
                    foreach (BaseQuad quad in CenterObject.QuadList)
                        quad.MyDrawOrder = ObjectDrawOrder.WithOutline;
                }
            }
        }

        int MinLoading;
        public void Start()
        {
            MinLoadLength = 10000;

            MinLoading = MinLoadLength;
            MinLoadLength = DefaultMinLoadLength;

            Fade = false;
            FadeAlpha = -.1f;
        }

        public void End()
        {
            Fade = true;
        }

        public void PreDraw()
        {
            MinLoading--;

            if (Fake && MinLoading == 0)
                End();

            if (Fade && MinLoading <= 0)
            {
                FadeAlpha += .07f;
                if (FadeAlpha > 1.2f)
                    Tools.ShowLoadingScreen = false;
            }
            BlackQuad.Quad.SetColor(new Color(0f, 0f, 0f, FadeAlpha));

            if (CenterObject != null)
            {
                CenterObject.PlayUpdate(.135f);
                
                //CenterObject.PlayUpdate(.85f);
                CenterObject.Update(null);

                CenterObject.PreDraw(Tools.Device, Tools.EffectWad);
                Tools.QDrawer.Flush();
            }
        }

        public void Draw(Camera cam)
        {
            Tools.Device.Clear(Color.Black);
            BackgroundQuad.FullScreen(cam);
            BackgroundQuad.Scale(1.25f);
            BlackQuad.FullScreen(cam);
            BlackQuad.Scale(1.25f);

            BackgroundQuad.Draw();
            Tools.QDrawer.Flush();
            if (CenterObject != null)
            {
                CenterObject.ContainedDraw();
                Tools.QDrawer.Flush();
            }
            LoadingText.Draw(cam);
            if (HintText != null) HintText.Draw(cam);
            if (TextObject != null) TextObject.Draw(cam);
            Tools.Render.EndSpriteBatch();
            BlackQuad.Draw();
            Tools.QDrawer.Flush();
        }
    }
}