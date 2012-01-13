using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class BobPhsxBouncy : BobPhsxNormal
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Bouncy";
            NameTemplate = "bouncey bounce";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroImage_Bouncy"), Color.White, DefaultIconWidth);
        }
        static readonly BobPhsxBouncy instance = new BobPhsxBouncy();
        public static BobPhsxBouncy Instance { get { return instance; } }

        public override InteractWithBlocks MakePowerup()
        {
            return new Powerup_Bouncy();
        }

        // Instancable class
        bool InitializedAnim;

        public BobPhsxBouncy()
        {
        }

        public override void Init(Bob bob)
        {
            base.Init(bob);

            InitializedAnim = false;
            MyBob.PlayerObject.Read(24, 0);
        }

        public override void  DefaultValues()
        {
 	        base.DefaultValues();

            BlobMod = .4f;

            BobInitialJumpSpeedDucking = 24;
            BobInitialJumpSpeed = 40;
            BobJumpLength = 0;
            BobJumpAccel = 0;
        }

        public override void UpdateReadyToJump()
        {
            if (!OnGround && CurJump > 0)
                base.UpdateReadyToJump();
            else
                ReadyToJump = true;
        }

        //float FakeVel = 0;
        float SuperBounce = 18;
        int SuperBounceGraceCount = 0;
        int SuperBounceGrace = 10;
        public override void Jump()
        {
            SuperBounceGrace = 9;
            if (!OnGround && CurJump > 0)
            {
                // Allow for super bounce a few frames after hitting the ground
                if (MyLevel.PlayMode == 0)
                {
                    if (MyBob.CurInput.A_Button && SuperBounceGraceCount > 0)
                    {
                        //Tools.Write("Delayed super bounce!");
                        yVel += SuperBounce;
                        SuperBounceGraceCount = 0;
                    }
                    else
                        SuperBounceGraceCount--;
                }

                base.Jump();
            }
            else
            {
                UpdateReadyToJump();

                if (CanJump)
                {
                    DoJump();

                    if (MyBob.CurInput.A_Button)
                        yVel += SuperBounce;
                    else
                    {
                        if (MyLevel.PlayMode == 0)
                        {
                            //FakeVel = yVel + SuperBounce;
                            SuperBounceGraceCount = SuperBounceGrace;
                        }
                    }
                }
            }
        }

        public override void AnimStep()
        {
            if (!InitializedAnim)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(24, 0, false);
                MyBob.PlayerObject.DequeueTransfers();

                InitializedAnim = true;
            }

            if (MyBob.Core.Data.Velocity.Y > 10f && !OnGround && StartJumpAnim)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(24, 0, false);
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= .85f;

                StartJumpAnim = false;
            }

            MyBob.PlayerObject.PlayUpdate(1000f / 60f / 150f);
        }
    }
}