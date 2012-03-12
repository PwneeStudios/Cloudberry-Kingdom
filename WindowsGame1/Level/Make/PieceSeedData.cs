using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public delegate void ModifyPieceSeedData(PieceSeedData Piece);
    public delegate void ModifyMakeData(ref Level.MakeData makeData);

    public enum LevelGeometry { Right, Up, OneScreen, Down, Big }
    public enum LevelZoom { Normal, Big }

    public class PieceSeedData
    {
        /// <summary>
        /// Uses the upgrade data inMyUpgrades1 to calculate the level gen data.
        /// </summary>
        public void CalculateSimple()
        {
            MyUpgrades1.CalcGenData(MyGenData.gen1, Style);

            RndDifficulty.ZeroUpgrades(MyUpgrades2);
            MyUpgrades1.UpgradeLevels.CopyTo(MyUpgrades2.UpgradeLevels, 0);
            MyUpgrades2.CalcGenData(MyGenData.gen2, Style);
        }

        public Action<Level> PreStage1, PreStage2;

        public AutoGen_Parameters this[AutoGen gen]
        {
            get { return Style.FindParams(gen); }
        }

        /// <summary>
        /// Type of level to be made, relating to shape and direction. Different from the GameType.
        /// </summary>
        public LevelGeometry GeometryType = LevelGeometry.Right;

        /// <summary>
        /// Type of level to be made, relating to the camera zoom.
        /// </summary>
        public LevelZoom ZoomType = LevelZoom.Normal;

        public float ExtraBlockLength = 0;

        public StyleData Style;

        public RichLevelGenData MyGenData;
        public Upgrades MyUpgrades1, MyUpgrades2;

        public Upgrades u { get { return MyUpgrades1; } }

        public Vector2 Start, End;
        public Vector2 CamZoneStartAdd, CamZoneEndAdd;

        public int Paths = -1;
        public bool LockNumOfPaths = false;

        public Level.LadderType Ladder;

        public PieceSeedData PieceSeed; // Used if this is a platform used for making new platforms

        public bool CheckpointsAtStart, InitialCheckpointsHere;

        public int MyPieceIndex = -1;

        public void Release()
        {
            if (Style != null) Style.Release(); Style = null;
            PieceSeed = null;
            MyGenData = null;
            MyLevelSeed = null;
        }

        public void CopyFrom(PieceSeedData piece)
        {
            //Style = piece.Style.Clone();
            MyUpgrades1.CopyFrom(piece.MyUpgrades1);
            MyUpgrades1.CalcGenData(MyGenData.gen1, Style);
            MyUpgrades2.CopyFrom(piece.MyUpgrades2);
            MyUpgrades2.CalcGenData(MyGenData.gen2, Style);
        }

        public void CalcBounds()
        {
        }

        public void StandardClose()
        {
            MyUpgrades1.CalcGenData(MyGenData.gen1, Style);

            RndDifficulty.ZeroUpgrades(MyUpgrades2);
            MyUpgrades1.UpgradeLevels.CopyTo(MyUpgrades2.UpgradeLevels, 0);
            MyUpgrades2.CalcGenData(MyGenData.gen2, Style);

            Style.MyInitialPlatsType = StyleData.InitialPlatsType.Door;
            Style.MyFinalPlatsType = StyleData.FinalPlatsType.Door;
        }

        LevelSeedData MyLevelSeed;
        public Rand Rnd { get { return MyLevelSeed.Rnd; } }

        public PieceSeedData(LevelSeedData LevelSeed)
        {
            MyLevelSeed = LevelSeed;
            Init(LevelGeometry.Right);
        }

        public PieceSeedData(int Index, LevelGeometry Type, LevelSeedData LevelSeed)
        {
            MyLevelSeed = LevelSeed;
            MyPieceIndex = Index;
            Init(Type);
        }

        void Init(LevelGeometry Type)
        {
            GeometryType = Type;

            if (MyLevelSeed != null)
            switch (GeometryType)
            {
                case LevelGeometry.Right: Style = new SingleData(Rnd); break;
                case LevelGeometry.Down: Style = new DownData(Rnd); break;
                case LevelGeometry.Up: Style = new UpData(Rnd); break;
                case LevelGeometry.OneScreen: Style = new OneScreenData(Rnd); break;
                case LevelGeometry.Big: Style = new BigData(Rnd); break;
            }
            
            MyGenData = new RichLevelGenData();
            MyGenData.gen1 = new LevelGenData();
            MyGenData.gen2 = new LevelGenData();

            MyUpgrades1 = new Upgrades();
            MyUpgrades2 = new Upgrades();

            //MyGenData.gen3 = new LevelGenData();
            //MyUpgrades1.CalcGenData(MyGenData.gen3);

            Ladder = Level.LadderType.None;
        }


        public void NoBlobs()
        {
            MyUpgrades1[Upgrade.FallingBlock] =
                Math.Max(MyUpgrades1[Upgrade.FallingBlock],
                         MyUpgrades1[Upgrade.FlyBlob]);
            MyUpgrades1[Upgrade.FlyBlob] = 0;

            MyUpgrades2[Upgrade.MovingBlock] =
                Math.Max(MyUpgrades2[Upgrade.MovingBlock],
                         MyUpgrades2[Upgrade.FlyBlob]);
            MyUpgrades2[Upgrade.FlyBlob] = 0;
        }
    }
}