using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    public class LoadingScreen : ILoadingScreen
    {
        public static int DefaultMinLoadLength = 85;
        public static int MinLoadLength;

        public bool Fake = false;
        public void MakeFake()
		{
			Fake = true;
			FadeAlpha = 1.2f;
            FadeInAlpha = 1.3f;
		}

        QuadClass BackgroundQuad, BlackQuad;
        ObjectClass CenterObject;
        EzText LoadingText, HintText;

        EzText TextObject;
        
        bool Fade;
        float FadeAlpha;

        public float FadeInAlpha = 0;

        public void AddHint(string hint, int extra_wait)
        {
            MinLoading += extra_wait;

            HintText = new EzText(hint, Resources.Font_Grobold42, 10000, true, true);
            HintText.Scale *= .6125f;
            CkColorHelper._x_x_HappyBlueColor(HintText);
            //HintText.OutlineColor = Color.Purple.ToVector4();
            HintText.Pos = new Vector2(0, -250);
        }

        public LoadingScreen()
        {
            BackgroundQuad = new QuadClass();
            BackgroundQuad.SetToDefault();

            BackgroundQuad.Set("LoadingStrip");
            BackgroundQuad.Quad.SetColor(Color.Gray);

            BlackQuad = new QuadClass();
            BlackQuad.SetToDefault();
            BlackQuad.Quad.SetColor(new Color(0, 0, 0, 0));

            LoadingText = new EzText(Localization.WordString(Localization.Words.Loading) + "...", Resources.Font_Grobold42, true, true);
            LoadingText.Scale *= .445f;
            LoadingText.FixedToCamera = true;
            LoadingText._Pos = new Vector2(21, -106);

            if (CenterObject != null)
            {
                CenterObject.Release();
                CenterObject = null;
            }
            TextObject = null;

            BobPhsx type;
			////if (Tools.WorldMap != null)
			//if (Tools.CurGameData == null || Tools.CurGameData.DefaultHeroType == null)
			//    type = Tools.WorldMap.DefaultHeroType;
			//else
			//    type = Tools.CurGameData.DefaultHeroType;
			type = BobPhsxNormal.Instance;

            //if (type is BobPhsxSpaceship)
            //{
            //    TextObject = new EzText("?", Resources.Font_Grobold42, true, true);
            //    CkColorHelper._x_x_HappyBlueColor(TextObject);
            //    TextObject.Scale *= 1.25f;
            //    TextObject.FixedToCamera = true;
            //    TextObject._Pos = new Vector2(11, 170);
            //}
            //else
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

				//if (type is BobPhsxSpaceship)
				//{
				//    //CenterObject.QuadList[1].SetColor(Color.White);
				//    //CenterObject.QuadList[1].MyMatrix = Matrix.Identity;
				//    //CenterObject.QuadList[1].MyEffect = Tools.HslGreenEffect;
				//    //CenterObject.QuadList[0].Show = false;
				//    //CenterObject.QuadList[2].Show = false;

				//    //foreach (BaseQuad quad in CenterObject.QuadList)
				//    //    quad.MyDrawOrder = ObjectDrawOrder.WithOutline;
				//}
            }
        }

        int MinLoading;
		
		int DrawCount = 0;
        //const int DrawCount_Max = 60 * 60 - 300; // 60 seconds, minus fade time and safety margin
        //const int DrawCount_Max = 60 * 11 - 90; // 11 seconds, minus fade time and safety margin. 11 seconds = 30 seconds (total allowed) - 9 seconds (screen saver)
        const int DrawCount_Max = 60 * 7 - 90; // 7 seconds, minus fade time and safety margin. 11 seconds = 30 seconds (total allowed) - 9 seconds (screen saver)


        public void Start()
        {
			DrawCount = 0;

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

            if (Fake && MinLoading <= 0 && (Resources.FakeFinalLoadDone || DrawCount > DrawCount_Max))
                End();

			if (Fake && FadeAlpha > -.1f && !Fade)
			{
				FadeAlpha -= .07f;
			}

			// Once we have been asked to stop showing the loading screen, we must fade out.
            if (Fade && MinLoading <= 0)
            {
				if (!Fake && !Resources.FinalLoadDone && DrawCount < 40 * 60)
				{
					// Do nothing. Keep loading.
				}
				else
				{
					FadeAlpha += .07f;
					if (Fake && FadeAlpha > 1.4f ||
						!Fake && FadeAlpha > 1.2f)
						Tools.ShowLoadingScreen = false;
				}
            }

            if (FadeInAlpha > 0)
            {
                FadeInAlpha -= .1f;
                BlackQuad.Quad.SetColor(new Color(0f, 0f, 0f, FadeInAlpha));
            }
            else
            {
                BlackQuad.Quad.SetColor(new Color(0f, 0f, 0f, FadeAlpha));
            }

            if (CenterObject != null)
            {
                CenterObject.PlayUpdate(CenterObject.LoadingRunSpeed);
                CenterObject.Update(null);
            }
        }

        public void Draw(Camera cam)
        {
			DrawCount++;

            Tools.Device.Clear(Color.Black);

            if (FadeInAlpha >= 1.0) return;

            BackgroundQuad.FullScreen(cam);

            BackgroundQuad.Scale(1.25f);
            BlackQuad.FullScreen(cam);
            BlackQuad.Scale(1.25f);

            if (CenterObject != null) CenterObject.MoveTo(new Vector2(0, -70));
            LoadingText._Pos = new Vector2(18, -140);

            BackgroundQuad.Draw();
            Tools.QDrawer.Flush();
            if (CenterObject != null)
            {
                //CenterObject.ContainedDraw();
                CenterObject.Draw(true);
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