using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom
{
    public class FadeInObject : GameObject
    {
        public FadeInObject()
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();

            PauseGame = true;

            MyGame.Black();

            // Find the initial door
            Door door = MyGame.MyLevel.FindIObject(LevelConnector.StartOfLevelCode) as Door;
            if (null != door)
            {
                foreach (Bob bob in MyGame.MyLevel.Bobs)
                    bob.Core.Show = false;
            }
            
            MyGame.WaitThenDo(1, Ready);
        }

        void Ready()
        {
            MyGame.WaitThenDo(20, End);
        }

        void End()
        {
            //MyGame.FadeIn(.032f);
            MyGame.FadeIn(MyLevel.MyLevelSeed == null ? .032f : MyLevel.MyLevelSeed.FadeInSpeed);

            PauseGame = false;

            Release();
        }

        protected override void MyPhsxStep()
        {
        }
    }
}