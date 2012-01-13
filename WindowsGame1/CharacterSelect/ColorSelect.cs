using Microsoft.Xna.Framework;
using Drawing;

namespace CloudberryKingdom
{
    public class ColorSelectPanel : GUI_Panel
    {
        public ColorSelect Grid;

        public ColorSelectPanel(Vector2 Size, int Width, int Height, int Control) : base(false)
        {
            this.Control = Control;

//#if NOT_PC
            this.AutoDraw = false;
//#endif

            Grid = new ColorSelect(Size, Width, Height);
            Grid.PlayerIndex = Control;

            Constructor();
        }

        public override void Init()
        {
            base.Init();

            Grid.FancyPos = new FancyVector2(Pos);
        }


        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            Grid.PhsxStep();
        }

        protected override void MyDraw()
        {
            base.MyDraw();

            Grid.Draw();
        }
    }

    /// <summary>
    /// Used to describe how a GUI element was exited from.
    /// </summary>
    public enum GUIExitState { Cancel, Select };

    public class ColorSelect : QuadSelect
    {
        /// <summary>
        /// Set to true when the ColorSelect is set to exit
        /// </summary>
        public bool Done = false;

        public GUIExitState ExitState;

        PieceQuad Backdrop;

        public ColorSelect(Vector2 Size, int Width, int Height)
        {
            Init(Size, Width, Height);
        }

        public override void Init(Vector2 Size, int Width, int Height)
        {
            base.Init(Size, Width, Height);

            // Make the backdrop
            Backdrop = new PieceQuad();
            Backdrop.Clone(PieceQuad.Get("DullMenu"));
            Backdrop.CalcQuads(Size);
        }

        public override void Draw()
        {
            Tools.EffectWad.FindByName("Paint").effect.Parameters["SceneTexture"].SetValue(Tools.TextureWad.FindByName("PaintSplotch").Tex);

            if (FancyPos != null)
                Backdrop.Base.Origin = FancyPos.Update();

            //Backdrop.Draw();

            base.Draw();
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            // Check for exit
            if (PhsxCount > DelayPhsxLength)
            {
                if (ButtonCheck.State(ControllerButtons.B, PlayerIndex).Pressed 
#if PC_VERSION
                    ||
                    //Tools.CurMouseDown() && !MouseInBox)
                    Tools.MouseReleased() && !MouseInBox)
#else
                    )
#endif
                {
                    Done = true;
                    ExitState = GUIExitState.Cancel;
                }
                else if (ButtonCheck.State(ControllerButtons.A, PlayerIndex).Pressed ||
                         ButtonCheck.State(ControllerButtons.X, PlayerIndex).Pressed)
                {
                    Done = true;
                    ExitState = GUIExitState.Select;
                }
            }
        }
    }
}