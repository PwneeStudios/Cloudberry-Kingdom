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
            Vector2 BL = new Vector2(1000000,1000000);
            Vector2 TR = new Vector2(-1000000,-1000000);

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

                BackgroundFloater cloud = new BackgroundFloater(level, Root + "_" + type.ToString(), BL.X, TR.X);
                cloud.Data.Position = new Vector2(Pos, level.Rnd.RndFloat(YRange.X, YRange.Y));

                Pos += level.Rnd.RndFloat(DistRange.X, DistRange.Y) / 2;

                Quads.Add(cloud);
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

        public void PhsxStep()
        {
            Data.Position += Data.Velocity;

            if (Data.Position.X + 2 * MyQuad.Base.e1.X < X_Left - 300)
                Data.Position.X = X_Right + 2 * MyQuad.Base.e1.X + 200;

            if (Data.Position.X - 2 * MyQuad.Base.e1.X > X_Right + 300)
                Data.Position.X = X_Left - 2 * MyQuad.Base.e1.X - 200;

            MyQuad.Base.Origin = Data.Position;

            if (SpinVelocity != 0)
                MyQuad.Angle += SpinVelocity;
        }

        public void Draw()
        {
            MyQuad.Draw();
        }
    }
}