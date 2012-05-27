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
using Drawing;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Blocks;

#if WINDOWS
using CloudberryKingdom;
using CloudberryKingdom.Viewer;
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
        }


        public static void Nothing() { }

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

public static string SourceTextureDirectory()
{
    return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()))), "Content\\Art");
}

public static Thread EasyThread(int affinity, string name, Action action)
{
    Thread LoadThread = new Thread(
        new ThreadStart(
            delegate
            {
#if XBOX
                        Thread.CurrentThread.SetProcessorAffinity(new[] { affinity });
#endif
                LoadThread = Thread.CurrentThread;
                EventHandler<EventArgs> abort = (s, e) =>
                {
                    if (LoadThread != null)
                    {
                        LoadThread.Abort();
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

    LoadThread.Start();

    return LoadThread;
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

        public static Color GrayColor(float val) { return new Color(Gray(val)); }

        public static Vector4 Gray(float val)
        {
            return new Vector4(val, val, val, 1);
        }

        public static int Modulo(int n, int p)
        {
            int M = n % p;
            if (M < 0) M += p;
            return M;
        }

        public static float Modulo(float n, float p)
        {
            float M = n - (int)(n / p) * p;
            if (M < 0) M += p;
            return M;
        }

        public static float ZigZag(float period, float t)
        {
            float p = period / 2;
            int n = (int)Math.Floor(t / p);
            t -= n * p;
            if (n % 2 == 0) return t / p;
            else return 1 - t / p;
        }

        public static float PeriodicCentered(float A, float B, float period, float t)
        {
            float Normalized = .5f + .5f * (float)Math.Sin(t * 2 * Math.PI / period);
            return A + (B - A) * Normalized;
        }

        public static float Periodic(float A, float B, float period, float t)
        {
            return Periodic(A, B, period, t, 0);
        }
        public static float Periodic(float A, float B, float period, float t, float PhaseDegree)
        {
            float Normalized = .5f - .5f * (float)Math.Cos(t * 2 * Math.PI / period + Tools.Radians(PhaseDegree));
            return A + (B - A) * Normalized;
        }
        public static Vector2 Periodic(Vector2 A, Vector2 B, float period, float t)
        {
            float Normalized = .5f - .5f * (float)Math.Cos(t * 2 * Math.PI / period);
            return A + (B - A) * Normalized;
        }

        public static string Time(int frames)
        {
            int h = Hours(frames);
            int m = Minutes(frames);
            int s = Seconds(frames);
            int mi = Milliseconds(frames);

            //return string.Format("{0:0}h:{1:00}m:{2:00}.{3:00}", h, m, s, mi);
            return string.Format("{0:0}:{1:00}:{2:00}.{3:00}", h, m, s, mi);
        }

        public static int Hours(int frames)
        {
            return (int)(frames / (60 * 60 * 62));
        }

        public static int Minutes(int frames)
        {
            float Remainder = frames - 60 * 60 * 62 * Hours(frames);
            return (int)(Remainder / (60 * 62));
        }

        public static int Seconds(int frames)
        {
            float Remainder = frames - 60 * 60 * 62 * Hours(frames) - 60 * 62 * Minutes(frames);
            return (int)(Remainder / 62);
        }

        public static int Milliseconds(int frames)
        {
            float Remainder = frames - 60 * 60 * 62 * Hours(frames) - 60 * 62 * Minutes(frames) - 62 * Seconds(frames);
            return (int)(100 * Remainder / 62f);
        }

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        public static float Degrees(float Radians)
        {
            return (float)(Radians * 180 / Math.PI);
        }

        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        public static float Radians(float Degrees)
        {
            return (float)(Degrees * Math.PI / 180);
        }

        public static float c = 180f / (float)Math.PI;
        public static float RadianDist(float a1, float a2)
        {
            return AngleDist(c * a1, c * a2);
        }
        public static float AngleDist(float a1, float a2)
        {
            float dist = Math.Abs(a1 - a2);
            dist = dist % 360f;
            return Math.Min(dist, 360 - dist); 
        }
        public static float VectorToAngle(Vector2 v)
        {
            return (float)Math.Atan2(v.Y, v.X);
        }
        public static Vector2 CartesianToPolar(Vector2 v)
        {
            Vector2 polar = new Vector2(VectorToAngle(v),
                                        polar.Y = v.Length());
            return polar;
        }
        public static Vector2 PolarToCartesian(Vector2 v)
        {
            return v.Y * AngleToDir(v.X);
        }

        /// <summary>
        /// Converts an angle in radians to a normalized direction vector.
        /// </summary>
        /// <param name="Angle">The angle in radians.</param>
        /// <returns></returns>
        public static Vector2 AngleToDir(double Angle)
        {
            return new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
        }

        /// <summary>
        /// Converts an angle in degrees to a normalized direction vector.
        /// </summary>
        /// <param name="Angle">The angle in degrees.</param>
        /// <returns></returns>
        public static Vector2 DegreesToDir(double Angle)
        {
            return AngleToDir(Math.PI / 180f * Angle);
        }


        public static float Snap(float x, float spacing)
        {
            return spacing * (int)(x / spacing);
        }
        public static Vector2 Snap(Vector2 x, Vector2 spacing)
        {
            return new Vector2(Snap(x.X, spacing.X), Snap(x.Y, spacing.Y));
        }

        /// <summary>
        /// Calculate the sup norm of a vector.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float SupNorm(Vector2 v)
        {
            return Math.Max(Math.Abs(v.X), Math.Abs(v.Y));
        }

        public static Vector2 Sign(Vector2 v)
        {
            return new Vector2(Math.Sign(v.X), Math.Sign(v.Y));
        }


        public static float Max(params float[] vals)
        {
            float max = vals[0];
            for (int i = 1; i < vals.Length; i++)
                max = Math.Max(max, vals[i]);
            return max;
        }

        /// <summary>
        /// Take in two vectors and find both the max and the min.
        /// </summary>
        public static void MaxAndMin(ref Vector2 p1, ref Vector2 p2, ref Vector2 Max, ref Vector2 Min)
        {
            if (p1.X > p2.X)
            {
                Max.X = p1.X;
                Min.X = p2.X;
            }
            else
            {
                Max.X = p2.X;
                Min.X = p1.X;
            }

            if (p1.Y > p2.Y)
            {
                Max.Y = p1.Y;
                Min.Y = p2.Y;
            }
            else
            {
                Max.Y = p2.Y;
                Min.Y = p1.Y;
            }
        }

        /// <summary>
        /// Returns the component-wise absolute value of the Vector2
        /// </summary>
        public static Vector2 Abs(Vector2 v)
        {
            return new Vector2(Math.Abs(v.X), Math.Abs(v.Y));
        }

        /// <summary>
        /// Checks whether two points are close to each other.
        /// </summary>
        /// <param name="v1">The first point.</param>
        /// <param name="v2">The second point.</param>
        /// <param name="Cutoff">The cutoff range.</param>
        /// <returns></returns>
        public static bool Close(Vector2 v1, Vector2 v2, Vector2 Cutoff)
        {
            if (v1.X > v2.X + Cutoff.X) return false;
            if (v1.X < v2.X - Cutoff.X) return false;
            if (v1.Y > v2.Y + Cutoff.Y) return false;
            if (v1.Y < v2.Y - Cutoff.Y) return false;
            return true;
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

        public static Vector2 Restrict(float min, float max, Vector2 val)
        {
            val.X = Restrict(min, max, val.X);
            val.Y = Restrict(min, max, val.Y);

            return val;
        }

        /// <summary>
        /// Restrict a value between a min and a max.
        /// </summary>
        public static float Restrict(float min, float max, float val)
        {
            if (val > max) val = max;
            if (val < min) val = min;
            
            return val;
        }
        public static void Restrict(float min, float max, ref float val)
        {
            val = Restrict(min, max, val);
        }

        /// <summary>
        /// Restrict a value between a min and a max.
        /// </summary>
        public static int Restrict(int min, int max, int val)
        {
            if (val > max) val = max;
            if (val < min) val = min;

            return val;
        }
        public static void Restrict(int min, int max, ref int val)
        {
            val = Restrict(min, max, val);
        }

        public static SimpleObject LoadSimpleObject(string file)
        {
            ObjectClass SourceObject;
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
        public static Viewer.Viewer viewer;
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

        public static CloudberryKingdom.CloudberryKingdomGame TheGame;
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

        public static bool UsingSpriteBatch;
        public static SpriteBatch spriteBatch;
        public static EzFont LilFont, ScoreTextFont;
        public static EzFont MonoFont;
        public static EzFont Font_Dylan60, Font_Dylan42, Font_Dylan20, Font_Dylan15, Font_Dylan24, Font_Dylan28;
        public static EzFont Font_DylanThick20, Font_DylanThick24, Font_DylanThick28;
        public static EzFont Font_DylanThin42;

        public static int[] VibrateTimes = { 0, 0, 0, 0 };

        public static int DifficultyTypes = Tools.GetValues<DifficultyParam>().Count();//Enum.GetValues(typeof(DifficultyType)).Length;
        public static int StyleTypes = 8;
        public static int UpgradeTypes = Tools.GetValues<Upgrade>().Count();//Enum.GetValues(typeof(Upgrade)).Length;

#if WINDOWS
        public static XnaInput.KeyboardState keybState, PrevKeyboardState;
        public static XnaInput.MouseState CurMouseState, PrevMouseState;
        public static Vector2 DeltaMouse;
        public static int DeltaScroll;
        public static bool Editing;

        public static Vector2 MousePos
        {
            get { return new Vector2(CurMouseState.X, CurMouseState.Y) / TheGame.SpriteScaling; }
            set { XnaInput.Mouse.SetPosition((int)(value.X * TheGame.SpriteScaling), (int)(value.Y  * TheGame.SpriteScaling)); }
        }

        public static bool Fullscreen
        {
            get { return TheGame.graphics.IsFullScreen; }
            set
            {
                if (value != Fullscreen)
                {
                    if (value)
                    {
                        int width = Tools.TheGame.graphics.PreferredBackBufferWidth,
                            height = Tools.TheGame.graphics.PreferredBackBufferHeight;

                        var safe = ResolutionGroup.SafeResolution(width, height);
                        width = safe.X;
                        height = safe.Y;
                        
                        ResolutionGroup.Use(width, height, false);
                    }

                    TheGame.graphics.ToggleFullScreen();

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
#else

#endif
        public static XnaInput.GamePadState[] padState, PrevpadState;

        public static EzTexture Texture(string name) { return TextureWad.FindByName(name); }
        public static EzSound Sound(string name) { return SoundWad.FindByName(name); }
        public static void Pop() { Pop(2); }
        public static void Pop(int Pitch) { Sound("Pop " + Restrict(1, 3, Pitch).ToString()).Play(); }

        public static GameTime gameTime;
        //public static Random Rnd;
        public static Rand GlobalRnd = new Rand(0);
        public static EzEffectWad EffectWad;
        public static EzEffect BasicEffect, NoTexture, CircleEffect, RainEffect, LightSourceEffect;
        public static Effect PaintEffect_SpriteBatch;
        public static EzTextureWad TextureWad;
        public static ContentManager SoundContentManager;
        public static EzSoundWad SoundWad, PrivateSoundWad;
        public static EzSongWad SongWad;
        public static QuadDrawer QDrawer;
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
        public static LoadingScreen CurrentLoadingScreen;

        public static void LoadBasicArt(ContentManager Content)
        {
            TextureWad = new EzTextureWad();
            TextureWad.AddTexture(Content.Load<Texture2D>("White"), "White");
            TextureWad.AddTexture(Content.Load<Texture2D>("Circle"), "Circle");
            TextureWad.AddTexture(Content.Load<Texture2D>("Smooth"), "Smooth");

#if PC_VERSION
            TextureWad.AddTexture(Content.Load<Texture2D>("ControllerScreen_PC"), "ControllerScreen");
#else
            TextureWad.AddTexture(Content.Load<Texture2D>("ControllerScreen"), "ControllerScreen");
#endif            

            TextureWad.DefaultTexture = TextureWad.TextureList[0];
        }

        public static string GetFileName(String FilePath)
        {
            int i = FilePath.LastIndexOf("\\");
            int j = FilePath.IndexOf(".", i);
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


        public static void PreloadArt(ContentManager Content)
        {
            String path = Path.Combine(Globals.ContentDirectory, "Art");
            string[] files = GetFiles(path, true);

            foreach (String file in files)
            {
#if EMBEDDEDLOAD
                if (GetFileExt(path, file) == "xnb")
                {
                    Tools.TextureWad.AddTexture(null, "Art\\" + GetFileName(path, file));
                }
#else
                string extension = GetFileExt(path, file);
                if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "bmp")
                {
                    TextureWad.AddTexture(Texture2D.FromFile(Tools.Device, file), GetFileName(path, file));
                }
#endif
            }
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
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\WaterBob"), "WaterBob");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\Paint"), "Paint");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\Lava"), "Lava");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\LightMap"), "LightMap");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\Rain"), "Rain");
            EffectWad.AddEffect(Content.Load<Effect>("Effects\\LightSource"), "LightSource");

            BasicEffect = EffectWad.EffectList[0];
            NoTexture = EffectWad.EffectList[1];
            CircleEffect = EffectWad.EffectList[2];
            RainEffect = EffectWad.FindByName("Rain");
            LightSourceEffect = EffectWad.FindByName("LightSource");

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
            CurrentLoadingScreen = new LoadingScreen();
            CurrentLoadingScreen.Start();
        }

        public static void BeginLoadingScreen_Fake(int Length)
        {
            LoadingScreen.MinLoadLength = Length;

            DoNotKillMusicOnNextLoadingscreen = false;

            Tools.ShowLoadingScreen = true;
            CurrentLoadingScreen = new LoadingScreen();
            CurrentLoadingScreen.Start();
            CurrentLoadingScreen.Fake = true;
        }

        public static void PlayHappyMusic()
        {
            Tools.SongWad.SetPlayList(Tools.Song_Happy);
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

        public static string RemoveComment(String str)
        {
            int CommentIndex = str.IndexOf("//");
            if (CommentIndex > 0)
                str = str.Substring(0, CommentIndex);
            return str;
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

            EzSound snd = new EzSound();
            snd.sound = Tools.SoundWad.FindByName(ParseToFileName(Component1)).sound;
            snd.DefaultVolume = float.Parse(Component2);
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
            if (!UsingSpriteBatch)
            {
                Tools.QDrawer.Flush();
                //Tools.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                //float scale = 1440f / 1280f;
                //float scale = 800f / 1280f;
                float scale = TheGame.SpriteScaling;

                if (AsPaint)
                {
                    PaintEffect_SpriteBatch.Parameters["xTexture"].SetValue(Tools.TextureWad.FindByName("PaintSplotch").Tex);
                    //PaintEffect_SpriteBatch.Parameters["SceneTexture"].SetValue(Tools.TextureWad.FindByName("PaintSplotch").Tex);
                    Tools.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, Tools.PaintEffect_SpriteBatch, Matrix.CreateScale(scale, scale, 1f));
                }
                else
                    Tools.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.CreateScale(scale, scale, 1f));

                UsingSpriteBatch = true;
            }
        }

        /// <summary>
        /// Core wrapper for drawing text. Assumes SpriteBatch is started.
        /// </summary>
        public static void DrawText(Vector2 pos, Camera cam, string str, SpriteFont font)
        {
            Vector2 loc = ToScreenCoordinates(pos, cam, Vector2.One);

            Tools.spriteBatch.DrawString(font, str, loc, Color.Azure, 0, Vector2.Zero, new Vector2(.5f, .5f), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Sets the standard render states.
        /// </summary>
        public static void SetStandardRenderStates()
        {
            Tools.QDrawer.SetAddressMode(true, true);
            // All these renderstates need to be ported to XNA 4.0
            /*
            Tools.Device.RenderState.DepthBufferEnable = true;

            Tools.Device.RenderState.AlphaBlendEnable = true;
            Tools.Device.RenderState.CullMode = CullMode.None;
            Tools.Device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            Tools.Device.RenderState.SourceBlend = Blend.SourceAlpha;

            Tools.Device.RenderState.AlphaSourceBlend = Blend.One;
            Tools.Device.RenderState.AlphaDestinationBlend = Blend.One;
             * */

            Tools.Device.RasterizerState = RasterizerState.CullNone;
            //Tools.Device.BlendState = BlendState.NonPremultiplied;
            Tools.Device.BlendState = BlendState.AlphaBlend;
            Tools.Device.DepthStencilState = DepthStencilState.DepthRead;

            ResetViewport();
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
        }

        public static void ResetViewport()
        {
            Tools.TheGame.graphics.GraphicsDevice.Viewport = Tools.TheGame.MainViewport;
        }

        /// <summary>
        /// Ends the SpriteBatch, if in use, and resets standard render states.
        /// </summary>
        public static void EndSpriteBatch()
        {
            if (UsingSpriteBatch)
            {
                UsingSpriteBatch = false;

                Tools.spriteBatch.End();

                SetStandardRenderStates();
            }
        }

        /// <summary>
        /// Premultiply a color's alpha against its RGB components.
        /// </summary>
        /// <param name="color">The normal, non-premultiplied color.</param>
        public static Color PremultiplyAlpha(Color color)
        {
            return new Color(color.R, color.G, color.B) * (color.A / 255f);
        }
        /// <summary>
        /// Premultiply a color's alpha against its RGB components.
        /// </summary>
        /// <param name="color">The normal, non-premultiplied color.</param>
        /// <param name="BlendAddRatio">When 0 blending is normal, when 1 blending is additive.</param>
        public static Color PremultiplyAlpha(Color color, float BlendAddRatio)
        {
            Color NewColor = PremultiplyAlpha(color);
            NewColor.A = (byte)(NewColor.A * (1 - BlendAddRatio));

            return NewColor;
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

        /// <summary>
        /// Interpolate between a list of values.
        /// t = 0 returns the first value, t = 1 the second value, etc
        /// </summary>
        public static float MultiLerpRestrict(float t, params float[] values)
        {
            if (t <= 0) return values[0];
            if (t >= values.Length - 1) return values[values.Length - 1];

            int i1 = Math.Min((int)t, values.Length - 1);
            int i2 = i1 + 1;

            return Lerp(values[i1], values[i2], t - i1);
        }

        public static Vector2 MultiLerpRestrict(float t, params Vector2[] values)
        {
            if (t <= 0) return values[0];
            if (t >= values.Length - 1) return values[values.Length - 1];

            int i1 = Math.Min((int)t, values.Length - 1);
            int i2 = i1 + 1;

            return Vector2.Lerp(values[i1], values[i2], t - i1);
        }

        public static int LerpRestrict(int v1, int v2, float t)
        {
            return (int)LerpRestrict((float)v1, (float)v2, t);
        }
        public static int Lerp(int v1, int v2, float t)
        {
            return (int)Lerp((float)v1, (float)v2, t);
        }
        public static float LerpRestrict(float v1, float v2, float t)
        {
            return Lerp(v1, v2, Restrict(0, 1, t));
        }
        public static float Lerp(float v1, float v2, float t)
        {
            return (1 - t) * v1 + t * v2;
        }
        public static Vector2 LerpRestrict(Vector2 v1, Vector2 v2, float t)
        {
            if (t > 1) return v2;
            if (t < 0) return v1;
            return Vector2.Lerp(v1, v2, t);
        }

        /// <summary>
        /// Linear interpolation with the additional property that
        /// if v1 == 0, then t <- Max(0, t - .5)
        /// </summary>
        public static float SpecialLerp(float v1, float v2, float t)
        {
            if (v1 == 0)
                t = (float)Math.Max(0, t - .5);

            return (1 - t) * v1 + t * v2;
        }

        /// <summary>
        /// Special linear interpolation, followed by a restriction between the v0 and v1 values.
        /// </summary>
        public static float SpecialLerpRestrict(float v1, float v2, float t)
        {
            return SpecialLerp(v1, v2, Restrict(0, 1, t));
        }


        public static float Lerp(Vector2 g1, Vector2 g2, float t)
        {
            float width = g2.X - g1.X;
            float s = (t - g1.X) / width;

            return g2.Y * s + g1.Y * (1 - s);
        }
        public static float DifficultyLerp(float Level1Val, float Level10Val, float level)
        {
            return Lerp(new Vector2(1, Level1Val), new Vector2(10, Level10Val), level);
        }
        public static float DifficultyLerpRestrict19(float Level1Val, float Level9Val, float level)
        {
            level = Restrict(1, 9, level);
            return DifficultyLerp19(Level1Val, Level9Val, level);
        }
        public static float DifficultyLerp19(float Level1Val, float Level9Val, float level)
        {
            return Lerp(new Vector2(1, Level1Val), new Vector2(9, Level9Val), level);
        }
        public static Vector2 DifficultyLerpRestrict19(Vector2 Level1Val, Vector2 Level9Val, float level)
        {
            level = Restrict(1, 9, level);
            return DifficultyLerp19(Level1Val, Level9Val, level);
        }
        public static Vector2 DifficultyLerp19(Vector2 Level1Val, Vector2 Level9Val, float level)
        {
            float t = (level - 1f) / (9f - 1f);
            return Vector2.Lerp(Level1Val, Level9Val, t);
        }
        public static float DifficultyLerpRestrict159(float Level1Val, float Level5Val, float Level9Val, float level)
        {
            level = Restrict(1, 9, level);
            return DifficultyLerp159(Level1Val, Level5Val, Level9Val, level);
        }
        public static float DifficultyLerp159(float Level1Val, float Level5Val, float Level9Val, float level)
        {
            return Level1Val * (level - 5f) * (level - 9f) / ((1f - 5f) * (1f - 9f)) +
                   Level5Val * (level - 1f) * (level - 9f) / ((5f - 1f) * (5f - 9f)) +
                   Level9Val * (level - 1f) * (level - 5f) / ((9f - 1f) * (9f - 5f));
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






        public static bool _AllTaken(bool[] list1, bool[] list2, int Length)
        {
            for (int i = 0; i < Length; i++)
                if (!list1[i] && list2[i])
                    return false;
            return true;
        }

        public static Vector2 Reciprocal(Vector2 v)
        {
            return new Vector2(v.Y, -v.X);
        }

        public static void RotatedBasis(float Degrees, ref Vector2 v)
        {
            Vector2 e1 = DegreesToDir(Degrees);
            Vector2 e2 = DegreesToDir(Degrees + 90);

            v = v.X * e1 + v.Y * e2;
        }

        public static void PointyAxisTo(ref BasePoint Base, Vector2 dir)
        {
            PointxAxisTo(ref Base, Reciprocal(dir));
        }
        public static void PointxAxisTo(ref BasePoint Base, Vector2 dir)
        {
            PointxAxisTo(ref Base.e1, ref Base.e2, dir);
        }
        public static void PointxAxisTo(ref Vector2 e1, ref Vector2 e2, Vector2 dir)
        {
            if (dir.Length() < .0001f) return;

            dir.Normalize();

            float l = e1.Length();
            e1 = dir * l;

            l = e2.Length();
            e2.X = -e1.Y;
            e2.Y = e1.X;
            e2.Normalize();
            e2 *= l;
        }
        public static void PointxAxisToAngle(ref BasePoint Base, float angle)
        {
            PointxAxisTo(ref Base, AngleToDir(angle));
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

        /// <summary>
        /// Encode a float as an RGBA color.
        /// </summary>
        public static Vector3 EncodeFloatRGBA(float v)
        {
            const float max24int = 256 * 256 * 256 - 1;
            Vector3 color = new Vector3((float)Math.Floor(v * max24int / (256 * 256)),
                                        (float)Math.Floor(v * max24int / 256),
                                        (float)Math.Floor(v * max24int));

            color.Z -= color.Y * 256f;
            color.Y -= color.X * 256f;

            return color / 255f;
        }

        /// <summary>
        /// Decode an RGBA color into a float (assuming the RGBA was an encoding of a float to start with)
        /// </summary>
        public static float DecodeFloatRGBA(Vector4 rgba)
        {
            return rgba.X * 1f + rgba.Y / 255f + rgba.Z / 65025f + rgba.W / 160581375f;
        }
    }
}