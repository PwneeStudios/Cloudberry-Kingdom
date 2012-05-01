using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class BackgroundFloaterList
    {
        public Level MyLevel;
        public List<BackgroundFloater> Floaters;

        public float Parralax;

        public BackgroundFloaterList()
        {
            Floaters = new List<BackgroundFloater>();
        }

        public void Release()
        {
            MyLevel = null;

            foreach (BackgroundFloater floater in Floaters)
                floater.Release();
        }

        public void SetLevel(Level level)
        {
            MyLevel = level;

            foreach (BackgroundFloater floater in Floaters)
                floater.SetLevel(level);
        }

        public void CalcBounds()
        {
            Vector2 BL = new Vector2(1000000, 1000000);
            Vector2 TR = new Vector2(-1000000, -1000000);

            foreach (BackgroundFloater floater in Floaters)
            {
                TR = Vector2.Max(TR, new Vector2(floater.X_Left, 0));
                BL = Vector2.Min(BL, new Vector2(floater.X_Right, 0));
            }

            UpdateBounds(BL, TR);
        }

        public void UpdateBounds(Vector2 BL, Vector2 TR)
        {
            foreach (BackgroundFloater floater in Floaters)
                floater.UpdateBounds(BL, TR);
        }

        public void Move(Vector2 shift)
        {
            foreach (BackgroundFloater floater in Floaters)
                floater.Move(shift);
        }

        public void Clear()
        {
            Floaters.Clear();
        }

        public void Clear(FloatRectangle Area)
        {
            Floaters.RemoveAll(floater =>
            {
                if (floater.Data.Position.X > Area.TR.X ||
                    floater.Data.Position.X < Area.BL.X ||
                    floater.Data.Position.Y > Area.TR.Y ||
                    floater.Data.Position.Y < Area.BL.Y)
                    return false;
                else
                    return true;
            });
        }

        public void Absorb(BackgroundFloaterList list)
        {
            foreach (BackgroundFloater floater in list.Floaters)
                floater.SetLevel(MyLevel);

            Floaters.AddRange(list.Floaters);

            CalcBounds();
        }

        public void PhsxStep()
        {
            foreach (BackgroundFloater Floater in Floaters)
                Floater.PhsxStep();
        }

        public void Draw() { Draw(1); }
        public void Draw(float CamMod)
        {
            Tools.QDrawer.Flush();

            Camera Cam = MyLevel.MainCamera;
            //Vector4 ParralaxCameraPos = new Vector4(Cam.Data.Position.X, Cam.Data.Position.Y, Cam.Zoom.X * Parralax, Cam.Zoom.Y * Parralax);
            //Tools.EffectWad.SetCameraPosition(ParralaxCameraPos);
            Cam.SetVertexZoom(Parralax * CamMod);

            foreach (BackgroundFloater Floater in Floaters)
                Floater.Draw();
        }

        public void Init(string Root, Vector2 BL, Vector2 TR, Level level)
        {
            MyLevel = level;

            Parralax = InfoWad.GetFloat(Root + "_Parralax");

            BackgroundFloater.AddSpan(Root, Floaters, BL, TR, MyLevel);
        }
    }
}