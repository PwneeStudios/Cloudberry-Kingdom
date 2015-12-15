using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class CharSelectBackdrop : CkBaseMenu
    {
        public CharSelectBackdrop()
            : base()
        {
        }

#if PC
		ClickableBack Back;
		public bool BackClicked = false;
#endif

        public override void SlideIn(int Frames)
        {
            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            if (Frames != 0) Frames = 20;
            base.SlideOut(PresetPos.Right, Frames);
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

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;

            MyPile = new DrawPile();
            EnsureFancy();

            //var Backdrop = new QuadClass("CharSelect", 1778);
            var Backdrop = new QuadClass("CharSelectPanels", 1778);
            MyPile.Add(Backdrop, "Backdrop");

#if PC
			Back = new ClickableBack(MyPile, false, true);

			QuadClass _q;
			_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(1500.002f, -778.8883f); _q.Size = new Vector2(56.24945f, 56.24945f); }
			_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }
#endif

            SetPos();
        }

        void SetPos()
        {
        }

        protected override void MyDraw()
        {
            if (CharacterSelectManager.FakeHide)
                return;

            base.MyDraw();
        }

		protected override void MyPhsxStep()
		{
			base.MyPhsxStep();

			if (!Active) return;

			// Update the back button and the scroll bar
			if (Back.UpdateBack(MyCameraZoom))
			{
				BackClicked = true;
			}

			if (CharacterSelectManager.AllExited())
			{
				Back.Show();
			}
			else
			{
				Back.Hide();
			}
		}
    }
}