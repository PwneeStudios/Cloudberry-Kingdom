using System;
using Microsoft.Xna.Framework;

using Drawing;

namespace CloudberryKingdom.Levels
{
    public class ClosingCircle
    {
        QuadClass Circle;
        Camera MyCamera;
        
        IPos CenterObj;
        Vector2 CenterPos;

        float Speed;

        public int FinishedCount = 0;

        public ClosingCircle(Camera camera, float Frames)
        {
            Init(camera, Frames);
        }
        public ClosingCircle(Camera camera, float Frames, IPos Center)
        {
            Init(camera, Frames);

            CenterObj = Center;
        }
        public ClosingCircle(Camera camera, float Frames, Vector2 Center)
        {
            Init(camera, Frames);

            CenterPos = Center;
        }

        void Init(Camera camera, float Frames)
        {
            MyCamera = camera;

            Circle = new QuadClass();
            Circle.Quad.MyEffect = Tools.EffectWad.FindByName("Circle");
            Circle.Quad.MyTexture = Tools.TextureWad.FindByName("White");

            //Circle.TextureName = "Star";
            //Circle.TextureName = "SpikeBlob";

            Circle.FullScreen(MyCamera);
            Circle.Scale(1.3f * (float)Math.Pow(2f, .5f));
            Circle.Base.e2.Y = Circle.Base.e1.X;

            Speed = Circle.Base.e1.X / Frames;
        }

        float angle = 0;
        public void UpdateCircle()
        {
            if (CenterObj == null)
                Circle.Base.Origin = CenterPos;
            else
                Circle.Base.Origin = CenterObj.Pos;

            if (!Tools.StepControl)
            {
                Circle.Base.e1.X -= Speed;
                Circle.Base.e2.Y -= Speed;

                angle += .015f;
                Circle.PointxAxisTo(angle);

                if (Circle.Size.X < Speed + 1) Done = true;
                Circle.Size = Circle.Size - new Vector2(Speed);
            }
        }

        bool Done = false;
        public void Draw()
        {
            if (!Done)
            {
                UpdateCircle();
                Circle.Draw();
            }
            else
                FinishedCount++;
        }
    }
}