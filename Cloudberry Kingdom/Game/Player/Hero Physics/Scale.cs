using Microsoft.Xna.Framework;
using System;


namespace CloudberryKingdom
{
    public class BobPhsxScale : BobPhsxNormal
    {
        public override void Set(BobPhsx phsx)
        {
            phsx.Oscillate = true;
            phsx.ForceDown = -7.5f;
            phsx.CapePrototype = Cape.CapeType.Normal;

            phsx.DollCamZoomMod = .535f;
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(0, 2, 0, 0);
            Name = Localization.Words.PhaseBob;
            Adjective = "phasing";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroIcon_Phase"), Color.White, 1.1f * DefaultIconWidth);
        }
        static readonly BobPhsxScale instance = new BobPhsxScale();
        public static new BobPhsxScale Instance { get { return instance; } }

        // Instancable class
        public BobPhsxScale()
        {
            Set(this);
        }
    }
}