using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;






namespace CloudberryKingdom
{
    public class MenuListItem
    {
        public Object obj;
        public Localization.Words word;

        public MenuListItem(Object obj, Localization.Words word)
        {
            this.obj = obj;
            this.word = word;
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

        public Localization.Words Name = Localization.Words.None;

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

        int IndexOf(List<MenuListItem> list, ClrTextFx clr)
        {
            int index = 0;
            foreach (var item in list)
            {
                if ((ClrTextFx)(item.obj) == clr)
                    return index;
                index++;
            }
            return -1;
        }

        int IndexOf(List<Hat> list, Hat hat)
        {
            int index = 0;
            foreach (var _hat in list)
            {
                if (_hat == hat)
                    return index;
                index++;
            }
            return -1;
        }


        public void WriteChunk_0(BinaryWriter writer)
        {
            var chunk = new Chunk();
            chunk.Type = 0;

            ClrTextFx clr;

            chunk.Write(Math.Abs(IndexOf(ColorSchemeManager.BeardInfo, BeardData)));
            
            clr = SkinColor;
            chunk.Write(Math.Abs(IndexOf(ColorSchemeManager.ColorList, clr)));

            clr = CapeColor;
            chunk.Write(Math.Abs(IndexOf(ColorSchemeManager.CapeColorList, clr)));

            clr = CapeOutlineColor;
            chunk.Write(Math.Abs(IndexOf(ColorSchemeManager.CapeOutlineColorList, clr)));

            chunk.Write(Math.Abs(IndexOf(ColorSchemeManager.HatInfo, HatData)));

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

        class FindColorLambda : LambdaFunc_1<MenuListItem, bool>
        {
            Localization.Words word;
            public FindColorLambda(Localization.Words word)
            {
                this.word = word;
            }

            public bool Apply(MenuListItem item)
            {
                return item.word == word;
            }
        }

        class FindHatLambda : LambdaFunc_1<Hat, bool>
        {
            Localization.Words word;
            public FindHatLambda(Localization.Words word)
            {
                this.word = word;
            }

            public bool Apply(Hat item)
            {
                return item.Name == word;
            }
        }

        public ColorScheme(Localization.Words skincolor, Localization.Words capecolor, Localization.Words capeoutlinecolor, Localization.Words hatname, Localization.Words beardname)
        {
            SkinColor = (ClrTextFx)Tools.Find(ColorSchemeManager.ColorList, new FindColorLambda(skincolor)).obj;
            CapeColor = (ClrTextFx)Tools.Find(ColorSchemeManager.CapeColorList, new FindColorLambda(capecolor)).obj;
            CapeOutlineColor = (ClrTextFx)Tools.Find(ColorSchemeManager.CapeOutlineColorList, new FindColorLambda(capeoutlinecolor)).obj;
            HatData = Tools.Find(ColorSchemeManager.HatInfo, new FindHatLambda(hatname));
            BeardData = Tools.Find(ColorSchemeManager.BeardInfo, new FindHatLambda(beardname));

            if (HatData == null) HatData = Hat.None;
            if (BeardData == null) BeardData = Hat.Vandyke;
        }
    }
}