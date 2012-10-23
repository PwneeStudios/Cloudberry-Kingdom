using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System;

using Microsoft.Xna.Framework;

using CoreEngine.Random;

namespace CoreEngine
{
    public class Set<T> : IEnumerable, IEnumerable<T>
    {
        public Dictionary<T, bool> dict = new Dictionary<T,bool>();

        public int Count { get { return dict.Count; } }

        public bool this[T item]
        {
            get { return dict.ContainsKey(item); }
        }

        public static Set<T> operator+(Set<T> set, T item)
        {
            if (!set.dict.ContainsKey(item))
                set.dict.Add(item, true);

            return set;
        }

        IEnumerator IEnumerable.GetEnumerator() { return dict.Keys.GetEnumerator(); }
        public IEnumerator<T> GetEnumerator() { return dict.Keys.GetEnumerator(); }

        public T Choose(Rand Rnd)
        {
            int i = Rnd.RndInt(0, dict.Count - 1);
            return dict.ElementAt(i).Key;
        }

        public bool Contains(T item)
        {
            if (item == null) return true;
            else return dict.ContainsKey(item);
        }

    }
}