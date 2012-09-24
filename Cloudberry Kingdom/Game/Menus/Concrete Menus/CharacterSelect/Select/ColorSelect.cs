using Microsoft.Xna.Framework;
using Drawing;

namespace CloudberryKingdom
{
    public abstract class BaseColorSelect : GUI_Panel
    {
        public BaseColorSelect()
            : base(false)
        {
        }

        public abstract int GetAssociatedIndex();
        public abstract void SetIndexViaAssociated(int index);
        public abstract bool Done();
        public abstract GuiExitState ExitState();
    }

    public class ListSelectPanel : BaseColorSelect
    {
        public MenuList MyList;

        public void SetPos(Vector2 pos)
        {
            MyPile.Pos += pos;
            MyMenu.Pos += pos;
        }

        public override void SetIndexViaAssociated(int index)
        {
            int CorrespondingIndex = MyList.MyList.FindIndex(item => (int)(item.MyObject) == index);
            if (CorrespondingIndex < 0) CorrespondingIndex = 0;
            MyList.SetIndex(CorrespondingIndex);
        }

        public override int GetAssociatedIndex()
        {
            return (int)MyList.CurObj;
        }

        public override bool Done()
        {
            return IsDone;
        }

        public override GuiExitState ExitState()
        {
            return MyExitState;
        }

        string Header;

        public ListSelectPanel(int Control, string Header)
            : base()
        {
            this.Control = Control;
            this.AutoDraw = false;

            this.Header = Header;

            Constructor();
        }

        bool IsDone = false;
        GuiExitState MyExitState;
        
        void Back()
        {
            MyExitState = GuiExitState.Cancel;
            IsDone = true;
        }

        void Select()
        {
            MyExitState = GuiExitState.Select;
            IsDone = true;
        }

        void OnSelect()
        {
        }

        public override void Constructor()
        {
            base.Constructor();

            MyPile = new DrawPile();
            MyMenu = new Menu(false);
            EnsureFancy();

            MyMenu.OnB = Cast.ToMenu(Back);
            MyMenu.OnA = Cast.ToMenu(Select);

            MyList = new MenuList();
            MyList.Name = "list";
            MyList.Center = true;
            MyList.AdditionalOnSelect = OnSelect;

            MyMenu.Add(MyList);

            MyPile.Add(new EzText(Header, Tools.Font_Grobold42, true), "Header");
        }

        public override void OnAdd()
        {
 	        base.OnAdd();
        
            MenuItem _item;
            _item = MyMenu.FindItemByName("list"); if (_item != null) { _item.SetPos = new Vector2(0f, 158.7301f); }

            MyMenu.Pos = new Vector2(-1418.571f, -484.127f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-3.968231f, 595.2379f); }
            MyPile.Pos = new Vector2(-1414.604f, -492.0635f);
        }
    }

    public class ColorSelectPanel : BaseColorSelect
    {
        public ColorSelect Grid;

        public override void SetIndexViaAssociated(int index)
        {
            Grid.SetIndexViaAssociated(index);
        }

        public override int GetAssociatedIndex()
        {
            return Grid.GetAssociatedIndex();
        }

        public override bool Done() { return Grid.Done; }
        public override GuiExitState ExitState() { return Grid.ExitState; }

        public ColorSelectPanel(Vector2 Size, int Width, int Height, int Control) : base()
        {
            this.Control = Control;

            this.AutoDraw = false;

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
    public enum GuiExitState { Cancel, Select };

    public class ColorSelect : QuadSelect
    {
        /// <summary>
        /// Set to true when the ColorSelect is set to exit
        /// </summary>
        public bool Done = false;

        public GuiExitState ExitState;

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
            //Backdrop.Clone(PieceQuad.Get(""));
            //Backdrop.CalcQuads(Size);
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
                    ExitState = GuiExitState.Cancel;
                }
                else if (ButtonCheck.State(ControllerButtons.A, PlayerIndex).Pressed ||
                         ButtonCheck.State(ControllerButtons.X, PlayerIndex).Pressed)
                {
                    Done = true;
                    ExitState = GuiExitState.Select;
                }
            }
        }
    }
}