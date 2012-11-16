using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.IO;




namespace CloudberryKingdom
{
    public class ObjectVector
    {
        public AnimationData AnimData;

        public Vector2 Pos, RelPos;
        public BaseQuad ParentQuad;
        public ObjectVector CenterPoint;
        public Lambda_1<Vector2> ModifiedEventCallback;

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
            ModifiedEventCallback = new DefaultCallbackLambda(this);

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
                ModifiedEventCallback.Apply(NewPos);
        }

        class DefaultCallbackLambda : Lambda_1<Vector2>
        {
            ObjectVector v;
            public DefaultCallbackLambda(ObjectVector v)
            {
                this.v = v;
            }

            public void Apply(Vector2 NewPos)
            {
                v.Pos = NewPos;

                v.RelPosFromPos();
            }
        }

#if EDITOR
        public void DefaultClickCallback() { }
#endif

        public void RelPosFromPos()
        {
            if (ParentQuad != null)
                ParentQuad.Set_RelPosFromPos(this);
            else
            {
                Vector2 C = Vector2.Zero;
                if (CenterPoint != null) C = CenterPoint.Pos;

                RelPos = Pos - C;
            }
        }

        public void PosFromRelPos()
        {
            if (ParentQuad != null)
                ParentQuad.Set_PosFromRelPos(this);
            else
            {
                Vector2 C = Vector2.Zero;
                if (CenterPoint != null) C = CenterPoint.Pos;

                Pos = RelPos + C;
            }
        }
    }
}