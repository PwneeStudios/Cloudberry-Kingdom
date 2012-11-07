using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class ToDoItem
    {
        public LambdaFunc<bool> MyFunc;
        public string Name;

        public int Step = 0;

        /// <summary>
        /// If true the function will be deleted without executing.
        /// </summary>
        public bool MarkedForDeletion
        {
            get { return _MarkedForDeltion; }
            set { _MarkedForDeltion = value; }
        }
        bool _MarkedForDeltion;

        /// <summary>
        /// Whether the item pauses when the game is paused.
        /// </summary>
        public bool PauseOnPause;

        public bool RemoveOnReset;

        public ToDoItem(LambdaFunc<bool> FuncToDo, string Name, bool PauseOnPause, bool RemoveOnReset)
        {
            MyFunc = FuncToDo;
            this.Name = Name;
            this.PauseOnPause = PauseOnPause;
            this.RemoveOnReset = RemoveOnReset;
        }

        /// <summary>
        /// Mark the function for deletion and prevent execution.
        /// </summary>
        public void Delete()
        {
            MarkedForDeletion = true;
        }
    }
}
