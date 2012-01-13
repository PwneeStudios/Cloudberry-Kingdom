using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.IO;
using Drawing;

namespace CloudberryKingdom
{
    public class InfoWad
    {
        public static Dictionary<String, Vector2> Vector2Dic;
        public static Dictionary<String, String> StringDic;
        public static Dictionary<String, float> floatDic;
        public static Dictionary<String, Color> ColorDic;
        public static Dictionary<String, EzSound> SoundDic;

        public static void Init()
        {
            Vector2Dic = new Dictionary<string, Vector2>();
            StringDic = new Dictionary<string, string>();
            floatDic = new Dictionary<string, float>();
            ColorDic = new Dictionary<string, Color>();
            SoundDic = new Dictionary<string, EzSound>();
        }

        public static Vector2 GetVec(String str) { return Vector2Dic[str]; }
        public static String GetStr(String str) { return StringDic[str]; }
        public static float GetFloat(String str) { return floatDic[str]; }
        public static Color GetColor(String str) { return ColorDic[str]; }
        public static EzSound GetSound(String str) { return SoundDic[str]; }

        public static string GetVarName(string line)
        {
            int Index = line.IndexOf('#');
            if (line.Contains(' '))
            {
                int EndIndex = line.IndexOf(' ', Index);
                if (EndIndex <= 0) EndIndex = line.Length;
                return line.Substring(Index + 1, EndIndex - Index - 1);
            }
            else
                return line.Substring(Index + 1);
        }

        public static void Read(String file)
        {
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader reader = new StreamReader(stream);

            String line;
            String type, key;

            line = reader.ReadLine();
            while (line != null)
            {
                line = Tools.RemoveComment(line);

                if (line.StartsWith("["))
                {
                    int EndIndex = line.IndexOf("]");
                    int EndIndex2 = line.IndexOf("]", EndIndex + 1);
                    
                    type = line.Substring(1, EndIndex - 1);
                    key = line.Substring(EndIndex + 2, EndIndex2 - 1 - EndIndex - 1);
                    
                    string _line = line.Substring(EndIndex2 + 1, line.Length - EndIndex2 - 1);

                    // Check if value is a variable
                    if (_line.Contains('#'))
                    {
                        string VarName = GetVarName(_line);
                        switch (type)
                        {
                            case "vec": Vector2Dic[key] = Vector2Dic[VarName]; break;
                            case "str": StringDic[key] = StringDic[VarName]; break;
                            case "dec": floatDic[key] = floatDic[VarName]; break;
                            case "clr": ColorDic[key] = ColorDic[VarName]; break;
                            case "snd": SoundDic[key] = SoundDic[VarName]; break;

                            default: throw (new Exception("Unknown variable type in .infowad file"));
                        }
                    }
                    else
                        switch (type)
                        {
                            case "vec": Vector2Dic[key] = Tools.ParseToVector2(_line); break;
                            case "str": StringDic[key] = Tools.ParseToFileName(_line); break;
                            case "dec": floatDic[key] = float.Parse(_line); break;
                            case "clr": ColorDic[key] = Tools.ParseToColor(_line); break;
                            case "snd": SoundDic[key] = Tools.ParseToEzSound(_line); break;

                            default: throw (new Exception("Unknown variable type in .infowad file"));
                        }
                }

                line = reader.ReadLine();
            }

            reader.Close();
            stream.Close();
        }
    }
}