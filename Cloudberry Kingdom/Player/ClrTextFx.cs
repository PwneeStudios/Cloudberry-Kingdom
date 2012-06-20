using System;
using Microsoft.Xna.Framework;
using Drawing;
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

        public static List<ClrTextFx> FullList = new List<ClrTextFx>();

        public ClrTextFx(int Guid, int Price, Color color)
        {
            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            Texture = Tools.TextureWad.TextureList[0];
            Effect = Tools.EffectWad.EffectList[0];
        }

        public ClrTextFx(int Guid, int Price, Color color, Color DetailColor)
        {
            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = color;
            this.DetailColor = DetailColor;
            Texture = Tools.TextureWad.TextureList[0];
            Effect = Tools.EffectWad.EffectList[0];
        }

        public ClrTextFx(int Guid, int Price, Color color, EzTexture texture)
        {
            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            this.Texture = texture;
            Effect = Tools.EffectWad.EffectList[0];
        }

        public ClrTextFx(int Guid, int Price, Color color, string texture)
        {
            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            this.Texture = Tools.TextureWad.FindByName(texture);
            Effect = Tools.EffectWad.EffectList[0];
        }

        public ClrTextFx(int Guid, int Price, Color color, string texture, bool UsePaintTexture)
        {
            this.Guid = Guid; this.Price = Price;
            this.UsePaintTexture = UsePaintTexture;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            this.Texture = Tools.TextureWad.FindByName(texture);
            Effect = Tools.EffectWad.EffectList[0];
        }

        public ClrTextFx(int Guid, int Price, Color color, string texture, bool UsePaintTexture, EzTexture PicTexture)
        {
            this.Guid = Guid; this.Price = Price;
            this.UsePaintTexture = UsePaintTexture;
            this.PicTexture = PicTexture;

            ModObject = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            this.Texture = Tools.TextureWad.FindByName(texture);
            Effect = Tools.EffectWad.EffectList[0];
        }

        public ClrTextFx(int Guid, int Price, Color color, string texture, bool UsePaintTexture, string PicTextureName)
        {
            this.Guid = Guid; this.Price = Price;
            this.UsePaintTexture = UsePaintTexture;
            this.PicTexture = Tools.TextureWad.FindByName(PicTextureName);

            ModObject = null; PicScale = Vector2.One; Name = "";

            this.Clr = this.DetailColor = color;
            this.Texture = Tools.TextureWad.FindByName(texture);
            Effect = Tools.EffectWad.EffectList[0];
        }

        public ClrTextFx(int Guid, int Price, Color color, string texture, Color DetailColor)
        {
            this.Guid = Guid; this.Price = Price;
            UsePaintTexture = true;

            ModObject = null; PicTexture = null; PicScale = Vector2.One; Name = "";

            this.Clr = color;
            this.Texture = Tools.TextureWad.FindByName(texture);
            this.DetailColor = DetailColor;
            Effect = Tools.EffectWad.EffectList[0];
        }

        public ClrTextFx(int Guid, int Price, Color color, EzTexture texture, Color DetailColor, EzEffect effect)
        {
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