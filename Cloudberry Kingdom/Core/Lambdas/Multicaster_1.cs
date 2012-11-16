using System;
using System.Collections.Generic;



namespace CloudberryKingdom
{
    public class Multicaster_1<T> : Lambda_1<T>
    {
        List<Lambda_1<T>> MyList = new List<Lambda_1<T>>();

        public void Apply(T t)
        {
            foreach (Lambda_1<T> L in MyList)
                L.Apply(t);
        }

        public void Clear()
        {
            MyList.Clear();
        }

        public void Add(Lambda_1<T> L)
        {
            MyList.Add(L);
        }
    }
}