using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Drawing;

using WindowsGame1.Levels;
using WindowsGame1.Bobs;

namespace WindowsGame1
{
    public delegate void DoorAction(Door door);
    public class Door : IObject
    {
        public void Release()
        {
            Core.Release();
        }

        public int DelayToClose;

        public bool SkipPhsx;
        bool Blocked;
        public bool UseCustomDoorSize;
        public Vector2 DoorSize, DoorOffset;

        public bool Locked, LockOnLevelCompletion;

        public LevelSeedData LevelSeed;
        public int BeatenCode; // Negative if level hasn't been beaten yet, the return code of the beaten level otherwise

        public QuadClass MyQuad;
        public Stars MyStars;

        public List<Door> Connections;

        public DoorAction OverrideAction, ExtraAction, PostMakeAction;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public void SetLevelSeed(LevelSeedData Seed)
        {
            LevelSeed = Seed;
            BeatenCode = -1;
        }

        public void MakeNew()
        {
            Core.Init();
            Core.MyType = ObjectType.Door;
            Core.DrawLayer = 1;
            Core.ResetOnlyOnReset = true;

            Core.EditHoldable = true;

            Locked = false;

            BeatenCode = 0;

            Connections.Clear();

            if (!Core.BoxesOnly)
            {
                MyQuad.Quad.Init();
                MyQuad.Quad.MyEffect = Tools.EffectWad.FindByName("Basic");
                MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName("Castle_Door");

                MyQuad.Quad.EnforceTextureRatio();
                MyQuad.Base.e1 = new Vector2(90, 0);
                MyQuad.Base.e2 = new Vector2(0, 90);

                MyStars.MakeNew();
            }
        }

        public Door(bool BoxesOnly)
        {
            CoreData = new ObjectData();

            Core.BoxesOnly = BoxesOnly;

            Connections = new List<Door>();

            MyQuad = new QuadClass();
            MyStars = new Stars();

            /*
            if (Tools.Rnd.Next(0,100) > 50)
                MyStars.SetMode(StarMode.Red);
            else
                MyStars.SetMode(StarMode.Text);
            MyStars.SetMode(StarMode.Draw);

            MyStars.SetStars(Tools.Rnd.Next(1,6));

             */

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void SetLock(bool Locked) { SetLock(Locked, false, false); }
        public void SetLock(bool Locked, bool AlwaysSet, bool PlaySound)
        {
            if (this.Locked != Locked || AlwaysSet)
            {
                this.Locked = Locked;

                string texname = MyQuad.Quad.MyTexture.Path;
                if (texname.Contains("_locked"))
                    texname = texname.Substring(0, texname.IndexOf("_locked"));
                if (Locked)
                {
                    MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName(texname + "_locked");
                    if (MyQuad.Quad.MyTexture == Tools.TextureWad.TextureList[0])
                        MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName(texname);
                    if (PlaySound)
                        InfoWad.GetSound("DoorClose").Play();
                }
                else
                {
                    MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName(texname);
                    if (PlaySound)
                        InfoWad.GetSound("DoorOpen").Play();
                }
            }
        }

        public void ConnectTo(Door door)
        {
            Connections.Clear();
            Connections.Add(door);
        }

        public void Connect(Door door)
        {
            ConnectTo(door);
            door.ConnectTo(this);
        }


        public Vector2 GetSize()
        {
            return new Vector2(MyQuad.Quad.v1.Pos.X * MyQuad.Base.e1.X, MyQuad.Quad.v1.Pos.Y * MyQuad.Base.e2.Y);
        }

        public void Update()
        {
            if (Core.BoxesOnly) return;

            MyQuad.Base.Origin = Core.Data.Position;
            MyStars.Pos = Core.Data.Position;
        }


        public void PhsxStep()
        {
            if (DelayToClose > 0)
            {
                DelayToClose--;

                if (DelayToClose == 0)
                {
                    SetLock(true, true, true);
                }
            }
        }

        public void PhsxStep2() { }

        bool OnScreen()
        {
            if (Core.BoxesOnly) return false;

            if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 150 + MyQuad.Base.e1.X || Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 150 + MyQuad.Base.e2.Y)
                return false;
            if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 150 - MyQuad.Base.e1.X || Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 500 - MyQuad.Base.e2.Y)
                return false;

            return true;
        }

        public void TextDraw()
        {
            if (!OnScreen()) return;

            if (Tools.DrawGraphics)
            {
                MyStars.Draw(true);
            }
        }
        
        public void Draw()
        {
            if (!OnScreen()) return;

            if (Tools.DrawGraphics)
            {
                Update();
                MyQuad.Draw();

                MyStars.Draw(false);
            }
        }

        public void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            Update();
        }

        public void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public void MoveBobToHere(Bob bob)
        {
            bob.Move(Core.Data.Position - bob.Core.Data.Position);
            bob.Core.Data.Velocity = Vector2.Zero;
            Blocked = true;
        }

        public void Interact(Bob bob)
        {
            if (Locked)
                return;

            if (!UseCustomDoorSize)
                DoorSize = new Vector2(MyQuad.Base.e1.X, MyQuad.Base.e2.Y);

            if (bob.CurInput.xVec.Y > .85f &&
                ((bob.MyObjectType == Bob.HeroType.Spaceship &&
                 (bob.Core.Data.Position - Core.Data.Position - DoorOffset).LengthSquared() < 3000) ||
                (Math.Abs(bob.Core.Data.Position.X - Core.Data.Position.X - DoorOffset.X) < DoorSize.X - bob.Box.Current.Size.X && 
                 Math.Abs(bob.Core.Data.Position.Y - Core.Data.Position.Y - DoorOffset.Y) < DoorSize.Y + 50)) &&
                !bob.CompControl && !Core.MyLevel.Watching && !Core.MyLevel.Replay)
            {
                if (!Blocked)
                {
                    if (OverrideAction != null)
                        OverrideAction(this);
                    else
                    {
                        if (LevelSeed != null && BeatenCode < 0)
                        {
                            //if (!WorldMapGameData.LockLevelStart)
                            {
                                if (ExtraAction != null)
                                    ExtraAction(this);

                                Tools.CurGameData.CurDoor = this;
                                GameData.StartLevel(LevelSeed);

                                if (PostMakeAction != null)
                                    PostMakeAction(this);
                            }
                        }
                        else
                        {
                            if (Connections.Count > 0)
                                Connections[BeatenCode].MoveBobToHere(bob);
                        }
                    }
                }
            }
            else
                Blocked = false;
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            Door DoorA = A as Door;

            DoorA.MyQuad.Clone(MyQuad);
            Locked = DoorA.Locked;
        }

        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);

            MyQuad.Write(writer);
        }
        public void Read(BinaryReader reader)
        {
            Core.Read(reader);

            MyQuad.Read(reader);
        }
    }
}