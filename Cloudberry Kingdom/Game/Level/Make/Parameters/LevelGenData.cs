using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public enum DifficultyParam
    {
        BigBoxX, BigBoxY,
        GeneralMinDist,
        MinBoxSizeX, MinBoxSizeY, MaxBoxSizeX, MaxBoxSizeY,

        LongJumpStart,
        MinFall, MaxFall, MinDrop, MaxDrop,
        TimeToBobTargetSwitch,
        JumpingSpeedRetardFactor, RetardJumpLength,
        DistancePast, DistancePast_NoJump,
        EdgeJumpDuration, NoEdgeJumpDuration,
        EdgeSafety, // How close to the edge of a block the computer can land on
        ApexWait, // How long after the apex of a jump should we wait before potentially landing on something?
        FillSparsity,
    };

    public enum BehaviorParam
    { 
        FallLengthBase, FallLengthAdd,
        BackLengthBase, BackLengthAdd,
        ForwardLengthBase, ForwardLengthAdd,
        MoveWeight, SitWeight, MoveLengthBase, MoveLengthAdd, SitLengthBase, SitLengthAdd,
        JumpWeight, NoJumpWeight, JumpLengthBase, JumpLengthAdd, NoJumpLengthBase, NoJumpLengthAdd,
        MoveTypePeriod, MoveTypeInnerPeriod
    };

    public class RichLevelGenData
    {
        public LevelGenData gen1, gen2;
        public Vector2 p1, p2;

        public void Set(DifficultyParam type, int val)
        {
            if (gen1 == null) return;
            gen1[type] = val;

            if (gen2 != null)
                gen2[type] = val;
        }
        public int Get(DifficultyParam type)
        {
            if (gen1 == null) return 0;
            if (gen2 == null) return gen1[type];

            return Math.Max(gen1[type], gen2[type]);
        }
        public int Get(DifficultyParam type, Vector2 pos)
        {
            if (gen1 == null) return 0;
            if (gen2 == null) return gen1[type];

            Vector2 tangent = p2 - p1;
            float length = tangent.LengthSquared();

            if (length < 100) return gen1[type];

            float t = Math.Max(0, Math.Min(1, Vector2.Dot(pos - p1, tangent) / length));

            return (int)((1 - t) * gen1[type] + t * gen2[type]);
        }


        public void Set(BehaviorParam type, int val)
        {
            gen1[type] = gen2[type] = val;
        }
        public int Get(BehaviorParam type)
        {
            if (gen1 == null) return 0;
            if (gen2 == null) return gen1[type];

            return Math.Max(gen1[type], gen2[type]);
        }
        public int Get(BehaviorParam type, Vector2 pos)
        {
            if (gen1 == null) throw (new Exception("No gen data to retrieve!"));
            if (gen2 == null) return gen1[type];

            Vector2 tangent = p2 - p1;
            float t = Vector2.Dot(pos, tangent) / tangent.LengthSquared();
            t = Math.Max(0, Math.Min(1, t));

            int val = (int)(.5f + (1 - t) * (float)gen1[type] + t * (float)gen2[type]);

            if (val < 1) 
                throw (new Exception("Nonpositive return!"));

            return val;
        }
    }

    public class LevelGenData
    {
        public int[] Difficulty;
        public int[] BehaviorParams;

        //public int Get(DifficultyParam type) { return Difficulty[(int)type]; }
        //public int Get(BehaviorParam type) { return Difficulty[(int)type]; }

        public int this[DifficultyParam type]
        {
            get { return Difficulty[(int)type]; }
            set { Difficulty[(int)type] = value; }
        }

        public int this[BehaviorParam type]
        {
            get { return BehaviorParams[(int)type]; }
            set { BehaviorParams[(int)type] = value; }
        }

        public LevelGenData()
        {
            Difficulty = new int[Tools.DifficultyTypes];
            Difficulty[(int)DifficultyParam.LongJumpStart] = 165;
            Difficulty[(int)DifficultyParam.MinFall] = 5;
            Difficulty[(int)DifficultyParam.MaxFall] = 10;
            Difficulty[(int)DifficultyParam.MinDrop] = 200;
            Difficulty[(int)DifficultyParam.MaxDrop] = 900;


            BehaviorParams = new int[]{ 4, 5, 20, 20, 5, 10,
                               2, 2, 2, 2, 2, 20,
                               2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
        }
    }
}