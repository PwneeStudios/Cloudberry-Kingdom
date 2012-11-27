using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;

namespace CloudberryKingdom
{
    public class BackgroundFloater
    {
        float _SpinVelocity;
        public float SpinVelocity
        {
            get { return _SpinVelocity; }
            set
            {
                if (MyQuad != null && MyQuad.FancyAngle == null)
                    MyQuad.FancyAngle = new FancyVector2();
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

        public virtual void Draw()
        {
            Tools.QDrawer.DrawQuad(ref MyQuad.Quad);
        }
    }
}