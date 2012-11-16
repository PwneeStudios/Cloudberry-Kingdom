using System;
using System.Collections.Generic;



namespace CloudberryKingdom
{
    public class Multicaster : Lambda
    {
        List<Lambda> MyList = new List<Lambda>();

        public void Apply()
        {
            foreach (Lambda L in MyList)
                L.Apply();
        }

        public void Clear()
        {
            MyList.Clear();
        }

        public void Add(Lambda L)
        {
            MyList.Add(L);
        }

        public void Remove(Lambda L)
        {
        }
    }
}