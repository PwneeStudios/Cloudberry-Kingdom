using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Levels
{
    public partial class Level
    {
        public class MakeData
        {
            public Vector2 TRBobMoveZone, BLBobMoveZone;

            public RichLevelGenData GenData;

            /// <summary>
            /// The PieceSeedData from which the current MakeData's level is being made
            /// </summary>
            public PieceSeedData PieceSeed;

            /// <summary>
            /// The LevelSeed which spawned the creation of this level.
            /// </summary>
            public LevelSeedData LevelSeed;

            public bool ComputerWaitAtStart;
            public int[] ComputerWaitAtStartLength;
            public float SparsityMultiplier;

            public int NumInitialBobs;

            /// <summary>
            /// Minimum number of start positions to make, regardless of the number of computer AIs
            /// </summary>
            public int MinStartPositionsToMake = 4;

            public Vector2 CamStartPos;
            public PhsxData[] Start;
            public Vector2[] CheckpointShift;
            public Bob.BobMove[] MoveData;

            public bool InitialPlats, InitialCamZone, FinalPlats;
            public bool SkinnyStart;

            public bool SetTRCamBound;

            public bool MidDivider;

            /// <summary>
            /// When making a multi-piece level, this index specifies which piece the MakeData refers to.
            /// </summary>
            public int Index;

            /// <summary>
            /// When making a multi-piece level, this index specifies how many pieces are being made.
            /// </summary>
            public int OutOf;

            /// <summary>
            /// Called after data has been initialized, immediately before level piece is made.
            /// </summary>
            public ModifyMakeData ModData;

            public void Release()
            {
                LevelSeed = null;
                PieceSeed = null;
                GenData = null;
            }

            public void Init(PieceSeedData data)
            {
                int N = data.Paths;

                ComputerWaitAtStart = true;
                ComputerWaitAtStartLength = new int[4];
                Vector2 WaitRange = data.Style.ComputerWaitLengthRange;
                for (int i = 0; i < 4; i++)
                    ComputerWaitAtStartLength[i] = (int)Tools.RndFloat(WaitRange);
                SparsityMultiplier = 1f;

                NumInitialBobs = N;

                Start = new PhsxData[N];
                CheckpointShift = new Vector2[N];
                MoveData = new Bob.BobMove[N];
                for (int i = 0; i < N; i++)
                {
                    Start[i] = new PhsxData();
                    CheckpointShift[i] = Vector2.Zero;
                    MoveData[i] = new Bob.BobMove();
                    MoveData[i].Init();
                }
                CamStartPos = Vector2.Zero;

                InitialPlats = FinalPlats = InitialCamZone = true;
                SkinnyStart = false;

                SetTRCamBound = true;
            }

            public Bob[] MakeBobs(Level level)
            {
                /*
                if (level.MySourceGame.MyGameFlags.IsDoppleganger)
                {
                    for (int i = 0; i < NumInitialBobs; i += 2)
                    {
                        MoveData[i + 1].Copy = i;

                        if (level.MySourceGame.MyGameFlags.IsDopplegangerInvert)
                            MoveData[i + 1].InvertDirX = true;
                    }
                }
                */


                Bob[] Computers = new Bob[NumInitialBobs];

                level.Bobs.Clear();
                for (int i = 0; i < NumInitialBobs; i++)
                {
                    //Computers[i] = new Bob(Prototypes.bob[level.DefaultHeroType], true);
                    Computers[i] = new Bob(level.DefaultHeroType, true);

                    Computers[i].InitAuto();
                    level.AddBob(Computers[i]);
                    Sleep();
                }

                for (int i = 0; i < NumInitialBobs; i++)
                {
                    Computers[i].Immortal = true;

                    Computers[i].ComputerWaitAtStart = ComputerWaitAtStart;
                    Computers[i].ComputerWaitAtStartLength = ComputerWaitAtStartLength[i];
                    Computers[i].MoveData = MoveData[i];

                    Sleep();
                }

                return Computers;
            }

            public LevelPiece MakeLevelPiece(Level level, Bob[] bobs, int Length, int StartPhsxStep)
            {
                LevelPiece Piece = level.StartNewPiece(Length, bobs);
                Piece.MyData = PieceSeed;
                Piece.MyMakeData = this;
                for (int i = 0; i < NumInitialBobs; i++)
                {
                    bobs[i].MyPiece = Piece;
                    Piece.StartData[i] = Start[i];
                    Piece.CheckpointShift[i] = CheckpointShift[i];
                    bobs[i].IndexOffset = StartPhsxStep;
                    Sleep();
                }

                Piece.StartPhsxStep = StartPhsxStep;

                return Piece;
            }
        }
    }
}
