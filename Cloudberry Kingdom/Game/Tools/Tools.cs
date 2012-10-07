#define EMBEDDEDLOAD

using System.Reflection;
using System;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
#if PC_VERSION
#else

#endif
using Microsoft.Xna.Framework.Graphics;
using XnaInput = Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CoreEngine;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Blocks;

#if WINDOWS
using CloudberryKingdom;
using CloudberryKingdom.Viewer;
#if INCLUDE_EDITOR
using Forms = System.Windows.Forms;
#endif
#endif

namespace CloudberryKingdom
{
    public static class StringExtension
    {
        public static string Capitalize(this string s)
        {
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }

    public static class Vector2Extension
    {
        public static int IndexMax<T>(this T[] list) where T : IComparable
        {
            T max = list[0];
            int index = 0;
            for (int i = 1; i < list.Length; i++)
                if (list[i].CompareTo(max) > 0)
                {
                    max = list[i];
                    index = i;
                }

            return index;
        }

        public static string ToSimpleString(this Vector2 v)
        {
            return string.Format("{0}, {1}", v.X, v.Y);
        }


        public static Vector2[] Map(this Vector2[] list, Func<Vector2, Vector2> map)
        {
            Vector2[] product = new Vector2[list.Length];
            list.CopyTo(product, 0);

            for (int i = 0; i < list.Length; i++)
                product[i] = map(product[i]);

            return product;
        }



        /// <summary>
        /// Whether the vector is less than or equal to another vector in both components
        /// </summary>
        public static bool LE(this Vector2 v1, Vector2 v2)
        {
            if (v1.X <= v2.X && v1.Y <= v2.Y) return true;
            return false;
        }
        /// <summary>
        /// Whether the vector is greater than or equal to another vector in both components
        /// </summary>
        public static bool GE(this Vector2 v1, Vector2 v2)
        {
            if (v1.X >= v2.X && v1.Y >= v2.Y) return true;
            return false;
        }
    }

    /// <summary>
    /// This extension to the StringBuilder class allows garbage free concatenation
    /// of string representations of integers.
    /// </summary>
    public static class StringBuilderExtension
    {
        /// <summary>
        /// The maximum number of digits an integer can be
        /// </summary>
        const int max_digits = 15;

        /// <summary>
        /// A working array to store digits of an integer
        /// </summary>
        static Int64[] digits = new Int64[max_digits];

        /// <summary>
        /// Clear the working array to allow for a new number to be constructed
        /// </summary>
        static void ClearDigits()
        {
            for (int i = 0; i < max_digits; i++)
                digits[i] = 0;
        }

        /// <summary>
        /// Returns the index of the last digit of the number being constructed
        /// </summary>
        /// <returns></returns>
        static int LastDigit()
        {
            for (int i = max_digits - 1; i >= 0; i--)
                if (digits[i] > 0)
                    return i;
            return 0;
        }

        /// <summary>
        /// The characters associated with each possible digit
        /// </summary>
        static char[] digit_char = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// Takes the digits from a number (already stored in a work array)
        /// and adds them to a StringBuilder
        /// </summary>
        static void DigitsToString(StringBuilder str, int NumDigits)
        {
            for (int i = NumDigits - 1; i >= 0; i--)
                str.Append(digit_char[digits[i]]);
        }

        /// <summary>
        /// Add a string representation of a number to a StringBuilder
        /// </summary>
        public static void Add(this StringBuilder str, Int64 num) { Add(str, num, 1); }
        /// <summary>
        /// Add a string representation of a number to a StringBuilder
        /// </summary>
        /// <param name="MinDigits">The minimum number of digits used in the string.
        /// The string is padded with zeros to the left if needed.</param>
        public static void Add(this StringBuilder str, Int64 num, int MinDigits)
        {
            if (num < 0)
            {
                str.Append('-');
                num *= -1;
            }

            ClearDigits();

            for (int i = max_digits - 1; i >= 0; i--)
            {
                double pow = Math.Pow(10, i);
                Int64 _pow = (Int64)Math.Round(pow);
                Int64 digit = num / _pow;
                digits[i] = digit;
                num -= _pow * digit;
            }
            
            int DigitsToAppend = Math.Max(LastDigit() + 1, MinDigits);
            DigitsToString(str, DigitsToAppend);
        }
    }

    public static class ListExtension
    {
        public static Vector2 Sum<T>(this List<T> list, Func<T, Vector2> map)
        {
            Vector2 sum = Vector2.Zero;
            foreach (T item in list)
                sum += map(item);

            return sum;
        }

        /// <summary>
        /// Returns a single randomly chosen item from the list
        /// </summary>
        public static T Choose<T>(this T[] list, Rand rnd)
        {
            return list[rnd.RndInt(0, list.Length - 1)];
        }

        /// <summary>
        /// Choose a random element from the list
        /// </summary>
        public static T Choose<T>(this List<T> list, Rand rnd)
        {
            if (rnd == null)
                return list[0];
            else
            //if (list == null || list.Count == 0) return null;
                return list[rnd.RndInt(0, list.Count - 1)];
        }

        public static int IndexOf<T>(this List<T> list, Predicate<T> match)
        {
            return list.IndexOf(list.Find(match));
        }

        public static int IndexMax<T>(this List<T> list, Func<T, float> val) where T : class
        {
            if (list.Count == 0) return 0;
            return list.IndexOf(Tools.ArgMax(list, val));
        }

        public static T ArgMax<T>(this List<T> list, Func<T, float> val) where T : class
        {
            if (list.Count == 0) return null;
            return Tools.ArgMax(list, val);
        }

        public static T ArgMin<T>(this List<T> list, Func<T, float> val) where T : class
        {
            if (list.Count == 0) return null;
            return Tools.ArgMin(list, val);
        }

#if XBOX
        /// <summary>
        /// Find an element of a list.
        /// </summary>
        public static T Find<T>(this List<T> list, Predicate<T> func)
        {
            foreach (T item in list)
            {
                if (func(item))
                    return item;
            }

            return default(T);
        }

        /// <summary>
        /// Find elements of a list.
        /// </summary>
        public static List<T> FindAll<T>(this List<T> list, Predicate<T> func)
        {
            List<T> matches = new List<T>();

            foreach (T item in list)
            {
                if (func(item))
                    matches.Add(item);
            }

            return matches;
        }

        /// <summary>
        /// Find the index of a matching element.
        /// </summary>
        public static int FindIndex<T>(this List<T> list, Predicate<T> func)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (func(list[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Whether a matching element exists.
        /// </summary>
        public static bool Exists<T>(this List<T> list, Predicate<T> func)
        {
            foreach (T item in list)
            {
                if (func(item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Remove matching elements of a list.
        /// </summary>
        public static void RemoveAll<T>(this List<T> list, Predicate<T> func)
        {
            int OpenSlot = 0;
            int i = 0;
            int N = list.Count;

            while (i < N)
            {
                if (func(list[i]))
                    i++;
                else
                {
                    list[OpenSlot] = list[i];

                    i++;
                    OpenSlot++;
                }
            }

            list.RemoveRange(OpenSlot, N - OpenSlot);
        }
#endif
        //public static void DeleteAll<T>(this List<T> list, Func<IObject, bool> func)
        //{
        //    foreach (T t in list)
        //    {
        //        IObject obj = t as IObject;

        //        if (null != obj && func(obj))
        //            obj.Core.MarkedForDeletion = true;
        //    }
        //}

        /// <summary>
        /// Loop through a list, knowing both the element and its index.
        /// </summary>
        public static void ForEach<T>(this List<T> list, Action<T, int> func)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                func(item, i);
            }
        }


        public static void AddRangeAndConvert<T, S>(this List<T> list, List<S> range) where T : class
                                                                               where S : class
        {
            foreach (S s in range)
                list.Add(s as T);
        }
    }

    public static class ArrayExtension
    {
        /// <summary>
        /// Returns a subarray of a given array.
        /// </summary>
        public static T[] Range<T>(this T[] array, int StartIndex, int EndIndex)
        {
            // Make sure we don't extend past the end of the array
            EndIndex = Math.Min(EndIndex, array.Length - 1);

            // Create a new array to store the range
            T[] range = new T[EndIndex - StartIndex + 1];

            for (int i = StartIndex; i <= EndIndex; i++)
                range[i - StartIndex] = array[i];

            return range;
        }
    }

    public static class DictionaryExtension
    {
        public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict,
                                             Func<KeyValuePair<TKey, TValue>, bool> condition)
        {
            foreach (var cur in dict.Where(condition).ToList())
            {
                dict.Remove(cur.Key);
            }
        }

        public static void AddOrOverwrite<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);
        }
    }

    public class Tools
    {
        public static void Assert(bool MustBeTrue)
        {
#if DEBUG
            if (!MustBeTrue)
                Break();
#endif
        }

        public static void Log(string dump)
        {
            var stream = File.Open("dump", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            var writer = new StreamWriter(stream);
            writer.Write(dump);
            writer.Close();
            stream.Close();
        }


        public static void Nothing() { }
        public static void Warning() { }

        public static bool AllUnique<T>(List<T> list, Func<T, int> transform)
        {
            Set<int> guids = new Set<int>();
            foreach (T item in list)
            {
                int guid = transform(item);
                if (guids.Contains(guid))
                    return false;
                else
                    guids += guid;
            }

            return true;
        }

        public static List<T> MakeUnique<T>(List<T> list, Func<T, int> transform)
        {
            Set<int> guids = new Set<int>();
            List<T> uniques = new List<T>();

            foreach (T item in list)
            {
                int guid = transform(item);
                if (guids.Contains(guid))
                    continue;
                else
                    uniques.Add(item);
            }

            return uniques;
        }

        public static void LevelIsFinished()
        {
            Tools.Write("Level made!");
            if (Tools.TheGame.LoadingScreen != null && !Tools.TheGame.LoadingScreen.IsDone)
                Tools.TheGame.LoadingScreen.Accelerate = true;
        }

public static void Break()
{
#if DEBUG
    Console.WriteLine("!");
#endif
}
public static void Write(object obj)
{
#if DEBUG
    Console.WriteLine("{0}", obj);
#endif
}
public static void Write(string str, params object[] objs)
{
#if DEBUG
#if WINDOWS
    if (objs.Length == 0) Console.WriteLine(str);
    else Console.WriteLine(str, objs);
#else
    if (objs.Length == 0) System.Diagnostics.Debug.WriteLine(str);
    else System.Diagnostics.Debug.WriteLine(str, objs);
#endif
#endif
}

public static string DefaultObjectDirectory()
{
    return Path.Combine(Globals.ContentDirectory, "Objects");
}
public static string DefaultDynamicDirectory()
{
    return Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(Globals.ContentDirectory, "DynamicLoad"));
}

public static string SourceTextureDirectory()
{
    return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()))), "Content\\Art");
}

public static Thread EasyThread(int affinity, string name, Action action)
{
    Thread NewThread = new Thread(
        new ThreadStart(
            delegate
            {
#if XBOX
                Thread.CurrentThread.SetProcessorAffinity(new[] { affinity });
#endif
                var ThisThread = Thread.CurrentThread;
                EventHandler<EventArgs> abort = (s, e) =>
                {
                    if (ThisThread != null)
                    {
                        ThisThread.Abort();
                    }
                };
                Tools.TheGame.Exiting += abort;

                action();

                Tools.TheGame.Exiting -= abort;
            }))
    {
        Name = name,
#if WINDOWS
        Priority = ThreadPriority.Highest,
#endif
    };

    NewThread.Start();

    return NewThread;
}

        public static AftermathData CurrentAftermath;

        public static bool IsMasochistic = false;

        // Test variables for automatically creating levels
        public static bool AutoLoop = false;
        public static int AutoLoopDelay = 0;

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }


        public static void RemoveAll<TSource>(List<TSource> source, Func<TSource, bool> remove)
        {
            source.RemoveAll(match => remove(match));
            return;
        }
        public static void RemoveAll<TSource>(List<TSource> source, Func<TSource, int, bool> remove)
        {
            int i = 0;
            int j = 0;
            int N = source.Count;
            while (i < N)
            {
                while (j < N && remove(source[j], j))
                    j++;

                if (j == N) break;

                source[i] = source[j];
                i++;
                j++;
            }

            if (i == N) return;

            source.RemoveRange(i, N - i);
        }
        /// <summary>
        /// Return the smallest element.
        /// </summary>
        public static TSource ArgMin<TSource>(IEnumerable<TSource> source, Func<TSource, float> val)
        {
            float min = source.Min(val);
            return source.First(element => val(element) == min);
        }
        /// <summary>
        /// Return the largest element.
        /// </summary>
        public static TSource ArgMax<TSource>(IEnumerable<TSource> source, Func<TSource, float> val)
        {
            float max = source.Max(val);
            return source.First(element => val(element) == max);
        }

        public static SimpleObject LoadSimpleObject(string file)
        {
            ObjectClass SourceObject;
            Tools.UseInvariantCulture();
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
            SourceObject = new ObjectClass(Tools.QDrawer, Tools.Device, Tools.Device.PresentationParameters, 100, 100, EffectWad.FindByName("BasicEffect"), TextureWad.FindByName("White"));
            SourceObject.ReadFile(reader, EffectWad, TextureWad);
            reader.Close();
            stream.Close();

            SourceObject.ConvertForSimple();

            return new SimpleObject(SourceObject);
        }
      
#if WINDOWS
#if INCLUDE_EDITOR
        public static bool EditorPause
        {
            get
            {
                if (!ViewerIsUp) return false;
                if (background_viewer != null)
                    return !background_viewer.PlayCheckbox.Checked;
                return false;
            }
        }
        public static bool ViewerIsUp { get { return gameobj_viewer != null || background_viewer != null; } }
        public static Viewer.GameObjViewer gameobj_viewer;
        public static Viewer.BackgroundViewer background_viewer;
#else
        public const bool EditorPause = false;
        public static bool ViewerIsUp { get { return false; } }
#endif

        public static BlockDialog Dlg;
        static bool _DialogUp = false;
        public static bool DialogUp { get { return _DialogUp; } set { _DialogUp = value; } }
#endif

        static float _VolumeFade;
        public static float VolumeFade
        {
            get { return _VolumeFade; }
            set
            {
                _VolumeFade = value;
                UpdateVolume();
            }
        }

        public static float CurSongVolume;
        public static WrappedFloat SoundVolume, MusicVolume;
        public static bool FixedTimeStep = false;
        public static bool FixedTimeStep_HasBeenSet = false;

#if INCLUDE_EDITOR && WINDOWS
        public static Forms.Form WinForm { get { return (Forms.Form)Forms.Form.FromHandle(GameClass.Window.Handle); } }
#endif

        public static XnaGameClass GameClass;

        public static CloudberryKingdomGame TheGame;
        public static Version GameVersion { get { return CloudberryKingdomGame.GameVersion; } }
        public static void AddToDo(Action todo) { TheGame.ToDo.Add(todo); }

        public static String[] ButtonNames = { "A", "B", "X", "Y", "RS", "LS", "RT", "LT", "RJ", "RJ", "LJ", "LJ", "DPad", "Start" };
        public static String[] DirNames = { "right", "up", "left", "down" };

        //public static Recycler Recycle = new Recycler();

        public static GameFactory CurGameType;
        public static GameData CurGameData;
        public static Level CurLevel;
        public static Camera DummyCamera;
        public static Camera CurCamera
        {
            get
            {
                if (CurLevel == null)
                {
                    if (DummyCamera == null) DummyCamera = new Camera();
                    return DummyCamera;
                }
                else return CurLevel.MainCamera;
            }
        }
        public static GameData WorldMap, TitleGame;

        public static EzFont Font_Grobold42, Font_Grobold42_2;
        public static EzFont LilFont;

        public static int[] VibrateTimes = { 0, 0, 0, 0 };

        public static int DifficultyTypes = Tools.GetValues<DifficultyParam>().Count();//Enum.GetValues(typeof(DifficultyType)).Length;
        public static int StyleTypes = 8;
        public static int UpgradeTypes = Tools.GetValues<Upgrade>().Count();//Enum.GetValues(typeof(Upgrade)).Length;

#if WINDOWS
        public static XnaInput.KeyboardState keybState, PrevKeyboardState;
        public static XnaInput.MouseState CurMouseState, PrevMouseState;
        public static Vector2 DeltaMouse, RawDeltaMouse;
        public static int DeltaScroll;
        public static bool MouseInWindow = false;
        public static bool Editing;

        public static Vector2 MousePos
        {
            get { return new Vector2(CurMouseState.X, CurMouseState.Y) / Tools.Render.SpriteScaling; }
            set { XnaInput.Mouse.SetPosition((int)(value.X * Tools.Render.SpriteScaling), (int)(value.Y * Tools.Render.SpriteScaling)); }
        }

        public static bool Fullscreen
        {
            get { return TheGame.MyGraphicsDeviceManager.IsFullScreen; }
            set
            {
                if (value != Fullscreen)
                {
                    if (value)
                    {
                        int width = Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferWidth,
                            height = Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferHeight;

                        var safe = ResolutionGroup.SafeResolution(width, height);
                        width = safe.X;
                        height = safe.Y;
                        
                        ResolutionGroup.Use(width, height, false);
                    }

                    TheGame.MyGraphicsDeviceManager.ToggleFullScreen();

                    // Reset the resolution, in case we were trimming the letterbox
                    if (ResolutionGroup.LastSetMode != null)
                        ResolutionGroup.Use(ResolutionGroup.LastSetMode);
                }
            }
        }

        /// <summary>
        /// Whether the left mouse button is currently down.
        /// </summary>
        public static bool CurMouseDown() { return CurMouseState.LeftButton == XnaInput.ButtonState.Pressed; }
        
        /// <summary>
        /// Whether the left mouse button was down on the last frame.
        /// </summary>
        public static bool PrevMouseDown() { return PrevMouseState.LeftButton == XnaInput.ButtonState.Pressed; }

        /// <summary>
        /// True when the left mouse button was pressed and released.
        /// </summary>
        public static bool MouseReleased() { return !CurMouseDown() && PrevMouseDown(); }

        /// <summary>
        /// True when the left mouse button isn't down currently and also wasn't down on the previous frame.
        /// </summary>
        public static bool MouseNotDown() { return !CurMouseDown() && !PrevMouseDown(); }

        /// <summary>
        /// True when the left mouse button is down currently or was down on the previous frame.
        /// </summary>
        public static bool MouseDown() { return CurMouseDown() || PrevMouseDown(); }

        /// <summary>
        /// True when the left mouse button is down currently and was NOT down on the previous frame.
        /// </summary>
        public static bool MousePressed() { return CurMouseDown() && !PrevMouseDown(); }

        /// <summary>
        /// Whether the left RightMouse button is currently down.
        /// </summary>
        public static bool CurRightMouseDown() { return CurMouseState.RightButton == XnaInput.ButtonState.Pressed; }

        /// <summary>
        /// Whether the left RightMouse button was down on the last frame.
        /// </summary>
        public static bool PrevRightMouseDown() { return PrevMouseState.RightButton == XnaInput.ButtonState.Pressed; }

        /// <summary>
        /// True when the left RightMouse button was pressed and released.
        /// </summary>
        public static bool RightMouseReleased() { return !CurRightMouseDown() && PrevRightMouseDown(); }

        public static Vector2 MouseGUIPos(Vector2 zoom)
        {
            //return Tools.ToGUICoordinates(new Vector2(Tools.CurMouseState.X, Tools.CurMouseState.Y), Tools.CurLevel.MainCamera, zoom);
            return Tools.ToGUICoordinates(MousePos, Tools.CurLevel.MainCamera, zoom);
        }

        public static Vector2 MouseWorldPos()
        {
            return Tools.ToWorldCoordinates(MousePos
            , Tools.CurLevel.MainCamera);
        }

        public static bool ShiftDown()
        {
            return
                Tools.keybState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) ||
                Tools.keybState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift);
        }

        public static bool CntrlDown()
        {
            return
                Tools.keybState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) ||
                Tools.keybState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl);
        }

        public static string RemoveAfter(string s, string occurence)
        {
            if (s.Contains(occurence)) s = s.Remove(s.IndexOf(occurence));
            return s;
        }
        public static string SantitizeOneLineString(string s, EzFont font)
        {
            s = RemoveAfter(s, "\n");
            s = RemoveAfter(s, "\t");

            s = s.Replace("{", " ");
            s = s.Replace("}", " ");

            try
            {
                font.Font.MeasureString(s);
            }
            catch
            {
                s = "Invalid";
            }

            return s;
        }

#else

#endif
        public static XnaInput.GamePadState[] padState, PrevpadState;

        /// <summary>
        /// Return just the file name of a path.
        /// </summary>
        public static string StripPath(string file)
        {
            int LastSlash = file.LastIndexOf("\\");
            if (LastSlash < 0)
                return file;
            else
                return file.Substring(LastSlash + 1);
        }

        /// <summary>
        /// Return just the first folder of the path.
        /// </summary>
        public static string FirstFolder(string path, string ignore)
        {
            int i = path.IndexOf(ignore);
            if (i >= 0)
                path = path.Substring(i + ignore.Length);

            int FirstSlash = path.IndexOf("\\");
            if (FirstSlash < 0)
                return path;
            else
                return path.Substring(0, FirstSlash);
        }

        public static EzTexture Texture(string name) { return TextureWad.FindByName(name); }
        public static EzSound Sound(string name) { return SoundWad.FindByName(name); }
        public static void Pop() { Pop(2); }
        public static void Pop(int Pitch) { Sound("Pop " + CoreMath.Restrict(1, 3, Pitch).ToString()).Play(); }

        public static GameTime gameTime;
        //public static Random Rnd;
        public static Rand GlobalRnd = new Rand(0);
        public static EzEffectWad EffectWad;
        public static EzEffect BasicEffect, NoTexture, CircleEffect, LightSourceEffect, HslEffect, HslGreenEffect, WindowEffect;
        public static Effect PaintEffect_SpriteBatch;
        public static EzTextureWad TextureWad;
        public static ContentManager SoundContentManager;
        public static EzSoundWad SoundWad, PrivateSoundWad;
        public static EzSongWad SongWad;
        public static QuadDrawer QDrawer;
        public static MainRender Render;
        public static GraphicsDevice Device;
        public static RenderTarget2D DestinationRenderTarget;
        public static bool ScreenshotMode, CapturingVideo;
        public static Texture2D Screenshot;
        public static float t, dt;
        public static int Screenshots = 0, VideoFrame = 0;
        public static int DrawCount, PhsxCount;

        public static EzSong Song_140mph, Song_Happy, Song_BlueChair, Song_Ripcurl, Song_Evidence, Song_GetaGrip, Song_House, Song_Nero, Song_FatInFire, Song_Heavens, Song_TidyUp, Song_WritersBlock;
        public static List<EzSong> SongList_Standard = new List<EzSong>();

        public static bool FreeCam = false;
        public static bool DrawBoxes = false;
        public static bool DrawGraphics = true;
        public static bool StepControl = false;
        static int _PhsxSpeed = 1;
        public static int PhsxSpeed { get { return _PhsxSpeed; } set { _PhsxSpeed = value; } }

        public static bool ShowLoadingScreen;
        public static ILoadingScreen CurrentLoadingScreen;

        public static void LoadBasicArt(ContentManager Content)
        {
            TextureWad = new EzTextureWad();
            TextureWad.AddTexture(Content.Load<Texture2D>("White"), "White");
            TextureWad.AddTexture(Content.Load<Texture2D>("Circle"), "Circle");
            TextureWad.AddTexture(Content.Load<Texture2D>("Smooth"), "Smooth");

            TextureWad.DefaultTexture = TextureWad.TextureList[0];
        }

        public static string GetFileName(String FilePath)
        {
            int i = FilePath.LastIndexOf("\\");
            int j = FilePath.IndexOf(".", i);
            if (j < 0) j = FilePath.Length;
            if (i < 0) return FilePath.Substring(0, j - 1);
            else return FilePath.Substring(i + 1, j - 1 - i);
        }

        public static string GetFileNamePlusExtension(String FilePath)
        {
            int i = FilePath.LastIndexOf("\\");
            int n = FilePath.Length;
            if (i < 0) return FilePath.Substring(0, n - 1);
            else return FilePath.Substring(i + 1, n - 1 - i);
        }

        public static string GetFileBigName(String FilePath)
        {
            int i = FilePath.LastIndexOf("\\");
            if (i < 0) return FilePath;

            string Path = FilePath.Substring(0, i);
            i = Path.LastIndexOf("\\");

            if (i < 0) return FilePath;
            else return FilePath.Substring(i + 1);
        }

        public static string GetFileName(String path, String FilePath)
        {
            int i = FilePath.IndexOf(path) + path.Length + 1;
            if (i < 0) return "ERROR";
            int j = FilePath.IndexOf(".", i);
            if (j <= i) return "ERROR";
        
            return FilePath.Substring(i, j - i);
        }

        public static string GetFileExt(String path, String FilePath)
        {
            int j = FilePath.IndexOf(".");
            if (j < 0) return "ERROR";
            
            return FilePath.Substring(j + 1);
        }

        public static string[] GetFiles(string path, bool IncludeSubdirectories)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(path));

            if (IncludeSubdirectories)
            {
                string[] dir = Directory.GetDirectories(path);
                for (int i = 0; i < dir.Length; i++)
                    files.AddRange(GetFiles(dir[i], IncludeSubdirectories));
            }

            return files.ToArray();
        }

        public static void LoadEffects(ContentManager Content, bool CreateNewWad)
        {
            if (CreateNewWad)
                EffectWad = new EzEffectWad();

            EffectWad.AddEffect(Content.Load<Effect>("Effects\\BasicEffect"), "Basic");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\NoTexture"), "NoTexture");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\Circle"), "Circle");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\Shell"), "Shell");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\FireballEffect"), "Fireball");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\Paint"), "Paint");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\Lava"), "Lava");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\LightMap"), "LightMap");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\LightSource"), "LightSource");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\BwEffect"), "BW");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\Hsl_Green"), "Hsl_Green");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\Hsl"), "Hsl");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\Window"), "Window");

            BasicEffect = EffectWad.EffectList[0];
            NoTexture = EffectWad.EffectList[1];
            CircleEffect = EffectWad.EffectList[2];
            LightSourceEffect = EffectWad.FindByName("LightSource");
            HslEffect = EffectWad.FindByName("Hsl");
            HslGreenEffect = EffectWad.FindByName("Hsl_Green");
            WindowEffect = EffectWad.FindByName("Window");

            PaintEffect_SpriteBatch = Content.Load<Effect>("Effects\\Paint_SpriteBatch");
        }

        public static float BoxSize(Vector2 TR, Vector2 BL)
        {
            return (TR.X - BL.X) * (TR.Y - BL.Y);
        }

        public static float CurVolume = -1;
        public static void UpdateVolume()
        {
            float NewVolume = Tools.MusicVolume.Val * Tools.VolumeFade * Tools.CurSongVolume;
            if (Tools.SongWad != null && Tools.SongWad.Paused) NewVolume = 0;
            if (NewVolume != CurVolume)
            {
                CurVolume = MediaPlayer.Volume = NewVolume;
            }
        }

        /// <summary>
        /// When true music will be stopped when a loading screen begins (The default behavior)
        /// </summary>
        public static bool KillMusicOnLoadingScreen = true;

        /// <summary>
        /// When true the next loading screen will not stop the music.
        /// </summary>
        public static bool DoNotKillMusicOnNextLoadingscreen = false;

        public static void BeginLoadingScreen(bool KillMusic)
        {
            if (KillMusic && Tools.SongWad != null && KillMusicOnLoadingScreen && !DoNotKillMusicOnNextLoadingscreen)
                Tools.SongWad.FadeOut();
            DoNotKillMusicOnNextLoadingscreen = false;

            Tools.ShowLoadingScreen = true;
            //if (CurrentLoadingScreen == null)
            
            //CurrentLoadingScreen = new LoadingScreen();
            CurrentLoadingScreen = new LoadingScreen2();

            CurrentLoadingScreen.Start();
        }

        public static void BeginLoadingScreen_Fake(int Length)
        {
            LoadingScreen.MinLoadLength = Length;

            DoNotKillMusicOnNextLoadingscreen = false;

            Tools.ShowLoadingScreen = true;
            CurrentLoadingScreen = new LoadingScreen();
            CurrentLoadingScreen.Start();
            CurrentLoadingScreen.MakeFake();
        }

        public static void PlayHappyMusic()
        {
            //Tools.SongWad.SetPlayList(Tools.Song_Happy);
            Tools.SongWad.SuppressNextInfoDisplay = true;
            Tools.SongWad.SetPlayList(Tools.Song_Heavens);
            Tools.SongWad.Start(true);
        }
        
        public static void EndLoadingScreen()
        {        
            Tools.CurrentLoadingScreen.End();
        }

        public static void EndLoadingScreen_Immediate()
        {
            Tools.ShowLoadingScreen = false;
        }

        /// <summary>
        /// Parses a string, stripping comments, and returning the sequence of bits it contains (separated strings).
        /// </summary>
        public static List<string> GetBitsFromLine(String line)
        {
            line = Tools.RemoveComment(line);

            var bits = line.Split(' ', '\t').ToList();
            bits.RemoveAll(bit => bit == "" || bit == " " || bit == "\t");

            return bits;
        }

        public static List<string> GetBitsFromReader(StreamReader reader)
        {
            return GetBitsFromLine(reader.ReadLine());
        }


        public static object ReadFields(object obj, StreamReader reader)
        {
            var line = reader.ReadLine();
            while (line != null)
            {
                var bits = Tools.GetBitsFromLine(line);
                
                if (bits.Count > 0)
                {
                    var first = bits[0];

                    bool WasReadable = false;
                    var info = obj.GetType().GetField(first);
                    //try
                    if (info != null)
                    {
                        info = obj.GetType().GetField(first);
                        if (info.FieldType.GetInterfaces().Contains(typeof(IReadWrite)))
                        {
                            WasReadable = true;
                            var rw = (IReadWrite)(info.GetValue(obj));
                            rw.Read(reader);
                            info.SetValue(obj, rw);
                        }
                        else
                        // List of IReadWrites
                        {
                            Type type = info.FieldType;
                            if (type.IsGenericType && type.GetGenericTypeDefinition()
                                    == typeof(List<>))
                            {
                                WasReadable = true;

                                var list = info.GetValue(obj) as System.Collections.IList;

                                Type itemType = type.GetGenericArguments()[0];
                                if (itemType.GetInterfaces().Contains(typeof(IReadWrite)))
                                    ReadList(reader, list, itemType);
                            }
                        }
                    }
                    //catch
                    else
                    {
                        WasReadable = false;
                    }

                    if (!WasReadable)
                        switch (first)
                        {
                            case "End":
                                return obj;

                            case "_MyTexture":
                                Tools.ReadLineToObj(obj, bits);
                                break;

                            default:
                                Tools.ReadLineToObj(obj, bits);
                                break;
                        }
                }

                line = reader.ReadLine();
            }

            return obj;
        }

        private static void ReadList(StreamReader reader, System.Collections.IList list, Type itemType)
        {
            var line = reader.ReadLine();
            bool ReadingList = true;

            while (line != null && ReadingList)
            {
                line = reader.ReadLine();
                var bits = Tools.GetBitsFromLine(line);

                switch (bits[0])
                {
                    case "Add":
                        ConstructorInfo constructor;
                        if (bits.Count > 1)
                        {
                            //var type = Type.GetType("CloudberryKingdom." + bits[1]);
                            var type = Type.GetType(bits[1]);
                            constructor = type.GetConstructor(Type.EmptyTypes);
                        }
                        else
                            constructor = itemType.GetConstructor(Type.EmptyTypes);

                        var newobj = constructor.Invoke(Type.EmptyTypes);
                        //ReadFields(newobj, reader);
                        if (newobj is IReadWrite) ((IReadWrite)newobj).Read(reader);
                        else ReadFields(newobj, reader);
                        list.Add(newobj);

                        break;
                    case "EndList":
                        ReadingList = false;
                        break;
                    case "End":
                        break;
                    default:
                        break;
                }
            }
        }
        static int WriteRecursiveDepth = 0;
        static int WriteObjId = 0;
        public static void WriteFields(object obj, StreamWriter writer, params string[] VariableNames)
        {
            WriteRecursiveDepth++;
            string WhiteSpace = "";
            for (int i = 1; i < WriteRecursiveDepth; i++)
                WhiteSpace += "  ";

            foreach (FieldInfo info in obj.GetType().GetFields())
            {
                // Check if field is listed as a variable to be written.
                if (VariableNames.Contains(info.Name))
                {
                    string line = null;
                    // int
                    if (info.FieldType == typeof(int))
                        line = ((int)info.GetValue(obj)).ToString();
                    // float
                    else if (info.FieldType == typeof(float))
                        line = ((float)info.GetValue(obj)).ToString();
                    // Vector2
                    else if (info.FieldType == typeof(Vector2))
                    {
                        var v = (Vector2)(info.GetValue(obj));
                        line = string.Format("{0} {1}", v.X, v.Y);
                    }
                    // Vector3
                    else if (info.FieldType == typeof(Vector3))
                    {
                        var v = (Vector3)(info.GetValue(obj));
                        line = string.Format("{0} {1} {2}", v.X, v.Y, v.Z);
                    }
                    // Vector4
                    else if (info.FieldType == typeof(Vector4))
                    {
                        var v = (Vector4)(info.GetValue(obj));
                        line = string.Format("{0} {1} {2} {3}", v.X, v.Y, v.Z, v.W);
                    }
                    // Color
                    else if (info.FieldType == typeof(Color))
                    {
                        var c = (Color)(info.GetValue(obj));
                        line = string.Format("{0} {1} {2} {3}", c.R, c.G, c.B, c.A);
                    }
                    // bool
                    else if (info.FieldType == typeof(bool))
                    {
                        var b = (bool)(info.GetValue(obj));
                        line = string.Format("{0}", b);
                    }
                    // string
                    else if (info.FieldType == typeof(string))
                        line = ((string)info.GetValue(obj)).ToString();
                    // EzTexture
                    else if (info.FieldType == typeof(EzTexture))
                    {
                        EzTexture texture = (EzTexture)info.GetValue(obj);
                        if (texture == null) continue;
                        else line = texture.Name.ToString();
                    }
                    // EzEffect
                    else if (info.FieldType == typeof(EzEffect))
                    {
                        EzEffect effect = (EzEffect)info.GetValue(obj);
                        if (effect == null) continue;
                        else line = effect.Name.ToString();
                    }
                    // PhsxData
                    else if (info.FieldType == typeof(PhsxData))
                    {
                        var d = (PhsxData)(info.GetValue(obj));
                        line = string.Format("{0} {1} {2} {3} {4} {5}", d.Position.X, d.Position.Y, d.Velocity.X, d.Velocity.Y, d.Acceleration.X, d.Acceleration.Y);
                    }
                    // BasePoint
                    else if (info.FieldType == typeof(BasePoint))
                    {
                        var b = (BasePoint)(info.GetValue(obj));
                        line = string.Format("{0} {1} {2} {3} {4} {5}", b.e1.X, b.e1.Y, b.e2.X, b.e2.Y, b.Origin.X, b.Origin.Y);
                    }
                    // MyOwnVertexFormat
                    else if (info.FieldType == typeof(MyOwnVertexFormat))
                    {
                        var v = (MyOwnVertexFormat)(info.GetValue(obj));
                        line = string.Format("{0} {1} {2} {3} {4} {5} {6} {7}", v.xy.X, v.xy.Y, v.uv.X, v.uv.Y, v.Color.R, v.Color.G, v.Color.B, v.Color.A);
                    }
                    else if (info.FieldType.GetInterfaces().Contains(typeof(IReadWrite)))
                    {
                        var rw = (IReadWrite)(info.GetValue(obj));
                        writer.WriteLine(WhiteSpace + info.Name);
                        rw.Write(writer);
                        writer.WriteLine(WhiteSpace + "End");
                        writer.WriteLine();
                    }
                    else
                    {
                        // List of IReadWrites
                        Type type = info.FieldType;
                        if (type.IsGenericType && type.GetGenericTypeDefinition()
                                == typeof(List<>))
                        {
                            Type itemType = type.GetGenericArguments()[0];
                            if (itemType.GetInterfaces().Contains(typeof(IReadWrite)))
                            {
                                writer.WriteLine(WhiteSpace + info.Name);
                                writer.WriteLine(WhiteSpace + "StartList");
                                foreach (var rw in (IEnumerable<IReadWrite>)info.GetValue(obj))
                                {
                                    writer.WriteLine(WhiteSpace + "Add " + rw.GetType().Namespace + "." + rw.GetType().Name);
                                    rw.Write(writer);
                                    writer.WriteLine(WhiteSpace + "End");
                                }
                                writer.WriteLine(WhiteSpace + "EndList");
                                writer.WriteLine();
                            }
                        }
                    }

                    if (line != null)
                        writer.WriteLine(WhiteSpace + string.Format("{0} {1}", info.Name, line));
                }
            }

            WriteRecursiveDepth--;
        }

        static void ResetWrite()
        {
            WriteRecursiveDepth = 0;
            WriteObjId = 0;
        }

        public static void WriteCode(string root, IReadWrite rw)
        {
            ResetWrite();

            var stream = File.Open("code_dump.code", FileMode.Create, FileAccess.Write, FileShare.None);
            var writer = new StreamWriter(stream);

            rw.WriteCode(root, writer);

            writer.Close();
            stream.Close();
        }

        public static string ToCode(Vector2 v)
        {
            return string.Format("new Vector2({0}f, {1}f)", v.X, v.Y);
        }

        public static string ToCode(string s)
        {
            return string.Format("\"{0}\"", s.Replace("\\", "\\\\"));
        }

        static bool LastLineWasBlank = false;
        public static void WriteFieldsToCode(object obj, string prefix, StreamWriter writer, params string[] VariableNames)
        {
            string _prefix = prefix;
            if (prefix.Length > 0)
                _prefix += ".";

            WriteRecursiveDepth++;
            string WhiteSpace = "";
            for (int i = 1; i < WriteRecursiveDepth; i++)
                WhiteSpace += "  ";

            foreach (FieldInfo info in obj.GetType().GetFields())
            {
                // Check if field is listed as a variable to be written.
                if (VariableNames.Contains(info.Name))
                {
                    string line = null;
                    // int
                    if (info.FieldType == typeof(int))
                        line = ((int)info.GetValue(obj)).ToString();
                    // float
                    else if (info.FieldType == typeof(float))
                    {
                        var v = (float)(info.GetValue(obj));
                        line = string.Format("{0}f", v);
                    }
                    // Vector2
                    else if (info.FieldType == typeof(Vector2))
                    {
                        var v = (Vector2)(info.GetValue(obj));
                        line = string.Format("new Vector2({0}f, {1}f)", v.X, v.Y);
                    }
                    // Vector3
                    else if (info.FieldType == typeof(Vector3))
                    {
                        var v = (Vector3)(info.GetValue(obj));
                        line = string.Format("new Vector3({0}f, {1}f, {2}f)", v.X, v.Y, v.Z);
                    }
                    // Vector4
                    else if (info.FieldType == typeof(Vector4))
                    {
                        var v = (Vector4)(info.GetValue(obj));
                        line = string.Format("new Vector4({0}f, {1}f, {2}f, {3}f)", v.X, v.Y, v.Z, v.W);
                    }
                    // Color
                    else if (info.FieldType == typeof(Color))
                    {
                        var c = (Color)(info.GetValue(obj));
                        line = string.Format("new Color({0}, {1}, {2}, {3})", c.R, c.G, c.B, c.A);
                    }
                    // bool
                    else if (info.FieldType == typeof(bool))
                    {
                        var b = (bool)(info.GetValue(obj));
                        line = b ? "true" : "false";
                    }
                    // string
                    else if (info.FieldType == typeof(string))
                        line = string.Format("\"{0}\"", ((string)info.GetValue(obj)));
                    // EzTexture
                    else if (info.FieldType == typeof(EzTexture))
                    {
                        var texture = (EzTexture)info.GetValue(obj);
                        if (null == texture)
                            line = "null";
                        else
                            line = string.Format("Tools.Texture(\"{0}\")", texture.Name);
                    }
                    // EzEffect
                    else if (info.FieldType == typeof(EzEffect))
                    {
                        EzEffect effect = (EzEffect)info.GetValue(obj);
                        if (effect == null) continue;
                        else
                        {
                            switch (effect.Name)
                            {
                                case "Basic": line = "Tools.BasicEffect"; break;
                                case "Window": line = "Tools.WindowEffect"; break;
                                default: line = string.Format("Tools.Effect(\"{0}\")", effect.Name.ToString()); break;
                            }
                        }
                    }
                    // PhsxData
                    else if (info.FieldType == typeof(PhsxData))
                    {
                        var d = (PhsxData)(info.GetValue(obj));
                        line = string.Format("new PhsxData({0}f, {1}f, {2}f, {3}f, {4}f, {5}f)", d.Position.X, d.Position.Y, d.Velocity.X, d.Velocity.Y, d.Acceleration.X, d.Acceleration.Y);
                    }
                    // MyOwnVertexFormat
                    else if (info.FieldType == typeof(MyOwnVertexFormat))
                    {
                        var v = (MyOwnVertexFormat)(info.GetValue(obj));
                        line = string.Format("new MyOwnVertexFormat(new Vector2({0}f, {1}f), new Vector2({2}f, {3}f), new Color({4}, {5}, {6}, {7}))", v.xy.X, v.xy.Y, v.uv.X, v.uv.Y, v.Color.R, v.Color.G, v.Color.B, v.Color.A);
                    }
                    // BasePoint
                    else if (info.FieldType == typeof(BasePoint))
                    {
                        var b = (BasePoint)(info.GetValue(obj));
                        line = string.Format("new Drawing.BasePoint({0}f, {1}f, {2}f, {3}f, {4}f, {5}f)", b.e1.X, b.e1.Y, b.e2.X, b.e2.Y, b.Origin.X, b.Origin.Y);
                    }
                    else if (info.FieldType.GetInterfaces().Contains(typeof(IReadWrite)))
                    {
                        var rw = (IReadWrite)(info.GetValue(obj));
                        if (null == rw && !info.FieldType.IsValueType) continue;

                        rw.WriteCode(_prefix + info.Name, writer);
                        if (!LastLineWasBlank) writer.WriteLine();
                        LastLineWasBlank = true;
                    }
                    else
                    {
                        // List of IReadWrites
                        Type type = info.FieldType;
                        if (type.IsGenericType && type.GetGenericTypeDefinition()
                                == typeof(List<>))
                        {
                            Type itemType = type.GetGenericArguments()[0];
                            if (itemType.GetInterfaces().Contains(typeof(IReadWrite)))
                            {
                                foreach (var rw in (IEnumerable<IReadWrite>)info.GetValue(obj))
                                {
                                    // Get a new unique name for this object.
                                    string item_name = GetObjName();

                                    // Get the string that constructs this object.
                                    string construct = null;
                                    if (rw is ViewReadWrite)
                                        construct = ((ViewReadWrite)rw).GetConstructorString();
                                    if (construct == null) construct = string.Format("new {0}()", rw.GetType());

                                    // Write the constructor.
                                    writer.WriteLine(WhiteSpace + string.Format("{0} {1} = {2};", rw.GetType(), item_name, construct));

                                    // Write the interior information of the object.
                                    rw.WriteCode(item_name, writer);

                                    // Add the object to the list.
                                    writer.WriteLine(WhiteSpace + string.Format("{2}{0}.Add({1});", info.Name, item_name, _prefix));
                                    if (!LastLineWasBlank) writer.WriteLine();
                                    LastLineWasBlank = true;
                                }
                                if (!LastLineWasBlank) writer.WriteLine();
                            }
                        }
                    }

                    if (line != null)
                    {
                        writer.WriteLine(WhiteSpace + string.Format("{2}{0} = {1};", info.Name, line, _prefix));
                        LastLineWasBlank = false;
                    }
                }
            }

            WriteRecursiveDepth--;
        }
        static int GetObjId()
        {
            WriteObjId++;
            return WriteObjId;
        }
        static string ObjName(int id) { return string.Format("__{0}", id); }
        static string GetObjName() { return ObjName(GetObjId()); }

        public static Dictionary<string, int> GetLocations(List<string> Bits, params string[] keywords)
        {
            var dict = new Dictionary<string,int>();
            for (int i = 0; i < Bits.Count; i++)
                if (keywords.Contains(Bits[i]))
                    dict.Add(Bits[i], i);
            return dict;
        }
        public static Dictionary<string, int> GetLocations(List<string> Bits, List<string> keywords)
        {
            var dict = new Dictionary<string, int>();
            for (int i = 0; i < Bits.Count; i++)
                if (keywords.Contains(Bits[i]))
                    dict.Add(Bits[i], i);
            return dict;
        }

        public static void ReadLineToObj(object obj, List<string> Bits)
        {
            ReadLineToObj(ref obj, Bits[0], Bits);
        }
        public static void ReadLineToObj(ref object obj, string field, List<string> Bits)
        {
            // If field name has a period in it, resolve recursively.
            var period = field.IndexOf(".");
            if (period > 0)
            {
                var subfield = field.Substring(period + 1);
                field = field.Substring(0, period);

                var subobject_field = obj.GetType().GetField(field);
                if (subobject_field == null) { Tools.Log(string.Format("Field {0} not found.", field)); return; }

                var subobject = subobject_field.GetValue(obj);
                if (subobject == null) { Tools.Log(string.Format("Subfield {0} was a null object and could not be written into.", field)); return; }

                ReadLineToObj(ref subobject, subfield, Bits);
            }
            // otherwise parse the input into the given field.
            else
            {
                var fieldinfo = obj.GetType().GetField(field);
                if (fieldinfo == null) { Tools.Log(string.Format("Field {0} not found.", field)); return; }


                // int
                if (fieldinfo.FieldType == typeof(int))
                    fieldinfo.SetValue(obj, int.Parse(Bits[1]));
                // float
                if (fieldinfo.FieldType == typeof(float))
                    fieldinfo.SetValue(obj, float.Parse(Bits[1]));
                // Vector2
                else if (fieldinfo.FieldType == typeof(Vector2))
                    fieldinfo.SetValue(obj, ParseToVector2(Bits[1], Bits[2]));
                // Vector4
                else if (fieldinfo.FieldType == typeof(Vector4))
                    fieldinfo.SetValue(obj, ParseToVector4(Bits[1], Bits[2], Bits[3], Bits[4]));
                // Color
                else if (fieldinfo.FieldType == typeof(Color))
                    fieldinfo.SetValue(obj, ParseToColor(Bits[1], Bits[2], Bits[3], Bits[4]));
                // bool
                else if (fieldinfo.FieldType == typeof(bool))
                    fieldinfo.SetValue(obj, bool.Parse(Bits[1]));
                // EzTexture
                else if (fieldinfo.FieldType == typeof(EzTexture))
                    fieldinfo.SetValue(obj, Tools.TextureWad.FindByName(Bits[1]));
                // EzEffect
                else if (fieldinfo.FieldType == typeof(EzEffect))
                    fieldinfo.SetValue(obj, Tools.EffectWad.FindByName(Bits[1]));
                // TextureOrAnim
                else if (fieldinfo.FieldType == typeof(TextureOrAnim))
                    fieldinfo.SetValue(obj, Tools.TextureWad.FindTextureOrAnim(Bits[1]));
                // string
                else if (fieldinfo.FieldType == typeof(string))
                    if (Bits.Count == 1)
                        fieldinfo.SetValue(obj, "");
                    else
                        fieldinfo.SetValue(obj, Bits[1]);
                // PhsxData
                else if (fieldinfo.FieldType == typeof(PhsxData))
                    fieldinfo.SetValue(obj, Tools.ParseToPhsxData(Bits[1], Bits[2], Bits[3], Bits[4], Bits[5], Bits[6]));
                // BasePoint
                else if (fieldinfo.FieldType == typeof(BasePoint))
                    fieldinfo.SetValue(obj, Tools.ParseToBasePoint(Bits[1], Bits[2], Bits[3], Bits[4], Bits[5], Bits[6]));
                // MyOwnVertexFormat
                else if (fieldinfo.FieldType == typeof(MyOwnVertexFormat))
                    fieldinfo.SetValue(obj, Tools.ParseToMyOwnVertexFormat(Bits[1], Bits[2], Bits[3], Bits[4], Bits[5], Bits[6], Bits[7], Bits[8]));
            }
        }

        public static bool BitsHasBit(List<string> Bits, string Bit)
        {
            if (Bits.Contains(Bit)) return true;
            else return false;
        }

        public static string RemoveComment(String str)
        {
            int CommentIndex = str.IndexOf("//");
            if (CommentIndex >= 0)
                str = str.Substring(0, CommentIndex);
            return str;
        }

        public static Vector2 ParseToVector2(String bit1, String bit2)
        {
            Vector2 Vec = Vector2.Zero;
            
            Vec.X = float.Parse(bit1);
            Vec.Y = float.Parse(bit2);
            
            return Vec;
        }

        public static Vector4 ParseToVector4(String bit1, String bit2, String bit3, String bit4)
        {
            Vector4 Vec = Vector4.Zero;

            Vec.X = float.Parse(bit1);
            Vec.Y = float.Parse(bit2);
            Vec.Z = float.Parse(bit3);
            Vec.W = float.Parse(bit4);

            return Vec;
        }

        public static PhsxData ParseToPhsxData(String bit1, String bit2, String bit3, String bit4, String bit5, String bit6)
        {
            PhsxData data = new PhsxData();

            data.Position.X = float.Parse(bit1);
            data.Position.Y = float.Parse(bit2);
            data.Velocity.X = float.Parse(bit3);
            data.Velocity.Y = float.Parse(bit4);
            data.Acceleration.X = float.Parse(bit5);
            data.Acceleration.Y = float.Parse(bit6);

            return data;
        }

        public static BasePoint ParseToBasePoint(String bit1, String bit2, String bit3, String bit4, String bit5, String bit6)
        {
            BasePoint b = new BasePoint();

            b.e1.X = float.Parse(bit1);
            b.e1.Y = float.Parse(bit2);
            b.e2.X = float.Parse(bit3);
            b.e2.Y = float.Parse(bit4);
            b.Origin.X = float.Parse(bit5);
            b.Origin.Y = float.Parse(bit6);

            return b;
        }

        public static MyOwnVertexFormat ParseToMyOwnVertexFormat(String bit1, String bit2, String bit3, String bit4, String bit5, String bit6, string bit7, string bit8)
        {
            MyOwnVertexFormat b = new MyOwnVertexFormat();

            b.xy.X = float.Parse(bit1);
            b.xy.Y = float.Parse(bit2);
            
            b.uv.X = float.Parse(bit3);
            b.uv.Y = float.Parse(bit4);

            b.Color.R = byte.Parse(bit5);
            b.Color.G = byte.Parse(bit6);
            b.Color.B = byte.Parse(bit7);
            b.Color.A = byte.Parse(bit8);

            return b;
        }

        public static Vector2 ParseToVector2(String str)
        {
            int CommaIndex = str.IndexOf(",");
            Vector2 Vec = new Vector2();

            String Component1, Component2;
            Component1 = str.Substring(0, CommaIndex);
            Component2 = str.Substring(CommaIndex + 1, str.Length - CommaIndex - 1);
            Vec.X = float.Parse(Component1);
            Vec.Y = float.Parse(Component2);

            return Vec;
        }

        public static Color ParseToColor(String bit1, String bit2, String bit3, String bit4)
        {
            Color c = Color.White;

            c.R = byte.Parse(bit1);
            c.G = byte.Parse(bit2);
            c.B = byte.Parse(bit3);
            c.A = byte.Parse(bit4);

            return c;
        }

        public static Color ParseToColor(String str)
        {
            int CommaIndex = str.IndexOf(",");
            int CommaIndex2 = str.IndexOf(",", CommaIndex + 1);
            int CommaIndex3 = str.IndexOf(",", CommaIndex2 + 1);

            String Component1, Component2, Component3, Component4;
            Component1 = str.Substring(0, CommaIndex);
            Component2 = str.Substring(CommaIndex + 1, CommaIndex2 - CommaIndex - 1);
            Component3 = str.Substring(CommaIndex2 + 1, CommaIndex3 - CommaIndex2 - 1);
            Component4 = str.Substring(CommaIndex3 + 1, str.Length - CommaIndex3 - 1);

            Color clr = new Color(byte.Parse(Component1), byte.Parse(Component2), byte.Parse(Component3), byte.Parse(Component4));

            return clr;
        }

        public static EzSound ParseToEzSound(String str)
        {
            int LineIndex = str.IndexOf("|");

            String Component1, Component2;
            Component1 = str.Substring(0, LineIndex);
            Component2 = str.Substring(LineIndex + 1, str.Length - LineIndex - 1);

            return NewSound(ParseToFileName(Component1), float.Parse(Component2));
        }

        public static EzSound NewSound(string name, float volume)
        {
            EzSound snd = new EzSound();
            snd.sound = Tools.SoundWad.FindByName(name).sound;
            snd.DefaultVolume = volume;
            snd.MaxInstances = 4;

            Tools.PrivateSoundWad.SoundList.Add(snd);

            return snd;
        }

        /// <summary>
        /// Returns the substring inside two quotations.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String ParseToFileName(String str)
        {
            int Quote1 = str.IndexOf("\"");
            int Quote2 = str.IndexOf("\"", Quote1 + 1);

            String Name = str.Substring(Quote1 + 1, Quote2 - Quote1 - 1);
            return Name;
        }

        /// <summary>
        /// Increases the number of phsx steps taken per frame.
        /// </summary>
        public static void IncrPhsxSpeed()
        {
            Tools.PhsxSpeed += 1;
            if (Tools.PhsxSpeed > 3) Tools.PhsxSpeed = 0;
        }

        /// <summary>
        /// Returns the number of elements in an enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int Length<T>()
        {
            return GetValues<T>().Count();
        }


        public static IEnumerable<T> GetValues<T>()
        {
            return (from x in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public)
                    select (T)x.GetValue(null));
        }

        public static byte FloatToByte(float x)
        {
            if (x <= 0) return (byte)0;
            if (x >= 1) return (byte)255;
            return (byte)(255 * x);
        }



        /// <summary>
        /// Moves the object to the specified location. Uses IObject.Move
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pos"></param>
        public static void MoveTo(ObjectBase obj, Vector2 pos)
        {
            obj.Move(pos - obj.Core.Data.Position);
        }

        /// <summary>
        /// Takes in world coordinates and returns screen coordinates.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static Vector2 ToScreenCoordinates(Vector2 pos, Camera cam, Vector2 ZoomMod)
        {
            Vector2 loc = Vector2.Zero;
            loc.X = (pos.X - cam.Data.Position.X) / cam.AspectRatio * cam.Zoom.X * ZoomMod.X;
            loc.Y = (pos.Y - cam.Data.Position.Y) * cam.Zoom.Y * ZoomMod.Y;

            loc.X *= cam.ScreenWidth / 2;
            loc.Y *= -cam.ScreenHeight / 2;

            loc.X += cam.ScreenWidth / 2;
            loc.Y += cam.ScreenHeight / 2;

            return loc;
        }

        /// <summary>
        /// Takes in screen coordinates and returns world coordinates.
        /// (0,0) corresponds to the top left corner of the screen.
        /// </summary>
        public static Vector2 ToGUICoordinates(Vector2 pos, Camera cam)
        {
            return ToWorldCoordinates(pos, cam, new Vector2(.001f, .001f));
        }
        public static Vector2 ToGUICoordinates(Vector2 pos, Camera cam, Vector2 zoom)
        {
            return ToWorldCoordinates(pos, cam, zoom);
        }

        /// <summary>
        /// Takes in screen coordinates and returns world coordinates.
        /// (0,0) corresponds to the top left corner of the screen.
        /// </summary>
        public static Vector2 ToWorldCoordinates(Vector2 pos, Camera cam)
        {
            return ToWorldCoordinates(pos, cam, cam.Zoom);
        }
        public static Vector2 ToWorldCoordinates(Vector2 pos, Camera cam, Vector2 zoom)
        {
            pos.X -= cam.ScreenWidth / 2;
            pos.Y -= cam.ScreenHeight / 2;

            pos.X /= cam.ScreenWidth / 2;
            pos.Y /= -cam.ScreenHeight / 2;

            pos.X = pos.X * cam.AspectRatio / zoom.X + cam.Data.Position.X;
            pos.Y = pos.Y / zoom.Y + cam.Data.Position.Y;

            return pos;
        }

        /// <summary>
        /// Incremented when StartGUIDraw is called.
        /// Decremented when EndGUIDraw is called.
        /// </summary>
        static int GUIDraws = 0;

        static float HoldIllumination;
        /// <summary>
        /// Call before drawing GUI elements unaffected by the camera.
        /// </summary>
        public static void StartGUIDraw()
        {
            GUIDraws++;
            if (GUIDraws > 1) return;

            // Start the GUI drawing if this is the first call to GUIDraw
            Tools.CurLevel.MainCamera.SetToDefaultZoom();

            // Save global illumination level
            HoldIllumination = Tools.QDrawer.GlobalIllumination;
            Tools.QDrawer.GlobalIllumination = 1f;
        }

        /// <summary>
        /// Call after finishing drawing GUI elements unaffected by the camera.
        /// </summary>
        public static void EndGUIDraw()
        {
            GUIDraws--;
            if (GUIDraws > 0) return;

            // End the GUI drawing if this is the last call to GUIDraw
            Tools.CurLevel.MainCamera.RevertZoom();

            // Restor global illumination level
            Tools.QDrawer.GlobalIllumination = HoldIllumination;
        }

        /// <summary>
        /// Starts the SpriteBatch if it isn't started already. The quad drawer is flushed first.
        /// </summary>
        public static void StartSpriteBatch() { StartSpriteBatch(false); }
        public static void StartSpriteBatch(bool AsPaint)
        {
            if (!Tools.Render.UsingSpriteBatch)
            {
                Tools.QDrawer.Flush();
                //Tools.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                //float scale = 1440f / 1280f;
                //float scale = 800f / 1280f;
                float scale = Tools.Render.SpriteScaling;

                if (AsPaint)
                {
                    PaintEffect_SpriteBatch.Parameters["xTexture"].SetValue(Tools.TextureWad.FindByName("PaintSplotch").Tex);
                    //PaintEffect_SpriteBatch.Parameters["SceneTexture"].SetValue(Tools.TextureWad.FindByName("PaintSplotch").Tex); 
                    Tools.Render.MySpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, Tools.PaintEffect_SpriteBatch, Matrix.CreateScale(scale, scale, 1f));
                }
                else
                    Tools.Render.MySpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.CreateScale(scale, scale, 1f));

                Tools.Render.UsingSpriteBatch = true;
            }
        }

        /// <summary>
        /// Core wrapper for drawing text. Assumes SpriteBatch is started.
        /// </summary>
        public static void DrawText(Vector2 pos, Camera cam, string str, SpriteFont font)
        {
            Vector2 loc = ToScreenCoordinates(pos, cam, Vector2.One);

            Tools.Render.MySpriteBatch.DrawString(font, str, loc, Color.Azure, 0, Vector2.Zero, new Vector2(.5f, .5f), SpriteEffects.None, 0);
        }

        public static void SetDefaultEffectParams(float AspectRatio)
        {
            foreach (EzEffect fx in EffectWad.EffectList)
            {
                fx.xCameraAspect.SetValue(AspectRatio);
                fx.effect.CurrentTechnique = fx.Simplest;
                fx.t.SetValue(Tools.t);
                fx.Illumination.SetValue(1f);
                fx.FlipVector.SetValue(new Vector2(-1, -1));
            }

            Matrix colorm;

        colorm = ColorHelper.LinearColorTransform(0); // Green
        //colorm = HsvTransform(1.3f, 1.2f, 100); // Gold
        //colorm = HsvTransform(1.5f, 1.5f, 100); // Gold 2
        //colorm = HsvTransform(1.3f, 1.2f, 200); // Hot pink
        //colorm = HsvTransform(1.3f, 1.2f, 250); // ?
        //colorm = HsvTransform(.5f, 0f, 0); // Black
        //colorm = HsvTransform(.15f, 0f, 0); // Dark Black
        //colorm = HsvTransform(.75f, 0f, 0); // Gray
        //colorm = HsvTransform(.8f, 1.3f, 225); // Purple
        //colorm = HsvTransform(.9f, 1.3f, 110); // Orange
        //colorm = LinearColorTransform(45); // Teal
        //colorm = LinearColorTransform(120); // Blue
        //colorm = HsvTransform(.95f, 1.3f, 0) * LinearColorTransform(240); // Red
        //colorm = HsvTransform(1.25f, 1.3f, 0) * LinearColorTransform(305); // Yellow

            HslGreenEffect.effect.Parameters["ColorMatrix"].SetValue(colorm);
            HslEffect.effect.Parameters["ColorMatrix"].SetValue(colorm);
            
            //colorm = HsvTransform(1f, 1f, 30) * 
            //        new Matrix(.6f, .6f, .6f, 0,
            //                    0, 0, 0, 0,
            //                    0, 0, 0, 0,
            //                    0, 0, 0, 1);
            //colorm = HsvTransform(.7f, 1f, 160);
            //colorm = HsvTransform(Num_0_to_2, 1f, Num_0_to_360);
            //colorm = HsvTransform(1f, 1f, 200);
        }

        public static float Num_0_to_360 = 0;
        public static float Num_0_to_2 = 0;
        public static bool ShowNums = false;

        public static void ModNums()
        {
            if (ButtonCheck.State(XnaInput.Keys.D1).Down)
                Num_0_to_360 = CoreMath.Restrict(0, 360, Num_0_to_360 + .1f * Tools.DeltaMouse.X);
            if (ButtonCheck.State(XnaInput.Keys.D2).Down)
                Num_0_to_2 = CoreMath.Restrict(0, 2, Num_0_to_2 + .001f * Tools.DeltaMouse.X);

            if (ButtonCheck.State(XnaInput.Keys.D1).Down || ButtonCheck.State(XnaInput.Keys.D2).Down)
                ShowNums = true;
        }


        public static bool DebugConvenience;// = true;

        /// <summary>
        /// Set a player's controller to vibrate.
        /// </summary>
        /// <param name="Index">The index of the player.</param>
        /// <param name="LeftMotor">The intensity of the left motor vibration (from 0.0 to 1.0)</param>
        /// <param name="RightMotor">The intensity of the left motor vibration (from 0.0 to 1.0)</param>
        /// <param name="Duration">The number of frames the vibration should persist.</param>
        public static void SetVibration(PlayerIndex Index, float LeftMotor, float RightMotor, int Duration)
        {
            if (DebugConvenience) return;

            VibrateTimes[(int)Index] = Duration;
            XnaInput.GamePad.SetVibration(Index, LeftMotor, RightMotor);
        }

        public static void UpdateVibrations()
        {
            for (int i = 0; i < 4; i++)
            {
                if (VibrateTimes[i] > 0)
                {
                    VibrateTimes[i]--;
                    if (VibrateTimes[i] <= 0)
                        SetVibration((PlayerIndex)i, 0, 0, 0);
                }
            }
        }

        public static Vector2[] FloatArrayToVectorArray_y(float[] v)
        {
            Vector2[] vec = new Vector2[v.Length];
            for (int i = 0; i < v.Length; i++)
                vec[i] = new Vector2(0, v[i]);
            return vec;
        }

        public static bool IncrementsContainsSum(int[] Incr, int S)
        {
            int Sum = 0;
            for (int i = 0; i < Incr.Length; i++)
            {
                Sum += Incr[i];
                if (Sum == S)
                    return true;
            }

            return false;
        }

        public static void UseInvariantCulture()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }

        public static bool _AllTaken(bool[] list1, bool[] list2, int Length)
        {
            for (int i = 0; i < Length; i++)
                if (!list1[i] && list2[i])
                    return false;
            return true;
        }

        public static string ScoreString(int num, int outof)
        {
            return num.ToString() + "/" + outof.ToString();
            //return "x" + num.ToString() + "/" + outof.ToString();
        }
        public static string ScoreString(int num)
        {
            return num.ToString();
            //return "x" + num.ToString();
        }

        /// <summary>
        /// Moves an IObject/IBound object to within BL.X and TR.X
        /// </summary>
        /// <param name="obj">The IObject to be moved. Must also be IBound</param>
        /// <param name="BL">BL extent of the bounded area</param>
        /// <param name="TR">TR extent of the bounded area</param>
        public static void EnsureBounds_X(IBound obj, Vector2 TR, Vector2 BL)
        {
            float TR_Bound = obj.TR_Bound().X;
            if (TR_Bound > TR.X)
                obj.MoveToBounded(new Vector2(TR.X - TR_Bound, 0));
            else
            {
                float BL_Bound = obj.BL_Bound().X;
                if (BL_Bound < BL.X)
                    obj.MoveToBounded(new Vector2(BL.X - BL_Bound, 0));
            }
        }
        /// <summary>
        /// Moves an IObject/IBound object to within BL.Y and TR.Y
        /// </summary>
        /// <param name="obj">The IObject to be moved. Must also be IBound</param>
        /// <param name="BL">BL extent of the bounded area</param>
        /// <param name="TR">TR extent of the bounded area</param>
        public static void EnsureBounds_Y(IBound obj, Vector2 TR, Vector2 BL)
        {
            float TR_Bound = obj.TR_Bound().Y;
            if (TR_Bound > TR.Y)
                obj.MoveToBounded(new Vector2(0, TR.Y - TR_Bound));
            else
            {
                float BL_Bound = obj.BL_Bound().Y;
                if (BL_Bound < BL.Y)
                    obj.MoveToBounded(new Vector2(0, BL.Y - BL_Bound));
            }
        }
        /// <summary>
        /// Moves an IObject/IBound object to within BL and TR
        /// </summary>
        /// <param name="obj">The IObject to be moved. Must also be IBound</param>
        /// <param name="BL">BL extent of the bounded area</param>
        /// <param name="TR">TR extent of the bounded area</param>
        public static void EnsureBounds(IBound obj, Vector2 TR, Vector2 BL)
        {
            Vector2 TR_Bound = obj.TR_Bound();
            Vector2 BL_Bound = obj.BL_Bound();
            
            if (TR_Bound.X > TR.X)
                obj.MoveToBounded(new Vector2(TR.X - TR_Bound.X, 0));
            else if (BL_Bound.X < BL.X)
                obj.MoveToBounded(new Vector2(BL.X - BL_Bound.X, 0));

            if (TR_Bound.Y > TR.Y)
                obj.MoveToBounded(new Vector2(0, TR.Y - TR_Bound.Y));
            else if (BL_Bound.Y < BL.Y)
                obj.MoveToBounded(new Vector2(0, BL.Y - BL_Bound.Y));
        }
    }
}