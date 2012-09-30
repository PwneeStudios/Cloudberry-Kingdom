using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class BobPhsxBig : BobPhsxNormal
    {
        public override void Set(BobPhsx phsx)
        {
            if (phsx is BobPhsxWheel)
                phsx.ModInitSize = new Vector2(1.45f);
            else
                phsx.ModInitSize = new Vector2(1.7f, 1.4f);

            phsx.CapePrototype = Cape.CapeType.Normal;
            phsx.CapeOffset += new Vector2(0, 20);

            BobPhsxNormal normal = phsx as BobPhsxNormal;
            if (null != normal)
            {
                normal.Gravity *= 1.55f;
                normal.SetAccels();

                Gravity *= .935f;
                MaxSpeed *= 1.5f;
                XAccel *= 1.5f;
            }
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(0, 3, 0, 0);
            Name = "Sumo";
            Adjective = "fatty";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroIcon_Classic"), Color.White, 1.28f * DefaultIconWidth);
        }
        static readonly BobPhsxBig instance = new BobPhsxBig();
        public static new BobPhsxBig Instance { get { return instance; } }

        // Instancable class
        public BobPhsxBig()
        {
            Set(this);
        }
    }
}