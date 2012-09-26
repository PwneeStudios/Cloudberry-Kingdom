using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Levels
{
    public class Floater_Parameters : AutoGen_Parameters
    {
        public Param FloaterMinDist, FloaterSparsity, FloaterPeriod, FloaterMaxAngle, FloaterPlaceDelay;

        public TunnelFill Tunnel;

        public struct _Special
        {
            /// <summary>
            /// A special fill type, creating a hallway of floaters.
            /// </summary>
            public bool Hallway;
        }
        public _Special Special;

        public Vector2 HallwaySpacing = Unset.Vector;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            Tunnel = new TunnelFill();
            SetVal(ref HallwaySpacing, u =>
                new Vector2(
                    300 - 5 * u[Upgrade.SpikeyGuy],
                    DifficultyHelper.Interp19(220, 80, u[Upgrade.SpikeyGuy])));

            if (PieceSeed.MyUpgrades1[Upgrade.SpikeyGuy] > 0 ||
                PieceSeed.MyUpgrades2[Upgrade.SpikeyGuy] > 0)
                DoStage2Fill = true;
            else
                DoStage2Fill = false;

            // General difficulty
            BobWidthLevel = new Param(PieceSeed, u =>
            {
                return u[Upgrade.SpikeyGuy];
            });

            FloaterMinDist = new Param(PieceSeed, u =>
            {
                return Math.Max(80, 800 - 63.5f * u[Upgrade.SpikeyGuy]);
            });

            FloaterPeriod = new Param(PieceSeed, u =>
            {
                return Math.Max(84, 274 - 10 * u[Upgrade.Speed]);
            });

            FloaterPlaceDelay = new Param(PieceSeed, u =>
            {
                return .705f * Math.Max(4.75f, 9 - u[Upgrade.SpikeyGuy] / 2f);
            });

            FloaterMaxAngle = new Param(PieceSeed, u =>
            {
                return Math.Min(750, 30 + 64 * u[Upgrade.SpikeyGuy]);
            });

            FloaterSparsity = new Param(PieceSeed, u =>
            {
                float sparsity = Math.Max(10, 10 - 0 * u[Upgrade.SpikeyGuy]);
                if (u[Upgrade.SpikeyGuy] <= 0)
                    sparsity = -1;
                return sparsity;
            });
        }
    }

    public sealed class Floater_AutoGen : AutoGen
    {
        static readonly Floater_AutoGen instance = new Floater_AutoGen();
        public static Floater_AutoGen Instance { get { return instance; } }

        static Floater_AutoGen() { }
        Floater_AutoGen()
        {
            Do_ActiveFill_1 = true;
            Do_PreFill_2 = true;
            //Generators.AddGenerator(this);
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            Floater_Parameters Params = new Floater_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupFloaters(BL, TR);

            // Get Floater parameters
            Floater_Parameters Params = (Floater_Parameters)level.Style.FindParams(Floater_AutoGen.Instance);

            if (Params.Special.Hallway)
                Params.Tunnel.CleanupTunnel(level);
        }

        void Hallway(Level level, Vector2 BL, Vector2 TR)
        {
            Floater_Parameters Params = (Floater_Parameters)level.Style.FindParams(Floater_AutoGen.Instance);

            TR.X += 700;

            Vector2 Spacing = Params.HallwaySpacing;
            float Width = TR.X - BL.X; int N = (int)(Width / Spacing.X);
            float Height = TR.Y - BL.Y; int M = (int)(Height / Spacing.Y);

            Params.Tunnel.Init(N, M);

            for (int i = 0; i < N; i++)
            for (int j = 0; j < M; j++)
            {
                Vector2 pos = Spacing * new Vector2(i, j) + BL;

                Floater floater = (Floater)CreateAt(level, pos);
                floater.Offset = 0;
                floater.Core.GenData.KeepIfUnused = true;
                floater.Core.GenData.EnforceBounds = false;

                Params.Tunnel.TunnelGUIDs[i, j] = floater.Core.MyGuid;
            }
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos)
        {
            // Get Floater parameters
            Floater_Parameters Params = (Floater_Parameters)level.Style.FindParams(Floater_AutoGen.Instance);

            // Get the new floater
            Floater NewFloater = (Floater)level.Recycle.GetObject(ObjectType.Floater, true);
            NewFloater.Init(pos, level);

            if (level.PieceSeed.GeometryType == LevelGeometry.Right)
                NewFloater.PivotPoint.Y = level.MainCamera.TR.Y + 160;
            else
            {
                NewFloater.PivotPoint.X = level.MainCamera.BL.X - 160;
                NewFloater.AddAngle = CoreMath.Radians(90);
                NewFloater.PivotLocationType = Floater.PivotLocationTypes.LeftRight;
            }

            NewFloater.Period = (int)Params.FloaterPeriod.GetVal(pos);
            NewFloater.Offset = level.Rnd.Rnd.Next(0, NewFloater.Period);
            NewFloater.MaxAngle = (int)Params.FloaterMaxAngle.GetVal(pos);
            NewFloater.MaxAngle *= .001f;
            NewFloater.CalculateLength();


            // Discrete period offsets
            int NumOffsets = 8;
            int Period = (int)(Params.FloaterPeriod.GetVal(pos) / NumOffsets) * NumOffsets;
            NewFloater.Period = Period;
            NewFloater.Offset = level.Rnd.Rnd.Next(0, NumOffsets) * Period / NumOffsets;

            NewFloater.Core.GenData.RemoveIfUnused = false;

            level.AddObject(NewFloater);

            return NewFloater;
        }

        Vector2 CalcPos(Bob bob, Vector2 BL, Vector2 TR, Rand Rnd)
        {
            Vector2 pos = bob.Core.Data.Position + new Vector2(Rnd.RndFloat(-600, 600), Rnd.RndFloat(-300, 400));
            pos.Y = Math.Min(pos.Y, TR.Y - 400);
            pos.Y = Math.Max(pos.Y, BL.Y + 220);
            return pos;
            pos.X = Math.Max(BL.X + 380, Math.Min(pos.X, TR.X - 350));
            if ((bob.MyPhsx.OnGround || bob.Core.Data.Velocity.Y < 0))
                pos.Y = bob.Core.Data.Position.Y + 200;
            pos.Y = Math.Max(bob.Core.MyLevel.MainCamera.BL.Y + 270, Math.Min(pos.Y, bob.Core.MyLevel.MainCamera.TR.Y - 550));
            if (Math.Abs(bob.Core.Data.Position.Y - pos.Y) < 100)
                pos.X += .4f * (pos.X - bob.Core.Data.Position.X);

            return pos;
        }

        public override void ActiveFill_1(Level level, Vector2 BL, Vector2 TR)
        {
            base.ActiveFill_1(level, BL, TR);

            // Get Floater parameters
            Floater_Parameters Params = (Floater_Parameters)level.Style.FindParams(Floater_AutoGen.Instance);

            if (!Params.DoStage2Fill) return;

            int Step = level.GetPhsxStep();

            foreach (Bob bob in level.Bobs)
            {
                if (!level.MainCamera.OnScreen(bob.Core.Data.Position, new Vector2(-200, -240)))
                    continue;

                Vector2 pos = bob.Core.Data.Position;
                int Delay = (int)Params.FloaterPlaceDelay.GetVal(pos);
                if (Step > 90 && Step % Delay == 0)
                {
                    Floater floater = (Floater)CreateAt(level, CalcPos(bob, BL, TR, level.Rnd));
                    Vector2 Padding = new Vector2(200, 375);

                    if (level.PieceSeed.GeometryType == LevelGeometry.Right)
                        Tools.EnsureBounds_X(floater, TR - Padding,
                                                      BL + Padding);
                    else
                        Tools.EnsureBounds_Y(floater, TR - Padding,
                                                      BL + Padding);
                }
            }
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);

            // Get Floater parameters
            Floater_Parameters Params = (Floater_Parameters)level.Style.FindParams(Floater_AutoGen.Instance);

            if (Params.Special.Hallway)
                Hallway(level, BL, TR);

            if (!Params.DoStage2Fill) return;

            float y = (TR.Y + BL.Y) / 2;
            Vector2 FillBL = new Vector2(BL.X + 600, y - level.MainCamera.GetHeight() / 2 + 220);
            Vector2 FillTR = new Vector2(TR.X, y + level.MainCamera.GetHeight() / 2 - 350);

            int Sparsity = (int)Params.FloaterSparsity.Val;
            level.Fill(FillBL, FillTR, 225 * Sparsity, 250,
                (Level.FillCallback)delegate(Vector2 pos)
                {
                    Floater floater = (Floater)CreateAt(level, pos);
                    Vector2 Padding = new Vector2(200, 375);

                    if (level.PieceSeed.GeometryType == LevelGeometry.Right)
                        Tools.EnsureBounds_X(floater, TR - Padding,
                                                      BL + Padding);
                    else
                        Tools.EnsureBounds_Y(floater, TR - Padding,
                                                      BL + Padding);
                });
        }
    }

    public partial class Level
    {
        public void CleanupFloaters(Vector2 BL, Vector2 TR)
        {
            // Get Floater parameters
            Floater_Parameters Params = (Floater_Parameters)Style.FindParams(Floater_AutoGen.Instance);

            Cleanup(ObjectType.Floater, delegate(Vector2 pos)
            {
                float dist = Params.FloaterMinDist.GetVal(pos);
                return new Vector2(dist, dist);
            }, BL + new Vector2(400, 0), TR - new Vector2(500, 0));
        }
    }
}