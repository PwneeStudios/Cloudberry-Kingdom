using System;
using System.IO;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Awards;

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
            TopHat, Knight, Toad, BubbleBobble, Brain, Gosu;

        public int Price;

        public string QuadName;
        public EzTexture HatPicTexture;
        public bool DrawHead, DrawSelf;
        public Vector2 HatPicScale, HatPicShift;

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
        /*
        public static bool operator ==(Hat A, Hat B)
        {
            return A.DrawHead == B.DrawHead && A.QuadName.CompareTo(B.QuadName) == 0;
        }

        public static bool operator !=(Hat A, Hat B)
        {
            return !(A == B);
        }*/
    }

    public struct ColorScheme
    {
        public static int version = 1;

        public void Write(BinaryWriter writer)
        {
            // Version
            writer.Write(version);

            ClrTextFx clr; int index;

            clr = OutlineColor;
            index = ColorSchemeManager.OutlineList.IndexOf(item => (ClrTextFx)(item.obj) == clr);
            writer.WriteSafeIndex(index);

            clr = SkinColor;
            index = ColorSchemeManager.ColorList.IndexOf(item => (ClrTextFx)(item.obj) == clr);
            writer.WriteSafeIndex(index);

            clr = CapeColor;
            index = ColorSchemeManager.CapeColorList.IndexOf(item => (ClrTextFx)(item.obj) == clr);
            writer.WriteSafeIndex(index);

            clr = CapeOutlineColor;
            index = ColorSchemeManager.CapeOutlineColorList.IndexOf(item => (ClrTextFx)(item.obj) == clr);
            writer.WriteSafeIndex(index);

            index = ColorSchemeManager.HatInfo.IndexOf(HatData);
            writer.WriteSafeIndex(index);
        }

        public void Read(BinaryReader reader)
        {
            // Version
            int LoadedVersion = reader.ReadInt32();
            int index;

            index = reader.ReadInt32();
            OutlineColor = (ClrTextFx)ColorSchemeManager.OutlineList[index].obj;

            index = reader.ReadInt32();
            SkinColor = (ClrTextFx)ColorSchemeManager.ColorList[index].obj;

            index = reader.ReadInt32();
            CapeColor = (ClrTextFx)ColorSchemeManager.CapeColorList[index].obj;

            index = reader.ReadInt32();
            CapeOutlineColor = (ClrTextFx)ColorSchemeManager.CapeOutlineColorList[index].obj;

            index = reader.ReadInt32();
            HatData = ColorSchemeManager.HatInfo[index];
        }

        public ClrTextFx OutlineColor, SkinColor, CapeColor, CapeOutlineColor;
        public Hat HatData;

        public void Init()
        {
            //HatData.Init();
            HatData = Hat.None;
        }

        public ColorScheme(string outlinecolor, string skincolor, string capecolor, string capeoutlinecolor, string hatname)
        {
            //HatData = new Hat();
            //HatData.Init();
            HatData = Hat.None;
            OutlineColor = (ClrTextFx)ColorSchemeManager.OutlineList.Find(delegate(MenuListItem item) { return string.Compare(item.str, outlinecolor, StringComparison.OrdinalIgnoreCase) == 0; }).obj;
            SkinColor = (ClrTextFx)ColorSchemeManager.ColorList.Find(delegate(MenuListItem item) { return string.Compare(item.str, skincolor, StringComparison.OrdinalIgnoreCase) == 0; }).obj;
            CapeColor = (ClrTextFx)ColorSchemeManager.CapeColorList.Find(delegate(MenuListItem item) { return string.Compare(item.str, capecolor, StringComparison.OrdinalIgnoreCase) == 0; }).obj;
            CapeOutlineColor = (ClrTextFx)ColorSchemeManager.CapeOutlineColorList.Find(delegate(MenuListItem item) { return string.Compare(item.str, capeoutlinecolor, StringComparison.OrdinalIgnoreCase) == 0; }).obj;
            HatData = ColorSchemeManager.HatInfo.Find(delegate(Hat hat) { return string.Compare(hat.QuadName, hatname, StringComparison.OrdinalIgnoreCase) == 0; });
        }
    }
}