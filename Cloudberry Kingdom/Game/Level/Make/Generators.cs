using System.Collections.Generic;

namespace CloudberryKingdom.Levels
{
    public sealed class Generators
    {
        static readonly Generators instance = new Generators();
        public static Generators Instance { get { return instance; } }

        public static List<AutoGen> Gens, PreFill_1_Gens, PreFill_2_Gens, ActiveFill_1_Gens, WeightedPreFill_1_Gens;

        static Generators() { }
        Generators()
        {
            Gens = new List<AutoGen>();
            PreFill_1_Gens = new List<AutoGen>();
            PreFill_2_Gens = new List<AutoGen>();
            ActiveFill_1_Gens = new List<AutoGen>();
            WeightedPreFill_1_Gens = new List<AutoGen>();

            AddGenerator(NormalBlock_AutoGen.Instance);
            AddGenerator(Ceiling_AutoGen.Instance);

            AddGenerator(Coin_AutoGen.Instance);

            AddGenerator(FireballEmitter_AutoGen.Instance);
            AddGenerator(FireSpinner_AutoGen.Instance);
            AddGenerator(Spike_AutoGen.Instance);
            AddGenerator(Laser_AutoGen.Instance);
            AddGenerator(Floater_AutoGen.Instance);
            AddGenerator(Floater_Spin_AutoGen.Instance);
            AddGenerator(Goomba_AutoGen.Instance);
            AddGenerator(FallingBlock_AutoGen.Instance);
            AddGenerator(GhostBlock_AutoGen.Instance);
            AddGenerator(MovingBlock_AutoGen.Instance);
            AddGenerator(BlockEmitter_AutoGen.Instance);
            AddGenerator(BouncyBlock_AutoGen.Instance);
            AddGenerator(Cloud_AutoGen.Instance);
            AddGenerator(SpikeyLine_AutoGen.Instance);
            AddGenerator(Firesnake_AutoGen.Instance);
            AddGenerator(FlyingBlock_AutoGen.Instance);
            AddGenerator(ConveyorBlock_AutoGen.Instance);

            AddGenerator(Pendulum_AutoGen.Instance);
            AddGenerator(Serpent_AutoGen.Instance);
            AddGenerator(LavaDrip_AutoGen.Instance);
        }

        public static void AddGenerator(AutoGen gen)
        {
            Gens.Add(gen);
            if (gen.Do_WeightedPreFill_1) WeightedPreFill_1_Gens.Add(gen);
            if (gen.Do_PreFill_1) PreFill_1_Gens.Add(gen);
            if (gen.Do_ActiveFill_1) ActiveFill_1_Gens.Add(gen);
            if (gen.Do_PreFill_2) PreFill_2_Gens.Add(gen);            
        }

        public static int IndexOf(AutoGen gen) { return Gens.IndexOf(gen); }
    }
}