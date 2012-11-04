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