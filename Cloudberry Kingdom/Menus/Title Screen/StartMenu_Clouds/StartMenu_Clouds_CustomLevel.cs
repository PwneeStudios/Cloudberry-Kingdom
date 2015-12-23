namespace CloudberryKingdom
{
    public class StartMenu_Clouds_CustomLevel : CustomLevel_GUI
    {
        public TitleGameData_Clouds Title;
        public StartMenu_Clouds_CustomLevel(TitleGameData_Clouds Title)
            : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(TitleBackgroundState.Scene_Blur_Dark);
            
            base.SlideIn(Frames);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, Frames);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            StartMenu.SetItemProperties_Red(item);

            if (item.MyText == null) return;
            item.MySelectedText.Shadow = item.MyText.Shadow = false;
        }

        public override void Init()
        {
 	        base.Init();

            CallDelay = 0;
            ReturnToCallerDelay = 0;
            MyMenu.OnB = MenuReturnToCaller;
        }

        public override void ReturnToCaller()
        {
            SlideOutLength = 0;
            base.ReturnToCaller();
        }
    }
}