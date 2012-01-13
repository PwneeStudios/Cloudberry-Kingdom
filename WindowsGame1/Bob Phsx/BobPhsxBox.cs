using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class BobPhsxBox : BobPhsxNormal
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Hero in a Box";
            NameTemplate = "hero in a box";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroImage_Box"), Color.White, DefaultIconWidth);
        }
        static readonly BobPhsxBox instance = new BobPhsxBox();
        public static BobPhsxBox Instance { get { return instance; } }

        // Instancable class
        bool InitializedAnim;

        public BobPhsxBox()
        {
            LandSound = Tools.SoundWad.FindByName("BoxHero_Land");
        }

        public override void DefaultValues()
        {
 	        base.DefaultValues();

            BobJumpAccel = (Gravity + 3.45f) / 19;
            BobInitialJumpSpeed = 6f;
            BobInitialJumpSpeedDucking = 6f;
            BobJumpLength = 18;
            BobJumpLengthDucking = 17;

            BobJumpAccel2 = (Gravity + 3.42f) / 19;
            BobInitialJumpSpeed2 = 14f;
            BobInitialJumpSpeedDucking2 = 12f;
            BobJumpLength2 = 17;
            BobJumpLengthDucking2 = 17;

            MaxSpeed = 13.6f;// 15f;
            XAccel = .45f;
            XFriction = .85f;
            BobXDunkFriction =
                        .65f;
        }

        public override void Init(Bob bob)
        {
            base.Init(bob);

            MyBob.JumpSound = Tools.SoundWad.FindByName("BoxHero_Jump");

            InitializedAnim = false;

            DefaultValues();

            OnGround = false;
            StartedJump = false;
            JumpCount = 0;
            FallingCount = 10000;

            Ducking = false;

            MyBob.PlayerObject.Read(4, 1);
        }

        public override void DuckingPhsx()
        {
            return;
        }

        protected virtual void ParentDoXAccel() { base.DoXAccel(); }
        public override void DoXAccel()
        {
            Ducking = true;
            base.DoXAccel();
            Ducking = false;
        }

        protected virtual void ParentGenerateInput(int CurPhsxStep) { base.GenerateInput(CurPhsxStep); }
        public override void GenerateInput(int CurPhsxStep)
        {
            base.GenerateInput(CurPhsxStep);

            if (MyBob.CurInput.xVec.X > 0)
                MyBob.CurInput.A_Button = true;
        }

        public override void AnimStep()
        {
            if (MyBob.PlayerObject.DestinationAnim() != 0 && OnGround || !InitializedAnim)
            {
                if (!InitializedAnim)
                {
                    MyBob.PlayerObject.AnimQueue.Clear();
                    InitializedAnim = true;
                }
                MyBob.PlayerObject.EnqueueAnimation(0, 0, false);
                MyBob.PlayerObject.DequeueTransfers();
            }

            if (MyBob.Core.Data.Velocity.Y > 10f && !OnGround && StartJumpAnim)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(2, 0.3f, false);
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= .85f;

                StartJumpAnim = false;
            }

            if (MyBob.Core.Data.Velocity.Y < -.1f && !OnGround && MyBob.PlayerObject.anim == 2 && MyBob.PlayerObject.LastAnimEntry.AnimSpeed > 0)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(2, .9f, false);
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= -1f;
            }

            MyBob.PlayerObject.PlayUpdate(1000f / 60f / 150f);
        }
    }
}