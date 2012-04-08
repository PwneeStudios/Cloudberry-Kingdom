using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Drawing;
using System;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public struct HeroSpec
    {
        public Hero_BaseType basetype;
        public Hero_Shape shape;
        public Hero_MoveMod move;

        public HeroSpec(Hero_BaseType basetype, Hero_Shape shape, Hero_MoveMod move)
        {
            this.basetype = basetype;
            this.shape = shape;
            this.move = move;
        }
    }

    public enum Hero_BaseType { Classic, Box, Wheel, Bouncy, Spaceship };
    public enum Hero_Shape { Classic, Small, Oscillate, Big };
    public enum Hero_MoveMod { Classic, Double, Jetpack };

    public class BobPhsx
    {
        protected LevelGeometry Geometry { get { return MyBob.Core.MyLevel.CurMakeData.PieceSeed.GeometryType; } }

        public static BobPhsx GetPhsx(Hero_BaseType BaseType)
        {
            switch (BaseType)
            {
                case Hero_BaseType.Classic: return BobPhsxNormal.Instance;
                case Hero_BaseType.Box: return BobPhsxBox.Instance;
                case Hero_BaseType.Bouncy: return BobPhsxBouncy.Instance;
                case Hero_BaseType.Wheel: return BobPhsxWheel.Instance;
                case Hero_BaseType.Spaceship: return BobPhsxSpaceship.Instance;
            }

            return null;
        }

        public static BobPhsx GetPhsx(Hero_Shape Shape)
        {
            switch (Shape)
            {
                case Hero_Shape.Small: return BobPhsxSmall.Instance;
                case Hero_Shape.Oscillate: return BobPhsxScale.Instance;
                case Hero_Shape.Big: return BobPhsxBig.Instance;
                case Hero_Shape.Classic: return BobPhsxNormal.Instance;
            }

            return null;
        }

        public static BobPhsx GetPhsx(Hero_MoveMod MoveMod)
        {
            switch (MoveMod)
            {
                case Hero_MoveMod.Double: return BobPhsxDouble.Instance;
                case Hero_MoveMod.Jetpack: return BobPhsxJetman.Instance;
                case Hero_MoveMod.Classic: return BobPhsxNormal.Instance;
            }

            return null;
        }

        public virtual void Set(BobPhsx phsx)
        {
        }

        public static BobPhsx MakeCustom(HeroSpec spec)
        {
            return MakeCustom(spec.basetype, spec.shape, spec.move);
        }

        public static BobPhsx MakeCustom(BobPhsx BaseType, BobPhsx Shape, BobPhsx MoveMod)
        {
            // Error catch. Spaceship can't be rocketman or double jump
            if (BaseType is BobPhsxSpaceship)
                MoveMod = BobPhsxNormal.Instance;

            // Make the phsx
            BobPhsx custom = BaseType.Clone();
            Shape.Set(custom);
            MoveMod.Set(custom);

            // Set the name
            if (BaseType is BobPhsxNormal && Shape is BobPhsxNormal && MoveMod is BobPhsxNormal)
                custom.Name = "Classic";
            else
            {
                string template = BaseType.NameTemplate;
                string adjective = Shape.Adjective;
                string adjective2 = MoveMod.Adjective;

                if (adjective.Length > 0) adjective += " ";
                if (adjective2.Length > 0) adjective2 += " ";

                custom.Name = adjective + adjective2 + template;
                custom.Name = custom.Name.Capitalize();
            }

            return custom;
        }

        public static BobPhsx MakeCustom(Hero_BaseType BaseType, Hero_Shape Shape, Hero_MoveMod MoveMod)
        {
            //"Fatty double jump hero in a box"
            //"Tiny spaceship"
            //"Oscillating  wheelie"

            return MakeCustom(GetPhsx(BaseType), GetPhsx(Shape), GetPhsx(MoveMod));
        }

        public virtual InteractWithBlocks MakePowerup()
        {
            return null;
        }

        public string Name = "None";
        public string Adjective = "";
        public string NameTemplate = "Hero";
        public ObjectIcon Icon;
        protected float DefaultIconWidth = 150;

        public Bob Prototype;
        public Cape.CapeType CapePrototype;

        bool SingletonInitialized = false;
        protected virtual void InitSingleton()
        {
            SingletonInitialized = true;
        }

        public Bob MyBob;
        public ObjectClass Obj { get { return MyBob.PlayerObject; } }

        public Camera Cam { get { return MyBob.Core.MyLevel.MainCamera; } }

        public Level MyLevel { get { return MyBob.Core.MyLevel; } }
        public ObjectData Core { get { return MyBob.Core; } }

        public RichLevelGenData GenData { get { return MyBob.Core.MyLevel.CurMakeData.GenData; } }

        public Vector2 Pos
        {
            get { return MyBob.Core.Data.Position; }
            set { MyBob.Core.Data.Position = value; }
        }
        public Vector2 ApparentVelocity
        {
            get { return Vel + new Vector2(GroundSpeed, 0); }
        }
        public Vector2 Vel
        {
            get { return MyBob.Core.Data.Velocity; }
            set { MyBob.Core.Data.Velocity = value; }
        }
        public float xVel
        {
            get { return MyBob.Core.Data.Velocity.X; }
            set { MyBob.Core.Data.Velocity.X = value; }
        }
        public float yVel
        {
            get { return MyBob.Core.Data.Velocity.Y; }
            set { MyBob.Core.Data.Velocity.Y = value; }
        }
        public Vector2 Acc
        {
            get { return MyBob.Core.Data.Acceleration; }
            set { MyBob.Core.Data.Acceleration = value; }
        }

        public virtual void DefaultValues() { }

        public float BlobMod = 1f;

        public bool Ducking;
        
        /// <summary>
        /// If true, then the player must first land, then release the A button, and then press A again to jump.
        /// </summary>
        public bool MustHitGroundToReadyJump = false;

        public float MaxSpeed, XAccel;

        public float Gravity;
        public float ForceDown = -1.5f;

        public float BobMaxFallSpeed = -29f;
        public bool OnGround, PrevOnGround, Jumped;
        public int AirTime = 0;

        public Vector2 JumpStartPos;
        public bool ApexReached;

        public bool DynamicLessThan(float val1, float val2)
        {
            return Gravity > 0 ? val1 < val2 : val1 > -val2;
        }

        public bool DynamicGreaterThan(float val1, float val2)
        {
            return Gravity > 0 ? val1 > val2 : val1 < -val2;
        }

        public virtual bool Sticky
        {
            get
            {
                return !OverrideSticky;
            }
        }

        public bool OverrideSticky = false;
        public float MaxJumpAccelMultiple = 1, JumpAccelModifier = 1;
        public float JumpLengthModifier = 1;
        public void ResetJumpModifiers()
        {
            OverrideSticky = false;
            MaxJumpAccelMultiple = JumpAccelModifier = JumpLengthModifier = 1;
        }

        /// <summary>
        /// Extra padding for when drawing the stickman to texture
        /// </summary>
        public Vector2 SpritePadding = Vector2.Zero;

        public virtual void ToSprites(Dictionary<int, SpriteAnim> SpriteAnims, Vector2 Padding)
        {
        }

        public virtual void Release()
        {
            MyBob = null;
        }

        public BobPhsx()
        {
            if (!SingletonInitialized)
                InitSingleton();
        }

        public virtual BobPhsx Clone()
        {
            return (BobPhsx)MemberwiseClone();
        }

        public virtual void KillJump() { }
        
        /// <summary>
        /// Called when an external force (such as a bouncy block) forces Bob toward a specific direction.
        /// </summary>
        public virtual void Forced(Vector2 Dir) { }

        public virtual void DampForcedJump()
        {
        }

        protected int DisableJumpCount;
        public virtual void DisableJump(int Length)
        {
            DisableJumpCount = Length;
        }

        public Vector2 ModInitSize = Vector2.One;
        public bool DoubleJumpModel = false;
        public bool JetpackModel = false;
        public virtual void Init(Bob bob)
        {
            MyBob = bob;

            if (Prototype != null && MyBob.PlayerObject != null)
            {
                Vector2 size = Prototype.PlayerObject.ParentQuad.Size;
                //Vector2 size = Prototypes.bob[BobPhsxNormal.Instance].PlayerObject.ParentQuad.Size;
                size *= ModInitSize;
                MyBob.PlayerObject.ParentQuad.Size = size;

                if (DoubleJumpModel) BobPhsxDouble.SetDoubleObject(MyBob.PlayerObject, this);
                if (JetpackModel) BobPhsxJetman.SetJetmanObject(MyBob.PlayerObject);
            }
        }

        float GroundSpeed;
        public virtual void Integrate()
        {
            GroundSpeed *= .925f;
            //if (Math.Abs(MyBob.GroundSpeed) > Math.Abs(GroundSpeed))
            if (OnGround || PrevOnGround)
                GroundSpeed = MyBob.GroundSpeed;

            Pos += Vel + new Vector2(GroundSpeed, 0);
        }

        public bool Oscillate = false;
        protected float ExplosionScale = 1.4f;
        protected float RunAnimSpeed = 1f;
        public float ScaledFactor = 1f;

        public float OscillateSize1 = .32f, OscillateSize2 = 2.08f, OscillatePeriod = 2 * 3.14159f;
        public float OscillateGravity1 = 2.534208f, OscillateGravity2 = 2.91155f;
        void OscillatePhsx()
        {
            float t = MyBob.Core.GetPhsxStep();
            float scale = Tools.Periodic(OscillateSize1, OscillateSize2, 30 * OscillatePeriod, t, 90);
            ScaledFactor = scale;

            Vector2 size = Prototype.PlayerObject.ParentQuad.Size * new Vector2(1.7f, 1.4f);
            size *= scale;
            MyBob.PlayerObject.ParentQuad.Size = size;

            Gravity = Tools.Periodic(OscillateGravity1, OscillateGravity2, 30 * OscillatePeriod, t, 90);
            RunAnimSpeed = 1f / ((scale - 1) * .16f + 1);
            ExplosionScale = 1.4f * ((scale - 1) * .5f + 1);

            Cape cape = MyBob.MyCape;
            if (cape != null)
            {
                cape.DoScaling = true;
                cape.ScaleCenter = cape.AnchorPoint[0];
                cape.Scale =
                    new Vector2(.975f * scale);
                    //new Vector2(.7f * scale);
            }
        }

        public Vector2 PrevVel, PrevPos;
        public virtual void PhsxStep()
        {
            if (OnGround) AirTime = 0;
            else AirTime++;

            if (MyBob.CurInput.A_Button) MyBob.Count_ButtonA++;
            else MyBob.Count_ButtonA = 0;

            if (Oscillate) OscillatePhsx();
        }
        public virtual void PhsxStep2()
        {
            CheckFor_xFlip();

            PrevOnGround = OnGround;
        }

        public void CopyPrev()
        {
            PrevVel = Vel;
            PrevPos = Pos + Vel;
        }

        /// <summary>
        /// True when the first phsx step hasn't finished yet.
        /// </summary>
        public bool FirstPhsxStep = true;


        /// <summary>
        /// Additional checks that should be performed at the end of the GenerateInput function.
        /// Typically these are things that are uniform across all BobPhsx types.
        /// </summary>
        protected void AdditionalGenerateInputChecks(int CurPhsxStep)
        {
            // Pause at the beginning
            if (CurPhsxStep < MyBob.ComputerWaitAtStartLength && MyBob.ComputerWaitAtStart)
            {
                MyBob.CurInput.xVec.X = 0;
                MyBob.CurInput.A_Button = false;
            }

            // Jump if the current block we're on says to jump
            if (ObjectLandedOn != null && OnGround && ObjectLandedOn.Core.GenData.JumpNow)
                MyBob.CurInput.A_Button = true;

            // Copy another bob's input
            //if (MyBob.MyPieceIndex > 0 && MyBob.MoveData.Copy >= 0)
            if (MyBob.MoveData.Copy >= 0)
            {
                MyBob.CurInput = MyBob.Core.MyLevel.Bobs[MyBob.MoveData.Copy].CurInput;
            }

            // Stay left of lowest bob
            //if (MyBob != MyLevel.LowestBob && MyLevel.LowestBob != null)
            //    if (MyBob.Pos.X > MyLevel.LowestBob.Pos.X - 30)
            //        Tools.Restrict(-1, 0, ref MyBob.CurInput.xVec.X);

            //if (MyBob != MyLevel.LowestBob && MyLevel.LowestBob != null)
            //    if (MyBob.Pos.X > MyLevel.LowestBob.Pos.X - 240)
            //        Tools.Restrict(-1, 0, ref MyBob.CurInput.xVec.X);

            FirstPhsxStep = false;
        }

        public virtual bool CheckFor_xFlip()
        {
            bool HoldFlip = MyBob.PlayerObject.xFlip;
            if (MyBob.CurInput.xVec.X > 0) MyBob.PlayerObject.xFlip = false;
            if (MyBob.CurInput.xVec.X < 0) MyBob.PlayerObject.xFlip = true;

            if (MyBob.MoveData.InvertDirX && MyBob.CurInput.xVec.X != 0) MyBob.PlayerObject.xFlip = !MyBob.PlayerObject.xFlip;

            return HoldFlip != MyBob.PlayerObject.xFlip;
        }

        public ObjectBase ObjectLandedOn;
        public virtual void LandOnSomething(bool MakeReadyToJump, ObjectBase ThingLandedOn)
        {
            ObjectLandedOn = ThingLandedOn;
        }
        public virtual void HitHeadOnSomething(ObjectBase ThingHit) { }

        public virtual void GenerateInput(int CurPhsxStep)
        {
            MyBob.WantsToLand_Reluctant =
                Pos.Y < MyBob.TargetPosition.Y + 1200;
            
            if (Pos.Y > MyLevel.MainCamera.TR.Y - 500)
                MyBob.WantsToLand_Reluctant = false;

            if (MyBob.WantsToLand) MyBob.WantsToLand_Reluctant = true;
        }

        public virtual void AnimStep()
        {
            CheckForAnimDone();
        }

        protected void CheckForAnimDone()
        {
            if (Obj.DonePlaying && MyBob.OnAnimFinish != null)
            {
                MyBob.OnAnimFinish();
                MyBob.OnAnimFinish = null;
            }
        }

        public virtual bool ReadyToPlace() { return false; }

        public virtual void IncrementJumpCounter()
        {
            if (!MyBob.CompControl && !MyBob.Core.MyLevel.Watching && MyBob.Core.MyLevel.PlayMode == 0)
            {
                MyBob.MyStats.Jumps++;

                // Check for Lots of Jumps awardment
                Awardments.CheckForAward_JumpAlot(MyBob);
            }
        }

        public int LastUsedStamp = 0;
        public virtual void SideHit(ColType side, BlockBase block)
        {
        }

        public virtual void Die(Bob.BobDeathType DeathType)
        {
        }

        public bool SkipInteraction(BlockBase block)
        {
            if (block.Core.MarkedForDeletion || !block.Core.Active || !block.IsActive || !block.Core.Real) return true;
            if (block.BlockCore.OnlyCollidesWithLowerLayers && block.Core.DrawLayer <= Core.DrawLayer) return true;
            return false;
        }

        public virtual void BlockInteractions()
        {
            foreach (BlockBase block in MyLevel.Blocks)
            {
                if (SkipInteraction(block)) continue;

                ColType Col = Phsx.CollisionTest(MyBob.Box, block.Box);

                if (Col != ColType.NoCol)
                    MyBob.InteractWithBlock(block.Box, block, Col);
            }
        }

        public virtual void BlockInteractions_Stage1()
        {
        }

        public bool PlacedJump = false, NextJumpIsPlacedJump = false;

        public virtual bool IsTopCollision(ColType Col, AABox box, BlockBase block)
        {
            return Col != ColType.NoCol && (Col == ColType.Top ||
                   Col != ColType.Bottom && Math.Max(MyBob.Box.Current.BL.Y, MyBob.Box.Target.BL.Y) > box.Target.TR.Y - Math.Max(-1.35 * Core.Data.Velocity.Y, 7));
        }

        public virtual bool IsBottomCollision(ColType Col, AABox box, BlockBase block)
        {
            return Col == ColType.Bottom ||
                Col != ColType.Bottom && Core.Data.Velocity.X != 0 && !OnGround && Math.Min(MyBob.Box.Current.TR.Y, MyBob.Box.Target.TR.Y) < box.Target.BL.Y + Math.Max(1.35 * Core.Data.Velocity.Y, 7);
        }

        public virtual void ModData(ref Level.MakeData makeData, StyleData Style)
        {
        }
    }
}
