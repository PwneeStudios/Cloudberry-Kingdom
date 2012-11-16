using System.Collections.Generic;
using Microsoft.Xna.Framework;



namespace CloudberryKingdom
{
    public class SwarmBundle
    {
        List<SwarmRecord> Swarms;
        public SwarmRecord CurrentSwarm;

        public bool Initialized;
        SpriteAnimGroup[] AnimGroup;

        List<BobLink> BobLinks;

        public void Release()
        {
            BobLinks = null;

            foreach (SwarmRecord swarm in Swarms)
                swarm.Release();
            Swarms = null;

            if (AnimGroup != null)
                for (int i = 0; i < AnimGroup.Length; i++)
                    if (AnimGroup[i] != null)
                        AnimGroup[i].Release();
            AnimGroup = null;
        }

        public SwarmBundle()
        {
            Initialized = false;

            Swarms = new List<SwarmRecord>();
        }

        public void Init(Level level)
        {
            if (Initialized) return;
            
            BobLinks = new List<BobLink>();
            if (level.MyGame.MyGameFlags.IsTethered)
            {
                foreach (Bob bob in level.Bobs)
                    if (bob.MyBobLinks != null)
                        BobLinks.AddRange(bob.MyBobLinks);

                foreach (BobLink link in BobLinks)
                {
                    link._j = level.Bobs.IndexOf(link.j);
                    link._k = level.Bobs.IndexOf(link.k);
                }
            }

            AnimGroup = new SpriteAnimGroup[4];

            int count = 0;
            foreach (Bob bob in level.Bobs)
            {
                AnimGroup[count] = new SpriteAnimGroup();
                AnimGroup[count].Init(bob.PlayerObject, bob.MyPhsx.SpritePadding, new BobToSpritesLambda(bob));
                count++;
            }
            Initialized = true;
        }

        class BobToSpritesLambda : Lambda_2<Dictionary<int, SpriteAnim>, Vector2>
        {
            Bob bob;
            public BobToSpritesLambda(Bob bob)
            {
                this.bob = bob;
            }

            public void Apply(Dictionary<int, SpriteAnim> dict, Vector2 pos)
            {
                bob.MyPhsx.ToSprites(dict, pos);
            }
        }

        public void SetSwarm(Level level, int i)
        {
            i = CoreMath.Restrict(0, Swarms.Count - 1, i);

            CurrentSwarm = Swarms[i];
            level.CurPiece = CurrentSwarm.MyLevelPiece;

            for (int j = 0; j < CurrentSwarm.MainRecord.NumBobs; j++)
            {
                if (j >= level.Bobs.Count) continue;
                level.Bobs[j].MyRecord = CurrentSwarm.MainRecord.Recordings[j];
            }
        }

        public int SwarmIndex
        {
            get { return Swarms.IndexOf(CurrentSwarm); }
        }

        public int NumSwarms
        {
            get { return Swarms.Count; }
        }

        public bool GetNextSwarm(Level level)
        {
            int i = SwarmIndex + 1;
            if (i < Swarms.Count)
            {
                SetSwarm(level, i);

                return true;
            }
            else
                return false;
        }

        public bool EndCheck(Level level)
        {
            return level.CurPhsxStep >= CurrentSwarm.MainRecord.Length;
        }

        public void StartNewSwarm()
        {
            CurrentSwarm = new SwarmRecord();
            Swarms.Add(CurrentSwarm);
        }

        public void Draw(int Step, Level level)
        {
            CurrentSwarm.Draw(Step, level, AnimGroup, BobLinks);
        }
    }
}