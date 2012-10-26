using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public enum IconType { Number, Boss, Obstacle, Hero, Place, Bungee };
    public class DoorIcon : GUI_Panel
    {
        public EzText MyText;

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

            float NormalSize = 110;// 90;
            switch (type) {
                case IconType.Number:
                    MyText = MakeText(s);
                    MyText.ZoomWithCam = true;
                    MyText.MyFloatColor = Color.CadetBlue.ToVector4();
                    MyText.OutlineColor = Color.AliceBlue.ToVector4();
                    MyPile.Add(MyText);
                    MyText.Pos = new Vector2(0, .5f * MyText.GetWorldHeight() - 24);

                    if (s.Length == 1) MyText.Y -= 4;
                    if (s.CompareTo("7") == 0) MyText.Pos += new Vector2(8, -12);

                    Backdrop = new QuadClass("levelicon", NormalSize, true);
                    break;

                case IconType.Boss:
                    Backdrop = new QuadClass("levelicon_boss", NormalSize, true);
                    break;

                case IconType.Bungee:
                    Backdrop = new QuadClass("levelicon_bungee", NormalSize, true);
                    break;

                case IconType.Place:
                    Backdrop = new QuadClass("levelicon_buildghost", NormalSize, true);
                    break;

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

        protected virtual EzText MakeText(string text)
        {
            float scale = .72f;
            if (text.Length == 2) scale = .55f;
            
            EzText eztext = new EzText(text, Resources.Font_Grobold42, 1000, true, false);
            eztext.Scale = scale;

            return eztext;
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