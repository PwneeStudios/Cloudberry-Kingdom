using System.Collections.Generic;

namespace CloudberryKingdom
{
    public class ChallengeGroup
    {
        public string Name;

        public List<Challenge> Challenges = new List<Challenge>();

        public ChallengeGroup(string Name)
        {
            this.Name = Name;
        }
    }
}