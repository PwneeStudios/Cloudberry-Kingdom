using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Waiting : StartMenuBase
    {
        CharacterSelect MyCharacterSelect;
        public Waiting(int Control, CharacterSelect MyCharacterSelect)
            : base(false)
        {
            this.Tags += Tag.CharSelect;
            this.Control = Control;
            this.MyCharacterSelect = MyCharacterSelect;

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

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;
            MyCharacterSelect.MyState = CharacterSelect.SelectState.Waiting;
            MyCharacterSelect.MyDoll.ShowBob = true;
            MyCharacterSelect.MyGamerTag.ShowGamerTag = true;
            MyCharacterSelect.MyHeroLevel.ShowHeroLevel = true;

            // Check for back.
            if (ButtonCheck.State(ControllerButtons.B, MyCharacterSelect.PlayerIndex).Pressed)
            {
                ReturnToCaller();
            }
        }
    }
}