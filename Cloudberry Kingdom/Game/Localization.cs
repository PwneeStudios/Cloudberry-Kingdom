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

using EasyStorage;

namespace CloudberryKingdom
{
    public static class Localization
    {
        static Dictionary<Language, Dictionary<Words, string>> Text;

		public static bool IsLoaded()
		{
			return Text != null;
		}

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
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            

            String line;

            // Read the file, adding translations to the corresponding dictionaries
            line = reader.ReadLine();
            int LineCount = 0;

            while (line != null)
            {
                line = line.Replace("\\n", "\n");
                var bits = line.Split('\t');

                for (int i = 0; i < NumLanguages; i++)
                    Text[(Language)i].Add((Words)LineCount, bits[i]);

                line = reader.ReadLine();
                LineCount++;
            }

            reader.Close();
            stream.Close();
        }

        public static string LanguageName(Language language)
        {
            return Text[language][Words.Identifier];
        }

        public static string WordString(Words Word)
        {
            return Text[CurrentLanguage.MyLanguage][Word];
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
            public string FontSuffix;

            public LanguageInfo(Language MyLanguage, string MyDirectory, string FontSuffix)
            {
                this.MyLanguage = MyLanguage;
                this.MyDirectory = MyDirectory;
                this.FontSuffix = FontSuffix;
            }
        }

        public static Dictionary<Language, LanguageInfo> Languages = new Dictionary<Language, LanguageInfo>(NumLanguages);

        static ContentManager Content = null;
        
        public static LanguageInfo CurrentLanguage;

        static void LoadFont()
        {
            string name = "Grobold_" + Localization.CurrentLanguage.FontSuffix;
            FontTexture = Content.Load<Texture2D>(Path.Combine("Fonts", name));

            Resources.hf = new HackFont(name);
            
            if (Resources.Font_Grobold42 != null)
            {
                Resources.Font_Grobold42.HFont.font = Resources.hf;
                Resources.Font_Grobold42.HOutlineFont.font = Resources.hf;
                Resources.Font_Grobold42_2.HFont.font = Resources.hf;
                Resources.Font_Grobold42_2.HOutlineFont.font = Resources.hf;
            }
        }

        public static Texture2D FontTexture;
        public static void SetLanguage(Language SelectedLanguage)
        {
			var HoldContent = Content;

            if (Content == null)
            {
				Content = new ContentManager(Tools.GameClass.Services, "Content");
                Initialize();
            }
            else
            {
				Content = new ContentManager(Tools.GameClass.Services, "Content");
				//Content.Unload();
            }

            CurrentLanguage = Languages[SelectedLanguage];

            // This loads the subtitle textures.
            //String path = Path.Combine(Content.RootDirectory, Path.Combine("Localization", Path.Combine("Subtitles", CurrentLanguage.MyDirectory)));
            //string[] files = Tools.GetFiles(path, false);

            // Load font. Lock first if it alread exists.
            if (Resources.hf == null)
            {
                LoadFont();
            }
            else
            {
                lock (Resources.hf)
                {
					//LoadFont();

					LoadFont();
					HoldContent.Unload();
					HoldContent = null;
                }
            }

            EasyStorageSettings.ResetSaveDeviceStrings();
        }

        public static Language IsoCodeToLanguage(string code)
        {
            switch (code)
            {
                case "en": return Language.English;
                case "fr": return Language.French;
                case "it": return Language.Italian;
                case "de": return Language.German;
                case "es": return Language.Spanish;
                case "pt": return Language.Portuguese;
                case "ko": return Language.Korean;
                case "zh": return Language.Chinese;
                case "ja": return Language.Japanese;
                case "ru": return Language.Russian;
                default: return Language.English;
            }
        }

        private static void Initialize()
        {
            Languages.Add(Language.Chinese, new LanguageInfo(Language.Chinese, "Chinese", "Chinese"));
            Languages.Add(Language.English, new LanguageInfo(Language.English, "English", "Western"));
            Languages.Add(Language.French, new LanguageInfo(Language.French, "French", "Western"));
            Languages.Add(Language.German, new LanguageInfo(Language.German, "German", "Western"));
            Languages.Add(Language.Italian, new LanguageInfo(Language.Italian, "Italian", "Western"));
            Languages.Add(Language.Japanese, new LanguageInfo(Language.Japanese, "Japanese", "Japanese"));
            Languages.Add(Language.Korean, new LanguageInfo(Language.Korean, "Korean", "Korean"));
            Languages.Add(Language.Portuguese, new LanguageInfo(Language.Portuguese, "Portuguese", "Western"));
            Languages.Add(Language.Russian, new LanguageInfo(Language.Russian, "Russian", "Western"));
            Languages.Add(Language.Spanish, new LanguageInfo(Language.Spanish, "Spanish", "Western"));

            string path = Path.Combine(Content.RootDirectory, Path.Combine("Localization", "Localization.tsv"));
            ReadTranslationGrid(path);
        }

        private static void ReadSubtitleInfo(string VideoName)
        {
            string path = Path.Combine("Content", Path.Combine("Localization", Path.Combine("Subtitles", Path.Combine(CurrentLanguage.MyDirectory, VideoName)))) + ".tsv";

            if (!File.Exists(path)) return;

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

            public string Text;

            public SubtitleAction(ActionType MyAction, float Time, string Text)
            {
                this.MyAction = MyAction;
                this.Time = Time;
                this.Text = Text == null ? null : Text.Replace("\\n", "\n");
            }
        }

        public static List<SubtitleAction> GetSubtitles(string VideoName)
        {
            Subtitles = null;
            ReadSubtitleInfo(VideoName);

            return Subtitles;
        }

        static List<SubtitleAction> Subtitles;

        static float ParseTime(string s)
        {
            var data = s.Split(':');

            float seconds = float.Parse(data[2]);
            float minutes = float.Parse(data[1]);

            return seconds + minutes * 60;
        }

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


                var data = line.Split('\t');

                if (data.Length <= 1) continue;

                switch (data[1])
                {
                    case "+":
                        if (data.Length < 3) continue;

                        Subtitles.Add(new SubtitleAction(SubtitleAction.ActionType.Show, ParseTime(data[0]), data[2]));
                        
                        Index++;

                        break;

                    case "-":
                        Subtitles.Add(new SubtitleAction(SubtitleAction.ActionType.Hide, ParseTime(data[0]), null));
                        break;
                }

                line = reader.ReadLine();
            }

            reader.Close();
            stream.Close();
        }

        public enum Words
        {
Identifier,
PressStart,
PressAnyKey,
TheArcade,
StoryMode,
FreePlay,
Options,
Exit,
ExitGame,
Yes,
No,
Language,
SoundVolume,
MusicVolume,
Controls,
EditControls,
Resolution,
FullScreen,
WindowBorder,
On,
Off,
QuickSpawn,
PowerUpMenu,
Left,
Right,
Up,
Down,
ReplayPrev,
ReplayNext,
ReplayToggle,
ToggleSlowMo,
Accept,
Back,
Menu,
Reset,
PressToJoin,
Custom,
Random,
Done,
Color,
Beard,
Hat,
Cape,
Lining,
Use,
Cancel,
White,
Silver,
Gray,
Black,
Cyan,
Blue,
Teal,
Indigo,
Purple,
Brown,
Red,
HotPink,
Orange,
Gold,
Yellow,
Green,
LimeGreen,
ForestGreen,
Ninja,
BrightWhite,
Clear,
Rugged,
Manhattan,
Lumberjack,
Goatee,
Vandyke,
None,
Viking,
Fedora,
Afro,
Halo,
Firehead,
Horns,
Bubble,
TopHat,
KnightHelmet,
OmNomNom,
BrainHat,
Gosu,
RobinHood,
Reggae,
Pumpkin,
PirateHat,
HardHat,
FourEyes,
BunnyEars,
Antlers,
ArrowThroughHead,
BrownBag,
TrafficCone,
PopeHat,
RiceHat,
SantaClaus,
Sombrero,
TikiMask,
Wizard,
Location,
Game,
ClassicGame,
Bungee,
WallLevel,
Hero,
Factory,
Difficulty,
Training,
Unpleasant,
Abusive,
Hardcore,
Masochistic,
Length,
Checkpoints,
Start,
Continue,
LoadLevel,
HeroFactory,
Base,
Jump,
Shape,
Play,
Test,
Passive,
Aggressive,
Next,
JumpDifficulty,
LevelSpeed,
Ceilings,
MovingBlocks,
GhostBlocks,
FallingBlocks,
Elevators,
Clouds,
BouncyBlocks,
Pendulums,
FlyingBlobs,
Firespinners,
Boulders,
SpikeyGuys,
Lasers,
Spikes,
Sludge,
Serpent,
SpikeyLines,
Fireballs,
Acceleration,
MaxVelocity,
Size,
Gravity,
MaxFallSpeed,
Friction,
JumpLength,
JumpAcc,
NumJumps,
DoubleJumpLength,
DoubleJumpAccel,
JetpackAcc,
JetpackFuel,
PhasedSize,
PhasedGravity,
PhasePeriod,
ClassicHero,
Jetman,
DoubleJump,
TinyBob,
Wheelie,
Spaceship,
HeroInABox,
Bouncy,
Rocketbox,
FatBob,
PhaseBob,
Viridian,
TimeMaster,
Meatboy,
JetpackWheelie,
TinyDoubleJump,
BoxJetpack,
DoubleJumpWheelie,
FatBouncy,
TinyBox,
PhasingJetpack,
BouncyJetpack,
FatDoubleJump,
Sea,
Hills,
Forest,
Cloud,
Cave,
Castle,
TheBeginning,
TheNextNinetyNine,
AGauntletOfDoom,
AlmostHero,
TheMasochist,
Escalation,
TimeCrisis,
HeroRush,
HybridRush,
HighScore,
BestLevel,
Leaderboard,
Score,
Level,
Ready,
Go,
GetToTheExit,
SecondsOnTheClock,
CoinsAddSeconds,
GetAHighScore,
Perfect,
ExtraLife,
Press,
Resume,
Statistics,
SaveLoad,
RemoveMe,
ExitLevel,
RandomSeed,
SaveSeed,
LoadSeed,
CopyToClipboard,
LoadFromClipboard,
RemovePlayerQuestion,
ExitLevelQuestion,
SaveRandomSeedAs,
SavedSeeds,
LoadTheFollowingSeedQuestion,
SeedSavedSuccessfully,
Hooray,
NoNameGiven,
Oh,
LevelsBeat,
Jumps,
Coins,
Grabbed,
CoinsOutOf,
Percent,
Blobs,
AverageLife,
Deaths,
Fireball,
Firespinner,
Boulder,
SpikeyGuy,
Spike,
Falling,
Lava,
Blob,
Laser,
FallingSpikey,
TimeLimit,
LeftBehind,
Other,
Total,
GameOver,
NewHighScore,
PlayAgain,
HighScores,
Local,
Global,
WatchComputer,
ShowPath,
ActivateSlowMo,
Paused,
Speed,
Step,
LevelCleared,
KeepSettings,
WatchReplay,
BackToFreeplay,
Pause,
Single,
Previous,
All,
End,
Loading,
HeroUnlocked,
NewHeroUnlocked,
AwardTitle_Campaign1,
AwardText_Campaign1,
AwardTitle_ArcadeHighScore,
AwardText_ArcadeHighScore,
AwardTitle_Bungee,
AwardText_Bungee,
AwardTitle_ArcadeHighScore2,
AwardText_ArcadeHighScore2,
AwardTitle_Die,
AwardText_Die,
AwardTitle_Campaign3,
AwardText_Campaign3,
AwardTitle_Invisible,
AwardText_Invisible,
AwardTitle_Hats,
AwardText_Hats,
AwardTitle_Campaign2,
AwardText_Campaign2,
AwardTitle_UnlockAllArcade,
AwardText_UnlockAllArcade,
AwardTitle_NoDeath,
AwardText_NoDeath,
AwardTitle_Save,
AwardText_Save,
AwardTitle_Obstacles,
AwardText_Obstacles,
AwardTitle_Buy,
AwardText_Buy,
AwardTitle_Campaign4,
AwardText_Campaign4,
AwardTitle_BuyHat,
AwardText_BuyHat,
AwardTitle_HeroRush2Level,
AwardText_HeroRush2Level,
AwardTitle_Replay,
AwardText_Replay,
Buy,
Bank,
Achievement,
AchievementUnlocked,
DeleteSeeds,
DeleteSeedsPlural,
MultiplierIncreased,
JumpHigherNote,
RespawnNoteGamepad,
RespawnNoteKeyboard,
PowerupNote,
Trophy,
TrophyEarned,
Awardment,
Required,
FriendsScores,
TopScores,
MyScores,
SortByLevel,
SortByScore,
ViewGamerCard,
UnlockFullGame,
NotRanked,
NotRankedFriends,
TotalArcade,
Select,
SignIn,
Chapter,
Chapter1,
Chapter2,
Chapter3,
Chapter4,
Chapter5,
Chapter6,
Credits,
PlayerLevel,
Delete,
UpSell_Campaign,
UpSell_Hero,
UpSell_SaveLoad,
UpSell_FreePlay,
UpSell_Exit,
Err_Ok,
Err_YesSelectNewDevice,
Err_NoContinueWithoutDevice,
Err_ReselectStorageDevice,
Err_StorageDeviceRequired,
Err_ForceDisconnectedReselectionMessage,
Err_PromptForDisconnectedMessage,
Err_ForceCancelledReselectionMessage,
Err_PromptForCancelledMessage,
Options_Xbox,
Err_MustBeSignedInToLive,
Err_MustBeSignedInToLiveToPlay,
Err_MustBeSignedInToLive_Header,
Err_MustBeSignedIn,
Err_MustBeSignedInToPlay,
Err_MustBeSignedIn_Header,
Err_QuitForSure,
Err_CorruptLoadHeader,
Err_CorruptLoad,
Err_MustBeSignedInToLiveForLeaderboards,
Err_NoSaveDevice,
Err_ControllerNotConnected,
Achievements,
Leaderboards,
ResumeGame,
PlayGame,

Err_PS3_CorruptLoad,
Err_PS3_NoGamePadDetected,
Err_PS3_PsnRequired_AskToSignIn,
Err_PS3_PsnRequired_WillUploadLater,
Err_PS3_NotEnoughSpace,
Err_PS3_SaveDataNotUsed,
Err_PS3_PsnLoggedOut,
Err_PS3_NotEnoughSpaceForTrophy,
ViewProfile_PS3,
OnlinePermission_PS3,
NotRanked_PS3,
FriendsScores_PS3,
PressStart_PS3,

PressStart_WiiU,
PressToJoin_WiiU,
Press_WiiU,
Press_PS3,

Xbox_NoPermissionToBuy,
Err_MustBeSignedInToLiveToAccess,

Saving,

Err_CanNotSaveLevel_NoSpace_Header,
Err_CanNotSaveLevel_NoSpace,

EnumLength
        };
    }
}