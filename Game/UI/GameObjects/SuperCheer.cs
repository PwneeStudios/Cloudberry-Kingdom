using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class SuperCheer : GameObject
    {
        int Rows;
        public SuperCheer(int Rows)
        {
            this.Rows = Rows;
        }

        public override void OnAdd()
        {
            base.OnAdd();

            for (int i = 0; i < Rows; i++)
                AddWave(24 * i);

            MyGame.Recycle.CollectObject(this);
        }

        void AddWave(int Delay)
        {
            Cheer cheer;

            float Spread = 3400;
            int Num = 13;
            float Step = Spread / (Num - 1);
            float Taper = 340;
            for (int i = 0; i < Num; i++)
            {
                float x = Step * i - Spread / 2;
                float y = -Taper * 4 * x * x / (Spread * Spread);
                Vector2 pos = new Vector2(x, y - 130);

                GameData mygame = MyGame;
                MyGame.WaitThenDo(i * 6 + Delay, () =>
                {
                    cheer = new Cheer();
                    cheer.Berry.Pos += pos;
                    mygame.AddGameObject(cheer);
                });
            }
        }
    }
}