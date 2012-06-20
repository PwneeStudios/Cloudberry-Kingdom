using Microsoft.Xna.Framework;
using Drawing;

namespace CloudberryKingdom
{
    public class BobPhsxJetman : BobPhsxNormal
    {
        public override void Set(BobPhsx phsx)
        {
            BobPhsxNormal normal = phsx as BobPhsxNormal;
            if (null != normal)
            {
                normal.JetPack = true;
                normal.NumJumps = 1;
            }

            phsx.JetpackModel = true;
            phsx.CapePrototype = Cape.CapeType.None;
        }

        public static void SetJetmanObject(ObjectClass obj)
        {
            if (obj.QuadList != null)
            {
                obj.FindQuad("Rocket").Show = true;
            }
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Jetman";
            Adjective = "jetman";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroImage_Jetman"), Color.White, 1.2f * DefaultIconWidth);
        }
        static readonly BobPhsxJetman instance = new BobPhsxJetman();
        public static new BobPhsxJetman Instance { get { return instance; } }

        public override InteractWithBlocks MakePowerup()
        {
            return new Powerup_Jetpack();
        }

        // Instancable class
        public BobPhsxJetman()
        {
            Set(this);
        }
    }
}