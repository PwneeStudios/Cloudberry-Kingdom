using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Obstacles;
using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom.Levels
{
    public class Coin_Parameters : AutoGen_Parameters
    {
        public bool Red = false;

        public Param MinDist, PlaceDelay;

        public enum FillTypes { None, Regular, Rush, CoinGrab };
        public FillTypes FillType = FillTypes.Regular;

        /// <summary> Whether coins should be placed on a grid lattice. </summary>
        public bool Grid;
        Vector2 GridSpacing = new Vector2(80, 80);
        public Vector2 SnapToGrid(Vector2 pos)
        {
            pos.X = (int)(pos.X / GridSpacing.X) * GridSpacing.X;
            pos.Y = (int)(pos.Y / GridSpacing.Y) * GridSpacing.Y;

            return pos;
        }

        public bool DoCleanup = true;

        /// <summary> The frame afterwhich coins can be placed. </summary>
        public int StartFrame;

        /// <summary> Used to determine if a coin should be placed (Regular style) </summary>
        public int Regular_Period, Regular_Offset, Regular_Period2, Regular_Offset2;
        /// <summary> Whehter a coin should be placed (Regular style) </summary>
        public bool Regular_ReadyToPlace(Level level, Bob bob, int Step)
        {
            return (Step % Regular_Period == Regular_Offset &&
                    Step / 50 % Regular_Period2 == Regular_Offset2 &&
                    (Step / 90) % level.Bobs.Count == level.Bobs.IndexOf(bob));
        }

        public bool CoinPlaced = false;

        public Vector2 TR_Bound_Mod, BL_Bound_Mod;

        public struct _Special
        {
        }
        public _Special Special;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            MinDist = new Param(PieceSeed);
            MinDist.SetVal(200);

            PlaceDelay = new Param(PieceSeed);
            PlaceDelay.SetVal(15);

            StartFrame = 60;

            Grid = true;

            Regular_Period = 15;
            Regular_Offset = level.Rnd.RndInt(0, Regular_Period - 1);
            Regular_Period2 = 2;
            Regular_Offset2 = level.Rnd.RndInt(0, Regular_Period2 - 1);

            BL_Bound_Mod = new Vector2(80, -120);
            TR_Bound_Mod = new Vector2(550, 320);
        }
    }

    public sealed class Coin_AutoGen : AutoGen
    {
        static readonly Coin_AutoGen instance = new Coin_AutoGen();
        public static Coin_AutoGen Instance { get { return instance; } }

        static Coin_AutoGen() { }
        Coin_AutoGen()
        {
            Do_ActiveFill_1 = true;
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            Coin_Parameters Params = new Coin_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupCoins(BL, TR);

            // Get Coin parameters
            Coin_Parameters Params = (Coin_Parameters)level.Style.FindParams(Coin_AutoGen.Instance);
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos)
        {
            return CreateAt(level, pos, true);
        }
        int offset;
        public ObjectBase CreateAt(Level level, Vector2 pos, bool NewOffset)
        {
            // Get Coin parameters
            Coin_Parameters Params = (Coin_Parameters)level.Style.FindParams(Coin_AutoGen.Instance);

            // Snap the coins to a grid
            if (Params.Grid)
                pos = Params.SnapToGrid(pos);

            // Get the new Coin
            Coin NewCoin = (Coin)level.Recycle.GetObject(ObjectType.Coin, true);

            NewCoin.Core.GenData.Used = true;
            NewCoin.Core.GenData.LimitGeneralDensity = false;

            level.AddObject(NewCoin);

            // Position
            if (Params.Red)
            {
                if (NewOffset)
                    offset = level.Rnd.RandomSnap(NewCoin.Period, 3);
                NewCoin.Offset = offset;

                NewCoin.SetType(Coin.CoinType.Red);
                Vector2 curpos = NewCoin.GetPos();
                NewCoin.Move(pos - curpos);
            }
            else
            {
                NewCoin.SetType(Coin.CoinType.Blue);
                NewCoin.Move(pos - NewCoin.Core.Data.Position);
            }

            return NewCoin;
        }

        enum BobPos { Center, High, Low, Regular };
        Vector2 CalcPos(Bob bob, Vector2 BL, Vector2 TR, BobPos pos)
        {
            Vector2 center = bob.Core.Data.Position;
            Vector2 top = new Vector2(center.X, bob.Box.TR.Y);
            Vector2 bottom = new Vector2(center.X, bob.Box.BL.Y);
            Vector2 avg = (top + bottom) / 2;
            switch (pos)
            {
                case BobPos.Center: return avg;
                case BobPos.High: return .9f * top + .1f * avg;
                case BobPos.Low: return .9f * bottom + .1f * avg;
                default:
                    return (center + bob.Box.TR) / 2;
            }
        }

        public override void ActiveFill_1(Level level, Vector2 BL, Vector2 TR)
        {
            base.ActiveFill_1(level, BL, TR);

            // Get Coin parameters
            Coin_Parameters Params = (Coin_Parameters)level.Style.FindParams(Coin_AutoGen.Instance);

            if (!Params.DoStage2Fill) return;

            int Step = level.GetPhsxStep();

            if (Step < Params.StartFrame) return;

            foreach (Bob bob in level.Bobs)
            {
                if (!level.MainCamera.OnScreen(bob.Core.Data.Position, new Vector2(-200, -240)) ||
                    level.Style.Masochistic)
                    continue;

                Vector2 pos = bob.Core.Data.Position;

                Coin coin;
                switch (Params.FillType)
                {
                    case Coin_Parameters.FillTypes.Regular:
                        if (Params.Regular_ReadyToPlace(level, bob, Step))
                            coin = (Coin)CreateAt(level, CalcPos(bob, BL, TR, BobPos.Regular));
                        break;

                    case Coin_Parameters.FillTypes.Rush:
                        if (Step % 15 == 0)
                            coin = (Coin)CreateAt(level, CalcPos(bob, BL, TR, BobPos.Regular));
                        break;

                    case Coin_Parameters.FillTypes.CoinGrab:
                        if (Step % Params.Regular_Period == 0 &&
                            (bob.Pos.X > bob.LastPlacedCoin.X + 40 || !Params.CoinPlaced))
                        {
                            Params.CoinPlaced = true;
                            bob.LastPlacedCoin = bob.Pos;

                            coin = (Coin)CreateAt(level, CalcPos(bob, BL, TR, BobPos.High));
                            coin = (Coin)CreateAt(level, CalcPos(bob, BL, TR, BobPos.Center), false);
                            coin = (Coin)CreateAt(level, CalcPos(bob, BL, TR, BobPos.Low), false);
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public partial class Level
    {
        public void CleanupCoins(Vector2 BL, Vector2 TR)
        {
            // Get Coin parameters
            Coin_Parameters Params = (Coin_Parameters)Style.FindParams(Coin_AutoGen.Instance);

            if (!Params.DoCleanup) return;

            // No coins near final doors
            if (PieceSeed.GeometryType == LevelGeometry.Right)
            {
                foreach (ObjectBase obj in Objects)
                    if (obj is Coin && obj.Pos.X > TR.X + 160)
                        Recycle.CollectObject(obj);
            }
            else if (PieceSeed.GeometryType == LevelGeometry.Up)
            {
                foreach (ObjectBase obj in Objects)
                    if (obj is Coin && obj.Pos.Y > TR.Y - 280)
                        Recycle.CollectObject(obj);
            }


            // General cleanup
            Cleanup(ObjectType.Coin, pos =>
            {
                float dist = Params.MinDist.GetVal(pos);
                return new Vector2(dist, dist);
            }, BL + Params.BL_Bound_Mod, TR + Params.TR_Bound_Mod);
        }
    }
}