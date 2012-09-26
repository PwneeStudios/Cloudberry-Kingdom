using System;

namespace CloudberryKingdom.Levels
{
    // Stores the level of each obstacles
    // CalcGenData translates this into actual parameters
    public class Upgrades
    {
        public float[] UpgradeLevels;

        public Upgrades()
        {
            Initialize();
        }

        public Upgrades(Upgrades u)
        {
            Initialize();
            CopyFrom(u);
        }

        void Initialize()
        {
            UpgradeLevels = new float[Tools.UpgradeTypes];
        }


        public void CopyFrom(Upgrades u)
        {
            u.UpgradeLevels.CopyTo(UpgradeLevels, 0);
        }

        /// <summary>
        /// Access the specified upgrade level
        /// </summary>
        /// <param name="upgrade">The specified upgrade type</param>
        public float this[Upgrade upgrade]
        {
            get { return UpgradeLevels[(int)upgrade]; }
            set
            {
                UpgradeLevels[(int)upgrade] = value;
            }
        }

        public int[] this[params Upgrade[] u]
        {
            get { return null; }
            set
            {
                if (u.Length != value.Length)
                    throw(new Exception("List length mismatch"));

                for (int i = 0; i < u.Length; i++)
                    this[u[i]] = value[i];
            }
        }

        /// <summary>
        /// Set every upgrade to level 0
        /// </summary>
        public void Zero()
        {
            for (int i = 0; i < UpgradeLevels.Length; i++)
                UpgradeLevels[i] = 0;
        }

        public static int MaxBobWidth = 360;
        public void CalcGenData(LevelGenData GenData, StyleData Style)
        {
            Style.Calculate(this);

            float JumpLevel = this[Upgrade.Jump];
            JumpLevel = 3.75f + .625f * JumpLevel;

            // Jump
            GenData[DifficultyParam.FillSparsity] = 100;
            int Min = GenData[DifficultyParam.MinBoxSizeX] =
                //(int)DifficultyHelper.Interp(180, 40, this[Upgrade.Jump]);
                (int)DifficultyHelper.InterpRestrict19(180, 40, this[Upgrade.Jump]);
            if (Min < 50) GenData[DifficultyParam.MinBoxSizeX] = 10;
            GenData[DifficultyParam.MaxBoxSizeX] = 
                //(int)Math.Max(Min + 1, 380 - 50 * this[Upgrade.Jump]);
                //(int)DifficultyHelper.Interp(460, Min + 1, this[Upgrade.Jump]);
                (int)DifficultyHelper.Interp(420, Min + 1, this[Upgrade.Jump]);
            GenData[DifficultyParam.MinBoxSizeY] = 2000;
            GenData[DifficultyParam.MaxBoxSizeY] = 2000;

            GenData[DifficultyParam.TimeToBobTargetSwitch] = (int)Math.Max(10, 700 - 150 * JumpLevel);

            GenData[DifficultyParam.JumpingSpeedRetardFactor] =
                            (int)Math.Min(100, 66f + 3.4f * JumpLevel);
                            //(int)Math.Min(100, 76f + 3f * JumpLevel);
            GenData[DifficultyParam.RetardJumpLength] = (int)DifficultyHelper.Interp159(9, 6f, -1, JumpLevel);
            GenData[DifficultyParam.DistancePast] = (int)Math.Min(10, 5 - 115f + .1f * 115f * JumpLevel);
            GenData[DifficultyParam.DistancePast_NoJump] = (int)Math.Min(800, .1f * 700f * JumpLevel);
            GenData[DifficultyParam.EdgeJumpDuration] = (int)Math.Min(800, .1f * 300f * JumpLevel);
            GenData[DifficultyParam.NoEdgeJumpDuration] = (int)Math.Max(0, 300f - .1f * 300f * JumpLevel);

            GenData[DifficultyParam.EdgeSafety] = (int)Math.Max(1, 20f - 2f * this[Upgrade.Jump]);

            GenData[DifficultyParam.ApexWait] =
                //(int)Math.Max(2, 12f - 1.1f * this[Upgrade.Jump]);
                (int)Math.Max(2, 8f - .95f * this[Upgrade.Jump]);




            GenData[BehaviorParam.ForwardLengthBase] = 60;
            GenData[BehaviorParam.ForwardLengthAdd] = 100;
            GenData[BehaviorParam.BackLengthBase] = 3;
            GenData[BehaviorParam.BackLengthAdd] = 15;
            if (Style.ReverseType == StyleData._ReverseType.None)
            {
                GenData[BehaviorParam.BackLengthBase] = 1;
                GenData[BehaviorParam.BackLengthAdd] = 1;
            }
            if (Style.ReverseType == StyleData._ReverseType.Normal2)
            {
                GenData[BehaviorParam.BackLengthBase] = 6;
                GenData[BehaviorParam.BackLengthAdd] = 12;
            }
            if (Style.ReverseType == StyleData._ReverseType.Normal3)
            {
                GenData[BehaviorParam.BackLengthBase] = 10;
                GenData[BehaviorParam.BackLengthAdd] = 18;
            }



            // Decrease reverse for higher jump levels
            float backbase = GenData[BehaviorParam.BackLengthBase];
            backbase = (int)(backbase * Math.Max(0.1f, 12 - JumpLevel) / 10);
            backbase = Math.Max(1, backbase);
            GenData[BehaviorParam.BackLengthBase] = (int)backbase;

            float backadd = GenData[BehaviorParam.BackLengthAdd];
            backadd = (int)(backadd * Math.Max(0.1f, 12 - JumpLevel) / 10);
            backadd = Math.Max(1, backadd);
            GenData[BehaviorParam.BackLengthAdd] = (int)backadd;

            GenData[BehaviorParam.FallLengthBase] = 15;
            GenData[BehaviorParam.FallLengthAdd] = 60;
            GenData[BehaviorParam.JumpLengthBase] = 15;
            GenData[BehaviorParam.JumpLengthAdd] = 60;
            Style.JumpType = StyleData._JumpType.Always;
            if (Style.JumpType == StyleData._JumpType.Always)
            {
                GenData[BehaviorParam.FallLengthBase] = 1;
                GenData[BehaviorParam.FallLengthAdd] = 10;
            }
            if (Style.JumpType == StyleData._JumpType.Alot)
            {
                GenData[BehaviorParam.FallLengthBase] = 10;
                GenData[BehaviorParam.FallLengthAdd] = 35;
            }
            if (Style.JumpType == StyleData._JumpType.Normal2)
            {
                GenData[BehaviorParam.FallLengthBase] = 1;
                GenData[BehaviorParam.FallLengthAdd] = 85;
            }

            GenData[BehaviorParam.MoveWeight] = (int)(50 + 1 * JumpLevel);
            GenData[BehaviorParam.MoveLengthBase] = 20;// +8 * JumpLevel;
            GenData[BehaviorParam.MoveLengthAdd] = 20;// +8 * JumpLevel;
            GenData[BehaviorParam.SitWeight] = (int)Math.Max(10, 20 - 4 * JumpLevel);
            GenData[BehaviorParam.SitLengthBase] = (int)Math.Max(3, 10 - 1 * JumpLevel);
            GenData[BehaviorParam.SitLengthAdd] = (int)Math.Max(25, 8 - 1 * JumpLevel);
            if (Style.PauseType == StyleData._PauseType.None)
            {
                GenData[BehaviorParam.SitWeight] = 1;
                GenData[BehaviorParam.SitLengthBase] = 1;
                GenData[BehaviorParam.SitLengthAdd] = 1;
            }
            if (Style.PauseType == StyleData._PauseType.Limited)
            {
                GenData[BehaviorParam.SitWeight] = (int)Math.Max(6, 20 - 5 * JumpLevel);
                GenData[BehaviorParam.SitLengthBase] = (int)Math.Max(2, 10 - 1 * JumpLevel);
                GenData[BehaviorParam.SitLengthAdd] = (int)Math.Max(15, 8 - 1 * JumpLevel);
            }
            if (Style.PauseType == StyleData._PauseType.Normal2)
            {
                GenData[BehaviorParam.SitWeight] = (int)Math.Max(7, 20 - 5 * JumpLevel);
                GenData[BehaviorParam.SitLengthBase] = (int)Math.Max(15, 10 - 1 * JumpLevel);
                GenData[BehaviorParam.SitLengthAdd] = (int)Math.Max(45, 8 - 1 * JumpLevel);
            }

            switch (Style.MoveTypePeriod)
            {
                case StyleData._MoveTypePeriod.Inf:
                    GenData[BehaviorParam.MoveTypePeriod] = 7000; break;
                case StyleData._MoveTypePeriod.Normal1:
                    GenData[BehaviorParam.MoveTypePeriod] = 100; break;
                case StyleData._MoveTypePeriod.Normal2:
                    GenData[BehaviorParam.MoveTypePeriod] = 250; break;
                case StyleData._MoveTypePeriod.Short:
                    GenData[BehaviorParam.MoveTypePeriod] = 40; break;
            }

            switch (Style.MoveTypeInnerPeriod)
            {
                case StyleData._MoveTypeInnerPeriod.Long:
                    GenData[BehaviorParam.MoveTypeInnerPeriod] = 65; break;
                case StyleData._MoveTypeInnerPeriod.Normal:
                    GenData[BehaviorParam.MoveTypeInnerPeriod] = 40; break;
                case StyleData._MoveTypeInnerPeriod.Short:
                    GenData[BehaviorParam.MoveTypeInnerPeriod] = 24; break;
            }

            // General
            GenData[DifficultyParam.GeneralMinDist] = Math.Max(40, 460 - 55 * (int)this[Upgrade.General]);
            
            GenData[DifficultyParam.BigBoxX] = Math.Max(0, MaxBobWidth - 40 * (int)this[Upgrade.General]);
            //GenData[DifficultyParam.BigBoxY] = 0;// Math.Max(0, 150 - 20 * this[Upgrade.General]);

            // Fun run
            if (Style.FunRun)
            {
                GenData[BehaviorParam.ForwardLengthBase] = Math.Max(1, GenData[BehaviorParam.ForwardLengthBase] / 2);
                GenData[BehaviorParam.ForwardLengthAdd] = Math.Max(1, GenData[BehaviorParam.ForwardLengthAdd] / 2);
                GenData[BehaviorParam.BackLengthBase] = Math.Max(1, GenData[BehaviorParam.BackLengthBase] / 2);
                GenData[BehaviorParam.BackLengthAdd] = Math.Max(1, GenData[BehaviorParam.BackLengthAdd] / 2);

                GenData[BehaviorParam.SitWeight] = Math.Max(1, GenData[BehaviorParam.SitWeight] / 2);
                GenData[BehaviorParam.SitLengthBase] = Math.Max(1, GenData[BehaviorParam.SitLengthBase] / 2);
                GenData[BehaviorParam.SitLengthAdd] = Math.Max(1, GenData[BehaviorParam.SitLengthAdd] / 2);
            }
        }
    }
}