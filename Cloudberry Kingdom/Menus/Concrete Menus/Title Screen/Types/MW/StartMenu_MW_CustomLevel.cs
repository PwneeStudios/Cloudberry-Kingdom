using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_MW_CustomLevel : CustomLevel_GUI
    {
        public TitleGameData_MW Title;
        public StartMenu_MW_CustomLevel(TitleGameData_MW Title)
            : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Blur_Dark);
            
            //base.SlideIn(0);
            base.SlideIn(Frames);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            //base.SlideOut(Preset, 0);
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
            //ReturnToCallerDelay = 10;
            MyMenu.OnB = MenuReturnToCaller;
        }

        public override void ReturnToCaller()
        {
            //Active = false;
            //SlideOutLength = 0;
            //Hide();
            //MyGame.PartialFade_InAndOut(0, .5f, 2, 2, base.ReturnToCaller);

            SlideOutLength = 0;
            base.ReturnToCaller();
        }
    }
}