using Microsoft.Xna.Framework;



namespace CloudberryKingdom
{
    public interface ILoadingScreen
    {
        void AddHint(string hint, int extra_wait);

        void Start();
        void End();

        void PreDraw();
        void Draw(Camera cam);

        void MakeFake();
    }
}