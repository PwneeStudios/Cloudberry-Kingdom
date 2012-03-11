using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class BobPhsxRandom : BobPhsx
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Random";
            Icon = ObjectIcon.RandomIcon;
        }
        static readonly BobPhsxRandom instance = new BobPhsxRandom();
        public static BobPhsxRandom Instance { get { return instance; } }

        public static BobPhsx ChooseHeroType()
        {
            return Tools.GlobalRnd.Choose(Bob.HeroTypes);
        }

        // Instancable class
        public BobPhsxRandom()
        {
        }
    }
}