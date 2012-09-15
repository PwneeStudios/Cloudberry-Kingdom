using System;
using System.Text;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class GUI_BlobQuota : GUI_Blobs
    {
        public int Quota;

        public GUI_BlobQuota(int Quota)
        {
            this.Quota = Quota;
        }

        protected override int OutOf()
        {
            return Quota;
        }

        public Action<GUI_Blobs> OnQuotaMet;
        public bool QuotaMet = false;

        Door FinalDoor;
        public override void OnAdd()
        {
            base.OnAdd();

            //FinalDoor = MyGame.MyLevel.FinalDoor;
            //FinalDoor.SetLock(true, true, false);
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (Blobs >= Quota)
            {
                Blobs = Quota;
                UpdateCoinText();

                if (!QuotaMet)
                {
                    // Action
                    if (OnQuotaMet != null) OnQuotaMet(this);

                    // Emphasize
                    MyPile.BubbleUp(true);

                    // Hide
                    MyGame.WaitThenDo(28, () => SlideOut(PresetPos.Top, 26), "", true, true);

                    //FinalDoor.SetLock(false);

                    QuotaMet = true;
                }
            }
            else
            {
                //FinalDoor.SetLock(true, true, false);

                QuotaMet = false;
            }
        }
    }

    public class GUI_Blobs : GUI_Panel
    {
        StringBuilder MyString = new StringBuilder(50, 50);

        public int Blobs, TotalBlobs = 0;

        /// <summary>
        /// Return a string representation of the number of Blobs
        /// </summary>
        /// <returns></returns>
        public StringBuilder BuildString()
        {
            MyString.Length = 0;

            MyString.Add(Blobs, 1);
            MyString.Append('/');
            MyString.Add(OutOf(), 1);

            return MyString;
        }

        protected virtual int OutOf() { return TotalBlobs; }

        bool AddedOnce = false;
        public override void OnAdd()
        {
            base.OnAdd();

            if (!AddedOnce)
            {
                Hide();
                
                Show();
            }

            AddedOnce = true;

            if (TotalBlobs == 0)
                TotalBlobs = MyGame.MyLevel.NumBlobs;
            Blobs = 0;
        }

        public override void Hide()
        {
            base.Hide();
            SlideOut(PresetPos.Top, 0);
        }

        public override void Show()
        {
            base.Show();
            //SlideIn();
            //MyPile.BubbleUp(false, 10, .7f);
            SlideIn(50);
        }

        public Vector2 ApparentPos
        {
            get { return Text.FancyPos.AbsVal + Text.GetWorldSize() / 2; }
        }

        QuadClass Blob;
        EzText Text;
        protected void UpdateCoinText()
        {
            Text.SubstituteText(BuildString());
        }
        
        public GUI_Blobs() { }

        public override void Init()
        {
            base.Init();

            MyPile = new DrawPile();
            EnsureFancy();

            Vector2 shift = new Vector2(-320, 0);
            MyPile.Pos = new Vector2(836.1112f, 806.6667f) - shift;
            SlideInLength = 0;

            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;

            MyPile.FancyPos.UpdateWithGame = true;

            QuadClass cloud = new QuadClass("Cloud1", 150, true);
            cloud.Pos = new Vector2(193.0659f, -22.74048f);
            cloud.Size = new Vector2(465.5865f, 259.2372f);
            MyPile.Add(cloud);

            Text = new EzText(BuildString().ToString(), Tools.Font_Grobold42_2, 450, false, false);
            Text.Scale = .55f;
            Text.Pos = new Vector2(0.3707275f, 73.3901f);
            Text.MyFloatColor = new Color(255, 255, 255).ToVector4();
            Text.OutlineColor = new Color(0, 0, 0).ToVector4();
            MyPile.Add(Text);

            Blob = new QuadClass("Score\\Blob", 150, true);
            Blob.Pos = new Vector2(-26.84131f, 11.98175f);
            Blob.Size = new Vector2(122.2223f, 193.6508f);
            Blob.ScaleXToMatchRatio();
            //Blob.Angle = 0.1055556f;
            MyPile.Add(Blob);
        }

        protected override void MyDraw()
        {
            if (!Core.Show || Core.MyLevel.SuppressCheckpoints) return;

            base.MyDraw();
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (MyGame.SoftPause) return;
            if (Hid || !Active) return;

            if (Core.MyLevel.Watching || Core.MyLevel.Finished) return;

            Blobs = PlayerManager.PlayerSum(p => p.TempStats.Blobs);

            //CoinText.Pos = new Vector2(CoinText.GetWorldWidth(), CoinText.Pos.Y);
            UpdateCoinText();
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);

            //SlideOut(PresetPos.Top, 0);
            SlideIn(30);
        }
    }
}