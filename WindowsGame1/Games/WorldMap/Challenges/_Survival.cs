using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class Challenge_Survival : Challenge
    {
        static readonly Challenge_Survival instance = new Challenge_Survival();
        public static Challenge_Survival Instance { get { return instance; } }

        Challenge_Survival()
        {
            ID = new Guid("0261b1e0-0f5c-42ca-a75d-1399d04ce1fc");
            Name = "Dylan Thomas";
        }

        public override void Start(int Difficulty)
        {
            base.Start(Difficulty);

            var Gui_LivesLeft = new GUI_LivesLeft(20);
            //Gui_LivesLeft.OnOutOfLives += OnOutOfLives;

            StringWorldEndurance StringWorld = new StringWorldEndurance(GetSeeds(), Gui_LivesLeft, 25);
            SetGameParent(StringWorld);
            StringWorld.Init();
            StringWorld.SetLevel(0);
        }

        protected override List<MakeSeed> MakeMakeList(int Difficulty)
        {
            List<MakeSeed> MakeList = new List<MakeSeed>();

            int diff = new int[] { 4, 12, 18, 30 }[Difficulty];
            float modlength = new float[] { 1f, 1.1f, 1.2f, 1.3f }[Difficulty];

            ////MakeList.Add(() => OnePiece(diff += 3,  (int)(26 * 30 * modlength)));
            MakeList.Add(() => OnePiece5(diff += 1, (int)(16 * 60 * modlength)));
            MakeList.Add(() => OnePiece4(diff += 1, (int)(16 * 60 * modlength)));
            MakeList.Add(() => OnePiece2(diff += 1, (int)(30 * 30 * modlength)));            
            MakeList.Add(() => OnePiece3(diff += 1, (int)(16 * 60 * modlength)));

            return MakeList;
        }

        void StandardInit(LevelSeedData data)
        {
            data.Seed = Tools.Rnd.Next();

            data.SetBackground(BackgroundType.Gray);
            data.DefaultHeroType = BobPhsxNormal.Instance;
        }

        void Fill(Level level, StyleData style)
        {
            FireballEmitter_Parameters Params = (FireballEmitter_Parameters)style.FindParams(FireballEmitter_AutoGen.Instance);
            Params.Special.SurvivalFill = true;
            Params.TunnelType = FireballEmitter_Parameters.TunnelTypes._45;
        }

        void SetUpgrades(PieceSeedData piece, int Difficulty)
        {
            piece.MyUpgrades1[Upgrade.Jump] = 4;
            piece.MyUpgrades1[Upgrade.Fireball] = Difficulty / 3;
            piece.MyUpgrades1[Upgrade.General] = Difficulty / 3 + (Difficulty % 3 > 0 ? 1 : 0);
            piece.MyUpgrades1[Upgrade.Speed] = Difficulty / 3 + (Difficulty % 3 > 1 ? 1 : 0);
        }

        LevelSeedData OnePiece(int Difficulty, int Length)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            data.Initialize(SurvivalGameData.Factory, LevelGeometry.OneScreen, 1, Length, piece =>
            {
                SetUpgrades(piece, Difficulty);

                piece.Paths = 1;

                OneScreenData Style = piece.Style as OneScreenData;
                Style.CamShift = new Vector2(-400, -400);

                piece.Style.MyModParams = (level, p) =>
                {
                    FireballEmitter_Parameters Params = (FireballEmitter_Parameters)p.Style.FindParams(FireballEmitter_AutoGen.Instance);
                    Params.Special.SurvivalFill = true;
                    Params.TunnelType = FireballEmitter_Parameters.TunnelTypes._45;
                    Params.Tunnel.Thickness = 2;
                };

                piece.StandardClose();
            });

            return data;
        }

        LevelSeedData OnePiece2(int Difficulty, int Length)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            data.Initialize(SurvivalGameData.Factory, LevelGeometry.OneScreen, 1, Length, piece =>
            {
                SetUpgrades(piece, Difficulty);

                piece.Paths = 1;

                OneScreenData Style = piece.Style as OneScreenData;
                Style.CamShift = new Vector2(0, -600);

                piece.Style.MyModParams = (level, p) =>
                {
                    FireballEmitter_Parameters Params = (FireballEmitter_Parameters)p.Style.FindParams(FireballEmitter_AutoGen.Instance);
                    Params.Special.SurvivalFill = true;
                    Params.TunnelType = FireballEmitter_Parameters.TunnelTypes._90;
                    Params.Tunnel.Thickness = 2;
                };

                piece.StandardClose();
            });

            return data;
        }

        LevelSeedData OnePiece3(int Difficulty, int Length)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            data.Initialize(SurvivalGameData.Factory, LevelGeometry.OneScreen, 1, Length, piece =>
            {
                SetUpgrades(piece, Difficulty);
                piece.MyUpgrades1[Upgrade.Speed] = Difficulty / 7;

                piece.Paths = 1;

                OneScreenData Style = piece.Style as OneScreenData;
                Style.Zoom = .55f;
                Style.CamShift = new Vector2(0, -150);

                piece.Style.MyModParams = (level, p) =>
                {
                    FireballEmitter_Parameters Params = (FireballEmitter_Parameters)p.Style.FindParams(FireballEmitter_AutoGen.Instance);
                    Params.Special.SurvivalFill = true;
                    Params.TunnelType = FireballEmitter_Parameters.TunnelTypes.Chaos;
                    Params.Tunnel.Thickness = 1;
                };

                piece.StandardClose();
            });

            return data;
        }

        LevelSeedData OnePiece4(int Difficulty, int Length)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            data.Initialize(SurvivalGameData.Factory, LevelGeometry.OneScreen, 1, Length, piece =>
            {
                SetUpgrades(piece, Difficulty);
                piece.MyUpgrades1[Upgrade.Speed] = Difficulty / 7;

                piece.Paths = 1;

                OneScreenData Style = piece.Style as OneScreenData;
                Style.Zoom = .55f;
                Style.CamShift = new Vector2(0, -150);

                piece.Style.MyModParams = (level, p) =>
                {
                    FireballEmitter_Parameters Params = (FireballEmitter_Parameters)p.Style.FindParams(FireballEmitter_AutoGen.Instance);
                    Params.Special.SurvivalFill = true;
                    Params.TunnelType = FireballEmitter_Parameters.TunnelTypes._4way;
                    Params.Tunnel.Thickness = 1;
                    Params.TunnelTimeSpace = (int)(Params.TunnelTimeSpace * 1.1f);
                };

                piece.StandardClose();
            });

            return data;
        }

        LevelSeedData OnePiece5(int Difficulty, int Length)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            data.Initialize(SurvivalGameData.Factory, LevelGeometry.OneScreen, 1, Length, piece =>
            {
                SetUpgrades(piece, Difficulty);
                piece.MyUpgrades1[Upgrade.Speed] = Difficulty / 7;

                piece.Paths = 1;

                OneScreenData Style = piece.Style as OneScreenData;
                Style.Zoom = 1f;
                Style.CamShift = new Vector2(-450, -150);

                piece.Style.MyModParams = (level, p) =>
                {
                    FireballEmitter_Parameters Params = (FireballEmitter_Parameters)p.Style.FindParams(FireballEmitter_AutoGen.Instance);
                    Params.Special.SurvivalFill = true;
                    Params.TunnelType = FireballEmitter_Parameters.TunnelTypes._0;
                    Params.Tunnel.Thickness = 1;
                };

                piece.StandardClose();
            });

            return data;
        }
    }
}