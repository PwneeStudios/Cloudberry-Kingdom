using Microsoft.Xna.Framework;
using Drawing;

namespace CloudberryKingdom
{
    public class BobPhsxDouble : BobPhsxNormal
    {
        public override void Set(BobPhsx phsx)
        {
            phsx.DoubleJumpModel = true;

            BobPhsxNormal normal = phsx as BobPhsxNormal;
            if (null != normal)
            {
                normal.JetPack = false;
                normal.NumJumps = 2;
            }
        }

        public static void SetDoubleObject(ObjectClass obj, BobPhsx phsx)
        {
            if (obj.QuadList != null)
            {
                obj.FindQuad("Arm_Right").Show = false;
                obj.FindQuad("Arm_Left").Show = false;
                bool feet = !(phsx is BobPhsxBox);
                {
                    obj.FindQuad("Foot_Left").Show = feet;
                    obj.FindQuad("Foot_Right").Show = feet;
                }
                obj.FindQuad("Leg_Left").Show = false;
                obj.FindQuad("Leg_Right").Show = false;
                obj.FindQuad("Body").Show = false;
            }
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Double Jump";
            Adjective = "double jump";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroImage_Double"), Color.White, .95f * DefaultIconWidth);
        }
        static readonly BobPhsxDouble instance = new BobPhsxDouble();
        public static new BobPhsxDouble Instance { get { return instance; } }

        public override InteractWithBlocks MakePowerup()
        {
            return new Powerup_DoubleJump();
        }

        // Instancable class
        public BobPhsxDouble()
        {
            Set(this);
        }
    }
}