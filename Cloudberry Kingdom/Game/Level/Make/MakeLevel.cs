using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Obstacles;
using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom
{
    public partial class LevelSeedData
    {
        public static bool NoDoublePaths = true;

        public Level MakeLevel(GameData game)
        {
            return MakeLevel(true, game);
        }
        public Level MakeLevel(bool MakeBackground, GameData game)
        {
            this.Seed = this.Seed;
            int TestNumber;

            game.EndMusicOnFinish = !NoMusicStart;

            game.DefaultHeroType = this.DefaultHeroType;// = BobPhsxMario.Instance;

            Level NewLevel = MakeNewLevel(game);
            NewLevel.Geometry = MyGeometry;
            Camera cam = NewLevel.MainCamera;

            // Set lava
            if (NewLevel.Info.AllowLava)
            //switch (LavaMakeTypes.NeverMake)
            switch (LavaMake)
            {
                case LavaMakeTypes.AlwaysMake: game.HasLava = true; break;
                case LavaMakeTypes.Random: game.HasLava = Rnd.RndBool(.38f); break;
                default: game.HasLava = false; break;
            }
            if (!NewLevel.MyBackground.AllowLava)
                game.HasLava = false;

            NewLevel.MyLevelSeed = this;

            float Height = 0;

            Level level = null;
            foreach (PieceSeedData Piece in PieceSeeds)
            {
                if (Piece.Ladder != Level.LadderType.None)
                {
                    NewLevel.MakeLadder(Piece);

                    if (PieceSeeds.IndexOf(Piece) == 0)
                        cam.BLCamBound = cam.Data.Position;

                    continue;
                }

                // Ensure that there are no blobs on a single path, multiplayer level
                if (Piece.Paths == 1 && !Piece.LockNumOfPaths)
                if (PlayerManager.GetNumPlayers() > 1 && Piece.MyUpgrades1[Upgrade.FlyBlob] > 0)
                {
                    Piece.Paths = 2;
                }

                // Ensure there are no double paths!
                // Would like to allow double paths, but they are broken sometimes.
                if (NoDoublePaths)
                    Piece.Paths = 1;

                // Ensure that bungee levels have at least two paths!
                if (MyGameFlags.IsTethered && Piece.Paths == 1)
                {
                    Piece.Paths = 2;
                }

                TestNumber = Rnd.RndInt(0, 1000);
                Tools.Write("Test 1 ---> {0}", TestNumber);
                


                level = new Level(true);
                level.MySourceGame = game;
                level.MyLevelSeed = this;
                level.MyTileSet = NewLevel.MyTileSet;
                level.DefaultHeroType = NewLevel.DefaultHeroType;
                level.MainCamera = new Camera();

                cam.Data.Position = Piece.Start + new Vector2(1000, 0);
                cam.Update();


                Level.MakeData makeData = new Level.MakeData();
                makeData.LevelSeed = this;
                makeData.PieceSeed = Piece;
                makeData.GenData = Piece.MyGenData;
                makeData.Index = PieceSeeds.IndexOf(Piece);
                makeData.OutOf = PieceSeeds.Count;

                int ReturnEarly = SetReturnEarly(Piece);

                level.Geometry = Piece.GeometryType;
                level.BoxesOnly = true;
                switch (Piece.GeometryType)
                {
                    case LevelGeometry.Right:
                        level.MakeSingle(3000, Piece.End.X, Piece.Start.X, 0, ReturnEarly, makeData);
                        break;

                    case LevelGeometry.Down:
                    case LevelGeometry.Up:
                        level.MakeVertical(3000, Math.Abs(Piece.End.Y - Piece.Start.Y), 0, ReturnEarly, makeData);
                        Height += Math.Abs(Piece.End.Y - Piece.Start.Y);
                        break;

                    //case LevelGeometry.Big:
                    //    level.MakeBig(this.Length, 0, ReturnEarly, makeData);
                    //    break;

                    //case LevelGeometry.OneScreen:
                    //    level.MakeOneScreen(this.Length, ReturnEarly, makeData);
                    //    break;
                }

                TestNumber = Rnd.RndInt(0, 1000);
                Tools.Write("Test 1.5 ---> {0}", TestNumber);
                    
                
                if (ReturnEarly > 0)
                {
                    //Tools.StepControl = true;
                    Tools.DrawBoxes = true;

                    //Bob Player = level.Bobs[0];
                    //NewLevel.Bobs.Clear();
                    //NewLevel.AddBob(Player);
                    level.MainEmitter = new CloudberryKingdom.Particles.ParticleEmitter(1000);
                    foreach (Bob bob in level.Bobs)
                        bob.SetColorScheme(ColorSchemeManager.ColorSchemes[0]);
                    level.SetToWatchMake = true;
                    NewLevel = level;

                    NewLevel.ReturnedEarly = true;

                    Tools.CurGameData = NewLevel.MySourceGame;

                    return NewLevel;
                }

                if (PieceSeeds.IndexOf(Piece) == 0)
                    NewLevel.MainCamera.BLCamBound = level.MainCamera.BLCamBound;


                level.ResetAll(false);
                                
                // Add checkpoints
                if (Piece.CheckpointsAtStart && !Piece.Style.SuppressCheckpoints)
                {
                    for (int i = 0; i < level.LevelPieces[0].NumBobs; i++)
                    {
                        Checkpoint checkpoint = (Checkpoint)game.Recycle.GetObject(ObjectType.Checkpoint, false);
                        checkpoint.Init(level);

                        PhsxData data = level.LevelPieces[0].StartData[i];
                        data.Position.X = level.LevelPieces[0].StartData[0].Position.X;
                        data.Position += level.LevelPieces[0].CheckpointShift[i];

                        checkpoint.Core.StartData = checkpoint.Core.Data = data;

                        checkpoint.MyPiece = level.LevelPieces[0];
                        checkpoint.MyPieceIndex = i;

                        level.AddObject(checkpoint);
                    }
                }

                
                level.Cleanup(ObjectType.Checkpoint, new Vector2(900, 900));
                level.CleanAllObjectLists();

                // Add initial start data
                if (Piece.InitialCheckpointsHere)
                {
                    NewLevel.CurPiece = level.LevelPieces[0];
                }

                // Absorb the new level
                NewLevel.AbsorbLevel(level);

                // Absorb the new level's time type
                if (level.TimeType != Level.TimeTypes.Regular)
                    NewLevel.TimeType = level.TimeType;

                NewLevel.Par += level.Par; // Add the level's par to the big level's par

                TestNumber = Rnd.RndInt(0, 1000);
                Tools.Write("Test 2 ---> {0}", TestNumber);
            }

            // Cleanup lava
            List<BlockBase> Lavas = new List<BlockBase>();
            foreach (var block in NewLevel.Blocks)
                if (block.Core.MyType == ObjectType.LavaBlock)
                    Lavas.Add(block);

            if (Lavas.Count > 0)
            {
                // Find the lowest watermark
                BlockBase Lowest = null;
                foreach (var lava in Lavas)
                    if (Lowest == null || lava.Box.TR.Y < Lowest.Box.TR.Y)
                        Lowest = lava;
                //Lavas.ArgMin(lava => lava.Box.TR.Y);

                // Extend left and right to cover whole level
                Lowest.Extend(Side.Left, NewLevel.BL.X - 5000);
                Lowest.Extend(Side.Right, NewLevel.TR.X + 5000);

                // Push down a bit
                Lowest.Extend(Side.Top, Lowest.Box.TR.Y - Rnd.RndFloat(0, 60));
                if (Lowest.Box.TR.Y < -840) Lowest.CollectSelf();

                // Remove extra lava blocks
                foreach (var lava in Lavas)
                    if (lava != Lowest)
                        lava.CollectSelf();
                //Lavas.ForEach(lava => { if (lava != Lowest) lava.CollectSelf(); });
            }

            if (MakeBackground)
                MakeTheBackground(NewLevel, Height);

            // Select first piece of level to start
            NewLevel.SetCurrentPiece(0);

            // Count coins, blobs
            NewLevel.CountCoinsAndBlobs();

            return NewLevel;
        }

        private void MakeTheBackground(Level NewLevel, float Height)
        {
            NewLevel.MyBackground.Init(NewLevel);
        }

        public static int ForcedReturnEarly = 0;
        private int SetReturnEarly(PieceSeedData Piece)
        {
            int ReturnEarly = ForcedReturnEarly;

            return ReturnEarly;
        }
    }
}