using System;
using System.Collections.Generic;
using System.Linq;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class Challenge_Place : Challenge
    {
        static readonly Challenge_Place instance = new Challenge_Place();
        public static Challenge_Place Instance { get { return instance; } }

        Challenge_Place()
        {
            ID = new Guid("dd08a9cd-29d3-4c48-bded-f7bf10bbc838");
            Name = "Hard Hat";
        }

        public override void Start(int Difficulty)
        {
            base.Start(Difficulty);

            StringWorldEndurance StringWorld = new StringWorldEndurance(GetSeeds(), null, 25);
            SetGameParent(StringWorld);
            StringWorld.Init();
            StringWorld.SetLevel(0);
        }

        protected override List<MakeSeed> MakeMakeList(int Difficulty)
        {
            List<MakeSeed> MakeList = new List<MakeSeed>();

            int diff = new int[] { 8, 14, 20, 30 }[Difficulty];

            MakeList.Add(() => Standard(diff - 1, 5500, LevelGeometry.Right, PlaceTypes.FallingBlock, Upgrade.Laser));
            MakeList.Add(() => Standard(diff + 2, 5500, LevelGeometry.Right, PlaceTypes.FlyingBlob, Upgrade.Fireball));
            MakeList.Add(() => Standard(diff + 4, 5500, LevelGeometry.Right, PlaceTypes.BouncyBlock, Upgrade.SpikeyGuy));
            MakeList.Add(() => Standard(diff + 2, 5500, LevelGeometry.Right, PlaceTypes.MovingBlock, Upgrade.Laser, Upgrade.SpikeyGuy));
            MakeList.Add(() => Standard(diff - 1, 5500, LevelGeometry.Right, PlaceTypes.FlyingBlob, Upgrade.Laser, Upgrade.SpikeyGuy, Upgrade.Fireball));

            return MakeList;
        }

        public static void StandardInit(LevelSeedData data, PlaceTypes placetype)
        {
            data.Seed = Tools.GlobalRnd.Rnd.Next();

            data.SetBackground(BackgroundType.Castle);
            data.DefaultHeroType = BobPhsxNormal.Instance;
            data.PlaceObjectType = placetype;
        }

        LevelSeedData Standard(int Difficulty, int Length, LevelGeometry Geometry, PlaceTypes placetype, params Upgrade[] upgrades)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data, placetype);

            data.Initialize(PlaceGameData.Factory, Geometry, 1, Length, piece =>
            {
                piece.MyUpgrades1[Upgrade.Jump] = 5;
                foreach (Upgrade upgrade in upgrades)
                    piece.MyUpgrades1[upgrade] = Difficulty / 3;
                if (upgrades.Contains(Upgrade.Fireball))
                    piece.MyUpgrades1[Upgrade.Fireball] = 3 * piece.MyUpgrades1[Upgrade.Fireball] / 2;
                piece.MyUpgrades1[Upgrade.Ceiling] = 2;
                piece.MyUpgrades1[Upgrade.General] = Difficulty / 2 + (Difficulty % 3 > 0 ? 1 : 0);
                piece.MyUpgrades1[Upgrade.Speed] = Difficulty / 4 + (Difficulty % 3 > 1 ? 1 : 0);

                piece.Paths = 1;

                piece.Style.MyModParams = (level, p) =>
                {
                    if (upgrades.Contains(Upgrade.Fireball))
                    {
                        FireballEmitter_Parameters Params = (FireballEmitter_Parameters)p.Style.FindParams(FireballEmitter_AutoGen.Instance);
                        Params.Special.BorderFill = true;
                        Params.NumAngles = -1;
                        Params.NumOffsets = 3;
                        //Params.Arc = true;

                        Params.DoStage2Fill = false;
                    }

                    //Ceiling_Parameters cParams = (Ceiling_Parameters)p.Style.FindParams(Ceiling_AutoGen.Instance);
                    //cParams.SetCementCeiling();

                    //Floater_Parameters Params = (Floater_Parameters)p.Style.FindParams(Floater_AutoGen.Instance);
                    //Params.Special.Hallway = true;
                    //Params.Tunnel.TunnelFloor = true;
                    //Params.DoStage2Fill = false;
                };

                piece.StandardClose();
            });

            return data;
        }

        // Random levels
        public static List<Upgrade> FreeUpgrades, BoundUpgrades;
        public static void InitLists()
        {
            if (FreeUpgrades == null)
            {
                FreeUpgrades = new List<Upgrade>(new Upgrade[] { Upgrade.Fireball, Upgrade.SpikeyGuy, Upgrade.Laser });
                BoundUpgrades = new List<Upgrade>(new Upgrade[] { Upgrade.FireSpinner, Upgrade.Pinky, Upgrade.Spike });
            }
        }

        static List<Upgrade> Picks1, Picks2;
        static void SetPieceSeed(PieceSeedData piece, TileSetInfo tileset, int Jump, int Dodge, int Speed, int JumpComplexity, int DodgeComplexity)
        {
            InitLists();

            // Pick upgrades            
            if (piece.MyPieceIndex == 0)
            {
                Picks1 = piece.Rnd.Choose(tileset.JumpUpgrades, JumpComplexity);
                Picks2 = piece.Rnd.Choose(FreeUpgrades, DodgeComplexity);
                Picks2.AddRange(piece.Rnd.Choose(BoundUpgrades, DodgeComplexity));
            }

            foreach (Upgrade upgrade in Picks1)
                piece.MyUpgrades1[upgrade] = Jump;


            int DodgeLevel = (int)(Dodge);
            if (DodgeComplexity == 2) DodgeLevel = DodgeLevel - 1;
            else if (DodgeComplexity == 3) DodgeLevel = DodgeLevel - 2;
            else if (DodgeComplexity == 4) DodgeLevel = DodgeLevel - 3;

            if (DodgeLevel < 1) DodgeLevel = 1;

            foreach (Upgrade upgrade in Picks2)
                piece.MyUpgrades1[upgrade] = DodgeLevel;

            piece.MyUpgrades1[Upgrade.Jump] = Jump;
            piece.MyUpgrades1[Upgrade.Ceiling] = (Jump + Dodge) / 2;
            piece.MyUpgrades1[Upgrade.General] = Dodge;
            piece.MyUpgrades1[Upgrade.Speed] = Speed;

            piece.Style.MyModParams = (level, p) =>
            {
            };

            piece.StandardClose();
        }

        
        public static LevelSeedData StandardLevel(int Difficulty, LevelGeometry Geometry)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data, Tools.GlobalRnd.Choose(PlaceGameData.EditorAllowedPlaceTypes));

            LevelSeedData.CustomDifficulty custom = StandardPieceMod(Difficulty, data);

            data.Initialize(NormalGameData.Factory, Geometry, 2, 4500, custom);

            return data;
        }

        public static LevelSeedData.CustomDifficulty StandardPieceMod(int Difficulty, LevelSeedData LevelSeed)
        {
            int Jump, Dodge, Speed, JumpComplexity, DodgeComplexity;

            switch (LevelSeed.Rnd.RndInt(0, 3))
            {
                default:
                    Jump = Tools.Restrict(1, 9, Difficulty - 2);
                    Dodge = Difficulty + 2;
                    Speed = (int)Tools.DifficultyLerp159(1, 3, 8, Difficulty);
                    JumpComplexity = 2;
                    DodgeComplexity = LevelSeed.Rnd.RndInt(3, 4);
                    break;
            }

            return
                piece => SetPieceSeed(piece, LevelSeed.MyTileSet, Jump, Dodge, Speed, JumpComplexity, DodgeComplexity);
        }
    }
}