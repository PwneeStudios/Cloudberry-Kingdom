using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class Multicaster_2<T1, T2> : Lambda_2<T1, T2>
    {
        List<Lambda_2<T1, T2>> MyList = new List<Lambda_2<T1, T2>>();

        public void Apply(T1 t1, T2 t2)
        {
            foreach (Lambda_2<T1, T2> L in MyList)
                L.Apply(t1, t2);
        }

        public void Clear()
        {
            MyList.Clear();
        }

        public void Add(Lambda_2<T1, T2> L)
        {
            MyList.Add(L);
        }
    }
}