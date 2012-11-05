using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    class TutorialHelper
    {
        public static Vector2 ReadyGoPos = new Vector2(0, 80);
        public static void ReadyGo(GameData game, Action End)
        {
            GUI_Text text = new GUI_Text(Localization.Words.Ready, ReadyGoPos);
            text.FixedToCamera = true;
            game.AddGameObject(text);

            game.WaitThenDo(36, () => text.Kill(false));
            game.WaitThenDo(40, () => ReadyGo_Go(game, End));
        }

        public static void ReadyGo_Go(GameData game, Action End)
        {
            GUI_Text text = new GUI_Text(Localization.Words.Go, ReadyGoPos);
            text.MyPile.MyPopPitch = 3;
            text.FixedToCamera = true;
            game.AddGameObject(text);

            game.WaitThenDo(20, () => End());
            game.WaitThenDo(30, () => text.Kill(false));
        }
    }
}
