using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CloudberryKingdom
{
    /// <summary>
    /// Wraps either an EzTexture or an AnimationData_Texture.
    /// The purpose of this class is to let a quad's texture be easily replaced by an animation, without additional fuss.
    /// Would be better to implement this functionality with inheritance, with EzTexture and AnimationData_Texture inheriting from a base class.
    /// </summary>
    public class TextureOrAnim
    {
        public EzTexture MyTexture;
        public AnimationData_Texture MyAnim;
        public bool IsAnim = false;

        public TextureOrAnim()
        {
        }

        public TextureOrAnim(string name)
        {
            Set(name);
        }

        public void Set(string name)
        {
            if (Tools.TextureWad.AnimationDict.ContainsKey(name))
            {
                MyAnim = Tools.TextureWad.AnimationDict[name];
                IsAnim = true;
            }
            else
            {
                MyTexture = Tools.Texture(name);
                IsAnim = false;
            }
        }

        public static implicit operator TextureOrAnim(EzTexture texture)
        {
            var t_or_a = new TextureOrAnim();
            t_or_a.MyTexture = texture;
            return t_or_a;
        }

        public static implicit operator TextureOrAnim(string name)
        {
            var t_or_a = new TextureOrAnim();
            t_or_a.Set(name);
            return t_or_a;
        }
    }
}