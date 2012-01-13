namespace CloudberryKingdom
{
    public class VerifyQuitCampaignMenu : VerifyQuitLevelMenu
    {
        public VerifyQuitCampaignMenu(int Control)
            : base(false)
        {
            this.VerifyString = "Exit campaign?";
            this.Control = Control;

            Constructor();
        }
    }
}