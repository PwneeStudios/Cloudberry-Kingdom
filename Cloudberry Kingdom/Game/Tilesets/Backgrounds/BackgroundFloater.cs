using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;

using CloudberryKingdom.Levels;

#if INCLUDE_EDITOR
using CloudberryKingdom.Viewer;
#endif

namespace CloudberryKingdom
{
    public class BackgroundFloater : ViewReadWrite
    {
#if INCLUDE_EDITOR
        public bool Selected = false, SoftSelected = false, FixedAspectPreference = true;

        public bool Editable
        {
            get
            {
                if (Parent == null) return false;

                return Parent.Show && !Parent.Lock;
            }
        }

        /// <summary>
        /// Get the list a floater belongs to.
        /// Note, this is expensive and should not exist in Release mode.
        /// There is no need for a floater to track who it's parent is, and only complicates the GC graph.
        /// </summary>
        public BackgroundFloaterList Parent
        {
            get
            {
                if (MyLevel == null) return null;
                if (MyLevel.MyBackground == null) return null;

                foreach (var list in MyLevel.MyBackground.MyCollection.Lists)
                    if (list.Floaters.Contains(this))
                        return list;
                return null;
            }
        }

        public BackgroundViewer.TreeNode_Floater Node = null;
#endif

        float _SpinVelocity;
        public float SpinVelocity
        {
            get { return _SpinVelocity; }
            set
            {
                if (MyQuad != null && MyQuad.FancyAngle == null)
                    MyQuad.FancyAngle = new CoreEngine.FancyVector2();
                _SpinVelocity = value;
            }
        }

        Level MyLevel;
        public string Name = null;

        public QuadClass MyQuad;
        public Vector2 uv_speed = Vector2.Zero, uv_offset = Vector2.Zero;

        public PhsxData Data, StartData;

        public Vector2 Pos
        {
            get
            {
                return Data.Position;
            }
        }

        /// <summary>
        /// Sets the current and start position of the floater.
        /// </summary>
        public void SetPos(Vector2 pos)
        {
            Data.Position = StartData.Position = pos;
        }

        public string Root;

        public virtual void Release()
        {
            MyLevel = null;

            MyQuad.Release();
        }

        public void SetLevel(Level level)
        {
            MyLevel = level;
        }

        public virtual void SetBackground(Background b)
        {
            //MyBackground = b;
        }

        /// <summary>
        /// Reset the floater to its start position.
        /// </summary>
        public void Reset()
        {
            Data.Position = StartData.Position;
            MyQuad.Quad.UV_Offset = uv_offset;

            InitialUpdate();
        }

        public void ChangeParallax(float Prev, float New)
        {
            float ratio = Prev / New;
            var cam = MyLevel.MainCamera.Pos;

            Data.Position = (Data.Position - cam) * ratio + cam;
            StartData.Position = (StartData.Position - cam) * ratio + cam;
            MyQuad.Size *= ratio;

            InitialUpdate();
        }

        public void Move(Vector2 shift)
        {
            Data.Position += shift;
            StartData.Position += shift;
            
            InitialUpdate();
        }

        public BackgroundFloater(BackgroundFloater source)
        {
            this.Data = source.Data;
            this.StartData = source.StartData;
#if DEBUG && INCLUDE_EDITOR
            this.FixedAspectPreference = source.FixedAspectPreference;
#endif
            this.MyLevel = source.MyLevel;
            this.MyQuad = new QuadClass(source.MyQuad);
            this.Name = source.Name;
        }

        public BackgroundFloater()
        {
            MyQuad = new QuadClass();
        }

        public BackgroundFloater(Level level)
        {
            MyLevel = level;

            MyQuad = new QuadClass();
        }

        public virtual void InitialUpdate()
        {
            MyQuad.Base.Origin = Data.Position = StartData.Position;
            MyQuad.Update();
            MyQuad.UpdateShift_Precalc();

            // If we are repeating more than once, or have UV speed, use texture wrapping.
            MyQuad.Quad.U_Wrap = MyQuad.Quad.V_Wrap = false;
            if (uv_speed.X != 0 || MyQuad.Quad.UV_Repeat.X > 1)
                MyQuad.Quad.U_Wrap = true;
            if (uv_speed.Y != 0 || MyQuad.Quad.UV_Repeat.Y > 1)
                MyQuad.Quad.V_Wrap = true;
        }

        public virtual void PhsxStep(BackgroundFloaterList list)
        {
            MyQuad.Quad.UV_Phsx(uv_speed);

            Data.Position += Data.Velocity;

            if (MyQuad.Quad.Right < list.BL.X - 100)
                Data.Position.X = list.TR.X + MyQuad.Quad.Width / 2 + 50;
            else if (MyQuad.Quad.Left > list.TR.X + 100)
                Data.Position.X = list.BL.X - MyQuad.Quad.Width / 2 - 50;

            MyQuad.Base.Origin = Data.Position;
            //if (MyQuad.TextureName.Contains("chan"))
            //{
            //    MyQuad.PointxAxisTo(CoreMath.Periodic(.005f, -.005f, 300, MyLevel.CurPhsxStep));
            //    InitialUpdate();
            //}
            //else
                MyQuad.PointxAxisTo(0);
            MyQuad.Update();

            if (SpinVelocity != 0)
            {
                MyQuad.Angle += SpinVelocity;
                MyQuad.Update();
            }
            else
            {
                MyQuad.UpdateShift();
            }
        }

#if DEBUG && INCLUDE_EDITOR
        protected void Draw_DebugExtra()
        {
            if (Tools.background_viewer == null || Tools.background_viewer.PlayCheckbox.Checked) return;

            if (Selected || SoftSelected)
            {
                var HoldTexture = MyQuad.Quad.MyTexture;
                var HoldColor = MyQuad.Quad.MySetColor;

                MyQuad.Quad.MyTexture = Tools.TextureWad.DefaultTexture;
                if (Selected)
                    MyQuad.Quad.SetColor(new Color(200, 200, 255, 135));
                else
                    MyQuad.Quad.SetColor(new Color(200, 200, 255, 45));

                Tools.QDrawer.DrawQuad(ref MyQuad.Quad);

                MyQuad.Quad.MyTexture = HoldTexture;
                MyQuad.Quad.SetColor(HoldColor);
            }
        }
#endif

        public virtual void Draw()
        {
            Tools.QDrawer.DrawQuad(ref MyQuad.Quad);

#if DEBUG && INCLUDE_EDITOR
            Draw_DebugExtra();
#endif
        }

        static string[] _bits_to_save = new string[] { "Name", "MyQuad", "uv_speed", "uv_offset", "Data", "StartData" };
        public override string[] GetViewables() { return _bits_to_save; }

        public override void Read(StreamReader reader)
        {
            base.Read(reader);
            InitialUpdate();
        }
    }
}