using Microsoft.Xna.Framework;
using Drawing;
using System;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class BobPhsxMario : BobPhsxNormal
    {
        public override void Set(BobPhsx phsx)
        {
            Set(phsx, Vector2.One);
        }
        public void Set(BobPhsx phsx, Vector2 modsize)
        {
            //phsx.ModInitSize = 1.25f * new Vector2(.27f, .27f) * modsize;
            //phsx.CapePrototype = Cape.CapeType.Small;
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Mario";
            Adjective = "Mario";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroImage_Mario"), Color.White, 1.2f * DefaultIconWidth);
        }
        static readonly BobPhsxMario instance = new BobPhsxMario();
        public static new BobPhsxMario Instance { get { return instance; } }

        public override InteractWithBlocks MakePowerup()
        {
            return new Powerup_Jetpack();
        }

        // Instancable class
        public BobPhsxMario()
        {
            Set(this);
        }

        public override void AnimStep()
        {
            //base.AnimStep();

            // Falling animation
            if (!OnGround && !Ducking && MyBob.PlayerObject.DestinationAnim() != 3 &&
                DynamicLessThan(yVel, -15))
            //if (!OnGround && !Ducking && !Jumped && MyBob.PlayerObject.DestinationAnim() != 3 && Math.Abs(xVel) < 4 &&
            //    DynamicLessThan(yVel, -15))
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(3, 0, true);
                //MyBob.PlayerObject.AnimQueue.Peek().AnimSpeed *= .7f;
                MyBob.PlayerObject.DequeueTransfers();
            }

            // ???
            if (!OnGround && !Ducking && !Jumped && MyBob.PlayerObject.DestinationAnim() != 2 &&
                DynamicLessThan(yVel, -15))
            {
                MyBob.PlayerObject.EnqueueAnimation(3, 0, false);
                //MyBob.PlayerObject.AnimQueue.Peek().AnimSpeed *= .7f;
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= .45f;
            }

            // Ducking animation
            if (Ducking && MyBob.PlayerObject.DestinationAnim() != 4)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(4, 0, false);
                //MyBob.PlayerObject.AnimQueue.Peek().AnimSpeed *= 12;
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= 2.5f;
            }
            // Reverse ducking animation
            if (!Ducking && MyBob.PlayerObject.DestinationAnim() == 4)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                if (yVel > 0)
                    MyBob.PlayerObject.EnqueueAnimation(2, 0, false);
                else
                    MyBob.PlayerObject.EnqueueAnimation(3, 0, false);
                MyBob.PlayerObject.DequeueTransfers();
                //MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= 100f;
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= 200f;
            }

            // Standing animation
            if (!Ducking)
                if (Math.Abs(xVel) < 1f && OnGround && MyBob.PlayerObject.DestinationAnim() != 0 ||
                    MyBob.PlayerObject.DestinationAnim() == 2 && OnGround && DynamicLessThan(yVel, 0))
                {
                    {
                        int HoldDest = MyBob.PlayerObject.DestinationAnim();
                        MyBob.PlayerObject.AnimQueue.Clear();
                        MyBob.PlayerObject.EnqueueAnimation(0, 0, true);
                        //MyBob.PlayerObject.AnimQueue.Peek().AnimSpeed *= 20;
                        MyBob.PlayerObject.DequeueTransfers();
                        if (HoldDest == 1)
                            MyBob.PlayerObject.DequeueTransfers();
                    }
                }

            // Running animation
            if (!Ducking)
                if ((Math.Abs(xVel) >= .35f && OnGround)
                    && (MyBob.PlayerObject.DestinationAnim() != 1 || MyBob.PlayerObject.AnimQueue.Count == 0 || !MyBob.PlayerObject.Play || !MyBob.PlayerObject.Loop))
                {
                    {
                        MyBob.PlayerObject.AnimQueue.Clear();

                        if (OnGround)
                        {
                            MyBob.PlayerObject.EnqueueAnimation(1, 0, true);
                            //MyBob.PlayerObject.AnimQueue.Peek().AnimSpeed *= 2.5f;
                            MyBob.PlayerObject.DequeueTransfers();
                        }
                    }
                }

            // Jump animation
            if (!Ducking)
                if (ShouldStartJumpAnim())
                {
                    int anim = 2; float speed = .85f;
                    if (CurJump > 1) { anim = 29; speed = 1.2f; }

                    MyBob.PlayerObject.AnimQueue.Clear();
                    MyBob.PlayerObject.EnqueueAnimation(anim, 0, false);
                    MyBob.PlayerObject.DequeueTransfers();
                    MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= speed;

                    StartJumpAnim = false;
                }
            // ???
            //if (!Ducking)
            //    if (DynamicLessThan(yVel, -.1f) && !OnGround && MyBob.PlayerObject.anim == 2 && MyBob.PlayerObject.LastAnimEntry.AnimSpeed > 0)
            //    {
            //        MyBob.PlayerObject.AnimQueue.Clear();
            //        MyBob.PlayerObject.EnqueueAnimation(2, .9f, false);
            //        MyBob.PlayerObject.DequeueTransfers();
            //        MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= -1f;
            //    }

            float AnimSpeed = 1.5f;
            if (MyBob.PlayerObject.DestinationAnim() == 1 && MyBob.PlayerObject.Loop)
                AnimSpeed = RunAnimSpeed * Math.Max(.35f, .1f * Math.Abs(xVel));
            if (MyBob.CharacterSelect)
                // Use time invariant update
                MyBob.PlayerObject.PlayUpdate(1000f * AnimSpeed * Tools.dt / 150f);
            else
                // Fixed speed update
                MyBob.PlayerObject.PlayUpdate(AnimSpeed * 17f / 19f * 1000f / 60f / 150f * 1.285f);
        }
    }
}