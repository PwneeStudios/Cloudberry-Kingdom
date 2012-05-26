using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
#if PC_VERSION
#elif XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif
using Drawing;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class CharacterSelectManager : IViewable
    {
        public static GUI_Panel ParentPanel;

        public virtual string[] GetViewables()
        {
            return new string[] { };
        }

        static readonly CharacterSelectManager instance = new CharacterSelectManager();
        public static CharacterSelectManager Instance { get { return instance; } }

        public static List<CharacterSelect> CharSelect = new List<CharacterSelect>(new CharacterSelect[] { null, null, null, null });
        public static bool IsShowing = false;

        static int[] Delay = { 0, 0, 0, 0 };

        static FancyVector2 CamPos;
        public static EzText ChooseYourHero_Text;
        static bool Show_ChooseYourHero = false;

        static int ChooseYourHero_LerpLength = 100;

        static Vector2
            HidePos = new Vector2(0, 2520),
            ShowPos = new Vector2(50.79541f, 900.1587f);

        public CharacterSelectManager()
        {
            CamPos = new FancyVector2();

#if XBOX_SIGNIN
            SignedInGamer.SignedIn += new System.EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
#endif
        }

#if XBOX_SIGNIN
        void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            if (CharSelect == null) return;

            UpdateAvailableHats();

            // Get the signed in player's saved color scheme and set the character select accordingly
            Tools.CurGameData.AddToDo(() =>
            {
                int index = (int)e.Gamer.PlayerIndex;
                CharacterSelect select = CharSelect[index];
                if (select != null)
                {
                    select.InitColorScheme(index);
                }
            });
        }
#endif
        /// <summary>
        /// Initialize the "Choose your hero!" text
        /// </summary>
        static void InitText()
        {
            //string Text = "Transmogrify!";
            string Text = "Choose your hero";

            ChooseYourHero_Text = new EzText(Text, Tools.Font_DylanThin42, 2000, true, true);
            ChooseYourHero_Text.Scale = 1.25f;
            ChooseYourHero_Text.FancyPos = new FancyVector2(CamPos);
            ChooseYourHero_Text.FancyPos.RelVal = HidePos;

            ChooseHeroTextStyle(ChooseYourHero_Text);
        }

        /// <summary>
        /// Set the given text to the style used in to display "Choose your hero!"
        /// </summary>
        public static void ChooseHeroTextStyle(EzText text)
        {
            text.MyFloatColor = new Color(252, 131, 0).ToVector4();
            text.OutlineColor = new Color(255, 255, 255).ToVector4();

            text.Shadow = true;
            text.ShadowOffset = new Vector2(10.5f, 10.5f);
            text.ShadowColor = new Color(30, 30, 30);
        }

        public static void DelayQuickJoin(int PlayerIndex)
        {
            Delay[PlayerIndex] = 0;
        }

        public static Set<Hat> AvailableHats;
        public static void UpdateAvailableHats()
        {
            // Determine which hats are availabe
            AvailableHats = new Set<Hat>();
            foreach (Hat hat in ColorSchemeManager.HatInfo)
                if (hat == Hat.None
                    ||
                    hat.AssociatedAward == null && PlayerManager.Bought(hat)
                    ||
                    hat.AssociatedAward != null && PlayerManager.Awarded(hat.AssociatedAward)
                    ||
                    CloudberryKingdomGame.UnlockAll)
                    AvailableHats += hat;
        }

        public static void Start(int PlayerIndex, bool QuickJoin)
        {
            UpdateAvailableHats();

            // If the character select is already up just return
            if (CharSelect[PlayerIndex] != null || Delay[PlayerIndex] < 3)
                return;

            CharSelect[PlayerIndex] = new CharacterSelect(PlayerIndex, QuickJoin);
            
            IsShowing = true;
        }

        public static void SetToLeave()
        {
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null)
                    CharSelect[i].SetState(SelectState.Leaving);
        }

        public static void SlideOutAll()
        {
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null)
                    CharSelect[i].SetState(SelectState.Back);
        }

        public static void FinishAll()
        {
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null)
                    Finish(i, false);
        }

        public static void Finish(int PlayerIndex, bool Join)
        {
            if (Join)
            {
                Tools.CurGameData.CreateBob(PlayerIndex, true);
            }

            CharSelect[PlayerIndex].Release();
            CharSelect[PlayerIndex] = null;
            Delay[PlayerIndex] = 0;
        }

        public static bool AllExited()
        {
            bool All = true;
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null && CharSelect[i].MyState != SelectState.PressAtoJoin)
                    All = false;
            return All;
        }

        public static bool AllFinished()
        {
            // False if no one has joined
            if (CharSelect.All(select => select == null || select.MyState == SelectState.PressAtoJoin))
                return false;

            // Otherwise true if no one is still selecting
            bool All = true;
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null &&
                    CharSelect[i].MyState != SelectState.Done &&
                    CharSelect[i].MyState != SelectState.PressAtoJoin &&
                    CharSelect[i].MyState != SelectState.Waiting)
                    All = false;
            return All;
        }

        public static bool AllNull()
        {
            return CharSelect.All<CharacterSelect>(delegate(CharacterSelect select) { return select == null; });
        }

        public static float BobZoom = 2.6f;
        public static float ZoomMod = 1.1f;
        public static void Draw()
        {
            if (!IsShowing)
                return;

            Camera cam = Tools.CurLevel.MainCamera;
            Vector2 HoldZoom = cam.Zoom;
            cam.Zoom = new Vector2(.001f, .001f) / ZoomMod;
            cam.SetVertexCamera();

            CamPos.AbsVal = CamPos.RelVal = cam.Data.Position;
            
            // Draw the non-text of each character select panel
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null)
                    CharSelect[i].Draw();

            // Draw the header text
            if (ChooseYourHero_Text != null)
            {
                ChooseYourHero_Text.FancyPos.Update();
                ChooseYourHero_Text.Draw(Tools.CurLevel.MainCamera);
            }
            Tools.EndSpriteBatch();

            // Draw the stickmen
            cam.SetVertexZoom(new Vector2(BobZoom));
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null)
                    CharSelect[i].DrawBob();
            Tools.QDrawer.Flush();
            cam.SetVertexCamera();
            
            // Draw the text of each character select panel
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null)
                    CharSelect[i].DrawText();

            Tools.EndSpriteBatch();
            
            // Draw additional non-text of each character select panel
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null)
                    CharSelect[i].Draw2();
            
            Tools.QDrawer.Flush();

            cam.Zoom = HoldZoom;
            cam.SetVertexCamera();
        }

        public static void PhsxStep()
        {
            for (int i = 0; i < 4; i++)
                if (Delay[i] < 3)
                    Delay[i]++;

            if (!IsShowing)
                return;

            bool IsChoosing = false; // True if at least one player is choosing a stickmen
            for (int i = 0; i < 4; i++)
            {
                if (CharSelect[i] != null)
                {
                    CharSelect[i].PhsxStep();
                    if (CharSelect[i].MyState == SelectState.Done)
                        Finish(i, CharSelect[i].Join);
                    else
                    {
                        if (CharSelect[i].MyState == SelectState.SimpleSelect || CharSelect[i].MyState == SelectState.CustomizeSelect)
                            IsChoosing = true;
                    }
                }
            }

            if (IsChoosing && !Show_ChooseYourHero)
            {
                Show_ChooseYourHero = true;

                if (ChooseYourHero_Text == null)
                    InitText();
                ChooseYourHero_Text.FancyPos.LerpTo(ShowPos, ChooseYourHero_LerpLength);
            }
            if (!IsChoosing && Show_ChooseYourHero)
            {
                Show_ChooseYourHero = false;

                ChooseYourHero_Text.FancyPos.LerpTo(HidePos, 50);
            }

            if (AllNull())
                IsShowing = false;
        }
    }
}