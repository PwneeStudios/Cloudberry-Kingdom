namespace CoreEngine
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