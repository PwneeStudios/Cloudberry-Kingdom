using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class BackgroundCollection : ViewReadWrite
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

        /// <summary>
        /// Reset the lists to their start position.
        /// </summary>
        public void Reset()
        {
            if (Lists == null) return;

            foreach (var list in Lists)
                list.Reset();
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

        public void SetBackground(Background b)
        {
            foreach (BackgroundFloaterList list in Lists)
                list.SetBackground(b);
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

        public void Draw() { Draw(1f, false); }
        public void Draw(float CamMod, bool Foreground)
        {
            foreach (BackgroundFloaterList list in Lists)
                if (list.Foreground == Foreground)
                    list.Draw(CamMod);

            FinishDraw();
        }

        public void PhsxStep()
        {
#if INCLUDE_EDITOR
            if (Tools.EditorPause) return;
#endif
            foreach (BackgroundFloaterList list in Lists)
                list.PhsxStep();
        }

        public override string[] GetViewables() { return new string[] { "Lists" }; }
    }
}