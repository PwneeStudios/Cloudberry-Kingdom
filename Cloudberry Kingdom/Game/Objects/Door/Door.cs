using System;
using System.IO;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom.InGameObjects
{
    public delegate void DoorAction(Door door);
    public class Door : ObjectBase, ILevelConnector
    {
        public class DoorTileInfo : TileInfoBase
        {
            public SpriteInfo Sprite = new SpriteInfo(null);
            public bool Show = true;

            public Vector2 SizePadding = Vector2.Zero;

            public Vector2 ShiftBottom = Vector2.Zero;
            public Vector2 ShiftStart = Vector2.Zero;
            public Vector2 CollisionSize = new Vector2(100, 200);

            public EzSound DoorOpen = Tools.NewSound("Door_Opening", 1);
            public EzSound DoorClose = Tools.NewSound("Door_Slamming", 1);

            public SpriteInfo Sign = new SpriteInfo("Sign_Off", new Vector2(275, -1));
            public bool ShowSign = false;
        }

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
        
        public bool Locked;

        public QuadClass MyQuad;

        DoorAction _OnOpen;
        public DoorAction OnOpen { get { return _OnOpen; } set { _OnOpen = value; } }

        DoorAction _OnEnter;
        public DoorAction OnEnter { get { return _OnEnter; } set { _OnEnter = value; } }
        public DoorAction ExtraPhsx;

        PressNote MyPressNote;

        public override void MakeNew()
        {
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
        }

        public Vector2 ShiftStart = Vector2.Zero;
        public Vector2 ShiftBottom = Vector2.Zero;
        public Vector2 DoorSize;

        public bool Mirror = false;

        Vector2 HitBoxPadding = Vector2.Zero;

        /// <summary>
        /// Sets the door to a default type associated with the given tile set.
        /// </summary>
        public void SetDoorType(TileSet TileSetType, Level level)
        {
            if (level != null && level.CurMakeData != null && level.CurMakeData.PieceSeed != null)
                HitBoxPadding = level.Style.DoorHitBoxPadding;

            Core.MyTileSet = TileSetType;

            var info = TileSetType.MyTileSetInfo.Doors;
            MyQuad.Quad.Init();

            if (Mirror)
            {
                info.Sprite.Offset.X *= -1;
                MyQuad.Set(info.Sprite);
                MyQuad.Quad.MirrorUV_Horizontal();
                info.Sprite.Offset.X *= -1;
            }
            else
                MyQuad.Set(info.Sprite);

            ShiftStart = info.ShiftStart;
            ShiftBottom = info.ShiftBottom;
            DoorSize = info.CollisionSize;

            MyQuad.Quad.Playing = false;

            SetObjectState();
        }

        public Door(bool BoxesOnly)
        {
            Core.BoxesOnly = BoxesOnly;

            MyQuad = new QuadClass();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public Vector2 GetBottom()
        {
            MyQuad.Update();
            //return ShiftBottom + new Vector2(Pos.X, MyQuad.BL.Y + 11.5f);
            return Pos;
        }

        public Vector2 GetTop()
        {
            MyQuad.Update();
            //return new Vector2(Pos.X, MyQuad.TR.Y + 11.5f);
            return Pos + new Vector2(0, 400);
        }

        /// <summary>
        /// Update the graphical display of the door to reflect whether it's locked or unlocked.
        /// </summary>
        void SetObjectState()
        {
            if (MyQuad == null || MyQuad.Show == false) return;

            if (Locked)
            {
                MyQuad.Quad.CalcTexture(0, 1);
            }
            else
            {
                MyQuad.Quad.CalcTexture(0, 0);
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
                        Info.Doors.DoorClose.Play();
                }
                else
                {
                    if (PlaySound && !SuppressSound)
                        Info.Doors.DoorOpen.Play();
                }
            }
        }

        public void Update()
        {
            if (Core.BoxesOnly) return;
            
            MyQuad.Base.Origin = Pos;
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
            
            if (Pos.X > Core.MyLevel.MainCamera.TR.X + Grace + MyQuad.Base.e1.X || Pos.Y > Core.MyLevel.MainCamera.TR.Y + Grace + MyQuad.Base.e2.Y)
                return false;
            if (Pos.X < Core.MyLevel.MainCamera.BL.X - Grace - MyQuad.Base.e1.X || Pos.Y < Core.MyLevel.MainCamera.BL.Y - 500 - MyQuad.Base.e2.Y)
                return false;

            return true;
        }
        
        public override void Draw()
        {
            if (!OnScreen() || !Core.Active) return;

            if (Tools.DrawGraphics)
            {
                Update();

                if (!Info.Doors.Show) return;

                MyQuad.Draw();
            }

            if (Tools.DrawBoxes)
            {
                Tools.QDrawer.DrawCircle(Pos, 30, Color.Red);
            }
        }

        /// <summary>
        /// Moves the foot of the door to the specified position.
        /// </summary>
        /// <param name="pos"></param>
        public void PlaceAt(Vector2 pos)
        {
            Move(pos + ShiftStart - Pos);
            //Move(pos - GetBottom());
            //Move(pos - Pos);
        }

        public override void Move(Vector2 shift)
        {
            Pos += shift;
            Update();
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;
            
            SetDoorType(Core.MyTileSet, null);

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
                bob.Move(ShiftStart + GetBottom() - bob.Feet() + new Vector2(0, 1));
            }
            else
                bob.Move(ShiftStart + Pos - bob.Pos);

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
                else return _DelayToShowNote_Nth;
            }
        }

        /// <summary> This length is used only for the first time a player opens a door. </summary>
        int _DelayToShowNote_First = 60;
        int _DelayToShowNote_Second = 75;
        int _DelayToShowNote_Nth = 180;


        /// <summary>
        /// Number of times a door has been successfully operated by a player.
        /// (since the game has launched).
        /// </summary>
        static int DoorOperated = 0;

        public bool UsedOnce = false;

        /// <summary>
        /// When true the player will exit through a door automatically, without pressing anything.
        /// </summary>
        public bool AutoOpen = false;

        public static bool AllowCompControl = false;
        public override void Interact(Bob bob)
        {
            if (Locked || OnOpen == null || MyLevel.PlayMode != 0)
                return;

            // Don't interact with code controlled Bobs
            if (bob.CodeControl) return;

            InteractingBob = bob;

            float scale = bob.GetScale().X;

            float x_pad = DoorSize.X + HitBoxPadding.X + Info.Doors.SizePadding.X + 22 + .018f * bob.Box.Current.Size.X + Math.Max(0, 36 * (scale - 1));
            x_pad = CoreMath.Restrict(Math.Abs(bob.Core.Data.Velocity.X * 1.3f), 500, x_pad);
            float y_pad = DoorSize.Y + HitBoxPadding.Y + Info.Doors.SizePadding.X + 50 + Math.Max(0, 80 * (scale - 1));

            bool InteractedWith = false;
            if ((
                (Math.Abs(bob.Pos.X - Pos.X) < x_pad &&
                 Math.Abs(bob.Pos.Y - Pos.Y) < y_pad)) &&
                (!bob.CompControl || AllowCompControl) && !Core.MyLevel.Watching && !Core.MyLevel.Replay)
            {
                NearCount++;
                //if (NearCount > 10 || MyPressNote != null) // For debugging purposes, always have the note show up.
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
                NearCount = CoreMath.Restrict(0, 30, NearCount);
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
                Awardments.CheckForAward_Bungee(MyLevel.MyGame);

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
            SuppressSound = DoorA.SuppressSound;

            HitBoxPadding = DoorA.HitBoxPadding;
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