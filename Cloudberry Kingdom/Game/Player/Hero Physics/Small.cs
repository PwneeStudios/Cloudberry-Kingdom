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
            phsx.ModInitSize = new Vector2(.6f) * modsize;
            
            phsx.CapePrototype = Cape.CapeType.Small;
            phsx.CapeOffset += new Vector2(0, -20);

            BobPhsxNormal normal = phsx as BobPhsxNormal;
            if (null != normal)
            {
                normal.Gravity *= .56f;
                normal.SetAccels();
                normal.ForcedJumpDamping = .85f;
            }
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(0, 1, 0, 0);
            Name = "Tiny Bob";
            Adjective = "tiny";
            //Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroIcon_Tiny"), Color.White, DefaultIconWidth);
            //Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroIcon_Classic"), Color.White, DefaultIconWidth * .6f);
            Icon = new PictureIcon(Tools.TextureWad.FindByName("Bob_Run_0024"), Color.White, DefaultIconWidth * .8f);
        }
        static readonly BobPhsxSmall instance = new BobPhsxSmall();
        new public static BobPhsxSmall Instance { get { return instance; } }

        // Instancable class
        public BobPhsxSmall()
        {
            Set(this);
        }
    }
}