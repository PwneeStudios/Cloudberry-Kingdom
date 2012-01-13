using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    /* Inherited autogen pattern
    public class boob : Floater_Spin_AutoGen
    {
        static readonly boob instance = new boob();
        public static new boob Instance { get { return instance; } }

        static boob() { }
        boob()
        {
            Do_PreFill_2 = true;
        }
    }
*/

    // Would be nice to get rid of this, and just rely on ...__AutoGen.instances, and have dictionaries with instances as keys
    public enum Upgrade { Fireball, Firesnake, FlyingBlock, Spike, FallingBlock, FlyBlob, FireSpinner, MovingBlock, Elevator, SpikeyGuy, Pinky, SpikeyLine, Laser, GhostBlock, BouncyBlock, Cloud, Conveyor, General, Speed, Jump, Ceiling };

    public class AutoGen_Parameters
    {
        /// <summary>
        /// Whether to intelligently spread out the period offsets of placed obstacles
        /// </summary>
        public static bool IntelliSpread = true;

        public int ChooseOffset(int Period)
        {
            if (AutoGen_Parameters.IntelliSpread)
                return Counter++ % NumOffsets * Period / NumOffsets;
            else
                return Tools.Rnd.Next(0, NumOffsets) * Period / NumOffsets;
        }

        public int EnforceOffset(int offset, int period)
        {
            int offset_size = period / NumOffsets;
            return (int)(offset / offset_size) * offset_size;
        }

        /// <summary>
        /// How many objects have been created
        /// </summary>
        public int Counter = 0;

        public PieceSeedData PieceSeed;
        public Param FillWeight;

        /// <summary>
        /// The difficulty level of the bounding box around the computer.
        /// </summary>
        public Param BobWidthLevel;

        /// <summary>
        /// Whether the obstacle should toggle Masochistic mode.
        /// </summary>
        public bool Masochistic = false;

        /// <summary>
        /// Number of possible emit angles.
        /// </summary>
        public int NumAngles = 2;

        /// <summary>
        /// Number of possible periods.
        /// </summary>
        public int NumPeriods = 2;

        /// <summary>
        /// Number of possible period offsets for each period. If -1 then all offsets are allowed.
        /// </summary>
        public int NumOffsets = 3;

        /// <summary>
        /// Whether to do the default Stage 2 PreFill
        /// </summary>
        public bool DoStage2Fill = true;

        public virtual void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            this.PieceSeed = PieceSeed;
        }

        public void SetVal(ref Vector2 val, Vector2 newval)
        {
            if (val != Unset.Vector) return;

            val = newval;
        }

        public void SetVal(ref Vector2 val, Func<Upgrades, Vector2> f)
        {
            if (val != Unset.Vector) return;

            if (PieceSeed.MyUpgrades1 != null)
                val = f(PieceSeed.MyUpgrades1);
        }

        public void SetVal(ref float val, float newval)
        {
            if (val != Unset.Float) return;

            val = newval;
        }

        public void SetVal(ref float val, ParamFunc f)
        {
            if (val != Unset.Float) return;

            if (PieceSeed.MyUpgrades1 != null)
                val = f(PieceSeed.MyUpgrades1);
        }

        public void SetVal(ref int val, int newval)
        {
            if (val != Unset.Int) return;

            val = newval;
        }

        public void SetVal(ref int val, ParamFunc f)
        {
            if (val != Unset.Int) return;

            if (PieceSeed.MyUpgrades1 != null)
                val = (int)f(PieceSeed.MyUpgrades1);
        }
    }

    public class AutoGen
    {
        public virtual AutoGen_Parameters SetParameters(PieceSeedData data, Level level) { return null; }

        public virtual IObject CreateAt(Level level, Vector2 pos) { return null;  }
        public virtual IObject CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR) { return null; }

        public bool Do_WeightedPreFill_1, Do_PreFill_1, Do_PreFill_2, Do_ActiveFill_1, Do_ActiveFill_2;
        public virtual void PreFill_1(Level level, Vector2 BL, Vector2 TR) { }
        public virtual void PreFill_2(Level level, Vector2 BL, Vector2 TR) { }
        public virtual void ActiveFill_1(Level level, Vector2 BL, Vector2 TR) { }
        public virtual void Cleanup_1(Level level, Vector2 BL, Vector2 TR) { }
        public virtual void Cleanup_2(Level level, Vector2 BL, Vector2 TR) { }
    }
}