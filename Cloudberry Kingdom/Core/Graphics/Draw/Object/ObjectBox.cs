using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.IO;

using CoreEngine;

namespace CoreEngine
{
    public class ObjectBox
    {
        public ObjectVector BL, TR;

        public string Name;
        public bool Show = true;

        public void Release()
        {
            BL.Release(); BL = null;
            TR.Release(); TR = null;
        }

        public void SetHold()
        {
            BL.AnimData.Hold = BL.RelPos;
            TR.AnimData.Hold = TR.RelPos;
        }

        public void ReadAnim(int anim, int frame)
        {
            BL.RelPos = BL.AnimData.Get(anim, frame);
            TR.RelPos = TR.AnimData.Get(anim, frame);
        }

        public void Record(int anim, int frame, bool UseRelativeCoords)
        {
            if (UseRelativeCoords)
            {
                BL.AnimData.Set(BL.RelPos, anim, frame);
                TR.AnimData.Set(TR.RelPos, anim, frame);
            }
            else
            {
                BL.AnimData.Set(BL.Pos, anim, frame);
                TR.AnimData.Set(TR.Pos, anim, frame);
            }
        }

        public void Transfer(int anim, float DestT, int AnimLength, bool Loop, bool Linear, float t)
        {
            BL.RelPos = BL.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
            TR.RelPos = TR.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
        }

        public void Calc(int anim, float t, int AnimLength, bool Loop, bool Linear)
        {
            BL.RelPos = BL.AnimData.Calc(anim, t, AnimLength, Loop, Linear);
            TR.RelPos = TR.AnimData.Calc(anim, t, AnimLength, Loop, Linear);
        }

        public void Write(BinaryWriter writer, ObjectClass MainObject)
        {
            BL.Write(writer, MainObject);
            TR.Write(writer, MainObject);

            // Version 53 4/1/2010
            // Write quad name
            // Write show bool
            writer.Write(Name);
            writer.Write(Show);            
        }

        public void Read(BinaryReader reader, ObjectClass MainObject, int VersionNumber)
        {
            BL.Read(reader, MainObject);
            TR.Read(reader, MainObject);

            // Version 53, 4/1/2010
            // Read name
            // Read show bool
            if (VersionNumber > 52)
            {
                Name = reader.ReadString();
                Show = reader.ReadBoolean();
            }
        }

        public ObjectBox(ObjectBox box, bool DeepClone)
        {
            BL = new ObjectVector();
            box.BL.Clone(BL, DeepClone);
            TR = new ObjectVector();
            box.TR.Clone(TR, DeepClone);

            Show = box.Show;
            Name = box.Name;
        }

        public ObjectBox()
        {
            BL = new ObjectVector();
            TR = new ObjectVector();

            Name = "Box";
        }

        public void Update()
        {
            BL.PosFromRelPos();
            TR.PosFromRelPos();
        }

        public Vector2 Center()
        {
            return (TR.Pos + BL.Pos) / 2;
        }

        public Vector2 Size()
        {
            return (TR.Pos - BL.Pos);
        }

#if EDITOR
        public List<ObjectVector> GetObjectVectors()
        {
            List<ObjectVector> ObjectVectorList = new List<ObjectVector>();

            ObjectVectorList.Add(BL);
            ObjectVectorList.Add(TR);

            return ObjectVectorList;
        }

        public void SaveState(int StateIndex)
        {
            TR.SaveState(StateIndex);
            BL.SaveState(StateIndex);
        }

        public void RecoverState(int StateIndex)
        {
            TR.RecoverState(StateIndex);
            BL.RecoverState(StateIndex);
        }
#endif

        public void DrawExtra(QuadDrawer Drawer, Color clr)
        {
            if (!Show) return;

            Drawer.DrawLine(BL.Pos, new Vector2(TR.Pos.X, BL.Pos.Y), clr, .02f);
            Drawer.DrawLine(BL.Pos, new Vector2(BL.Pos.X, TR.Pos.Y), clr, .02f);
            Drawer.DrawLine(TR.Pos, new Vector2(TR.Pos.X, BL.Pos.Y), clr, .02f);
            Drawer.DrawLine(TR.Pos, new Vector2(BL.Pos.X, TR.Pos.Y), clr, .02f);
        }
    }
}