using Microsoft.Xna.Framework;

using Drawing;
using System.IO;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Particles;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Checkpoint : ObjectBase, IObject
    {
        public void TextDraw() { }
        public void Release()
        {
            Core.Release();
            MyPiece = null;
        }

        public bool Taken, TakenAnimFinished;
        bool GhostFaded;

        float Taken_Scale, Taken_Alpha;

        static EzSound MySound;

        static Particle DieTemplate;
        static bool TemplateInitialized;

        public bool SkipPhsx;

        public bool Touched;

        public AABox Box;
        public SimpleQuad MyQuad;
        public BasePoint Base;

        public SimpleObject MyObject;

        public LevelPiece MyPiece;
        public int MyPieceIndex;

        public void MakeNew()
        {
            Taken = TakenAnimFinished = false;

            Core.Init();
            Core.MyType = ObjectType.Checkpoint;
            Core.DrawLayer = 8;

            Core.ResetOnlyOnReset = true;

            MyPiece = null;
            MyPieceIndex = -1;

            SetAnimation();

            Init();
        }

        public Checkpoint()
        {
            Box = new AABox();

            MyQuad = new SimpleQuad();
            MyObject = new SimpleObject(Prototypes.Checkpoint, false);

            MakeNew();

            Core.BoxesOnly = false;
        }

        void SetAnimation()
        {
            MyObject.Read(0, 0);
            MyObject.Play = true;
            MyObject.Loop = true;
            //MyObject.EnqueueAnimation(0, (float)MyLevel.Rnd.Rnd.NextDouble() * 1.5f, true);
            MyObject.EnqueueAnimation(0, (float)0, true);
            MyObject.DequeueTransfers();
            MyObject.Update();
        }

        public void Revert()
        {
            Taken = false;
            ResetTakenAnim();

            MyObject.SetColor(new Color(1f, 1f, 1f, 1f));
        }

        void ResetTakenAnim()
        {
            TakenAnimFinished = false;
            Taken_Scale = 1;
            Taken_Alpha = 1f;
        }

        public void Die()
        {
            Taken = true;
            ResetTakenAnim();

            if (Core.MyLevel.PlayMode != 0) return;

            Game.CheckpointGrabEvent(this);

            MySound.Play();
        }

        public void Init()
        {
            Vector2 Size = InfoWad.GetVec("Checkpoint_Size");
            Vector2 TextureSize = InfoWad.GetVec("Checkpoint_TextureSize");

            if (!TemplateInitialized)
            {
                TemplateInitialized = true;

                MySound = InfoWad.GetSound("Checkpoint_Sound");

                DieTemplate = new Particle();
                DieTemplate.MyQuad.Init();
                DieTemplate.MyQuad.MyEffect = Tools.BasicEffect;
                DieTemplate.MyQuad.MyTexture = Tools.TextureWad.FindByName(InfoWad.GetStr("Checkpoint_Texture"));
                DieTemplate.SetSize(TextureSize.X);
                DieTemplate.SizeSpeed = new Vector2(10, 10);
                DieTemplate.AngleSpeed = .013f;
                DieTemplate.Life = 20;
                DieTemplate.MyColor = new Vector4(1f, 1f, 1f, .75f);
                DieTemplate.ColorVel = new Vector4(0, 0, 0, -.065f);
            }

            Box.Initialize(Core.Data.Position, Size);

            if (!Core.BoxesOnly)
            {
                MyQuad.MyEffect = Tools.BasicEffect;
                MyQuad.MyTexture = Tools.TextureWad.FindByName(InfoWad.GetStr("Checkpoint_Texture"));
                MyQuad.Init();
            }

            Base.e1 = new Vector2(TextureSize.X, 0);
            Base.e2 = new Vector2(0, TextureSize.Y);

            Update();
        }

        public void AnimStep()
        {
            if (MyObject.DestinationAnim() == 0 && MyObject.Loop)
                MyObject.PlayUpdate(1f/3f);//MyAnimSpeed);
        }

        public void PhsxStep()
        {
            if (!Core.Active) return;

            if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 350 ||
                Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 400 ||
                Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 350 ||
                Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 350)
            {
                SkipPhsx = true;
                return;
            }

            if (Taken && !TakenAnimFinished)
            {
                Taken_Scale += .045f;
                Taken_Alpha -= .035f;
                if (Taken_Alpha < 0)
                {
                    ResetTakenAnim();
                    TakenAnimFinished = true;
                }
            }

            AnimStep();

            Box.SetTarget(Core.Data.Position, Box.Current.Size);
            if (SkipPhsx) Box.SwapToCurrent();

            SkipPhsx = false;
        }

        public void PhsxStep2()
        {
            if (!Core.Active) return;
            if (SkipPhsx) return;

            Box.SwapToCurrent();

            Update();
        }


        public void Update()
        {
            MyObject.Base.Origin -= MyObject.Boxes[0].Center() - Box.Current.Center;

            MyObject.Base.e1.X = 1;
            MyObject.Base.e2.Y = 1;
            MyObject.Update();

            Vector2 CurSize = MyObject.Boxes[0].Size() / 2;
            float Scale = Box.Current.Size.X / CurSize.X;
            if (Taken)
                Scale *= Taken_Scale;
            MyObject.Base.e1.X = Scale;
            MyObject.Base.e2.Y = Scale;

            MyObject.Update();
        }


        public void Reset(bool BoxesOnly)
        {
            Core.Active = true;

            Core.Data.Position = Core.StartData.Position;

            Box.SetTarget(Core.Data.Position, Box.Current.Size);
            Box.SwapToCurrent();

            Update();
        }

        public void Move(Vector2 shift)
        {
            Core.StartData.Position += shift;
            Core.Data.Position += shift;
            Box.Move(shift);
        }

        public void Interact(Bob bob)
        {
            if (Taken) return;
            if (!Core.Active) return;
            if (Core.MyLevel.SuppressCheckpoints || Core.MyLevel.GhostCheckpoints) return;

            ColType Col = Phsx.CollisionTest(bob.Box2, Box);
            if (Col != ColType.NoCol)
            {
                Die();

                if (Core.MyLevel.PlayMode == 0 && MyPiece != null)
                {
                    // Track stats
                    bob.MyStats.Checkpoints++;
                    bob.MyStats.Score += 250;

                    // Erase taken coins
                    Core.MyLevel.KeepCoinsDead();                    

                    // Set current level piece
                    Core.MyLevel.SetCurrentPiece(MyPiece);

                    //////Core.MyLevel.CurPiece = MyPiece;

                    //////// Change piece associated with each bob
                    //////int Count = 0;
                    //////foreach (Bob _bob in bob.Core.MyLevel.Bobs)
                    //////{                        
                    //////    _bob.MyPiece = MyPiece;
                    //////    _bob.MyPieceIndex = Count % MyPiece.NumBobs;

                    //////    Count++;
                    //////}

                    // Game's checkpoint action
                    Core.MyLevel.MyGame.GotCheckpoint(bob);

                    // Kill other checkpoints
                    foreach (IObject obj in Core.MyLevel.Objects)
                    {
                        Checkpoint checkpoint = obj as Checkpoint;
                        if (null != checkpoint)
                            if (checkpoint.MyPiece == MyPiece)
                                checkpoint.Die();
                    }
                }
            }
        }

        public void SetAlpha()
        {
            if (Core.MyLevel.GhostCheckpoints)
            {
                if (!GhostFaded)
                {
                    MyQuad.SetColor(new Color(255, 255, 255, 90));
                    MyObject.SetColor(new Color(255, 255, 255, 90));
                    GhostFaded = true;
                }
            }
            else
            {
                if (GhostFaded)
                {
                    MyQuad.SetColor(new Color(255, 255, 255, 255));
                    MyObject.SetColor(new Color(255, 255, 255, 255));
                    GhostFaded = false;
                }

                if (Taken)
                {
                    MyObject.SetColor(new Color(1f, 1f, 1f, Taken_Alpha));
                }
            }
        }

        public void Draw()
        {
            if (TakenAnimFinished && !Core.MyLevel.GhostCheckpoints) return;
            if (!Core.Active) return;
            if (Core.MyLevel.SuppressCheckpoints && !Core.MyLevel.GhostCheckpoints) return;

            if (Box.Current.BL.X > Core.MyLevel.MainCamera.TR.X + 150 || Box.Current.BL.Y > Core.MyLevel.MainCamera.TR.Y + 150)
                return;
            if (Box.Current.TR.X < Core.MyLevel.MainCamera.BL.X - 200 || Box.Current.TR.Y < Core.MyLevel.MainCamera.BL.Y - 150)
                return;

            if (Tools.DrawGraphics && !Core.BoxesOnly)
            {
                SetAlpha();
                //Tools.QDrawer.DrawQuad(MyQuad);

                MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
                Tools.QDrawer.Flush();
            }
            if (Tools.DrawBoxes)
                Box.Draw(Tools.QDrawer, Color.Bisque, 10);
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            Checkpoint CheckpointA = A as Checkpoint;

            GhostFaded = CheckpointA.GhostFaded;
            Taken = CheckpointA.Taken;

            Box.SetTarget(CheckpointA.Box.Target.Center, CheckpointA.Box.Target.Size);
            Box.SetCurrent(CheckpointA.Box.Current.Center, CheckpointA.Box.Current.Size);
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
public bool PreDecision(Bob bob) { return false; }
//StubStubStubEnd7
    }
}