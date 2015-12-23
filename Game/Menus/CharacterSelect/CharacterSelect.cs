using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#if PC
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class CharacterSelect
    {
        public enum SelectState { Beginning, Selecting, Waiting };
        public SelectState MyState = SelectState.Beginning;

        public bool Join = false;

        /// <summary>
        /// True if the character select has been brought up mid game
        /// </summary>
        public bool QuickJoin;

        public GamerTag MyGamerTag;
        public HeroLevel MyHeroLevel;
        public Doll MyDoll;

        public int PlayerIndex;
        public PlayerData Player { get { return PlayerManager.Get(PlayerIndex); } }

        public int[] ItemIndex = new int[5];
        public List<MenuListItem>[] ItemList = { ColorSchemeManager.ColorList, ColorSchemeManager.OutlineList, ColorSchemeManager.HatList, ColorSchemeManager.CapeColorList, ColorSchemeManager.CapeOutlineColorList };

        Vector2 Center, NormalZoomCenter;

        public static float Width;
        public static Vector2[] Centers;
        public void InitCenters()
        {
            Vector2 Spacing = new Vector2(880, 0);
            Centers = new Vector2[]
            {
                -1.5f * Spacing,
                -.5f * Spacing,
                .5f * Spacing,
                1.5f * Spacing
            };

			// For Vita
			//Centers = new Vector2[]
			//{
			//    new Vector2(-960, 0),
			//    new Vector2(10000, 0),
			//    new Vector2(10000, 0),
			//    new Vector2(10000, 0),
			//};
        }

        public static void Shift(GUI_Panel panel)
        {
            panel.Shift(Centers[panel.Control]);
        }

        public void Release()
        {
            MyGamerTag.Release();
            MyHeroLevel.Release();
            MyDoll.Release();
        }

        public bool Fake = false;
        public CharacterSelect(int PlayerIndex, bool QuickJoin)
        {
            GameData game = Tools.CurGameData;

            Tools.StartGUIDraw();

            this.PlayerIndex = PlayerIndex;
            this.QuickJoin = QuickJoin;

            InitCenters();
            Center = Centers[PlayerIndex];
            NormalZoomCenter = Center;

            MyDoll = new Doll(PlayerIndex, this);
            MyGamerTag = new GamerTag(PlayerIndex, this);
            MyHeroLevel = new HeroLevel(PlayerIndex, this);
            game.AddGameObject(MyDoll);
            game.AddGameObject(MyGamerTag);
            game.AddGameObject(MyHeroLevel);

            InitColorScheme(PlayerIndex);

            if (QuickJoin && PlayerIndex >= 0 && PlayerManager.Get(PlayerIndex) != null && PlayerManager.Get(PlayerIndex).Exists)
            {
                Fake = true;
                game.AddGameObject(new Waiting(PlayerIndex, this, false));
            }
            else
            {
                Fake = false;
                game.AddGameObject(new JoinText(PlayerIndex, this));
            }
            
            /*
            if (QuickJoin)
            {
#if XBOX || XBOX_SIGNIN
                if (Player.MyGamer == null)
                    game.AddGameObject(new SignInMenu(this));
                else
                    game.AddGameObject(new SimpleMenu(PlayerIndex, this));
#else
                game.AddGameObject(new SimpleMenu(PlayerIndex, this));
#endif
            }
            else
            {
#if PC
                if (PlayerIndex == 0)
                    game.AddGameObject(new SimpleMenu(PlayerIndex, this));
                else
                    game.AddGameObject(new JoinText(this));
#else
                game.AddGameObject(new JoinText(this));
#endif
            }
            */
            Tools.EndGUIDraw();
        }

        public void InitColorScheme(int PlayerIndex)
        {
            if (Player.ColorSchemeIndex == Unset.Int)
                SetIndex(PlayerIndex);
            else
                SetIndex(Player.ColorSchemeIndex);
        }

        public void Randomize()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 1 || i == 2) continue;

                List<MenuListItem> list = new List<MenuListItem>(ItemList[i].Capacity);
                foreach (var item in ItemList[i])
                    if (PlayerManager.BoughtOrFree((Buyable)item.obj))
                        list.Add(item);

                ItemIndex[i] = ItemList[i].IndexOf(list.Choose(Tools.GlobalRnd));
            }

            Hat hat = CharacterSelectManager.AvailableHats.Choose(Tools.GlobalRnd);
            Hat beard = CharacterSelectManager.AvailableBeards.Choose(Tools.GlobalRnd);
            ItemIndex[1] = ColorSchemeManager.BeardInfo.IndexOf(beard);
            ItemIndex[2] = ColorSchemeManager.HatInfo.IndexOf(hat);

            Customize_UpdateColors();

            // Save new custom color scheme
            Player.CustomColorScheme = Player.ColorScheme;
            Player.ColorSchemeIndex = -1;
        }

        int HoldCapeIndex = 1, HoldCapeOutlineIndex = 1;
        public void Customize_UpdateColors()
        {
            bool ShowingCape =
                Player.ColorScheme.CapeColor.Clr.A > 0 ||
                Player.ColorScheme.CapeOutlineColor.Clr.A > 0;

            Player.ColorScheme.SkinColor = (ClrTextFx)ItemList[0][ItemIndex[0]].obj;
            Player.ColorScheme.BeardData = ColorSchemeManager.BeardInfo[ItemIndex[1]];
            Player.ColorScheme.HatData = ColorSchemeManager.HatInfo[ItemIndex[2]];
            Player.ColorScheme.CapeColor = (ClrTextFx)ItemList[3][ItemIndex[3]].obj;
            Player.ColorScheme.CapeOutlineColor = (ClrTextFx)ItemList[4][ItemIndex[4]].obj;

            // If the cape has gone from not-shown to shown,
            // make sure both the cape color and cape outline color aren't invisible.
            if (!ShowingCape &&
                (Player.ColorScheme.CapeColor.Clr.A > 0 ||
                 Player.ColorScheme.CapeOutlineColor.Clr.A > 0))
            {
                if (Player.ColorScheme.CapeColor.Equals(ColorSchemeManager.None)
                    || Player.ColorScheme.CapeColor.Clr.A == 0)
                {
                    ItemIndex[3] = HoldCapeIndex;
                    Player.ColorScheme.CapeColor = (ClrTextFx)ItemList[3][ItemIndex[3]].obj;
                }
                if (Player.ColorScheme.CapeOutlineColor.Equals(ColorSchemeManager.None)
                    || Player.ColorScheme.CapeOutlineColor.Clr.A == 0)
                {
                    ItemIndex[4] = HoldCapeOutlineIndex;
                    Player.ColorScheme.CapeOutlineColor = (ClrTextFx)ItemList[4][ItemIndex[4]].obj;
                }
            }

            // If the outline color is null, set the cape color to null and vis a versa
            if (Player.ColorScheme.CapeOutlineColor.Equals(ColorSchemeManager.None)
                && Player.ColorScheme.CapeColor.Clr.A > 0)
            {
                HoldCapeIndex = ItemIndex[3];
                ItemIndex[3] = 0;
                Player.ColorScheme.CapeColor = (ClrTextFx)ItemList[3][ItemIndex[3]].obj;
            }
            if (Player.ColorScheme.CapeColor.Equals(ColorSchemeManager.None)
                && Player.ColorScheme.CapeOutlineColor.Clr.A > 0)
            {
                HoldCapeOutlineIndex = ItemIndex[4];
                ItemIndex[4] = 0;
                Player.ColorScheme.CapeOutlineColor = (ClrTextFx)ItemList[4][ItemIndex[4]].obj;
            }

            MyDoll.UpdateColorScheme();
        }

        public bool HasCustom()
        {
            return Player.CustomColorScheme.SkinColor.Effect != null;
        }

        public bool AvailableColorScheme(ColorScheme scheme)
        {
            return CharacterSelectManager.AvailableHats.Contains(scheme.HatData) &&
                    CharacterSelectManager.AvailableBeards.Contains(scheme.BeardData) &&
                    PlayerManager.BoughtOrFree(scheme.SkinColor) &&
                    PlayerManager.BoughtOrFree(scheme.CapeColor) &&
                    PlayerManager.BoughtOrFree(scheme.CapeOutlineColor);
        }

        public void SetIndex(int i)
        {
            Player.ColorSchemeIndex = i;

            if (Player.ColorSchemeIndex == -1)
            {
                Player.ColorScheme = Player.CustomColorScheme;
                MyDoll.MyDoll.SetColorScheme(Player.ColorScheme);
            }
            else
            {
                Player.ColorSchemeIndex = CoreMath.Restrict(0, ColorSchemeManager.ColorSchemes.Count - 1, Player.ColorSchemeIndex);

                Player.ColorScheme = ColorSchemeManager.ColorSchemes[Player.ColorSchemeIndex];                
                MyDoll.MyDoll.SetColorScheme(Player.ColorScheme);
            }

            // Make sure indices match up to the color scheme.
            CopyIndicesFromColorScheme();
        }

        Predicate<MenuListItem> Match(ClrTextFx obj)
        {
            return _match => ((ClrTextFx)_match.obj).Guid == obj.Guid;
        }

        /// <summary>
        /// Find the indices that would reproduce the current color scheme.
        /// </summary>
        void CopyIndicesFromColorScheme()
        {
            ItemIndex[0] = ItemList[0].IndexOf(Match(Player.ColorScheme.SkinColor));
            ItemIndex[1] = ColorSchemeManager.BeardInfo.IndexOf(Player.ColorScheme.BeardData);
            ItemIndex[2] = ColorSchemeManager.HatInfo.IndexOf(Player.ColorScheme.HatData);
            ItemIndex[3] = ItemList[3].IndexOf(Match(Player.ColorScheme.CapeColor));
            ItemIndex[4] = ItemList[4].IndexOf(Match(Player.ColorScheme.CapeOutlineColor));

            for (int i = 0; i <= 4; i++)
                if (ItemIndex[i] < 0)
                    ItemIndex[i] = 0;
        }

        public void PhsxStep()
        {
        }

        public void Draw()
        {
        }
    }
}