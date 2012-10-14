using System;
using System.IO;
using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Awards;
using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom.Bobs
{
    public class MenuListItem
    {
        public Object obj;
        public String str;

        public MenuListItem(Object obj, String str)
        {
            this.obj = obj;
            this.str = str;
        }
    }

    public class Hat : Buyable
    {
        public int GetGuid() { return Guid; }
        public int GetPrice() { return Price; }

        public int Guid = 0;

        public static int Cheap = 100, Mid = 250, Expensive = 1000;

        public static Hat None, Viking, Fedora, Afro, Halo, Ghost, CheckpointHead, FallingBlockHead, BlobHead,
            MovingBlockHead, SpikeyHead, FallingBlock3Head, Pink, Bubble, FireHead, Horns, Cloud, NoHead,
            TopHat, Knight, Toad, BubbleBobble, Brain, Gosu,
            RobinHood, Rasta, Pumpkin, BunnyEars, Pirate, Miner, Glasses, Antlers, Arrow, Bag, Cone, Pope, Rice, Santa, Sombrero, Tiki, Wizard;

        public static Hat Vandyke, Beard, BigBeard, Goatee, Mustache;

        public int Price;

        public string QuadName;
        public EzTexture HatPicTexture;
        public bool DrawHead, DrawSelf;
        public Vector2 HatPicScale, HatPicShift;

        public bool AllowsFacialHair = true;

        public string Name = null;

        public Awardment AssociatedAward;

        public EzTexture GetTexture()
        {
            if (HatPicTexture != null)
                return HatPicTexture;
            else
            {
                BaseQuad quad = Prototypes.bob[BobPhsxNormal.Instance].PlayerObject.FindQuad(QuadName);
                if (quad != null)
                    return quad.MyTexture;
                else
                    return Fireball.EmitterTexture;
            }
        }

        public void Init()
        {
            HatPicScale = Vector2.One;
            QuadName = null;
            DrawHead = true;
            DrawSelf = true;
        }

        public Hat()
        {
            HatPicScale = Vector2.One;
            HatPicShift = Vector2.Zero;
            HatPicTexture = null;
            DrawSelf = false;
            QuadName = "None";
            DrawHead = true;

            AssociatedAward = null;
        }

        public Hat(string QuadName)
        {
            HatPicScale = Vector2.One;
            HatPicShift = Vector2.Zero;
            HatPicTexture = null;
            DrawSelf = true;
            this.QuadName = QuadName;
            DrawHead = true;

            AssociatedAward = null;
        }

        public Hat(string QuadName, bool DrawHead)
        {
            HatPicScale = Vector2.One;
            HatPicShift = Vector2.Zero;
            HatPicTexture = null;
            DrawSelf = true;
            this.QuadName = QuadName;
            this.DrawHead = DrawHead;

            AssociatedAward = null;
        }

        public Hat(string QuadName, bool DrawHead, bool DrawSelf)
        {
            HatPicScale = Vector2.One;
            HatPicShift = Vector2.Zero;
            HatPicTexture = null;
            this.QuadName = QuadName;
            this.DrawHead = DrawHead;
            this.DrawSelf = DrawSelf;

            AssociatedAward = null;
        }
    }

    public struct ColorScheme
    {
        public override string ToString()
        {
            return string.Format("\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\"", SkinColor.Name, CapeColor.Name, CapeOutlineColor.Name, HatData.Name, BeardData.Name);
        }

        public void WriteChunk_0(BinaryWriter writer)
        {
            var chunk = new Chunk();
            chunk.Type = 0;

            ClrTextFx clr;

            chunk.Write(Math.Abs(ColorSchemeManager.BeardInfo.IndexOf(BeardData)));
            
            clr = SkinColor;
            chunk.Write(Math.Abs(ColorSchemeManager.ColorList.IndexOf(item => (ClrTextFx)(item.obj) == clr)));

            clr = CapeColor;
            chunk.Write(Math.Abs(ColorSchemeManager.CapeColorList.IndexOf(item => (ClrTextFx)(item.obj) == clr)));

            clr = CapeOutlineColor;
            chunk.Write(Math.Abs(ColorSchemeManager.CapeOutlineColorList.IndexOf(item => (ClrTextFx)(item.obj) == clr)));

            chunk.Write(Math.Abs(ColorSchemeManager.HatInfo.IndexOf(HatData)));

            chunk.Finish(writer);
        }

        public void ReadChunk_0(Chunk chunk)
        {
            int index;

            try
            {
                index = chunk.ReadInt();
                BeardData = ColorSchemeManager.BeardInfo[index];
            }
            catch
            {
                BeardData = ColorSchemeManager.BeardInfo[0];
            }

            try
            {
                index = chunk.ReadInt();
                SkinColor = (ClrTextFx)ColorSchemeManager.ColorList[index].obj;
            }
            catch
            {
                SkinColor = (ClrTextFx)ColorSchemeManager.ColorList[0].obj;
            }

            try
            {
                index = chunk.ReadInt();
                CapeColor = (ClrTextFx)ColorSchemeManager.CapeColorList[index].obj;
            }
            catch
            {
                CapeColor = (ClrTextFx)ColorSchemeManager.CapeColorList[0].obj;
            }

            try
            {
                index = chunk.ReadInt();
                CapeOutlineColor = (ClrTextFx)ColorSchemeManager.CapeOutlineColorList[index].obj;
            }
            catch
            {
                CapeOutlineColor = (ClrTextFx)ColorSchemeManager.CapeOutlineColorList[0].obj;
            }

            try
            {
                index = chunk.ReadInt();
                HatData = ColorSchemeManager.HatInfo[index];
            }
            catch
            {
                HatData = ColorSchemeManager.HatInfo[0];
            }
        }

        public ClrTextFx SkinColor, CapeColor, CapeOutlineColor;
        public Hat HatData, BeardData;

        public void Init()
        {
            HatData = Hat.None;
            BeardData = Hat.None;
        }

        public ColorScheme(string skincolor, string capecolor, string capeoutlinecolor, string hatname, string beardname)
        {
            SkinColor = (ClrTextFx)ColorSchemeManager.ColorList.Find(item => string.Compare(item.str, skincolor, StringComparison.OrdinalIgnoreCase) == 0).obj;
            CapeColor = (ClrTextFx)ColorSchemeManager.CapeColorList.Find(item => string.Compare(item.str, capecolor, StringComparison.OrdinalIgnoreCase) == 0).obj;
            CapeOutlineColor = (ClrTextFx)ColorSchemeManager.CapeOutlineColorList.Find(item => string.Compare(item.str, capeoutlinecolor, StringComparison.OrdinalIgnoreCase) == 0).obj;
            HatData = ColorSchemeManager.HatInfo.Find(hat => string.Compare(hat.Name, hatname, StringComparison.OrdinalIgnoreCase) == 0);
            BeardData = ColorSchemeManager.BeardInfo.Find(beard => string.Compare(beard.Name, beardname, StringComparison.OrdinalIgnoreCase) == 0);

            if (HatData == null) HatData = Hat.None;
            if (BeardData == null) BeardData = Hat.Vandyke;
        }
    }
}