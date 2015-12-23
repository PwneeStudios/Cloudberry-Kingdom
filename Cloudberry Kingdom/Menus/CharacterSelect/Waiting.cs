
using CoreEngine;

namespace CloudberryKingdom
{
    public class Waiting : CkBaseMenu
    {
        CharacterSelect MyCharacterSelect;
        bool CanGoBack;
        public Waiting(int Control, CharacterSelect MyCharacterSelect, bool CanGoBack)
            : base(false)
        {
            this.Tags += Tag.CharSelect;
            this.Control = Control;
            this.MyCharacterSelect = MyCharacterSelect;
            this.CanGoBack = CanGoBack;

            Constructor();
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            MyCharacterSelect = null;
        }

        public override void Init()
        {
            base.Init();

            SlideInLength = 0;
            SlideOutLength = 0;
            CallDelay = 0;
            ReturnToCallerDelay = 0;

            if (MyCharacterSelect.QuickJoin)
                MyCharacterSelect.Join = true;

            MyPile = new DrawPile();
            EnsureFancy();

            CharacterSelect.Shift(this);
        }

        protected override void MyDraw()
        {
            if (CharacterSelectManager.FakeHide)
                return;

            base.MyDraw();
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();

			if (Active && !MyCharacterSelect.Player.Exists) { ReturnToCaller(false); return; }
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

			if (Active && !MyCharacterSelect.Player.Exists) { ReturnToCaller(false); return; }

            if (!Active) return;
            MyCharacterSelect.MyState = CharacterSelect.SelectState.Waiting;
            MyCharacterSelect.MyDoll.ShowBob = true;
            MyCharacterSelect.MyGamerTag.ShowGamerTag = true;
            MyCharacterSelect.MyHeroLevel.ShowHeroLevel = true;

            // Check for back.
            if (CanGoBack && CharacterSelectManager.Active && ButtonCheck.State(ControllerButtons.B, MyCharacterSelect.PlayerIndex).Pressed)
            {
                ReturnToCaller();
            }
        }
    }
}