using Microsoft.Xna.Framework;
using CoreEngine;

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

            phsx.CapePrototype = Cape.CapeType.None;
        }

        public static void SetDoubleObject(ObjectClass obj, BobPhsx phsx)
        {
            if (obj.QuadList != null)
            {
                // Angel wings.
                //obj.FindQuad("Wing1").Show = true;
                //obj.FindQuad("Wing2").Show = true;
                //obj.FindQuad("Wings").Show = true;
                obj.FindQuad("Wings_Front").Show = true;
                obj.FindQuad("Wings_Back").Show = true;
                obj.FindQuad("Wings_Front").MyEffect = Tools.HslGreenEffect;
                obj.FindQuad("Wings_Back").MyEffect = Tools.HslGreenEffect;

                BaseQuad quad = obj.FindQuad("Arm_Right");
                if (quad != null) quad.Show = false;
                else return;

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

            Specification = new HeroSpec(0, 0, 1, 0);
            Name = "Double Jump";
            Adjective = "double jump";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroIcon_Double"), Color.White, 1.1f * DefaultIconWidth * 286f / 240f);
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