﻿using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;

namespace CloudberryKingdom.Levels
{
    public class BlockEmitter_Parameters : AutoGen_Parameters
    {
        public enum BoxStyle { TopOnly, FullBox, NoSides, Meatboy };

        public enum Style { Full, Separated }
        public Style MyStyle;

        /// <summary>
        /// The frame number on which the last used emitter was marked as used.
        /// </summary>
        public int LastUsedTimeStamp = 0;

        public Param Delay, Speed, Width, SpeedAdd, WidthAdd, Types, Amp, Dist, DistAdd, KeepUnused;
        public float StepCutoff = 1350;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            MyStyle = (Style)level.Rnd.RndEnum<Style>();

            if (level.Geometry == LevelGeometry.Up && level.DefaultHeroType is BobPhsxMeat)
            {
                FillWeight = new Param(PieceSeed, u => Math.Min(8, .95f * u[Upgrade.Elevator]));
                Width = new Param(PieceSeed, u => DifficultyHelper.Interp159(180, 140, 95, u[Upgrade.Elevator]));
                DistAdd = new Param(PieceSeed, u => .35f * DifficultyHelper.Interp19(.135f * 2400, .385f * 250, u[Upgrade.Elevator]));
                Delay = new Param(PieceSeed, u => .4f * Math.Max(60, 120 - 6 * u[Upgrade.Speed]));
            }
            else
            {
                FillWeight = new Param(PieceSeed, u => .5f * u[Upgrade.Elevator]);
                Width = new Param(PieceSeed, u => DifficultyHelper.Interp159(150, 120, 75, u[Upgrade.Elevator]));
                DistAdd = new Param(PieceSeed, u => DifficultyHelper.Interp19(.135f * 2800, .385f * 500, u[Upgrade.Elevator]));
                Delay = new Param(PieceSeed, u => Math.Max(60, 120 - 6 * u[Upgrade.Speed]));
            }

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                KeepUnused.SetVal(u => BobPhsxSpaceship.KeepUnused(u[Upgrade.GhostBlock]));
            }

            Dist = new Param(PieceSeed, u =>
            {
                if (u[Upgrade.Elevator] > 0)
                    return .105f * DifficultyHelper.Interp19(5000, 500, u[Upgrade.Elevator]);
                else
                    return 1400;
            });

            Amp = new Param(PieceSeed, u => Math.Min(450, 0 + 30 * u[Upgrade.Elevator]));

            Types = new Param(PieceSeed, u => Math.Min(2, u[Upgrade.Elevator] / 4));

            Speed = new Param(PieceSeed, u => DifficultyHelper.Interp19(5.6f, 16f, u[Upgrade.Speed]));

            SpeedAdd = new Param(PieceSeed);
            SpeedAdd.SetVal(u => DifficultyHelper.Interp19(5.6f, 13f, u[Upgrade.Speed])
                             * .025f * u[Upgrade.Elevator]);

            WidthAdd = new Param(PieceSeed);
            WidthAdd.SetVal(u =>
            {
                return 0;
            });
        }
    }

    public sealed class BlockEmitter_AutoGen : AutoGen
    {
        static readonly BlockEmitter_AutoGen instance = new BlockEmitter_AutoGen();
        public static BlockEmitter_AutoGen Instance { get { return instance; } }

        static BlockEmitter_AutoGen() { }
        BlockEmitter_AutoGen()
        {
            Do_PreFill_1 = true;
            Do_WeightedPreFill_1 = true;
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos)
        {
            return base.CreateAt(level, pos);

            // Do nothing
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            BlockEmitter_Parameters Params = new BlockEmitter_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void PreFill_1(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_1(level, BL, TR);

            // Get BlockEmitter parameters
            BlockEmitter_Parameters Params = (BlockEmitter_Parameters)level.Style.FindParams(BlockEmitter_AutoGen.Instance);

            Vector2 Pos = BL;
            int count = 0;

            // Fill horizontal level
            if (level.PieceSeed.GeometryType == LevelGeometry.Right)
                while (Pos.X < level.Fill_TR.X)
                {
                    if (Params.Dist.GetVal(Pos) < Params.StepCutoff)
                    {
                        BlockEmitter bm = (BlockEmitter)level.Recycle.GetObject(ObjectType.BlockEmitter, true);
                        bm.Init(Pos, level, level.CurMakeData.PieceSeed.ElevatorBoxStyle);

                        float Vel = GetVel(Params, ref Pos);

                        bool Bottom = false;
                        switch (level.Style.ElevatorSwitchType)
                        {
                            case StyleData._ElevatorSwitchType.AllDown: Bottom = true; break;
                            case StyleData._ElevatorSwitchType.AllUp: Bottom = false; break;
                            case StyleData._ElevatorSwitchType.Alternate: Bottom = count % 2 == 0; break;
                            case StyleData._ElevatorSwitchType.Random: Bottom = level.Rnd.Rnd.NextDouble() > .5; break;
                        }

                        if (Bottom)
                        {
                            bm.EmitData.Position = bm.Core.Data.Position = new Vector2(Pos.X, level.MainCamera.BL.Y - 200);
                            bm.EmitData.Velocity = new Vector2(0, Vel);
                        }
                        else
                        {
                            bm.EmitData.Position = bm.Core.Data.Position = new Vector2(Pos.X, level.MainCamera.TR.Y + 200);
                            bm.EmitData.Velocity = new Vector2(0, -Vel);
                        }

                        SetAndAdd(level, Params, Pos, bm);
                        bm.Range.Y = level.MainCamera.GetHeight() / 2;
                    }

                    Pos.X += Params.Dist.GetVal(Pos) + level.Rnd.RndFloat(0, Params.DistAdd.GetVal(Pos));

                    count++;
                }
            // Fill vertical level
            else
                while (Pos.Y < TR.Y)
                {
                    if (Params.Dist.GetVal(Pos) < Params.StepCutoff)
                    {
                        float DistAdd = Params.Dist.GetVal(Pos);
                        if (level.PieceSeed.GeometryType == LevelGeometry.Down)
                            DistAdd *= .2f;

                        if (DistAdd < Params.StepCutoff)
                        {
                            BlockEmitter bm = (BlockEmitter)level.Recycle.GetObject(ObjectType.BlockEmitter, true);
                            bm.Init(Pos, level, level.CurMakeData.PieceSeed.ElevatorBoxStyle);

                            float Vel = GetVel(Params, ref Pos);

                            bool Left = false;
                            switch (level.Style.ElevatorSwitchType)
                            {
                                case StyleData._ElevatorSwitchType.AllDown: Left = true; break;
                                case StyleData._ElevatorSwitchType.AllUp: Left = false; break;
                                case StyleData._ElevatorSwitchType.Alternate: Left = count % 2 == 0; break;
                                case StyleData._ElevatorSwitchType.Random: Left = level.Rnd.Rnd.NextDouble() > .5; break;
                            }

                            if (Left)
                            {
                                bm.EmitData.Position = bm.Core.Data.Position = new Vector2(level.MainCamera.BL.X - 200, Pos.Y);
                                bm.EmitData.Velocity = new Vector2(Vel, 0);
                            }
                            else
                            {
                                bm.EmitData.Position = bm.Core.Data.Position = new Vector2(level.MainCamera.TR.X + 200, Pos.Y);
                                bm.EmitData.Velocity = new Vector2(-Vel, 0);
                            }

                            SetAndAdd(level, Params, Pos, bm);
                            bm.Range.Y = bm.Range.X;
                            bm.Range.X = level.MainCamera.GetWidth() / 2;
                        }
                    }

                    Pos.Y += Params.Dist.GetVal(Pos) + level.Rnd.RndFloat(0, Params.DistAdd.GetVal(Pos));

                    count++;
                }
        }

        private static float GetVel(BlockEmitter_Parameters Params, ref Vector2 Pos)
        {
            //float SpeedAdd = Params.SpeedAdd.GetVal(Pos);

            float Vel = Params.Speed.GetVal(Pos) +
                        0;
            //Tools.ChooseOne(SpeedAdd, 0, -SpeedAdd);

            //MyLevel.Rnd.RndFloat(-Params.SpeedAdd.GetVal(Pos),
            //             Params.SpeedAdd.GetVal(Pos));

            return Vel;
        }

        private static void SetAndAdd(Level level, BlockEmitter_Parameters Params, Vector2 Pos, BlockEmitter bm)
        {
            bm.Delay = (int)Params.Delay.GetVal(Pos);

            // Longer delays for vertical levels
            if (level.Geometry == LevelGeometry.Up || level.Geometry == LevelGeometry.Down)
                bm.Delay *= 2;

            bm.Size = new Vector2(0, 40);
            bm.Size.X = Params.Width.GetVal(Pos) +
                        level.Rnd.RndFloat(0, Params.WidthAdd.GetVal(Pos));
            bm.AlwaysOn = true;
            
            //bm.Core.GenData.RemoveIfUnused = level.DefaultHeroType is BobPhsxSpaceship ? false : true;
            bm.Core.GenData.Decide_RemoveIfUnused(Params.KeepUnused.GetVal(Pos), level.Rnd);

            // Discrete period offsets
            int NumOffsets = 4; // Params.NumOffsets;
            bm.Offset = level.Rnd.Rnd.Next(0, NumOffsets) * bm.Delay / NumOffsets;

            bm.MyMoveType = (MovingPlatform.MoveType)level.Rnd.Rnd.Next(0, (int)Params.Types.GetVal(Pos));
            bm.Amp = Params.Amp.GetVal(Pos);

            if (level.Style.RemoveBlockOnOverlap)
                bm.Core.GenData.RemoveIfOverlap = true;

            level.AddObject(bm);
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
        }
    }
}