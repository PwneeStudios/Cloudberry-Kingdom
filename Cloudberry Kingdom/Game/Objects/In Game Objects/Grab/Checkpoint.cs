using Microsoft.Xna.Framework;

using CoreEngine;
using System.IO;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Particles;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.InGameObjects
{
    public class Checkpoint : ObjectBase
    {
        public class CheckpointTileInfo : TileInfoBase
        {
            public TextureOrAnim Sprite = new TextureOrAnim("Checkpoint3");
            public Vector2 Size = new Vector2(115, 115);
            public Vector2 TextureSize = new Vector2(170, 170);

            public EzSound MySound = Tools.NewSound("Checkpoint", .6f);

            public Particle DieTemplate;
            public CheckpointTileInfo()
            {
                DieTemplate = new Particle();
                DieTemplate.MyQuad.Init();
                DieTemplate.MyQuad.MyEffect = Tools.BasicEffect;
                DieTemplate.MyQuad.MyTexture = Tools.Texture("Checkpoint3");
                DieTemplate.SetSize(TextureSize.X);
                DieTemplate.SizeSpeed = new Vector2(10, 10);
                DieTemplate.AngleSpeed = .013f;
                DieTemplate.Life = 20;
                DieTemplate.MyColor = new Vector4(1f, 1f, 1f, .75f);
                DieTemplate.ColorVel = new Vector4(0, 0, 0, -.065f);
            }
        }

        public override void Release()
        {
            base.Release();

            MyPiece = null;
        }

        public bool Taken, TakenAnimFinished;
        bool GhostFaded;

        float Taken_Scale, Taken_Alpha;

        static EzSound MySound;

        public bool SkipPhsx;

        public bool Touched;

        public AABox Box;
        public SimpleQuad MyQuad;
        public BasePoint Base;

        public SimpleObject MyObject;

        public LevelPiece MyPiece;
        public int MyPieceIndex;

        public override void MakeNew()
        {
            Taken = TakenAnimFinished = false;

            CoreData.Init();
            CoreData.MyType = ObjectType.Checkpoint;
            CoreData.DrawLayer = 8;

            CoreData.ResetOnlyOnReset = true;

            MyPiece = null;
            MyPieceIndex = -1;

            SetAnimation();
        }

        public Checkpoint()
        {
            Box = new AABox();

            MyQuad = new SimpleQuad();
            MyObject = new SimpleObject(Prototypes.CheckpointObj, false);

            MakeNew();

            CoreData.BoxesOnly = false;
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

            if (CoreData.MyLevel.PlayMode != 0) return;

            Game.CheckpointGrabEvent(this);

            Info.Checkpoints.MySound.Play();
        }

        public void Init(Level level)
        {
            base.Init(Vector2.Zero, level);

            Box.Initialize(CoreData.Data.Position, level.Info.Checkpoints.Size);

            if (!CoreData.BoxesOnly)
            {
                MyQuad.MyEffect = Tools.BasicEffect;
                MyQuad.Set(level.Info.Checkpoints.Sprite);
                MyQuad.Init();
            }

            Base.e1 = new Vector2(level.Info.Checkpoints.TextureSize.X, 0);
            Base.e2 = new Vector2(0, level.Info.Checkpoints.TextureSize.Y);

            Update();
        }

        public void AnimStep()
        {
            if (MyObject.DestinationAnim() == 0 && MyObject.Loop)
                MyObject.PlayUpdate(1f/3f);//MyAnimSpeed);
        }

        public override void PhsxStep()
        {
            if (!CoreData.Active) return;

            if (CoreData.Data.Position.X > CoreData.MyLevel.MainCamera.TR.X + 350 ||
                CoreData.Data.Position.X < CoreData.MyLevel.MainCamera.BL.X - 400 ||
                CoreData.Data.Position.Y > CoreData.MyLevel.MainCamera.TR.Y + 350 ||
                CoreData.Data.Position.Y < CoreData.MyLevel.MainCamera.BL.Y - 350)
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

            Box.SetTarget(CoreData.Data.Position, Box.Current.Size);
            if (SkipPhsx) Box.SwapToCurrent();

            SkipPhsx = false;
        }

        public override void PhsxStep2()
        {
            if (!CoreData.Active) return;
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

        public override void Reset(bool BoxesOnly)
        {
            CoreData.Active = true;

            CoreData.Data.Position = CoreData.StartData.Position;

            Box.SetTarget(CoreData.Data.Position, Box.Current.Size);
            Box.SwapToCurrent();

            Update();
        }

        public override void Move(Vector2 shift)
        {
            CoreData.StartData.Position += shift;
            CoreData.Data.Position += shift;
            Box.Move(shift);
        }

        public override void Interact(Bob bob)
        {
            if (Taken) return;
            if (!CoreData.Active) return;
            if (CoreData.MyLevel.SuppressCheckpoints || CoreData.MyLevel.GhostCheckpoints) return;

            ColType Col = Phsx.CollisionTest(bob.Box2, Box);
            if (Col != ColType.NoCol)
            {
                Die();

                if (CoreData.MyLevel.PlayMode == 0 && MyPiece != null)
                {
                    // Track stats
                    bob.MyStats.Checkpoints++;
                    bob.MyStats.Score += 250;

                    // Erase taken coins
                    CoreData.MyLevel.KeepCoinsDead();                    

                    // Set current level piece
                    CoreData.MyLevel.SetCurrentPiece(MyPiece);

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
                    CoreData.MyLevel.MyGame.GotCheckpoint(bob);

                    // Kill other checkpoints
                    foreach (ObjectBase obj in CoreData.MyLevel.Objects)
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
            if (CoreData.MyLevel.GhostCheckpoints)
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

        public override void Draw()
        {
            if (TakenAnimFinished && !CoreData.MyLevel.GhostCheckpoints) return;
            if (!CoreData.Active) return;
            if (CoreData.MyLevel.SuppressCheckpoints && !CoreData.MyLevel.GhostCheckpoints) return;

            if (Box.Current.BL.X > CoreData.MyLevel.MainCamera.TR.X + 150 || Box.Current.BL.Y > CoreData.MyLevel.MainCamera.TR.Y + 150)
                return;
            if (Box.Current.TR.X < CoreData.MyLevel.MainCamera.BL.X - 200 || Box.Current.TR.Y < CoreData.MyLevel.MainCamera.BL.Y - 150)
                return;

            if (Tools.DrawGraphics && !CoreData.BoxesOnly)
            {
                SetAlpha();
                //Tools.QDrawer.DrawQuad(ref MyQuad);

                MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
                Tools.QDrawer.Flush();
            }

			if (Tools.DrawBoxes)
			{
				Tools.QDrawer.DrawCircle(CoreData.Data.Position, 100, new Color(255, 134, 26, 235));

				//Box.Draw(Tools.QDrawer, Color.Bisque, 10);
			}
        }

        public override void Clone(ObjectBase A)
        {
            CoreData.Clone(A.CoreData);

            Checkpoint CheckpointA = A as Checkpoint;

            GhostFaded = CheckpointA.GhostFaded;
            Taken = CheckpointA.Taken;

            Box.SetTarget(CheckpointA.Box.Target.Center, CheckpointA.Box.Target.Size);
            Box.SetCurrent(CheckpointA.Box.Current.Center, CheckpointA.Box.Current.Size);
        }
    }
}