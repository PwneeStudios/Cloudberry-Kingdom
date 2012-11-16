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

namespace CloudberryKingdom
{
    public static class Localization
    {
        public enum Words { Identifier, PressStart, PressAnyKey, TheArcade, StoryMode, FreePlay, Options, Exit, ExitGame, Yes, No, Language, SoundVolume, MusicVolume, Controls, EditControls, Resolution, FullScreen, WindowBorder, On, Off, QuickSpawn, PowerUpMenu, Left, Right, Up, Down, ReplayPrev, ReplayNext, ReplayToggle, ToggleSlowMo, Accept, Back, Menu, Reset, PressToJoin, Custom, Random, Done, Color, Beard, Hat, Cape, Lining, Use, Cancel, White, Silver, Gray, Black, Cyan, Blue, Teal, Indigo, Purple, Brown, Red, HotPink, Orange, Gold, Yellow, Green, LimeGreen, ForestGreen, Ninja, BrightWhite, Clear, Rugged, Manhattan, Lumberjack, Goatee, Vandyke, None, Viking, Fedora, Afro, Halo, Firehead, Horns, Bubble, TopHat, KnightHelmet, MushroomHat, OmNomNom, BrainHat, Gosu, RobinHood, Reggae, Pumpkin, PirateHat, HardHat, FourEyes, BunnyEars, Antlers, ArrowThroughHead, BrownBag, TrafficCone, PopeHat, RiceHat, SantaClaus, Sombrero, TikiMask, Wizard, Location, Game, ClassicGame, WallLevel, Bungee, Hero, Difficulty, Training, Unpleasant, Abusive, Hardcore, Masochistic, Length, Checkpoints, Start, Continue, LoadLevel, HeroFactory, Base, Jump, Shape, Play, Test, Passive, Aggressive, Next, JumpDifficulty, LevelSpeed, Ceilings, MovingBlocks, GhostBlocks, FallingBlocks, Elevators, Clouds, BouncyBlocks, Pendulums, FlyingBlobs, Firespinners, Boulders, SpikeyGuys, Lasers, Spikes, Sludge, Serpent, SpikeyLines, Fireballs, Acceleration, MaxVelocity, Size, Gravity, MaxFallSpeed, Friction, JumpLength, JumpAcc, NumJumps, DoubleJumpLength, DoubleJumpAccel, JetpackAcc, JetpackFuel, PhasedSize, PhasedGravity, PhasePeriod, ClassicHero, Jetman, DoubleJump, TinyBob, Wheelie, Spaceship, HeroInABox, Bouncy, Rocketbox, FatBob, PhaseBob, Viridian, TimeMaster, Meatboy, JetpackWheelie, Factory, Sea, Hills, Forest, Cloud, Cave, Castle, TheBeginning, TheNextNinetyNine, AGauntletOfDoom, AlmostHero, TheMasochist, Escalation, TimeCrisis, HeroRush, HybridRush, HighScore, BestLevel, Leaderboard, Score, Level, Ready, Go, GetToTheExit, SecondsOnTheClock, CoinsAddSeconds, GetAHighScore, Perfect, ExtraLife, Press, Resume, Statistics, SaveLoad, RemoveMe, ExitLevel, RandomSeed, SaveSeed, LoadSeed, CopyToClipboard, LoadFromClipboard, RemovePlayerQuestion, ExitLevelQuestion, SaveRandomSeedAs, SavedSeeds, LoadTheFollowingSeedQuestion, SeedSavedSuccessfully, Hooray, NoNameGiven, Oh, LevelsBeat, Jumps, Coins, Grabbed, CoinsOutOf, Percent, AverageLife, Deaths, Fireball, Firespinner, Boulder, SpikeyGuy, Spike, Falling, Lava, Blob, Laser, FallingSpikey, TimeLimit, LeftBehind, Other, Total, GameOver, NewHighScore, PlayAgain, HighScores, Local, Global, WatchComputer, ShowPath, ActivateSlowMo, Paused, Speed, Step, LevelCleared, KeepSettings, WatchReplay, BackToFreeplay, Pause, Single, Previous, All, End, Loading, HeroUnlocked, NewHeroUnlocked, Buy, HatsForSale, Bank, Achievement, AchievementUnlocked, DeleteSeeds, DeleteSeedsPlural, MultiplierIncreased, JumpHigherNote, RespawnNoteGamepad, RespawnNoteKeyboard, PowerupNote };

        static Dictionary<Language, Dictionary<Words, string>> Text;

        private static void ReadTranslationGrid(string path)
        {
            if (!File.Exists(path)) return;

            // Create new dictionaries for each language
            Text = new Dictionary<Language, Dictionary<Words, string>>();
            for (int i = 0; i < NumLanguages; i++)
                Text.Add((Language)i, new Dictionary<Words, string>());

            // Open the giant translation file
            Tools.UseInvariantCulture();
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader reader = new StreamReader(stream);

            String line;

            // Read the file, adding translations to the corresponding dictionaries
            line = reader.ReadLine();
            int LineCount = 0;

            while (line != null)
            {
                //var bits = line.Split('|');
                var bits = line.Split('\t');

                for (int i = 0; i < NumLanguages; i++)
                    Text[(Language)i].Add((Words)LineCount, bits[i + 2]);

                line = reader.ReadLine();
                LineCount++;
            }

            reader.Close();
            stream.Close();
        }

        public static string WordString(Words Word)
        {
            //return Text[Language.Portuguese][Word];
            return Text[CurrentLanguage.MyLanguage][Word];
            //return "boob";
        }

        public static string WordToTextureName(Words Word)
        {
            return "white";
        }

        public static string WordMarkup(Words Word)
        {
            int Size = 80;
            return string.Format("{{p{1},?,{0}}}", Size, WordToTextureName(Word));
        }

        public static string WordMarkup(Words Word, int Size)
        {
            return string.Format("{{p{1},{0},?}}", Size, WordToTextureName(Word));
        }

        public const int NumLanguages = 10;
        public enum Language { English, Japanese, German, Portuguese, Italian, French, Spanish, Russian, Korean, Chinese };

        public class LanguageInfo
        {
            public Language MyLanguage;
            public string MyDirectory;

            public LanguageInfo(Language MyLanguage, string MyDirectory)
            {
                this.MyLanguage = MyLanguage;
                this.MyDirectory = MyDirectory;
            }
        }

        public static Dictionary<Language, LanguageInfo> Languages = new Dictionary<Language, LanguageInfo>(NumLanguages);

        static ContentManager Content = null;
        
        public static LanguageInfo CurrentLanguage;

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

            string path = Path.Combine(Content.RootDirectory, "Localization.tsv");
            ReadTranslationGrid(path);
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