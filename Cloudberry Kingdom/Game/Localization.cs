using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using CoreEngine;

namespace CloudberryKingdom
{
    public static class Localization
    {
        public enum Words { PressStart, PressAnyKey, TheArcade, StoryMode, FreePlay, Options, Exit, ExitGame, Yes, No, Language, SoundVolume, MusicVolume, Controls, EditControls, Resolution, FullScreen, WindowBorder, On, Off, QuickSpawn, PowerUpMenu, Left, Right, Up, Down, ReplayPrev, ReplayNext, ReplayToggle, ToggleSlowMo, Accept, Back, Menu, Reset, PressToJoin, Custom, Random, Done, Color, Beard, Hat, Cape, Lining, Use, Cancel, White, Silver, Gray, Black, Cyan, Blue, Teal, Indigo, Purple, Brown, Red, HotPink, Orange, Gold, Yellow, Green, LimeGreen, ForestGreen, Ninja, BrightWhite, Clear, Rugged, Manhattan, Lumberjack, Goatee, Vandyke, None, Viking, Fedora, Afro, Halo, Firehead, Horns, Bubble, TopHat, KnightHelmet, MushroomHat, OmNomNom, BrainHat, Gosu, RobinHood, Reggae, Pumpkin, PirateHat, HardHat, FourEyes, BunnyEars, Antlers, ArrowThroughHead, BrownBag, TrafficCone, PopeHat, RiceHat, SantaClaus, Sombrero, TikiMask, Wizard, Location, Game, Classic, WallLevel, Hero, Factory, Difficulty, Training, Unpleasant, Abusive, Hardcore, Masochistic, Length, Checkpoints, Start, Continue, LoadLevel, HeroFactory, Base, Jump, Size, Play, Test, Passive, Aggressive, Next, JumpDifficulty, LevelSpeed, Ceilings, MovingBlocks, GhostBlocks, FallingBlocks, Elevators, Clouds, BouncyBlocks, Pendulums, FlyingBlobs, Firespinners, Boulders, SpikeyGuys, Lasers, Spikes, Sludge, Serpent, SpikeyLines, Fireballs, Acceleration, MaxVelocity, Size, Gravity, MaxFallSpeed, Friction, JumpLength, JumpAcc, NumJumps, DoubleJumpLength, DoubleJumpAccel, JetpackAcc, JetpackFuel, PhasedSize, PhasedGravity, PhasePeriod, Classic, Jetman, DoubleJump, TinyBob, Wheelie, Spaceship, HeroInABox, Bouncy, Rocketbox, FatBob, PhaseBob, Viridian, TimeMaster, Meatboy, JetpackWheelie, Factory, Sea, Hills, Forest, Cloud, Cave, Castle, TheBeginning, TheNextNinetyNine, AGauntletOfDoom, AlmostHero, TheMasochist, Escalation, TimeCrisis, HeroRush, HybridRush, HighScore, BestLevel, Leaderboard, Score, Level, Ready, Go, GetToTheExit, SecondsOnTheClock, CoinsAddSeconds, GetAHighScore, Perfect, ExtraLife, Press, Resume, Statistics, SaveLoad, RemoveMe, ExitLevel, RandomSeed, SaveSeed, LoadSeed, CopyToClipboard, LoadFromClipboard, RemovePlayerQuestion, ExitLevelQuestion, SaveRandomSeedAs, SavedSeeds, LoadTheFollowingSeedQuestion, SeedSavedSuccessfully, Hooray, LevelsBeat, Jumps, Coins, Grabbed, CoinsOutOf, Percent, Blobs, Checkpoints, AverageLife, Deaths, Fireball, Firespinner, Boulder, SpikeyGuy, Spike, Falling, Lava, Blob, Laser, FallingSpikey, TimeLimit, LeftBehind, Other, Total, GameOver, Level, Score, NewHighScore, PlayAgain, HighScores, Local, Global, WatchComputer, ShowPath, ActivateSlowMo, Paused, Speed, Step, LevelCleared, KeepSettings, WatchReplay, BackToFreeplay, Pause, Single, Previous, All, End, Loading, HeroUnlocked, NewHeroUnlocked };

        public enum Language { English, Spanish, Portuguese, German, Italian, French, Chinese, Korean, Japanese, Russian };
        public class LanguageInfo
        {
            Language MyLanguage;
            public string MyDirectory;

            public LanguageInfo(Language MyLanguage, string MyDirectory)
            {
                this.MyLanguage = MyLanguage;
                this.MyDirectory = MyDirectory;
            }
        }

        public static Dictionary<Language, LanguageInfo> Languages = new Dictionary<Language, LanguageInfo>(Tools.Length<Language>());

        static ContentManager Content = null;
        
        public static LanguageInfo CurrentLanguage;
        static string LanguageDir;

        public static void SetLanguage(Language SelectedLanguage)
        {
            if (Content == null)
            {
                Initialize();
            }
            else
            {
                Content.Unload();
            }

            CurrentLanguage = Languages[SelectedLanguage];

            String path = Path.Combine(Content.RootDirectory, "Subtitles", CurrentLanguage.MyDirectory);
            string[] files = Tools.GetFiles(path, false);

            foreach (String file in files)
            {
                if (Tools.GetFileExt(path, file) == "xnb")
                {
                    //var texture = Tools.TextureWad.AddTexture(null, Tools.GetFileName(path, file));
                    var texture = Tools.TextureWad.AddTexture(null, Tools.GetFileName("Content", file));
                    texture.Load();
                }
            }
        }

        private static void Initialize()
        {
            Content = new ContentManager(Tools.GameClass.Services, Path.Combine("Content", "Localization"));

            Languages.Add(Language.Chinese, new LanguageInfo(Language.Chinese, "Chinese"));
            Languages.Add(Language.English, new LanguageInfo(Language.English, "English"));
            Languages.Add(Language.French, new LanguageInfo(Language.French, "French"));
            Languages.Add(Language.German, new LanguageInfo(Language.German, "German"));
            Languages.Add(Language.Italian, new LanguageInfo(Language.Italian, "Italian"));
            Languages.Add(Language.Japanese, new LanguageInfo(Language.Japanese, "Japanese"));
            Languages.Add(Language.Korean, new LanguageInfo(Language.Korean, "Korean"));
            Languages.Add(Language.Portuguese, new LanguageInfo(Language.Portuguese, "Portuguese"));
            Languages.Add(Language.Russian, new LanguageInfo(Language.Portuguese, "Russian"));
            Languages.Add(Language.Spanish, new LanguageInfo(Language.Spanish, "English"));
        }

        private static void ReadSubtitleInfo(string VideoName)
        {
            string path = Path.Combine("Content", "Localization", "Subtitles", VideoName) + ".txt";

            ReadSubtitles(path);
        }

        public class SubtitleAction
        {
            /// <summary>
            /// The time the action happens, in seconds.
            /// </summary>
            public float Time;

            public enum ActionType { Show, Hide };
            public ActionType MyAction;

            public EzTexture MyTexture;

            public SubtitleAction(ActionType MyAction, float Time, EzTexture MyTexture)
            {
                this.MyAction = MyAction;
                this.Time = Time;
                this.MyTexture = MyTexture;
            }
        }

        public static List<SubtitleAction> GetSubtitles(string VideoName)
        {
            Subtitles = null;
            ReadSubtitleInfo(VideoName);

            return Subtitles;
        }

        static List<SubtitleAction> Subtitles;

        private static void ReadSubtitles(string path)
        {
            if (!File.Exists(path)) return;

            Subtitles = new List<SubtitleAction>(50);

            Tools.UseInvariantCulture();
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader reader = new StreamReader(stream);

            String line;

            int Index = 0;

            line = reader.ReadLine();
            while (line != null)
            {
                line = Tools.RemoveComment_DashStyle(line);
                if (line == null || line.Length <= 1)
                {
                    line = reader.ReadLine();
                    continue;
                }

                int space = line.IndexOf(' ');
                string identifier, data;
                if (space > 0)
                {
                    identifier = line.Substring(0, space);
                    data = line.Substring(space + 1);
                }
                else
                {
                    identifier = line;
                    data = "";
                }

                switch (identifier)
                {
                    case "show":
                        var SubtitleTexture = Tools.Texture(string.Format("Chunk_{0}", Index));
                        Subtitles.Add(new SubtitleAction(SubtitleAction.ActionType.Show, float.Parse(data), SubtitleTexture));
                        
                        Index++;

                        break;

                    case "hide":
                        Subtitles.Add(new SubtitleAction(SubtitleAction.ActionType.Hide, float.Parse(data), null));
                        break;
                }

                line = reader.ReadLine();
            }

            reader.Close();
            stream.Close();
        }
    }
}