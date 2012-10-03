using System;
using Microsoft.Xna.Framework;
using CoreEngine;
using CloudberryKingdom.Bobs;
using System.Collections.Generic;

namespace CloudberryKingdom
{
    public interface Buyable
    {
        int GetGuid();
        int GetPrice();
    }

    public struct ClrTextFx : Buyable
    {
        public int Guid, Price;
        public int GetGuid() { return Guid; }
        public int GetPrice() { return Price; }

        public string Name;

        /// <summary>
        /// A function that may modify the player object.
        /// </summary>
        public Action<Bob> ModObject;

        public EzTexture PicTexture;
        public Vector2 PicScale;

        public bool UsePaintTexture;

        public bool Equals(ClrTextFx a)
        {
            if (string.Compare(a.Name, Name) == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This color is used instead of a texture in situations where the texture can't be applied.
        /// Example: the Spaceship object and the colored Gamer Tags (player names)
        /// </summary>
        public Color DetailColor;

        public Color Clr; public EzTexture Texture; public EzEffect Effect;

        /// <summary>
        /// This matrix is used to hue rotate Bob's outfit.
        /// </summary>
        public Matrix M;

        public static List<ClrTextFx> FullList = new List<ClrTextFx>();

        public ClrTextFx(int Guid, int Price, Color color, Matrix M)
        {
            this.M = M;

            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            Texture = Tools.TextureWad.TextureList[0];
            Effect = Tools.BasicEffect;;
        }

        public ClrTextFx(int Guid, int Price, Color color, Matrix M, string Name)
        {
            this.M = M;
            this.Name = Name;

            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            Texture = Tools.TextureWad.TextureList[0];
            Effect = Tools.BasicEffect; ;
        }

        public ClrTextFx(int Guid, int Price, Color color, Color DetailColor, Matrix M, string Name)
        {
            this.M = M;
            this.Name = Name;

            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = color;
            this.DetailColor = DetailColor;
            Texture = Tools.TextureWad.TextureList[0];
            Effect = Tools.BasicEffect;;
        }

        public ClrTextFx(int Guid, int Price, Color color, EzTexture texture)
        {
            this.M = Matrix.Identity;

            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            this.Texture = texture;
            Effect = Tools.BasicEffect;
        }

        public ClrTextFx(int Guid, int Price, Color color, string texture)
        {
            this.M = Matrix.Identity;

            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            this.Texture = Tools.TextureWad.FindByName(texture);
            Effect = Tools.BasicEffect;;
        }

        public ClrTextFx(int Guid, int Price, Color color, string texture, bool UsePaintTexture)
        {
            this.M = Matrix.Identity;

            this.Guid = Guid; this.Price = Price;
            this.UsePaintTexture = UsePaintTexture;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            this.Texture = Tools.TextureWad.FindByName(texture);
            Effect = Tools.BasicEffect;;
        }

        public ClrTextFx(int Guid, int Price, Color color, string texture, bool UsePaintTexture, EzTexture PicTexture)
        {
            this.M = Matrix.Identity;

            this.Guid = Guid; this.Price = Price;
            this.UsePaintTexture = UsePaintTexture;
            this.PicTexture = PicTexture;

            ModObject = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            this.Texture = Tools.TextureWad.FindByName(texture);
            Effect = Tools.BasicEffect;;
        }

        public ClrTextFx(int Guid, int Price, Color color, string texture, bool UsePaintTexture, string PicTextureName)
        {
            this.M = Matrix.Identity;

            this.Guid = Guid; this.Price = Price;
            this.UsePaintTexture = UsePaintTexture;
            this.PicTexture = Tools.TextureWad.FindByName(PicTextureName);

            ModObject = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            this.Texture = Tools.TextureWad.FindByName(texture);
            Effect = Tools.BasicEffect;;
        }

        public ClrTextFx(int Guid, int Price, Color color, string texture, Color DetailColor)
        {
            this.M = Matrix.Identity;

            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = color;
            this.Texture = Tools.TextureWad.FindByName(texture);
            this.DetailColor = DetailColor;
            Effect = Tools.BasicEffect;;
        }

        public ClrTextFx(int Guid, int Price, Color color, EzTexture texture, Color DetailColor, EzEffect effect)
        {
            this.M = Matrix.Identity;

            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = color;
            this.Texture = texture;
            this.DetailColor = DetailColor;
            this.Effect = effect;
        }

        public ClrTextFx(int Guid, int Price, Color color, string texture, Color DetailColor, string effect)
        {
            this.M = Matrix.Identity;

            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = color;
            this.Texture = Tools.TextureWad.FindByName(texture);
            this.DetailColor = DetailColor;
            Effect = Tools.EffectWad.FindByName(effect);
        }

        public static bool operator ==(ClrTextFx A, ClrTextFx B)
        {
            if (A.ModObject != B.ModObject) return false;
            if (A.Clr != B.Clr) return false;
            if (A.Effect != B.Effect) return false;
            if (A.Texture != B.Texture) return false;
            return true;
        }

        public static bool operator !=(ClrTextFx A, ClrTextFx B)
        {
            return !(A == B);
        }
    }
}