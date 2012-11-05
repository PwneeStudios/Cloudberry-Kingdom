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
            phsx.CapeOffset += new Vector2(0, -20);

            BobPhsxNormal normal = phsx as BobPhsxNormal;
            if (null != normal)
            {
                normal.Gravity *= 1.55f;
                normal.SetAccels();

                normal.Gravity *= .935f;
                normal.MaxSpeed *= 1.5f;
                normal.XAccel *= 1.5f;
            }
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(0, 3, 0, 0);
            Name = Localization.Words.FatBob;
            Adjective = "Fatty";

            Icon = new PictureIcon(Tools.TextureWad.FindByName("Bob_Run_0024"), Color.White, DefaultIconWidth * 1.55f);
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