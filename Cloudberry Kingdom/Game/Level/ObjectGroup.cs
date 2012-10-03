using Microsoft.Xna.Framework;
using CloudberryKingdom.Levels;
using System.Collections.Generic;

using CoreEngine;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class ObjectGroup : ViewReadWrite
    {
        public override string[] GetViewables()
        {
            return new string[] { };
        }

        public void Release()
        {
            if (lvl != null) lvl.Release(); lvl = null;
            IObjects = null;
            Center = null;

            if (FancyPos != null) FancyPos.Release();
            if (FancyScale != null) FancyScale.Release();
        }

        public FancyVector2 FancyPos, FancyScale;

        List<ObjectBase> IObjects = new List<ObjectBase>();
        public Level lvl;

        ObjectBase Center;
        public ObjectGroup()
        {
            Init(null, null, Vector2.Zero, new Vector2(590));
        }
        public ObjectGroup(string filename, FancyVector2 center)
        {
            Init(filename, center, Vector2.Zero, new Vector2(590));
        }
        public ObjectGroup(string filename, FancyVector2 center, Vector2 pos)
        {
            Init(filename, center, pos, new Vector2(590));
        }
        public ObjectGroup(string filename, FancyVector2 center, Vector2 pos, Vector2 size)
        {
            Init(filename, center, pos, size);
        }
        void Init(string filename, FancyVector2 center, Vector2 pos, Vector2 size)
        {
            FancyPos = new FancyVector2(center);
            FancyScale = new FancyVector2(); FancyScale.RelVal = new Vector2(1000);

            if (filename != null && filename.Length > 0)
            {
                lvl = new Level(filename, Tools.CurGameData);
                lvl.MyGame = null;
                lvl.MainCamera = new Camera();
                lvl.MainCamera.SetPhsxType(Camera.PhsxType.Fixed);

                IObjects.AddRange(lvl.Objects);
                IObjects.AddRangeAndConvert(lvl.Blocks);
                //IObjects.AddRange(lvl.Blocks);

                Center = lvl.FindBlock("Center");
                if (Center == null)
                    Center = IObjects[0];
            }

            Pos = pos;
            Size = size;
        }

        public void Add(ObjectBase IObject) { IObjects.Add(IObject); }

        public Vector2 PosOf(string Code1)
        {
            Vector2 pos = lvl.FindBlock(Code1).Pos;
            return pos + FancyPos.AbsVal - Center.Pos;
        }

        public void Update()
        {
            FancyPos.Update();

            lvl.MainCamera.Pos = -(FancyPos.AbsVal - Tools.CurLevel.MainCamera.Pos - Center.Pos);
            lvl.MainCamera.Zoom = .001f * .001f * FancyScale.RelVal;
        }

        public Vector2 Pos
        {
            get { return FancyPos.RelVal; }
            set { FancyPos.RelVal = value; }
        }

        public Vector2 Size
        {
            get { return FancyScale.RelVal; }
            set { FancyScale.RelVal = value; }
        }

        public void Draw() { Draw(0, 100, true); }
        public void Draw(int StartLayer, int EndLayer, bool DoPhsx)
        {
            Update();
            if (DoPhsx)
                lvl.MainCamera.PhsxStep();

            Tools.QDrawer.Flush();
            Vector4 HoldCam = Tools.EffectWad.CameraPosition;
            lvl.Draw(true, StartLayer, EndLayer);
            Tools.QDrawer.Flush();
            Tools.EffectWad.SetCameraPosition(HoldCam);
        }

        public void Shift(Vector2 shift)
        {
            foreach (ObjectBase obj in IObjects)
                obj.Move(shift);
        }
    }
}