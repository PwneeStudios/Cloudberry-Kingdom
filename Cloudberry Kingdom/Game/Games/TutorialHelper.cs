using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;




namespace CloudberryKingdom
{
    class TutorialHelper
    {
        class TextKillerHelper : Lambda
        {
            GUI_Text text;

            public TextKillerHelper(GUI_Text text)
            {
                this.text = text;
            }

            public void Apply()
            {
                text.Kill(false);
            }
        }

        class ReadyGo_GoProxy : Lambda
        {
            GameData game;
            Lambda End;

            public ReadyGo_GoProxy(GameData game, Lambda End)
            {
                this.game = game;
                this.End = End;
            }

            public void Apply()
            {
                TutorialHelper.ReadyGo_Go(game, End);
            }
        }

        public static Vector2 ReadyGoPos = new Vector2(0, 80);
        public static void ReadyGo(GameData game, Lambda End)
        {
            GUI_Text text = new GUI_Text(Localization.Words.Ready, ReadyGoPos);
            text.FixedToCamera = true;
            game.AddGameObject(text);

            game.WaitThenDo(36, new TextKillerHelper(text));
            game.WaitThenDo(40, new ReadyGo_GoProxy(game, End));
        }

        public static void ReadyGo_Go(GameData game, Lambda End)
        {
            GUI_Text text = new GUI_Text(Localization.Words.Go, ReadyGoPos);
            text.MyPile.MyPopPitch = 3;
            text.FixedToCamera = true;
            game.AddGameObject(text);

            game.WaitThenDo(20, End);
            game.WaitThenDo(30, new TextKillerHelper(text));
        }
    }
}
