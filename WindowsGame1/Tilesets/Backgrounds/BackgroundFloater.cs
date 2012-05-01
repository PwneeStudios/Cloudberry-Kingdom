using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

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
                    MyQuad.FancyAngle = new Drawing.FancyVector2();
                _SpinVelocity = value;
            }
        }

        Level MyLevel;

        public QuadClass MyQuad;

        public PhsxData Data;

        public float X_Left, X_Right;

        public string Root;

        public void Release()
        {
            MyLevel = null;
            MyQuad.Release();
        }

        public void SetLevel(Level level)
        {
            MyLevel = level;
        }

        public void Move(Vector2 shift)
        {
            X_Left += shift.X;
            X_Right += shift.X;

            Data.Position += shift;
        }

        public void UpdateBounds(Vector2 BL, Vector2 TR)
        {
            X_Left = Math.Min(X_Left, BL.X);
            X_Right = Math.Max(X_Right, TR.X);
        }

        public static void AddSpan(string Root, List<BackgroundFloater> Quads, Vector2 BL, Vector2 TR, Level level)
        {
            int Types = (int)InfoWad.GetFloat(Root + "_Num");
            float[] Ratios = new float[Types];
            for (int i = 0; i < Types; i++)
                Ratios[i] = InfoWad.GetFloat(Root + "_" + i.ToString() + "_Ratio");

            float Pos = BL.X;
            while (Pos < TR.X)
            {
                int type = level.Rnd.Choose(Ratios);

                Vector2 YRange = InfoWad.GetVec(Root + "_" + type.ToString() + "_YRange");
                Vector2 DistRange = InfoWad.GetVec(Root + "_" + type.ToString() + "_Dist");

                Pos += level.Rnd.RndFloat(DistRange.X, DistRange.Y) / 2;

                Vector2 SpeedRange = InfoWad.GetVec(Root + "_" + type.ToString() + "_SpeedRange");
                
                BackgroundFloater floater;
                if (SpeedRange.X == 0 && SpeedRange.Y == 0)
                    floater = new BackgroundFloater_Stationary(level, Root + "_" + type.ToString());
                else
                    floater = new BackgroundFloater(level, Root + "_" + type.ToString(), BL.X, TR.X);

                floater.Data.Position = new Vector2(Pos, level.Rnd.RndFloat(YRange.X, YRange.Y));
                floater.InitialUpdate();

                Pos += level.Rnd.RndFloat(DistRange.X, DistRange.Y) / 2;

                Quads.Add(floater);
            }
        }

        public BackgroundFloater(Level level, float X_Left, float X_Right)
        {
            this.X_Left = X_Left;
            this.X_Right = X_Right;

            MyLevel = level;

            MyQuad = new QuadClass();
        }

        public BackgroundFloater(Level level, string Root, float X_Left, float X_Right)
        {
            this.Root = Root;
            this.X_Left = X_Left;
            this.X_Right = X_Right;

            MyLevel = level;

            MyQuad = new QuadClass();
            MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName(InfoWad.GetStr(Root + "_Texture"));

            Vector2 Size = InfoWad.GetVec(Root + "_Size");
            Vector2 AddSize = InfoWad.GetVec(Root + "_SizeAdd");
            MyQuad.Base.e1 = new Vector2(Size.X + MyLevel.Rnd.RndFloat(0, AddSize.X), 0);
            MyQuad.Base.e2 = new Vector2(0, Size.Y + MyLevel.Rnd.RndFloat(0, AddSize.Y));

            MyQuad.Quad.SetColor(InfoWad.GetColor(Root + "_Color"));

            Vector2 SpeedRange = InfoWad.GetVec(Root + "_SpeedRange");
            Data.Velocity.X = MyLevel.Rnd.RndFloat(SpeedRange.X, SpeedRange.Y);
        }

        public virtual void InitialUpdate()
        {
            MyQuad.Base.Origin = Data.Position;
            MyQuad.Update();
            MyQuad.UpdateShift_Precalc();
        }

        public virtual void PhsxStep()
        {
            Data.Position += Data.Velocity;

            if (Data.Position.X + 2 * MyQuad.Base.e1.X < X_Left - 300)
                Data.Position.X = X_Right + 2 * MyQuad.Base.e1.X + 200;

            if (Data.Position.X - 2 * MyQuad.Base.e1.X > X_Right + 300)
                Data.Position.X = X_Left - 2 * MyQuad.Base.e1.X - 200;

            MyQuad.Base.Origin = Data.Position;

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
            Tools.QDrawer.DrawQuad(MyQuad.Quad);
        }
    }
}