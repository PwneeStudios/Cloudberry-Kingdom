using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using System.Collections.Generic;
using System.Linq;
using Drawing;

namespace CloudberryKingdom
{
    public static class DoorListExtension
    {
        public static void Add(this List<string> list, Door door)
        {
            list.Add(door.Core.EditorCode1);
        }
    }

    public class DoorData
    {
        public bool Completed;
        public int Score = 0, Coins = 0, TotalCoins = 0;

        public void Clear()
        {
            Score = Coins = TotalCoins = 0;
        }
    }

    public class DoorDict<T> : Dictionary<string, T>
    {
        public string WorldName;

        public DoorDict() { Exit = new _exit(this); }

        public struct _exit
        {
            DoorDict<T> Parent;
            string NumToCode(int num) { return Parent.WorldName + "_" + num.ToString() + "exit"; }
            public T this[int num]
            {
                get { return Parent[NumToCode(num)]; }
            }
            public _exit(DoorDict<T> Parent) { this.Parent = Parent; }
        }
        public _exit Exit;

        public string NumToCode(int num) { return WorldName + "_" + num.ToString(); }
        public T this[int num]
        {
            get { return base[NumToCode(num)]; }
            set { base[NumToCode(num)] = value; }
        }

        public T this[Door door]
        {
            get { return base[door.Core.EditorCode1]; }
            set { base[door.Core.EditorCode1] = value; }
        }

        public void Add(Door door, T item)
        {
            this.AddOrOverwrite(door.Core.EditorCode1, item);
        }
    }

    public class WorldMap : GameData
    {
        protected bool DEBUG_FirstDoorVictory = false;
        protected bool DEBUG_AllDoorsOpen = false;

        protected bool PreventDoorActions = false;

        public GameData SourceWorld;
        public bool ReleaseSourceWorld;
        public void Load(WorldMap world)
        {
            Tools.CurGameData = world;
            //world.PhsxStepsToDo = 2;
            world.SourceWorld = this;
            world.ReleaseSourceWorld = true;
        }

        //public void LoadAsSubGame(GameData game, bool AddGUI = true, string title = null, Action<GUI_Level> ModTitle = null)
        public void LoadAsSubGame(GameData game, bool AddGUI)
        {
            LoadAsSubGame(game, AddGUI, null, null);
        }
        public void LoadAsSubGame(GameData game, string title)
        {
            LoadAsSubGame(game, true, title, null);
        }
        public void LoadAsSubGame(GameData game, Action<GUI_Level> ModTitle)
        {
            LoadAsSubGame(game, true, null, ModTitle);
        }
        public void LoadAsSubGame(GameData game, string title, Action<GUI_Level> ModTitle)
        {
            LoadAsSubGame(game, true, title, ModTitle);
        }
        public void LoadAsSubGame(GameData game, bool AddGUI, string title, Action<GUI_Level> ModTitle)
        {
            if (AddGUI)
            {
                if (title == null) title = "  ";

                game.MyLevel.Name = title;
                Campaign.AddCampaignGUI(game.MyLevel, ModTitle);
            }

            Tools.CurGameData = game;
            game.ParentGame = this;
        }

        public static void AddTitle(GameData Game, string Title, int Frames)
        {
            AddTitle(Game, Title, Frames, GUI_Text.Style.Bubble);
        }
        public static void AddTitle(GameData Game, string Title, int Frames, GUI_Text.Style Style)
        {
            GUI_Text text = GUI_Text.SimpleTitle(Title, Style);
            Game.AddGameObject(text);

            CampaignMenu.HappyBlueColor(text.MyText);

            Game.WaitThenDo_Pausable(Frames, () =>
                text.Kill(true));
        }


        /// <summary>
        /// Update all doors, closing or opening as necessary.
        /// </summary>
        protected void SetDoors() { SetDoors(null); }
        /// <summary>
        /// Update all doors, except for a specified door.
        /// </summary>
        /// <param name="except">The specified door.</param>
        protected void SetDoors(Door except)
        {
            foreach (string code in Doors.Keys)
                if (Doors[code] != except)
                    Doors[code].SetLock(!Unlocked(code));
        }

        protected bool Unlocked(string code)
        {
            if (code.Contains("exit")) return false;

            // DEBUG: open every door
            if (DEBUG_AllDoorsOpen) return true;

            if (Dependence[code].Count == 0)
                return true;
            else
                return Dependence[code].Any(dependence => Data[Doors[dependence]].Completed);
        }

        protected void AddDependence(int dependent, params int[] source)
        {
            for (int i = 0; i < source.Length; i++)
                Dependence[dependent].Add(Doors[source[i]]);
        }

        protected string WorldName;
        protected DoorDict<List<string>> Dependence = new DoorDict<List<string>>();
        protected DoorDict<DoorData> Data;// = new Dictionary<Door, bool>();
        protected DoorDict<Door> Doors = new DoorDict<Door>();
        protected DoorDict<DoorIcon> Icons = new DoorDict<DoorIcon>();
        protected virtual void ProcessDoor(Door door)
        {
            // Add to dictionaries
            string code = door.Core.EditorCode1;

            Doors.AddOrOverwrite(code, door);
            if (!Data.ContainsKey(code))
                Data.AddOrOverwrite(code, new DoorData());
            Dependence.AddOrOverwrite(code, new List<string>());

            // Mod door interaction size
            door.ModDoorSize = new Vector2(.9125f, 1f);
        }

        void ProcessDoors()
        {
            foreach (ObjectBase obj in MyLevel.Objects)
            {
                Door door = obj as Door; if (null == door || door.Core.MarkedForDeletion) continue;

                //int num;
                //try { num = int.Parse(door.Core.EditorCode1); }
                //catch { num = -1; }

                ProcessDoor(door);
            }

            MakeLevelIcons();

            AssignDoors();

            SetDoors();
        }

        protected virtual void AssignDoors()
        {
        }

        protected Door FindDoor(int num)
        {
            return FindDoor(num.ToString());
        }

        protected Door FindDoor(string code)
        {
            return MyLevel.FindIObject(code) as Door;
        }

        protected int DoorDelay_ReturnFrom = 35,
                            DoorDelay_ToLevel = 20,
                            DoorDelay_WorldStart = 25,
                            DoorDelay_ExitWorld = 16,
                            DoorDelay_SetDoors = 0; //20; // How long to wait before locking/unlocking other doors when returning from a level.
        public override void EnterFrom(Door door) { EnterFrom(door, DoorDelay_WorldStart); }
        public override void EnterFrom(Door door, int Wait)
        {
            PreventDoorActions = true;

            base.EnterFrom(door, Wait);

            // Allow doors to work again
            WaitThenDo(45, () => PreventDoorActions = false);
        }

        public void EnterFromAndClose(Door door) { EnterFromAndClose(door, DoorDelay_WorldStart); }
        public void EnterFromAndClose(Door door, int Wait)
        {
            EnterFrom(door, Wait);
            WaitThenDo(Wait + 40, () => door.SetLock(true, true, true, false));
        }
        
        public DoorAction MakeDoorAction(Door door, Action action)
        {
            return MakeDoorAction(door, action, DoorDelay_ExitWorld);
        }
        public DoorAction MakeDoorAction(Door door, Action action, int Delay)
        {
            door.OnOpen = _door =>
            {
                // Only allow one action at a time
                if (PreventDoorActions) return;
                PreventDoorActions = true;

                // Close the door
                this.AddToDo(() =>
                    door.SetLock(true, false, true));

                // Do the action
                this.WaitThenDo(Delay, action);
            };
            return door.OnOpen;
        }

        public DoorAction MakeDoorAction_Level(Door door, LevelSeedData seed) { return MakeDoorAction_Level(door, seed, door); }
        public DoorAction MakeDoorAction_Level(Door door, LevelSeedData seed, Door target)
        {
            Campaign.ToSingleSeed(seed);
            return MakeDoorAction_Level(door, () =>
                {
                    GameData game = GameData.StartLevel(seed);
                    game.ParentGame = this;
                }, target);
        }
        public DoorAction MakeDoorAction_Level(Door door, Action action) { return MakeDoorAction_Level(door, action, door); }
        public DoorAction MakeDoorAction_Level(Door door, Action action, Door target)
        {
            MakeDoorAction(door, () =>
                {
                    Campaign.CurData = Campaign.Data[door];
                    Campaign.CurData.Clear();

                    OnReturnTo_OneOff += () =>
                        {
                            door.KillNote();
                            
                            Door enter;
                            if (Tools.CurrentAftermath.Success)
                            {
                                Data[door].Completed = true;
                                enter = target;
                            }
                            else
                                enter = door;

                            EnterFrom(enter, DoorDelay_ReturnFrom);
                            if (enter.Core.EditorCode1.Contains("exit"))
                                WaitThenDo(DoorDelay_ReturnFrom + 40, () => enter.SetLock(true));

                            if (DoorDelay_SetDoors == 0)
                                SetDoors(enter);
                            else
                                WaitThenDo(DoorDelay_SetDoors, SetDoors);
                        };

                    action();

                    /*
                    GameData game = GameData.StartLevel(seed);
                    game.ParentGame = this;
                    game.EndGame = replay =>
                        {
                            game.StandardFinish(replay);

                            OnReturnTo_OneOff += () =>
                                {
                                    door.KillNote();
                                    
                                    EnterFrom(target, DoorDelay_ReturnFrom);
                                    
                                    if (Tools.CurrentAftermath.Success)
                                        Completed[door] = true;

                                    WaitThenDo(20, SetDoors);
                                };
                        };*/
                }, DoorDelay_ToLevel);
            return door.OnOpen;
        }

        public WorldMap()
        {
            Init();
        }

        public WorldMap(bool CallConstructor)
        {
            DEBUG_AllDoorsOpen = true;
            //DEBUG_FirstDoorVictory = true;
            if (CallConstructor) Init();
        }

        public override void SetCreatedBobParameters(Bob bob)
        {
            base.SetCreatedBobParameters(bob);

            bob.Immortal = false;
            bob.ScreenWrap = false;
        }

        protected void MakeCenteredCamZone(float Zoom)
        {
            MakeCenteredCamZone(Zoom, "Center", "Center");
        }

        protected void MakeCenteredCamZone(float Zoom, string Name)
        {
            MakeCenteredCamZone(Zoom, Name, Name);
        }
        protected void MakeCenteredCamZone(float Zoom, string Name, string Name2)
        {
            Vector2 Center = Vector2.Zero;
            if (Name != null) 
                Center = MyLevel.FindBlock(Name).Pos;

            Vector2 Center2 = Vector2.Zero;
            if (Name2 != null)
                Center2 = MyLevel.FindBlock(Name2).Pos;
            MyLevel.MainCamera.Pos = MyLevel.MainCamera.Data.Position = MyLevel.MainCamera.Target = MyLevel.CurPiece.CamStartPos
                = Center;

            CameraZone CamZone = (CameraZone)Recycle.GetObject(ObjectType.CameraZone, false);
            CamZone.Init(Vector2.Zero, new Vector2(100000, 100000));
            CamZone.Start = Center;
            CamZone.End = Center2;
            CamZone.FreeY = false;
            CamZone.Zoom *= Zoom;
            MyLevel.AddObject(CamZone);
            MyLevel.MainCamera.MyZone = CamZone;

            MyLevel.MainCamera.Zoom = new Vector2(CamZone.Zoom * .001f);
        }

        protected void MakeBackground(BackgroundTemplate type)
        {
            MyLevel.MyBackground = Background.Get(type);
            MyLevel.MyBackground.Init(MyLevel);
            MyLevel.BL.X -= 12000;
            MyLevel.TR.X += 12000;
            MyLevel.MyBackground.AddSpan(MyLevel.BL, MyLevel.TR);
        }

        public Level MakeLevel(string FileName)
        {
            Level level = new Level();
            level.MainCamera = new Camera();

            level.CurPiece = level.StartNewPiece(0, null, 4);
            level.CurPiece.StartData[0].Position = new Vector2(-26000, 600);
            level.MainCamera.BLCamBound = new Vector2(-100000, 0);
            level.MainCamera.TRCamBound = new Vector2(100000, 0);
            level.MainCamera.Update();

            // No time limit
            level.TimeLimit = -1;

            level.MyGame = this;
            if (FileName != null)
                level.Load(FileName);

            return level;
        }

        protected bool DoDoorProcessing = true;
        public void Init(string FileName) { Init(FileName, true); }
        public void Init(string FileName, bool SetAsWorldMap)
        {
            base.Init();

            AllowQuickJoin = true;

            Dependence.WorldName = WorldName;
            Doors.WorldName = WorldName;
            Data.WorldName = WorldName;

            Tools.CurLevel = MyLevel = MakeLevel(FileName);
            Tools.CurGameData = MyLevel.MyGame = this;
            Tools.CurGameType = WorldMap.Factory;
            if (SetAsWorldMap)
                Tools.WorldMap = this;
            
            ParentGame = Tools.TitleGame;

            PhsxStepsToDo += 2;

            // No time limit
            MyLevel.HaveTimeLimit = false;

            // Doors
            MyLevel.Objects.FindAll(obj => obj is Door).ForEach(obj =>
                {
                    Door dummydoor = obj as Door;
                    //Vector2 bottom = dummydoor.GetBottom();
                    Vector2 bottom = dummydoor.Pos;

                    // Add door
                    Door door = (Door)Recycle.GetObject(ObjectType.Door, false);
                    door.Clone(dummydoor);
                    door.StampAsUsed(0);

                    door.SetDoorType(Door.Types.Grass);

                    MyLevel.AddObject(door);

                    // Place the door above the block
                    //door.PlaceAt(bottom);
                    door.Pos = bottom;

                    // Remove dummy
                    dummydoor.Core.Active = false;
                    dummydoor.CollectSelf();
                });

            MyLevel.CleanAllObjectLists();

            // Count coins
            MyLevel.CountCoinsAndBlobs();
            PlayerManager.ExistingPlayers.ForEach(player =>
            {
                MyLevel.CoinsCountInStats = true;
                player.Stats.TotalCoins += MyLevel.NumCoins;
                player.Stats.TotalBlobs += MyLevel.NumBlobs;
            });

            // Doors
            if (DoDoorProcessing)
                ProcessDoors();

            // Spawn point
            MakeSpawnPoint();
        }

        protected virtual void SetHeroType(BobPhsx type)
        {
            DefaultHeroType = MyLevel.DefaultHeroType = type;
        }

        protected virtual void MakeMvp()
        {
            MvpOnly = true;

            MyLevel.Bobs.Clear();
            MyLevel.PlayMode = 0;

            MakeBobs(MyLevel);
            ReviveAll();

            // Position Bobs
            MvpBob.Move(MyLevel.CurPiece.StartData[0].Position - MyLevel.Bobs[0].Core.Data.Position);
        }

        protected virtual void MakePlayers()
        {
            MyLevel.Bobs.Clear();
            MyLevel.PlayMode = 0;

            MakeBobs(MyLevel);
            ReviveAll();

            // Position Bobs
            for (int i = 0; i < 4; i++)
                if (PlayerManager.Get(i).Exists)
                {
                    MyLevel.Bobs[i].Move(MyLevel.CurPiece.StartData[i].Position - MyLevel.Bobs[i].Core.Data.Position);
                }
        }

        public void MoveBobsToSpawnPoint()
        {
            foreach (Bob bob in MyLevel.Bobs)
                Tools.MoveTo(bob, MyLevel.CurPiece.StartData[bob.MyPieceIndex].Position);
        }

        public void SetSpawnPoint(Vector2 pos, Vector2 offset)
        {
            for (int i = 0; i < 4; i++)
                MyLevel.CurPiece.StartData[i].Position = pos + i * offset;
        }

        protected virtual void MakeSpawnPoint()
        {
            BlockBase block = MyLevel.FindBlock("SpawnPoint");
            if (block != null)
            {
                SetSpawnPoint(block.Pos, new Vector2(100, 0));
                
                return;
            }

            ObjectBase obj = MyLevel.FindIObject("Enter");
            if (obj != null)
            {
                SetSpawnPoint(obj.Pos, Vector2.Zero);

                return;
            }
        }

        protected virtual void MakeLevelIcons()
        {
            EzTexture levelicon = Tools.TextureWad.FindByName("levelicon");
            foreach (BlockBase block in MyLevel.Blocks)
            {
                Doodad doodad = block as Doodad;
                if (null != doodad &&
                    doodad.Core.EditorCode3.CompareTo("icon") == 0)
                    //doodad.MyQuad.Quad.MyTexture == levelicon)
                {
                    int num = int.Parse(doodad.Core.EditorCode1);

                    DoorIcon icon = new DoorIcon(num);
                    icon.Core.DrawLayer = doodad.Core.DrawLayer;
                    //icon.Pos.RelVal = doodad.Pos;
                    icon.Pos.SetCenter(doodad);
                    icon.Pos.RelVal = Vector2.Zero;
                    AddGameObject(icon);

                    doodad.Core.Show = false;

                    Icons.Add(Doors[num], icon);
                    Icons[num] = icon;
                }
            }
        }

        public override void ReturnTo(int code)
        {
            ClearToDoOnReturnTo = false;
            base.ReturnTo(code);

            foreach (Bob bob in MyLevel.Bobs)
            {
                PlayerManager.ReviveBob(bob);
                /*bob.ScreenWrap = false;
                for (int i = 0; i < 20; i++)
                {
                    if (bob.MyPlayerIndex == PlayerIndex.Two)
                        Tools.Write("");
                    Tools.MoveTo(bob, Cam.Pos);
                    //bob.Core.Data.Position = Cam.Pos;
                    bob.Core.Data.Velocity = Vector2.Zero;
                    bob.Core.Data.Acceleration = Vector2.Zero;

                    bob.Core.Show = true;
                    bob.PhsxStep();
                    bob.AnimAndUpdate();
                    bob.PhsxStep2();
                }
                bob.Core.Show = false;*/
            }

            Tools.CurGameType = WorldMap.Factory;
            Tools.CurGameData = this;
            Tools.WorldMap = this;
            Tools.CurLevel = MyLevel;
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            if (ReleaseSourceWorld && SourceWorld != null)
            {
                if (!SourceWorld.Released)
                    SourceWorld.Release();
                SourceWorld = null;
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }

        public override void BobDie(Level level, Bob bob)
        {
            base.BobDie(level, bob);
        }

        public override void BobDoneDying(Level level, Bob bob)
        {
            base.BobDoneDying(level, bob);
        }

        public override void Release()
        {
            base.Release();

            if (Released) return;

            SourceWorld = null;
            Doors = null;
        }



        public void CampaignEnd()
        {
            EndGame = StandardFinish;
            MyLevel.PreventReset = true;
            RemoveGameObjects(GameObject.Tag.RemoveOnLevelFinish);
            AllowQuickJoin = false;

            // Register victory
            LockManager.RegisterCampaignBeaten(Campaign.Index);

            // Calc scores
            Campaign.CalcScore();

            // Awardments
            Awardments.CheckForAward_BeatCampaign(Campaign.Index);
            Awardments.CheckForAward_FastCampaign(Campaign.Index);
            Awardments.CheckForAward_EbenezerAbusiveCastle(Campaign.Index);
            Awardments.CheckForAward_PerfectEasyCastle(Campaign.Index);
            Awardments.CheckForAward_NoDeathNormalCastle(Campaign.Index);
            Awardments.CheckForAward_PartiallyInvisible(Campaign.Index);
            Awardments.CheckForAward_TotallyInvisible(Campaign.Index);

            WaitThenDo(Awardments.AwardDelay(), () =>
            {
                // Score screen
                CampaignList scores = SaveGroup.CampaignScores[Campaign.Index];
                ScoreScreen_CampaignEnd score = new ScoreScreen_CampaignEnd(StatGroup.Campaign, this, scores);
                AddGameObject(score);
            });
        }

        protected void MakeWorldTitle(string str)
        {
            MakeWorldTitle(str, 50, 255, .011f, Vector2.Zero, false);
        }
        protected void MakeWorldTitle(string str, int WaitToFadeIn, int WaitToFadeOut, float FadeSpeed, Vector2 Shift, bool Bubble)
        {
            GUI_Text.Style style = Bubble ? GUI_Text.Style.Bubble : GUI_Text.Style.None;
            var title = GUI_Text.SimpleTitle(str, style);
            CampaignMenu.HappyBlueColor(title.MyText);
            title.NoPosMod = true;
            title.MyPile.FancyPos.Playing = false;
            title.MyPile.Alpha = 0;
            title.MyPile.FancyPos.RelVal += Shift;
            WaitThenDo(WaitToFadeIn, () => title.MyPile.AlphaVel = FadeSpeed);
            WaitThenDo(WaitToFadeOut, () => title.MyPile.AlphaVel = -FadeSpeed);
            AddGameObject(title);
        }
    }
}