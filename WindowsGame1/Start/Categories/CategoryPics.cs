using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class CategoryPics : GUI_Panel
    {
        public override void OnAdd()
        {
            base.OnAdd();

            //this.SlideOut(PresetPos.Right, 0);
        }

        QuadClass CategoryPic, Locked;
        public EzText Text;
        public CategoryPics()
        {
            MyPile = new DrawPile();

            // Title
            Text = new EzText("Campaign", Tools.Font_DylanThin42);
            MyPile.Add(Text);

            // Cloud back
            QuadClass Cloudback = new QuadClass();
            Cloudback.SetToDefault();
            Cloudback.TextureName = "menupic_bg_cloud";
            Cloudback.Scale(965 * 1.042f);
            Cloudback.ScaleYToMatchRatio();
            Cloudback.Pos = new Vector2(950, 100) + new Vector2(-5.96875f, -178.5714f);
            MyPile.Add(Cloudback);


            CategoryPic = new QuadClass();
            CategoryPic.SetToDefault();
            CategoryPic.TextureName = "Campaign";
            CategoryPic.Scale(965 * 1.042f);
            CategoryPic.ScaleYToMatchRatio();

            CategoryPic.Pos = new Vector2(950, 100) + new Vector2(-5.96875f, -178.5714f);
            MyPile.Add(CategoryPic);

            // Locked
            Locked = new QuadClass(null, true, true);
            Locked.TextureName = "Locked";
            Locked.ScaleYToMatchRatio(650);

            Locked.Pos = new Vector2(950, 100) + new Vector2(3.96875f, -178.5714f);
            MyPile.Add(Locked);

            // Position the draw pile
            MyPile.Pos = new Vector2(-178, 40f);
        }

        public void RemoveText()
        {
            if (Text == null) return;

            MyPile.Remove(Text);
            Text = null;
        }

        public void Set(string name, string text, Vector2 pos, bool locked)
        {
            Set(name, text, pos, locked, new Vector2(398.4121f, 532.5394f));
        }
        public void Set(string name, string text, Vector2 pos, bool locked, Vector2 lockedpos)
        {
            Set(name, text, pos, locked, lockedpos, new Vector2(443.454f, 166.5054f), 0.349205f);
        }
        public void Set(string name, string text, Vector2 pos, bool locked, Vector2 lockedpos, Vector2 lockedsize, float lockedangle)
        {
            bool Substitute = true;
            if (Text == null)
            {
                if (text.Length == 0) text = "  ";
                Text = new EzText(text, Tools.Font_DylanThin42, 1500, false, false, .66f);
                Text.Scale = .55f;
                MyPile.Add(Text);
                Substitute = false;
            }

            if (name == null)
            {
                CategoryPic.Show = false;
                Text.Show = false;
                Locked.Shadow = false;
                Locked.Show = false;
                return;
            }
            else
                CategoryPic.Show = true;

            CategoryPic.TextureName = name;
            CategoryPic.ScaleYToMatchRatio();

            //locked = true;
            if (locked)
            {
                Text.Show = true;
                //Text.Show = false;
                Text.Pos = pos;

                Locked.Pos = lockedpos;
                Locked.Size = lockedsize;
                Locked.Angle = lockedangle;
                Locked.Show = true;
            }
            else
            {
                Text.Show = false;
                //Text.Show = true;

                if (Substitute)
                    Text.SubstituteText(text);
                Text.Pos = pos;

                Locked.Show = false;
            }
        }
    }
}