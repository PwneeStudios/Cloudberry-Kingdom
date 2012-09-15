using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Goombas;
using Drawing;

namespace CloudberryKingdom
{
    public static class Campaign
    {
        /// <summary>
        /// Whether we are playing the campaign right now.
        /// </summary>
        public static bool IsPlaying = false;

        public static int NumDifficulties = 5;
        public static float Difficulty;
        public static int Index;
        public static DifficultyFunc D;

        public static DoorDict<DoorData> Data;
        public static DoorData CurData;

        public static bool PartiallyInvisible, TotallyInvisible;

        public static int Score, Attempts, Time;
        public static int Coins, TotalCoins;
        public static void CalcScore()
        {
            //Score = PlayerManager.PlayerSum(p => p.CampaignStats.Score);
            
            Score = Data.Values.Sum(d => d.Score);
            Coins = Data.Values.Sum(d => d.Coins);
            TotalCoins = Data.Values.Sum(d => d.TotalCoins);
        }

        static int[] LevelLength = { 5000, 5500, 5700, 5900, 6500 };
        static int[] LevelNumPieces = { 1,    2,    2,    2,    2 };

        public static int[] BossQuota = { 40, 50, 60, 70, 80 };

        /// <summary>
        /// Get the base length for how long a campaign level should be.
        /// </summary>
        /// <returns></returns>
        public static int Length()
        {
            return LevelLength[Index];
        }
        public static int Length(float Mod)
        {
            return (int)(Mod * LevelLength[Index]);
        }

        /// <summary>
        /// Get the number of pieces a campaign level should have.
        /// </summary>
        /// <returns></returns>
        public static int NumPieces()
        {
            return LevelNumPieces[Index];
        }

        static int[] StartingBank = { 999, 100, 10, 0, -99 };
        static float[] IndexDifficulty = { 0, .85f, 1.9f, 3f, 4f };
        public static void InitCampaign(int difficulty)
        {
            IsPlaying = true;

            Tools.CurrentAftermath = null;

            PlayerManager.CoinsSpent = -StartingBank[difficulty];

            PartiallyInvisible = TotallyInvisible = true;

            // Perfect
            PerfectScoreObject.GlobalBonus = 0;
            PerfectScoreObject.GlobalObtained = false;

            // Campaign stats
            Score = Attempts = Time = 0;
            Coins = TotalCoins = 0;
            PlayerManager.ExistingPlayers.ForEach(player =>
                player.CampaignStats.Clean());

            // Difficulty
            Index = difficulty;
            //Difficulty = difficulty;
            Difficulty = IndexDifficulty[Index];
            D = DifficultyGroups.GetFunc(Difficulty);

            // Door data
            Data = new DoorDict<DoorData>();

            // Intros
            Campaign_IntroWorld.WatchedIntro = false;
            Campaign_TunnelOutside.WatchedIntro = false;
            Campaign_Boss.Beaten = false;

            // Boss
            SavePrincessRush_Intro.CampaignReset();
            Campaign_BossNew.CampaignReset();
        }

        static void StandardInit(LevelSeedData data)
        {
            data.Seed = data.Rnd.Rnd.Next();

            data.MyGameType = NormalGameData.Factory;
            data.SetTileSet(data.Rnd.Choose(new TileSet[] { TileSets.Terrace, TileSets.Dungeon, TileSets.Castle }));

            data.DefaultHeroType = BobPhsxNormal.Instance;

            data.NumPieces = 1;
            data.Length = 3000;
        }

        public static LevelSeedData HeroLevel(float Difficulty, BobPhsx Hero, float LengthMod, TileSet tileset)
        {
            return HeroLevel(Difficulty, Hero, Campaign.Length(LengthMod), Campaign.NumPieces(), LevelGeometry.Right, tileset);
        }
        public static LevelSeedData HeroLevel(float Difficulty, BobPhsx Hero, float LengthMod)
        {
            return HeroLevel(Difficulty, Hero, Campaign.Length(LengthMod), Campaign.NumPieces(), LevelGeometry.Right);
        }
        public static LevelSeedData HeroLevel(float Difficulty, BobPhsx Hero, int Length, int NumPieces)
        {
            return HeroLevel(Difficulty, Hero, Length, NumPieces, LevelGeometry.Right);
        }
        public static LevelSeedData HeroLevel(float Difficulty, BobPhsx Hero, int Length, int NumPieces, LevelGeometry Geometry)
        {
            return HeroLevel(Difficulty, Hero, Length, NumPieces, Geometry, TileSets.Random);
        }
        public static LevelSeedData HeroLevel(float Difficulty, BobPhsx Hero, int Length, int NumPieces, LevelGeometry Geometry, TileSet tileset)
        {
            if (Difficulty < 0) Difficulty = 0;

            LevelSeedData data = new LevelSeedData();

            // Randomize tileset if none is specified
            if (tileset == TileSets.Random)
                StandardInit(data);
            else
            {
                StandardInit(data);
                data.SetTileSet(tileset);
            }

            data.DefaultHeroType = Hero;

            LevelSeedData.CustomDifficulty custom = DifficultyGroups.FixedPieceMod(Difficulty, data);

            data.Initialize(NormalGameData.Factory, Geometry, NumPieces, Length, custom);

            return data;
        }

        public static LevelSeedData WallLevel(float Difficulty, BobPhsx Hero, float LengthMod)
        {
            return WallLevel(Difficulty, Hero, Length(LengthMod));
        }
        static LevelSeedData WallLevel(float Difficulty, BobPhsx Hero, int Length)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);
            data.PieceLength = data.Length = Length;
            data.DefaultHeroType = Hero;

            data.SetTileSet(TileSets.Dungeon);

            data.StandardInit((p, u) =>
            {
                DifficultyGroups.FixedPieceSeed(p, Difficulty, Hero);

                p.Paths = 1;

                p.Style.ComputerWaitLengthRange = new Vector2(4, 23);

                p.Style.MyModParams = (level, piece) =>
                {
                    var Params = (NormalBlock_Parameters)piece.Style.FindParams(NormalBlock_AutoGen.Instance);
                    Wall wall = Params.SetWall(data.MyGeometry);
                    wall.Space = 20; wall.MyBufferType = Wall.BufferType.Space;
                    p.CamZoneStartAdd.X = -2000;
                    wall.StartOffset = -600;
                    wall.Speed = 17.5f;
                    wall.InitialDelay = 72;
                };
            });

            return data;
        }

        public static LevelSeedData Bungee(float Difficulty, BobPhsx Hero, float LengthMod)
        {
            return Bungee(Difficulty, Hero, Campaign.Length(LengthMod), Campaign.NumPieces());
        }
        public static LevelSeedData Bungee(float Difficulty, BobPhsx Hero, int Length, int NumPieces)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);
            data.MyGameFlags.IsTethered = true;

            data.PieceLength = data.Length = Length;
            data.NumPieces = NumPieces;
            data.DefaultHeroType = Hero;

            data.SetTileSet(TileSets.Dungeon);

            data.StandardInit((p, u) =>
            {
                DifficultyGroups.FixedPieceSeed(p, Difficulty, Hero);
            });

            return data;
        }

        public static LevelSeedData _WallLevel()
        {
            LevelSeedData data = new LevelSeedData();

            data.SetTileSet(TileSets.Dungeon);
            data.DefaultHeroType = BobPhsxNormal.Instance;
            data.MyGameType = NormalGameData.Factory;

            //data.PieceLength = 7000;
            data.PieceLength = 3000;
            data.NumPieces = 1;

            data.StandardInit((p, u) =>
            {
                u[Upgrade.FlyBlob] = 2;
                u[Upgrade.BouncyBlock] = 2;
                u[Upgrade.MovingBlock] = 5;
                u[Upgrade.Jump] = 7;

                p.Style.MyModParams = (level, piece) =>
                {
                    var Params = (NormalBlock_Parameters)piece.Style.FindParams(NormalBlock_AutoGen.Instance);
                    Wall wall = Params.SetWall(data.MyGeometry);
                    wall.Space = 20; wall.MyBufferType = Wall.BufferType.Space;
                    p.CamZoneStartAdd.X = -2000;
                    wall.StartOffset = -600;
                    wall.Speed = 17.5f;
                    wall.InitialDelay = 72;

                    //Coin_Parameters Params = (Coin_Parameters)p.Style.FindParams(Coin_AutoGen.Instance);
                    //Params.Regular_Period = 15;
                    //Params.FillType = Coin_Parameters.FillTypes.CoinGrab;
                    //Params.Grid = false;
                    //Params.DoCleanup = false;
                };
            });

            data.PostMake += level =>
            {                
                // Title
                level.MyGame.WaitThenDo_Pausable(33, () =>
                    WorldMap.AddTitle(level.MyGame, "Obstacle\n   Training", 175));
            };

            return data;
        }

        public static LevelSeedData BonusLevel_1()
        {
            return BonusLevel((p, u) => {
                u[Upgrade.FireSpinner] = 
                u[Upgrade.Spike] =
                u[Upgrade.Pinky] =
                //u[Upgrade.Laser] =
                u[Upgrade.SpikeyGuy] =
                u[Upgrade.SpikeyLine] = 7;
                u[Upgrade.FlyBlob] = 1;
            });
        }

        public static LevelSeedData BonusLevel(Action<PieceSeedData, Upgrades> CustomDiff)
        {
            LevelSeedData data = new LevelSeedData();

            //data.MyBackgroundType = BackgroundType.Dungeon;
            //data.MyBackgroundType = BackgroundType.Gray;
            data.SetTileSet(TileSets.DarkTerrace);

            data.DefaultHeroType = BobPhsxNormal.Instance;
            data.MyGameType = NormalGameData.Factory;

            data.PieceLength = 5225;
            data.NumPieces = 1;

            data.StandardInit((p, u) =>
            {
                p.ZoomType = LevelZoom.Big;
                p.ExtraBlockLength = 1000;

                u[Upgrade.Jump] = 2;

                p.Style.MyModParams = (level, piece) =>
                {
                    var CoinParams = (Coin_Parameters)piece[Coin_AutoGen.Instance];
                    CoinParams.DoStage2Fill = false;

                    //var Params = (FireSpinner_Parameters)piece[FireSpinner_AutoGen.Instance];
                    //Params.FireSpinnerMinDist = 40;
                    //Params.MinFireSpinnerDensity = Params.MaxFireSpinnerDensity = 150;
                    //Params.NumAngles = 8;
                    //p.MyGenData.gen1[DifficultyParam.GeneralMinDist] = 10;
                };

                CustomDiff(p, u);
            });

            data.NoDefaultMake = true;
            data.LavaMake = LevelSeedData.LavaMakeTypes.NeverMake;
            data.PostMake += level => LevelSeedData.AddGameObjects_BareBones(level, false);
            data.PostMake += level =>
            {
                GameData game = level.MyGame;

                // Reset sooner after death
                game.SetDeathTime(GameData.DeathTime.Fast);

                // Delay entrance
                game.MyLevel.AllowRecording = false;
                game.MyLevel.HaveTimeLimit = false;
                game.MyLevel.Bobs.ForEach(bob => bob.Immortal = true);
                game.MyLevel.PreventReset = true;
                game.PhsxStepsToDo += 3;
                level.StartDoor.SetLock(true, true, false);
                game.ToDoOnReset.Add(() => game.WaitThenDo(1, () => level.StartDoor.HideBobs()));
                game.AddGameObject(new ContinuousDiff());
            };

            return data;
        }

        public static void ToSingleSeed(LevelSeedData data)
        {
            if (data.NoDefaultMake) return;

            data.PostMake += level =>
                {
                    LevelSeedData.PostMake_Standard(level, !data.NoMusicStart, true);
                    level.MyGame.MakeScore = () => new ScoreScreen_Campaign(StatGroup.Level, level.MyGame);

                    AddCampaignGUI(level);
                };
        }

        public static void AddCampaignGUI(Level level)
        {
            AddCampaignGUI(level, null);
        }
        public static void AddCampaignGUI(Level level, Action<GUI_Level> ModTitle)
        {
            var title = new GUI_CampaignLevel();
            if (ModTitle != null) ModTitle(title);

            level.MyGame.AddGameObject(title, new GUI_CampaignScore());
        }

        public static void ToStringSeed(LevelSeedData data)
        {
            if (data.NoDefaultMake) return;

            data.PostMake += level =>
                {
                    LevelSeedData.PostMake_StringWorldStandard(level);
                };
        }

        public static void UseBobLighting_NotCampaign(Level lvl)
        {
            lvl.UseLighting = true; lvl.StickmanLighting = true; lvl.SetBobLightRadius(1);
            Tools.SongWad.SuppressNextInfoDisplay = true;
        }
        public static void UseBobLighting(Level lvl)
        {
            lvl.UseLighting = true; lvl.StickmanLighting = true; lvl.SetBobLightRadius(Campaign.Index);
            Tools.SongWad.SuppressNextInfoDisplay = true;
            //lvl.MyGame.EnterFrom(lvl.StartDoor, 80);
            //lvl.MyGame.WaitThenDo(1, lvl.MyGame.HideBobs);

            //lvl.MyGame.WaitThenDo(20, lvl.FadeBobLightSourcesIn);
        }

        public static void DiskTarget(List<Goomba> blobs, Vector2 pos, float radius) { DiskTarget(blobs.Count, blobs, pos, radius); }
        public static int DiskTarget(int n) { return DiskTarget(n, null, Vector2.Zero, 1); }
        public static int DiskTarget(int n, List<Goomba> blobs, Vector2 pos, float radius)
        {
            int N = (int)Math.Sqrt(n) + 2;
            int M = n / N;

            if (blobs == null) return N * M;

            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                {
                    if (blobs[i * M + j] != null)
                        blobs[i * M + j].Target = pos + (j * radius / M) * Tools.AngleToDir(i * 2 * Math.PI / N);
                }

            return N * M;
        }

        public static void RectangleTarget(List<Goomba> blobs, Vector2 pos, Vector2 size) { RectangleTarget(blobs.Count, blobs, pos, size); }
        public static int RectangleTarget(int n) { return RectangleTarget(n, null, Vector2.Zero, Vector2.One); }
        public static void RectangleTarget(List<Goomba> blobs, BlockBase block)
        {
            RectangleTarget(blobs.Count, blobs, block.Box.Current.Center, 2*block.Box.Current.Size);
        }
        public static int RectangleTarget(int n, List<Goomba> blobs, Vector2 pos, Vector2 size)
        {
            int N = (int)Math.Sqrt(n);
            int M = n / N;

            if (blobs == null) return N * M;

            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                    blobs[i * M + j].Target = pos + new Vector2((j * size.X / M), (i * size.Y / N)) - .5f * size;

            return N * M;
        }

        public static void MoveBlobsToTarget(List<Goomba> blobs)
        {
            foreach (Goomba blob in blobs)
                Tools.MoveTo(blob, blob.Target);
        }

        public static void GrabPrincess(GameData game, PrincessBubble princess, PrincessPos type)
        {
            GrabPrincess(game, princess, type, null, null);
        }
        public static void GrabPrincess(GameData game, PrincessBubble princess, PrincessPos type, Action<Goomba> ProcessBlob, Action OnGrabbed)
        {
            int n = DiskTarget(45);
            List<Goomba> blobs = MakeTargetBlobs(n, game);
            DiskTarget(blobs, princess.Pos, 260);
            if (ProcessBlob != null) foreach (Goomba blob in blobs) ProcessBlob(blob);

            // Determine location for blobs to start from
            Vector2 blobpos;
            switch (type) {
                case PrincessPos.CenterToRight: blobpos = new Vector2(-game.Cam.GetWidth(), 0); break;
                case PrincessPos.CenterToUp: blobpos = new Vector2(0, -game.Cam.GetHeight()); break;
                case PrincessPos.Specified: blobpos = SpecifiedBlobPos; break;
                default: blobpos = Vector2.Zero; break;
            }

            // Place blobs off screen
            foreach (Goomba blob in blobs)
            {
                Tools.MoveTo(blob, blob.Target + game.Rnd.RndDir(0) + blobpos);
                blob.Core.RemoveOnReset = true;
            }

            // Continuously track the princess
            game.CinematicToDo(() =>
            {
                // Track, aiming higher than the princess if held by a player
                Vector2 pos = princess.Pos;
                if (princess.MyBob != null && princess.Core.Held) pos.Y += 100;
                DiskTarget(blobs, pos, 260);
                ProcessBlobs(blobs, null);

                // Grab if close
                //Vector2 AvgPos = blobs.Sum(blob => blob.Pos) / blobs.Count;
                //if ((AvgPos - princess.Pos).Length() < 100)
                blobs.RemoveAll(blob => blob == null);
                float AvgDist = blobs.Sum(blob => (blob.Pos - blob.Target).Length()) / blobs.Count;
                if (AvgDist < 220)
                {
                    if (OnGrabbed != null) OnGrabbed(); OnGrabbed = null;

                    princess.Drop();
                    princess.MyState = PrincessBubble.State.BlobHeld;
                    princess.Core.RemoveOnReset = true;
                    Vector2 vel = Vector2.Zero;
                    game.CinematicToDo(() =>
                    {
                        // Move princess + blobs
                        switch (type)
                        {
                            case PrincessPos.CenterToRight:
                                if (vel.X < 23) vel.X += .135f;
                                break;
                            case PrincessPos.CenterToUp:
                                if (vel.Y < 23) vel.Y += .135f;
                                break;
                            case PrincessPos.Specified:
                                if (Math.Abs(vel.X) < SpecifiedVel.X) vel.X += Math.Sign(SpecifiedVel.X) * .135f;
                                if (Math.Abs(vel.Y) < SpecifiedVel.Y) vel.Y += Math.Sign(SpecifiedVel.Y) * .135f;
                                break;
                            default: break;
                        }

                        princess.Move(vel);
                        DiskTarget(blobs, princess.Pos, 260);

                        ProcessBlobs(blobs, blob => blob.Move(vel));

                        return false;
                    });

                    return true;
                }

                return false;
            });
        }

        static void ProcessBlobs(List<Goomba> blobs, Action<Goomba> process)
        {
            for (int i = 0; i < blobs.Count; i++)
            {
                var blob = blobs[i];

                if (blob == null) continue;

                if (blob.Core.MarkedForDeletion)
                    blobs[i] = null;
                else
                    if (process != null)
                        process(blob);
            }
        }

        public static List<Goomba> MakeTargetBlobs(int n, GameData game)
        {
            List<Goomba> Blobs = new List<Goomba>();
            for (int i = 0; i < n; i++)
            {
                Goomba blob = (Goomba)game.Recycle.GetObject(ObjectType.FlyingBlob, false);
                blob.Init(Vector2.Zero, game.MyLevel);
                blob.SetStandardTargetParams();
                Blobs.Add(blob);
                game.MyLevel.AddObject(blob);
            }

            Blobs = game.Rnd.Shuffle(Blobs);

            return Blobs;
        }

        static Vector2 LvlWithPrincess_PrincessPos;
        static int LvlWithPrincess_InitialDelay, LvlWithPrincess_DoorOpenWait, LvlWithPrincess_CanResetWait, LvlWithPrincess_CrossUntil;
        public static int LvlWithPrincess_SetBack = 360 + 172;
        static void LevelWithPrincess_SetParams()
        {
            LvlWithPrincess_InitialDelay = 45;
            LvlWithPrincess_DoorOpenWait = 360;
            LvlWithPrincess_PrincessPos = new Vector2(360, 25);

            LvlWithPrincess_DoorOpenWait = 245;
            LvlWithPrincess_CanResetWait = 90;


            LvlWithPrincess_InitialDelay = 195 - 40;
            LvlWithPrincess_DoorOpenWait = 310;

            LvlWithPrincess_CrossUntil = 195;// 240;
        }

        static Vector2 SwoopDown_vel, SwoopDown_acc, SwoopDown_pos, SwoopDown_offset, SwoopDown_initialoffset;
        static float SwoopDown_princessang;
        static void SwoopDown_Init()
        {
            SwoopDown_vel = new Vector2(17, -17.5f);
            SwoopDown_acc = new Vector2(0.1f, .3f);
            SwoopDown_pos = new Vector2(-900, 1200);

            SwoopDown_offset = new Vector2(200, 170) * .98f;
            SwoopDown_initialoffset = SwoopDown_offset * .15f;// .45f;
            SwoopDown_princessang = 142;



            
            SwoopDown_vel = new Vector2(15.5f, -20.5f);
            SwoopDown_acc = new Vector2(0.1f, .42f);
            SwoopDown_pos = new Vector2(-950, 1200);

            SwoopDown_vel = new Vector2(12f, -22.5f);
            SwoopDown_acc = new Vector2(0.1f, .35f);
            SwoopDown_pos = new Vector2(-1050, 1200);
        }
        static Vector2 Get_SwoopDown_offset(int step)
        {
           return .8f * SwoopDown_offset +
                    .2f * Tools.Periodic(SwoopDown_initialoffset, SwoopDown_offset, 100, step-80);
        }
        public static void SwoopDown(Level lvl)
        {
            SwoopDown_Init();

            // Create the princess
            var princess = new PrincessBubble(lvl.LeftMostCameraZone().Start + SwoopDown_pos);
            princess.Rotate(SwoopDown_princessang);
            lvl.AddObject(princess);

            // Create blobs
            int n = DiskTarget(45);
            List<Goomba> blobs = MakeTargetBlobs(n, lvl.MyGame);
            DiskTarget(blobs, princess.Pos + SwoopDown_initialoffset, 260);
            MoveBlobsToTarget(blobs);
            
            // Temporary blobs
            foreach (Goomba blob in blobs)
                blob.Core.RemoveOnReset = true;

            // Loop
            Vector2 vel = SwoopDown_vel;
            int step = 0;
            lvl.MyGame.CinematicToDo(() =>
            {
                step++;
                vel += SwoopDown_acc;

                princess.Move(vel);
                DiskTarget(blobs, princess.Pos + Get_SwoopDown_offset(step), 260);
                foreach (Goomba blob in blobs) blob.Move(vel);

                return false;
            });
        }



        static Vector2 LinearCarry_offset, LinearCarry_initialoffset;
        static float LinearCarry_princessang;
        static void LinearCarry_Init()
        {
            LinearCarry_offset = new Vector2(200, 170) * .8f;
            LinearCarry_initialoffset = LinearCarry_offset * .5f;// .45f;
            LinearCarry_princessang = 142;
        }
        static Vector2 Get_LinearCarry_offset(int step)
        {
            return .8f * LinearCarry_offset +
                     .2f * Tools.Periodic(LinearCarry_initialoffset, LinearCarry_offset, 100, step - 80);
        }
        public static void LinearCarry(Level lvl, PhsxData data)
        {
            LinearCarry_Init();

            // Create the princess
            var princess = new PrincessBubble(data.Position);
            princess.Rotate(LinearCarry_princessang);
            lvl.AddObject(princess);

            // Create blobs
            int n = DiskTarget(45);
            List<Goomba> blobs = MakeTargetBlobs(n, lvl.MyGame);
            DiskTarget(blobs, princess.Pos + LinearCarry_initialoffset, 260);
            MoveBlobsToTarget(blobs);

            // Temporary blobs
            foreach (Goomba blob in blobs)
                blob.Core.RemoveOnReset = true;

            // Loop
            Vector2 vel = data.Velocity;
            int count = 0;
            lvl.MyGame.CinematicToDo(() =>
            {
                count++;
                vel += data.Acceleration;

                princess.Move(vel);
                DiskTarget(blobs, princess.Pos + Get_LinearCarry_offset(count), 260);
                foreach (Goomba blob in blobs) blob.Move(vel);

                return false;
            });
        }


        public static Vector2 SpecifiedPrincessPos, SpecifiedBlobPos, SpecifiedVel;
        public enum PrincessPos { CenterToRight, CenterToUp, Specified };
        public static void LevelWithPrincess(Level lvl, bool Dramatic, PrincessPos type, bool MakePrincess)
        {
            GameData game = lvl.MyGame;
            LevelWithPrincess_SetParams();

            if (lvl.StartDoor != null)
                lvl.StartDoor.SetLock(true, true, false);

            if (MakePrincess)
            {
                lvl.PreventReset = true;
                lvl.PreventHelp = true;
            }

            game.AddToDo(() =>
            {
                // Determine the position to place the princess in
                Vector2 pos;
                switch (type) {
                    case PrincessPos.CenterToRight: pos = LvlWithPrincess_PrincessPos; break;
                    case PrincessPos.CenterToUp: pos = Vector2.Zero; break;
                    case PrincessPos.Specified: pos = SpecifiedPrincessPos; break;
                    default: pos = LvlWithPrincess_PrincessPos; break;
                }

                // Create the princess
                PrincessBubble princess;
                if (MakePrincess)
                {
                    princess = new PrincessBubble(lvl.LeftMostCameraZone().Start + pos);
                    lvl.AddObject(princess);
                }
                else
                    princess = lvl.Objects.Find(obj => obj is PrincessBubble) as PrincessBubble;

                game.HideBobs();

                // If there's a start door then enter through it
                if (lvl.StartDoor != null)
                {
                    if (Dramatic)
                    {
                        game.DramaticEntry(lvl.StartDoor, LvlWithPrincess_DoorOpenWait);
                        game.CinematicToDo(LvlWithPrincess_DoorOpenWait + LvlWithPrincess_CanResetWait,
                            () => lvl.PreventReset = lvl.PreventHelp = false);
                    }
                    else
                    {
                        game.EnterFrom(lvl.StartDoor, LvlWithPrincess_DoorOpenWait - 20);
                        game.CinematicToDo(LvlWithPrincess_DoorOpenWait + LvlWithPrincess_CanResetWait - 60,
                            () => lvl.PreventReset = lvl.PreventHelp = false);
                    }
                }

                // Grab the princess
                game.CinematicToDo(LvlWithPrincess_InitialDelay, () =>
                {
                    Campaign.GrabPrincess(game, princess, type);
                });

                // Cross blobs
                int count = 0;
                game.WaitThenAddToToDo(1, () =>
                {
                    count++;
                    if (count > 53 && count < LvlWithPrincess_CrossUntil && count % 2 == 0)
                        MakeBlob(game.MyLevel);

                    //RemoveBlobs(game.MyLevel);

                    return count >= LvlWithPrincess_CrossUntil;
                });
            });

            game.PhsxStepsToDo += 3;
        }

        public static void RemoveBlobs(Level level)
        {
            // Remove blobs that are above the screen
            level.GetObjectList(ObjectType.FlyingBlob).ForEach(blob =>
            {
                if (!level.MainCamera.OnScreen(blob.Pos, 800))
                    blob.CollectSelf();
            });
        }

        public static void MakeBlob(Level level)
        {
            // Make blob
            Goomba blob = (Goomba)level.Recycle.GetObject(ObjectType.FlyingBlob, false);
            var pos = new Vector2(level.MainCamera.BL.X - 500, level.Rnd.RndFloat(level.MainCamera.BL.Y, level.MainCamera.TR.Y));
            blob.Init(pos, level);

            blob.NeverSkip = true;

            blob.DeleteOnDeath = true;
            blob.Core.RemoveOnReset = true;
            blob.RemoveOnArrival = true;

            blob.MaxVel = 32.5f;
            blob.MaxAcc = 2;

            blob.MyPhsxType = Goomba.PhsxType.ToTarget;
            blob.Target = blob.Core.Data.Position + new Vector2(7000, 0);
            float acc = Math.Sign(blob.Pos.Y - level.MainCamera.Pos.Y);
            if (acc == 0) acc = 1;
            blob.Core.Data.Acceleration.Y = acc * .55f;

            level.AddObject(blob);
        }

        public static void CarryPrinces(LevelSeedData data)
        {
            data.PostMake += level =>
            {
                // Give each bob a princess
                foreach (Bob bob in level.Bobs)
                {
                    PrincessBubble princess = new PrincessBubble(Vector2.Zero);
                    princess.ShowWithMyBob = true;
                    level.AddObject(princess);
                    princess.PickUp(bob);
                }

                // Give computer bobs a princess too
                level.OnWatchComputer += () =>
                {
                    level.Objects.FindAll(obj => obj is PrincessBubble).ForEach(obj =>
                    {
                        obj.Core.Show = false;
                        obj.Core.Active = false;
                        obj.Core.EditorCode2 = "save";
                    });

                    foreach (Bob bob in level.Bobs)
                    {
                        PrincessBubble princess = new PrincessBubble(Vector2.Zero);
                        level.AddObject(princess);
                        princess.PickUp(bob);
                    }
                };
                level.OnEndReplay += () =>
                {
                    level.Objects.FindAll(obj => obj is PrincessBubble).ForEach(obj =>
                    {
                        if (obj.Core.EditorCode2.Length == 0)
                            obj.CollectSelf();
                        else
                        {
                            obj.Core.Show = true;
                            obj.Core.Active = true;
                        }
                    });
                };
            };
        }
    }
}