using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using CloudberryKingdom.Coins;

namespace CloudberryKingdom
{
    public class Campaign_Chaos : WorldMap
    {
        public static bool WatchedOnce = false;

        GUI_Panel Panel;

        ChaosBackground Chaos;

        public Campaign_Chaos(EzSong song)
            : base(false)
        {
            Campaign_Chaos.WatchedOnce = true;

            Data = Campaign.Data;
            WorldName = "Chaos";

            Init(null);

            MyLevel.EraseNonDoodadCoins();

            MakeCenteredCamZone(.85f, null, null);
            MakeBackground(BackgroundType.Chaos);
            Chaos = MyLevel.MyBackground as ChaosBackground;

            if (song != null)
                WaitThenDo(45, () => Tools.SongWad.LoopSong(song));

            MyLevel.PreventReset = true;
            //MyLevel.PreventReset = false;

            Cinematic();

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());
        }

        void SetTextProperties(EzText text)
        {
            text.Scale = .735f;

            if (style == EzText.Style.Normal)
            {
                text.Alpha = 0;
                text.AlphaVel = .0145f;
            }
            else
            {
            }

            Panel.MyPile.Add(text);
        }

        EzText text1, text2, text3;

        //EzText.Style style = EzText.Style.FadingOff; int MarkShift = -112;
        EzText.Style style = EzText.Style.Normal; int MarkShift = -88;
        void FirstText()
        {
            string str = string.Format("In the {0} was the void...", EzText.ColorToMarkup(CampaignMenu.DifficultyColor[2], MarkShift, "beginning"));
            var text = new EzText(str, Tools.Font_DylanThin42, style);
            text.Pos = new Vector2(-1300.654f, 392.157f);
            SetTextProperties(text);

            text1 = text;

            if (style == EzText.Style.Normal)
                CinematicToDo(123, (Action)SecondText);
            else
                CinematicToDo(160, (Action)SecondText);
        }
        void SecondText()
        {
            string str = string.Format("...from it spawned {0}.", EzText.ColorToMarkup(new Color(205, 10, 10), MarkShift, "chaos"));
            var text = new EzText(str, Tools.Font_DylanThin42, style);
            text.Pos = new Vector2(-850.654f, 0.157f);
            SetTextProperties(text);

            text2 = text;

            CinematicToDo(165, (Action)FadeTexts);
        }

        float AlphaVel = -.05f;
        void FadeTexts()
        {
            text1.KillBitByBit(1); text1.AlphaVel = AlphaVel;
            text2.KillBitByBit(1.75f); text2.AlphaVel = AlphaVel;

            CinematicToDo(61, (Action)Shake);
            WaitThenAddToToDo(100 + 43, Explosion);
        }

        void Shake()
        {
            Chaos.AllowShake = true;
            Tools.CurCamera.StartShake(.5f, 36);
        }

        bool Explosion()
        {
            int step = GameData.CurItemStep;
            ParticleEffects.PieceExplosion(MyLevel, Cam.Pos, step, 1);
            if (step == 0) Tools.Sound("Piece Explosion").Play();

            if (step == 30) Chaos.CamMod = 0.01f;
            if (step > 40)
            {
                Chaos.StarsOnly = false;
                Chaos.CamMod +=
                    //.01f;
                        .045f;
                Tools.Restrict(0, 1, ref Chaos.CamMod);
            }

            //if (step > 173) return true;
            if (step > 73)
            {
                CinematicToDo(215, (Action)PostExplosionText);
                return true;
            }
            else return false;
        }

        void PostExplosionText()
        {
            string str;
            if (style == EzText.Style.Normal)
                str = string.Format("In the chaos", EzText.ColorToMarkup(CampaignMenu.DifficultyColor[5], MarkShift, "void"));
            else
                str = string.Format("In the chaos\n    the {0} found beauty,", EzText.ColorToMarkup(CampaignMenu.DifficultyColor[5], MarkShift, "void"));
                

            var text = new EzText(str, Tools.Font_DylanThin42, style);
            text.Pos = new Vector2(-1300.654f, 392.157f);
            SetTextProperties(text);

            text1 = text;

            if (style == EzText.Style.Normal)
                CinematicToDo(70, (Action)PostExplosionText2);
            else
                CinematicToDo(160, (Action)PostExplosionText3);
        }

        void PostExplosionText2()
        {
            string str = string.Format("     the {0} found beauty,", EzText.ColorToMarkup(CampaignMenu.DifficultyColor[5], MarkShift, "void"));
            var text = new EzText(str, Tools.Font_DylanThin42, style);
            text.Pos = new Vector2(-1300.654f, 200);
            SetTextProperties(text);

            text2 = text;

            CinematicToDo(27, (Action)BringPrincess);

            if (style == EzText.Style.Normal)
                CinematicToDo(113, (Action)PostExplosionText3);
            else
                CinematicToDo(145, (Action)PostExplosionText3);
        }

        void PostExplosionText3()
        {
            string str = string.Format("a {0}.", EzText.ColorToMarkup(Color.HotPink, MarkShift, "princess"));
            var text = new EzText(str, Tools.Font_DylanThin42, style);
            text.Pos = new Vector2(-850.654f, 0.157f);
            SetTextProperties(text);

            text3 = text;

            int wait = 110;// 165;
            //CinematicToDo(wait, (Action)BringPrincess);
            CinematicToDo(wait + 20, (Action)FadeTexts2);
        }

        PrincessBubble princess;
        void BringPrincess()
        {
            princess = new PrincessBubble(Cam.Pos + new Vector2(2300, 0));
            princess.Core.RemoveOnReset = true;
            MyLevel.AddObject(princess);
            princess.RotateSpeed = .25f;
            princess.Core.Data.Velocity = new Vector2(-4.5f, 0);
            princess.MyState = PrincessBubble.State.Integrate;

            AddToDo((Func<bool>)PrincessPhsx);
        }

        void FadeTexts2()
        {
            text1.KillBitByBit(1); text1.AlphaVel = AlphaVel;
            text2.KillBitByBit(1.5f); text2.AlphaVel = AlphaVel;
            text3.KillBitByBit(2f); text3.AlphaVel = AlphaVel;
        }

        int PrincessCount = 0;
        bool PrincessPhsx()
        {
            if (princess == null || princess.Core.Released) return true;

            if (princess.Pos.X > Cam.Pos.X + 680)
            {
                PrincessCount = 0;
                return false;
            }
            else
            {
                princess.Core.Data.Velocity *= .985f;
                //princess.RotateSpeed *= .98f;
                PrincessCount++;

                if (PrincessCount == 1)
                    CinematicToDo(3, (Action)AloneText1);

                if (Math.Abs(princess.Core.Data.Velocity.X) < .1f)
                {
                    princess.Core.Data.Velocity.X = 0;
                    return true;
                }
                else return false;
            }
        }

        void AloneText1()
        {
            string str = string.Format("But the beauty was alone...");
            var text = new EzText(str, Tools.Font_DylanThin42, style);
            text.Pos = new Vector2(-1300.654f, 392.157f);
            SetTextProperties(text);

            text1 = text;

            if (style == EzText.Style.Normal)
                CinematicToDo(123, (Action)AloneText2);
            else
                CinematicToDo(160, (Action)AloneText2);
        }
        void AloneText2()
        {
            string str = string.Format("there was no one to {0} her.", EzText.ColorToMarkup(Color.HotPink, MarkShift, "love"));
            var text = new EzText(str, Tools.Font_DylanThin42, style);
            text.Pos = new Vector2(-850.654f, 0.157f);
            SetTextProperties(text);

            text2 = text;

            CinematicToDo(133, (Action)FadeAloneTexts);
        }

        void FadeAloneTexts()
        {
            text1.KillBitByBit(1); text1.AlphaVel = AlphaVel;
            text2.KillBitByBit(1.75f); text2.AlphaVel = AlphaVel;

            CinematicToDo(80, (Action)BlobText1);
        }

        void BlobText1()
        {
            string str = string.Format("So the void made the {0},", EzText.ColorToMarkup(CampaignMenu.DifficultyColor[2], MarkShift, "blobs"));
            var text = new EzText(str, Tools.Font_DylanThin42, style);
            text.Pos = new Vector2(-1300.654f, 392.157f);
            SetTextProperties(text);

            text1 = text;

            CinematicToDo(7, (Action)BringBlobs);
        }
        void BringBlobs()
        {
            Campaign.LevelWithPrincess(MyLevel, true, Campaign.PrincessPos.CenterToRight, false);
            CinematicToDo(110, (Action)BlobText2);
        }
        void BlobText2()
        {
            string str = string.Format("to kidnap her.");
            var text = new EzText(str, Tools.Font_DylanThin42, style);
            text.Pos = new Vector2(-850.654f, 0.157f);
            SetTextProperties(text);

            text2 = text;

            CinematicToDo(165, (Action)FadeBlobTexts);
        }
        void FadeBlobTexts()
        {
            text1.KillBitByBit(1); text1.AlphaVel = AlphaVel;
            text2.KillBitByBit(1.75f); text2.AlphaVel = AlphaVel;

            CinematicToDo(70, (Action)YouText1);
        }


        void YouText1()
        {
            string str = string.Format("and the void made a {0}...", EzText.ColorToMarkup(CampaignMenu.DifficultyColor[1], MarkShift, "hero"));
            var text = new EzText(str, Tools.Font_DylanThin42, style);
            text.Pos = new Vector2(-1300.654f, 392.157f);
            SetTextProperties(text);

            text1 = text;

            CinematicToDo(125, (Action)BringStickman);

            if (style == EzText.Style.Normal)
                CinematicToDo(141, (Action)YouText2);
            else
                CinematicToDo(160, (Action)YouText2);
        }
        void YouText2()
        {
            string str = string.Format("to save her.");
            var text = new EzText(str, Tools.Font_DylanThin42, style);
            text.Pos = new Vector2(-850.654f, 0.157f);
            SetTextProperties(text);

            text2 = text;

            //CinematicToDo(5, (Action)BringStickman);
            CinematicToDo(145, (Action)FadeYouTexts);
            CinematicToDo(255, (Action)ExistenceText);
        }
        void FadeYouTexts()
        {
            text1.KillBitByBit(1); text1.AlphaVel = AlphaVel;
            text2.KillBitByBit(1.75f); text2.AlphaVel = AlphaVel;
        }

        void BringStickman()
        {
            PrincessBubble stickman = new PrincessBubble(Cam.Pos + new Vector2(-2300, 0));
            stickman.Core.RemoveOnReset = true;
            stickman.MyPile.Clear();
            var quad = new QuadClass("Score\\stickman", 300, true);
            stickman.MyPile.Add(quad);
            stickman.RotateSpeed = -.25f;
            stickman.Core.Data.Velocity = new Vector2(4.5f, 0);
            stickman.MyState = PrincessBubble.State.Integrate;
            MyLevel.AddObject(stickman);
        }

        void MakeText(string str, Vector2 pos, int text_num, int delay)
        {
            CinematicToDo(delay, () =>
            {
                var text = new EzText(str, Tools.Font_DylanThin42, style);
                text.Pos = pos;
                SetTextProperties(text);

                switch (text_num)
                {
                    case 1: text1 = text; break;
                    case 2: text2 = text; break;
                    case 3: text3 = text; break;
                }
            });
        }
        void FadeText()
        {
            if (text1 != null && text1.Alpha > 0.1f) { text1.KillBitByBit(1); text1.AlphaVel = AlphaVel; }
            if (text2 != null && text2.Alpha > 0.1f) { text2.KillBitByBit(1.5f); text2.AlphaVel = AlphaVel; }
            if (text3 != null && text3.Alpha > 0.1f) { text3.KillBitByBit(2f); text3.AlphaVel = AlphaVel; }
        }

        void ExistenceText()
        {
            MakeText(string.Format("among the {0} worlds\n      of the multiverse,", EzText.ColorToMarkup(CampaignMenu.DifficultyColor[2], MarkShift, "infinite")),
                     new Vector2(-1300.654f, 392.157f), 1, 0);
            MakeText("this same story must repeat...",
                     new Vector2(-850.654f, 0.157f), 2, 111);

            int fade = 251;
            CinematicToDo(fade, 
                (Action)FadeText);

            MakeText(string.Format("until true love is found.", EzText.ColorToMarkup(CampaignMenu.DifficultyColor[2], MarkShift, "infinite")),
                     new Vector2(-1050.654f, 200f), 1, fade + 103);

            CinematicToDo(fade + 40 + 103 + 140, (Action)FadeText);
            CinematicToDo(fade + 40 + 103 + 140 + 110, (Action)FadeToBlack);
            CinematicToDo(fade + 40 + 103 + 140 + 110 + 115, (Action)ReturnToWorldMap_Immediate);
        }
        
        

        void Cinematic()
        {
            Chaos.StarsOnly = true;

            //style = EzText.Style.Normal;

            Panel = new GUI_Panel();
            Panel.MyPile = new DrawPile();
            Panel.MyPile.FancyPos.UpdateWithGame = true;
            AddGameObject(Panel);

            //WaitThenAddToToDo(10, Explosion);
            FadeIn(.02f); CinematicToDo(80, (Action)FirstText);
            //CinematicToDo(10, (Action)PostExplosionText);
            //CinematicToDo(10, (Action)BringStickman);
        }

        public override void AdditionalReset()
        {
            base.AdditionalReset();

            Panel.Release();
            AddToDo((Action)Cinematic);
            //Cinematic();
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            // Remove standard GUI
            foreach (GameObject obj in MyGameObjects)
                if (obj is GUI_CampaignLevel || obj is GUI_CampaignScore)
                    if (!obj.Core.Released)
                        obj.ForceRelease();

            // Skip on 'Enter'
            if (PhsxCount == 20)
            {
                Listener PressA_Listener = null;
                PressA_Listener = new Listener(ControllerButtons.A, () =>
                     {
                         FadeToBlack(.0275f);
                         Tools.SongWad.FadeOut();

                         WaitThenDo(55, (Action)ReturnToWorldMap_Immediate);

                         PressA_Listener.Release();
                     });
                PressA_Listener.PreventRelease = true;
                PressA_Listener.Control = -2;
                AddGameObject(PressA_Listener);
            }
        }
    }
}