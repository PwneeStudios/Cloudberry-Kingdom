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
    /// <summary>
    /// Should be replaced. Right now this is used for drawing the players in the replays, but replays should instead be reusing the same draw code for the player as the normal draw code.
    /// </summary>
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