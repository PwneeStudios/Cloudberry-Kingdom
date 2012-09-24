using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.IO;

using Drawing;

namespace Drawing
{
    public delegate void MoveCallback(Vector2 Pos);
    public delegate void ClickCallback();

    public class ObjectVector
    {
        public AnimationData AnimData;

        public Vector2 Pos, RelPos;
        public BaseQuad ParentQuad;
        public ObjectVector CenterPoint;
        public MoveCallback ModifiedEventCallback;

        public void Release()
        {
            ParentQuad = null;
            CenterPoint = null;
            ModifiedEventCallback = null;
        }

#if EDITOR
        public const int NumSaveStates = 10;
        public Vector2[] SavedStates = new Vector2[NumSaveStates];

        public ClickCallback ClickEventCallback;
#endif

        public void Write(BinaryWriter writer, ObjectClass MainObject)
        {
            if (ParentQuad == null || ParentQuad == MainObject.ParentQuad)
                writer.Write(-1);
            else
                writer.Write(MainObject.QuadList.IndexOf(ParentQuad));

            WriteReadTools.WriteVector2(writer, RelPos);
            WriteReadTools.WriteVector2(writer, Pos);
            AnimData.Write(writer);
        }

        public void Read(BinaryReader reader, ObjectClass MainObject)
        {
            int ParentQuadInt = reader.ReadInt32();
            if (ParentQuadInt == -1)
                ParentQuad = MainObject.ParentQuad;
            else
                ParentQuad = MainObject.QuadList[ParentQuadInt];

            WriteReadTools.ReadVector2(reader, ref RelPos);
            WriteReadTools.ReadVector2(reader, ref Pos);
            AnimData.Read(reader);
        }

#if EDITOR
        public void SaveState(int StateIndex)
        {
            SavedStates[StateIndex] = RelPos;
        }

        public void RecoverState(int StateIndex)
        {
            RelPos = SavedStates[StateIndex];
        }
#endif

        public void Clone(ObjectVector dest)
        {
            Clone(dest, true);
        }

        public void Clone(ObjectVector dest, bool CloneAnimData)
        {
            dest.Pos = Pos;
            dest.RelPos = RelPos;

            if (CloneAnimData)
                dest.AnimData = new AnimationData(AnimData);
            else
                dest.AnimData = AnimData;
        }

        public void CopyAnim(ObjectVector vec, int Anim)
        {
            AnimData.CopyAnim(vec.AnimData, Anim);
        }

        public ObjectVector()
        {
            AnimData.Init();// = new AnimationData();

            Pos = new Vector2();
            ModifiedEventCallback = DefaultCallback;

#if EDITOR
            ClickEventCallback = DefaultClickCallback;
#endif

            CenterPoint = null;
        }

#if EDITOR
        public void Click()
        {
            if (ClickEventCallback != null)
                ClickEventCallback();
        }
#endif

        public void Move(Vector2 NewPos)
        {
            if (ModifiedEventCallback != null)
                ModifiedEventCallback(NewPos);
        }

        public void DefaultCallback(Vector2 NewPos)
        {
            Pos = NewPos;

            RelPosFromPos();
        }

#if EDITOR
        public void DefaultClickCallback() { }
#endif

        public void RelPosFromPos()
        {
            Vector2 C = new Vector2(0, 0);
            if (CenterPoint != null) C = CenterPoint.Pos;

            if (ParentQuad != null)
            {
                if (ParentQuad is Quad)
                {
                    Quad PQuad = (Quad)ParentQuad;
                    if (CenterPoint == null) C = PQuad.Center.Pos;

                    Vector2 Dif = Pos - C;// PQuad.Center.Pos;
                    Vector2 axis = PQuad.xAxis.Pos - PQuad.Center.Pos;
                    RelPos.X = Vector2.Dot(Dif, axis) / axis.LengthSquared();
                    axis = PQuad.yAxis.Pos - PQuad.Center.Pos;
                    RelPos.Y = Vector2.Dot(Dif, axis) / axis.LengthSquared();
                }
            }
            else
                RelPos = Pos - C;
        }

        // Assumes there is a parent quad and that it is a quad
        public void FastPosFromRelPos(Quad parent)
        {
            Pos = parent.Center.Pos * (1 - RelPos.X - RelPos.Y) + RelPos.X * (parent.xAxis.Pos) + RelPos.Y * (parent.yAxis.Pos);
        }

        public void PosFromRelPos()
        {
            Vector2 C1 = new Vector2(0, 0);
            Vector2 C2 = new Vector2(0, 0);
            if (CenterPoint != null) C1 = CenterPoint.Pos;

            if (ParentQuad != null)
            {
                if (ParentQuad is Quad)
                {
                    Quad PQuad = (Quad)ParentQuad;
                    C2 = PQuad.Center.Pos;
                    if (CenterPoint == null) C1 = PQuad.Center.Pos;

                    Pos = C1 + RelPos.X * (PQuad.xAxis.Pos - C2) + RelPos.Y * (PQuad.yAxis.Pos - C2);
                }
            }
            else
                Pos = RelPos + C1;
        }
    }
}