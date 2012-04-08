#define EDITOR

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
    public class SpriteAnimGroup
    {
        public Dictionary<int, SpriteAnim> SpriteAnims;

        public void Release()
        {
            foreach (SpriteAnim anim in SpriteAnims.Values)
                anim.Release();

            SpriteAnims = null;
        }

        public SpriteAnimGroup()
        {
            SpriteAnims = new Dictionary<int, SpriteAnim>();
        }

        public delegate void ToSpriteFunc(Dictionary<int, SpriteAnim> SpriteAnims, Vector2 Padding);
        public void Init(ObjectClass Obj, Vector2 ExtraPadding, ToSpriteFunc SpriteFunc)
        {
            // Make sure stickman is oriented correctly            
            Obj.xFlip = false;
            Obj.yFlip = false;
            Obj.ContainedQuadAngle = 0;

            Vector2 Padding = new Vector2(10, 90) + ExtraPadding;
            /*
            SpriteAnims.Add(0, Obj.AnimToSpriteFrames(0, 1, true, Padding));
            SpriteAnims.Add(1, Obj.AnimToSpriteFrames(1, 1, true, 1, 1, Padding));
            SpriteAnims.Add(2, Obj.AnimToSpriteFrames(2, 1, false, 1, 1, Padding));
            SpriteAnims.Add(4, Obj.AnimToSpriteFrames(4, 1, false, 1, 1, Padding));
            SpriteAnims.Add(5, Obj.AnimToSpriteFrames(5, 1, false, 1, 1, Padding));*/

            SpriteFunc(SpriteAnims, Padding);


            foreach (SpriteAnim sprite in SpriteAnims.Values)
                sprite.Padding = Padding;
        }

        public Texture2D Get(int anim, float t, ref Vector2 padding)
        {
            if (!SpriteAnims.ContainsKey(anim))
                t = anim = 0;

            float dt = SpriteAnims[anim].dt;
            int frame = Math.Min((int)(t / dt), SpriteAnims[anim].Frames.Length - 1);
            padding = SpriteAnims[anim].Padding;
            return SpriteAnims[anim].Frames[frame];
        }
    }

    public class SpriteAnim
    {
        public float dt;
        public Texture2D[] Frames;
        public Vector2 Padding;

        public void Release()
        {
            if (Frames != null)
                for (int i = 0; i < Frames.Length; i++)
                    Frames[i].Dispose();
        }
    }
}