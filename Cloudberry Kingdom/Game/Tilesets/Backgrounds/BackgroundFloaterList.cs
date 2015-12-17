using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class BackgroundFloaterList : ViewReadWrite
    {
        public string Name = null;

        public bool Foreground = false;
        public bool Fixed = false;

        public bool Show = true;

#if INCLUDE_EDITOR
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

        public void SetBackground(Background Background)
        {
            foreach (BackgroundFloater floater in Floaters)
                floater.SetBackground(Background);
        }

		public void RotateLeft()
		{
			foreach (BackgroundFloater floater in Floaters)
				floater.RotateLeft();
		}

		public Vector2 GetBL()
		{
			Vector2 BL = new Vector2(float.MaxValue);
			foreach (BackgroundFloater floater in Floaters)
				BL = Vector2.Min(BL, floater.GetBL());

			return BL;
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
            if (!Show) return;

            if (DoPreDraw) return;

            Tools.QDrawer.Flush();

            Camera Cam = MyLevel.MainCamera;

            Cam.SetVertexZoom(Parallax * CamMod);

            foreach (BackgroundFloater Floater in Floaters)
                Floater.Draw();
        }

        public bool DoPreDraw = false;

        public override string[] GetViewables() { return new string[] { "Name", "Parallax", "Floaters", "Fixed", "Foreground", "DoPreDraw" }; }
    }
}