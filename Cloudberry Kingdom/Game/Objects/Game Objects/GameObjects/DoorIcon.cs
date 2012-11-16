using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace CloudberryKingdom
{
    public enum IconType { Number, Boss, Obstacle, Hero, Place, Bungee };
    public class DoorIcon : GUI_Panel
    {
        int Level;
        public DoorIcon(int Level)
        {
            this.Level = Level;
            Init(Level.ToString());
        }

        public DoorIcon(BobPhsx PhsxType, Vector2 pos, float Scale)
        {
            Init("");
            SetIcon(PhsxType);
            Pos.RelVal = pos;
            icon.SetScale(Scale);

            Core.DrawLayer = 2;
        }

        public string s;
        public Upgrade upgrade;
        public BobPhsx hero;
        ObjectIcon icon;
        public void SetIcon(BobPhsx hero) { this.hero = hero; SetIcon(IconType.Hero); }
        public void SetIcon(Upgrade upgrade) { this.upgrade = upgrade; SetIcon(IconType.Obstacle); }
        public void SetIcon(IconType type)
        {
            MyPile.Clear();
            QuadClass Backdrop = null;

            switch (type) {
                case IconType.Obstacle:
                    icon = ObjectIcon.CreateIcon(upgrade);
                    break;

                case IconType.Hero:
                    icon = hero.Icon.Clone(ObjectIcon.IconScale.NearlyFull);
                    break;
            }

            if (Backdrop != null)
                MyPile.Add(Backdrop);
        }

        public void Init(string text)
        {
            FixedToCamera = false;
            Core.DrawLayer = 9;

            s = text;
            MyPile = new DrawPile();

            SetIcon(IconType.Number);

            //MyPile.BubbleUp(true);
            Active = true;
            Hid = false;
        }

        public void Kill() { Kill(true); }
        public void Kill(bool sound)
        {
            MyPile.BubbleDownAndFade(sound);
            ReleaseWhenDoneScaling = true;
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;
        }

        protected override void MyDraw()
        {
            //if (Level == 201)
            //    Tools.Write("");

            if (Hid) return;

            base.MyDraw();
            if (!IsOnScreen) return;

            if (icon != null)
            {
                // Flip if level is flipped
                if (Core.MyLevel != null && Core.MyLevel.ModZoom.X < 0)
                    icon.Flipped = true;

                icon.Pos = MyPile.FancyPos.Update();
                icon.Draw(false);
            }
        }
    }
}