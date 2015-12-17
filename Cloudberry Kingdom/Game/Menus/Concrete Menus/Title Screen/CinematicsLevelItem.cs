namespace CloudberryKingdom
{
    class CinematicsLevelItem : MenuItem
    {
        public string Movie = "";

        public CinematicsLevelItem(EzText Text, string Movie)
            : base(Text)
        {
            this.Movie = Movie;
        }
    }
}