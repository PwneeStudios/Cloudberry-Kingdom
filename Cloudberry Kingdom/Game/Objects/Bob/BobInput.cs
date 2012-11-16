using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public struct BobInput
    {
        public bool A_Button, B_Button;
        public Vector2 xVec;

        public void Clean()
        {
            A_Button = B_Button = false;
            xVec = Vector2.Zero;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(A_Button);
            writer.Write(B_Button);
            writer.Write(xVec);
        }

        public void Read(BinaryReader reader)
        {
            A_Button = reader.ReadBoolean();
            B_Button = reader.ReadBoolean();
            xVec = reader.ReadVector2();
        }
    }
}