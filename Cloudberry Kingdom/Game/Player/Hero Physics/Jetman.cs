using Microsoft.Xna.Framework;
using CoreEngine;

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

            phsx.CapePrototype = Cape.CapeType.None;

            if (phsx is BobPhsxWheel && null != normal)
            {
                phsx.JetpackModel = false;

                normal.ThrustPos1 = new Vector2(-141f, -75);
                normal.ThrustPos2 = new Vector2(141f, -75);
                normal.ThrustDir1 = new Vector2(-.4f, -.6f);
                normal.ThrustDir2 = new Vector2(.4f, -.6f);

                //normal.ThrustPos1 = new Vector2(-101f, -70);
                //normal.ThrustPos2 = new Vector2(127f, -70);
                //normal.ThrustDir1 = new Vector2(-.4f, -.6f);
                //normal.ThrustDir2 = new Vector2(.4f, -.6f);

                normal.ThrustType = RocketThrustType.Double;
            }
            else
            {
                phsx.JetpackModel = true;

                normal.ThrustPos1 = new Vector2(-45f, -70);
                normal.ThrustDir1 = new Vector2(-.115f, -1.1f);
                normal.ThrustType = RocketThrustType.Single;

                normal.ThrustPos_Duck = ThrustPos1 + new Vector2(-35, 20f);
                normal.ThrustDir_Duck = new Vector2(-.35f, -.8f);
            }
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

            Specification = new HeroSpec(0, 0, 2, 0);
            Name = "Jetman";
            Adjective = "jetman";

            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroIcon_Jetman"), Color.White, 1.1f * DefaultIconWidth);
            ((PictureIcon)Icon).IconQuad.Quad.Shift(new Vector2(-.25f, -.01f));
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