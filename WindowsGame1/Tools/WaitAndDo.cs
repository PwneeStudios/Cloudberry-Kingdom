using System;
using System.Collections.Generic;

namespace CloudberryKingdom
{
    public class WaitAndDo
    {
        public int WaitLength, Count;
        Func<bool> MyFunc;

        public bool Done = false;

        public WaitAndDo(int WaitLength, Func<bool> Function)
        {
            this.WaitLength = WaitLength;
            MyFunc = Function;
        }

        public void PhsxStep()
        {
            Count++;
            if (Count >= WaitLength)
            {
                MyFunc();

                Done = true;
            }
        }
    }

    public class WaitAndDoList : List<WaitAndDo>
    {
        public WaitAndDoList()
        {
        }

        public void Add(int WaitLength, Func<bool> Function)
        {
            this.Add(new WaitAndDo(WaitLength, Function));
        }

        public void PhsxStep()
        {
            foreach (WaitAndDo item in this)
                item.PhsxStep();

            this.RemoveAll(item => item.Done);
        }
    }
}