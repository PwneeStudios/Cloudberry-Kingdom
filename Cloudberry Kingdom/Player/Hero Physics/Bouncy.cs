using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class BobPhsxBouncy : BobPhsxNormal
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(3, 0, 0, 0);
            Name = Localization.Words.Bouncy;
            NameTemplate = "bouncey bounce";
            
            Icon = new PictureIcon(Tools.TextureWad.FindByName("Bob_Horse_0000"), Color.White, 1.35875f * DefaultIconWidth);
            ((PictureIcon)Icon).IconQuad.Quad.Shift(new Vector2(0, -.035f));
        }
        static readonly BobPhsxBouncy instance = new BobPhsxBouncy();
        public static new BobPhsxBouncy Instance { get { return instance; } }

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

            MyBob.JumpSound = Tools.SoundWad.FindByName("BouncyJump");
            DullSound = Tools.SoundWad.FindByName("BouncyJump_Small");
        }
        protected CoreSound DullSound = null;

        public override void DefaultValues()
        {
 	        base.DefaultValues();

            BlobMod = .4f;

            BobInitialJumpSpeedDucking = 24;
            BobInitialJumpSpeed = 40;
            BobJumpLength = 0;
            BobJumpAccel = 0;

            CapeOffset = new Vector2(0, 30);
        }

        public override void DuckingPhsx()
        {
            bool Down = MyBob.CurInput.xVec.Y < -.4f;
            if (Down)
                Ducking = true;
            else
                Ducking = false;
        }

        public override void UpdateReadyToJump()
        {
            if (!OnGround && CurJump > 0)
                base.UpdateReadyToJump();
            else
                ReadyToJump = true;
        }

        protected override void PlayJumpSound()
        {
            //base.PlayJumpSound();
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
						//if (MyLevel.PlayMode == 0 && !MyBob.CharacterSelect) MyBob.JumpSound.Play();
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
                    {
                        yVel += SuperBounce;
                        if (MyLevel.PlayMode == 0 && !MyBob.CharacterSelect) MyBob.JumpSound.Play();
                    }
                    else
                    {
                        if (MyLevel.PlayMode == 0 && !MyBob.CharacterSelect)
                        {
                            DullSound.Play();
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

            //if (MyBob.Core.Data.Velocity.Y > 10f && !OnGround && StartJumpAnim)
            if (ShouldStartJumpAnim())
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(24, 0, false);
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= .85f;

                StartJumpAnim = false;
            }

            if (MyBob.IsSpriteBased)
                MyBob.PlayerObject.PlayUpdate(1);
            else
                MyBob.PlayerObject.PlayUpdate(1000f / 60f / 150f);
        }
    }
}