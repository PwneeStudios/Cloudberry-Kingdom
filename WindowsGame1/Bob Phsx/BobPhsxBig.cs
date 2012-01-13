using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class BobPhsxBig : BobPhsxNormal
    {
        public override void Set(BobPhsx phsx)
        {
            phsx.ModInitSize = new Vector2(1.7f, 1.4f);
            phsx.CapePrototype = Cape.CapeType.Normal;

            BobPhsxNormal normal = phsx as BobPhsxNormal;
            if (null != normal)
            {
                normal.Gravity *= 1.55f;
            }
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Sumo";
            Adjective = "fatty";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroImage_Big"), Color.White, 1.1f * DefaultIconWidth);
        }
        static readonly BobPhsxBig instance = new BobPhsxBig();
        public static BobPhsxBig Instance { get { return instance; } }

        // Instancable class
        public BobPhsxBig()
        {
            Set(this);
        }
    }
}