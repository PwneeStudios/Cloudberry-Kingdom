using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class BobPhsxSmall : BobPhsxNormal
    {
        public override void Set(BobPhsx phsx)
        {
            Set(phsx, Vector2.One);
        }
        public void Set(BobPhsx phsx, Vector2 modsize)
        {
            //phsx.ModInitSize = new Vector2(.6f, .65f);
            phsx.ModInitSize = new Vector2(.6f, .65f) * modsize;// *.98f;
            phsx.CapePrototype = Cape.CapeType.Small;

            BobPhsxNormal normal = phsx as BobPhsxNormal;
            if (null != normal)
            {
                normal.Gravity *= .56f;

                normal.BobJumpLength = (int)(normal.BobJumpLength * 1.2f);
                normal.BobJumpAccel *= 1.11f;

                normal.ForcedJumpDamping = .85f;
            }
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Tiny Bob";
            Adjective = "tiny";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroImage_Tiny"), Color.White, DefaultIconWidth);
        }
        static readonly BobPhsxSmall instance = new BobPhsxSmall();
        public static BobPhsxSmall Instance { get { return instance; } }

        // Instancable class
        public BobPhsxSmall()
        {
            Set(this);
        }
    }
}