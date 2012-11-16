using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif




namespace CloudberryKingdom
{
    public class CharacterSelectManager : ViewReadWrite
    {
        public static Lambda OnBack, OnDone;

        public static GUI_Panel ParentPanel;

        public override string[] GetViewables()
        {
            return new string[] { };
        }

        static readonly CharacterSelectManager instance = new CharacterSelectManager();
        public static CharacterSelectManager Instance { get { return instance; } }

        public static List<CharacterSelect> CharSelect = new List<CharacterSelect>(new CharacterSelect[] { null, null, null, null });
        public static bool IsShowing = false;

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

#if XBOX || XBOX_SIGNIN
            SignedInGamer.SignedIn += new System.EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
#endif
        }

#if XBOX || XBOX_SIGNIN
        void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            if (CharSelect == null) return;

            UpdateAvailableHats();

            // Get the signed in player's saved color scheme and set the character select accordingly
            Tools.CurGameData.AddToDo(new SignInGamerLambda(e));
        }

        class SignInGamerLambda : Lambda
        {
            SignedInEventArgs e;
            public SignInGamerLambda(SignedInEventArgs e)
            {
                this.e = e;
            }

            public void Apply()
            {
                int index = (int)e.Gamer.PlayerIndex;
                CharacterSelect select = CharSelect[index];
                if (select != null)
                {
                    select.InitColorScheme(index);
                }
            }
        }
#endif

        public static Set<Hat> AvailableHats;
        public static void UpdateAvailableHats()
        {
            UpdateAvailableBeards();

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

        public static Set<Hat> AvailableBeards;
        static void UpdateAvailableBeards()
        {
            // Determine which Beards are availabe
            AvailableBeards = new Set<Hat>();
            foreach (Hat Beard in ColorSchemeManager.BeardInfo)
                if (Beard == Hat.None
                    ||
                    Beard.AssociatedAward == null && PlayerManager.Bought(Beard)
                    ||
                    Beard.AssociatedAward != null && PlayerManager.Awarded(Beard.AssociatedAward)
                    ||
                    CloudberryKingdomGame.UnlockAll)
                    AvailableBeards += Beard;
        }

        static CharSelectBackdrop Backdrop;
        public static void Start(GUI_Panel Parent)
        {
            ParentPanel = Parent;
            
            // Add the backdrop
            Backdrop = new CharSelectBackdrop();
            Parent.MyGame.AddGameObject(Backdrop);

            // Start the selects for each player
            Parent.MyGame.WaitThenDo(0, new _StartAllProxy(), "StartCharSelect");
        }

        class _StartAllProxy : Lambda
        {
            public void Apply()
            {
                CharacterSelectManager._StartAll();
            }
        }

        static void _StartAll()
        {
            for (int i = 0; i < 4; i++)
                Start(i, false);
        }

        static void Start(int PlayerIndex, bool QuickJoin)
        {
            Active = true;
            IsShowing = true;
            UpdateAvailableHats();

            // If the character select is already up just return
            if (CharSelect[PlayerIndex] != null)
                return;

            CharSelect[PlayerIndex] = new CharacterSelect(PlayerIndex, QuickJoin);
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
        }

        static void EndCharSelect(int DelayOnReturn, int DelayKillCharSelect)
        {
            Cleanup();

            if (CharacterSelectManager.ParentPanel != null)
                CharacterSelectManager.ParentPanel.Show();
        }

        static void Cleanup()
        {
            var game = Tools.CurGameData;

            CharacterSelectManager.FinishAll();

            game.KillToDo("StartCharSelect");

            game.RemoveGameObjects(GameObject.Tag.CharSelect);
            Backdrop.Release();
        }

        static bool AllExited()
        {
            bool All = true;
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null && CharSelect[i].MyState != CharacterSelect.SelectState.Beginning)
                    All = false;
            return All;
        }

        static bool AllFinished()
        {
            // False if no one has joined
            bool SomeOneIsHere = false;
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null &&
                    CharSelect[i].MyState != CharacterSelect.SelectState.Beginning)
                    SomeOneIsHere = true;
            
            if (!SomeOneIsHere)
                return false;

            // Otherwise true if no one is still selecting
            bool All = true;
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null &&
                    CharSelect[i].MyState == CharacterSelect.SelectState.Selecting)
                    All = false;
            return All;
        }

        static bool AllNull()
        {
            return Tools.All(CharSelect, NullLambda_static);
        }

        static NullLambda NullLambda_static = new NullLambda();
        class NullLambda : LambdaFunc_1<CharacterSelect, bool>
        {
            public NullLambda()
            {
            }

            public bool Apply(CharacterSelect select)
            {
                return select == null;
            }
        }

        public static int DrawLayer = 10;
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
            
            // Draw the stickmen
            float setzoom = 0;
            for (int i = 0; i < 4; i++)
                if (CharSelect[i] != null)
                {
                    float _zoom = BobZoom;

                    if (setzoom != _zoom)
                    {
                        Tools.QDrawer.Flush();
                        cam.SetVertexZoom(new Vector2(_zoom));
                        setzoom = BobZoom;
                    }
                    CharSelect[i].MyDoll.DrawBob();
                }
            Tools.QDrawer.Flush();
            cam.SetVertexCamera();

            cam.Zoom = HoldZoom;
            cam.SetVertexCamera();
        }

        class AfterFinishedHelper : Lambda
        {
            public void Apply()
            {
                CharacterSelectManager.AfterFinished();
            }
        }

        static void AfterFinished()
        {
            IsShowing = false;
            
            Cleanup();
            if (OnDone != null) OnDone.Apply();
        }

        static bool Active = false;
        public static void PhsxStep()
        {
            if (!Active) return;

            if (!IsShowing)
                return;

            if (AllNull())
                IsShowing = false;

            // Check for finishing from character selection
            if (AllFinished() && IsShowing)
            {
                Active = false;
                Tools.CurGameData.SlideOut_FadeIn(0, new AfterFinishedHelper());
            }

            // Check for ready to exit from character selection
            if (AllExited() && IsShowing)
            {
                if (ButtonCheck.State(ControllerButtons.B, -2).Pressed)
                {
                    Active = false;
                    EndCharSelect(0, 0);
                }
            }
        }
    }
}