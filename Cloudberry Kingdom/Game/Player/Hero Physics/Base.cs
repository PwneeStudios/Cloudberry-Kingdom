using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CoreEngine;
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
        public Hero_Special special;

        public HeroSpec(int basetype, int shape, int move, int special)
        {
            this.basetype = (Hero_BaseType)basetype;
            this.shape = (Hero_Shape)shape;
            this.move = (Hero_MoveMod)move;
            this.special = (Hero_Special)special;
        }

        public HeroSpec(Hero_BaseType basetype, Hero_Shape shape, Hero_MoveMod move)
        {
            this.basetype = basetype;
            this.shape = shape;
            this.move = move;
            this.special = Hero_Special.Classic;
        }

        public HeroSpec(Hero_BaseType basetype, Hero_Shape shape, Hero_MoveMod move, Hero_Special special)
        {
            this.basetype = basetype;
            this.shape = shape;
            this.move = move;
            this.special = special;
        }

        public static HeroSpec operator +(HeroSpec A, HeroSpec B)
        {
            return new HeroSpec(B.basetype == Hero_BaseType.Classic ? A.basetype : B.basetype,
                                B.shape == Hero_Shape.Classic ? A.shape : B.shape,
                                B.move == Hero_MoveMod.Classic ? A.move : B.move,
                                B.special == Hero_Special.Classic ? A.special : B.special);
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", (int)basetype, (int)shape, (int)move, (int)special);
        }
    }

    public enum Hero_BaseType { Classic, Box, Wheel, Bouncy, Spaceship, Meat, RocketBox };
    public enum Hero_Shape { Classic, Small, Oscillate, Big };
    public enum Hero_MoveMod { Classic, Double, Jetpack, Invert };
    public enum Hero_Special { Classic, Braid };

    public class BobPhsx
    {
#if DEBUG
        public void ResetInfo()
        {
            InitSingleton();
        }
#endif

        public static class DefaultInfo
        {
            public static EzSound DoubleJump_Sound = Tools.NewSound("Jump5", .1f);
            public static EzSound BobBoxJump_Sound = Tools.NewSound("BoxHero_Land", 1);
            public static EzSound BobJetpack_Sound = Tools.NewSound("Jetpack", .15f);
            public static int BobJetpack_SoundDelay = 5;
        }

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
                case Hero_BaseType.Meat: return BobPhsxMeat.Instance;
                case Hero_BaseType.RocketBox: return BobPhsxRocketbox.Instance;
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
                case Hero_MoveMod.Invert: return BobPhsxInvert.Instance;
                case Hero_MoveMod.Classic: return BobPhsxNormal.Instance;
            }

            return null;
        }

        public static BobPhsx GetPhsx(Hero_Special Special)
        {
            switch (Special)
            {
                case Hero_Special.Classic: return BobPhsxNormal.Instance;
                case Hero_Special.Braid: return BobPhsxBraid.Instance;
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

        public static BobPhsx MakeCustom(BobPhsx BaseType, BobPhsx Shape, BobPhsx MoveMod, BobPhsx Special)
        {
            // Error catch. Spaceship can't be rocketman or double jump
            if (BaseType is BobPhsxSpaceship)
                MoveMod = BobPhsxNormal.Instance;

            // Error catch. Invert must be classic, and must be the base class.
            if (MoveMod is BobPhsxInvert)
            {
                BaseType = BobPhsxInvert.Instance;
                MoveMod = BobPhsxNormal.Instance;
            }

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

            // Set the specificaiton for this hero.
            custom.Specification = BaseType.Specification + Shape.Specification + MoveMod.Specification;

            return custom;
        }

        public static BobPhsx MakeCustom(string BaseType, string Shape, string MoveMod, string Special)
        {
            int _BaseType, _Shape, _MoveMod, _Special;

            try
            {
                _BaseType = int.Parse(BaseType);
                _Shape = int.Parse(Shape);
                _MoveMod = int.Parse(MoveMod);
                _Special = int.Parse(Special);
            }
            catch
            {
                _BaseType = _Shape = _MoveMod = _Special = 0;
            }

            _BaseType = CoreMath.Restrict(0, Tools.Length<Hero_BaseType>() - 1, _BaseType);
            _Shape = CoreMath.Restrict(0, Tools.Length<Hero_Shape>() - 1, _Shape);
            _MoveMod = CoreMath.Restrict(0, Tools.Length<Hero_MoveMod>() - 1, _MoveMod);
            _Special = CoreMath.Restrict(0, Tools.Length<Hero_Special>() - 1, _Special);

            return MakeCustom(_BaseType, _Shape, _MoveMod);
        }

        public static BobPhsx MakeCustom(int BaseType, int Shape, int MoveMod)
        {
            return MakeCustom((Hero_BaseType)BaseType, (Hero_Shape)Shape, (Hero_MoveMod)MoveMod);
        }

        public static BobPhsx MakeCustom(int BaseType, int Shape, int MoveMod, int Special)
        {
            return MakeCustom((Hero_BaseType)BaseType, (Hero_Shape)Shape, (Hero_MoveMod)MoveMod, (Hero_Special)Special);
        }

        public static BobPhsx MakeCustom(Hero_BaseType BaseType, Hero_Shape Shape, Hero_MoveMod MoveMod)
        {
            return MakeCustom(GetPhsx(BaseType), GetPhsx(Shape), GetPhsx(MoveMod), BobPhsxNormal.Instance);
        }

        public static BobPhsx MakeCustom(Hero_BaseType BaseType, Hero_Shape Shape, Hero_MoveMod MoveMod, Hero_Special Special)
        {
            return MakeCustom(GetPhsx(BaseType), GetPhsx(Shape), GetPhsx(MoveMod), GetPhsx(Special));
        }
        
        public virtual InteractWithBlocks MakePowerup()
        {
            return null;
        }

        public enum CustomData
        {
            gravity, accel, maxspeed, maxfall, jumplength, jumplength2, jumpaccel, jumpaccel2,
            jetpackaccel, jetpackfuel, numjumps,
            size, size2, gravity2, phaseperiod,
            friction
        }

        public struct CustomPhsxData
        {
            public static void InitStatic()
            {
                _Bounds = new DataBounds[Length];
            }

            public struct DataBounds
            {
                public float DefaultValue, MinValue, MaxValue;
                public DataBounds(float DefaultValue, float MinValue, float MaxValue)
                {
                    this.DefaultValue = DefaultValue;
                    this.MinValue = MinValue;
                    this.MaxValue = MaxValue;
                }
            }

            static DataBounds[] _Bounds;
            public static DataBounds Bounds(CustomData type)
            {
                InitBounds();
                return _Bounds[(int)type];
            }
            public static DataBounds Bounds(int i)
            {
                InitBounds();
                return _Bounds[i];
            }

            static bool BoundsSet = false;
            static void InitBounds()
            {
                if (BoundsSet) return;
                BoundsSet = true;

                _Bounds[(int)CustomData.gravity] =      new DataBounds(1f, .5f, 2f);
                _Bounds[(int)CustomData.accel] =        new DataBounds(1f, .5f, 2f);
                _Bounds[(int)CustomData.maxspeed] =     new DataBounds(1f, .5f, 2f);
                _Bounds[(int)CustomData.maxfall] =      new DataBounds(1f, .33f, 3f);
                _Bounds[(int)CustomData.jumplength] =   new DataBounds(1f, .5f, 2f);
                _Bounds[(int)CustomData.jumplength2] =  new DataBounds(1f, .5f, 2f);
                _Bounds[(int)CustomData.jumpaccel] =    new DataBounds(1f, .5f, 2f);
                _Bounds[(int)CustomData.jumpaccel2] =   new DataBounds(1f, .5f, 2f);
                _Bounds[(int)CustomData.jetpackaccel] = new DataBounds(1f, .75f, 2f);
                _Bounds[(int)CustomData.jetpackfuel] =  new DataBounds(1f, .5f, 3f);
                _Bounds[(int)CustomData.numjumps] =     new DataBounds(1f, 2f, 4f);
                _Bounds[(int)CustomData.size] =         new DataBounds(1f, .2f, 2.1f);
                _Bounds[(int)CustomData.size2] =        new DataBounds(2.08f, .2f, 2.1f);
                _Bounds[(int)CustomData.gravity2] =     new DataBounds(1f, .5f, 2f);
                _Bounds[(int)CustomData.phaseperiod] =  new DataBounds(1f, .35f, 2f);
                _Bounds[(int)CustomData.friction] =     new DataBounds(1f, 0f, 3f);
            }

            float[] data;

            public static int Length = 16;

            public void Init()
            {
                data = new float[Length];
            }

            public void Init(params float[] vals)
            {
                data = new float[Length];

                Tools.Assert(vals.Length == data.Length);

                this[CustomData.gravity] = vals[0];
                this[CustomData.accel] = vals[1];
                this[CustomData.maxspeed] = vals[2];
                this[CustomData.maxfall] = vals[3];
                this[CustomData.jumplength] = vals[4];
                this[CustomData.jumplength2] = vals[5];
                this[CustomData.jumpaccel] = vals[6];
                this[CustomData.jumpaccel] = vals[7];
                this[CustomData.jetpackaccel] = vals[8];
                this[CustomData.jetpackfuel] = vals[9];
                this[CustomData.numjumps] = vals[10];
                this[CustomData.size] = vals[11];
                this[CustomData.size2] = vals[12];
                this[CustomData.gravity2] = vals[13];
                this[CustomData.phaseperiod] = vals[14];
                this[CustomData.friction] = vals[15];
            }

            public override string ToString()
            {
                string str = "ph:";

                for (int i = 0; i < data.Length; i++)
                {
                    str += data[i];
                    if (i + 1 < data.Length) str += ",";
                }
                str += ";";

                return str;
            }

            public void Init(string str)
            {
                // Break the data up by commas
                var terms = str.Split(',');

                Init();

                // Try and load the data into the array.
                try
                {
                    for (int i = 0; i < terms.Length; i++)
                    {
                        float v = float.Parse(terms[i]);
                        data[i] = CoreMath.Restrict(Bounds(i).MinValue, Bounds(i).MaxValue, v);
                    }
                }
                catch
                {
                    for (int i = 0; i < data.Length; i++)
                        data[i] = Bounds(i).DefaultValue;
                }
            }

            public float this[CustomData type]
            {
                get { return data[(int)type]; }
                set { data[(int)type] = value; }
            }
        }

        /// <summary>
        /// If true this BobPhsx has custom physic's parameters, such as gravity, friction, etc.
        /// </summary>
        public bool CustomPhsx = false;
        public CustomPhsxData MyCustomPhsxData;

        public void SetCustomPhsx(CustomPhsxData data)
        {
            CustomPhsx = true;
            MyCustomPhsxData = data;

            // Generic phsx
            Gravity *= data[CustomData.gravity];
            XAccel *= data[CustomData.accel];
            MaxSpeed *= data[CustomData.maxspeed];
            ModInitSize *= data[CustomData.size];
            ModCapeSize *= data[CustomData.size];

            // Wheelie phsx
            BobPhsxWheel wheel = this as BobPhsxWheel;
            if (null != wheel)
            {
                wheel.AngleAcc *= (float)Math.Pow(data[CustomData.accel], 1.5f);
                wheel.MaxAngleSpeed *= data[CustomData.maxspeed];
            }

            BobPhsxNormal normal = this as BobPhsxNormal;
            if (normal is BobPhsxNormal)
            {
                // Normal phsx
                BobMaxFallSpeed *= data[CustomData.maxfall];

                normal.BobJumpLength = (int)(normal.BobJumpLength * data[CustomData.jumplength]);
                normal.BobJumpLength2 = (int)(normal.BobJumpLength2 * data[CustomData.jumplength2]);
                normal.SetAccels(normal.BobJumpLength);
                normal.BobJumpAccel *= data[CustomData.jumpaccel];
                normal.BobJumpAccel2 *= data[CustomData.jumpaccel2];

                normal.XFriction *= data[CustomData.friction];

                // Jetpack phsx
                if (normal.JetPack)
                {
                    normal.JetPackAccel *= data[CustomData.jetpackaccel];
                    normal.JetPackLength = (int)(normal.JetPackLength * data[CustomData.jetpackfuel]);
                }

                // Double jump phsx
                if (normal.NumJumps > 1)
                {
                    normal.NumJumps = (int)(data[CustomData.numjumps]);
                }
            }

            // Phase phsx
            if (Oscillate)
            {
                OscillateSize1 = data[CustomData.size];
                OscillateSize2 = data[CustomData.size2];
                OscillateGravity1 *= data[CustomData.gravity];
                OscillateGravity2 *= data[CustomData.gravity2];
                OscillatePeriod *= data[CustomData.phaseperiod];
            }
        }

        public HeroSpec Specification;
        public string Name = "None";
        public int Id = -1;

        public string Adjective = "";
        public string NameTemplate = "Hero";
        public ObjectIcon Icon;
        protected float DefaultIconWidth = 150;

        public Bob Prototype;
        public Cape.CapeType CapePrototype;
        public Vector2 CapeOffset = Vector2.Zero;
        public Vector2 CapeOffset_Ducking = new Vector2(-20, 0);
        public Vector2 CapeGravity = new Vector2(0, -1.45f) / 1.45f;
        public Vector2 ModCapeSize = Vector2.One;
        public float DollCamZoomMod = 1f;

        public Vector2 HeroDollShift = Vector2.Zero;

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
        public int DuckingCount = 0;
        
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
        public enum RocketThrustType { None, Single, Double };
        public RocketThrustType ThrustType = RocketThrustType.None;
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
            float scale = CoreMath.Periodic(OscillateSize1, OscillateSize2, 30 * OscillatePeriod, t, 90);
            ScaledFactor = scale;

            Vector2 size = Prototype.PlayerObject.ParentQuad.Size * new Vector2(1.7f, 1.4f);
            size *= scale;
            MyBob.PlayerObject.ParentQuad.Size = size;

            Gravity = CoreMath.Periodic(OscillateGravity1, OscillateGravity2, 30 * OscillatePeriod, t, 90);
            RunAnimSpeed = 1f / ((scale - 1) * .16f + 1);
            ExplosionScale = 1.4f * ((scale - 1) * .5f + 1);

            Cape cape = MyBob.MyCape;
            if (cape != null)
            {
                cape.DoScaling = true;
                cape.ScaleCenter = cape.AnchorPoint[0];
                cape.Scale = new Vector2(.975f * scale);
            }
        }

        public float ReverseDirectionBoost = 2.7f;

        // Ice parameters
        public bool DoFastTakeOff = true;
        public float ReverseDirectionBoostMod = 1f;
        public float FricMod = 1f;
        public float AccelMod = 1f;
        public bool IceRun = false;

        void SetIceParams()
        {
            if (IceRun)
            {
                if (OnGround)
                {
                    AccelMod = .6f;
                    FricMod = .085f;
                    ReverseDirectionBoostMod = 0.3f;
                    DoFastTakeOff = false;
                }
                else
                {
                    AccelMod = .8325f;
                    FricMod = .435f;
                    ReverseDirectionBoostMod = 0.75f;
                    DoFastTakeOff = true;
                }
            }
            else
            {
                AccelMod = 1f;
                FricMod = 1f;
                ReverseDirectionBoostMod = 1f;
                DoFastTakeOff = true;
            }
        }


        public Vector2 PrevVel, PrevPos;
        public virtual void PhsxStep()
        {
            SetIceParams();

            if (OnGround) AirTime = 0;
            else AirTime++;

            if (MyBob.CurInput.A_Button) MyBob.Count_ButtonA++;
            else MyBob.Count_ButtonA = 0;

            if (Oscillate) OscillatePhsx();
        }

        public int SameInputDirectionCount = 0;
        public virtual void PhsxStep2()
        {
            if (Math.Sign(MyBob.PrevInput.xVec.X) == Math.Sign(MyBob.CurInput.xVec.X) && Math.Sign(MyBob.CurInput.xVec.X) != 0)
                SameInputDirectionCount++;
            else
                SameInputDirectionCount = 0;

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
            //        CoreMath.Restrict(-1, 0, ref MyBob.CurInput.xVec.X);

            //if (MyBob != MyLevel.LowestBob && MyLevel.LowestBob != null)
            //    if (MyBob.Pos.X > MyLevel.LowestBob.Pos.X - 240)
            //        CoreMath.Restrict(-1, 0, ref MyBob.CurInput.xVec.X);

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

        public virtual void ModLadderPiece(PieceSeedData piece)
        {
        }

        public virtual void OnInitBoxes()
        {
        }

        /// <summary>
        /// Do any initial actions needed for using this hero as a Doll.
        /// </summary>
        public virtual void DollInitialize()
        {
        }
    }
}
