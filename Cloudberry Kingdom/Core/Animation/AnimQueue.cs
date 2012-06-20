using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.IO;

namespace Drawing
{
    public enum AnimQueueEntryType { Play, PlayUntil, Transfer };
    public class AnimQueueEntry
    {
        public AnimQueueEntryType Type;
        public float AnimSpeed, StartT, EndT, DestT;
        public bool Loop;
        public int anim;

        public bool Initialized;

        public AnimQueueEntry() { }
        public AnimQueueEntry(AnimQueueEntry entry)
        {
            Type = entry.Type;
            AnimSpeed = entry.AnimSpeed;
            StartT = entry.StartT;
            EndT = entry.EndT;
            DestT = entry.DestT;
            Loop = entry.Loop;
            anim = entry.anim;
            Initialized = entry.Initialized;
        }
    }
}