using System;
using System.Linq;

using Microsoft.Xna.Framework;

using CoreEngine.Random;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom.Levels
{
    public class Fireball_Parameters : AutoGen_Parameters
    {
        public Param FireballMaxAngle, FireballMinDist, MaxFireballDensity, Period;
        public Param KeepUnused;

        /// <summary>
        /// Whether the fireballs arc.
        /// </summary>
        public bool Arc = false;

        public bool DoFill = false;

        public float BorderFillStep = Unset.Float;
        public bool BorderTop = true;

        public float SurvivalHallwaySpeed = Unset.Float;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            NumAngles = PieceSeed.Style.Masochistic ? 100 : 4;
            NumPeriods = PieceSeed.Style.Masochistic ? 2 : 1;
            NumOffsets = PieceSeed.Style.Masochistic ? 100 : 16;

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                KeepUnused.SetVal(u => BobPhsxSpaceship.KeepUnused(u[Upgrade.Fireball]));
            }

            FillWeight = new Param(PieceSeed, u => u[Upgrade.Fireball]);

            SetVal(ref SurvivalHallwaySpeed, u => DifficultyHelper.Interp19(20, 45, u[Upgrade.Speed]));

            SetVal(ref BorderFillStep, u =>
            {
                if (u[Upgrade.Fireball] > 0)
                    DoFill = true;

                float v = DifficultyHelper.Interp159(800, 500, 200, u[Upgrade.Fireball]);
                if (PieceSeed.Style.Masochistic) v *= .7f;

                return v;
            });

            // General difficulty
            BobWidthLevel = new Param(PieceSeed);
            BobWidthLevel.SetVal(u => u[Upgrade.Fireball]);

            FireballMaxAngle = new Param(PieceSeed);
            FireballMaxAngle.SetVal(u => DifficultyHelper.Interp159(.01f, .3f, .75f, u[Upgrade.Fireball]));

            FireballMinDist = new Param(PieceSeed);
            FireballMinDist.SetVal(u => DifficultyHelper.Interp159(700, 340, 120, u[Upgrade.Fireball]));

            MaxFireballDensity = new Param(PieceSeed);
            MaxFireballDensity.SetVal(u => 4 * u[Upgrade.Fireball]);

            Period = new Param(PieceSeed);
            Period.SetVal(u => DifficultyHelper.InterpRestrict159(240, 195, 148, .7f * u[Upgrade.Speed] + .3f * u[Upgrade.Fireball]));
        }
    }

    public sealed class Fireball_AutoGen : AutoGen
    {
        static readonly Fireball_AutoGen instance = new Fireball_AutoGen();
        public static Fireball_AutoGen Instance { get { return instance; } }

        static Fireball_AutoGen() { }
        Fireball_AutoGen()
        {
            Do_PreFill_2 = true;
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            Fireball_Parameters Params = new Fireball_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)
        {
            base.CreateAt(level, pos, BL, TR);

            StyleData Style = level.Style;
            RichLevelGenData GenData = level.CurMakeData.GenData;
            PieceSeedData piece = level.CurMakeData.PieceSeed;

            // Get emitter parameters
            Fireball_Parameters Params = (Fireball_Parameters)level.Style.FindParams(Fireball_AutoGen.Instance);

            int Period = (int)Params.Period.GetVal(pos);
            int Offset = Params.ChooseOffset(Period, level.Rnd);

            return null;
        }

        void inner(Fireball_Parameters Params, Level level, Vector2 pos, int i, LevelGeometry Geometry)
        {
            Fireball emitter = (Fireball)CreateAt(level, pos);

            float Speed = (i == 0 ? 1 : -1);

            if (Params.Arc)
            {
                emitter.CoreData.StartData.Acceleration.Y = -Math.Abs(Speed) / 100f;
            }

            float MaxAngle = Params.FireballMaxAngle.GetVal(pos);
            if (level.Style.Masochistic) MaxAngle *= 1.25f;

            double Angle = Fireball_AutoGen.GetAngle(MaxAngle, Params.NumAngles, level.Rnd);
            if (Geometry == LevelGeometry.Right) Angle += Math.PI / 2;
            emitter.CoreData.StartData.Velocity = CoreMath.AngleToDir(Angle);

            if (level.Style.Masochistic) emitter.Period = (int)(.9f * emitter.Period);

            float v = Math.Abs(2 * emitter.CoreData.StartData.Position.Y / (.785f * emitter.Period));
            emitter.CoreData.StartData.Velocity *= Speed * v / emitter.CoreData.StartData.Velocity.Y;
            float actual_v = emitter.CoreData.StartData.Velocity.Length();
            
            //emitter.Period = (int)(emitter.Period * actual_v / v);
            emitter.CoreData.StartData.Velocity *= v / actual_v;

            level.AddObject(emitter);

            emitter.CoreData.GenData.EnforceBounds = false;
        }

        void BorderFill(Level level, Vector2 BL, Vector2 TR)
        {
            Fireball_Parameters Params = (Fireball_Parameters)level.Style.FindParams(Fireball_AutoGen.Instance);

            LevelGeometry Geometry = level.CurMakeData.PieceSeed.GeometryType;

            Vector2 pos = BL;
            
            float shift = -50;
            
            for (int i = 0; i <= 1; i++)
            {
                if (!Params.BorderTop && i == 1) continue;

                if (Geometry == LevelGeometry.Right)
                {
                    shift = -250;
                    pos.Y = (i == 0 ? level.MainCamera.BL.Y + shift : level.MainCamera.TR.Y - shift);
                    
                    pos.X = BL.X + Params.BorderFillStep/2;
                    while (pos.X < TR.X + 400)
                    {
                        int n = level.Rnd.RndInt(1, 3);
                        for (int q = 0; q < n; q++)
                            inner(Params, level, pos, i, Geometry);
                        pos.X += n * Params.BorderFillStep;
                    }
                }
                else
                {
                    pos.X = (i == 0 ? level.MainCamera.BL.X + shift : level.MainCamera.TR.X - shift);
                    for (pos.Y = BL.Y; pos.Y < TR.Y; pos.Y += Params.BorderFillStep)
                        inner(Params, level, pos, i, Geometry);
                }
            }
        }
        
        public Fireball_Parameters GetParams(Level level)
        {
            return (Fireball_Parameters)level.Style.FindParams(Fireball_AutoGen.Instance);
        }
        
        public override ObjectBase CreateAt(Level level, Vector2 pos)
        {
            Fireball_Parameters Params = GetParams(level);

            Fireball emitter = (Fireball)level.Recycle.GetObject(ObjectType.Fireball, true);

            emitter.Init(new PhsxData(), level);

            emitter.CoreData.Data.Position = pos;
            emitter.CoreData.StartData.Position = pos;

            emitter.Period = (level.Rnd.Rnd.Next(Params.NumPeriods - 1) + 1) * (int)Params.Period.GetVal(pos);
            
            if (Params.NumOffsets < 0)
                emitter.Offset = level.Rnd.Rnd.Next(emitter.Period);
            else
                emitter.Offset = level.Rnd.Rnd.Next(0, Params.NumOffsets) * emitter.Period / Params.NumOffsets;

            return emitter;
        }

        public static double GetAngle(float MaxAngle, int NumAngles, Rand Rnd)
        {
            if (NumAngles < 0)
                return Rnd.Rnd.NextDouble() * 2 * MaxAngle - MaxAngle;
            else if (NumAngles == 1)
                return 0;
            else
                return Rnd.Rnd.Next(0, NumAngles) * 2f * MaxAngle / (NumAngles - 1) - MaxAngle;
        }


        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);

            // Get Fireball parameters
            Fireball_Parameters Params = (Fireball_Parameters)level.Style.FindParams(Fireball_AutoGen.Instance);

            int Length = level.CurPiece.PieceLength;

            if (Params.DoStage2Fill && Params.DoFill)
                BorderFill(level, BL, TR);
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);

            // Get Fireball parameters
            Fireball_Parameters Params = (Fireball_Parameters)level.Style.FindParams(Fireball_AutoGen.Instance);
        }
    }
}
