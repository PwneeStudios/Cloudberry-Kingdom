using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

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
            
            MyGame.WaitThenDo(1, new ReadyProxy(this));
        }

        class ReadyProxy : Lambda
        {
            FadeInObject fio;

            public ReadyProxy(FadeInObject fio)
            {
                this.fio = fio;
            }

            public void Apply()
            {
                fio.Ready();
            }
        }

        void Ready()
        {
            MyGame.WaitThenDo(20, new EndProxy(this));
        }

        class EndProxy : Lambda
        {
            FadeInObject fio;

            public EndProxy(FadeInObject fio)
            {
                this.fio = fio;
            }

            public void Apply()
            {
                fio.End();
            }
        }

        void End()
        {
            MyGame.FadeIn(.032f);

            PauseGame = false;

            Release();
        }

        protected override void MyPhsxStep()
        {
        }
    }
}