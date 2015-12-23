using Microsoft.Xna.Framework;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class BobPhsxUpsideDown : BobPhsxNormal
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(0, 0, 3, 0);
            Name = Localization.Words.UpsideDownMan;
            Adjective = "Anti-Grav";
            
            Icon = new PictureIcon(Tools.TextureWad.FindByName("Bob_Run_0024"), Color.White, DefaultIconWidth * -1.2f);

            HeroDollShift = new Vector2(0, 100);
        }
        static readonly BobPhsxUpsideDown instance = new BobPhsxUpsideDown();
        public static new BobPhsxUpsideDown Instance { get { return instance; } }

        // Instancable class
        public BobPhsxUpsideDown()
        {
            Set(this);
        }

        public override void Set(BobPhsx phsx)
        {
            Set(phsx, Vector2.One);
        }
        public void Set(BobPhsx phsx, Vector2 modsize)
        {
            BobPhsxNormal normal = phsx as BobPhsxNormal;
            if (null != normal && normal.Gravity > 0)
            {
                normal.Gravity *= -1;
                normal.ForceDown *= -1;
                normal.BobJumpAccel *= -1;
                normal.BobInitialJumpSpeed *= -1;
                normal.BobJumpAccel2 *= -1;
                normal.BobInitialJumpSpeed2 *= -1;
                normal.BobInitialJumpSpeedDucking *= -1;
                normal.BobInitialJumpSpeedDucking2 *= -1;
            }
        }

        public override void Init(Bobs.Bob bob)
        {
            base.Init(bob);

            Set(this);
        }

        public override void ModData(ref Level.MakeData makeData, StyleData Style)
        {
            base.ModData(ref makeData, Style);

            makeData.TopLikeBottom_Thin = true;
            makeData.BlocksAsIs = true;

            var Ceiling_Params = (Ceiling_Parameters)Style.FindParams(Ceiling_AutoGen.Instance);
            Ceiling_Params.Make = false;

            Style.BlockFillType = StyleData._BlockFillType.Invertable;
            Style.UseLowerBlockBounds = true;
            Style.OverlapCleanupType = StyleData._OverlapCleanupType.Sophisticated;

            Style.TopSpace = 50;

            var MParams = (MovingBlock_Parameters)Style.FindParams(MovingBlock_AutoGen.Instance);
            if (MParams.Aspect == MovingBlock_Parameters.AspectType.Tall)
                MParams.Aspect = MovingBlock_Parameters.AspectType.Thin;

            var GhParams = (GhostBlock_Parameters)Style.FindParams(GhostBlock_AutoGen.Instance);
            GhParams.BoxType = GhostBlock_Parameters.BoxTypes.Long;
        }

        public override void ModLadderPiece(PieceSeedData piece)
        {
            base.ModLadderPiece(piece);

            piece.ElevatorBoxStyle = BlockEmitter_Parameters.BoxStyle.NoSides;
        }

        public override void DollInitialize()
        {
            base.DollInitialize();

            LandOnSomething(true, null);
        }
    }
}