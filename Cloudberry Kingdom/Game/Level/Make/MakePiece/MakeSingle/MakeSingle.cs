using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Coins;
using CloudberryKingdom.Blocks;

using System.Threading;

namespace CloudberryKingdom.Levels
{
    public partial class Level
    {
        public MakeData CurMakeData;
        public StyleData Style { get { return CurMakeData.PieceSeed.Style; } }
        public PieceSeedData PieceSeed { get { return CurMakeData.PieceSeed; } }
        public LevelGeometry Geometry;

        public bool CreationError;

        /// <summary>
        /// When true the position of the computer will be recorded.
        /// </summary>
        public bool RecordPosition;

        public int PlayMode, NumModes = 2;
        
        public bool SetToWatchMake;


        public LevelPiece StartNewPiece(int Length, Bob[] Computer)
        {
            return StartNewPiece(Length, Computer, 1);
        }
        public LevelPiece StartNewPiece(int Length, Bob[] Computer, int NumBobs)
        {
            LevelPiece NewPiece = new LevelPiece(Length, this, Computer, NumBobs);
            LevelPieces.Add(NewPiece);
            return NewPiece;
        }

        public void Clone(Level A)
        {
            foreach (BlockBase block in A.Blocks)
            {
                BlockBase DestBlock = (BlockBase)MySourceGame.Recycle.GetObject(block.Core.MyType, false);
                DestBlock.Clone(block);

                AddBlock(DestBlock);
            }

            foreach (ObjectBase obj in A.Objects)
            {
                ObjectBase DestObj = (ObjectBase)MySourceGame.Recycle.GetObject(obj.Core.MyType, false);
                DestObj.Clone(obj);

                AddObject(DestObj);
            }
        }

        public delegate void FillCallback(Vector2 pos);
        public void Fill(Vector2 BL, Vector2 TR, float xstep, float ystep, FillCallback FillFunc)
        {
            Fill(BL, TR, new Vector2(xstep, xstep), ystep, FillFunc);
        }
        public void Fill(Vector2 BL, Vector2 TR, Vector2 xstep, float ystep, FillCallback FillFunc)
        {
            if (Math.Sign(TR.X - BL.X) != Math.Sign(xstep.X)) return;
            if (Math.Sign(TR.Y - BL.Y) != Math.Sign(ystep)) return;

            Vector2 pos = BL;
            while (pos.Y < TR.Y)
            {
                pos.X = BL.X;
                while (pos.X <= TR.X)
                {
                    FillFunc(pos);
                    pos.X += Rnd.RndFloat(xstep);
                }
                if (TR.X - (pos.X - xstep.X) > .5f * xstep.X)
                {
                    pos.X = TR.X;
                    FillFunc(pos);
                }

                pos.Y += ystep;
            }
        }

        public void FadeMusic()
        {
            if (Tools.SongWad != null)
                Tools.SongWad.FadeOut();
        }

        /// <summary>
        /// Give a bonus to player's score fore beating the level.
        /// Increment the number of levels the player has finished.
        /// </summary>
        public void EndOfLevelBonus(PlayerData FinishingPlayer) { EndOfLevelBonus(FinishingPlayer, true); }
        public void EndOfLevelBonus(PlayerData FinishingPlayer, bool IncrLevels)
        {
            List<PlayerData> players;
            if (FinishingPlayer == null) players = PlayerManager.AlivePlayers;
            else { players = new List<PlayerData>(); players.Add(FinishingPlayer); }

            foreach (PlayerData player in players)
            {
                if (IncrLevels)
                    player.Stats.Levels++;
                player.Stats.Score += 500;
            }
        }

        public bool CoinsCountInStats = false;
        public int NumCoins, NumBlobs, TotalCoinScore;
        public bool Finished = false;
        /// <summary>
        /// Ends the playable level: players can no longer die, scores are finalized
        /// </summary>
        public void EndLevel()
        {
            Finished = true;

            if (MyGame != null)
            {
                MyGame.RemoveGameObjects(GameObject.Tag.RemoveOnLevelFinish);
                MyGame.AllowQuickJoin = false;
            }

            PlayerManager.ExistingPlayers.ForEach(player =>
            {
                CoinsCountInStats = true;
                player.Stats.TotalCoins += NumCoins;
                player.Stats.TotalBlobs += NumBlobs;
            });

            KeepCoinsDead();

            if (Recording)
            {
                AddCurRecording();
                Recording = false;
            }
            HaveTimeLimit = false;

            if (FinishedLevel != null) FinishedLevel();

            // Prevent additional deaths/replays/resets
            foreach (Bob bob in Bobs)
            {
                bob.Immortal = true;
                bob.ScreenWrap = true;
            }

            CanWatchComputer = false;
            PreventReset = true;
            SetToReset = false;
        }

        /// <summary>
        /// Ends the playable level: players can no longer die
        /// </summary>
        public void SoftEndLevel()
        {
            Finished = true;

            // Prevent additional deaths/replays/resets
            foreach (Bob bob in Bobs)
                bob.Immortal = true;

            CanWatchComputer = false;
            PreventReset = true;
            SetToReset = false;
        }


        /// <summary>
        /// Undoes the ending of the playable level: players can no longer die
        /// </summary>
        public void UndoSoftEndLevel()
        {
            Finished = false;

            // Prevent additional deaths/replays/resets
            foreach (Bob bob in Bobs)
                bob.Immortal = false;

            PreventReset = false;
        }

        public BlockBase LastSafetyBlock = null;
        public const float SafetyNetHeight = 124;
        public BlockBase Stage1SafetyNet(Vector2 BL, Vector2 TR, Vector2 size, float xstep, StyleData.GroundType Type)
        {
            bool Virgin = false;
            bool Used = false;
            bool BoxesOnly = false;
            bool InvertDraw = false;
            bool Invert = false;

            switch (Type)
            {
                case StyleData.GroundType.Used:
                    Used = true;
                    break;

                case StyleData.GroundType.VirginUsed:
                    Virgin = Used = true;
                    break;

                case StyleData.GroundType.InvertedUsed:
                    Virgin = Used = true;
                    //InvertDraw = true;
                    Invert = true;
                    break;

                case StyleData.GroundType.SafetyNet:
                    break;

                case StyleData.GroundType.InvertSafetyNet:
                    Invert = true;
                    break;

                case StyleData.GroundType.InvisibleUsed:
                    Used = true;
                    BoxesOnly = true;
                    Virgin = true;
                    break;

                default:
                    return null;
            }

            // Safety net
            BlockBase LastBlock = null;
            Fill(BL + new Vector2(0, SafetyNetHeight), new Vector2(TR.X, BL.Y + SafetyNetHeight + 1), xstep, 50,
                pos =>
                {
                    if (Type == StyleData.GroundType.SafetyNet)
                    {
                        // Don't make an extra block at the end to fill out the fill
                        if (pos.X == TR.X) return;
                    }

                    NormalBlock block;

                    block = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
                    block.Core.AlwaysBoxesOnly = BoxesOnly;
                    block.Init(pos + new Vector2(0, -size.Y), size, MyTileSetInfo);
                    block.Core.GenData.RemoveIfUnused = true;
                    block.BlockCore.BlobsOnTop = true;
                    //block.Core.GenData.AlwaysLandOn = true;
                    block.Core.GenData.AlwaysUse = true;
                    block.BlockCore.NonTopUsed = true;
                    block.Invert = Invert; 
                    block.BlockCore.Virgin = Virgin;
                    block.BlockCore.GenData.Used = Used;
                    block.BlockCore.MyOrientation = InvertDraw ? PieceQuad.Orientation.UpsideDown : PieceQuad.Orientation.Normal;

                    AddBlock(block);

                    LastBlock = block;
                });

            return LastBlock;
        }

        public BlockBase MadeBackBlock;
        /// <summary>
        /// Creates a door at the specified position, as well as a backdrop block.
        /// </summary>
        public Door PlaceDoorOnBlock_Unlayered(Vector2 pos, BlockBase block, bool AddBackdrop)
        {
            return PlaceDoorOnBlock(pos, block, AddBackdrop, MyTileSet, false);
        }
        public Door PlaceDoorOnBlock(Vector2 pos, BlockBase block, bool AddBackdrop)
        {
            return PlaceDoorOnBlock(pos, block, AddBackdrop, MyTileSet, true);
        }
        public Door PlaceDoorOnBlock(Vector2 pos, BlockBase block, bool AddBackdrop, TileSet BackdropTileset)
        {
            return PlaceDoorOnBlock(pos, block, AddBackdrop, BackdropTileset, true);
        }
        public Door PlaceDoorOnBlock(Vector2 pos, BlockBase block, bool AddBackdrop, TileSet BackdropTileset, bool LayeredDoor)
        {
            int DesiredDoorLayer = 0, DesiredDoorLayer2 = 0;

            // Add door
            Door door = (Door)Recycle.GetObject(ObjectType.Door, false);
            //door.Layered = LayeredDoor;
            door.StampAsUsed(0);

            door.SetDoorType(BackdropTileset);

            AddObject(door);

            // Place the door above the block
            block.Box.CalcBounds_Full();
            door.PlaceAt(new Vector2(pos.X, block.Box.TR.Y + 1));

            // If we don't want a backdrop we're done
            if (!AddBackdrop)
            {
                door.Core.DrawLayer = DesiredDoorLayer;
                door.Core.DrawLayer2 = DesiredDoorLayer2;
                return door;
            }

            // Add a backdrop block that the door opens into
            NormalBlock backblock = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
            MadeBackBlock = backblock;
            backblock.Clone(block);

            door.MyBackblock = backblock;

            if (BackdropTileset.DungeonLike)
            {
                if (CurMakeData.PieceSeed.ZoomType == LevelZoom.Big)
                {
                    backblock.Extend(Side.Top, door.Pos.Y + 800);
                    backblock.Extend(Side.Bottom, door.Pos.Y - 2800);

                    SetBackblockProperties(backblock);
                }
                else
                {
                    backblock.Extend(Side.Top, MainCamera.TR.Y + 25);
                    backblock.Extend(Side.Bottom, MainCamera.BL.Y);

                    SetBackblockProperties(backblock);

                    // Additional block to complete the lower portion of the backblock
                    NormalBlock backblock2 = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
                    backblock2.Clone(block);

                    backblock2.Extend(Side.Top, MainCamera.BL.Y + 125);
                    backblock2.Extend(Side.Bottom, MainCamera.BL.Y - 300);

                    SetBackblockProperties(backblock2);
                }
            }
            else if (BackdropTileset == TileSets.Island)
            {
                BackdropTileset = backblock.Core.MyTileSet = TileSets.Terrace;
                backblock.Box.TopOnly = false;
                backblock.Extend(Side.Top, block.Box.Current.TR.Y + 800);
                backblock.Extend(Side.Bottom, block.Box.Current.TR.Y - 30);
                backblock.Extend(Side.Left, block.Box.Current.BL.X + 70);
                backblock.Extend(Side.Right, block.Box.Current.TR.X - 70);

                SetBackblockProperties(backblock);
            }
            else
            {
                backblock.Extend(Side.Top, MainCamera.TR.Y + 500);
                backblock.Extend(Side.Bottom, MainCamera.BL.Y - 500);

                SetBackblockProperties(backblock);
            }
            backblock.Core.MyTileSet = BackdropTileset;

            // Cement
            if (MyTileSet == TileSets.Cement)
                backblock.Core.MyTileSet = TileSets.CastlePiece;

            // Make sure door is just in front of backdrop
            door.Core.DrawLayer = DesiredDoorLayer;

            return door;
        }

        void SetBackblockProperties(NormalBlock backblock)
        {
            float shade = .85f;
            backblock.MyDraw.SetTint(new Color(shade, shade, shade));

            backblock.Core.GenData.Used = true;
            backblock.BlockCore.NonTopUsed = true;
            backblock.BlockCore.Virgin = true;

            backblock.BlockCore.UseTopOnlyTexture = false;
            backblock.Box.TopOnly = true;

            backblock.BlockCore.DrawLayer = 0;

            backblock.Core.Real = false;

            AddBlock(backblock);
        }

        public static void SpreadStartPositions(LevelPiece piece, MakeData make, Vector2 pos, Vector2 SpanPer)
        {
            int n = Math.Max(1, make.NumInitialBobs);
            Vector2 span = SpanPer * (n - 1);
            Vector2 add = span / n;
            for (int i = 0; i < n; i++)
            {
                //PhsxData[] data = piece.StartData;
                piece.StartData[i].Position = pos + span / 2 - i * add;
            }
        }

        /// <summary>
        /// Create the initial platforms the players start on.
        /// </summary>
        public float MakeInitialPlats(Vector2 BL, Vector2 TR, SingleData Style)
        {
            Vector2 size;
            Vector2 pos;
            NormalBlock block = null;

            size = new Vector2(350, 2250);
            if (CurMakeData.SkinnyStart)
                size.X = 75;

            pos = new Vector2(BL.X + 10, (BL.Y + TR.Y) / 2) + new Vector2(0, -size.Y);

            pos += Info.ShiftStartBlock;

            switch (Style.MyInitialPlatsType)
            {
                case StyleData.InitialPlatsType.Spaceship:
                    return MakeInitial_Spaceship(ref BL, ref TR, ref pos, ref block);

                case StyleData.InitialPlatsType.Door:
                case StyleData.InitialPlatsType.CastleToTerrace:
                    size.X += 100;
                    pos.X -= 50;

                    block = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);

                    // New style end blocks
                    if (MyTileSet.FixedWidths)
                    {
                        block.BlockCore.StartPiece = true;
                        block.Core.DrawLayer = 0;
                    }

                    block.Init(pos, size, MyTileSetInfo);
                    block.BlockCore.BlobsOnTop = false;
                    block.StampAsUsed(0);
                    block.Core.GenData.RemoveIfUnused = false;

                    // Whether this start piece is a Castle-To-Terrace transition
                    bool CastleToTerrace = Style.MyInitialPlatsType == StyleData.InitialPlatsType.CastleToTerrace;

                    if (MyTileSet == TileSets.Cement || CastleToTerrace)
                        block.Core.MyTileSet = TileSets.Catwalk;

                    if (CurMakeData.PieceSeed.ZoomType == LevelZoom.Big)
                        block.Extend(Side.Left, block.Box.BL.X + 30);

                    // Randomize height of start
                    block.Extend(Side.Top, block.Box.TR.Y +
                        Rnd.RndFloat(Style.InitialDoorYRange) +
                        0);

                    AddBlock(block);

                    // Make the door
                    if (CurMakeData.PieceSeed.ZoomType == LevelZoom.Big)
                        pos.X += 150;

                    // Sky
                    if (block.Core.MyTileSet == TileSets.Island)
                    {
                        block.Stretch(Side.Left, 55);
                        block.Move(new Vector2(160, 0));
                        pos.X += 265;
                    }

                    pos.X += Info.ShiftStartDoor;

                    Door door;
                    if (CastleToTerrace)
                    {
                        block.Stretch(Side.Left, -2000);
                        block.Stretch(Side.Right, 500);
                        door = PlaceDoorOnBlock(pos + new Vector2(380, 0), block, true, TileSets.CastlePiece);
                    }
                    else
                        door = PlaceDoorOnBlock(pos, block, MyTileSet.CustomStartEnd ? false : true);
                    door.Core.EditorCode1 = LevelConnector.StartOfLevelCode;

                    // Shift start position
                    SpreadStartPositions(CurPiece, CurMakeData, door.Pos, new Vector2(50, 0));

                    return block.Box.TR.X + Rnd.RndFloat(100, 250);

                case StyleData.InitialPlatsType.Normal:
                    return MakeInitial_Normal(BL, TR, size);

                case StyleData.InitialPlatsType.LandingZone:
                    return MakeInitial_LandingZone(ref BL, ref TR, ref size);
            }

            return 0;
        }

        private float MakeInitial_LandingZone(ref Vector2 BL, ref Vector2 TR, ref Vector2 size)
        {
            NormalBlock nblock;
            Vector2 Pos = BL;
            size = new Vector2(2200, 200);
            nblock = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
            nblock.Core.MyTileSet = MyTileSet;
            nblock.Init(Pos, size, MyTileSetInfo);
            nblock.Extend(Side.Bottom, MainCamera.BL.Y - 300);
            nblock.Extend(Side.Top, MainCamera.BL.Y + 500);
            //nblock.Extend(Side.Right, Pos.X + MainCamera.GetWidth() + 7500);
            //nblock.Extend(Side.Left, Pos.X + MainCamera.GetWidth() - 230);

            nblock.Core.GenData.RemoveIfUnused = false;
            nblock.BlockCore.Virgin = true;
            nblock.Core.GenData.AlwaysLandOn = true;

            nblock.Core.EditorCode1 = "Landing Platform";

            AddBlock(nblock);

            // Shift start position
            for (int i = 0; i < CurMakeData.NumInitialBobs; i++)
            {
                CurPiece.StartData[i].Position.X += size.X - 500;
            }

            return VanillaFill(BL + new Vector2(size.X, 0), new Vector2(BL.X + size.X + 500, TR.Y - 500), 600);
        }

        private float MakeInitial_Spaceship(ref Vector2 BL, ref Vector2 TR, ref Vector2 pos, ref NormalBlock block)
        {
            BL = BL + new Vector2(-600, -400);
            TR = new Vector2(BL.X + 750, TR.Y + 400);

            return TR.X + 400;


            pos = BL;
            while (pos.Y < TR.Y)
            {
                block = NormalBlock_AutoGen.Instance.CreateCementBlockLine(this, pos, new Vector2(TR.X, pos.Y));
                pos.Y += 2 * block.Box.Current.Size.Y;

                block.Core.GenData.KeepIfUnused = true;
                block.Core.GenData.RemoveIfUnused = false;
                block.Core.GenData.RemoveIfOverlap = false;
                block.BlockCore.BlobsOnTop = false;
                block.Core.GenData.AlwaysLandOn = true;
                block.BlockCore.Virgin = true;
                block.BlockCore.BlobsOnTop = false;

                AddBlock(block);
            }

            return TR.X + 400;
        }

        private float MakeInitial_Normal(Vector2 BL, Vector2 TR, Vector2 size)
        {
            NormalBlock block = null;

            Fill(BL, new Vector2(BL.X + 10, TR.Y), 200, 250,
                _pos =>
                {
                    block = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
                    block.Init(_pos + new Vector2(0, -size.Y), size, MyTileSetInfo);
                    block.Core.GenData.RemoveIfUnused = true;
                    block.BlockCore.BlobsOnTop = false;
                    block.Core.GenData.AlwaysLandOn = true;
                    AddBlock(block);
                });

            if (block == null) return 0;
            else return block.Box.TR.X;
        }

        delegate void MakeAt(Vector2 Pos);
        public void Stage1FinalPlats(Vector2 BL, Vector2 TR, ObjectType Type, ref MakeData makeData)
        {
            TR.X += 400;

            List<BouncyBlock> bouncys = new List<BouncyBlock>();
            GenerationData.UsedCallback UsedCallback = delegate()
            {
                foreach (BouncyBlock block in bouncys)
                    if (!block.Core.GenData.Used)
                        block.Core.MarkedForDeletion = true;
            };

            MakeAt MakeBouncy = delegate(Vector2 Pos)
            {
                BouncyBlock bouncy = (BouncyBlock)Recycle.GetObject(ObjectType.BouncyBlock, false);
                bouncy.Init(Pos, new Vector2(170, 170), 90, this);
                bouncy.Core.DrawLayer = 9;

                bouncy.Core.GenData.RemoveIfUnused = true;
                bouncy.BlockCore.BlobsOnTop = false;
                bouncy.Core.GenData.AlwaysLandOn = true;

                bouncy.Core.GenData.OnUsed = UsedCallback;

                AddBlock(bouncy);
                bouncys.Add(bouncy);
            };

            float Top;
            
            Top = -750;
            while (Top > BL.Y - 250)
            {
                MakeBouncy(new Vector2(TR.X - 250, Top));
                Top -= 100;
            }
            Top = 0;
            while (Top > BL.Y - 250)
            {                
                MakeBouncy(new Vector2(TR.X, Top));
                Top -= 100;
            }            
        }

        public delegate void ModBlockCallback(BlockBase block);
        public float VanillaFill(Vector2 BL, Vector2 TR, float width)
        {
            return VanillaFill(BL, TR, width, 200, null, null);
        }
        public float VanillaFill(Vector2 BL, Vector2 TR, float width, float ystep, ModBlockCallback PreInit, ModBlockCallback PostInit)
        {
            Vector2 Pos = BL;

            float step = 2 * width;

            NormalBlock block = null;
            while (Pos.X < TR.X)
            {
                while (Pos.Y < TR.Y)
                {
                    block = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);

                    if (PreInit != null)
                        PreInit(block);

                    block.Init(Pos + new Vector2(width, -200), new Vector2(width, 200), MyTileSetInfo);
                    block.Extend(Side.Bottom, BL.Y - 300 - CurMakeData.PieceSeed.ExtraBlockLength);

                    block.Core.GenData.RemoveIfUnused = true;
                    block.BlockCore.Virgin = true;
                    block.Core.GenData.AlwaysLandOn = true;
                    block.Core.GenData.AlwaysUse = true;

                    // Make the last column of blocks EdgeJumpOnly
                    if (Pos.X + step >= TR.X)
                        block.Core.GenData.EdgeJumpOnly = true;

                    if (PostInit != null)
                        PostInit(block);

                    AddBlock(block);

                    Pos.Y += ystep;
                }

                Pos.Y = BL.Y;
                Pos.X += step;
            }

            return Pos.X;
        }

        public float RandomBlocks(Vector2 BL, Vector2 TR, ref MakeData makeData)
        {
            Vector2 Pos = BL;

            float width = 400;
            float step = 2 * width;

            NormalBlock block = null;
            while (Pos.X < TR.X)
            {
                block = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
                block.Init(Pos + new Vector2(width, Rnd.RndFloat(200, 1000)), new Vector2(Rnd.RndFloat(width / 2, width), 200), MyTileSetInfo);
                block.Extend(Side.Bottom, BL.Y - 300);

                block.Core.GenData.RemoveIfUnused = true;
                block.BlockCore.BlobsOnTop = false;
                block.Core.GenData.AlwaysLandOn = true;

                // Make the last column of blocks EdgeJumpOnly
                if (Pos.X + step >= TR.X)
                    block.Core.GenData.EdgeJumpOnly = true;

                block.StampAsUsed(CurPhsxStep);

                AddBlock(block);

                Pos.Y = BL.Y;
                Pos.X += Rnd.RndFloat(step, 2 * step);
            }

            return Pos.X;
        }

        static int CountToSleep = 0;
        static void Sleep()
        {
            //Thread.Sleep(3);
        }
        static void CheckToSleep()
        {
            //CountToSleep++;
            //if (CountToSleep >= 5)
            //{
            //    CountToSleep = 0;
            //    Sleep();
            //}
        }
        
        public void Stage1RndFill(Vector2 BL, Vector2 TR, Vector2 BL_Cutoff, float Sparsity)
        {
            var NParams = (NormalBlock_Parameters)Style.FindParams(NormalBlock_AutoGen.Instance);

            float[] Weights = new float[Generators.WeightedPreFill_1_Gens.Count];

            Vector2 xstep = new Vector2(CurMakeData.PieceSeed.Style.FillxStep * Sparsity, 0);
            xstep.Y = xstep.X;
            Fill(BL, TR, xstep, CurMakeData.PieceSeed.Style.FillyStep,
                pos =>
                {
                    float MaxWeight = 0;
                    
                    // Find the relative weights of all the obstacles we wish to fill with 
                    for (int i = 0; i < Generators.WeightedPreFill_1_Gens.Count; i++)
                    {
                        AutoGen gen = Generators.WeightedPreFill_1_Gens[i];

                        if (gen != NormalBlock_AutoGen.Instance)
                        {                            
                            float weight = Style.FindParams(gen).FillWeight.GetVal(pos);
                            
                            Weights[i] = weight;
                            MaxWeight = Math.Max(MaxWeight, weight);
                        }
                    }

                    float NormalBlockTotal = Math.Max(0, 3f - MaxWeight / 3f);

                    Weights[Generators.WeightedPreFill_1_Gens.IndexOf(NormalBlock_AutoGen.Instance)] =
                        NParams.CustomWeight ?
                            NParams.FillWeight.GetVal(pos)
                            :
                            NormalBlockTotal * Style.ModNormalBlockWeight;
                            

                    // Choose a random generator and make a new obstacle with it
                    int choice = Rnd.Choose(Weights);
                    AutoGen chosen_gen = Generators.WeightedPreFill_1_Gens[choice];
                    ObjectBase NewObj = chosen_gen.CreateAt(this, pos, BL_Cutoff, TR);

                    if (NewObj == null) return;

                    // Keep new object if it's unused?
                    if (Rnd.RndFloat(0, 1) < CurMakeData.PieceSeed.Style.ChanceToKeepUnused
                        && pos.Y > BL.Y + 400) // Don't keep unused blocks that are too low
                    {
                        NewObj.Core.GenData.RemoveIfUnused = false;
                        NewObj.Core.GenData.RemoveIfOverlap = true;
                    }

                    CheckToSleep();
                });
        }




        public CameraZone MakeCameraZone()
        {
            return MakeCameraZone(new Vector2((CurMakeData.PieceSeed.End.X - CurMakeData.PieceSeed.Start.X) / 2 + 150, MainCamera.GetHeight() / 2));
        }
        public CameraZone MakeCameraZone(Vector2 Size)
        {
            PieceSeedData s = CurMakeData.PieceSeed;

            CameraZone CamZone = (CameraZone)Recycle.GetObject(ObjectType.CameraZone, false);
            CamZone.Init((s.Start + s.End) / 2, Size);
            CamZone.Start = s.Start + s.CamZoneStartAdd;
            CamZone.End = s.End + s.CamZoneEndAdd;

            if (s.ZoomType == LevelZoom.Big)
            {
                CamZone.Zoom = .55f;
                CamZone.Start.X += 1550;
                CamZone.End.X -= 600;
            }

            Vector2 Tangent = CamZone.End - CamZone.Start;
            if (CurMakeData.InitialCamZone)
                CamZone.Start = CamZone.Start + new Vector2(MainCamera.GetWidth() / 2 - 300, 0);
            else
                CamZone.Start -= 2 * Tangent;
            CamZone.End += 2 * Tangent;

            AddObject(CamZone);

            return CamZone;
        }

        /// <summary>
        /// A BL bound for filling, offset by beginning platforms.
        /// </summary>
        public Vector2 FillBL;

        /// <summary>
        /// The bounds of the random block fill.
        /// </summary>
        public Vector2 Fill_TR, Fill_BL;

        static bool showdebug = false;
        static bool dodebug = false;
        void DEBUG(string str)
        {
#if DEBUG
            if (!showdebug || !dodebug) return;

            CloudberryKingdom_XboxPC.debugstring = str;
            Thread.Sleep(5000);
            CloudberryKingdom_XboxPC.debugstring = "DOING" + str;
#endif
        }

        public void PREFILL()
        {
            if (!dodebug) return;

            for (int i = 0; i < 15000; i++)
            {
                Recycle.CollectObject(Recycle.GetNewObject(ObjectType.MovingPlatform, true));
                Recycle.CollectObject(Recycle.GetNewObject(ObjectType.BlockEmitter, true));
                Recycle.CollectObject(Recycle.GetNewObject(ObjectType.SpikeyLine, true));
                Recycle.CollectObject(Recycle.GetNewObject(ObjectType.NormalBlock, true));
                Recycle.CollectObject(Recycle.GetNewObject(ObjectType.Coin, true));
            }
        }

        public static string Pre1, Pre2, Post;
        public static int Step1, Step2;

        public float MaxRight, EndBuffer;
        public int LastStep;
        public bool MakeSingle(int Length, float MaxRight, float MaxLeft, int StartPhsxStep, int ReturnEarly, MakeData makeData)
        {
            int TestNumber;

            // Tracking info
            Pre1 = Pre2 = Post = ""; Step1 = Step2 = 0;
            Pre1 += 'A';

            this.MaxRight = MaxRight;

            PREFILL();
            DEBUG("Pre stage 1, about to fill");
            TestNumber = Rnd.RndInt(0, 1000);
            Tools.Write(string.Format("Test: {0}", TestNumber));

            CurMakeData = makeData;            
            InitMakeData(CurMakeData);
            SingleData Style = (SingleData)CurMakeData.PieceSeed.Style;

            // Calculate the style parameters
            Style.CalcGenParams(CurMakeData.PieceSeed, this);

            // Move camera
            MainCamera.Data.Position = CurMakeData.CamStartPos;
            MainCamera.Update();

            // New bobs
            Bob[] Computers = CurMakeData.MakeBobs(this);

            // New level piece
            LevelPiece Piece = CurPiece = CurMakeData.MakeLevelPiece(this, Computers, Length, StartPhsxStep);            

            // Camera Zone
            CameraZone CamZone = MakeCameraZone();
            CamZone.SetZoom(MainCamera);
            Sleep();

            // Set the camera start position
            if (CurMakeData.InitialCamZone)
                CurPiece.CamStartPos = CurMakeData.CamStartPos = CamZone.Start;
            else
                CurPiece.CamStartPos = CurMakeData.CamStartPos;


            Vector2 BL_Cutoff;

            float Left;
            Left = MaxLeft;

            NormalBlock_Parameters BlockParams = (NormalBlock_Parameters)Style.FindParams(NormalBlock_AutoGen.Instance);
            if (!BlockParams.DoInitialPlats) CurMakeData.InitialPlats = false;
            if (!BlockParams.DoFinalPlats) CurMakeData.FinalPlats = false;

            // Initial platform
            if (CurMakeData.InitialPlats && Style.MakeInitialPlats)
            {
                BL_Cutoff = new Vector2(MaxLeft, MainCamera.BL.Y);
                Left = MakeInitialPlats(new Vector2(MaxLeft - 10, MainCamera.BL.Y + 50), new Vector2(MaxRight, MainCamera.TR.Y - 50), Style);
                Sleep();
            }


            // Safety nets
            float SafetyWidth = 300;
            float ExtraSpace = 50;
            float VoidHeight = 0; // How much space to put at the bottom, with no Stage1

            // Make lava
            if (MySourceGame.HasLava)
            {
                VoidHeight = 40;
                LavaBlock lblock = (LavaBlock)Recycle.GetObject(ObjectType.LavaBlock, false);
                lblock.Init(MainCamera.BL.Y + Rnd.RndFloat(300, 800) + Style.LowerSafetyNetOffset,
                            MaxLeft - 1000, MaxRight + 1000, 5000);
                lblock.StampAsUsed(0);
                AddBlock(lblock);
            }

            // Invert phsx safety blocks
            if (CurMakeData.TopLikeBottom)
                Stage1SafetyNet(new Vector2(MaxLeft - 7500, MainCamera.TR.Y - VoidHeight - 215 - Style.LowerSafetyNetOffset + 1000 + Style.UpperSafetyNetOffset),
                            new Vector2(MaxRight + 1500, MainCamera.TR.Y - VoidHeight - 65 - Style.LowerSafetyNetOffset + 1000),
                            new Vector2(SafetyWidth, 500), 2 * SafetyWidth + ExtraSpace, Style.MyTopType);
            else if (CurMakeData.TopLikeBottom_Thin)
                Stage1SafetyNet(new Vector2(MaxLeft - 7500, MainCamera.TR.Y - VoidHeight - 215 - Style.LowerSafetyNetOffset + 1000 + Style.UpperSafetyNetOffset),
                            new Vector2(MaxRight + 1500, MainCamera.TR.Y - VoidHeight - 65 - Style.LowerSafetyNetOffset + 1000),
                            new Vector2(100, 500), 2 * 150 + 50, Style.MyTopType);

            LastSafetyBlock = Stage1SafetyNet(new Vector2(MaxLeft, MainCamera.BL.Y + VoidHeight + 65 + Style.LowerSafetyNetOffset),
                            new Vector2(MaxRight + 500, MainCamera.BL.Y + VoidHeight + 215 + Style.LowerSafetyNetOffset),
                            new Vector2(SafetyWidth, 200 + CurMakeData.PieceSeed.ExtraBlockLength), 2*SafetyWidth+ExtraSpace, Style.MyGroundType);

            //Stage1SafetyNet(new Vector2(MaxLeft, MainCamera.TR.Y + Style.UpperSafetyNetOffset),
            //                new Vector2(MaxRight, MainCamera.TR.Y + Style.UpperSafetyNetOffset + 100),
            //                new Vector2(750, 200), 1500, Style.MyTopType);
            Sleep();


            // Mid divider
            if (CurMakeData.MidDivider)
            {
                BlockBase block = NormalBlock_AutoGen.Instance.CreateCementBlockLine(this,
                    new Vector2(MaxLeft + 300, MainCamera.Data.Position.Y),
                    new Vector2(MaxRight + 600, MainCamera.Data.Position.Y));
                /*
                NormalBlock block = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
                block.Init(TR, Vector2.Zero);
                block.Extend(Side.Left, MaxLeft);
                block.Extend(Side.Right, MaxRight);
                block.Extend(Side.Top, MainCamera.Data.Position.Y + 30);
                block.Extend(Side.Bottom, MainCamera.Data.Position.Y - 30);*/
                //block.Core.GenData.Used = true;
                block.StampAsUsed(0);

                AddBlock(block);
            }

            // Final platform
            EndBuffer = 0;
            MakeThing MakeFinalPlat = null;
            if (CurMakeData.FinalPlats)
            {
                if (Style.MyFinalPlatsType == StyleData.FinalPlatsType.Door)
                {
                    if (DefaultHeroType == BobPhsxRocketbox.Instance || DefaultHeroType is BobPhsxSpaceship)
                        MakeFinalPlat = new MakeFinalDoor_Float(this);
                    else
                        MakeFinalPlat = new MakeFinalDoor(this);
                }
            }

            if (MakeFinalPlat != null) MakeFinalPlat.Phase1();

            Sleep();
            Pre1 += 'B';


            // Pre Fill #1
            FillBL = new Vector2(Left + 0, MainCamera.BL.Y + 150 + VoidHeight);

            Vector2 BL_Bound = new Vector2(MaxLeft + 100, MainCamera.BL.Y);//MainCamera.BLCamBound.Y - 1000);
            Vector2 TR_Bound = new Vector2(MaxRight - 400, MainCamera.TR.Y);// MainCamera.TRCamBound.Y + 1000);

            Fill_BL = new Vector2(Left, MainCamera.BL.Y + Style.BottomSpace);
            Fill_TR = new Vector2(MaxRight + 100, MainCamera.TR.Y - Style.TopSpace);
            foreach (AutoGen gen in Generators.PreFill_1_Gens)
            {
                //gen.PreFill_1(this, BL_Bound, TR_Bound);
                gen.PreFill_1(this, FillBL, TR_Bound);
                Sleep();
            }

            // Change sparsity multiplier
            if (CurMakeData.SparsityMultiplier == 1)
                CurMakeData.SparsityMultiplier = CurMakeData.GenData.Get(DifficultyParam.FillSparsity) / 100f;


            // Stage 1 Random fill
            if (BlockParams.DoStage1Fill)
            {
                BL_Cutoff = new Vector2(Left + 0, MainCamera.BL.Y);
                Fill_BL = new Vector2(Left, MainCamera.BL.Y + Style.BottomSpace);
                Fill_TR = new Vector2(MaxRight + 100, MainCamera.TR.Y - Style.TopSpace);
                Stage1RndFill(Fill_BL, Fill_TR, BL_Cutoff, 1 * CurMakeData.SparsityMultiplier);

                // Add a row at the very bottom, just to be safe.
                Fill_BL = new Vector2(Fill_BL.X, Fill_BL.Y + 35);
                Fill_TR = new Vector2(Fill_TR.X, Fill_BL.Y + 65);
                Stage1RndFill(Fill_BL, Fill_TR, BL_Cutoff, 1 * CurMakeData.SparsityMultiplier);

                // Add a second row at the very bottom, just to be super safe.
                Fill_BL = new Vector2(Fill_BL.X, Fill_BL.Y + 65);
                Fill_TR = new Vector2(Fill_TR.X, Fill_BL.Y + 85);
                Stage1RndFill(Fill_BL, Fill_TR, BL_Cutoff, 1 * CurMakeData.SparsityMultiplier);

                if (CurMakeData.TopLikeBottom)
                {
                    // Add a row at the very top, just to be safe.
                    Fill_BL = new Vector2(Fill_BL.X, Fill_TR.Y - 35);
                    Fill_TR = new Vector2(Fill_TR.X, Fill_TR.Y - 5);
                    Stage1RndFill(Fill_BL, Fill_TR, BL_Cutoff, 1 * CurMakeData.SparsityMultiplier);
                }
            }

            Pre1 += 'C';
            DEBUG("Pre stage 1, about to reset");
            TestNumber = Rnd.RndInt(0, 1000);
            Tools.Write(string.Format("Test: {0}", TestNumber));

            PlayMode = 2;
            RecordPosition = true;
            
            if (PieceSeed.PreStage1 != null) PieceSeed.PreStage1(this);
            ResetAll(true);

            DEBUG("Pre stage 1, about to run through");

            // Set special Bob parameters
            MySourceGame.SetAdditionalBobParameters(Computers);

            CurMakeData.TRBobMoveZone = new Vector2(MaxRight + EndBuffer, MainCamera.TR.Y + 500);
            CurMakeData.BLBobMoveZone = new Vector2(MaxLeft, MainCamera.BL.Y - 500);
            if (ReturnEarly == 1) return false;

            TestNumber = Rnd.RndInt(0, 1000);
            Tools.Write(string.Format("Test a: {0}", TestNumber));
            
            // Stage 1 Run through
            Pre1 += 'D';
            Stage1(BL_Bound, TR_Bound, Length);
            Pre2 += 'A';

            TestNumber = Rnd.RndInt(0, 1000);
            Tools.Write(string.Format("Test b: {0}", TestNumber));

            // Continue making Final Platform
            if (MakeFinalPlat != null) MakeFinalPlat.Phase2();

            //Console.WriteLine("Stage 1 finished at " + LastStep.ToString());
            LastPoint = Bobs[0].Core.Data.Position + new Vector2(10, 0);

            // Update the level's par time
            CurPiece.Par = LastStep;
            Par += CurPiece.Par;

            TestNumber = Rnd.RndInt(0, 1000);
            Tools.Write(string.Format("Test c: {0}", TestNumber));

            DEBUG("Done with stage 1 run through, about to cleanup");

            // Stage 1 Cleanup
            Stage1Cleanup(BL_Bound, TR_Bound);
            Pre2 += 'B';

            CurPiece.PieceLength = LastStep - StartPhsxStep;


            DEBUG("Pre stage 2, about to fill");


            // Pre Fill #2
            foreach (AutoGen gen in Generators.PreFill_2_Gens)
            {
                //gen.PreFill_2(this, BL_Bound, TR_Bound);
                gen.PreFill_2(this, FillBL, TR_Bound);
                Sleep();
            }


            FinalizeBlocks();

            Pre2 += 'C';
            DEBUG("Pre stage 2, about to reset");
            TestNumber = Rnd.RndInt(0, 1000);
            Tools.Write(string.Format("Test d: {0}", TestNumber));

            PlayMode = 1;
            RecordPosition = false;
            if (PieceSeed.PreStage2 != null) PieceSeed.PreStage2(this);
            ResetAll(true);
            Sleep();

            MySourceGame.SetAdditionalBobParameters(Computers);

            if (ReturnEarly == 2) return false;

            DEBUG("Pre stage 2, about to run through");
            Pre2 += 'D';

            // Stage 2 Run through
            Stage2();
            Post += 'A';

            DEBUG("Done with stage 2 run through, about to cleanup");
            showdebug = true;

            // Stage 2 Cleanup
            Stage2Cleanup(BL_Bound, TR_Bound);
            Post += 'B';

            //Recycle.Empty();

            // Finish making Final Platform
            if (MakeFinalPlat != null) { MakeFinalPlat.Phase3(); MakeFinalPlat.Cleanup(); }

            return false;
        }

        void Stage1(Vector2 BL_Bound, Vector2 TR_Bound, int Length)
        {
            int OneFinishedCount = 0; // Number of frames since at least one player finished
            int AdditionalSteps = 10;//200; // Steps to take after computer reaches end
            while (CurPhsxStep - Bobs[0].IndexOffset < Length)// CurPiece.PieceLength)
            {
                Step1 = CurPhsxStep;
                /*
                // End if all bobs have arrived
                if (!Bobs.Any(bob => bob.Core.Data.Position.X < MaxRight + EndBuffer))
                    OneFinishedCount += 8;

                // End after first computer arrives at end
                if (Bobs.Any(bob => bob.Core.Data.Position.X > MaxRight + EndBuffer - 100))
                    OneFinishedCount++;
                */
                // Do the above without delegates
                bool Any;
                Any = false; foreach (Bob bob in Bobs) if (bob.Core.Data.Position.X < MaxRight + EndBuffer) Any = true;
                if (!Any) OneFinishedCount += 8;
                Any = false; foreach (Bob bob in Bobs) if (bob.Core.Data.Position.X > MaxRight + EndBuffer - 100) Any = true;
                if (Any) OneFinishedCount++;


                if (OneFinishedCount > AdditionalSteps)
                    break;

                PhsxStep(true);
                foreach (AutoGen gen in Generators.ActiveFill_1_Gens)
                    //gen.ActiveFill_1(this, BL_Bound, TR_Bound);
                    gen.ActiveFill_1(this, FillBL, TR_Bound);
            }
            LastStep = CurPhsxStep;
        }

        void Stage1Cleanup(Vector2 BL_Bound, Vector2 TR_Bound)
        {
            SingleData Style = (SingleData)CurMakeData.PieceSeed.Style;

            foreach (AutoGen gen in Generators.Gens)
                gen.Cleanup_1(this, BL_Bound, TR_Bound);

            // Overlapping blocks
            if (Style.RemovedUnusedOverlappingBlocks)
                BlockOverlapCleanup();
            Sleep();

            // Remove unused objects
            foreach (ObjectBase obj in Objects)
                if (!obj.Core.GenData.Used && obj.Core.GenData.RemoveIfUnused)
                    Recycle.CollectObject(obj);
            CleanObjectList();
            Sleep();
            
            // Remove unused blocks
            foreach (BlockBase _block in Blocks)
                if (!_block.Core.GenData.Used && _block.Core.GenData.RemoveIfUnused)
                    Recycle.CollectObject(_block);
            CleanBlockList();
            CleanDrawLayers();
            Sleep();
        }

        void Stage2()
        {
            while (CurPhsxStep < LastStep)
            {
                Step2 = CurPhsxStep;
                PhsxStep(true);
            }
            //Console.WriteLine("Stage 2 finished at " + CurPhsxStep.ToString());
        }


        BSP_Line MyBSP;
        void Stage2Cleanup(Vector2 BL_Bound, Vector2 TR_Bound)
        {
            OverlapCleanup();
            CleanAllObjectLists();
            Sleep();

            // Create the Binary Space Partition
            //MyBSP = new BSP_Line(Objects);
            MyBSP = null;

            // Limit general density of all obstacles.
            Cleanup(Objects.FindAll(obj => obj.Core.GenData.LimitGeneralDensity), delegate(Vector2 pos)
            {
                float dist = CurMakeData.GenData.Get(DifficultyParam.GeneralMinDist, pos);
                return new Vector2(dist, dist);
            }, true, BL_Bound, TR_Bound);
            Sleep();

            // Kill the BSP
            MyBSP = null;

            // Note:
            // Eventually you should pass the BSP object directly to the cleanup.
            // This way Firespinner cleanup could have an exclusively firespinner BSP.



            /*Cleanup(ObjectType.Coin, delegate(Vector2 pos)
            {
                return new Vector2(180, 180);
            }, BL_Bound, TR_Bound + new Vector2(500, 0));
            Sleep();*/
            foreach (ObjectBase obj in Objects)
            {
                Coin coin = obj as Coin;
                if (null != coin)
                    if (coin.Core.AddedTimeStamp > LastStep)
                        Recycle.CollectObject(coin);
            }

            foreach (AutoGen gen in Generators.Gens)
                gen.Cleanup_2(this, BL_Bound, TR_Bound);

            // Clean up deleted objects
            CleanAllObjectLists();
        }

        public void SortBlocks()
        {
            Blocks.Sort(delegate(BlockBase A, BlockBase B) { return A.BlockCore.Layer.CompareTo(B.BlockCore.Layer); });
        }

        public void OverlapCleanup()
        {
            foreach (ObjectBase obj in Objects)
            {
                if (obj.Core.GenData.NoBlockOverlap)
                {
                    foreach (BlockBase block in Blocks)
                    {
                        if (block.BlockCore.RemoveOverlappingObjects && block != obj.Core.ParentBlock && Phsx.PointAndAABoxCollisionTest(ref obj.Core.Data.Position, block.Box, obj.Core.GenData.OverlapWidth))
                            Recycle.CollectObject(obj);
                    }
                }
            }
        }

        public void BlockOverlapCleanup()
        {
            if (Style.OverlapCleanupType == StyleData._OverlapCleanupType.Regular)
                RegularBlockCleanup();
            else
                SpaceshipBlockCleanup();
        }

        void SpaceshipBlockCleanup()
        {
            foreach (BlockBase block2 in Blocks)
            {
                if (block2.Core.MarkedForDeletion) continue;

                foreach (BlockBase block in Blocks)
                {
                    if (block2.Core.MarkedForDeletion) break;
                    if (block.Core.GenData.Used || block.Core.MarkedForDeletion) continue;

                    if (block != block2 && block.Core.GenData.RemoveIfOverlap && block2.Core.GenData.RemoveIfOverlap &&
                         ((block.Core.Data.Position - block2.Core.Data.Position).Length() < CurMakeData.PieceSeed.Style.MinBlockDist || Phsx.BoxBoxOverlap(block.Box, block2.Box)))
                    {
                        switch (block.Core.GenData.MyOverlapPreference)
                        {
                            case GenerationData.OverlapPreference.RemoveHigherThanMe:
                                if (block2.Box.Target.TR.Y > block.Box.Target.TR.Y)
                                    Recycle.CollectObject(block2);
                                else
                                    Recycle.CollectObject(block);
                                break;

                            case GenerationData.OverlapPreference.RemoveLowerThanMe:
                                if (block2.Box.Target.TR.Y > block.Box.Target.TR.Y)
                                    Recycle.CollectObject(block);
                                else
                                    Recycle.CollectObject(block2);
                                break;

                            case GenerationData.OverlapPreference.RemoveRandom:
                                if (Rnd.RndBool())
                                    Recycle.CollectObject(block2);
                                else
                                    Recycle.CollectObject(block);
                                break;
                        }
                    }
                }
            }
        }

        void RegularBlockCleanup()
        {
            foreach (BlockBase block2 in Blocks)
            {
                if (block2.Core.MarkedForDeletion) continue;

                foreach (BlockBase block in Blocks)
                {
                    if (block.Core.GenData.Used || block.Core.MarkedForDeletion) continue;

                    if (block != block2 && block.Core.GenData.RemoveIfOverlap &&
                         ((block.Core.Data.Position - block2.Core.Data.Position).Length() < CurMakeData.PieceSeed.Style.MinBlockDist || Phsx.BoxBoxOverlap(block.Box, block2.Box)))
                    {
                        Recycle.CollectObject(block);
                    }
                }
            }
        }


        void InitMakeData(MakeData makeData)
        {
            PieceSeedData Piece = makeData.PieceSeed;

            makeData.Init(Piece);

            switch (makeData.PieceSeed.Paths)
            {
                case 1:
                    makeData.PieceSeed.Style.SetSinglePathType(makeData, this, Piece);

                    break;

                case 2:
                    makeData.PieceSeed.Style.SetDoubePathType(makeData, this, Piece);

                    break;

                case 3:
                    makeData.PieceSeed.Style.SetTriplePathType(makeData, this, Piece);

                    break;
            }


            if (makeData.Index == 0)
            {
                makeData.InitialPlats = true;
                makeData.InitialCamZone = true;
            }
            else
            {
                makeData.InitialPlats = false;
                makeData.InitialCamZone = false;
            }

            if (makeData.Index == makeData.OutOf - 1)
                makeData.FinalPlats = true;
            else
            {
                makeData.FinalPlats = false;
            }

            makeData.CamStartPos = MainCamera.Data.Position;

            if (makeData.ModData != null)
                makeData.ModData(ref makeData);
        }
    }
}
