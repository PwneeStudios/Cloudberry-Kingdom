using System.Collections.Generic;

namespace CloudberryKingdom
{
    /*
    public class ChallengeList
    {
        public static List<ChallengeGroup> Groups;
        public static List<Challenge> Challenges;

        public static void Init()
        {
            if (Groups != null) return;

            Groups = new List<ChallengeGroup>();
            Challenges = new List<Challenge>();

            ChallengeGroup group;

            group = new ChallengeGroup("Challenges");
            group.Challenges.Add(Challenge_Fireballs.Instance);
            group.Challenges.Add(Challenge_Survival.Instance);
            group.Challenges.Add(Challenge_Place.Instance);
            group.Challenges.Add(new Challenge("Indigo Glow"));
            group.Challenges.Add(new Challenge("Double Trouble"));
            Groups.Add(group);

            group = new ChallengeGroup("Round Two");
            group.Challenges.Add(new Challenge("BFF"));
            group.Challenges.Add(new Challenge("Stairway to Heaven"));
            group.Challenges.Add(new Challenge("Lost Cause"));
            group.Challenges.Add(new Challenge("You in a Box"));
            group.Challenges.Add(new Challenge("Tough Luck"));
            Groups.Add(group);

            group = new ChallengeGroup("Bigger, Better");
            group.Challenges.Add(new Challenge("1 Second to Live"));
            group.Challenges.Add(new Challenge("Minute Man"));
            group.Challenges.Add(new Challenge("Permadeath"));
            group.Challenges.Add(new Challenge("777"));
            Groups.Add(group);

            group = new ChallengeGroup("Trois");
            group.Challenges.Add(new Challenge("Hot Pursuit"));
            group.Challenges.Add(new Challenge("Inervate"));
            group.Challenges.Add(new Challenge("Jack of All Heros"));
            group.Challenges.Add(new Challenge("To the Metal"));
            Groups.Add(group);

            // Add all challenges from each group to the challenge list
            foreach (ChallengeGroup cgroup in Groups)
                Challenges.AddRange(cgroup.Challenges);
        }
    }*/
}