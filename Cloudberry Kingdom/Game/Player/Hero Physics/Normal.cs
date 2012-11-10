using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CoreEngine;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom
{
    public class BobPhsxNormal : BobPhsx
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(0, 0, 0, 0);
            Name = Localization.Words.ClassicHero;
            NameTemplate = "hero";
            
            //Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroIcon_Classic"), Color.White, DefaultIconWidth * 1.1f);
            Icon = new PictureIcon(Tools.TextureWad.FindByName("Bob_Run_0024"), Color.White, DefaultIconWidth * 1.2f);
        }
        static readonly BobPhsxNormal instance = new BobPhsxNormal();
        public static BobPhsxNormal Instance { get { return instance; } }

        // Instancable class
        bool InitializedAnim = false;


        protected EzSound LandSound = null;
        EzSound DoubleJump = BobPhsx.DefaultInfo.DoubleJump_Sound;
        EzSound ThrustSound = BobPhsx.DefaultInfo.BobJetpack_Sound;
        int ThrustSoundDelay = BobPhsx.DefaultInfo.BobJetpack_SoundDelay;
        int ThrustSoundCount;

        int RndMoveType, Offset;
        int RndThrustType;

        public override bool Sticky
        {
            get
            {
                if (Jumped || NoStickPeriod > 0)
                //if (JumpCount > 0 || Jumped || StartJumpAnim)
                    return false;
                else
                    return base.Sticky;
            }
        }

        public int BobFallDelay;
        public float XFriction, BobXDunkFriction;

        public int BobJumpLength, BobJumpLengthDucking;
        public float BobInitialJumpSpeed, BobInitialJumpSpeedDucking, BobJumpAccel2;

        public int BobJumpLength2, BobJumpLengthDucking2;
        public float BobInitialJumpSpeed2, BobInitialJumpSpeedDucking2, BobJumpAccel;

        public int BobEdgeJump; // 1 if computer waits until edge before jumping, and never walks off, 0 otherwise
        public int BobEdgeJumpLength; // Duration of BobEdgeJump

        public float JetPackAccel;
        public bool Thrusting, ReadyToThrust;

               

        public bool StartedJump = false;
        public int _JumpCount = 0;
        public int JumpCount { get { return _JumpCount; } set { _JumpCount = value; } }

        public override void KillJump()
        {
            base.KillJump();
            JumpCount = 0;
        }

        public int FallingCount;
        public bool ReadyToJump;

        /// <summary>
        /// The Y coordinate of the last jump apex
        /// </summary>
        public float ApexY;

        /// <summary>
        /// The number of frames since the last jump apex
        /// </summary>
        public int CountSinceApex;

        int AutoMoveLength, AutoMoveType, AutoStrafeLength, AutoSetToJumpLength, AutoSetToJumpType;
        int AutoDirLength, AutoDir, AutoDirLength_SetTo;
        int AutoFallOrJumpLength, AutoFallOrJump;

        public int NumJumps = 1;
        protected int CurJump;
        int JumpDelay = 10;
        int JumpDelayCount;
        //static int ComputerJumpDelay = 55;

        public bool JetPack = false;
        public int JetPackLength = 60;
        public int JetPackCushion = 12;

        /// <summary> The number of frames the jetpack has been activated, since last touchdown </summary>
        int JetPackCount;

        /// <summary> Y velocity must be less than this to jump </summary>
        float MaxVerticalSpeed_Jump = 23f;

        /// <summary> Y velocity must be less than this thrust with the jetpack </summary>
        float MaxVerticalSpeed_Thrust = 28f;

        protected bool StartJumpAnim;

        

        public BobPhsxNormal()
        {
            //LandSound = Tools.SoundWad.FindByName("Land");

            ThrustSound.MaxInstances = 8;
            this.JetPack = false;
            this.NumJumps = 1;

            DefaultValues();
        }

        public BobPhsxNormal(int NumJumps, bool JetPack)
        {
            this.JetPack = JetPack;
            this.NumJumps = NumJumps;

            ThrustSound.MaxInstances = 8;

            DefaultValues();
        }

        public void SetAccels() { SetAccels(19); }
        public void SetAccels(float JumpLength)
        {
            if (JumpLength > 0) BobJumpAccel = (Gravity + 3.45f) / JumpLength;
            if (JumpLength > 0) BobJumpAccel2 = (Gravity + 3.42f) / JumpLength;
            JetPackAccel = Gravity * 1.19f;
        }

        public override void DefaultValues()
        {
            Gravity = 2.95f;
            SetAccels();
            
            BobInitialJumpSpeed = 6f;
            BobInitialJumpSpeedDucking = 6f;
            BobJumpLength = 19;
            BobJumpLengthDucking = 17;

            
            BobInitialJumpSpeed2 = 14f;
            BobInitialJumpSpeedDucking2 = 12f;
            BobJumpLength2 = 18;
            BobJumpLengthDucking2 = 17;

            

            BobFallDelay = 5;
            BobXDunkFriction = .63f;

            MaxSpeed = 18.2f;
            XAccel = .53f * MaxSpeed / 17f;
            XFriction = .78f * MaxSpeed / 17f;
            MaxSpeed = 17.2f;


            // Slight faster
            MaxSpeed *= 1.04f;
            XAccel *= 1.1175f;
        }

        public override void Init(Bob bob)
        {
            base.Init(bob);

            Offset = int.MinValue;

            StartJumpAnim = false;

            OnGround = false;
            StartedJump = false;
            JumpCount = 0;
            FallingCount = 10000;

            Ducking = false;

            CurJump = 0;

            bob.MyCapeType = CapePrototype;
            if (CapePrototype == Cape.CapeType.None)
                bob.CanHaveCape = false;
        }

        public override bool CheckFor_xFlip()
        {
            bool flipped = base.CheckFor_xFlip();

            if (flipped && MyBob.PlayerObject.DestinationAnim() == 1)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(1, 0, true);
                MyBob.PlayerObject.DequeueTransfers();
            }

            return flipped;
        }

        public virtual void DuckingPhsx()
        {
            bool Down = MyBob.CurInput.xVec.Y < -.4f;
            if (Down)
                Ducking = true;
            else
                Ducking = false;

            if (Ducking) DuckingCount++;
            else DuckingCount = 0;

            // Correct ducking sprite offset
            if (DuckingCount == 2)
            {
                Vector2 shift = new Vector2(0, -30);

                Pos += shift;

                MyBob.Box.Target.Center += shift;
                MyBob.Box2.Target.Center += shift;
                MyBob.Box.Target.CalcBounds();
                MyBob.Box2.Target.CalcBounds();

                Obj.ParentQuad.Center.Move(Obj.ParentQuad.Center.Pos + shift);
                Obj.ParentQuad.Update();
                Obj.Update(null);

                if (MyBob.MyCape != null)
                    MyBob.MyCape.Move(shift);
            }
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            //Console.WriteLine("{0}  {1}", Pos, Vel);

            if (NoStickPeriod > 0)
                NoStickPeriod--;

            DuckingPhsx();

            DoXAccel();
            DoYAccel();

            if (OnGround) Jumped = false;

            if (OnGround && FallingCount > 0)
                FallingCount = 0;
            else
            {
                FallingCount++;
                CountSinceApex++;
            }            

            Jump();

            OnGround = false;
        }

        public Vector2 ThrustPos1;
        public Vector2 ThrustDir1;
        public Vector2 ThrustPos_Duck;
        public Vector2 ThrustDir_Duck;
        public Vector2 ThrustPos2;
        public Vector2 ThrustDir2;

        public override void PhsxStep2()
        {
            base.PhsxStep2();            

            // Rocketman thrust
            if (MyBob.Core.MyLevel.PlayMode == 0 && JetPack && Thrusting)
            {
                Vector2 Mod = new Vector2(MyBob.PlayerObject.xFlip ? -1 : 1, 1) *
                    MyBob.PlayerObject.ParentQuad.Size / new Vector2(260);

                int layer = Math.Max(1, MyBob.Core.DrawLayer - 1);

                float scale = .5f * (Math.Abs(Mod.X) - 1) + 1;

                if (Ducking)
                    ParticleEffects.Thrust(MyBob.Core.MyLevel, layer, Pos + Mod * ThrustPos_Duck, Mod * ThrustDir_Duck, Vel / 1.5f, scale);
                else
                    ParticleEffects.Thrust(MyBob.Core.MyLevel, layer, Pos + Mod * ThrustPos1, Mod * ThrustDir1, Vel / 1.5f, scale);

                if (ThrustType == RocketThrustType.Double)
                    ParticleEffects.Thrust(MyBob.Core.MyLevel, layer, Pos + Mod * ThrustPos2, Mod * ThrustDir2, Vel / 1.5f, scale);
            }
        }


        protected bool AutoAllowComputerToJumpOnLand = true;
        public virtual void UpdateReadyToJump()
        {
            if (AutoAllowComputerToJumpOnLand)
            {
                if ((MustHitGroundToReadyJump && OnGround || !MustHitGroundToReadyJump && true) &&
                    !MyBob.CurInput.A_Button &&
                    (DynamicLessThan(yVel, 0) || OnGround || CurJump < NumJumps) ||
                    (MyBob.Core.MyLevel.PlayMode != 0 || MyBob.CompControl))
                    ReadyToJump = true;
            }
            else
            {
                if ((MustHitGroundToReadyJump && OnGround || !MustHitGroundToReadyJump && true) &&
                    !MyBob.CurInput.A_Button &&
                    (DynamicLessThan(yVel, 0) || OnGround || CurJump < NumJumps))
                    ReadyToJump = true;
            }

            //if (MyBob.Core.MyLevel.PlayMode != 0 || MyBob.CompControl)
            //    if (NumJumps > 1 && !OnGround && CurJump > 0 && JumpDelayCount > -25)
            //        ReadyToJump = false;

            // Update ReadyToThrust
            if (JetPack && !OnGround && (!MyBob.CurInput.A_Button || !StartedJump))
                ReadyToThrust = true;
            else
                ReadyToThrust = false;
        }

        public bool CanJump
        {
            get
            {
                return ReadyToJump && yVel < MaxVerticalSpeed_Jump &&
                    (OnGround || FallingCount < BobFallDelay ||
                     CurJump < NumJumps && JumpDelayCount <= 0 && CurJump > 0);
            }
        }

        public bool ExternalPreventJump
        {
            get { return DisableJumpCount > 0; }
        }

        public virtual void Jump()
        {
            JumpDelayCount--;

            UpdateReadyToJump();

            if (ExternalPreventJump)
            {
                DisableJumpCount--;
                return;
            }

            if (CanJump && MyBob.CurInput.A_Button)
            {
                DoJump();
            }
        }

        /// <summary>
        /// For this many frames Bob will not stick to blocks
        /// (so that he can successfully jump off of fast upward moving blocks)
        /// </summary>
        int NoStickPeriod;
        protected virtual void DoJump()
        {
            // Track whether this jump is immediately from a placed object
            PlacedJump = NextJumpIsPlacedJump;
            NextJumpIsPlacedJump = false;

            if (OnGround || FallingCount < BobFallDelay + 5)
                CurJump = 0;

            Jumped = true;

            StartJumpAnim = true;

            CurJump++;
            JumpDelayCount = JumpDelay;

            ReadyToJump = false;

            FallingCount = 1000;

            float speed;
            if (CurJump == 1)
            {
                if (!Ducking)
                {
                    speed = BobInitialJumpSpeed;
                    JumpCount = BobJumpLength;
                }
                else
                {
                    speed = BobInitialJumpSpeedDucking;
                    JumpCount = BobJumpLengthDucking;
                }
            }
            else
            {
                if (!Ducking)
                {
                    speed = BobInitialJumpSpeed2;
                    JumpCount = BobJumpLength2;
                }
                else
                {
                    speed = BobInitialJumpSpeedDucking2;
                    JumpCount = BobJumpLengthDucking2;
                }
            }

            NoStickPeriod = 3;
            JumpCount = (int)(JumpCount * JumpLengthModifier);

            if (MyBob.Core.MyLevel.PlayMode == 0 && !MyBob.CharacterSelect)
            {
                PlayJumpSound();
            }

            if (Gravity > 0 && yVel > 0 && CurJump == 1
                ||
                Gravity < 0 && yVel < 0 && CurJump == 1)
            {
                float max = yVel + JumpAccelModifier * speed;
                float min = MaxJumpAccelMultiple * speed;
                yVel = CoreMath.Restrict(min, max, yVel);
            }
            else
                yVel = JumpAccelModifier * speed;
            StartedJump = true;

            JumpStartPos = Pos;
            ApexReached = false;
            ApexY = -20000000;

            IncrementJumpCounter();
        }

        protected virtual void PlayJumpSound()
        {
            if (CurJump > 1)
                DoubleJump.Play();
            else
            {
                if (MyBob.JumpSound == null)
                    Bob.JumpSound_Default.Play();
                else
                    MyBob.JumpSound.Play();
            }
        }

        public virtual float GetXAccel()
        {
            return XAccel;
        }

        public virtual void DoXAccel()
        {
            IceRun = false;

            float accel = GetXAccel();
            accel *= AccelMod;

            if (MyBob.CurInput.xVec.X < -.15f && xVel > -MaxSpeed)
            {
                if (xVel <= 0)
                    xVel += accel * MyBob.CurInput.xVec.X;
                else
                    if (!(Ducking && OnGround))
                        xVel += ReverseDirectionBoost * ReverseDirectionBoostMod * accel * MyBob.CurInput.xVec.X;

                if (DoFastTakeOff && xVel < 0 && xVel > -MaxSpeed / 10)
                    xVel = -MaxSpeed / 10;
            }
            if (MyBob.CurInput.xVec.X > .15f && xVel < MaxSpeed)
            {
                if (xVel >= 0)
                    xVel += accel * MyBob.CurInput.xVec.X;
                else
                    if (!(Ducking && OnGround))
                        xVel += ReverseDirectionBoost * ReverseDirectionBoostMod * accel * MyBob.CurInput.xVec.X;

                if (DoFastTakeOff && xVel > 0 && xVel < MaxSpeed / 10)
                    xVel = MaxSpeed / 10;
            }

            bool Run = false;
            if (Math.Abs(MyBob.CurInput.xVec.X) < .15f || !Run && Math.Abs(xVel) > MaxSpeed || (Ducking && (OnGround || FallingCount < 2)))
            {
                if (Ducking && (OnGround || FallingCount < 2) && Math.Abs(xVel) < 2) xVel = 0;

                float fric = XFriction;
                if (Ducking && (OnGround || FallingCount < 2)) fric = BobXDunkFriction;

                fric *= FricMod;

                if (Math.Abs(xVel) < 2)
                    xVel /= 12f;
                else
                {
                    float Prev_xVel = xVel;
                    xVel -= Math.Sign(xVel) * 7f / 4f * fric;

                    if (!Ducking)
                    {
                        // If we were above max speed do not reduce below max speed this frame if xVec.x > 0
                        if (Math.Abs(Prev_xVel) >= MaxSpeed && Math.Abs(xVel) < MaxSpeed)
                            xVel = Math.Sign(xVel) * MaxSpeed * .999f;
                    }
                }
            }
        }

        public virtual void DoYAccel()
        {
            Thrusting = false;
            if (JetPack && (ReadyToThrust || Thrusting))
            if (CurJump >= NumJumps && JumpDelayCount <= 0 || !OnGround && CurJump == 0 && FallingCount >= BobFallDelay)
                if (MyBob.CurInput.A_Button && JetPackCount < JetPackLength + JetPackCushion)
                {
                    if (MyBob.Core.MyLevel.PlayMode == 0)
                    {
                        if (ThrustSoundCount <= 0 && ThrustSound != null)
                        {
                            ThrustSoundCount = ThrustSoundDelay;
                            ThrustSound.PlayModulated(.02f);
                        }
                        ThrustSoundCount--;
                    }

                    Thrusting = true;
                    JetPackCount++;
                    float PowerLossRatio = (float)(JetPackLength + JetPackCushion - JetPackCount) / (float)JetPackCushion;
                    PowerLossRatio = Math.Min(1, PowerLossRatio);
                    
                    if (yVel < MaxVerticalSpeed_Thrust)
                        yVel += JetPackAccel * PowerLossRatio;
                }

            if (StartedJump && MyBob.CurInput.A_Button && JumpCount > 0)
            {
                if (CurJump == 1)
                    yVel += BobJumpAccel * (float)(JumpCount);
                else
                    yVel += BobJumpAccel2 * (float)(JumpCount);
                JumpCount -= 1;
            }
            else
                StartedJump = false;

            if (StartedJump || (FallingCount >= BobFallDelay || Ducking))
            {
                float HoldVel = yVel;

                // Only apply gravity if we haven't reached terminal velocity
                if (DynamicGreaterThan(yVel, BobMaxFallSpeed))
                    yVel -= Gravity;
                //if (yVel > BobMaxFallSpeed && Gravity > 0)
                //    yVel -= Gravity;
                //if (yVel < -BobMaxFallSpeed && Gravity < 0)
                //    yVel -= Gravity;

                if (Math.Sign(HoldVel) != Math.Sign(yVel))
                {
                    if (MyBob.OnApexReached != null) MyBob.OnApexReached(); MyBob.OnApexReached = null;

                    ApexReached = true;
                    ApexY = Pos.Y;
                    CountSinceApex = 0;
                }
            }

            //yVel = Math.Max(yVel, BobMaxFallSpeed);

            if (!Thrusting)
                ThrustSoundCount = 0;
        }

        public virtual void PlayLandSound()
        {
            LandSound.Play(.47f);
        }

        public override void LandOnSomething(bool MakeReadyToJump, ObjectBase ThingLandedOn)
        {
            base.LandOnSomething(MakeReadyToJump, ThingLandedOn);

            if (LandSound != null && MyBob.Core.MyLevel.PlayMode == 0 && ObjectLandedOn is BlockBase && !PrevOnGround)
                PlayLandSound();

            ReadyToJump = ReadyToJump || MakeReadyToJump;

            MyBob.BottomCol = true;

            OnGround = true;
            MyBob.PopModifier = 0;

            CurJump = 1;
            JetPackCount = 0;

            if (ObjectLandedOn is FlyingBlob)
                FallingCount = -4;

            if (ObjectLandedOn is BouncyBlock)
            {
                FallingCount = -1;
                ReadyToJump = false;
            }
        }

        public override void HitHeadOnSomething(ObjectBase ThingHit)
        {
            base.HitHeadOnSomething(ThingHit);

            JumpCount = 0;
        }

        public virtual float RetardxVec()
        {
            float RetardFactor = .01f * MyBob.Core.MyLevel.CurMakeData.GenData.Get(DifficultyParam.JumpingSpeedRetardFactor, Pos);
            if (!OnGround && xVel > RetardFactor * MaxSpeed)
                return 0;
            else
                return 1;
        }


        // Survival variables
        BlockBase SafetyBlock = null;
        int JumpCountdown = 0, TurnCountdown = 0, Dir = 0;

        public void GenerateInput_Survival(int CurPhsxStep)
        {
            MyBob.CurInput.A_Button = false;
            if (JumpCountdown == 0 || JumpCount > 0)
            {
                JumpCountdown = MyLevel.Rnd.RndInt(0, 20);//150);

                MyBob.CurInput.A_Button = true;
            }
            else
                JumpCountdown--;

            if (TurnCountdown == 0)
            {
                if (Dir == 0) Dir = 1;

                Dir *= -1;
                TurnCountdown = MyLevel.Rnd.RndInt(0, 135);
            }
            else
                TurnCountdown--;

            BlockBase block = ObjectLandedOn as BlockBase;
            if (null != block)
                SafetyBlock = block;

            if (SafetyBlock != null)
            {
                SafetyBlock.Box.CalcBounds();
                if (Pos.X > SafetyBlock.Box.TR.X - 70)
                    Dir = -1;
                if (Pos.X < SafetyBlock.Box.BL.X + 70)
                    Dir = 1;
            }
            else
                Dir = 0;

            MyBob.CurInput.xVec.X = Dir;
        }

        public float MinHeightAttained = 0, MinGroundHeightAttained;
        public void GenerateInput_Vertical(int CurPhsxStep)
        {
            // Track minimum position reached
            if (CurPhsxStep < 30) { MinGroundHeightAttained = MinHeightAttained = Pos.Y; }
            else
            {
                if (OnGround)
                    MinGroundHeightAttained = Math.Min(MinGroundHeightAttained, Pos.Y);
                MinHeightAttained = Math.Min(MinHeightAttained, Pos.Y);
            }

            MyBob.CurInput.A_Button = false;
            if (JumpCountdown == 0 ||
                ((JumpCount > 0 || !OnGround) && JumpCountdown < 60))
            {
                JumpCountdown = MyLevel.Rnd.RndInt(0, 20);//150);

                MyBob.CurInput.A_Button = true;
            }
            else
                JumpCountdown--;

            if (TurnCountdown <= 0)
            {
                if (Dir == 0) Dir = 1;

                Dir *= -1;
                TurnCountdown = MyLevel.Rnd.RndInt(0, 135);
            }
            else
                TurnCountdown--;

            Camera cam = MyBob.Core.MyLevel.MainCamera;
            float HardBound = 1000; float SoftBound = 1500;
            if (Pos.X > cam.TR.X - HardBound) Dir = -1;
            if (Pos.X < cam.BL.X + HardBound) Dir = 1;
            if (Pos.X > cam.TR.X - SoftBound && Dir == 1) TurnCountdown -= 2;
            if (Pos.X < cam.BL.X + SoftBound && Dir == -1) TurnCountdown -= 2;

            MyBob.CurInput.xVec.X = Dir;

            // Decide if the computer should want to land or not
            if (Geometry == LevelGeometry.Up)
            {
                MyBob.WantsToLand = (CurPhsxStep / 30) % 3 == 0;
                if (Vel.Y < -10) MyBob.WantsToLand = true;
            }
            if (Geometry == LevelGeometry.Down)
            {
                MyBob.WantsToLand = (CurPhsxStep / 50) % 4 == 0 &&
                    //Pos.Y < MinHeightAttained + 30;
                    Pos.Y < MinGroundHeightAttained - 350;

                if (Pos.Y > MinHeightAttained + 100)
                {
                    MyBob.CurInput.A_Button = false;
                    JumpCountdown += 120;
                }
            }
        }


        /// <summary> When true Bob aims to go as high as possible </summary>
        bool Up;

        void GenerateInput_Right(int CurPhsxStep)
        {
            Vector2 TR = MyBob.Core.MyLevel.MainCamera.TR;
            Vector2 BL = MyBob.Core.MyLevel.MainCamera.BL;

            SetTarget(GenData);


            ////////////////////////
            // Generate the input //
            ////////////////////////

            MyBob.CurInput.B_Button = false;

            if (Pos.Y < MyBob.TargetPosition.Y && ApexReached && !OnGround)
            {
                MyBob.CurInput.B_Button = true;
            }

            // BobEdgeJump, decide whether computer should always jump off edges
            if (BobEdgeJumpLength <= 0)
            {
                BobEdgeJumpLength = MyLevel.Rnd.RndInt(20, 80);
                if (BobEdgeJump == 1) BobEdgeJump = 0; else BobEdgeJump = 1;
                if (BobEdgeJump == 1)
                {
                    int Duration = GenData.Get(DifficultyParam.EdgeJumpDuration, Pos);
                    BobEdgeJumpLength = MyLevel.Rnd.Rnd.Next(Duration, 2 * Duration);
                }
                else
                {
                    int Duration = GenData.Get(DifficultyParam.NoEdgeJumpDuration, Pos);
                    if (Duration > 30)
                        BobEdgeJumpLength = MyLevel.Rnd.Rnd.Next(Duration, 2 * Duration);
                    else
                    {
                        BobEdgeJump = 1;
                        BobEdgeJumpLength = 100;
                    }
                }
            }
            else
                BobEdgeJumpLength--;

            if (MyLevel.Style.AlwaysEdgeJump)
                BobEdgeJump = 1;

            if (AutoFallOrJumpLength == 0)
            {
                if (AutoFallOrJump == 1) AutoFallOrJump = -1; else AutoFallOrJump = 1;
                if (AutoFallOrJump == 1)
                    AutoFallOrJumpLength = MyLevel.Rnd.Rnd.Next(GenData.Get(BehaviorParam.JumpLengthAdd, Pos)) + GenData.Get(BehaviorParam.JumpLengthBase, Pos);
                else
                    AutoFallOrJumpLength = MyLevel.Rnd.Rnd.Next(GenData.Get(BehaviorParam.FallLengthAdd, Pos)) + GenData.Get(BehaviorParam.FallLengthBase, Pos);
            }

            if (AutoDirLength == 0)
            {
                if (AutoDir == 1) AutoDir = -1; else AutoDir = 1;
                if (AutoDir == 1)
                    AutoDirLength = MyLevel.Rnd.Rnd.Next(GenData.Get(BehaviorParam.ForwardLengthAdd, Pos)) + GenData.Get(BehaviorParam.ForwardLengthBase, Pos);
                else
                    AutoDirLength = MyLevel.Rnd.Rnd.Next(GenData.Get(BehaviorParam.BackLengthAdd, Pos)) + GenData.Get(BehaviorParam.BackLengthBase, Pos);
                AutoDirLength_SetTo = AutoDirLength;

                // Get rid of spastic computer
                //if (AutoDirLength_SetTo < 7)
                //    if (MyLevel.Rnd.RndBool())
                //        AutoDirLength_SetTo = AutoDirLength = 10;
            }

            if ((MyBob.Core.MyLevel.GetPhsxStep() + Offset) % 400 == 0)
            {
                AutoMoveType = 0;
                AutoMoveLength = MyLevel.Rnd.Rnd.Next(GenData.Get(BehaviorParam.SitLengthAdd, Pos)) + GenData.Get(BehaviorParam.SitLengthBase, Pos);
            }
            if (AutoMoveLength == 0)
            {
                int rnd = MyLevel.Rnd.Rnd.Next(GenData.Get(BehaviorParam.MoveWeight, Pos) + GenData.Get(BehaviorParam.SitWeight, Pos));
                //if (rnd < GenData.Get(CompTweak.MoveWeight, Pos))                
                {
                    AutoMoveType = 1;
                    AutoMoveLength = MyLevel.Rnd.Rnd.Next(GenData.Get(BehaviorParam.MoveLengthAdd, Pos)) + GenData.Get(BehaviorParam.MoveLengthBase, Pos);
                }
                //else
                //{
                //    AutoMoveType = 0;
                //    AutoMoveLength = 60;// MyLevel.Rnd.Rnd.Next(GenData.Get(CompTweak.SitLengthAdd, Pos)) + GenData.Get(CompTweak.SitLengthBase, Pos);
                //}
            }

            if (AutoSetToJumpLength == 0)
            {
                int rnd = MyLevel.Rnd.Rnd.Next(GenData.Get(BehaviorParam.JumpWeight, Pos) + GenData.Get(BehaviorParam.NoJumpWeight, Pos));
                if (rnd < GenData.Get(BehaviorParam.JumpWeight, Pos))
                {
                    AutoSetToJumpType = 1;
                    AutoSetToJumpLength = MyLevel.Rnd.Rnd.Next(GenData.Get(BehaviorParam.JumpLengthAdd, Pos)) + GenData.Get(BehaviorParam.JumpLengthBase, Pos);
                }
                else
                {
                    AutoSetToJumpType = 0;
                    AutoSetToJumpLength = MyLevel.Rnd.Rnd.Next(GenData.Get(BehaviorParam.NoJumpLengthAdd, Pos)) + GenData.Get(BehaviorParam.NoJumpLengthBase, Pos);
                }
            }

            AutoMoveLength--;
            AutoStrafeLength--;
            AutoSetToJumpLength--;
            AutoDirLength--;
            AutoFallOrJumpLength--;

            if (AutoStrafeLength <= 0)
            {
                AutoStrafeLength = 60;
            }

            if (AutoMoveType == 1)
            {
                MyBob.CurInput.xVec.X = AutoDir;
                
                // Get rid of spastic computer
                //if (AutoDirLength_SetTo < 5)
                //    MyBob.CurInput.xVec.X = 0;
            }
            else
                MyBob.CurInput.xVec.X = 0;

            if (AutoSetToJumpType == 1 && AutoFallOrJump > 0 || JumpCount > 0)
                MyBob.CurInput.A_Button = true;


            if (Gravity > 0)
            {
                if (Pos.Y > MyBob.MoveData.MaxTargetY && OnGround ||
                    Pos.Y > MyBob.MoveData.MaxTargetY + 250 ||
                    Pos.Y > MyBob.TargetPosition.Y - 150 && JumpDelayCount < 2 && CurJump > 0)
                    MyBob.CurInput.A_Button = false;
            }
            if (Pos.Y < MyBob.MoveData.MinTargetY && AutoFallOrJump > 0)
            {
                MyBob.CurInput.A_Button = true;
            }

            if (AutoFallOrJump < 0)
                MyBob.CurInput.A_Button = false;


            // Jetpack extra
            if (JetPack)
            {
                float EngageJetpackHeight = MyBob.TargetPosition.Y + 250;
                if (Pos.Y < EngageJetpackHeight && JumpDelayCount < 2 && CurJump > 0 ||
                    AutoFallOrJump >= 0)
                {
                    MyBob.CurInput.A_Button = true;
                }
                if ((ReadyToThrust || Thrusting) && (AutoStrafeLength < 10 || Pos.Y > TR.Y - 150))
                    MyBob.CurInput.A_Button = false;
                if (yVel < -10 && Pos.Y > MyBob.TargetPosition.Y - 300)
                    MyBob.CurInput.A_Button = true;
            }

            if (Pos.X > MyBob.Core.MyLevel.CurMakeData.TRBobMoveZone.X ||
                Pos.Y > MyBob.Core.MyLevel.CurMakeData.TRBobMoveZone.Y)
            {
                MyBob.CurInput.A_Button = false;
                MyBob.CurInput.xVec.X = 0;
            }


            if (AutoMoveType == 0)
                MyBob.CurInput.A_Button = false;


            if (MyLevel.Style.AlwaysEdgeJump)
            {
                if (!Jumped)
                    MyBob.CurInput.A_Button = false;
                MyBob.CurInput.xVec.X = 1f;
            }


            // Better jump control: don't jump until edge of block
            if (Jumped)
            {
                MyBob.CurInput.xVec *= RetardxVec();

                if (yVel > 0 && Pos.Y < MyBob.TargetPosition.Y + 600)
                    MyBob.CurInput.A_Button = true;

                float MaxDip = 250; // How far below Target.Y we can go before needing to jump
                if (JetPack || NumJumps > 1) // Make higher for Jetman and Double jump
                    if (BobEdgeJump == 1)
                    { MaxDip = -100; if (yVel < 0) MaxDip = -450; }
                if (Pos.Y < MyBob.TargetPosition.Y - MaxDip)
                    MyBob.CurInput.A_Button = true;
            }
            BlockBase block = ObjectLandedOn as BlockBase;
            if (null != block && (OnGround || FallingCount < BobFallDelay) && (BobEdgeJump == 1 || block.Core.GenData.EdgeJumpOnly))
            {
                MyBob.Box.CalcBounds();
                float bobx = MyBob.Box.BL.X;
                float blockx = block.Box.TR.X;
                float DistancePast = GenData.Get(DifficultyParam.DistancePast, Pos);//5;
                float DistancePast_NoJump = GenData.Get(DifficultyParam.DistancePast_NoJump, Pos);//500;
                if (bobx > blockx + DistancePast || bobx >= blockx - 20 && FallingCount >= BobFallDelay - 2)
                    MyBob.CurInput.A_Button = true;
                else if (bobx > blockx - DistancePast_NoJump)
                    MyBob.CurInput.A_Button = false;
                else if (block.Core.GenData.EdgeJumpOnly)
                    MyBob.CurInput.A_Button = false;
            }

            // Better jump control: don't use full extent of jump
            int RetardJumpLength = GenData.Get(DifficultyParam.RetardJumpLength, Pos);
            if (!OnGround && RetardJumpLength >= 1 && JumpCount < RetardJumpLength && JumpCount > 1)
                MyBob.CurInput.A_Button = false;

            // Decide if the computer should want to land or not            
            if (Gravity > 0)
            {
                if ((ReadyToThrust || CanJump) && Pos.Y > BL.Y + 900)
                    MyBob.WantsToLand = Pos.Y < MyBob.TargetPosition.Y - 400;
                else
                    MyBob.WantsToLand = Pos.Y < MyBob.TargetPosition.Y + 200;
            }
            else
            {
                if ((ReadyToThrust || CanJump) && Pos.Y < TR.Y - 900)
                    MyBob.WantsToLand = Pos.Y > MyBob.TargetPosition.Y + 400;
                else
                    MyBob.WantsToLand = Pos.Y > MyBob.TargetPosition.Y - 200;
            }

            // Jetpack extra-extra
            if (JetPack)
            {
                switch (RndThrustType)
                {
                    case 0:
                        HighThrusts(CurPhsxStep);
                        break;
                    case 1:
                        MyBob.CurInput.A_Button = true;
                        break;
                    case 2:
                        break;
                }

                if (CurPhsxStep % 75 == 0)
                    RndThrustType = MyLevel.Rnd.Rnd.Next(0, 3);
            }

            // Masochistic
            if (MyLevel.Style.Masochistic)
            {
                if (Pos.Y < TR.Y - 400 && xVel > -2 && Pos.X > CurPhsxStep * (4000f / 800f))
                {
                    switch ((CurPhsxStep / 60) % 2)
                    {
                        case 0:
                            MyBob.CurInput.xVec.Y = -1;
                            if (yVel < 0)
                                MyBob.CurInput.A_Button = true;
                            MyBob.CurInput.xVec.X = 0;
                            break;
                        case 1:
                            break;
                    }
                }
                else
                {
                    MyBob.CurInput.xVec.X = 1;
                    MyBob.CurInput.xVec.Y = 0;
                    MyBob.CurInput.A_Button = true;
                }
            }


            // Don't land near the apex of a jump
            PreventEarlyLandings(GenData);
            
            // Double jump extra
            if (NumJumps > 1)
            {
                AutoAllowComputerToJumpOnLand = false;
                
                if (Jumped && CurJump > 0 && NumJumps > 1 && yVel < 0)
                {
                    switch ((CurPhsxStep / 60) % 3)
                    {
                        case 0:
                            if (CurJump < NumJumps && Pos.Y > MyBob.TargetPosition.Y - 600)
                                MyBob.WantsToLand = false;
                            if (yVel < -6)
                                MyBob.CurInput.A_Button = AutoAllowComputerToJumpOnLand = MyBob.WantsToLand;
                            break;

                        case 1:
                            MyBob.CurInput.A_Button = AutoAllowComputerToJumpOnLand = true;
                            break;

                        case 2:
                            MyBob.CurInput.A_Button = AutoAllowComputerToJumpOnLand = false;
                            break;
                    }
                }

                if (CurJump > 0 && Pos.Y > MyBob.TargetPosition.Y - 200)
                    MyBob.CurInput.A_Button = AutoAllowComputerToJumpOnLand = false;

                // n-jump hero
                if (CurJump < NumJumps && NumJumps > 2)
                {
                    if (yVel < -6)
                        MyBob.CurInput.A_Button = AutoAllowComputerToJumpOnLand = MyBob.WantsToLand;

                    if (MyBob.CurInput.A_Button)
                        MyBob.WantsToLand = false;

                    if (JumpCount > 1 && CurJump > 1)
                        MyBob.CurInput.A_Button = true;

                    if (Pos.Y < MyBob.TargetPosition.Y && CurJump < NumJumps)
                    {
                        MyBob.CurInput.A_Button = AutoAllowComputerToJumpOnLand = true;
                        MyBob.WantsToLand = false;
                    }
                }
            }

            // Always prevent jump if we are near the top
            if (Pos.Y > TR.Y - 150)
            {
                MyBob.CurInput.xVec.X = 1;
                MyBob.CurInput.xVec.Y = 0;
                MyBob.CurInput.A_Button = false;
            }
        }

        

private void HighThrusts(int CurPhsxStep)
{
                        switch ((CurPhsxStep / 60) % 4)
                        {
                            case 0:
                                if (JetPackCount > 5)
                                    MyBob.CurInput.A_Button = false;
                                if (MyBob.WantsToLand && JetPackCount < JetPackLength - 1 && yVel < 0)
                                    MyBob.CurInput.A_Button = true;
                                if (!MyBob.WantsToLand && JetPackCount < JetPackLength - 1 && yVel < 0)
                                    MyBob.CurInput.A_Button = true;
                                break;
                            case 1:
                                MyBob.CurInput.A_Button = true;
                                break;
                            case 2:
                                if (yVel < 0)
                                    MyBob.CurInput.A_Button = true;
                                break;
                            case 3:
                                break;
                        }
}protected virtual void SetTarget(RichLevelGenData GenData)
        {
            int Period = GenData.Get(BehaviorParam.MoveTypePeriod, Pos);
            float InnerPeriod = GenData.Get(BehaviorParam.MoveTypeInnerPeriod, Pos);
            float MinTargetY = MyBob.MoveData.MinTargetY;
            float MaxTargetY = MyBob.MoveData.MaxTargetY;

            float t = 0;
            int Step = MyBob.Core.MyLevel.GetPhsxStep() + Offset;

            // If this is the first phsx step choose a move type
            if (FirstPhsxStep)
            {
                RndThrustType = MyLevel.Rnd.Rnd.Next(0, 3);

                if (MyLevel.Rnd.RndFloat() < .5f)
                    RndMoveType = 10;
                else
                    RndMoveType = MyLevel.Rnd.Rnd.Next(0, 6);

                if (MyLevel.Style.AlwaysCurvyMove)
                    RndMoveType = 10;
            }
            //RndMoveType = 10; /// DANGER DANGER 

            ///////////////////////////////////////
            // Pick target for Bob to strive for //
            ///////////////////////////////////////
            bool AllowTypeSwitching = true; // Whether we can switch between move types
            switch (RndMoveType)
            {
                case 0: t = ((float)Math.Cos(Step / InnerPeriod) + 1) / 2; break;
                case 1: t = ((float)Math.Sin(Step / InnerPeriod) + 1) / 2; break;
                case 2:
                    InnerPeriod *= 3;
                    t = Math.Abs((Step % (int)InnerPeriod) / InnerPeriod);
                    break;
                case 3:
                    InnerPeriod *= 7f / 4f;
                    t = ((float)Math.Sin(Step / InnerPeriod) + 1) / 2;
                    break;
                case 4:
                    if ((Step / 100) % 2 == 0)
                        t = .275f;
                    else
                        t = .7f;
                    break;
                case 5:
                    if ((Step / 100) % 2 == 0)
                        t = .05f;
                    else
                        t = .3f;
                    break;
                case 6:
                    RndMoveType = 10;
                    break;
                case 10:
                    // Hard up and hard down.
                    AllowTypeSwitching = false;

                    if (FirstPhsxStep) Up = MyLevel.Rnd.RndBool();
                    if (Up)
                    {
                        MyBob.TargetPosition.Y = MaxTargetY;
                        if (MyBob.Pos.Y > MyBob.TargetPosition.Y - 200)
                            Up = false;
                    }
                    else
                    {
                        MyBob.TargetPosition.Y = MinTargetY;
                        if (MyBob.Pos.Y < MyBob.TargetPosition.Y + 200)
                            Up = true;
                    }
                    break;
                default:
                    // Do nothing. This fixes TargetPost.Y and creates straight levels.
                    break;
            }
            if (RndMoveType < 6)
                MyBob.TargetPosition.Y = MyBob.MoveData.MinTargetY + t * (MyBob.MoveData.MaxTargetY - MyBob.MoveData.MinTargetY);

            if (AllowTypeSwitching && MyBob.Core.MyLevel.GetPhsxStep() % Period == 0)
                RndMoveType = MyLevel.Rnd.Rnd.Next(0, 7);

        }

        protected virtual void PreventEarlyLandings(RichLevelGenData GenData)
        {
            int ApexWait = GenData.Get(DifficultyParam.ApexWait, Pos);
            if (!OnGround && (!ApexReached || CountSinceApex < ApexWait))
                MyBob.WantsToLand = false;
        }


        public override void GenerateInput(int CurPhsxStep)
        {
            // Initialize the offset value if it hasn't been set yet
            if (Offset == int.MinValue)
                Offset = MyLevel.Rnd.Rnd.Next(0, 300);

            base.GenerateInput(CurPhsxStep);

            switch (Geometry)
            {
                case LevelGeometry.Right:
                    GenerateInput_Right(CurPhsxStep);
                    break;

                case LevelGeometry.Down:
                case LevelGeometry.Up:
                    GenerateInput_Vertical(CurPhsxStep);
                    break;

                case LevelGeometry.OneScreen:
                    GenerateInput_Survival(CurPhsxStep);
                    break;
            }

            // Let go of A for 1 frame
            if (MyBob.PrevInput.A_Button && yVel < -5)
                MyBob.CurInput.A_Button = false;

            AdditionalGenerateInputChecks(CurPhsxStep);

#if DEBUG
            //CloudberryKingdomGame.debugstring = string.Format("{0}, {1}", JumpDelayCount, MyBob.CurInput.A_Button);
            //CloudberryKingdomGame.debugstring = string.Format("{0}, {1}", Pos.X, CurPhsxStep);
            CloudberryKingdomGame.debugstring = RndThrustType.ToString();
#endif
        }

        public float ForcedJumpDamping = 1;
        public override void DampForcedJump()
        {
            base.DampForcedJump();

            MyBob.NewVel *= ForcedJumpDamping;
        }



        public override void AnimStep()
        {
            if (MyBob.IsSpriteBased)
                SpriteAnimStep();
            else
                BezierAnimStep();
        }

        protected void BezierAnimStep()
        {
            base.AnimStep();

            if (!InitializedAnim)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                InitializedAnim = true;
                MyBob.PlayerObject.EnqueueAnimation(0, 0, false);
                MyBob.PlayerObject.DequeueTransfers();
            }

            
            // Falling animation
            if (!OnGround && !Ducking && !Jumped && MyBob.PlayerObject.DestinationAnim() != 3 && Math.Abs(xVel) < 4 &&
                DynamicLessThan(yVel, -15))
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(3, 0, true);
                MyBob.PlayerObject.AnimQueue.Peek().AnimSpeed *= .7f;
            }

            // ???
            if (!OnGround && !Ducking && !Jumped && MyBob.PlayerObject.DestinationAnim() != 2 &&
                DynamicLessThan(yVel, -15))
            {
                MyBob.PlayerObject.EnqueueAnimation(2, 0.3f, false);
                MyBob.PlayerObject.AnimQueue.Peek().AnimSpeed *= .7f;
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= .45f;
            }

            // Ducking animation
            if (Ducking && MyBob.PlayerObject.DestinationAnim() != 4)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(4, 0, false);
                MyBob.PlayerObject.AnimQueue.Peek().AnimSpeed *= 12;
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= 2.5f;
            }
            // Reverse ducking animation
            if (!Ducking && MyBob.PlayerObject.DestinationAnim() == 4)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                if (yVel > 0)
                    MyBob.PlayerObject.EnqueueAnimation(2, 0.3f, false);
                else
                    MyBob.PlayerObject.EnqueueAnimation(3, 0.3f, false);
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= 100f;
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
                        MyBob.PlayerObject.AnimQueue.Peek().AnimSpeed *= 20;
                        if (HoldDest == 1)
                            MyBob.PlayerObject.DequeueTransfers();
                    }
                }

            // Running animation
            if (!Ducking)
                if ((Math.Abs(xVel) >= 1f && OnGround)
                    && (MyBob.PlayerObject.DestinationAnim() != 1 || MyBob.PlayerObject.AnimQueue.Count == 0 || !MyBob.PlayerObject.Play || !MyBob.PlayerObject.Loop))
                {
                    {
                        MyBob.PlayerObject.AnimQueue.Clear();

                        if (OnGround)
                        {
                            MyBob.PlayerObject.EnqueueAnimation(1, 2.5f, true);
                            MyBob.PlayerObject.AnimQueue.Peek().AnimSpeed *= 2.5f;
                        }
                    }
                }

            // Jump animation
            if (!Ducking)
                if (ShouldStartJumpAnim())
                {
                    int anim = 2; float speed = .85f;
                    if (CurJump > 1) { anim = 29; speed = .002f; }

                    MyBob.PlayerObject.AnimQueue.Clear();
                    MyBob.PlayerObject.EnqueueAnimation(anim, 0.3f, false);
                    MyBob.PlayerObject.DequeueTransfers();
                    MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= speed;

                    StartJumpAnim = false;
                }
            // ???
            if (!Ducking)
                if (DynamicLessThan(yVel, -.1f) && !OnGround && MyBob.PlayerObject.anim == 2 && MyBob.PlayerObject.LastAnimEntry.AnimSpeed > 0)
                {
                    MyBob.PlayerObject.AnimQueue.Clear();
                    MyBob.PlayerObject.EnqueueAnimation(2, .9f, false);
                    MyBob.PlayerObject.DequeueTransfers();
                    MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= -1f;
                }

            float AnimSpeed = 1f;
            if (MyBob.PlayerObject.DestinationAnim() == 1 && MyBob.PlayerObject.Loop)
            {
                if (IceRun)
                    AnimSpeed = RunAnimSpeed * Math.Max(.35f, .1f * SameInputDirectionCount);
                else
                    AnimSpeed = RunAnimSpeed * Math.Max(.35f, .1f * Math.Abs(xVel));
            }

            if (MyBob.CharacterSelect)
                // Use time invariant update
                MyBob.PlayerObject.PlayUpdate(1000f * AnimSpeed * Tools.dt / 150f);
            else
                // Fixed speed update
                MyBob.PlayerObject.PlayUpdate(AnimSpeed * 17f/19f * 1000f / 60f / 150f);            
        }

        protected virtual void SpriteAnimStep()
        {
            // Falling animation
            if (!OnGround && !Ducking && MyBob.PlayerObject.DestinationAnim() != 3 &&
                DynamicLessThan(yVel, -15))
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(3, 0, false);
                MyBob.PlayerObject.DequeueTransfers();
            }

            // ???
            if (!OnGround && !Ducking && !Jumped && MyBob.PlayerObject.DestinationAnim() != 2 &&
                DynamicLessThan(yVel, -15))
            {
                MyBob.PlayerObject.EnqueueAnimation(3, 0, false);
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= .45f;
            }

            // Ducking animation
            if (Ducking && MyBob.PlayerObject.DestinationAnim() != 4)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(4, 1, false);
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= 2.5f;
            }
            // Reverse ducking animation
            if (!Ducking && MyBob.PlayerObject.DestinationAnim() == 4)
            {
                MyBob.PlayerObject.DoSpriteAnim = false;

                MyBob.PlayerObject.AnimQueue.Clear();
                if (yVel > 0)
                {
                    MyBob.PlayerObject.Read(2, 1);
                    MyBob.PlayerObject.EnqueueAnimation(2, 0, false);
                }
                else
                {
                    MyBob.PlayerObject.Read(3, 1);
                    MyBob.PlayerObject.EnqueueAnimation(3, 0, false);
                }
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= 2f;

                StartJumpAnim = false;
            }

            // Standing animation
            if (!Ducking)
                if ((Math.Abs(xVel) < 1f || IceRun) && OnGround && MyBob.PlayerObject.DestinationAnim() != 0 ||
                    MyBob.PlayerObject.DestinationAnim() == 2 && OnGround && DynamicLessThan(yVel, 0))
                {
                    if (!(MyBob.PlayerObject.DestinationAnim() == 5 && SameInputDirectionCount > 0))
                    if (!(IceRun && SameInputDirectionCount > 0 && OnGround))
                    {
                        int HoldDest = MyBob.PlayerObject.DestinationAnim();
                        MyBob.PlayerObject.AnimQueue.Clear();
                        MyBob.PlayerObject.EnqueueAnimation(0, 1, true);

                        MyBob.PlayerObject.DequeueTransfers();
                        if (HoldDest == 1)
                            MyBob.PlayerObject.DequeueTransfers();
                    }
                }

            // Running animation
            if (!Ducking)
            {
                // Slide
                if (!IceRun && Math.Abs(xVel) >= .35f && OnGround && Math.Sign(MyBob.CurInput.xVec.X) != Math.Sign(xVel)
                    && Math.Abs(MyBob.CurInput.xVec.X) > .35f)
                {
                    if (MyBob.PlayerObject.DestinationAnim() != 5)
                    {
                        MyBob.PlayerObject.EnqueueAnimation(5, 0, false);
                        MyBob.PlayerObject.DequeueTransfers();
                    }
                }
                else
                    
                if (((Math.Abs(xVel) >= .35f && !IceRun || SameInputDirectionCount > 0 && IceRun) && OnGround)
                    && (MyBob.PlayerObject.DestinationAnim() != 1 || MyBob.PlayerObject.AnimQueue.Count == 0 || !MyBob.PlayerObject.Play || !MyBob.PlayerObject.Loop))
                {
                    {
                        MyBob.PlayerObject.AnimQueue.Clear();

                        if (OnGround)
                        {
                            MyBob.PlayerObject.EnqueueAnimation(1, 0, true);
                            MyBob.PlayerObject.DequeueTransfers();
                        }
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

            float AnimSpeed = 1.5f;
            if (MyBob.PlayerObject.DestinationAnim() == 1 && MyBob.PlayerObject.Loop)
            {
                if (IceRun)
                {
                    //AnimSpeed = 1.29f * RunAnimSpeed * Math.Min(1f, .056f * SameInputDirectionCount);
                    AnimSpeed = 1.29f * RunAnimSpeed * Math.Min(1f, SameInputDirectionCount != 0 ? 1 : 0);
                }
                else
                    AnimSpeed = 1.29f * RunAnimSpeed * Math.Max(.35f, .1f * Math.Abs(xVel));
            }

            if (MyBob.CharacterSelect)
                // Use time invariant update
                //MyBob.PlayerObject.PlayUpdate(1000f * AnimSpeed * Tools.dt / 150f);
                Obj.PlayUpdate(Obj.LoadingRunSpeed);
            else
                // Fixed speed update
                Obj.PlayUpdate(1);

            //MyBob.PlayerObject.Read(0, 0);

            Obj.DoSpriteAnim = true;
            MyBob.InitBoxesForCollisionDetect();
        }

        /// <summary>
        /// Whether we should start playing the jump animation.
        /// </summary>
        public virtual bool ShouldStartJumpAnim()
        {
            return DynamicGreaterThan(yVel, 10f) && !OnGround && StartJumpAnim;
        }

        public virtual void SetDeathVel(Bob.BobDeathType DeathType)
        {
            if (MyBob.KillingObject != null)
            {
                switch(MyBob.KillingObject.Core.MyType)
                {
                    case ObjectType.Laser:
                        break;

                        //// Big explosion
                        //int sign = -Math.Sign(xVel);
                        //if (sign == 0) sign = MyLevel.Rnd.RndBit();

                        //Vel = new Vector2(170 * sign, 12);
                        //Acc = new Vector2(0, -1.9f);

                        //for (int i = 0; i < 3; i++)
                        //    Fireball.Explosion(Pos + MyLevel.Rnd.RndDir(60), MyBob.Core.MyLevel, .98f * Vel, 1, 1);
                        //return;

                    case ObjectType.Fireball:
                        MyBob.FlamingCorpse = true;

                        //// Throw Bob away from the fireball
                        //Vector2 Dir = Vector2.Normalize(Pos - MyBob.KillingObject.Core.Data.Position);
                        //if (float.IsNaN(Dir.X)) Dir = new Vector2(0, 1);
                        //Dir.Y = Math.Abs(Dir.Y);
                        //if (Dir.Y < .1f) Dir.Y = .1f;

                        Vector2 Dir = new Vector2(Math.Sign(Pos.X - MyBob.KillingObject.Core.Data.Position.X), 1);

                        Vel = Dir * 40;
                        Acc = new Vector2(0, -1.9f);

                        return;
                }
            }

            //Vel = new Vector2(0, 35);
            //Acc = new Vector2(0, -1.9f);

            Vel = new Vector2(0, 36);
            Acc = new Vector2(0, -2.2f);

            if (Bob.AllExplode)
                Acc = 1.35f * Acc;

            //Vel = new Vector2(0, 30);
            //Acc = new Vector2(0, -2.95f);
        }

        
        protected void Explode()
        {
            Fireball.Explosion(MyBob.Core.Data.Position, MyBob.Core.MyLevel, .1f * Vel, ExplosionScale, ExplosionScale / 1.4f);
            Fireball.Explosion(MyBob.Core.Data.Position, MyBob.Core.MyLevel, .1f * Vel, ExplosionScale, ExplosionScale / 1.4f);
            Tools.SoundWad.FindByName("DustCloud_Explode").Play(.4f);
        }

        public override void Die(Bob.BobDeathType DeathType)
        {
            base.Die(DeathType);

            ObjectClass obj = MyBob.PlayerObject;

            if (Bob.AllExplode)
            {
                Explode();
            }

            SetDeathVel(DeathType);            

            obj.AnimQueue.Clear();
            obj.EnqueueAnimation(5, 0, false, true);
            //obj.EnqueueAnimation("ToPieces", 0, false, true);
            //obj.EnqueueAnimation("ToPieces", 0, false, true);
            obj.DequeueTransfers();
            obj.DestAnim().AnimSpeed *= 1.85f;

            obj.DequeueTransfers();

            return;
        }

        public override void ToSprites(Dictionary<int, SpriteAnim> SpriteAnims, Vector2 Padding)
        {
            base.ToSprites(SpriteAnims, Padding);

            ObjectClass Obj = MyBob.PlayerObject;
            SpriteAnims.Add(0, Obj.AnimToSpriteFrames(0, 1, true, Padding));
            SpriteAnims.Add(1, Obj.AnimToSpriteFrames(1, 1, true, 1, 1, Padding));
            SpriteAnims.Add(2, Obj.AnimToSpriteFrames(2, 1, false, 1, 1, Padding));
            SpriteAnims.Add(4, Obj.AnimToSpriteFrames(4, 1, false, 1, 1, Padding));
            SpriteAnims.Add(5, Obj.AnimToSpriteFrames(5, 1, false, 1, 1, Padding));
        }

        public override void DollInitialize()
        {
            base.DollInitialize();

            Obj.anim = 1;
        }
    }
}