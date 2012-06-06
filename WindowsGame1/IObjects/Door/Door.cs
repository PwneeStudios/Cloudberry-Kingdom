using System;
using System.IO;

using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public delegate void DoorAction(Door door);
    public class Door : ObjectBase, ILevelConnector
    {
        public class DoorTileInfo
        {
            public TextureOrAnim Sprite = null;
            public Vector2 Size = Vector2.One;
            public bool Show = true;
        }

        public enum Types { Brick, Rock, Grass, Dark };
        public Types MyType;

        LevelSeedData _NextLevelSeedData;
        public LevelSeedData NextLevelSeedData { get { return _NextLevelSeedData; } set { _NextLevelSeedData = value; } }

        public NormalBlock MyBackblock;

        public override void Release()
        {
            base.Release();

            InteractingBob = null;
            MyBackblock = null;

            OnOpen = OnEnter = ExtraPhsx = null;
        }

        public bool SkipPhsx;

        /// <summary>
        /// Block the door immediately after a Bob appears in it, so that the player doesn't accidently go back through.
        /// </summary>
        bool TemporaryBlock;
        
        public Vector2 DoorSize, DoorOffset;

        public bool Locked;

        bool UseObject = false; // Whether to use an Object or a QuadClass to draw the door
        public QuadClass MyQuad;
        public SimpleObject MyObject;

        DoorAction _OnOpen;
        public DoorAction OnOpen { get { return _OnOpen; } set { _OnOpen = value; } }
        public DoorAction OnEnter, ExtraPhsx;

        PressNote MyPressNote;

        public override void MakeNew()
        {
            UseObject = false;
            MyType = Types.Brick;

            Core.Init();
            Core.MyType = ObjectType.Door;
            Core.DrawLayer = 1;
            Core.DrawLayer2 = 1;
            Core.DrawSubLayer = 1000; Core.FixSubLayer = true;
            Core.ResetOnlyOnReset = true;

            Core.EditHoldable = true;

            UsedOnce = false;

            MoveFeet = false;
            NoNote = false;

            Locked = false;

            MyBackblock = null;

            if (!Core.BoxesOnly)
            {
                //if (UseObject)
                {
                    MyObject = new SimpleObject(Prototypes.Door, false);

                    MyObject.Base.e1 = new Vector2(600, 0);
                    MyObject.Base.e2 = new Vector2(0, 600);

                    SetObjectState();
                }
                //else
                {
                    MyQuad.Quad.Init();
                    MyQuad.Quad.MyEffect = Tools.BasicEffect;
                    MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName("Castle_Door");

                    MyQuad.Quad.EnforceTextureRatio();
                    MyQuad.Base.e1 = new Vector2(90, 0);
                    MyQuad.Base.e2 = new Vector2(0, 90);
                }
            }
        }

        bool _Layered = true;
        public bool Layered
        {
            get { return _Layered; }
            set
            {
                _Layered = value;
                if (Layered) IndexOfBack = IndexOfFront = 0;
                else IndexOfBack = IndexOfFront = -1;
            }
        }

        /// <summary>
        /// Sets the door to a default type associated with the given tile set.
        /// </summary>
        public void SetDoorType(TileSet TileSetType)
        {
            SetColor(Color.White);

            SetDoorType(TileSetType.DoorType);
            SetColor(TileSetType.Tint);
            
            //switch (TileSetType)
            //{
            //    case TileSets.Rain:
            //    case TileSets.Terrace: SetDoorType(Types.Grass); break;
            //    case TileSets.DarkTerrace: SetDoorType(Types.Grass);
            //        SetColor(TileSets.DarkTerrace.Tint); break;
            //    case TileSets.Dungeon: SetDoorType(Types.Rock); break;
            //    case TileSets.Dark: SetDoorType(Types.Dark); break;
            //    default: SetDoorType(Types.Brick); break;
            //}
        }

        public void SetColor(Vector4 color) { SetColor(new Color(color)); }

        public void SetColor(Color color)
        {
            if (MyObject != null)
                MyObject.SetColor(color);
        }

        /// <summary> This is the quad index of the back quad to the door's object. </summary>
        int IndexOfBack = 0;

        /// <summary> This is the quad index of the front quad to the door's object. </summary>
        int IndexOfFront = 0;

        /// <summary>
        /// Set the graphical style of the door.
        /// </summary>
        public void SetDoorType(Types DoorType)
        {
            MyType = DoorType;
            UseObject = true;

            switch (DoorType) {
                case Types.Brick:
                    MyObject = new SimpleObject(Prototypes.Door, false);
                    MyObject.Base.SetScale(new Vector2(608, 600));
                    break;
                case Types.Rock:
                    MyObject = new SimpleObject(Prototypes.Door, false);
                    MyObject.Quads[MyObject.GetQuadIndex("Frame")].TextureName = "pillardoor";
                    MyObject.Base.SetScale(new Vector2(608, 600));
                    break;
                case Types.Grass:
                    MyObject = new SimpleObject(Prototypes.GrassDoor, false);
                    MyObject.Base.SetScale(new Vector2(615, 625));
                    break;
                case Types.Dark:
                    MyObject = new SimpleObject(Prototypes.Door, false);
                    MyObject.Quads[MyObject.GetQuadIndex("Front")].TextureName = "darkdoor";
                    MyObject.Quads[MyObject.GetQuadIndex("Back")].TextureName = "darkdoorback";
                    MyObject.Quads[MyObject.GetQuadIndex("Frame")].TextureName = "darkframe";
                    MyObject.Base.SetScale(new Vector2(608, 600));
                    break;
            }

            if (IndexOfBack >= 0)
            {
                IndexOfBack = MyObject.GetQuadIndex("Back");
                IndexOfFront = MyObject.GetQuadIndex("Front");
            }

            SetObjectState();
        }

        public Door(bool BoxesOnly)
        {
            Core.BoxesOnly = BoxesOnly;

            MyObject = new SimpleObject(Prototypes.Door, false);
            MyQuad = new QuadClass();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public Vector2 GetBottom()
        {
            if (UseObject)
            {
                Update();
                //MyObject.Update();
                return new Vector2(Core.Data.Position.X, MyObject.Quads[0].BL.Y + 11.5f);
            }
            else
            {
                MyQuad.Update();
                return new Vector2(Core.Data.Position.X, MyQuad.BL.Y + 11.5f);
            }
        }

        public Vector2 GetTop()
        {
            if (UseObject)
            {
                Update();
                //MyObject.Update();
                return new Vector2(Core.Data.Position.X, MyObject.Quads[0].TR.Y + 11.5f);
            }
            else
            {
                MyQuad.Update();
                return new Vector2(Core.Data.Position.X, MyQuad.TR.Y + 11.5f);
            }
        }

        /// <summary>
        /// Update the graphical display of the door to reflect whether it's locked or unlocked.
        /// </summary>
        void SetObjectState()
        {
            if (UseObject)
            {
                if (Locked)
                {
                    MyObject.Read(0, 0);
                    MyObject.Quads[MyObject.GetQuadIndex("Back")].Hide = true;
                    MyObject.Quads[MyObject.GetQuadIndex("Front")].Hide = false;
                }
                else
                {
                    MyObject.Read(1, 0);
                    MyObject.Quads[MyObject.GetQuadIndex("Back")].Hide = false;
                    MyObject.Quads[MyObject.GetQuadIndex("Front")].Hide = true;
                }
                MyObject.Play = false;
            }
            else
            {
                string texname = MyQuad.Quad.MyTexture.Path;
                if (texname.Contains("_locked"))
                    texname = texname.Substring(0, texname.IndexOf("_locked"));

                if (Locked)
                {
                    MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName(texname + "_locked");
                    if (MyQuad.Quad.MyTexture == Tools.TextureWad.TextureList[0])
                        MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName(texname);
                }
                else
                {
                    MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName(texname);
                }
            }
        }

        public void HideBobs()
        {
            Core.MyLevel.Bobs.ForEach(bob => bob.Core.Show = false);
        }

        public void ShowBobs()
        {
            Core.MyLevel.Bobs.ForEach(bob => bob.Core.Show = true);
        }

        public bool SuppressSound = false;

        public void SetLock(bool Locked) { SetLock(Locked, false, false, true); }
        public void SetLock(bool Locked, bool AlwaysSet, bool PlaySound) { SetLock(Locked, AlwaysSet, PlaySound, true); }
        public void SetLock(bool Locked, bool AlwaysSet, bool PlaySound, bool Hide)
        {
            if (this.Locked != Locked || AlwaysSet)
            {
                this.Locked = Locked;

                SetObjectState();

                if (Locked)
                {
                    // Hide the player closing the door
                    if (Hide && HideInteractingBobOnDoorClose && InteractingBob != null)
                    {
                        if (ActivatingBob != null)
                        {
                            ActivatingBob.Core.Show = false;
                            ActivatingBob.SetLightSourceToFade();
                        }
                        else
                        {
                            InteractingBob.Core.Show = false;
                            InteractingBob.SetLightSourceToFade();
                        }
                    }

                    if (PlaySound && !SuppressSound)
                        InfoWad.GetSound("DoorClose").Play();
                }
                else
                {
                    if (PlaySound && !SuppressSound)
                        InfoWad.GetSound("DoorOpen").Play();
                }
            }
        }

        public Vector2 ModDoorSize = Vector2.One;
        public Vector2 GetSize()
        {
            if (UseObject)
            {
                SimpleQuad quad = MyObject.Quads[0];
                return ModDoorSize *
                    new Vector2(quad.v1.Pos.X * MyObject.Base.e1.X, quad.v1.Pos.Y * MyObject.Base.e2.Y);
            }
            else
            {
                return ModDoorSize * 
                    new Vector2(MyQuad.Quad.v1.Pos.X * MyQuad.Base.e1.X, MyQuad.Quad.v1.Pos.Y * MyQuad.Base.e2.Y);
            }
        }

        public void Update()
        {
            if (Core.BoxesOnly) return;

            if (UseObject)
            {
                // Flip xFlips if the camera is horizontally flipped!
                if (Core.MyLevel != null && Core.MyLevel.ModZoom.X < 0)
                    MyObject.xFlip = true;
                else
                    MyObject.xFlip = false;

                // Flip yFlips if the camera is vertically flipped!
                if (Core.MyLevel != null && Core.MyLevel.ModZoom.Y < 0)
                    MyObject.yFlip = true;
                else
                    MyObject.yFlip = false;

                MyObject.Base.Origin = Core.Data.Position;
                MyObject.Update();
            }
            else
                MyQuad.Base.Origin = Core.Data.Position;
        }


        public void MakeNote()
        {
            if (Core.BoxesOnly || Core.MyLevel == null)
                return;

            // Don't show a note for this door if it has been used before.
            if (UsedOnce) return;

            if (MyPressNote == null)
            {
                MyPressNote = new PressNote(this);
                Core.MyLevel.MyGame.AddGameObject(MyPressNote);
            }
            else
                MyPressNote.FadeIn();
        }

        public void KillNote()
        {
            if (MyPressNote != null)
            {
                MyPressNote.CollectSelf();
                MyPressNote = null;
            }
        }

        int ShakeStep, ShakeIntensity;
        public void Shake(int Length, int Intensity, bool Sound)
        {
            ShakeStep = Length;
            ShakeIntensity = Intensity;
            save = Pos;

            if (Sound)
                Tools.SoundWad.FindByName("Bash").Play(1f);
        }

        void DoShake()
        {
            if (ShakeStep > 0)
            {
                if (step % 2 == 0)
                {
                    Pos = save;
                    Pos += new Vector2(MyLevel.Rnd.Rnd.Next(-ShakeIntensity, ShakeIntensity), MyLevel.Rnd.Rnd.Next(-ShakeIntensity, ShakeIntensity));
                }

                ShakeStep--;
                if (ShakeStep == 0)
                    Pos = save;
            }
        }

        int step = 0;
        bool shake;
        Vector2 save;
        public override void PhsxStep()
        {
            DoShake();

            if (ExtraPhsx != null)
                ExtraPhsx(this);
        }

        bool OnScreen()
        {
            if (Core.BoxesOnly) return false;

            float Grace = 300;
            if (Core.MyLevel.ModZoom.X < 0) Grace += 500;
            if (UseObject)
            {
                if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + Grace + MyObject.Base.e1.X || Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + Grace + MyObject.Base.e2.Y)
                    return false;
                if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - Grace - MyObject.Base.e1.X || Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 500 - MyObject.Base.e2.Y)
                    return false;
            }
            else
            {
                if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + Grace + MyQuad.Base.e1.X || Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + Grace + MyQuad.Base.e2.Y)
                    return false;
                if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - Grace - MyQuad.Base.e1.X || Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 500 - MyQuad.Base.e2.Y)
                    return false;
            }

            return true;
        }
        
        public override void Draw()
        {
            if (!OnScreen() || !Core.Active) return;

            if (Tools.DrawGraphics)
            {
                Update();

                if (!Info.Doors.Show) return;

                if (UseObject)
                {
                    // Draw the first layer of the door
                    if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
                    {
                        MyObject.Draw();
                    }
                    // Draw the second layer of the door
                    else
                    {
                        if (IndexOfBack >= 0)
                            //Tools.QDrawer.DrawQuad(ref MyObject.Quads[IndexOfBack]);
                            MyObject.DrawQuad(ref MyObject.Quads[IndexOfBack]);

                        if (IndexOfFront >= 0)
                            //Tools.QDrawer.DrawQuad(ref MyObject.Quads[IndexOfFront]);
                            MyObject.DrawQuad(ref MyObject.Quads[IndexOfFront]);
                    }                    
                }
                else
                    MyQuad.Draw();
            }
        }

        /// <summary>
        /// Moves the foot of the door to the specified position.
        /// </summary>
        /// <param name="pos"></param>
        public void PlaceAt(Vector2 pos)
        {
            Move(pos - GetBottom());
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            Update();
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;

            MyPressNote = null;
        }

        public void MoveBobs()
        {
            Core.MyLevel.Bobs.ForEach(bob => MoveBobToHere(bob));
        }

        public bool MoveFeet = false;
        public void MoveBobToHere(Bob bob)
        {
            if (MoveFeet)
            {
                bob.Move(GetBottom() - bob.Feet() + new Vector2(0, 1));
            }
            else
                bob.Move(Core.Data.Position - bob.Core.Data.Position);

            bob.Core.Data.Velocity = Vector2.Zero;
            TemporaryBlock = true;
        }

        /// <summary>
        /// The Bob interacting with the door.
        /// </summary>
        public Bob InteractingBob;

        /// <summary>
        /// The Bob activating the door.
        /// </summary>
        public Bob ActivatingBob;

        /// <summary>
        /// When true, when a player closes the door he is hid.
        /// </summary>
        public bool HideInteractingBobOnDoorClose = true;

        /// <summary>
        /// How long a player has been near enough to the door to open it.
        /// </summary>
        int NearCount = 0;

        public void ClearNote()
        {
            MyPressNote = null;
            NearCount = 0;
        }

        public bool NoNote = false;

        /// <summary>
        /// How many frames to wait while the player is close to the door
        /// before showing a note that explains how to open the door.
        /// </summary>
        int DelayToShowNote
        {
            get
            {
                if (NoNote) return 100000;

                if (DoorOperated == 0) return _DelayToShowNote_First;
                else if (DoorOperated == 1) return _DelayToShowNote_Second;
                else return _DelayToShowNote;
            }
        }

        /// <summary> This length is used only for the first time a player opens a door. </summary>
        int _DelayToShowNote_First = 48;
        int _DelayToShowNote_Second = 75;
        /// <summary> This length is used for every time the player has already opened a door at least once. </summary>
        int _DelayToShowNote = 180;


        /// <summary>
        /// Number of times a door has been successfully operated by a player.
        /// (since the game has launched).
        /// </summary>
        static int DoorOperated = 0;

        public bool UsedOnce = false;

        /// <summary>
        /// When true the player will exit through a door automatically, without pressing anything.
        /// </summary>
        public static bool AutoOpen = false;

        public static bool AllowCompControl = false;
        public override void Interact(Bob bob)
        {
            if (Locked || OnOpen == null)
                return;

            // Don't interact with code controlled Bobs
            if (bob.CodeControl) return;

            InteractingBob = bob;

            //DoorSize = new Vector2(MyQuad.Base.e1.X + 50, MyQuad.Base.e2.Y);
            DoorSize = GetSize();

            bool InteractedWith = false;
            if (((bob.MyObjectType is BobPhsxSpaceship &&
                 (bob.Core.Data.Position - Core.Data.Position - DoorOffset).LengthSquared() < 3000 + Math.Max(0, 1400 * (bob.MyPhsx.ScaledFactor - 1))) ||
                (Math.Abs(bob.Core.Data.Position.X - Core.Data.Position.X - DoorOffset.X) < DoorSize.X - 15 + .025f * bob.Box.Current.Size.X + Math.Max(0, 45 * (bob.MyPhsx.ScaledFactor - 1)) &&
                 Math.Abs(bob.Core.Data.Position.Y - Core.Data.Position.Y - DoorOffset.Y) < DoorSize.Y + 50 + Math.Max(0, 80 * (bob.MyPhsx.ScaledFactor - 1)))) &&
                (!bob.CompControl || AllowCompControl) && !Core.MyLevel.Watching && !Core.MyLevel.Replay)
            {
                NearCount++;
                if (NearCount > DelayToShowNote || MyPressNote != null)
                    MakeNote();

#if WINDOWS
                if (ButtonCheck.State(ControllerButtons.X, (int)bob.MyPlayerIndex).Down ||
                    (bob.CurInput.xVec.Y > .85f && bob.GetPlayerData().KeyboardUsedLast) || AutoOpen)
#else
                if (ButtonCheck.State(ControllerButtons.X, bob.MyPlayerIndex).Down || AutoOpen)
#endif
                {
                    InteractedWith = true;
                    HaveBobUseDoor(bob);
                }
            }
            else
            {
                NearCount = Tools.Restrict(0, 30, NearCount);
                NearCount--;
            }

            if (!InteractedWith)
                TemporaryBlock = false;
        }

        public void Do() { OnOpen(this); }

        public void HaveBobUseDoor(Bob bob)
        {
            ActivatingBob = bob;
            UsedOnce = true;

            if (!TemporaryBlock)
            {
                bob.TemporaryPlaceBlock = true;
                OnOpen(this);
                DoorOperated++;
            }
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Door DoorA = A as Door;

            DoorA.MyQuad.Clone(MyQuad);
            Locked = DoorA.Locked;
            ModDoorSize = DoorA.ModDoorSize;
            SuppressSound = DoorA.SuppressSound;
        }

        public override void Write(BinaryWriter writer)
        {
            Core.Write(writer);

            MyQuad.Write(writer);
        }

        public override void Read(BinaryReader reader)
        {
            Core.Read(reader);

            MyQuad.Read(reader);
        }
    }
}