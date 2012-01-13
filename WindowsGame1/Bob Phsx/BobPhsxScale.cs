using Microsoft.Xna.Framework;
using System;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class BobPhsxScale : BobPhsxNormal
    {
        public override void Set(BobPhsx phsx)
        {
            phsx.Oscillate = true;
            phsx.ForceDown = -7.5f;
            phsx.CapePrototype = Cape.CapeType.Normal;
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();
            
            Name = "Phase Bob";
            Adjective = "phasing";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroImage_Phase"), Color.White, DefaultIconWidth);
        }
        static readonly BobPhsxScale instance = new BobPhsxScale();
        public static BobPhsxScale Instance { get { return instance; } }

        // Instancable class
        public BobPhsxScale()
        {
            Set(this);
        }
    }
}