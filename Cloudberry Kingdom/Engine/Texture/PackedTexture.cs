using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CloudberryKingdom;

namespace CoreEngine
{
    public class PackedTexture
    {
        public CoreTexture MyTexture;

        public struct SubTexture
        {
            public string name;
            public Vector2 BL, TR;
        }
        
        /// <summary>
        /// The subtextures held inside this PackedTexture.
        /// </summary>
        public List<SubTexture> SubTextures = new List<SubTexture>();
        
        public PackedTexture(string name)
        {
            MyTexture = Tools.TextureWad.FindByName(name);
        }

        public void Add(string name, float x1, float y1, float x2, float y2)
        {
            SubTexture sub;
            sub.name = name;
            sub.BL = new Vector2(x1, y2);
            sub.TR = new Vector2(x2, y1);

            SubTextures.Add(sub);
        }
    }
}