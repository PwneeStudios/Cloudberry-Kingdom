
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class BobPhsxTimeship : BobPhsxSpaceship
    {
        public static float KeepUnused(float UpgradeLevel)
        {
            return .5f + .03f * UpgradeLevel;
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(5, 0, 0, 0);
            Name = Localization.Words.TimeShip;
            NameTemplate = "time ship";
            Icon = new PictureIcon(Tools.Texture("Spaceship_Paper"), Color.White, 1.15f * DefaultIconWidth);
        }
        static readonly BobPhsxTimeship instance = new BobPhsxTimeship();
        public static BobPhsxTimeship Instance { get { return instance; } }

        // Instancable class
        int AutoMoveLength, AutoMoveType, AutoStrafeLength;
        int AutoDirLength, AutoDir;

        int RndMoveType;

        public BobPhsxTimeship()
        {
            DefaultValues();
        }

        public override void DefaultValues()
        {
            base.DefaultValues();
        }

        public override void Init(Bob bob)
        {
            base.Init(bob);
        }

        public override void PhsxStep()
        {
            base.PhsxStep();
        }

        public override void SideHit(ColType side, BlockBase block)
        {
            base.SideHit(side, block);
        }

        public override void ModData(ref Level.MakeData makeData, StyleData Style)
        {
            base.ModData(ref makeData, Style);

            Style.TimeType = Level.TimeTypes.ySync;
        }
    }
}