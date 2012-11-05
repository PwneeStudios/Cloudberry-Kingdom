using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace CloudberryKingdom
{
    public class LoadSeedAs : VerifyBaseMenu
    {
        public LoadSeedAs(int Control, PlayerData Player)
            : base(false)
        {
            this.Control = Control;
            this.Player = Player;
            FixedToCamera = true;

            Constructor();
        }

        PlayerData Player;
        GUI_TextBox TextBox;
        EzText HeaderText;
        public override void Init()
        {
            base.Init();

            this.FontScale *= .9f;

            // Header
            string Text = "Load the following Seed...?";
            HeaderText = new EzText(Text, ItemFont);
            HeaderText.Name = "Header";
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            HeaderText.Pos = HeaderPos;

            // Question
            //HeaderText = new EzText("?", ItemFont);
            //HeaderText.Name = "Question";
            //SetHeaderProperties(HeaderText);
            //MyPile.Add(HeaderText);
            //HeaderText.Pos = HeaderPos;

            MenuItem item;

            // Load seed
            item = new MenuItem(new EzText(Localization.Words.LoadSeed, ItemFont));
            item.Name = "Load";
            item.Go = Load;
            AddItem(item);


            MakeBackButton();

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            SetPosition();
            MyMenu.SortByHeight();
            MyMenu.SelectItem(0);
        }

        public override void Release()
        {
            base.Release();

            TextBox.Release();
        }

        private void SetPosition()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = new Vector2(1269.445f, 161f); }
            _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(1338.89f, -52.8888f); }

            MyMenu.Pos = new Vector2(-1125.001f, -319.4444f);

            MyPile.FindEzText("Header").Pos = new Vector2(61.11098f, 821.8887f);

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1175.696f, 233.3334f); _q.Size = new Vector2(1500f, 803.2258f); }

            MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            string clipboard = null;

#if WINDOWS
            try
            {
                clipboard = System.Windows.Forms.Clipboard.GetText();
            }
            catch
            {
                clipboard = "<Error>";
            }
#endif

            if (clipboard == null || clipboard.Length == 0)
                clipboard = "Type in a seed!";

            TextBox = new GUI_TextBox(clipboard, Vector2.Zero, new Vector2(1.85f, .65f), .95f);
            TextBox.MaxLength = 80;
            TextBox.FixedToCamera = false;
            TextBox.Pos.SetCenter(MyPile.FancyPos);
            TextBox.Pos.RelVal = new Vector2(1175.001f, 277.7778f);
            TextBox.OnEnter += OnEnter;
            TextBox.OnEscape += Back;
            MyGame.AddGameObject(TextBox);

            SetPosition();
        }

        void Back()
        {
            TextBox.Active = false;
            ReturnToCaller();
        }

        void Load(MenuItem _item)
        {
            if (TextBox != null)
            {
                TextBox.Enter();
                return;
            }

            SavedSeedsGUI.LoadSeed(TextBox.Text, this);
        }

        void OnEnter()
        {
            SavedSeedsGUI.LoadSeed(TextBox.Text, this);
            Active = false;
            ReturnToCaller();

            //MyGame.WaitThenDo(35, () =>
            //{
            //    float width = MyGame.Cam.GetWidth();
            //    TextBox.Pos.LerpTo(new Vector2(-width, 0), 20);
            //});
        }
    }
}