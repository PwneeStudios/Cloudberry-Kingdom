using System;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Particles;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Goombas
{
    public class Goomba : ObjectBase, IBound
    {
        public enum BlobColor { Green, Pink, Blue, Grey, Gold };
        public enum PhsxType { Prescribed, ToTarget };
        public enum PrescribedMoveType { Line, Circle, Star };

        static Particle BlobGooTemplate;
        static EzSound SquishSound;

        public int Period, Offset;
        public Vector2 Displacement;

        public Vector2 Target, TargetVel;
        public bool HasArrived, RemoveOnArrival;
        public float ArrivedRadius;
        public float MaxVel = 16, MaxAcc = 2, DistAccMod = 1;
        public bool FinalizedParams = false;
        public float Damp = .96f;
        public float DampRange = 2;

        public bool GiveVelocity;

        public PrescribedMoveType MyMoveType;
        public PhsxType MyPhsxType;

        public BlobColor MyColor;

        static float[] BobMaxSpeed = { 2f, 0f };
        static float BobXAccel = .53f;
        //static float Gravity = 2.95f;
        static float BobXFriction = 1f;

        public SimpleObject MyObject;
        public float MyAnimSpeed;

        /// <summary>
        /// Source from which to copy object vertex info
        /// </summary>
        public Goomba CopySource;

        /// <summary>
        /// Whether to delete the blob permanently when it dies
        /// </summary>
        public bool DeleteOnDeath;

        public AABox Box, Box2;

        public int Life, StartLife;

        public int Direction;

        public bool NeverSkip = false;

        Bob KillingBob;

        public void SetColor(BlobColor color)
        {
            MyColor = color;
            if (MyObject.Quads != null && MyObject.Quads.Length >= 2)
            {
                switch (MyColor)
                {
                    case BlobColor.Green:
                        MyObject.Quads[2].MyTexture = Tools.TextureWad.FindByName("Blob2_Body"); break;
                    case BlobColor.Pink:
                        MyObject.Quads[2].MyTexture = Tools.TextureWad.FindByName("Blob2_Body2"); break;
                    case BlobColor.Blue:
                        MyObject.Quads[2].MyTexture = Tools.TextureWad.FindByName("Blob2_Body3"); break;
                    case BlobColor.Grey:
                        MyObject.Quads[2].MyTexture = Tools.TextureWad.FindByName("Blob2_Body4"); break;
                    case BlobColor.Gold:
                        MyObject.Quads[2].MyTexture = Tools.TextureWad.FindByName("Blob2_Body5"); break;
                }
            }
        }

        static EzTexture GetGooTexture(BlobColor color)
        {
            switch (color)
            {
                case BlobColor.Green:
                    return Tools.TextureWad.FindByName("BlobGoo");
                case BlobColor.Pink:
                    return Tools.TextureWad.FindByName("BlobGoo2");
                case BlobColor.Blue:
                    return Tools.TextureWad.FindByName("BlobGoo3");
                case BlobColor.Grey:
                    return Tools.TextureWad.FindByName("BlobGoo4");
                case BlobColor.Gold:
                    return Tools.TextureWad.FindByName("BlobGoo5");
            }

            return null;
        }

        public override void MakeNew()
        {
            CopySource = null;

            Target = TargetVel = Vector2.Zero;
            Core.Data = new PhsxData();

            MyAnimSpeed = InfoWad.GetFloat("FlyingBlob_AnimSpeed");

            MyPhsxType = PhsxType.Prescribed;
            HasArrived = false;
            
            RemoveOnArrival = false;
            DeleteOnDeath = false;
            ArrivedRadius = 250;
            FinalizedParams = false;
            DistAccMod = 1;
            Damp = .96f;
            DampRange = 2;
            
            SetColor(BlobColor.Green);

            Core.Init();
            Core.DrawLayer = 4;
            Core.MyType = ObjectType.FlyingBlob;
            Core.Holdable = true;

            Displacement = Vector2.Zero;
            Offset = 0;
            Period = 1;

            MyObject.Linear = true;

            MyObject.Boxes[0].Animated = false;
            MyObject.Boxes[1].Animated = false;

            Box.Initialize(Core.Data.Position, Prototypes.flyinggoomba.MyObject.Boxes[0].Size() / 2);
            Box2.Initialize(Core.Data.Position, Prototypes.flyinggoomba.MyObject.Boxes[1].Size() / 2);

            MyObject.Read(0, 0);
            MyObject.Update();

            Core.WakeUpRequirements = true;
            NeverSkip = false;
           
            StartLife = Life = 1;
            Direction = -1;

            GiveVelocity = false;

            Box.SetTarget(Core.Data.Position, Box.Current.Size);
            Box.SwapToCurrent();

            UpdateObject();

            Box2.SetTarget(MyObject.GetBoxCenter(1), Box2.Current.Size);
            Box2.SwapToCurrent();
        }

        public Goomba(bool BoxesOnly)
        {
            MyObject = new SimpleObject(Prototypes.flyinggoomba.MyObject, BoxesOnly);

            Box = new AABox();
            Box2 = new AABox();

            SetAnimation();

            MakeNew();


            Core.BoxesOnly = BoxesOnly;           
        }

        public void SetAnimation()
        {
            MyObject.AnimQueue.Clear();
            MyObject.Read(0, 0);
            MyObject.Play = true;
            MyObject.Loop = true;
            //MyObject.EnqueueAnimation(0, (float)MyLevel.Rnd.Rnd.NextDouble() * 1.5f, true);
            MyObject.EnqueueAnimation(0, (float)0, true);
            MyObject.DequeueTransfers();
            MyObject.Update();
        }

        public Goomba(string file, EzEffectWad EffectWad, EzTextureWad TextureWad)
        {
            CoreData = new ObjectData();
            Core.Active = true;

            // Initialize statics
            SquishSound = Tools.SoundWad.FindByName("Blob_Squish");

            BlobGooTemplate = new Particle();
            BlobGooTemplate.MyQuad.Init();
            BlobGooTemplate.MyQuad.MyEffect = Tools.BasicEffect;
            BlobGooTemplate.MyQuad.MyTexture = Tools.TextureWad.FindByName("BlobGoo");
            BlobGooTemplate.SetSize(55);
            BlobGooTemplate.SizeSpeed = new Vector2(4, 4);
            BlobGooTemplate.AngleSpeed = .06f;
            BlobGooTemplate.Life = 37;
            BlobGooTemplate.MyColor = new Vector4(1.5f, 1.5f, 1.5f, 2f);
            BlobGooTemplate.ColorVel = new Vector4(0.01f, 0.01f, 0, -.072f);
            BlobGooTemplate.Data.Acceleration = new Vector2(0, -1.5f);

            ObjectClass SourceObject;
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
            //SourceObject = new ObjectClass(Tools.QDrawer, Tools.Device, Tools.Device.PresentationParameters, 100, 100, EffectWad.FindByName("BasicEffect"), TextureWad.FindByName("White"));
            SourceObject = new ObjectClass(Tools.QDrawer, Tools.Device, EffectWad.FindByName("BasicEffect"), TextureWad.FindByName("White"));
            SourceObject.ReadFile(reader, EffectWad, TextureWad);
            reader.Close();
            stream.Close();

            //MyObject.FinishLoading(Tools.QDrawer, Tools.Device, Tools.TextureWad, Tools.EffectWad, Tools.Device.PresentationParameters, Tools.Device.PresentationParameters.BackBufferWidth / 8, Tools.Device.PresentationParameters.BackBufferHeight / 8);

            SourceObject.ConvertForSimple();
            MyObject = new SimpleObject(SourceObject);
            MyObject.Base.e1 *= 555;// *.125f;
            MyObject.Base.e2 *= 555;// *.125f;

            MyObject.Read(0, 0);
            MyObject.Play = true;
            MyObject.EnqueueAnimation(0, 0, true);
            MyObject.DequeueTransfers();
            MyObject.Update();


            Core.Data.Position = new Vector2(100, 50);
            Core.Data.Velocity = new Vector2(0, 0);

            Box = new AABox(Core.Data.Position, MyObject.Boxes[0].Size() / 2);
            Box2 = new AABox(Core.Data.Position, MyObject.Boxes[1].Size() / 2);

            StartLife = Life = 1;
            Direction = -1;
        }

        public void Death()
        {
            Core.Active = false;
            if (DeleteOnDeath) Core.Recycle.CollectObject(this);
            if (Core.MyLevel.PlayMode != 0) return;

            // Change player's kill stats
            if (KillingBob != null && KillingBob.GiveStats())
                KillingBob.MyTempStats.Blobs++;

            Squish(Vector2.Zero);
        }

        public void Squish(Vector2 vel)
        {
            var emitter = Core.MyLevel.MainEmitter;

            for (int k = 0; k < 9; k++)
            {
                var p = emitter.GetNewParticle(BlobGooTemplate);
                p.MyQuad.MyTexture = GetGooTexture(MyColor);

                Vector2 Dir = MyLevel.Rnd.RndDir();
                p.Data.Position = Core.Data.Position + 50 * Dir;
                p.Data.Velocity = 20 * (float)MyLevel.Rnd.Rnd.NextDouble() * Dir;

                p.Data.Velocity.Y = .5f * Math.Abs(p.Data.Velocity.Y);
                p.Data.Velocity.Y += 6;
                p.Data.Velocity += vel;
                p.AngleSpeed *= 2 * (float)(MyLevel.Rnd.Rnd.NextDouble() - .5f);
            }

            SquishSound.PlayModulated(.02f);
        }

        public void XAccel(bool Left, bool Right, bool Run)
        {
            if (Left && Core.Data.Velocity.X > -BobMaxSpeed[(int)MyMoveType])
            {
                if (Core.Data.Velocity.X <= 0)
                    Core.Data.Velocity.X -= BobXAccel;
                else
                    Core.Data.Velocity.X -= 2.7f * BobXAccel;

                if (Core.Data.Velocity.X < 0 && Core.Data.Velocity.X > -BobMaxSpeed[(int)MyMoveType] / 10)
                    Core.Data.Velocity.X = -BobMaxSpeed[(int)MyMoveType] / 10;
            }
            if (Right && Core.Data.Velocity.X < BobMaxSpeed[(int)MyMoveType])
            {
                if (Core.Data.Velocity.X >= 0)
                    Core.Data.Velocity.X += BobXAccel;
                else
                    Core.Data.Velocity.X += 2.7f * BobXAccel;

                if (Core.Data.Velocity.X > 0 && Core.Data.Velocity.X < BobMaxSpeed[(int)MyMoveType] / 10)
                    Core.Data.Velocity.X = BobMaxSpeed[(int)MyMoveType] / 10;
            }

            if (!(Left || Right) || !Run && Math.Abs(Core.Data.Velocity.X) > BobMaxSpeed[(int)MyMoveType])
            {
                if (Math.Abs(Core.Data.Velocity.X) < 2)
                    Core.Data.Velocity.X /= 2f;
                else
                {
                    if (Math.Abs(Core.Data.Velocity.X) > BobMaxSpeed[(int)MyMoveType])
                        Core.Data.Velocity.X -= Math.Sign(Core.Data.Velocity.X) * BobXFriction;
                    else
                        Core.Data.Velocity.X -= Math.Sign(Core.Data.Velocity.X) * 7f / 4f * BobXFriction;
                }
            }
        }

        public Vector2 TR_Bound()
        {
            Vector2 max =
                Vector2.Max(
                Vector2.Max(CalcPosition(0), CalcPosition(.5f)),
                Vector2.Max(CalcPosition(0.25f), CalcPosition(.75f)));
            return max;
        }

        public Vector2 BL_Bound()
        {
            Vector2 min =
                Vector2.Min(
                Vector2.Min(CalcPosition(0), CalcPosition(.5f)),
                Vector2.Min(CalcPosition(0.25f), CalcPosition(.75f)));

            return min;
        }

        public Vector2 CalcPosition(float t)
        {
            switch (MyMoveType)
            {
                case PrescribedMoveType.Line:
                    return Core.StartData.Position + Displacement * (float)Math.Cos(2 * Math.PI * t);                    

                case PrescribedMoveType.Circle:
                    return Core.StartData.Position +
                        new Vector2(Displacement.X * (float)Math.Cos(2 * Math.PI * t),
                                    Displacement.Y * (float)Math.Sin(2 * Math.PI * t));                    

                case PrescribedMoveType.Star:
                    return Core.StartData.Position +
                        new Vector2(Displacement.X * (float)Math.Cos(2 * Math.PI * t)
                                                   * (1 + (float)Math.Cos(2 * Math.PI * t)),
                                    Displacement.Y * (float)Math.Sin(2 * Math.PI * t)
                                                   * (1 + (float)Math.Cos(2 * Math.PI * t)));                    
            }

            return Core.StartData.Position;
        }


        void UpdatePos()
        {
            switch (MyPhsxType)
            {
                case PhsxType.Prescribed:
                    //int Step = Tools.Modulo(Core.GetPhsxStep() + Offset, Period);
                    float Step = Tools.Modulo(Core.GetIndependentPhsxStep() + Offset, (float)Period);

                    if (!Core.Held)
                        Core.Data.Position = CalcPosition((float)Step / Period);

                    break;

                case PhsxType.ToTarget:
                    Target += TargetVel;
                    TargetVel += Core.Data.Acceleration;

                    Vector2 dif = Target - Core.Data.Position;
                    float dist = dif.Length();

                    if (dist < ArrivedRadius)
                        HasArrived = true;

                    float acc = Math.Min(MaxAcc, .0016f * dist * DistAccMod);

                    if (dist < DampRange)
                        acc = 0;
                    else
                        Core.Data.Velocity *= Damp;
                    if (dist > 1)
                    {
                        dif.Normalize();
                        Core.Data.Velocity += acc * dif;
                    }
                    float vel = Core.Data.Velocity.Length();
                    if (vel > MaxVel) Core.Data.Velocity *= MaxVel / vel;

                    Core.Data.Position += Core.Data.Velocity;

                    break;
            }
        }

        public override void PhsxStep()
        {
            if (!Core.Active) return;

            UpdatePos();

            if (!NeverSkip)
            {
                //Vector2 PhsxCutoff = new Vector2(Math.Max(Math.Abs(Displacement.X) + 250, 1200), 1000);
                Vector2 PhsxCutoff = new Vector2(400);
                if (Core.MyLevel.BoxesOnly) PhsxCutoff = new Vector2(0, 1000);
                if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, PhsxCutoff))
                {
                    Core.SkippedPhsx = true;
                    Core.WakeUpRequirements = true;
                    return;
                }
            }
            Core.SkippedPhsx = false;

            if (!Core.BoxesOnly) AnimStep();

            bool Right, Left;
            Right = Left = false;

            UpdateObject();

            Box.Current.Center = MyObject.Boxes[0].Center();
            Box.Current.Size = MyObject.Boxes[0].Size() / 2;
            Box.SetTarget(Box.Current.Center, Box.Current.Size + new Vector2(.0f, .02f));

            Box2.Current.Center = MyObject.Boxes[0].Center();
            Box2.Current.Size = MyObject.Boxes[0].Size() / 2;
            Box2.Current.Size.X -= 40;

            Box2.SetTarget(Box2.Current.Center, Box2.Current.Size + new Vector2(.0f, .02f));

            if (Core.WakeUpRequirements)
            {
                Box.SwapToCurrent();
                Box2.SwapToCurrent();

                Core.WakeUpRequirements = false;
            }           

            if (Right) MyObject.xFlip = true;
            if (Left) MyObject.xFlip = false;

            if (HasArrived && RemoveOnArrival)
                CollectSelf();
        }

        public override void PhsxStep2()
        {
            if (!Core.Active) return;
            if (Core.SkippedPhsx) return;

            if (Life <= 0) Death();

            Box.SwapToCurrent();
            Box2.SwapToCurrent();
        }

        public void AnimStep()
        {
            if (!Core.Active) return;
            if (Core.SkippedPhsx) return;
            //MyObject.Linear = false;

            if (CopySource == null)
            if (MyObject.DestinationAnim() == 0 && MyObject.Loop)
                MyObject.PlayUpdate(MyAnimSpeed * Core.IndependentDeltaT);
                //MyObject.PlayUpdate(MyAnimSpeed);
        }

        public void UpdateObject()
        {
            if (MyObject != null)
            {
                MyObject.Base.Origin = Core.Data.Position;
                //MyObject.Base.e1.X = 490f;
                //MyObject.Base.e2.Y = 490f;

                if (CopySource != null)
                    MyObject.CopyUpdate(CopySource.MyObject);
                else
                    MyObject.Update();
            }
        }

        public override void Draw()
        {
            if (!Core.Held)
            {
                if (!Core.Active || Core.SkippedPhsx) return;

                Vector2 BL = Box.Current.BL - new Vector2(225, 225);
                if (BL.X > Core.MyLevel.MainCamera.TR.X || BL.Y > Core.MyLevel.MainCamera.TR.Y)
                    return;
                Vector2 TR = Box.Current.TR + new Vector2(260, 225);
                if (TR.X < Core.MyLevel.MainCamera.BL.X || TR.Y < Core.MyLevel.MainCamera.BL.Y)
                    return;
            }

            if (Tools.DrawGraphics)
                 MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
            if (Tools.DrawBoxes)
            {
                Box.DrawT(Tools.QDrawer, Color.Blue, 10);
                Box2.DrawT(Tools.QDrawer, Color.Blue, 10);
                Box.Draw(Tools.QDrawer, Color.Azure, 10);
                Box2.Draw(Tools.QDrawer, Color.Azure, 10);
            }
        }

        public void MoveToBounded(Vector2 shift)
        {
            Move(shift);
        }

        public override void Move(Vector2 shift)
        {
            Core.StartData.Position += shift;
            Core.Data.Position += shift;

            Target += shift;

            Box.Move(shift);
            Box2.Move(shift);

            MyObject.Base.Origin += shift;
            MyObject.Update();
        }

        public override void Interact(Bob bob)
        {
            if (!Core.Active) return;
            if (Life <= 0) return;

            bool UnderFoot, SideHit, Overlap;
            bool Delete = false; // Used for Stage 1 Level Generation
            if (!Core.SkippedPhsx)
            {
                float VelY = Box.Target.TR.Y - Box.Current.TR.Y;

                UnderFoot = SideHit = false;
                ColType Col2 = Phsx.CollisionTest(bob.Box, Box);

                if (Col2 == ColType.Top)
                    UnderFoot = true;
                else
                {
                    ColType Col = Phsx.CollisionTest(bob.Box, Box2);

                    if (Col == ColType.Left || Col == ColType.Right)
                    {
                        SideHit = true;

                        float GraceY = 82;// 76; // Extra grace space for jumping off of blob
                        if (Game != null && Game.ModdedBlobGrace)
                            GraceY = Game.BlobGraceY;

                        if (bob.Box.Current.BL.Y > Box2.Current.BL.Y - GraceY ||
                            bob.Box.Target.BL.Y > Box2.Target.BL.Y - GraceY)
                            UnderFoot = true;

                        // If this is a computer and it might successfully jump off this blob, then
                        if (UnderFoot && Core.MyLevel.PlayMode == 2)
                        {
                            // check to make sure we aren't just barely hitting the blob.
                            // We want a solid hit.
                            float ComputerGraceY =  GraceY - Core.GenData.EdgeSafety;
                            Delete = true;
                            if (bob.Box.Current.BL.Y > Box2.Current.BL.Y - ComputerGraceY ||
                                bob.Box.Target.BL.Y > Box2.Target.BL.Y - ComputerGraceY)
                                Delete = false;
                        }
                    }
                    else if (Col == ColType.Bottom)
                        SideHit = true;
                }

                if (UnderFoot && !SideHit)
                    if (bob.Core.Data.Velocity.Y > 4 && bob.Core.Data.Velocity.Y > VelY)
                        UnderFoot = false;

                if (UnderFoot && SideHit) SideHit = false;
                if (!UnderFoot && !SideHit) return;

                bool DoInteraction = true;

                if (Core.MyLevel.PlayMode == 2)
                {
                    Overlap = Phsx.BoxBoxOverlap(bob.Box, Box);

                    //if (UnderFoot && bob.Core.Data.Position.Y > bob.TargetPosition.Y) Delete = true;
                    if (UnderFoot && bob.WantsToLand == false) Delete = true;
                    if (UnderFoot && bob.BottomCol) Delete = true;
                    if (SideHit) Delete = true;
                    if (Overlap && Col2 == ColType.NoCol) Delete = true;
                    if (Core.GenData.RemoveIfOverlap) Delete = true;
                    if (Core.GenData.Used) Delete = false;
                    if (Delete)
                    {
                        Core.Recycle.CollectObject(this);

                        Core.Active = false;
                        DoInteraction = false;
                    }
                    else
                    {
                        StampAsUsed(Core.MyLevel.CurPhsxStep);
                    }
                }

                if (DoInteraction && (UnderFoot || SideHit))
                {
                    if (Core.MyLevel.DefaultHeroType is BobPhsxSpaceship)
                        UnderFoot = false;

                    if (UnderFoot)
                    {
                        Life--;
                        KillingBob = bob;
                        if (bob.GiveStats())
                            bob.MyTempStats.Score += 50;

                        float NewY = Box.Target.TR.Y + bob.Box.Current.Size.Y + .01f;
                        if (NewY > bob.Core.Data.Position.Y && Col2 == ColType.Top)
                            bob.Core.Data.Position.Y = NewY;

                        // If the player had a reasonable Y-velocity, override it
                        if (bob.Core.Data.Velocity.Y <= 30)
                        {
                            bob.Core.Data.Velocity.Y = 9.5f * bob.MyPhsx.BlobMod;

                            if (GiveVelocity && VelY > 0)
                                bob.Core.Data.Velocity.Y += VelY * bob.MyPhsx.BlobMod;

                            // The player landed on something
                            bob.MyPhsx.LandOnSomething(true, this);
                        }

                        // This code is to modify the player's velocity rather than override it.
                        // (For when the velocity is large)
                        ////else
                        ////{
                        ////    bob.MyPhsx.JumpLengthModifier = (30f - (bob.Core.Data.Velocity.Y - 4)) / 30f;
                        ////    if (bob.MyPhsx.JumpLengthModifier > 0)
                        ////        bob.MyPhsx.JumpLengthModifier = (float)Math.Pow(bob.MyPhsx.JumpLengthModifier, .385f);
                        ////}

                        //bob.MyPhsx.JumpLengthModifier = 1.1f;
                        bob.MyPhsx.MaxJumpAccelMultiple = 1 + .8f * bob.MyPhsx.BlobMod;
                    }
                    else
                        bob.Die(Bob.BobDeathType.Blob, this);
                }
            }
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;
            Life = StartLife;
            Core.WakeUpRequirements = true;

            MyObject.Read(0, 0);
            MyObject.Play = true;
            MyObject.Loop = true;
            MyObject.AnimQueue.Clear();
            MyObject.EnqueueAnimation(0, (float)MyLevel.Rnd.Rnd.NextDouble() * 1.5f, true);
            MyObject.DequeueTransfers();
            MyObject.Update();
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Goomba GoombaA = A as Goomba;            

            MyMoveType = GoombaA.MyMoveType;

            Target = GoombaA.Target;
            TargetVel = GoombaA.TargetVel;
            HasArrived = GoombaA.HasArrived;
            RemoveOnArrival = GoombaA.RemoveOnArrival;
            FinalizedParams = GoombaA.FinalizedParams;
            DistAccMod = GoombaA.DistAccMod;
            Damp = GoombaA.Damp;
            DampRange = GoombaA.DampRange;

            SetColor(GoombaA.MyColor);

            Period = GoombaA.Period;
            Offset = GoombaA.Offset;
            Displacement = GoombaA.Displacement;

            Direction = GoombaA.Direction;

            Life = GoombaA.Life;
            StartLife = GoombaA.StartLife;

            GiveVelocity = GoombaA.GiveVelocity;

            Core.WakeUpRequirements = true;
        }

        public void SetStandardTargetParams()
        {
            MaxVel = 31;
            MaxAcc = 4.9f;

            MyPhsxType = Goomba.PhsxType.ToTarget;
            Core.DrawLayer = 9;
        }
    }
}
