using System;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class FlyingBlob_Parameters : AutoGen_Parameters
    {
        public Param Range, Period, KeepUnused, Size;
        
        /// <summary>
        /// How far from the lowest point on the blob the computer must be
        /// in order to be allowed to use the blob.
        /// </summary>
        public Param EdgeSafety;

        public int[] MotionLevel = { 0,        1,          2,  3,     4,        5,       7,     8 };
        public enum MotionType     { Vertical, Horizontal, AA, Cross, Straight, Cirlces, Heart, All };
        public MotionType Motion;

        public struct _Special
        {
            /// <summary>
            /// A special fill type. Creates a tunnel of flying blobs.
            /// </summary>
            public bool Tunnel;

            /// <summary>
            /// A special fill type. One giant circle. All blobs orbit the same point.
            /// </summary>
            public bool Pinwheel;
        }
        public _Special Special;

        /// <summary>
        /// Whether the tunnel has a ceiling of flying blobs.
        /// </summary>
        public bool TunnelCeiling = true;
        /// <summary>
        /// The size of the displacement for the blobs comprising the tunnel.
        /// </summary>
        public float TunnelDisplacement = 0;
        /// <summary>
        /// The motion type of the blobs comprising the tunnel.
        /// </summary>
        public MotionType TunnelMotionType = MotionType.Horizontal;

        public ulong[,] TunnelGUIDs;
        

        public FlyingBlob_Parameters()
        {
        }

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            var u = PieceSeed.u;

            float FlyingBlobLevel = PieceSeed.MyUpgrades1[Upgrade.FlyBlob];

            Motion = (MotionType)level.Rnd.Choose(MotionLevel, (int)FlyingBlobLevel);

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                KeepUnused.SetVal(BobPhsxSpaceship.KeepUnused(u[Upgrade.FlyBlob]));
            }

            FillWeight = new Param(PieceSeed,
                u[Upgrade.FlyBlob]);

            float val = DifficultyHelper.Interp(40, 500, .5f * (u[Upgrade.Jump] + u[Upgrade.FlyBlob]));
            if (val < 80)
                val = 0;
            Range = new Param(PieceSeed, val);

            float speed = 200 - 20 * u[Upgrade.Speed] + 25 * .5f * (u[Upgrade.Jump] + u[Upgrade.FlyBlob]);
            Period = new Param(PieceSeed, CoreMath.Restrict(40, 1000, speed));

            EdgeSafety = new Param(PieceSeed);
            EdgeSafety.SetVal(Math.Max(6f, DifficultyHelper.Interp(45f, 6f, u[Upgrade.FlyBlob])));

            Size = new Param(PieceSeed, 1);
        }
    }

    public sealed class FlyingBlob_AutoGen : AutoGen
    {
        static readonly FlyingBlob_AutoGen instance = new FlyingBlob_AutoGen();
        public static FlyingBlob_AutoGen Instance { get { return instance; } }

        static FlyingBlob_AutoGen() { }
        FlyingBlob_AutoGen()
        {
            Do_WeightedPreFill_1 = true;
            Do_PreFill_1 = true;
            //Generators.AddGenerator(this);
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            FlyingBlob_Parameters Params = new FlyingBlob_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        void Circle(Level level, Vector2 Center, float Radius, int Num, int Dir)
        {
            Circle(level, Center, Radius, Num, Dir, 1f);
        }
        void Circle(Level level, Vector2 Center, float Radius, int Num, int Dir, float ModPeriod)
        {
            for (int j = 0; j < Num; j++)
            {
                FlyingBlob blob = (FlyingBlob)CreateAt(level, Center);

                blob.SetColor(FlyingBlob.BlobColor.Blue);

                blob.Period = 3 * blob.Period / 2;
                blob.Offset = (int)(j * ((float)blob.Period / Num));

                SetMoveType(blob, Radius, FlyingBlob_Parameters.MotionType.Cirlces, level.Rnd);

                blob.Displacement.X = Dir * Math.Abs(blob.Displacement.X);

                blob.Core.GenData.RemoveIfUnused = false;

                level.AddObject(blob);
            }
        }

        void Pinwheel(Level level, Vector2 BL, Vector2 TR)
        {
            Vector2 Center = (TR + BL) / 2;

            int Num = 30;

            //Circle(level, Center, 160 * 7, Num, -1);
            //Circle(level, Center, 160 * 10, Num, 1);

            float mod = 1.2f;
            for (int i = 7; i <= 19; i += 3)
            {
                Circle(level, Center, 160 * i, Num, 1, mod);
                mod += .2f;
            }

            //for (int i = 4; i <= 8; i += 2)
              //  Circle(level, Center, 160 * i + 90, Num, 1);
        }


        void SetTunnelBlobParameter(FlyingBlob blob, FlyingBlob_Parameters Params, Rand Rnd)
        {
            blob.SetColor(FlyingBlob.BlobColor.Pink);

            blob.Core.GenData.RemoveIfUnused = false;

            SetMoveType(blob, Params.TunnelDisplacement, Params.TunnelMotionType, Rnd);
            blob.Displacement.X = Math.Abs(blob.Displacement.X);
            
            blob.Offset = 0;
        }

        void Tunnel(Level level, Vector2 BL, Vector2 TR)
        {
            // Get Goomba parameters
            FlyingBlob_Parameters Params = (FlyingBlob_Parameters)level.Style.FindParams(FlyingBlob_AutoGen.Instance);

            BL.X = level.FillBL.X;

            float Step = 150;
            Vector2 size = (TR - BL) / Step;
            int N = (int)size.X, M = (int)size.Y;

            Params.TunnelGUIDs = new ulong[N, M];

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    FlyingBlob blob = (FlyingBlob)CreateAt(level, BL + new Vector2(i, j) * Step);
                    SetTunnelBlobParameter(blob, Params, level.Rnd);

                    Params.TunnelGUIDs[i, j] = blob.Core.MyGuid;
                }
            }
        }

        void Clean(ObjectBase[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    if (array[i,j] != null && array[i,j].Core.MarkedForDeletion)
                        array[i, j] = null;
        }
        void Clean(ObjectBase[,] array, int i, int j)
        {
            if (array[i, j] == null) return;
            array[i, j].CollectSelf();
            array[i, j] = null;
        }
        void CleanupTunnel(Level level)
        {
            // Get Goomba parameters
            FlyingBlob_Parameters Params = (FlyingBlob_Parameters)level.Style.FindParams(FlyingBlob_AutoGen.Instance);

            ulong[,] GUIDs = Params.TunnelGUIDs;
            ObjectBase[,] Blobs = new ObjectBase[GUIDs.GetLength(0), GUIDs.GetLength(1)];

            for (int i = 0; i < Blobs.GetLength(0); i++)
                for (int j = 0; j < Blobs.GetLength(1); j++)
                    Blobs[i,j] = level.GuidToObj(GUIDs[i,j]);

            // Head room
            for (int i = 0; i < Blobs.GetLength(0); i++)
            {
                for (int j = Blobs.GetLength(1) - 1; j >= 1;  j--)
                {
                    if (Blobs[i, j] == null) continue;
                    if (Blobs[i, j].Core.GenData.Used) continue;

                    if (Blobs[i, j - 1] == null)
                        Clean(Blobs, i, j);
                }
            }

            // Remove ceiling
            if (!Params.TunnelCeiling)
            {
                for (int i = 0; i < Blobs.GetLength(0); i++)
                {
                    int j;
                    for (j = Blobs.GetLength(1) - 1; j >= 1; j--)
                        if (Blobs[i, j] == null) break;

                    for (; j < Blobs.GetLength(1); j++)
                        Clean(Blobs, i, j);
                }
            }

            for (int i = 0; i < Blobs.GetLength(0); i++)
            {
                for (int j = 0; j < Blobs.GetLength(1); j++)
                {
                    if (Blobs[i,j] == null) continue;
                    if (Blobs[i, j].Core.GenData.Used) continue;
                    
                    if (j - 1 >= 0 && Blobs[i, j - 1] == null) continue;
                    if (j + 1 < Blobs.GetLength(1) && Blobs[i, j + 1] == null) continue;
                    if (j - 2 >= 0 && Blobs[i, j - 2] == null) continue;
                    if (j + 2 < Blobs.GetLength(1) && Blobs[i, j + 2] == null) continue;
                    if (j - 3 >= 0 && Blobs[i, j - 3] == null) continue;
                    if (j + 3 < Blobs.GetLength(1) && Blobs[i, j + 3] == null) continue;

                    Blobs[i, j].CollectSelf();
                }
            }
            Clean(Blobs);

            // Remove warts
            for (int i = 1; i < Blobs.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < Blobs.GetLength(1); j++)
                {
                    if (Blobs[i, j] == null) continue;
                    if (Blobs[i, j].Core.GenData.Used) continue;

                    if (Blobs[i - 1, j] == null && Blobs[i + 1, j] == null && Blobs[i, j - 1] == null)
                        Clean(Blobs, i, j);
                }
            }

            GUIDs = Params.TunnelGUIDs = null;
            Blobs = null;
        }
        
        public override void PreFill_1(Level level, Vector2 BL, Vector2 TR)
        {
            base.ActiveFill_1(level, BL, TR);

            // Get Goomba parameters
            FlyingBlob_Parameters Params = (FlyingBlob_Parameters)level.Style.FindParams(FlyingBlob_AutoGen.Instance);

            if (Params.Special.Tunnel)
                Tunnel(level, BL, TR);
            if (Params.Special.Pinwheel)
                Pinwheel(level, BL, TR);
        }

        public override void Cleanup_1(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_1(level, BL, TR);

            // Get Goomba parameters
            FlyingBlob_Parameters Params = (FlyingBlob_Parameters)level.Style.FindParams(FlyingBlob_AutoGen.Instance);

            if (Params.Special.Tunnel)
                CleanupTunnel(level);
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
        }

        public void SetMoveType(FlyingBlob fblob, float Displacement, FlyingBlob_Parameters.MotionType mtype, Rand Rnd)
        {
            switch (mtype)
            {
                case FlyingBlob_Parameters.MotionType.Vertical:
                    fblob.MyMoveType = FlyingBlob.PrescribedMoveType.Line;
                    fblob.Displacement = new Vector2(0, .5f * Displacement);
                    break;

                case FlyingBlob_Parameters.MotionType.Horizontal:
                    fblob.MyMoveType = FlyingBlob.PrescribedMoveType.Line;
                    fblob.Displacement = new Vector2(Displacement, 0);
                    break;
                    
                case FlyingBlob_Parameters.MotionType.Cross:
                    fblob.MyMoveType = FlyingBlob.PrescribedMoveType.Line;
                    if (Rnd.Rnd.NextDouble() > .5)
                        fblob.Displacement = new Vector2(Displacement, .5f * Displacement);
                    else
                        fblob.Displacement = new Vector2(-Displacement, .5f * Displacement);
                    break;

                case FlyingBlob_Parameters.MotionType.Cirlces:
                    fblob.MyMoveType = FlyingBlob.PrescribedMoveType.Circle;
                    fblob.Displacement = new Vector2(Displacement * .7f, Displacement * .5f);
                    fblob.Displacement.X *= Rnd.Rnd.Next(0, 2) * 2 - 1;
                    break;

                case FlyingBlob_Parameters.MotionType.AA:
                    if (Rnd.Rnd.NextDouble() > .5)
                        SetMoveType(fblob, Displacement, FlyingBlob_Parameters.MotionType.Vertical, Rnd);
                    else
                        SetMoveType(fblob, Displacement, FlyingBlob_Parameters.MotionType.Horizontal, Rnd);
                    break;

                case FlyingBlob_Parameters.MotionType.Straight:
                    if (Rnd.Rnd.NextDouble() > .5)
                        SetMoveType(fblob, Displacement, FlyingBlob_Parameters.MotionType.Cross, Rnd);
                    else
                        SetMoveType(fblob, Displacement, FlyingBlob_Parameters.MotionType.AA, Rnd);
                    break;

                case FlyingBlob_Parameters.MotionType.Heart:
                    fblob.MyMoveType = FlyingBlob.PrescribedMoveType.Star;
                    fblob.Displacement = new Vector2(Displacement * .7f, Displacement * .7f);
                    fblob.Displacement.X *= Rnd.Rnd.Next(0, 2) * 2 - 1;
                    break;

                case FlyingBlob_Parameters.MotionType.All:
                    double rnd = Rnd.Rnd.NextDouble();
                    if (rnd > .66666)
                        SetMoveType(fblob, Displacement, FlyingBlob_Parameters.MotionType.Straight, Rnd);
                    else
                    {
                        if (rnd > .33333)
                            SetMoveType(fblob, Displacement, FlyingBlob_Parameters.MotionType.Cirlces, Rnd);
                        else
                            SetMoveType(fblob, Displacement, FlyingBlob_Parameters.MotionType.Heart, Rnd);
                    }
                    break;
            }
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)
        {
            base.CreateAt(level, pos, BL, TR);

            FlyingBlob NewBlob = (FlyingBlob)BasicCreateAt(level, pos);

            TR.X += 200;
            Tools.EnsureBounds_X(NewBlob, TR, BL);

            // If the blob is too low make sure it's path is horizontal
            if (pos.Y < BL.Y + 500)
                SetMoveType(NewBlob, CoreMath.SupNorm(NewBlob.Displacement), FlyingBlob_Parameters.MotionType.Horizontal, level.Rnd);

            level.AddObject(NewBlob);

            return NewBlob;
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos)
        {
            base.CreateAt(level, pos);

            FlyingBlob NewBlob = (FlyingBlob)BasicCreateAt(level, pos);

            level.AddObject(NewBlob);

            return NewBlob;
        }

        ObjectBase BasicCreateAt(Level level, Vector2 pos)
        {
            StyleData Style = level.Style;
            RichLevelGenData GenData = level.CurMakeData.GenData;
            PieceSeedData piece = level.CurMakeData.PieceSeed;

            // Get Goomba parameters
            FlyingBlob_Parameters Params = (FlyingBlob_Parameters)level.Style.FindParams(FlyingBlob_AutoGen.Instance);

            // Make the new blob
            FlyingBlob NewBlob = (FlyingBlob)level.Recycle.GetObject(ObjectType.FlyingBlob, true);
            NewBlob.Init(pos, level);

            NewBlob.Core.Data.Position = NewBlob.Core.StartData.Position = pos;
            NewBlob.Period = (int)Params.Period.GetVal(pos);

            //NewBlob.Offset = MyLevel.Rnd.Rnd.Next(0, NewBlob.Period);
            NewBlob.Offset = level.Style.GetOffset(NewBlob.Period, pos,
                                                        level.Style.FlyingBlobOffsetType);

            float Displacement = Params.Range.GetVal(pos);
            SetMoveType(NewBlob, Displacement, Params.Motion, level.Rnd);

            // Decide if we should keep the blob even if unused
            if (level.Rnd.Rnd.NextDouble() < Params.KeepUnused.GetVal(pos))
                NewBlob.Core.GenData.RemoveIfUnused = false;
            else
                NewBlob.Core.GenData.RemoveIfUnused = true;

            if (level.Style.RemoveBlockOnOverlap)
                NewBlob.Core.GenData.RemoveIfOverlap = true;

            NewBlob.Core.GenData.EdgeSafety = Params.EdgeSafety.GetVal(pos);

            return NewBlob;
        }
    }
}
