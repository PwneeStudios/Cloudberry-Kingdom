using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace CloudberryKingdom
{
    public class SaveSeedAs : VerifyBaseMenu
    {
        public SaveSeedAs(int Control, PlayerData Player)
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
            CallDelay = 0;
            ReturnToCallerDelay = 0;
            
            MenuItem item;

            // Header
            HeaderText = new EzText(Localization.Words.SaveRandomSeedAs, ItemFont);
            HeaderText.Name = "Header";
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);

            // Save seed
            item = new MenuItem(new EzText(Localization.Words.SaveSeed, ItemFont));
            item.Name = "Save";
            item.Go = Save;
            AddItem(item);


            MakeBackButton();

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            SetPosition();
            MyMenu.SortByHeight();
            MyMenu.SelectItem(0);
        }

        void Save(MenuItem _item)
        {
            // Save the seed
            if (TextBox.Text.Length > 0)
            {
                Player.MySavedSeeds.SaveSeed(Tools.CurLevel.MyLevelSeed.ToString(), TextBox.Text);

                // Success!
                var ok = new AlertBaseMenu(Control, Localization.Words.SeedSavedSuccessfully, Localization.Words.Hooray);
                ok.OnOk = new OnOkProxy(this);
                Call(ok);
            }
            else
            {
                // Failure!
                var ok = new AlertBaseMenu(Control, Localization.Words.NoNameGiven, Localization.Words.Oh);
                ok.OnOk = new OnOkProxy(this);
                Call(ok);
            }

            Hide(PresetPos.Left);
            Active = false;
        }

        public override void OnReturnTo()
        {
            // Do nothing
        }

        class OnOkProxy : Lambda
        {
            SaveSeedAs ssa;

            public OnOkProxy(SaveSeedAs ssa)
            {
                this.ssa = ssa;
            }

            public void Apply()
            {
                ssa.OnOk();
            }
        }

        void OnOk()
        {
            this.SlideOutTo = PresetPos.Left;
            ReturnToCaller(false);
            
            this.Hid = true;
            this.Active = false;
        }

        public override void Release()
        {
            base.Release();

            TextBox.Release();
        }

        private void SetPosition()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(1269.445f, 161f); }
            _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(1338.89f, -52.8888f); }

            MyMenu.Pos = new Vector2(-1125.001f, -319.4444f);

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1175.696f, 233.3334f); _q.Size = new Vector2(1500f, 803.2258f); }
            MyPile.FindEzText("Header").Pos = new Vector2(61.11098f, 821.8887f);

            MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            TextBox = new GUI_TextBox(Tools.CurLevel.MyLevelSeed.SuggestedName(), Vector2.Zero, new Vector2(1.85f, .65f), .95f);
            TextBox.MaxLength = 50;
            TextBox.FixedToCamera = false;
            TextBox.Pos.SetCenter(MyPile.FancyPos);
            TextBox.Pos.RelVal = new Vector2(1175.001f, 277.7778f);
            TextBox.OnEnter.Add(new SaveSeedAsOnEnterLambda(this));
            TextBox.OnEscape.Add(new SaveSeedAsOnEscapeLambda(this));
            MyGame.AddGameObject(TextBox);

            SetPosition();
        }

        class SaveSeedAsOnEscapeLambda : Lambda
        {
            SaveSeedAs ssa;
            public SaveSeedAsOnEscapeLambda(SaveSeedAs ssa)
            {
                this.ssa = ssa;
            }

            public void Apply()
            {
                ssa.TextBox.Active = false;
                ssa.ReturnToCaller();
            }
        }

        class SaveSeedAsOnEnterLambda : Lambda
        {
            SaveSeedAs ssa;
            public SaveSeedAsOnEnterLambda(SaveSeedAs ssa)
            {
                this.ssa = ssa;
            }

            public void Apply()
            {
                if (ssa.TextBox.Text.Length <= 0)
                    return;

                ssa.Save(null);
            }
        }
    }
}