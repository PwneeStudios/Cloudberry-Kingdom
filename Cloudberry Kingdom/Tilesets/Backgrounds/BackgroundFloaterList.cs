using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class BackgroundFloaterList : ViewReadWrite
    {
        public string Name = null;

#if INCLUDE_EDITOR
        public bool Show = true;

        /// <summary>
        /// When locked a layer can not be edited.
        /// </summary>
        public bool Lock = false;

        public bool Editable
        {
            get
            {
                return Show && !Lock;
            }
        }
#endif

        public Level MyLevel;
        public List<BackgroundFloater> Floaters;

        public float Parallax;

        /// <summary>
        /// Reset the list to its start position.
        /// </summary>
        public void Reset()
        {
            if (Floaters == null) return;

            foreach (var floater in Floaters)
                floater.Reset();
        }

        public void SetParallaxAndPropagate(float Parallax)
        {
            float PrevParallax = this.Parallax;
            this.Parallax = Parallax;

            foreach (var floater in Floaters)
                floater.ChangeParallax(PrevParallax, Parallax);
        }

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
        }

        public Vector2 BL, TR;
        public void PhsxStep()
        {
            TR = MyLevel.MyBackground.TR;
            BL = MyLevel.MyBackground.BL;
            var c = MyLevel.MainCamera.Pos;

            TR = c + (TR - c) / Parallax;
            BL = c + (BL - c) / Parallax;

            foreach (BackgroundFloater Floater in Floaters)
                Floater.PhsxStep(this);
        }

        public void Draw() { Draw(1); }
        public void Draw(float CamMod)
        {
#if DEBUG && INCLUDE_EDITOR
            if (!Show) return;
#endif

            Tools.QDrawer.Flush();

            Camera Cam = MyLevel.MainCamera;

            Cam.SetVertexZoom(Parallax * CamMod);

            foreach (BackgroundFloater Floater in Floaters)
                Floater.Draw();
        }

        public void Init(string Root, Vector2 BL, Vector2 TR, Level level)
        {
            MyLevel = level;

            Parallax = InfoWad.GetFloat(Root + "_Parralax");

            BackgroundFloater.AddSpan(Root, Floaters, BL, TR, MyLevel);
        }

        public override string[] GetViewables() { return new string[] { "Name", "Parallax", "Floaters" }; }
    }
}