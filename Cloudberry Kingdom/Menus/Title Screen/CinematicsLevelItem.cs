namespace CloudberryKingdom
{
    class CinematicsLevelItem : MenuItem
    {
        public string Movie = "";

        public CinematicsLevelItem(Text Text, string Movie)
            : base(Text)
        {
            this.Movie = Movie;
        }
    }
}