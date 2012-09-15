using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public struct Version : IComparable
    {
        public int MajorVersion, MinorVersion, SubVersion;
        public Version(int Major, int Minor, int Sub)
        {
            MajorVersion = Major;
            MinorVersion = Minor;
            SubVersion = Sub;
        }

        public int CompareTo(object o)
        {
            Version v = (Version)o;

            if (v.MajorVersion == MajorVersion)
            {
                if (v.MinorVersion == MinorVersion)
                    return this.SubVersion.CompareTo(v.SubVersion);
                else
                    return this.MinorVersion.CompareTo(v.MinorVersion);
            }
            else
                return this.MajorVersion.CompareTo(v.MajorVersion);
        }

        public static bool operator >(Version v1, Version v2) { return v1.CompareTo(v2) > 0; }
        public static bool operator >=(Version v1, Version v2) { return v1.CompareTo(v2) >= 0; }
        public static bool operator <(Version v1, Version v2) { return v1.CompareTo(v2) < 0; }
        public static bool operator <=(Version v1, Version v2) { return v1.CompareTo(v2) <= 0; }
    }
}