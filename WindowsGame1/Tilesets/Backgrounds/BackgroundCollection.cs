using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class BackgroundCollection
    {
        public Level MyLevel;

        public List<BackgroundFloaterList> Lists;

        public BackgroundCollection(Level level)
        {
            MyLevel = level;

            Lists = new List<BackgroundFloaterList>();
        }

        public void Release()
        {
            MyLevel = null;

            foreach (BackgroundFloaterList list in Lists)
                list.Release();
        }

        public void Sort()
        {
            Lists.Sort((list1, list2) => list1.Parallax.CompareTo(list2.Parallax));
        }

        public void SetLevel(Level level)
        {
            MyLevel = level;

            foreach (BackgroundFloaterList list in Lists)
                list.SetLevel(level);
        }

        public void Move(Vector2 shift)
        {
            foreach (BackgroundFloaterList list in Lists)
                list.Move(shift);
        }

        public void Clear()
        {
            Clear(new FloatRectangle(Vector2.Zero, new Vector2(100000000)));
        }
        public void Clear(FloatRectangle Area)
        {
            for (int i = 0; i < Lists.Count; i++)
                Lists[i].Clear(Area);
        }

        public void Absorb(BackgroundCollection collection)
        {
            for (int i = 0; i < Lists.Count; i++)
                Lists[i].Absorb(collection.Lists[i]);
        }

        public void UpdateBounds(Vector2 BL, Vector2 TR)
        {
            foreach (BackgroundFloaterList list in Lists)
                list.UpdateBounds(BL, TR);
        }

        public void FromInfoWad(string Root, Vector2 BL, Vector2 TR, Level level)
        {
            int NumLists = (int)InfoWad.GetFloat(Root + "_Num");

            for (int i = 0; i < NumLists; i++)
            {
                BackgroundFloaterList NewList = new BackgroundFloaterList();
                NewList.Init(Root + "_" + i.ToString(), BL, TR, level);

                if (i < Lists.Count)
                {
                    Lists[i].Absorb(NewList);
                }
                else
                    Lists.Add(NewList);
            }
        }

        public void DrawLayer(int Layer)
        {
            if (Layer < Lists.Count)
                Lists[Layer].Draw();
        }

        public void FinishDraw()
        {
            Tools.QDrawer.Flush();

            Camera Cam = MyLevel.MainCamera;
            Cam.SetVertexCamera();
            //Vector4 cameraPos = new Vector4(Cam.Data.Position.X, Cam.Data.Position.Y, Cam.Zoom.X, Cam.Zoom.Y);
            //Tools.EffectWad.SetCameraPosition(cameraPos);
        }

        public void Draw() { Draw(1f); }
        public void Draw(float CamMod)
        {
            foreach (BackgroundFloaterList list in Lists)
                list.Draw(CamMod);

            FinishDraw();
        }

        public void PhsxStep()
        {
            foreach (BackgroundFloaterList list in Lists)
                list.PhsxStep();
        }
    }
}