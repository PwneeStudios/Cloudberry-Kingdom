namespace CloudberryKingdom
{
    public class LevelItem : MenuItem
    {
        public int StartLevel, MenuIndex;

        public LevelItem(Text Text, int StartLevel, int MenuIndex, bool Locked)
            : base(Text)
        {
            this.StartLevel = StartLevel - 1;
            this.MenuIndex = MenuIndex;
        }
    }
}