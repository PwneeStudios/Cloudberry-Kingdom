using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Goombas;
using CloudberryKingdom.Coins;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom.Bobs
{
    public struct BobInput
    {
        public bool A_Button, B_Button;
        public Vector2 xVec;

        public void Clean()
        {
            A_Button = B_Button = false;
            xVec = Vector2.Zero;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(A_Button);
            writer.Write(B_Button);
            writer.Write(xVec);
        }

        public void Read(BinaryReader reader)
        {
            A_Button = reader.ReadBoolean();
            B_Button = reader.ReadBoolean();
            xVec = reader.ReadVector2();
        }
    }
 
    public class BobLink
    {
        public int _j, _k;
        public Bob j, k;
        public float L, a_in, a_out, MaxForce;

        public void Release()
        {
            j = null;
            k = null;
        }

        public BobLink()
        {
            j = k = null;

            L = 0;
            a_out = 0;

            ////a_in = .005f;
            ////MaxForce = 5;

            a_in = .00525f;
            MaxForce = 5.15f;
        }

        public bool Inactive
        {
            get
            {
                // Don't draw the bungee if we are a dead spaceship or if we explode on death and are dead
                if ((Bob.AllExplode && !Bob.ShowCorpseAfterExplode) || j.Core.MyLevel.DefaultHeroType is BobPhsxSpaceship && (j.Dead || j.Dying || k.Dead || k.Dying))
                    return true;

                // Don't draw the bungee if one of the players isn't being drawn.
                if (!j.Core.Show || !k.Core.Show)
                    return true;

                return false;
            }
        }

        public void Draw()
        {
            if (Inactive) return;

            Draw(j.Core.Data.Position, k.Core.Data.Position);
        }

        public void Draw(Vector2 p1, Vector2 p2)
        {
            Tools.QDrawer.DrawLine(p1, p2, Color.WhiteSmoke, 15);
        }

        public void PhsxStep(Bob bob)
        {
            if (Inactive) return;

            float Length = (j.Core.Data.Position - k.Core.Data.Position).Length();
            
            Vector2 Tangent = (j.Core.Data.Position - k.Core.Data.Position);
            
            if (Length < 1) Tangent = Vector2.Zero;
            else Tangent /= Length;

            float Force;
            if (Length < L) Force = a_out * (Length - L);
            else Force = a_in * (Length - L);
            if (Math.Abs(Force) > MaxForce)
                Force = Math.Sign(Force) * MaxForce;

            Vector2 Bottom = Vector2.Min(j.Core.Data.Position, k.Core.Data.Position);
            if (bob.Core.Data.Position.Y > Bottom.Y)
                Force /= 5;

            Vector2 VectorForce = Force * Tangent;
            if (bob == j) VectorForce *= -1;

            Tangent = VectorForce;
            Tangent.Normalize();
            float v = Vector2.Dot(bob.Core.Data.Velocity, Tangent);
            if (v < 25)
                bob.Core.Data.Velocity += VectorForce;
        }

        public void Connect(Bob bob1, Bob bob2)
        {
            j = bob1;
            k = bob2;
            if (bob1.MyBobLinks == null) bob1.MyBobLinks = new List<BobLink>();
            if (bob2.MyBobLinks == null) bob2.MyBobLinks = new List<BobLink>();
            bob1.MyBobLinks.Add(this);
            bob2.MyBobLinks.Add(this);
        }
    }

    public class Bob : IObject
    {
        public float LightSourceFade = 1, LightSourceFadeVel = 0;
        public void ResetLightSourceFade()
        {
            LightSourceFade = 1;
            LightSourceFadeVel = 0;
        }
        public void SetLightSourceToFade()
        {
            LightSourceFadeVel = -.022f;
        }
        public void SetLightSourceToFadeIn()
        {
            LightSourceFadeVel = .022f;
            LightSourceFade = 0;
        }
        void DoLightSourceFade()
        {
            LightSourceFade += LightSourceFadeVel;
            Tools.Restrict(0, 1, ref LightSourceFade);
        }

        public void SetAnimation(string Name, float PlaySpeed)
        {
            SetAnimation(Name, 0, false, PlaySpeed);
        }
        public void SetAnimation(string Name, float StartT, bool Loop, float PlaySpeed)
        {
            PlayerObject.PlayUpdate(.01f);
            UpdateObject();

            PlayerObject.AnimQueue.Clear();
            PlayerObject.EnqueueAnimation(Name, Tools.RndFloat(0, 1), Loop, true, 1, PlaySpeed, false);
            PlayerObject.DequeueTransfers();

            PlayerObject.PlayUpdate(.01f);
            UpdateObject();
        }

        public void TransferToAnimation(string Name, float PlaySpeed)
        {
            TransferToAnimation(Name, 0, false, PlaySpeed);
        }
        public void TransferToAnimation(string Name, float StartT, bool Loop, float PlaySpeed)
        {
            PlayerObject.PlayUpdate(.01f);
            UpdateObject();

            PlayerObject.AnimQueue.Clear();
            PlayerObject.EnqueueAnimation(Name, Tools.RndFloat(0, 1), Loop, true, 1, PlaySpeed, false);

            PlayerObject.PlayUpdate(.01f);
            UpdateObject();
        }

        public bool Dopple = false;
        public Vector2 LastPlacedCoin;

        public static bool AllExplode = true;
        public static bool ShowCorpseAfterExplode = false;

        public void TextDraw() { }

        public BobPhsx MyHeroType;

        public bool FadingIn;
        public float Fade;

        public void SetToFadeIn()
        {
            FadingIn = true;
            Fade = 0;
        }

        public static int ImmortalLength = 55;
        public int ImmortalCountDown;
        public bool Moved;

        public ColorScheme MyColorScheme;
        public IObject HeldObject;

        public int HeldObjectIteration;

        public bool DrawOutline, CanHaveCape;
        public BobPhsx MyObjectType;

        public float NewY, NewVel, Xvel;

        public void Release()
        {
            ControlFunc = null;
            OnLand = null;
            OnApexReached = null;
            OnAnimFinish = null;

            AddedCoins = null;

            Core.Release();

            MyPiece = null;
            if (MyRecord != null) MyRecord.Release(); MyRecord = null;

            if (MyBobLinks != null)
                foreach (BobLink link in MyBobLinks)
                    link.Release();
            MyBobLinks = null;

            if (MyCape != null) MyCape.Release(); MyCape = null;

            if (MyPhsx != null) MyPhsx.Release(); MyPhsx = null;

            if (PlayerObject != null) PlayerObject.Release(); PlayerObject = null;

            if (Body != null) Body.Release(); Body = null;
            if (temp != null) temp.Release(); temp = null;
        }

        public void SetObject(ObjectClass obj, bool boxesOnly)
        {
            if (PlayerObject != null) PlayerObject.Release();

            PlayerObject = new ObjectClass(obj, BoxesOnly, false);
            Vector2 size = PlayerObject.BoxList[0].Size();
            float ratio = size.Y / size.X;
            int width = Tools.TheGame.Resolution.Bob.X;

            PlayerObject.FinishLoading();//Tools.QDrawer, Tools.Device, Tools.TextureWad, Tools.EffectWad, Tools.Device.PresentationParameters, width, (int)(width * ratio), false);

            Head = null;
            Body = null;
        }

        public void SetColorScheme(ColorScheme scheme)
        {
            if (BoxesOnly || PlayerObject.QuadList == null) return;

            if (scheme.HatData == null) scheme.HatData = Hat.None;

            if (MyHeroType is BobPhsxSpaceship)
                ;//scheme.HatData.QuadName = "None";
            else
            {
                PlayerObject.FindQuad("Head").Show = scheme.HatData.DrawHead;
                foreach (BaseQuad quad in PlayerObject.QuadList)
                {
                    if (quad.Name.Contains("Hat_"))
                    {
                        Quad _Quad = quad as Quad;
                        if (string.Compare(quad.Name, scheme.HatData.QuadName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            quad.Show = scheme.HatData.DrawSelf;

                            if (null != _Quad) _Quad.ShowChildren();
                        }
                        else
                        {
                            quad.Show = false;
                            if (null != _Quad) _Quad.HideChildren();
                        }
                    }
                }
            }
 
            PlayerObject.MySkinTexture = scheme.SkinColor.Texture;
            if (scheme.SkinColor.Effect == null)
                PlayerObject.MySkinEffect = Tools.EffectWad.EffectList[0];
            else
                PlayerObject.MySkinEffect = scheme.SkinColor.Effect;


            PlayerObject.OutlineColor = scheme.OutlineColor.Clr;
            PlayerObject.InsideColor = scheme.SkinColor.Clr;
            if (MyCape != null)
            {
                MyCape.MyColor = scheme.CapeColor.Clr;
                MyCape.MyOutlineColor = scheme.CapeOutlineColor.Clr;
                MyCape.MyQuad.Quad.MyTexture = scheme.CapeColor.Texture;
                MyCape.MyQuad.Quad.MyEffect = scheme.CapeColor.Effect;

                if (scheme.CapeColor.ModObject != null)
                    scheme.CapeColor.ModObject(this);
            }

            MyColorScheme = scheme;
        }

        public struct BobMove
        {
            public bool PlacePlatforms;
            public PlaceTypes PlaceObject;

            public float MaxTargetY, MinTargetY;

            public int Copy;

            /// <summary>
            /// If true the x acceleration is inverted
            /// </summary>
            public bool InvertDirX;

            public void Init()
            {
                MaxTargetY = 600;
                MinTargetY = -500;

                Copy = -1;
            }
        }

        public Vector2 CapeWind, Wind;

        public Hat MyHat;
        public Cape.CapeType MyCapeType;
        public Cape MyCape;
        public bool ShowCape;
        public Color InsideColor;
        
        ObjectVector temp = null;
        BendableQuad Body = null;
        Quad Head = null;

        public List<BobLink> MyBobLinks;

        public int SideHitCount;

        public bool CanInteract = true;

        public BobMove MoveData;

        public BobInput CurInput, PrevInput;
        public BobPhsx MyPhsx;

        /// <summary>
        /// Whether the computer wants to land on a potential block (For PlayMode == 2)
        /// </summary>
        public bool WantsToLand;

        /// <summary>
        /// Whether the computer would be willing to land but prefers not to.
        /// </summary>
        public bool WantsToLand_Reluctant;

        public Vector2 TargetPosition = Vector2.Zero;
        BlockEmitter LastLockedEmitter;

        public float GroundSpeed;

        public bool ComputerWaitAtStart;
        public int ComputerWaitAtStartLength = 0;

        public bool SaveNoBlock;
        public int PlaceDelay = 23;
        public int PlaceTimer;
        public PlaceTypes PlaceType;



        public bool Immortal, DoNotTrackOffScreen = false;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public bool TopCol, BottomCol;

        public FancyVector2 FancyPos;
        public bool CompControl, CharacterSelect, CharacterSelect2, Cinematic, DrawWithLevel = true, AffectsCamera = true;
        public int IndexOffset;
        public bool SuckedIn;
        public Seed SuckedInSeed;

        public int ControlCount = 0;

        /// <summary>
        /// A callback called when the Bob lands on something.
        /// </summary>
        public Action OnLand;

        /// <summary>
        /// A callback called when the Bob reaches his jump apex.
        /// </summary>
        public Action OnApexReached;
        

        public bool CodeControl = false;
        public Action<int> ControlFunc;
        public Action<int> CinematicFunc;
        public Action OnAnimFinish;

        public void SetCodeControl()
        {
            CodeControl = true;
            ControlCount = 0;
            ControlFunc = null;
        }

        public LevelPiece MyPiece;

        /// <summary>
        /// Which start position in the current level piece this Bob belongs to
        /// </summary>
        public int MyPieceIndex;

        /// <summary>
        /// If more than one Bob belongs to the same start position, this is the Bobs' ordering
        /// </summary>
        public int MyPieceIndexOffset;
        public ComputerRecording MyRecord;


        public bool Dying, Dead, FlamingCorpse;

        public bool BoxesOnly;

        public bool ScreenWrap, ScreenWrapToCenter, CollideWithCamera = true;

        public EzSound JumpSound, DieSound;
        public static EzSound JumpSound_Default, DieSound_Default;

        public PlayerIndex MyPlayerIndex;
        public PlayerData MyPlayerData
        {
            get 
            {
                return PlayerManager.Get(MyPlayerIndex);
            }
        }


        public bool TryPastTop;

        
        public ObjectClass PlayerObject;


        public Block LastCeiling = null;
        Vector2 LastCoinPos;

        List<Coin> AddedCoins;

        public int MinFall, MinDrop;

        public bool MakingLava, MakingCeiling;

        public AABox Box, Box2;

        public Vector2 Feet()
        {
            Box.CalcBounds();

            return new Vector2(Box.Current.Center.X, Box.BL.Y);
        }

        /// <summary>
        /// A list of boxes to allow for different difficulty levels for different obstacles.
        /// </summary>
        List<AABox> Boxes;
        int NumBoxes = 10;

        public AABox GetBox(int DifficultyLevel)
        {
            int index = Tools.Restrict(0, Boxes.Count - 1, DifficultyLevel);
            return Boxes[index];
        }

        /// <summary>
        /// A collision box corresponding to the normal size of Box2 during actual gameplay.
        /// </summary>
        public AABox RegularBox2;

        public void MakeNew()
        {
        }

        public static List<BobPhsx> HeroTypes = new List<BobPhsx>(new BobPhsx[]
            { BobPhsxNormal.Instance, BobPhsxJetman.Instance, BobPhsxDouble.Instance, BobPhsxSmall.Instance, BobPhsxWheel.Instance, BobPhsxSpaceship.Instance, BobPhsxBox.Instance,
                //});
                BobPhsxBouncy.Instance, BobPhsxRocketbox.Instance, BobPhsxBig.Instance, BobPhsxScale.Instance });

        /// <summary>
        /// How many time the bob has popped something without hitting the ground.
        /// </summary>
        public int PopModifier = 1;

        //public Bob(Bob bob, bool boxesOnly)
        public Bob(BobPhsx type, bool boxesOnly)
        {
            //MyHeroType = bob.MyHeroType;
            MyHeroType = type;
            Bob bob = type.Prototype;


            DrawOutline = bob.DrawOutline;
            CanHaveCape = bob.CanHaveCape;
            MyObjectType = bob.MyObjectType;

            //MyHat = new Hat();

            CoreData = new ObjectData();
            Core.DrawLayer = 6;
            Core.Show = true;

            BoxesOnly = boxesOnly;

            SetObject(bob.PlayerObject, BoxesOnly);

            Core.Data.Position = bob.Core.Data.Position;
            Core.Data.Velocity = bob.Core.Data.Velocity;
            PlayerObject.ParentQuad.Update();
            PlayerObject.Update(null);

            Box = new AABox(Core.Data.Position, PlayerObject.BoxList[1].Size() / 2);
            Box2 = new AABox(Core.Data.Position, PlayerObject.BoxList[2].Size() / 2);

            SetHeroPhsx(MyHeroType);

            SetColorScheme(bob.MyColorScheme);
        }

        public Bob(string file, EzEffectWad EffectWad, EzTextureWad TextureWad)
        {
            LoadFromFile(file, EffectWad, TextureWad, BobPhsxNormal.Instance);
        }
        public Bob(string file, EzEffectWad EffectWad, EzTextureWad TextureWad, BobPhsx MyHeroType)
        {
            LoadFromFile(file, EffectWad, TextureWad, MyHeroType);
        }
        public void LoadFromFile(string file, EzEffectWad EffectWad, EzTextureWad TextureWad, BobPhsx HeroType)
        {
            this.MyHeroType = HeroType;

            CoreData = new ObjectData();
            Core.Show = true;

            JumpSound = JumpSound_Default =
                    // Tools.SoundWad.FindByName("Jump");
                     Tools.SoundWad.FindByName("Jump5");
            JumpSound.DefaultVolume =
                //.2f;
                .1f;
            JumpSound.DelayTillNextSoundCanPlay = 10;

            DieSound = DieSound_Default = InfoWad.GetSound("BobDie_Sound");

            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);

            Vector2 size = new Vector2(1,2);
            float ratio = size.Y / size.X;
            int width = Tools.TheGame.Resolution.Bob.X;
            int height = (int)(width * ratio);            
            
            PlayerObject = new ObjectClass(Tools.QDrawer, Tools.Device, Tools.Device.PresentationParameters, width, height, EffectWad.FindByName("BasicEffect"), TextureWad.FindByName("White"));
            PlayerObject.ReadFile(reader, EffectWad, TextureWad);
            reader.Close();
            stream.Close();
            

            PlayerObject.OutlineWidth = new Vector2(1);//1150, 1150);
            PlayerObject.OutlineColor = Color.BlueViolet;
            PlayerObject.ParentQuad.Scale(new Vector2(260, 260) * 1.0f);
            PlayerObject.Read(0, 0);
            PlayerObject.Play = true;

            Core.Data.Position = new Vector2(100, 50);
            Core.Data.Velocity = new Vector2(0, 0);
            PlayerObject.ParentQuad.Update();
            PlayerObject.Update(null);
            //blobBox = new AABox(blob.Position, new Vector2((PlayerObject.BoxList[0].TR.Pos.X - PlayerObject.BoxList[0].BL.Pos.X) / 2, (PlayerObject.BoxList[0].TR.Pos.Y - PlayerObject.BoxList[0].BL.Pos.Y) / 2));
            Box = new AABox(Core.Data.Position, PlayerObject.BoxList[1].Size() / 2);
            Box2 = new AABox(Core.Data.Position, PlayerObject.BoxList[2].Size() / 2);
            //                new Vector2(blobTexture.Width/2, blobTexture.Height/2));

            MyPhsx = new BobPhsx();
            MyPhsx.Init(this);

            //PlayerObject.ConvertForSimple();
            SetColorScheme(ColorSchemeManager.ColorSchemes[0]);
        }

        public void InitAuto()
        {
            LastLockedEmitter = null;
        }

        public void SetToCinematic()
        {
            Cinematic = true;
            if (MyPhsx != null) MyPhsx.Release();
            MyPhsx = new BobPhsxCinematic();
            MyPhsx.Init(this);
            if (MyCape == null)
                MakeCape(Cape.CapeType.Normal);

            ControlCount = 0;
            ControlFunc = null;
        }

        public void EndCinematic()
        {
            if (!Cinematic) return;

            Move(new Vector2(0, 1));

            Cinematic = false;
            if (MyPhsx != null) MyPhsx.Release();
            SetHeroPhsx(MyHeroType);

            AffectsCamera = true;

            UpdateObject();
            InitBoxesForCollisionDetect();
            Box.SwapToCurrent();
            Box2.SwapToCurrent();
        }

        public void SwitchHero(BobPhsx hero)
        {
            Vector2 HoldVel = MyPhsx.Vel;

            if (MyCape != null) MyCape.Release(); MyCape = null;

            SetObject(hero.Prototype.PlayerObject, false);
            SetHeroPhsx(hero);

            if (MyCape != null) MyCape.Move(Pos);

            //MakeCape();

            SetColorScheme(PlayerManager.Get(this).ColorScheme);

            MyPhsx.Vel = HoldVel;
            
            //PhsxStep();
            AnimAndUpdate();
            //PhsxStep2();
        }

        /// <summary>
        /// When true the player can not move.
        /// </summary>
        public bool Immobile = false;

        public void SetHeroPhsx(BobPhsx type)
        {
            MyCapeType = Cape.CapeType.Normal;

            MyPhsx = type.Clone();
            //MyPhsx = new BobPhsxNormal();

            MyPhsx.Init(this);
            MakeCape(MyCapeType);
        }

        public void MakeCape(Cape.CapeType CapeType)
        {
            if (MyCape == null && !BoxesOnly && CanHaveCape)
            {
                MyCape = new Cape(this, CapeType);
                MyCape.Reset();
            }
        }

        public void SetPlaceAnimData()
        {
            if (BoxesOnly) return;

            PlayerObject.ImportAnimDataShallow(Prototypes.PlaceBob,
                                        Prototypes.PlaceBob.FindQuads("Arm_Left", "Arm_Right", "Hand_Left", "Hand_Right"),
                                        new List<string>(new string[] { "Stand", "Run", "Duck", "Jump", "Die", "JumpContinue" }));
        }

        public void UnsetPlaceAnimData()
        {
            ObjectClass regular = Prototypes.bob[BobPhsxNormal.Instance].PlayerObject;
            PlayerObject.ImportAnimDataShallow(regular,
                                        regular.FindQuads("Arm_Left", "Arm_Right", "Hand_Left", "Hand_Right"),
                                        new List<string>(new string[] { "Stand", "Run", "Duck", "Jump", "Die", "JumpContinue" }));
        }

        public void Init(bool BoxesOnly, PhsxData StartData, GameData game)
        {
            Core.Show = true;

            HeldObjectIteration = 0;

            BobPhsx type = game.DefaultHeroType;
            if (Core.MyLevel != null)
                type = Core.MyLevel.DefaultHeroType;
            MyHeroType = type;

            Core.DrawLayer = 6;

            if (CharacterSelect2)
            {
                MyPhsx = new BobPhsxCharSelect();
                MyPhsx.Init(this);
                MakeCape(Cape.CapeType.Normal);
            }
            else if (Cinematic)
                SetToCinematic();
            else
                SetHeroPhsx(type);

            ImmortalCountDown = ImmortalLength;
            Moved = false;
            //MyColorScheme = Generic.ColorSchemes[0];

            PlaceTimer = 0;

            GroundSpeed = 0;

            Dead = Dying = false;
                        
            Move(StartData.Position - Core.Data.Position);
            Core.StartData = Core.Data = StartData;


            if (PlayerObject == null)
            {
                //PlayerObject = new ObjectClass(Prototypes.bob[type].PlayerObject, BoxesOnly, false);
                PlayerObject = new ObjectClass(type.Prototype.PlayerObject, BoxesOnly, false);

                PlayerObject.FinishLoading(); //PlayerObject.FinishLoading(Tools.QDrawer, Tools.Device, Tools.TextureWad, Tools.EffectWad, Tools.Device.PresentationParameters, 200, 350);
                Vector2 size = PlayerObject.BoxList[0].Size();
                float ratio = size.Y / size.X;
                int width = Tools.TheGame.Resolution.Bob.X;
                int height = (int)(width * ratio);
                PlayerObject.FinishLoading(Tools.QDrawer, Tools.Device, Tools.TextureWad, Tools.EffectWad, Tools.Device.PresentationParameters, width, height);//Tools.Device.PresentationParameters.BackBufferWidth / 4, Tools.Device.PresentationParameters.BackBufferHeight / 4);
            }

            
            // Copy placebob's arm anim data
            if (MoveData.PlacePlatforms && !BoxesOnly && PlayerObject.QuadList != null)
            {
                SetPlaceAnimData();
            }

            PlayerObject.Read(0, 0);
            PlayerObject.Play = true;

            PlayerObject.ParentQuad.Update();
            PlayerObject.Update(null);

            Move(StartData.Position - Core.Data.Position);
            Core.Data = StartData;            
            Box.SetTarget(Core.Data.Position, Box.Current.Size);
            Box2.SetTarget(Core.Data.Position, Box2.Current.Size);
            Box.SwapToCurrent();
            Box2.SwapToCurrent();
            UpdateObject();

            Box.CalcBounds();
            Box2.CalcBounds();

            LastCoinPos = Core.Data.Position;


            if (MyCape != null)
            {
                MyCape.AnchorPoint[0] = Core.Data.Position;
                MyCape.Reset();
            }

            SetColorScheme(MyColorScheme);
        }

        public bool TemporaryPlaceBlock;
        bool ReadyToPlace = false;
        public void ButtonPhsx()
        {
            if (MoveData.PlacePlatforms && !TemporaryPlaceBlock)
            {
                PlaceTimer--;

                if (Core.GetPhsxStep() > 10)
                if (CurInput.A_Button && !PrevInput.A_Button && PlaceTimer <= 0 && ReadyToPlace)
                {
                    PlaceTimer = PlaceDelay;
                    SaveNoBlock = true;

                    HeldObjectIteration++;

                    Vector2 pos;
                    Vector2 offset;
                    Block NewBlock;
                    IObject NewObj = null;
                    switch (MoveData.PlaceObject)
                    {
                        case PlaceTypes.SuperBouncyBlock:
                            offset = new Vector2(2f * Core.Data.Velocity.X * (Math.Max(0, Core.Data.Velocity.Y / MyPhsx.Gravity) + 1f), 0);
                            offset.X -= 60 * Math.Sign(Core.Data.Velocity.X);
                            NewObj = NewBlock = MakeSuperBouncyBlock(offset);

                            Core.MyLevel.AddPop(NewBlock.Box.Current.Center);

                            break;

                        case PlaceTypes.BouncyBlock:
                            offset = new Vector2(2f * Core.Data.Velocity.X * (Math.Max(0, Core.Data.Velocity.Y / MyPhsx.Gravity) + 1f), 0);
                            offset.X -= 60 * Math.Sign(Core.Data.Velocity.X);
                            NewObj = NewBlock = MakeBouncyBlock(offset);

                            Core.MyLevel.AddPop(NewBlock.Box.Current.Center);

                            break;

                        case PlaceTypes.FallingBlock:
                            offset = new Vector2(2f * Core.Data.Velocity.X * (Math.Max(0, Core.Data.Velocity.Y / MyPhsx.Gravity) + 1f), 0);
                            offset.X -= 60 * Math.Sign(Core.Data.Velocity.X);
                            NewObj = NewBlock = MakeFallingBlock(offset);

                            Core.MyLevel.AddPop(NewBlock.Box.Current.Center);

                            break;

                        case PlaceTypes.GhostBlock:
                            offset = new Vector2(2f * Core.Data.Velocity.X * (Math.Max(0, Core.Data.Velocity.Y / MyPhsx.Gravity) + 1f), 0);
                            offset.X -= 60 * Math.Sign(Core.Data.Velocity.X);
                            NewObj = NewBlock = MakeGhostBlock(offset);

                            Core.MyLevel.AddPop(NewBlock.Box.Current.Center);

                            break;

                        case PlaceTypes.MovingBlock:
                            offset = new Vector2(2f * Core.Data.Velocity.X * (Math.Max(0, Core.Data.Velocity.Y / MyPhsx.Gravity) + 1f), 0);
                            offset.X -= 60 * Math.Sign(Core.Data.Velocity.X);
                            offset.Y -= 40;
                            MovingBlock MBlock = MakeMovingBlock2(offset);
                            NewObj = (IObject)MBlock;

                            Core.MyLevel.AddPop(MBlock.Box.Current.Center);

                            break;

                        case PlaceTypes.FlyingBlob:
                            pos = new Vector2(Box.Current.Center.X, Box.Current.BL.Y);
                            pos.X += 2f * Core.Data.Velocity.X * (Math.Max(0, Core.Data.Velocity.Y / MyPhsx.Gravity) + 1f);
                            pos.Y -= 135;

                            Goomba NewGoomba = (Goomba)Core.Recycle.GetObject(ObjectType.FlyingBlob, false);
                            NewObj = NewGoomba;

                            NewGoomba.Core.Data.Position = NewGoomba.Core.StartData.Position = pos;
                            NewGoomba.Period = 100;
                            NewGoomba.Offset = 0;
                            NewGoomba.Displacement = new Vector2(0, 0);
                            NewGoomba.UpdateObject();

                            Core.MyLevel.AddObject(NewGoomba);

                            Core.MyLevel.AddPop(NewGoomba.Core.Data.Position, 155);
                            break;

                        case PlaceTypes.NormalBlock:
                            pos = new Vector2(Box.Current.Center.X, Box.Current.BL.Y);
                            pos.X += 2f * Core.Data.Velocity.X * (Math.Max(0, Core.Data.Velocity.Y / MyPhsx.Gravity) + 1f);
                            NormalBlock NBlock = (NormalBlock)Core.Recycle.GetObject(ObjectType.NormalBlock, false);
                            NBlock.Init(pos, new Vector2(135, 100));
                            NBlock.Extend(Side.Top, Box.Target.BL.Y - 1f);
                            NBlock.Extend(Side.Bottom, NBlock.Box.Current.BL.Y - 100);

                            NewObj = (IObject)NBlock;

                            Core.MyLevel.AddPop(NBlock.Box.Current.Center, 155);
                            break;
                    }

                    NewBlock = NewObj as Block;
                    if (null != NewBlock)
                        Core.MyLevel.AddBlock(NewBlock);


                    if (NewObj != null)
                    {
                        NewObj.Core.GenData.Used = true;
                        NewObj.Core.RemoveOnReset = true;
                        NewObj.Core.Placed = true;
                    }

                    MyPhsx.NextJumpIsPlacedJump = true;
                }
            }

            TemporaryPlaceBlock = false;
        }




        public Vector2 PosToPlace()
        {
            int Dir = Math.Sign(Core.Data.Velocity.X);

            Vector2 pos = Vector2.Zero;
            if (Dir == 1) pos.X = Box.Target.TR.X + 75f / 1 - .01f;
            else if (Dir == -1) pos.X = Box.Target.BL.X - 75f / 1 + .01f;
            else pos.X = Box.Target.Center.X;
            pos.Y = Box.Target.BL.Y - 75f / 1 - .01f;

            return pos;
        }

        public Block MakeFallingBlock(Vector2 offset)
        {
            Vector2 pos = PosToPlace();

            FallingBlock NewBlock = new FallingBlock(false);
            int Life = 26;

            // Get FallingBlock parameters
            FallingBlock_Parameters Params = (FallingBlock_Parameters)Core.MyLevel.CurPiece.MyData.Style.FindParams(FallingBlock_AutoGen.Instance);

            Life = (int)Params.Delay.GetVal(Core.Data.Position);

            NewBlock.Init(pos + offset, new Vector2(75, 75), Life);
            NewBlock.BlockCore.BoxesOnly = BoxesOnly;

            return NewBlock as Block;
        }

        public Block MakeSuperBouncyBlock(Vector2 offset)
        {
            Vector2 pos = PosToPlace();

            BouncyBlock NewBlock = new BouncyBlock(false);

            // Get BouncyBlock parameters
            BouncyBlock_Parameters Params = (BouncyBlock_Parameters)Core.MyLevel.CurPiece.MyData.Style.FindParams(BouncyBlock_AutoGen.Instance);

            int Speed = 75;// 100;

            offset.Y -= 90;
            NewBlock.Init(pos + offset, new Vector2(125, 125), Speed);
            NewBlock.BlockCore.BoxesOnly = BoxesOnly;

            return NewBlock as Block;
        }

        public Block MakeBouncyBlock(Vector2 offset)
        {
            Vector2 pos = PosToPlace();

            BouncyBlock NewBlock = new BouncyBlock(false);

            // Get BouncyBlock parameters
            BouncyBlock_Parameters Params = (BouncyBlock_Parameters)Core.MyLevel.CurPiece.MyData.Style.FindParams(BouncyBlock_AutoGen.Instance);

            int Speed = (int)Params.Speed.GetVal(Core.Data.Position);

            NewBlock.Init(pos + offset, new Vector2(75, 75), Speed);
            NewBlock.BlockCore.BoxesOnly = BoxesOnly;

            return NewBlock as Block;
        }

        public Block MakeGhostBlock(Vector2 offset)
        {
            return MakeGhostBlock(offset, Math.Sign(Core.Data.Velocity.X));
        }
        public Block MakeGhostBlock(Vector2 offset, int Dir)
        {
            Vector2 pos = Vector2.Zero;
            if (Dir == 1) pos.X = Box.Target.TR.X + 75f / 1 - .01f;
            else if (Dir == -1) pos.X = Box.Target.BL.X - 75f / 1 + .01f;
            else pos.X = Box.Target.Center.X;
            pos.Y = Box.Target.BL.Y - 75f / 1 - .01f;
            GhostBlock NewBlock = new GhostBlock(false);
            NewBlock.Init(pos + offset, new Vector2(75, 75));
            NewBlock.BlockCore.BoxesOnly = BoxesOnly;

            // Get GhostBlock parameters
            GhostBlock_Parameters Params = (GhostBlock_Parameters)Core.MyLevel.CurPiece.MyData.Style.FindParams(GhostBlock_AutoGen.Instance);

            //RichLevelGenData GenData = ((PlaceGameData)Tools.CurGameData).MyGenData;
            //int InLength = GenData.Get(DifficultyType.GhostBlockInLength, pos);
            //int OutLength = GenData.Get(DifficultyType.GhostBlockOutLength, pos);

            int InLength = (int)Params.InLength.GetVal(pos);
            int OutLength = (int)Params.OutLength.GetVal(pos);
            
            int Offset = -Core.MyLevel.CurPhsxStep;

            NewBlock.InLength = InLength;
            NewBlock.OutLength = OutLength;
            NewBlock.Offset = Offset;


            return NewBlock as Block;
        }

        public MovingBlock MakeMovingBlock2(Vector2 offset)
        {
            int Dir = Math.Sign(Core.Data.Velocity.X);

            Vector2 pos = Vector2.Zero;
            if (Dir == 1) pos.X = Box.Target.TR.X + 75f / 1 - .01f;
            else if (Dir == -1) pos.X = Box.Target.BL.X - 75f / 1 + .01f;
            else pos.X = Box.Target.Center.X;
            pos.Y = Box.Target.BL.Y - 75f / 1 - .01f;
            //MovingBlock2 NewBlock = (MovingBlock2)Core.Recycle.GetObject(ObjectType.MovingBlock2, false);
            MovingBlock NewBlock = new MovingBlock(false);

            // Get MovingBlock parameters
            MovingBlock_Parameters Params = (MovingBlock_Parameters)Core.MyLevel.CurPiece.MyData.Style.FindParams(MovingBlock_AutoGen.Instance);

            //RichLevelGenData GenData = ((PlaceGameData)Tools.CurGameData).MyGenData;
            //NewBlock.Period = GenData.Get(DifficultyType.MovingBlock2Period, pos);
            //float Displacement = GenData.Get(DifficultyType.MovingBlock2Range, pos);

            NewBlock.Period = (int)Params.Period.GetVal(pos);
            float Displacement = (int)Params.Range.GetVal(pos);

            switch (HeldObjectIteration % 4)
            {
                case 0:
                    NewBlock.Offset = -Core.MyLevel.CurPhsxStep + 3 * NewBlock.Period / 4;
                    NewBlock.Displacement = new Vector2(0, Displacement);
                    break;

                case 1:
                    NewBlock.Offset = -Core.MyLevel.CurPhsxStep - NewBlock.Period / 4;
                    NewBlock.Displacement = new Vector2(Displacement, Displacement);
                    break;

                case 2:
                    NewBlock.Offset = -Core.MyLevel.CurPhsxStep - NewBlock.Period / 4;
                    NewBlock.Displacement = new Vector2(Displacement, 0);
                    break;

                case 3:
                    NewBlock.Offset = -Core.MyLevel.CurPhsxStep - NewBlock.Period / 4;
                    NewBlock.Displacement = new Vector2(-Displacement, Displacement);
                    break;
            }

            offset.Y -= 125;            
            
            NewBlock.Init(pos + offset, new Vector2(120, 120));
            NewBlock.BlockCore.BoxesOnly = BoxesOnly;

            return NewBlock as MovingBlock;
        }

        /// <summary>
        /// Whether this Bob is a player.
        /// </summary>
        public bool IsPlayer = true;

        /// <summary>
        /// Get the player data associated with this Bob.
        /// If the Bob isn't controlled by a player return null.
        /// </summary>
        /// <returns></returns>
        public PlayerData GetPlayerData()
        {
            if (!IsPlayer) return null;

            return PlayerManager.Get((int)MyPlayerIndex);
        }

        public bool GiveStats()
        {
            return Core.MyLevel.PlayMode == 0 && !CompControl && !Core.MyLevel.Watching && !Dead && !Dying;
        }

        public PlayerStats MyStats
        {
            get { return PlayerManager.Get((int)MyPlayerIndex).Stats; }
        }

        public PlayerStats MyTempStats
        {
            get { return PlayerManager.Get((int)MyPlayerIndex).TempStats; }
        }

        /// <summary>
        /// The number of frames since the player has died.
        /// </summary>
        int DeathCount = 0;

        public IObject KillingObject = null;
        public enum BobDeathType                { None,   Fireball,   Firesnake,    FireSpinner,    Floater,  Pinky,    Spike,   Fall,      Lava,   Blob,   Laser,   Time,         LeftBehind,    Other,   Total };
        public static string[] BobDeathNames = { "none", "fireball", "fire snake", "fire spinner", "spikey guy", "pinky", "spike", "falling", "lava", "blob", "laser", "time limit", "left behind", "other", "Total"};
        /// <summary>
        /// Kill the player.
        /// </summary>
        /// <param name="DeathType">The type of death.</param>
        /// <param name="ForceDeath">Whether to force the players death, ignoring immortality.</param>
        /// <param name="DoAnim">Whether to do the death animation.</param>
        public void Die(BobDeathType DeathType, bool ForceDeath, bool DoAnim)
        {
            Die(DeathType, null, ForceDeath, DoAnim);
        }
        public void Die(BobDeathType DeathType)
        {
            Die(DeathType, null, false, true);
        }
        public void Die(BobDeathType DeathType, IObject KillingObject)
        {
            Die(DeathType, KillingObject, false, true);
        }
        public void Die(BobDeathType DeathType, IObject KillingObject, bool ForceDeath, bool DoAnim)
        {
            if (Dying) return;

            if (!ForceDeath)
            {
                if (Immortal ||
                    (!Core.MyLevel.Watching && Core.MyLevel.PlayMode == 0 && ImmortalCountDown > 0)) return;

                if (CompControl) return;
            }

            DeathCount = 0;

            Core.DrawLayer = 9;

            FlamingCorpse = false;

            this.KillingObject = KillingObject;

#if XBOX
            Tools.SetVibration(MyPlayerIndex, .5f, .5f, 45);
#endif

            if (HeldObject != null && PlaceTimer <= 0)
                Core.MyLevel.AddPop(HeldObject.Core.Data.Position);

            // Update stats
            if (DeathType != BobDeathType.None)
                MyStats.DeathsBy[(int)BobDeathType.Total]++;

            MyStats.DeathsBy[(int)DeathType]++;

            Dying = true;

            if (DoAnim)
                MyPhsx.Die(DeathType);
            
            Tools.CurGameData.BobDie(Core.MyLevel, this);
        }

        /// <summary>
        /// Whether we can kill the current player.
        /// The player must be player controlled and not already dead.
        /// </summary>
        public bool CanDie
        {
            get
            {
                return !Immortal && !Dead && !Dying && Core.MyLevel.PlayMode == 0 && !Core.MyLevel.Watching;
            }
        }

        /// <summary>
        /// Whether we can finish a current level.
        /// The player must be player controlled and not already dead.
        /// </summary>
        public bool CanFinish
        {
            get
            {
                return !Dead && !Dying && Core.MyLevel.PlayMode == 0 && !Core.MyLevel.Watching;
            }
        }

        public void DyingPhsxStep()
        {
            DeathCount++;

            if (Core.Data.Velocity.Y > -30)
                Core.Data.Velocity += Core.Data.Acceleration;
            
            Core.Data.Position += Core.Data.Velocity;

            PlayerObject.PlayUpdate(1000f / 60f / 150f);

            // Check to see if any other players are alive
            /*
            if (PlayerManager.AllDead())
            {
                // Check to see if we should give a hint about quickspawning
                if (Hints.CurrentGiver != null)
                {
                    Hints.CurrentGiver.Check_QuickSpawn();
                }
            }*/

            // Check to see if we've fallen past the edge of the screen,
            // if so, officially declare the player dead.
            if (!Dead && (
                (IsVisible() && Core.Show && Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - MyGame.DoneDyingDistance)
                ||
                (!IsVisible() && DeathCount > MyGame.DoneDyingCount)))
            {
                Tools.CurGameData.BobDoneDying(Core.MyLevel, this);
                Dead = true;
            }

            Box.Current.Size = PlayerObject.BoxList[1].Size() / 2;
            Box.SetTarget(Core.Data.Position, Box.Current.Size);
        }

        public GameData MyGame
        {
            get { return Core.MyLevel.MyGame; }
        }

        public void CheckForScreenWrap()
        {
            if (ScreenWrap)
            {
                if (ScreenWrapToCenter)
                {
                    bool OffScreen = false;
                    if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 100)
                        OffScreen = true;
                    if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 100)
                        OffScreen = true;
                    if (Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 600)
                        OffScreen = true;
                    if (Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 600)
                        OffScreen = true;

                    if (OffScreen)
                    {
                        // Find highest bob
                        Vector2 Destination = Core.MyLevel.MainCamera.Data.Position;
                        if (Core.MyLevel.Bobs.Count > 1)
                        {
                            Bob HighestBob = null;
                            foreach (Bob bob in Core.MyLevel.Bobs)
                            {
                                if (bob != this && bob.AffectsCamera && (HighestBob == null || bob.Core.Data.Position.Y > HighestBob.Core.Data.Position.Y))
                                {
                                    HighestBob = bob;
                                }
                            }
                            Destination = HighestBob.Core.Data.Position;
                        }
                        Move(Destination - Core.Data.Position);
                        Core.MyLevel.AddPop(Core.Data.Position);
                    }
                }
                else
                {
                    // Do the screen wrap
                    bool Moved = false;
                    Vector2 w = Core.MyLevel.MainCamera.TR - Core.MyLevel.MainCamera.BL + new Vector2(1200, 1600);
                    if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 100)
                    {
                        Moved = true;
                        Move(new Vector2(w.X, 0));
                    }
                    if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 100)
                    {
                        Moved = true;
                        Move(new Vector2(-w.X, 0));
                    }
                    if (Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 600)
                    {
                        Moved = true;
                        Move(new Vector2(0, w.Y));
                    }
                    if (Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 600)
                    {
                        Moved = true;
                        Move(new Vector2(0, -w.Y));
                    }

                    // If multiplayer, decrease the bob's camera weight
                    //if (Moved && PlayerManager.GetNumPlayers() > 1)
                    //{
                    //    CameraWeight = 0;
                    //    CameraWeightSpeed = .01f;
                    //}
                }
            }
        }
        public float CameraWeight = 1, CameraWeightSpeed;


        public void GetPlayerInput()
        {
//#elif XBOX
            CurInput.Clean();

            if (Immobile) return;

            GamePadState pad = Tools.padState[(int)MyPlayerIndex];
            //pad = Tools.padState[0];

            if (pad.IsConnected)
            {
                if (pad.Buttons.A == ButtonState.Pressed)
                {
                    CurInput.A_Button = true;
                }
                else CurInput.A_Button = false;

                CurInput.xVec.X = CurInput.xVec.Y = 0;
                if (Math.Abs(pad.ThumbSticks.Left.X) > .15f)
                    CurInput.xVec.X = pad.ThumbSticks.Left.X;
                if (Math.Abs(pad.ThumbSticks.Left.Y) > .15f)
                    CurInput.xVec.Y = pad.ThumbSticks.Left.Y;

                if (pad.DPad.Right == ButtonState.Pressed)
                    CurInput.xVec.X = 1;
                if (pad.DPad.Left == ButtonState.Pressed)
                    CurInput.xVec.X = -1;
                if (pad.DPad.Up == ButtonState.Pressed)
                    CurInput.xVec.Y = 1;
                if (pad.DPad.Down == ButtonState.Pressed)
                    CurInput.xVec.Y = -1;


                CurInput.B_Button = (pad.Buttons.LeftShoulder == ButtonState.Pressed
                            || pad.Buttons.RightShoulder == ButtonState.Pressed);
                //PrevB_Button = (Tools.prevPadState[(int)MyPlayerIndex].Buttons.A == ButtonState.Pressed);
            }
//#endif

#if WINDOWS
            Vector2 KeyboardDir = Vector2.Zero;

            //if (MyPlayerIndex == PlayerIndex.One)
            {
                CurInput.A_Button |= Tools.keybState.IsKeyDownCustom(Keys.Up);
                CurInput.A_Button |= Tools.keybState.IsKeyDownCustom(ButtonCheck.Up_Secondary);
                KeyboardDir.X = KeyboardDir.Y = 0;
                if (Tools.keybState.IsKeyDownCustom(Keys.Up)) KeyboardDir.Y = 1;
                if (Tools.keybState.IsKeyDownCustom(Keys.Down)) KeyboardDir.Y = -1;
                if (Tools.keybState.IsKeyDownCustom(Keys.Right)) KeyboardDir.X = 1;
                if (Tools.keybState.IsKeyDownCustom(Keys.Left)) KeyboardDir.X = -1;
                if (Tools.keybState.IsKeyDownCustom(ButtonCheck.Left_Secondary)) KeyboardDir.X = -1;
                if (Tools.keybState.IsKeyDownCustom(ButtonCheck.Right_Secondary)) KeyboardDir.X = 1;
                if (Tools.keybState.IsKeyDownCustom(ButtonCheck.Up_Secondary)) KeyboardDir.Y = 1;
                if (Tools.keybState.IsKeyDownCustom(ButtonCheck.Down_Secondary)) KeyboardDir.Y = -1;
                //CurInput.B_Button |= Tools.keybState.IsKeyDownCustom(Keys.C);
                CurInput.B_Button |= Tools.keybState.IsKeyDownCustom(ButtonCheck.Back_Secondary);
            }

            //if (MyPlayerIndex == PlayerIndex.Two)
            if (false)
            {
                CurInput.A_Button |= Tools.keybState.IsKeyDownCustom(Keys.Y);
                KeyboardDir.X = KeyboardDir.Y = 0;
                if (Tools.keybState.IsKeyDownCustom(Keys.Y)) KeyboardDir.Y = 1;
                if (Tools.keybState.IsKeyDownCustom(Keys.H)) KeyboardDir.Y = -1;
                if (Tools.keybState.IsKeyDownCustom(Keys.J)) KeyboardDir.X = 1;
                if (Tools.keybState.IsKeyDownCustom(Keys.G)) KeyboardDir.X = -1;
            }

            if (KeyboardDir.LengthSquared() > CurInput.xVec.LengthSquared())
                CurInput.xVec = KeyboardDir;
#endif        

            // Invert left-right for inverted levels
            if (Core.MyLevel != null && Core.MyLevel.ModZoom.X < 0)
                CurInput.xVec.X *= -1;
        }
        
        public void GetRecordedInput(int Step)
        {
            if (Core.MyLevel.Replay)
            {
                if (Step < MyRecord.Input.Length)
                    CurInput = MyRecord.Input[Step];
                
                return;
            }

            if (MyPiece != null && Step < MyPiece.PieceLength)
            {
                CurInput = MyRecord.Input[Step];
            }
            else
            {
                CurInput.xVec = new Vector2(1, 0);
                CurInput.A_Button = true;
                CurInput.B_Button = false;
            }
        }


        void RecordInput(int Step)
        {
            MyRecord.Input[Step] = CurInput;
        }




        public void AnimStep()
        {
            if (Dying) return;

            MyPhsx.AnimStep();
        }

        /// <summary>
        /// Set to true to skip the next call to PrepareToDraw
        /// </summary>
        public bool SkipPrepare = false;

        /// <summary>
        /// Prepare the stickman for drawing.
        /// SHOULD BE REPLACED BY NON-SHADER MEANS.
        /// </summary>
        public void PrepareToDraw()
        {
            if (SkipPrepare)
            {
                SkipPrepare = false;
                return;
            }

            if (BoxesOnly) return;

            if (DrawOutline && Core.Show)
            {
                Vector2 size = PlayerObject.BoxList[0].Size();
                float ratio = size.Y / size.X;
                int width = Tools.TheGame.Resolution.Bob.X;
                int height = (int)(width * ratio);

                PlayerObject.MakeRenderTargetUnique(width, height);
                PlayerObject.PreDraw(Tools.Device, Tools.EffectWad);
            }
        }

        /// <summary>
        ///// When true the call to AnimAndUpdate must be done manually.
        /// </summary>
        public bool ManualAnimAndUpdate = false;
        public void AnimAndUpdate()
        {
            AnimStep();
            UpdateObject();
        }

        /// <summary>
        /// The position of the stickman object
        /// </summary>
        public Vector2 ObjectPos
        {
            get { return PlayerObject.ParentQuad.Center.Pos; }
        }

        public void UpdateObject()
        {
            Vector2 NewCenter = Core.Data.Position - (PlayerObject.BoxList[1].TR.Pos - PlayerObject.ParentQuad.Center.Pos - Box.Current.Size);
            PlayerObject.ParentQuad.Center.Move(NewCenter);
            PlayerObject.ParentQuad.Update();

            PlayerObject.Update(null);
            //Core.Data.Position = PlayerObject.BoxList[1].Center();
        }

        public void UpdateColors()
        {
            if (MyObjectType is BobPhsxSpaceship && PlayerObject.QuadList != null)
            {
                PlayerObject.QuadList[1].SetColor(MyColorScheme.OutlineColor.DetailColor);
                PlayerObject.QuadList[2].SetColor(MyColorScheme.SkinColor.DetailColor);
            }
        }

        public static bool GuideActivated = false;
        static QuadClass GuideQuad;
        void InitGuideQuad()
        {
            if (GuideQuad != null) return;

            GuideQuad = new QuadClass();
            //GuideQuad.TextureName = "Circle";
            GuideQuad.EffectName = "Circle";
            GuideQuad.Size = new Vector2(100, 100);
        }

        static int GuideLength = 8;
        static float Guide_h = 1f / GuideLength;
        void DrawGuidePiece(int Step, Vector2[] Loc, int i)
        {
            if (Loc.Length > Step)
            {
                InitGuideQuad();

                Vector2 Size = new Vector2(100 - 50 * Guide_h * i);
                //Vector2 Size = new Vector2(40);

                GuideQuad.Quad.SetColor(new Color(0f, 0f, 0f, 1f - Guide_h * i));
                GuideQuad.Size = Size * 1.15f;

                GuideQuad.Pos = Loc[Step];
                GuideQuad.Draw();



                GuideQuad.Quad.SetColor(new Color(0f, 1f, 0f, 1f - Guide_h * i));
                GuideQuad.Size = Size;

                GuideQuad.Pos = Loc[Step];
                GuideQuad.Draw();
            }
        }

        void InitSectionDraw()
        {
                Vector2 Size = new Vector2(15);

                GuideQuad.Quad.SetColor(Color.PowderBlue);
                //GuideQuad.Quad.SetColor(Color.Black);
                //GuideQuad.Quad.SetColor(new Color(0,255,0,150));
                GuideQuad.Size = Size;
        }
        void DrawSection(int Step, Vector2[] Loc)
        {
                GuideQuad.Pos = Loc[Step];
                GuideQuad.Draw();
        }

        void DrawGuide()
        {            
            if (MyPiece != null && MyPiece.Recording != null && MyPiece.Recording.Length > MyPieceIndex)
            {
                int Step = Core.MyLevel.GetPhsxStep();
                Step = Math.Max(0, Step - 2);

                Vector2[] Loc = MyPiece.Recording[MyPieceIndex].AutoLocs;

                // Style 1
                InitGuideQuad();
                int N = Math.Min(1000, Loc.Length);
                N = Math.Min(N, MyPiece.PieceLength);
                InitSectionDraw();
                for (int i = 1; i < N; i += 2)
                    DrawSection(i, Loc);
                if (Loc != null && N > Step)
                    DrawGuidePiece(Step, Loc, 2);

                // Style 2
                //if (Loc != null && Loc.Length > Step)
                //{
                //    InitGuideQuad();

                //    for (int i = 0; i < GuideLength; i++)
                //        DrawGuidePiece(Step + 5 * i, Loc, i);
                //}
            }
        }

        /// <summary>
        /// Whether the player is visible on the screen.
        /// </summary>
        public bool IsVisible()
        {
            if (!Core.Show) return false;

            if (Bob.AllExplode)
            {
                if (Dying || Dead) return false;
            }

            return true;
        }

        public void Draw()
        {
            bool SkipDraw = false;

            // Draw guide
            if (GuideActivated && Core.MyLevel != null && !Core.MyLevel.Watching && !Core.MyLevel.Replay)
                DrawGuide();

            if (!Core.Show)
                return;

            if (Dying || Dead)
            {
                if (Bob.AllExplode && !Bob.ShowCorpseAfterExplode) return;

                if (MyObjectType is BobPhsxSpaceship)
                {
                    return;
                }
            }

            if ((Dying || Dead) && FlamingCorpse)                
                ParticleEffects.Flame(Core.MyLevel, Core.Data.Position + 1.5f * Core.Data.Velocity, Core.MyLevel.GetPhsxStep(), 1f, 10, false);

            UpdateColors();

            // Draw guide
            //if (GuideActivated && Core.MyLevel != null && !Core.MyLevel.Watching && !Core.MyLevel.Replay)
            //    DrawGuide();

            if (FadingIn)
            {
                Fade += .033f;
                if (Fade >= 1)
                {
                    FadingIn = false;
                    Fade = 1;
                    PlayerObject.ContainedQuad.SetColor(new Color(1, 1, 1, 1));
                }

                if (MyCape != null)
                    MyCape._MyColor.A = (byte)(255 * Fade);
            }


            if (MyCape != null && CanHaveCape && Core.Show && ShowCape && !SkipDraw)
            {                
                MyCape.Draw();
            }

            if (Tools.DrawGraphics && !BoxesOnly && Core.Show)
            {
                PlayerObject.OutlineColor = MyColorScheme.OutlineColor.Clr;
                PlayerObject.InsideColor = MyColorScheme.SkinColor.Clr;

                if (FadingIn)
                {
                    PlayerObject.OutlineColor = new Color(PlayerObject.OutlineColor.ToVector4() * Fade);
                    PlayerObject.InsideColor = new Color(PlayerObject.InsideColor.ToVector4() * Fade);
                    PlayerObject.ContainedQuad.SetColor(new Color(1, 1, 1, Fade));
                }

                if (!SkipDraw)
                {
                    if (DrawOutline)
                        PlayerObject.ContainedDraw();
                    else
                        PlayerObject.Draw(Tools.EffectWad, true);
                }
            }

            if (Tools.DrawBoxes)
            {
                Box.Draw(Tools.QDrawer, Color.Wheat, 12);
                Box2.Draw(Tools.QDrawer, Color.Wheat, 12);
                Box2.DrawT(Tools.QDrawer, Color.Wheat, 12);

                if (Boxes != null)
                {
                    Boxes[0].Draw(Tools.QDrawer, Color.Red, 8);
                    Boxes[3].Draw(Tools.QDrawer, Color.Green, 8);
                    Boxes[8].Draw(Tools.QDrawer, Color.Blue, 8);
                }
            }

            // Held object
            if (HeldObject != null && PlaceTimer <= 0 && !Dead && !Dying)
            {
                HeldObject.Draw();
            }
        }

        public void Move(Vector2 shift)
        {
            Core.Data.Position += shift;

            Box.Move(shift);
            Box2.Move(shift);

            if (PlayerObject == null)
                return;

            PlayerObject.ParentQuad.Center.Move(PlayerObject.ParentQuad.Center.Pos + shift);
            PlayerObject.ParentQuad.Update();
            PlayerObject.Update(null, ObjectDrawOrder.WithOutline);

            if (MyCape != null)
                MyCape.Move(shift);
        }

        public void Reset(bool BoxesOnly) { }
        public void Interact(Bob bob) { }














































        public void InteractWithBlock(AABox box, Block block, ColType Col)
        {            
            if (block != null && !block.IsActive) return;

            if (block != null && Col != ColType.NoCol) block.Hit(this);

            if (block != null && Col != ColType.NoCol)
                if (Col != ColType.Top)
                    block.BlockCore.NonTopUsed = true;

            //float yvel = box.Target.TR.Y - box.Current.TR.Y;

            ColType OriginalColType = Col;

            if (Col != ColType.NoCol && (Col == ColType.Top ||
                Col != ColType.Bottom && Math.Max(Box.Current.BL.Y, Box.Target.BL.Y) > box.Target.TR.Y - Math.Max(-1.35 * Core.Data.Velocity.Y, 7)))
            {
                Col = ColType.Top;

                NewY = box.Target.TR.Y + Box.Current.Size.Y + .01f;

                if (Core.Data.Position.Y <= NewY)
                {
                    //NewVel = Math.Max(Core.Data.Velocity.Y, -3 + box.Target.TR.Y - box.Current.TR.Y);
                    NewVel = Math.Max(-1000, MyPhsx.ForceDown + box.Target.TR.Y - box.Current.TR.Y);

                    if (block != null)
                    {
                        block.BlockCore.StoodOn = true;
                        block.LandedOn(this);
                    }

                    if (!TopCol)
                    {
                        BottomCol = true;
                        //Console.WriteLine("  ON BLOCK");

                        if (OriginalColType == ColType.Top)
                        {
                            Core.Data.Position.Y = NewY;

                            if (block == null || block.BlockCore.GivesVelocity)
                            {
                                //if (MyPhsx.OverrideSticky)
                                if (MyPhsx.Sticky)
                                {
                                    //if (NewVel > 0) NewVel *= .6f;
                                    Core.Data.Velocity.Y = NewVel;
                                }
                                else
                                    Core.Data.Velocity.Y = Math.Max(NewVel, Core.Data.Velocity.Y);
                            }

                            GroundSpeed = (box.Target.TR.X - box.Current.TR.X + box.Target.BL.X - box.Current.BL.X) / 2;
                            if (block != null)
                                GroundSpeed += block.BlockCore.GroundSpeed;
                        }
                        else
                        {
                            // We hit the block from the side, just at the top edge
                            // Keep bigger Y values
                            Core.Data.Position.Y = Math.Max(Core.Data.Position.Y, NewY);
                            if (block == null || block.BlockCore.GivesVelocity)
                                Core.Data.Velocity.Y = Math.Max(Core.Data.Velocity.Y, NewVel);
                        }                        

                        MyPhsx.ObjectLandedOn = block;
                        MyPhsx.LandOnSomething(false);
                        
                        if (OnLand != null) OnLand(); OnLand = null;
                    }
                }
            }

            if (Col != ColType.NoCol && (block == null || block.BlockCore.MyType != ObjectType.LavaBlock))
            {
                if (!box.TopOnly)
                {
                    if (Col == ColType.Bottom && !(Col == ColType.Left || Col == ColType.Right))
                    {
                        TopCol = true;
                    }

                    if (Col != ColType.Bottom && Core.Data.Velocity.X != 0 && !MyPhsx.OnGround && Math.Min(Box.Current.TR.Y, Box.Target.TR.Y) < box.Target.BL.Y + Math.Max(1.35 * Core.Data.Velocity.Y, 7))
                        Col = ColType.Bottom;

                    NewY = box.Target.BL.Y - Box.Current.Size.Y - .01f;
                    if (Core.Data.Position.Y > NewY && Col == ColType.Bottom)
                    {
                        if (MyPhsx.OnGround && block.BlockCore.DoNotPushHard)
                        {
                            block.Smash(this);
                            return;
                        }

                        MyPhsx.HitHeadOnSomething();

                        if (block != null)
                            block.HitHeadOn(this);

                        if (OriginalColType == ColType.Bottom)
                        {
                            Core.Data.Position.Y = NewY;

                            if (block == null || block.BlockCore.GivesVelocity)
                            {
                                NewVel = Math.Min(Math.Min(0, NewVel), box.Target.BL.Y - box.Current.BL.Y) + 10;
                                Core.Data.Velocity.Y = Math.Min(NewVel, Core.Data.Velocity.Y);
                            }

                            //float blockvel = box.Target.TR.Y - box.Current.TR.Y;
                            //if (Core.Data.Velocity.Y > blockvel)
                            //    Core.Data.Velocity.Y = blockvel;

                            //if (NewVel < 0)
                            //    if (block == null || block.BlockCore.GivesVelocity)
                            //    {
                            //        NewVel = Math.Min(0, box.Target.BL.Y - box.Current.BL.Y) + 7;
                            //        Core.Data.Velocity.Y = Math.Min(NewVel, Core.Data.Velocity.Y);
                            //    }
                        }
                        else
                        {
                            // We hit the block from the side, just at the bottom edge
                            // Keep smaller Y values
                            Core.Data.Position.Y = Math.Min(Core.Data.Position.Y, NewY);
                            if (block == null || block.BlockCore.GivesVelocity)
                                //Core.Data.Velocity.Y = Math.Min(Core.Data.Velocity.Y, NewVel);
                            {
                                NewVel = Math.Min(0, box.Target.BL.Y - box.Current.BL.Y) + 10;
                                Core.Data.Velocity.Y = Math.Min(NewVel, Core.Data.Velocity.Y);
                            }

                        }                        
                    }
                    else
                    {
                        Xvel = box.Target.TR.X - box.Current.TR.X;

                        if (Col == ColType.Left)
                        {
                            if (block != null)
                                block.SideHit(this);

                            MyPhsx.SideHit(Col);

                            Core.Data.Position.X = box.Target.BL.X - Box.Current.Size.X - .01f;

                            SideHitCount += 2;
                            if (SideHitCount > 5) Core.Data.Velocity.X *= .4f;

                            if (block == null || block.BlockCore.GivesVelocity)
                            if (Xvel < Core.Data.Velocity.X)
                                if (Box.Current.BL.Y < box.Current.TR.Y - 35 &&
                                    Box.Current.TR.Y > box.Current.BL.Y + 35)
                                    Core.Data.Velocity.X = Xvel;
                        }

                        if (Col == ColType.Right)
                        {
                            if (block != null)
                                block.SideHit(this);

                            MyPhsx.SideHit(Col);

                            Core.Data.Position.X = box.Target.TR.X + Box.Current.Size.X + .01f;

                            SideHitCount += 2;
                            if (SideHitCount > 5) Core.Data.Velocity.X *= .4f;

                            if (block == null || block.BlockCore.GivesVelocity)
                            if (Xvel > Core.Data.Velocity.X)
                                if (Box.Current.BL.Y < box.Current.TR.Y - 35 &&
                                    Box.Current.TR.Y > box.Current.BL.Y + 35)
                                    Core.Data.Velocity.X = Xvel;
                        }
                    }
                }
            }
        }




        void InitBoxesForCollisionDetect()
        {
            Box.Current.Size = PlayerObject.BoxList[1].Size() / 2;
            Box2.Current.Size = PlayerObject.BoxList[2].Size() / 2;

            if (Core.MyLevel.PlayMode != 0 && Core.MyLevel.DefaultHeroType is BobPhsxSpaceship)
            {
                Box.Current.Size *= 1.2f;
                Box2.Current.Size *= 1.2f;
            }

            Box.SetTarget(Core.Data.Position, Box.Current.Size + new Vector2(.0f, .02f));
            Box.Target.TR.Y += 5;
            //Box.Current.BL.Y += 0;
            Box2.SetTarget(Core.Data.Position, Box2.Current.Size);
        }


        public void UpdateCape()
        {
            if (!CanHaveCape) // || !ShowCape)
                return;

            MyCape.Wind = CapeWind;
            Vector2 AdditionalWind = Vector2.Zero;
            if (Core.MyLevel != null && Core.MyLevel.MyBackground != null)
            {
                AdditionalWind += Core.MyLevel.MyBackground.Wind;
                MyCape.Wind += AdditionalWind;
            }
            //MyCape.Wind.X -= .2f * Core.Data.Velocity.X;

            if (MyPhsx.Ducking && MyObjectType != BobPhsxBox.Instance)
                MyCape.GravityScale = .4f;
            else
                MyCape.GravityScale = 1f;

            // Set the anchor point
            if (temp == null) temp = new ObjectVector();            
            if (Body == null) Body = (BendableQuad)PlayerObject.FindQuad("Body");
            temp.ParentQuad = Body.ParentQuad;

            Body.MySpline.GetBothVectors(1.20f, 0, ref temp.RelPos, ref temp.RelPos);
            temp.FastPosFromRelPos(Body.ParentQuad);
            Vector2 vel = MyPhsx.ApparentVelocity;
            MyCape.AnchorPoint[0] = temp.Pos + (vel);

            //Console.WriteLine("{0} {1} {2}", Core.Data.Position, MyCape.AnchorPoint[0], MyCape.AnchorPoint[1]);

            if (Core.MyLevel != null)
            {
                float t = Core.MyLevel.GetPhsxStep() / 2.5f;
                if (CharacterSelect2)
                    t = Tools.DrawCount / 2.5f;
                float AmplitudeX = Math.Min(2.5f, Math.Abs(vel.X - AdditionalWind.X) / 20);
                MyCape.AnchorPoint[0].Y += 15 * (float)(Math.Cos(t) * AmplitudeX);
                float Amp = 2;
                if (vel.Y < 0)
                    Amp = 8;
                float AmplitudeY = Math.Min(2.5f, Math.Abs(vel.Y - AdditionalWind.Y) / 45);
                MyCape.AnchorPoint[0].X += Amp * (float)(Math.Sin(t) * AmplitudeY);
            }
            //MyCape.AnchorPoint[0].X += .1f * (Core.Data.Velocity).X;
            Vector2 CheatShift = Vector2.Zero;//new Vector2(.15f, .35f) * Core.Data.Velocity;
            float l = (vel - 2*AdditionalWind).Length();
            if (l > 15)
            {
                CheatShift = (vel - 1*AdditionalWind);
                CheatShift.Normalize();
                CheatShift = (l - 15) * CheatShift;
            }
            MyCape.Move(CheatShift);
            //for (int i = 0; i < 1; i++)
            MyCape.PhsxStep();
            //MyCape.MyColor = Color.Gray;
        }

        public void CorePhsxStep()
        {
        }

        public void DollPhsxStep()
        {
            CurInput.A_Button = false;
            CurInput.B_Button = false;
            CurInput.xVec = Vector2.Zero;

            // Phsyics update
            MyPhsx.PhsxStep();

            // Integrate velocity
            Core.Data.Position += Core.Data.Velocity;// +new Vector2(GroundSpeed, 0);

            // Cape
            if (Core.MyLevel.PlayMode == 0 && MyCape != null)
                UpdateCape();

            MyPhsx.OnGround = true;
        }

        public void EndSuckedIn()
        {
            SuckedIn = false;
            PlayerObject.ContainedQuadAngle = 0;
        }
        void SuckedInPhsxStep()
        {
            Vector2 acc = SuckedInSeed.Core.Data.Position - Core.Data.Position;
            float l = acc.Length();
            acc.Normalize();
            if (l > 550)//425)
            {
                Vector2 normal = new Vector2(-acc.Y, acc.X);
                acc = acc * Math.Min(15, l) + normal * Tools.RndFloat(2.4f, 2.6f);// 2.5f;
            }
            else
            {
                Core.Data.Velocity.Normalize();
                Core.Data.Velocity *= 40;// Tools.RndFloat(35, 45);// 40;
            }
            Core.Data.Velocity += acc;
            Core.Data.Velocity *= .9f;

            PlayerObject.ContainedQuadAngle = -Core.MyLevel.CurPhsxStep / 4f;

            // Integrate velocity
            Core.Data.Position += Core.Data.Velocity;// +new Vector2(GroundSpeed, 0);

            // Cape
            if (Core.MyLevel.PlayMode == 0 && MyCape != null)
                UpdateCape();
            Wind /= 2;
            CapeWind /= 2;
        }

        /// <summary>
        /// Whether to do object interactions.
        /// </summary>
        public bool DoObjectInteractions = true;

        public void PhsxStep2() { }
        public void PhsxStep()
        {
            DoLightSourceFade();

            if (!Core.Show)
                return;

            if (CharacterSelect2)
            {
                DollPhsxStep();
                return;
            }

            if (SuckedIn)
            {
                SuckedInPhsxStep();
                return;
            }

            if (ImmortalCountDown > 0)
            {
                ImmortalCountDown--;
                if (ImmortalCountDown < ImmortalLength - 15)
                if (Math.Abs(CurInput.xVec.X) > .5f || CurInput.A_Button)
                    ImmortalCountDown = 0;
            }

            SaveNoBlock = false;


            int CurPhsxStep = Core.MyLevel.CurPhsxStep;



            // Bob connections
            if (MyBobLinks != null)
                foreach (BobLink link in MyBobLinks)
                    link.PhsxStep(this);

            if (Dying)
            {
                DyingPhsxStep();

                // Cape
                if (Core.MyLevel.PlayMode == 0 && MyCape != null)
                    UpdateCape();

                return;
            }

            // Track Star bonus book keeping
            if (GiveStats() && Core.MyLevel.CurPhsxStep > 45)
            {
                MyTempStats.FinalTimeSpent++;

                if (Math.Abs(CurInput.xVec.X) < .75f)
                    MyTempStats.FinalTimeSpentNotMoving++;
            }

            // Held object
            if (HeldObject != null)
            {
                Vector2 HeadPos;
                if (BoxesOnly)
                {
                    HeadPos = new Vector2(Pos.X, Box.TR.Y);   
                }
                else
                {
                    if (Head == null) Head = (Quad)PlayerObject.FindQuad("Head");
                    HeadPos = Head.Center.Pos;
                }

                Vector2 offset = Vector2.Zero;
                switch (HeldObject.Core.MyType)
                {
                    case ObjectType.FlyingBlob: offset = new Vector2(-16, -20); break;
                    case ObjectType.FallingBlock: offset = new Vector2(0, -6); break;
                    case ObjectType.MovingBlock: offset = new Vector2(0, -6); break;
                    case ObjectType.GhostBlock: offset = new Vector2(0, 0); break;
                }

                Vector2 BodyPosition = new Vector2((HeadPos.X + Core.Data.Position.X) / 2, HeadPos.Y + 46);
                if (PlayerObject.xFlip)
                    BodyPosition.X = 2 * PlayerObject.FlipCenter.X - BodyPosition.X;
                HeldObject.Move(BodyPosition + .9f * Core.Data.Velocity + HeldObject.Core.HeldOffset + offset - HeldObject.Core.Data.Position);
                if (HeldObject is PrincessBubble)
                    ;
                else
                    HeldObject.PhsxStep();
            }

            // Increment life counter
            if (Core.MyLevel.PlayMode == 0 && !CompControl && !Core.MyLevel.Watching)
                MyStats.TimeAlive++;

            // Screen wrap
            CheckForScreenWrap();



            if (!CharacterSelect)
            {
                int Mode = Core.MyLevel.PlayMode;
                if (Core.MyLevel.NumModes == 1)
                    if (Mode == 1) Mode = 2;

                switch (Mode)
                {
                    case 0:
                        if (!CompControl)
                        {
                            if (Cinematic)
                                ;//AnimAndUpdate();
                            else
                            {
                                if (CodeControl)
                                {
                                    CurInput.Clean();
                                    if (ControlFunc != null)
                                    {
                                        ControlCount++;
                                        ControlFunc(ControlCount);
                                    }
                                }
                                else
                                    GetPlayerInput();
                            }
                        }
                        else
                            GetRecordedInput(CurPhsxStep - IndexOffset);

                        break;

                    case 1:
                        GetRecordedInput(CurPhsxStep - IndexOffset);

                        break;

                    case 2:
                        MyPhsx.GenerateInput(CurPhsxStep);
                        RecordInput(CurPhsxStep - IndexOffset);

                        break;
                }
            }
            else
            {
                CurInput.A_Button = false;
                CurInput.B_Button = false;
                CurInput.xVec = Vector2.Zero;
            }

            //ButtonPhsx();
            ReadyToPlace = MyPhsx.ReadyToPlace();


            // Phsyics update
            if (MoveData.InvertDirX) CurInput.xVec.X *= -1;
            float Windx = Wind.X;
            if (MyPhsx.OnGround) Windx /= 2;
            Core.Data.Velocity.X -= Windx;
            MyPhsx.PhsxStep();
            Core.Data.Velocity.X += Windx;
            MyPhsx.CopyPrev();
            if (MoveData.InvertDirX) CurInput.xVec.X *= -1;

            // Collision with screen boundary
            if (CollideWithCamera && !Cinematic)
            {
                Box.CalcBounds();
                if (Box.TR.X > Core.MyLevel.MainCamera.TR.X - 40 && Core.Data.Velocity.X > 0)
                {
                    Core.Data.Velocity.X = 0;
                    MyPhsx.SideHit(ColType.Right);
                    if (Box.TR.X > Core.MyLevel.MainCamera.TR.X - 20 && Core.Data.Velocity.X > 0)
                    {
                        Move(new Vector2(Core.MyLevel.MainCamera.TR.X - 20 - Box.TR.X, 0));
                    }
                }
                if (Box.BL.X < Core.MyLevel.MainCamera.BL.X + 40 && Core.Data.Velocity.X < 0)
                {
                    Core.Data.Velocity.X = 0;
                    MyPhsx.SideHit(ColType.Left);
                    if (Box.BL.X < Core.MyLevel.MainCamera.BL.X + 20 && Core.Data.Velocity.X < 0)
                    {
                        Move(new Vector2(Core.MyLevel.MainCamera.BL.X + 20 - Box.BL.X, 0));
                    }
                }
            }

            // Integrate velocity
            if (!Cinematic)
                //Core.Data.Position += Core.Data.Velocity + new Vector2(GroundSpeed, 0);
                MyPhsx.Integrate();

            // Cape
            if (Core.MyLevel.PlayMode == 0 && MyCape != null)
                UpdateCape();
            Wind /= 2;
            CapeWind /= 2;

            // If cinematic, don't do any death or object interactions
            if (Cinematic)
            {
                AnimAndUpdate();
                ControlCount++;
                if (CinematicFunc != null) CinematicFunc(ControlCount);

                return;
            }

            // If too high, knock Bob down a bit
            if (Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 900 && Core.Data.Velocity.Y > 0)
                Core.Data.Velocity.Y = 0;

            // Check for death by falling or by off screen
            if (Core.MyLevel.PlayMode == 0)
            {
                float DeathDist = 650;
                if (Core.MyLevel.MyGame.MyGameFlags.IsTethered) DeathDist = 900;
                if (Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - DeathDist)
                {
                    Die(BobDeathType.Fall);
                }
                else
                    if (Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 1500 ||
                        Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 550 ||
                        Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 550)
                    {
                        Die(BobDeathType.LeftBehind);
                    }
            }

            // Check for death by time out
            if (Core.MyLevel.PlayMode == 0 && Core.MyLevel.CurPhsxStep > Core.MyLevel.TimeLimit && Core.MyLevel.TimeLimit > 0)
                Die(BobDeathType.Time);

            // Initialize boxes for collision detection
            InitBoxesForCollisionDetect();

            /////////////////////////////////////////////////////////////////////////////////////////////
            //                 Block Interactions                                                      //
            /////////////////////////////////////////////////////////////////////////////////////////////            
            BlockInteractions();

            /////////////////////////////////////////////////////////////////////////////////////////////
            //                 Object Interactions                                                     //
            /////////////////////////////////////////////////////////////////////////////////////////////            
            if (DoObjectInteractions)
                ObjectInteractions();

            ButtonPhsx();

            // Reset boxes to normal
            Box.SetCurrent(Core.Data.Position, Box.Current.Size);
            Box2.SetCurrent(Core.Data.Position, Box2.Current.Size);

//if (Core.MyLevel.PlayMode != 0)
//    for (int i = 0; i <= NumBoxes; i++)
//    {
//        AABox box = Boxes[i];
//        box.SetCurrent(Core.Data.Position, box.Current.Size);
//    }

            // Closing phsx
            MyPhsx.PhsxStep2();

            PrevInput = CurInput;
        }

        /// <summary>
        /// Calculate all interactions between the player and every IObject in the level.
        /// </summary>
        void ObjectInteractions()
        {
            if (Core.MyLevel.PlayMode != 0)
            {
                // Create list of boxes
                if (Boxes == null)
                {
                    Boxes = new List<AABox>();
                    for (int i = 0; i <= NumBoxes; i++)
                        Boxes.Add(new AABox(Vector2.Zero, Vector2.One));
                }

                // Update box list
                UpdateBoxList();
            }
            else
                Box2.SetTarget(Core.Data.Position, Box2.Current.Size);
            Box.SetTarget(Core.Data.Position, Box.Current.Size + new Vector2(.0f, .2f));


            foreach (IObject obj in Core.MyLevel.ActiveObjectList)
            {
                if (!obj.Core.MarkedForDeletion && obj.Core.Real && obj.Core.Active && obj.Core.Show)
                    obj.Interact(this);
            }
        }

        /// <summary>
        /// Update the list of AABox boxes used by the computer when creating the level.
        /// </summary>
        void UpdateBoxList()
        {
            float extra = 0;// 5;
            for (int i = 0; i <= NumBoxes; i++)
            {
                AABox box = Boxes[i];

                box.Current.Size = Box2.Current.Size;
                
                //box.Current.Size.X += Upgrades.MaxBobWidth * ((NumBoxes - i) * .1f);

                box.Current.Size.X += extra + .7f * Upgrades.MaxBobWidth * ((NumBoxes - i) * .1f);
                box.Current.Size.Y += extra + .23f * Upgrades.MaxBobWidth * ((NumBoxes - i) * .1f);

                box.SetCurrent(Box2.Current.Center, box.Current.Size);
                box.SetTarget(Core.Data.Position, box.Current.Size);
            }
            RegularBox2 = Boxes[Boxes.Count - 1];

            Box2.Current.Size.X += extra + .7f * Upgrades.MaxBobWidth;
            Box2.Current.Size.Y += extra + .23f * Upgrades.MaxBobWidth;
            Box2.SetTarget(Core.Data.Position, Box2.Current.Size);
        }

        void DeleteObj(IObject obj)
        {
            obj.Core.DeletedByBob = true;
            Core.Recycle.CollectObject(obj);
        }

        /// <summary>
        /// Calculate all interactions between the player and every Block in the level.
        /// </summary>
        void BlockInteractions()
        {
            int CurPhsxStep = Core.MyLevel.CurPhsxStep;

            GroundSpeed = 0;

            SideHitCount--;
            if (SideHitCount < 0) SideHitCount = 0;

            MyPhsx.ResetJumpModifiers();

            BottomCol = TopCol = false;
            if (CanInteract)
                if (Core.MyLevel.PlayMode != 2)
                {
                    if (Core.MyLevel.DefaultHeroType is BobPhsxSpaceship && Core.MyLevel.PlayMode == 0)
                    {
                        foreach (Block block in Core.MyLevel.Blocks)
                        {
                            if (!block.Core.MarkedForDeletion && block.IsActive && Phsx.BoxBoxOverlap(Box2, block.Box))
                            {
                                if (!Immortal)
                                    Die(BobDeathType.Other);
                                else
                                    block.Hit(this);
                            }
                        }
                    }
                    else
                    {
                        foreach (Block block in Core.MyLevel.Blocks)
                        {
                            if (block.Core.MarkedForDeletion || !block.IsActive || !block.Core.Real) continue;
                            if (block.BlockCore.OnlyCollidesWithLowerLayers && block.Core.DrawLayer <= Core.DrawLayer)
                                continue;

                            if (block.Core.MyTileSetType == TileSet.OutsideGrass)
                                Tools.Write("");

                            ColType Col = Phsx.CollisionTest(Box, block.Box);
                            if (Col != ColType.NoCol)
                            {
                                InteractWithBlock(block.Box, block, Col);
                            }
                        }
                    }
                }
                else
                {
                    Ceiling_Parameters CeilingParams = (Ceiling_Parameters)Core.MyLevel.CurPiece.MyData.Style.FindParams(Ceiling_AutoGen.Instance);

                    foreach (Block block in Core.MyLevel.Blocks)
                    {
                        if (block.Core.MarkedForDeletion || !block.IsActive || !block.Core.Real) continue;
                        if (block.BlockCore.OnlyCollidesWithLowerLayers && block.Core.DrawLayer <= Core.DrawLayer)
                            continue;

                        if (block.BlockCore.Ceiling)// && !block.Core.GenData.Used)
                        {
                            if (Core.Data.Position.X > block.Box.Current.BL.X - 100 &&
                                Core.Data.Position.X < block.Box.Current.TR.X + 100)
                            {
                                float NewBottom = block.Box.Current.BL.Y;
                                // If ceiling has a left neighbor make sure we aren't too close to it
                                if (block.BlockCore.TopLeftNeighbor != null)
                                {
                                    if (NewBottom > block.BlockCore.TopLeftNeighbor.Box.Current.BL.Y - 100)
                                        NewBottom = Math.Max(NewBottom, block.BlockCore.TopLeftNeighbor.Box.Current.BL.Y + 120);
                                }
                                block.Extend(Side.Bottom, Math.Max(NewBottom, Math.Max(Box.Target.TR.Y, Box.Current.TR.Y) + CeilingParams.BufferSize.GetVal(Core.Data.Position)));
                                if (block.Box.Current.Size.Y < 170 ||
                                    block.Box.Current.BL.Y > Core.MyLevel.MainCamera.TR.Y - 75)
                                {
                                    DeleteObj(block);
                                }
                            }
                            continue;
                        }

                        // For lava blocks...
                        if (block is LavaBlock)
                        {
                            // If the computer gets close, move the lava block down
                            if (Box.Current.TR.X > block.Box.Current.BL.X &&
                                Box.Current.BL.X < block.Box.Current.TR.X)
                            {
                                Core.MyLevel.PushLava(Box.Target.BL.Y - 60, block as LavaBlock);
                            }
                            continue;
                        }

                        if (!block.IsActive) continue;

                        ColType Col = Phsx.CollisionTest(Box, block.Box);
                        bool Overlap;
                        if (!block.Box.TopOnly || block.Core.GenData.RemoveIfOverlap)
                            Overlap = Phsx.BoxBoxOverlap(Box, block.Box);
                        else
                            Overlap = false;
                        if (Col != ColType.NoCol || Overlap)
                        {
                            if (block.BlockCore.Ceiling)
                            {
                                block.Extend(Side.Bottom, Math.Max(block.Box.Current.BL.Y, Math.Max(Box.Target.TR.Y, Box.Current.TR.Y) + CeilingParams.BufferSize.GetVal(Core.Data.Position)));
                                continue;
                            }
                            
                            bool Delete = false;
                            bool MakeTopOnly = false;
                            if (SaveNoBlock) Delete = true;
                            if (BottomCol && Col == ColType.Top) Delete = true;
                            //if (Col == ColType.Top && Core.Data.Position.Y > TargetPosition.Y) Delete = true;
                            if (Col == ColType.Top && WantsToLand == false) Delete = true;
                            if (Col == ColType.Bottom && Core.Data.Position.Y < TargetPosition.Y) Delete = true;
                            if (Col == ColType.Left || Col == ColType.Right) MakeTopOnly = true;// Delete = true;
                            if (TopCol && Col == ColType.Bottom) Delete = true;
                            //if (block is MovingBlock2 && Col == ColType.Bottom) Delete = true;
                            if (Col == ColType.Bottom) Delete = true;
                            //if (CurPhsxStep < 2) Delete = true;
                            if (Overlap && Col == ColType.NoCol && !block.Box.TopOnly && !(block is NormalBlock && !block.BlockCore.NonTopUsed)) Delete = true;
                            if ((Col == ColType.Bottom || Overlap) && Col != ColType.Top) MakeTopOnly = true;
                            if ((Col == ColType.Left || Col == ColType.Right) && Col != ColType.Top)
                            {
                                if (Box.Current.TR.Y < block.Box.Current.TR.Y)
                                    MakeTopOnly = true;
                                else
                                    MakeTopOnly = true;
                                //MakeTopOnly = false;
                                //Delete = true;
                            }
                            if (block.BlockCore.NonTopUsed || !(block is NormalBlock))
                                if (MakeTopOnly)
                                {
                                    MakeTopOnly = false;
                                    Delete = true;
                                }

                            // Do not allow for TopOnly blocks
                            // NOTE: Wanted to have this uncommented, but needed to comment it
                            // out in order for final door blocks to work right
                            // (otherwise a block might be used and made not-toponly and block the computer)
                            //if (MakeTopOnly) { MakeTopOnly = false; Delete = true; }

                            if (MakeTopOnly && block.BlockCore.DeleteIfTopOnly)
                            {
                                if (block.Core.GenData.Used)
                                    MakeTopOnly = Delete = false;
                                else
                                    Delete = true;
                            }

                            if (MakeTopOnly)
                            {
                                block.Extend(Side.Bottom, Math.Max(block.Box.Current.BL.Y, Math.Max(Box.Target.TR.Y, Box.Current.TR.Y) + CeilingParams.BufferSize.GetVal(Core.Data.Position)));
                                ((NormalBlock)block).CheckHeight();
                                if (Col != ColType.Top)
                                    Col = ColType.NoCol;
                            }

                            // Don't land on the very edge of the block
                            if (!Delete && !MyPhsx.OnGround)
                            {
                                float Safety = block.BlockCore.GenData.EdgeSafety;
                                if (Box.BL.X > block.Box.TR.X - Safety ||
                                    Box.TR.X < block.Box.BL.X + Safety)
                                {
                                    Delete = true;
                                }
                            }

                            // Don't land on a block that says not to
                            bool DesiresDeletion = false;
                            {
                                if (block.Core.GenData.TemporaryNoLandZone ||
                                    !block.Core.GenData.Used && !block.PermissionToUse())
                                    DesiresDeletion = Delete = true;
                            }


                            if (block.Core.GenData.Used) Delete = false;
                            if (!DesiresDeletion && block.Core.GenData.AlwaysLandOn && !block.Core.MarkedForDeletion && Col == ColType.Top) Delete = false;
                            if (!DesiresDeletion && block.Core.GenData.AlwaysLandOn_Reluctantly && WantsToLand_Reluctant && !block.Core.MarkedForDeletion && Col == ColType.Top) Delete = false;
                            if (Overlap && block.Core.GenData.RemoveIfOverlap) Delete = true;
                            if (!DesiresDeletion && block.Core.GenData.AlwaysUse && !block.Core.MarkedForDeletion) Delete = false;

                            // Shift bottom of block if necessary
                            if (!Delete && !block.BlockCore.DeleteIfTopOnly)
                            {
                                float NewBottom = Math.Max(block.Box.Current.BL.Y,
                                                           Math.Max(Box.Target.TR.Y, Box.Current.TR.Y) + CeilingParams.BufferSize.GetVal(Core.Data.Position));

                                if (block is NormalBlock &&
                                    (Col == ColType.Bottom || Overlap) && Col != ColType.Top &&
                                    !block.BlockCore.NonTopUsed)
                                {
                                    block.Extend(Side.Bottom, NewBottom);
                                    ((NormalBlock)block).CheckHeight();
                                }

                                // Delete the box if it was made TopOnly but TopOnly is not allowed for this block
                                if (block.Box.TopOnly && block.BlockCore.DeleteIfTopOnly)
                                    Delete = true;
                            }

                            // We're done deciding if we should delete the block or not.
                            // If we should delete it, delete.
                            if (Delete)
                            {
                                DeleteObj(block);
                                block.IsActive = false;
                            }
                            // Otherwise keep it and interact with it
                            else
                            {
                                Delete = false;

                                if (Col != ColType.NoCol)
                                {
                                    // We changed the blocks property, so Bob may no longer be on a collision course with it. Check to see if he is before marking block as used.
                                    if (!block.Box.TopOnly || Col == ColType.Top)
                                    {
                                        if (block.Core.GenData.RemoveIfUsed)
                                            Delete = true;

                                        if (!Delete)
                                        {
                                            InteractWithBlock(block.Box, block, Col);
                                            block.StampAsUsed(CurPhsxStep);

                                            // Normal blocks delete surrounding blocks when stamped as used
                                            if (block.Core.GenData.DeleteSurroundingOnUse && block is NormalBlock)
                                                foreach (Block nblock in Core.MyLevel.Blocks)
                                                {
                                                    NormalBlock Normal = nblock as NormalBlock;
                                                    if (null != Normal && !Normal.Core.MarkedForDeletion && !Normal.Core.GenData.AlwaysUse)
                                                        if (!Normal.Core.GenData.Used &&
                                                            Math.Abs(Normal.Box.Current.TR.Y - block.Box.TR.Y) < 15 &&
                                                            !(Normal.Box.Current.TR.X < block.Box.Current.BL.X - 350 || Normal.Box.Current.BL.X > block.Box.Current.TR.X + 350))
                                                        {
                                                            DeleteObj(Normal); 
                                                            Normal.IsActive = false;
                                                        }
                                                }

                                            // Ghost blocks delete surrounding blocks when stamped as used
                                            if (block is GhostBlock)
                                                foreach (Block gblock in Core.MyLevel.Blocks)
                                                {
                                                    GhostBlock ghost = gblock as GhostBlock;
                                                    if (null != ghost && !ghost.Core.MarkedForDeletion)
                                                        if (!ghost.Core.GenData.Used &&
                                                            (ghost.Core.Data.Position - block.Core.Data.Position).Length() < 200)
                                                        {
                                                            DeleteObj(ghost);
                                                            ghost.IsActive = false;
                                                        }
                                                }
                                        }
                                    }
                                }

                                Delete = false;
                                if (block.Core.GenData.RemoveIfOverlap)
                                {
                                    if (Phsx.BoxBoxOverlap(Box, block.Box))
                                        Delete = true;
                                }
                            }
                        }
                    }
                }
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);
        }
        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnUsed() { }
public void OnMarkedForDeletion() { }
public void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
//StubStubStubEnd6
    }
}
