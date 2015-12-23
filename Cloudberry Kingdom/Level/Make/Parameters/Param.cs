using Microsoft.Xna.Framework;

using CoreEngine.Random;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public delegate Vector2 VectorParamFunc(Upgrades upgrades);
    public struct VectorParam
    {
        public Param X, Y;

        public VectorParam(PieceSeedData PieceSeed)
        {
            X = new Param(PieceSeed);
            Y = new Param(PieceSeed);
        }

        public VectorParam(PieceSeedData PieceSeed, VectorParamFunc f)
        {
            X = new Param(PieceSeed);
            Y = new Param(PieceSeed);

            if (f != null)
                SetVal(f);
        }
    
        public void SetVal(VectorParamFunc f)
        {
            X.SetVal(u => f(u).X);
            Y.SetVal(u => f(u).Y);
        }

        public Vector2 GetVal()
        {
            return new Vector2(X.GetVal(), Y.GetVal());
        }

        public Vector2 GetVal(Vector2 Pos)
        {
            return new Vector2(X.GetVal(Pos), Y.GetVal(Pos));
        }

        /// <summary>
        /// Assuming the top and bottom values of the vector are ranges of a value,
        /// this method returns a randomly chosen value in that range.
        /// </summary>
        /// <returns></returns>
        public float RndFloat(Vector2 Pos, Rand Rnd)
        {
            return Rnd.RndFloat(GetVal(Pos));
        }
    }

    public delegate float ParamFunc(Upgrades upgrades);
    public struct Param
    {
        PieceSeedData PieceSeed;
        float val1, val2;
        bool val1_IsSet, val2_IsSet;

        public float Val
        {
            get
            {
                return val1;
            }
            set
            {
                val1 = value;
                val2_IsSet = false;
            }
        }

        public Param(float val)
        {
            PieceSeed = null;

            val1 = val2 = val;
            val1_IsSet = val2_IsSet = true;
        }

        public Param(PieceSeedData PieceSeed)
        {
            val1 = val2 = 0;
            val1_IsSet = val2_IsSet = false;

            this.PieceSeed = PieceSeed;
        }

        public Param(PieceSeedData PieceSeed, ParamFunc f)
        {
            val1 = val2 = 0;
            val1_IsSet = val2_IsSet = false;

            this.PieceSeed = PieceSeed;

            if (f != null)
                SetVal(f);
        }
    
        public void SetVal(ParamFunc f)
        {
            if (PieceSeed.MyUpgrades1 != null)
            {
                val1 = f(PieceSeed.MyUpgrades1);
                val1_IsSet = true;
            }
            if (PieceSeed.MyUpgrades2 != null && PieceSeed.Start != PieceSeed.End)
            {
                val2 = f(PieceSeed.MyUpgrades2);
                val2_IsSet = true;
            }
        }

        public float GetVal()
        {
            if (PieceSeed == null) return val1;

            RichLevelGenData data = PieceSeed.MyGenData;

            if (!val1_IsSet || data.gen1 == null) return 0;
            if (!val2_IsSet || data.gen2 == null) return val1;

            return val1;
        }

        public float GetVal(Vector2 Pos)
        {
            if (PieceSeed == null) return val1;

            RichLevelGenData data = PieceSeed.MyGenData;

            if (!val1_IsSet || data.gen1 == null) return 0;
            if (!val2_IsSet || data.gen2 == null) return val1;

            Vector2 tangent = data.p2 - data.p1;
            float t = Vector2.Dot(Pos - data.p1, tangent) / tangent.LengthSquared();

            return (1 - t) * val1 + t * val2;
        }

        public static implicit operator Param(float val)
        {
            return new Param(val);
        }
    }
}