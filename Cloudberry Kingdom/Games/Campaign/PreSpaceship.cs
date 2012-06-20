using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using Drawing;
namespace CloudberryKingdom
{
    public class PreSpaceship : GUI_Panel
    {
        public PreSpaceship() { }

        public override void  OnAdd()
        {
 	        base.OnAdd();

            // Pause background level
            PauseLevel = true;

            MyPile = new DrawPile();
            EnsureFancy();

            MyPile.FancyPos.UpdateWithGame = true;

            // Hide current song info
            Tools.SongWad.DisplayingInfo = false;

            // Black backdrop
            QuadClass Backdrop = new QuadClass();
            Backdrop.FullScreen(MyGame.Cam); Backdrop.Pos = Vector2.Zero;
            Backdrop.Quad.SetColor(Color.Black);
            MyPile.Add(Backdrop);

            // Centered text
            EzText MyText = new EzText("And now for something\ncompletely different.", Tools.Font_DylanThin42, 1500, true, true);
            CampaignMenu.HappyBlueColor(MyText);
            MyPile.Add(MyText);

            MyText.Alpha = 0;
            MyGame.AddToDo(() => {
                if (MyText.Alpha < 1) { MyText.Alpha += .03125f; return false; }
                else return true;
            });

            // Show for a period and remove
            MyGame.WaitThenDo(153, () => Release());
            MyGame.WaitThenDo(153, () => Tools.SongWad.LoopSong(Tools.Song_BlueChair));
        }

        protected override void MyDraw()
        {
            if (MyGame == null) return;

            //MyPile.FancyPos.SetCenter(MyGame.Cam);
            base.MyDraw();
        }
    }
}