using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Drawing;

namespace CloudberryKingdom
{
    public class ObjectIcon : IViewable
    {
        public virtual string[] GetViewables()
        {
            return new string[] { };
        }

        public bool Flipped = false;

        public static ObjectIcon RobotIcon, PathIcon, SlowMoIcon;
        public static ObjectIcon UncheckedIcon, CheckIcon, CheckpointIcon, RandomIcon, CustomIcon, CustomHoverIcon;
        public static Dictionary<Upgrade, ObjectIcon> UpgradeIcons;
        public static Dictionary<ObjectType, ObjectIcon> ObjIcons;

        public static ObjectIcon CreateIcon(Upgrade upgrade)
        {
            return UpgradeIcons[upgrade].Clone();
        }

        public static ObjectIcon CreateIcon(ObjectType obj)
        {
            return ObjIcons[obj].Clone();
        }

        public static void InitIcons()
        {
            UpgradeIcons = new Dictionary<Upgrade, ObjectIcon>();

            float StandardWidth = 161 * 1.31f;
            UpgradeIcons.Add(Upgrade.BouncyBlock, new PictureIcon("Bouncy blocks", "BouncyBlock1", Color.Lime, StandardWidth*.555f));
            UpgradeIcons.Add(Upgrade.Cloud, new PictureIcon("Clouds", "Icon_Cloud3", Color.LightGray, StandardWidth*1.135f));
            UpgradeIcons.Add(Upgrade.Elevator, new PictureIcon("Elevators", "Icon_Palette", Color.LightBlue, StandardWidth*.810f));
            UpgradeIcons.Add(Upgrade.FallingBlock, new PictureIcon("Falling blocks", "FallingBlock1", Color.Red, StandardWidth*.555f));
            UpgradeIcons.Add(Upgrade.FireSpinner, new PictureIcon("Fire spinners", "Icon_FireSpinner2", Color.Orange, StandardWidth*1.022f));
            UpgradeIcons.Add(Upgrade.SpikeyGuy, new PictureIcon("Spikey guys", "Icon_Spikey", Color.LightGray, StandardWidth*.835f));
            UpgradeIcons.Add(Upgrade.Pinky, new PictureIcon("Pinkies", "Pinky", Color.LightGray, StandardWidth*.835f));
            UpgradeIcons.Add(Upgrade.FlyBlob, new PictureIcon("Flying blobs", "Icon_Blob", Color.Lime, StandardWidth*1.056f, new Vector2(0, -45)));
            UpgradeIcons.Add(Upgrade.GhostBlock, new PictureIcon("Ghost blocks", "Icon_Ghost", Color.Lime, StandardWidth*1.148f, new Vector2(0, -80)));
            UpgradeIcons.Add(Upgrade.Laser, new PictureIcon("Lasers", "Icon_Laser2", Color.Red, StandardWidth*.72f));
            UpgradeIcons.Add(Upgrade.MovingBlock, new PictureIcon("Moving blocks", "Blue_Small", Color.LightBlue, StandardWidth*.62f));
            UpgradeIcons.Add(Upgrade.Spike, new PictureIcon("Spikes", "Icon_Spike2", Color.LightGray, StandardWidth*.99f));
            UpgradeIcons.Add(Upgrade.Fireball, new PictureIcon("Fireballs", "Icon_Fireball", Color.Orange, StandardWidth*.905f));
            UpgradeIcons.Add(Upgrade.Firesnake, new PictureIcon("Firesnake", "Icon_Firesnake", Color.Orange, StandardWidth * .905f));
            UpgradeIcons.Add(Upgrade.SpikeyLine, new PictureIcon("Spikey line", "Icon_SpikeyLine", Color.Orange, StandardWidth * .905f));

            //UpgradeIcons.Add(Upgrade.Jump, new PictureIcon("Jump level", "Jump", Color.Orange, StandardWidth * 1.07f));
            UpgradeIcons.Add(Upgrade.Jump, new PictureIcon("Jump difficulty", "Jump", Color.Orange, StandardWidth * 1.07f));
            UpgradeIcons.Add(Upgrade.Speed, new PictureIcon("Level speed", "SpeedIcon", Color.Orange, StandardWidth * 1.036f));
            UpgradeIcons.Add(Upgrade.Ceiling, new PictureIcon("Ceilings", "CeilingIcon", Color.Orange, StandardWidth * .9f));

            ObjIcons = new Dictionary<ObjectType,ObjectIcon>();
            ObjIcons.Add(ObjectType.FallingBlock, UpgradeIcons[Upgrade.FallingBlock]);
            ObjIcons.Add(ObjectType.MovingBlock, UpgradeIcons[Upgrade.MovingBlock]);
            ObjIcons.Add(ObjectType.GhostBlock, UpgradeIcons[Upgrade.GhostBlock]);
            ObjIcons.Add(ObjectType.FlyingBlob, UpgradeIcons[Upgrade.FlyBlob]);
            ObjIcons.Add(ObjectType.BouncyBlock, UpgradeIcons[Upgrade.BouncyBlock]);


            CheckIcon = new PictureIcon("Check", Color.Lime, StandardWidth * .85f);
            UncheckedIcon = new PictureIcon("Uncheck", Color.Lime, StandardWidth * .85f);
            

            CheckpointIcon = new PictureIcon("Icon_Checkpoint", Color.Lime, StandardWidth * .85f);
            RandomIcon = new PictureIcon("Unknown", Color.Lime, StandardWidth * 1.2f);
            CustomIcon = new PictureIcon("Gears", Color.Lime, StandardWidth * 1.35f);

            RobotIcon = new PictureIcon("Robot", Color.Lime, StandardWidth * 1.07f);
            PathIcon = new PictureIcon("Path", Color.Lime, StandardWidth * 1.07f);
            SlowMoIcon = new PictureIcon("SlowMo", Color.Lime, StandardWidth * 1.07f);
        }

        public QuadClass Backdrop;
        public Color BarColor;

        public string DisplayText;

        public FancyVector2 FancyPos = new FancyVector2();

        public Vector2 Pos { get { return FancyPos.RelVal; } set { FancyPos.RelVal = value; } }

        public OscillateParams MyOscillateParams;
        public ObjectIcon()
        {
            MyOscillateParams.Set(2f, 1.02f, .215f);

            Backdrop = new QuadClass(null, true);
            Backdrop.SetToDefault();
            Backdrop.TextureName = "Icon_Backdrop";
            Backdrop.ScaleYToMatchRatio(210);
        }

        public virtual void SetShadow(bool Shadow)
        {
        }

        public virtual void Fade(bool fade)
        {
        }

        public enum IconScale { Widget, Full, NearlyFull };
        public virtual ObjectIcon Clone(IconScale ScaleType)
        {
            ObjectIcon icon = new ObjectIcon();

            icon.DisplayText = DisplayText;

            return icon;
        }

        public virtual ObjectIcon Clone()
        {
            return Clone(IconScale.Full);
        }

        public float PrevSetRatio = 1;
        public virtual void SetScale(float Ratio)
        {
            PrevSetRatio = Ratio;
        }

        public virtual void Draw(bool Selected)
        {
            FancyPos.Update();
            Backdrop.Pos = FancyPos.AbsVal;
            //Backdrop.Draw();
        }

#if PC_VERSION
        public virtual bool HitTest(Vector2 pos)
        {
            return false;
        }
#endif
    }

    public class PictureIcon : ObjectIcon
    {
        public override string[] GetViewables()
        {
            return new string[] { };
        }

        public QuadClass IconQuad;

        public EzTexture IconTexture;
        public float NormalWidth;

        public PictureIcon(string DisplayText, string IconTextureString, Color BarColor, float Width)
        {
            this.DisplayText = DisplayText;
            Init(Tools.TextureWad.FindByName(IconTextureString), BarColor, Width);
        }
        public PictureIcon(string DisplayText, string IconTextureString, Color BarColor, float Width, Vector2 HitPadding)
        {
            this.DisplayText = DisplayText;
            this.HitPadding = HitPadding;
            Init(Tools.TextureWad.FindByName(IconTextureString), BarColor, Width);
        }
        public PictureIcon(string IconTextureString, Color BarColor, float Width)
        {
            Init(Tools.TextureWad.FindByName(IconTextureString), BarColor, Width);
        }
        public PictureIcon(EzTexture IconTexture, Color BarColor, float Width)
        {
            Init(IconTexture, BarColor, Width);
        }

        void Init(EzTexture IconTexture, Color BarColor, float Width)
        {
            this.IconTexture = IconTexture;
            this.BarColor = BarColor;
            this.NormalWidth = Width;

            IconQuad = new QuadClass(FancyPos, true);
            IconQuad.SetToDefault();
            IconQuad.Quad.MyTexture = IconTexture;
            IconQuad.ScaleYToMatchRatio(Width);

            IconQuad.Shadow = true;
            IconQuad.ShadowColor = new Color(.2f, .2f, .2f, 1f);
            IconQuad.ShadowOffset = new Vector2(12, 12);
        }

        public override void SetShadow(bool Shadow)
        {
            base.SetShadow(Shadow);

            IconQuad.Shadow = Shadow;
        }

        public override void Fade(bool fade)
        {
            base.Fade(fade);

            if (fade)
                IconQuad.Quad.SetColor(new Color(100, 100, 100));
            else
                IconQuad.Quad.SetColor(Color.White);
        }

        public override ObjectIcon Clone(IconScale ScaleType)
        {
            float width = NormalWidth;
            if (ScaleType == IconScale.Widget)
                width *= .3f;
            if (ScaleType == IconScale.NearlyFull)
                width *= .9f;

            PictureIcon icon = new PictureIcon(IconTexture, BarColor, width);
            icon.DisplayText = DisplayText;

            icon.HitPadding = HitPadding;

            return (ObjectIcon)icon;
        }

        public override void SetScale(float Ratio)
        {
            base.SetScale(Ratio);

            IconQuad.Scale(Ratio * NormalWidth / IconQuad.Size.X);
        }

        public override void Draw(bool Selected)
        {
            base.Draw(Selected);

            if (Selected)
            {
                Vector2 HoldSize = IconQuad.Size;
                IconQuad.Scale(MyOscillateParams.GetScale());
                //IconQuad.Scale(Oscillate.GetScale(SelectCount, 2f, 1.02f, .215f));
                IconQuad.Draw();
                IconQuad.Size = HoldSize;
            }
            else
            {
                // Flip if level is flipped
                if (Flipped)
                {
                    if (IconQuad.Base.e1.X > 0)
                        IconQuad.SizeX *= -1;
                }

                IconQuad.Draw();
                MyOscillateParams.Reset();
            }
        }

        public Vector2 HitPadding;
#if PC_VERSION
        public override bool HitTest(Vector2 pos)
        {
            return IconQuad.HitTest(pos, HitPadding) ||
                base.HitTest(pos);
        }
#endif
    }

    public class CustomHoverIcon : ObjectIcon
    {
        public override string[] GetViewables()
        {
            return new string[] { };
        }

        public QuadClass GearQuad, YQuad;

        public CustomHoverIcon()
        {
            YQuad = new QuadClass(FancyPos, true);
            YQuad.SetToDefault();
            YQuad.TextureName = "Controller_Y_Big";
            YQuad.ScaleYToMatchRatio(60);
            YQuad.Pos = new Vector2(60f, 0f);

            GearQuad = new QuadClass(FancyPos, true);
            GearQuad.SetToDefault();
            GearQuad.TextureName = "Gears";
            GearQuad.ScaleYToMatchRatio(82);
            GearQuad.Pos = new Vector2(-60.55469f, -16.66663f);
        }

        public override void Draw(bool Selected)
        {
            base.Draw(Selected);

            YQuad.Draw();
            GearQuad.Draw();            
        }
    }
}