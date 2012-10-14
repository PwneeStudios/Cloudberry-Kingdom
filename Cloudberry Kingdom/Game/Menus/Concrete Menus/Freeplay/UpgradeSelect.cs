using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class UpgradeSelect : GridSelect, IViewableList
    {
        public override string[] GetViewables() { return new string[] { }; }

        public void GetChildren(List<InstancePlusName> ViewableChildren)
        {
            if (Upgrades != null)
                foreach (UpgradeBar bar in Upgrades)
                {
                    string name = Upgrades.IndexOf(bar).ToString();
                    ViewableChildren.Add(new InstancePlusName(bar, name));
                }
        }

        public static int[] UpgradeLevels = { 0, 2, 4, 6, 9 };
        /// <summary>
        /// Convert the grid values to actual upgrade levels.
        /// </summary>
        public void UpdatePieceSeed(PieceSeedData PieceSeed)
        {
            foreach (UpgradeBar bar in Upgrades)
                PieceSeed.MyUpgrades1[bar.MyUpgradeType] = UpgradeLevels[bar.NumBars];
        }

        /// <summary>
        /// Convert the upgrade levels to values on the grid.
        /// </summary>
        public void UpdateBars(PieceSeedData PieceSeed)
        {
            foreach (UpgradeBar bar in Upgrades)
                bar.NumBars = CalcNumBars(PieceSeed.MyUpgrades1[bar.MyUpgradeType]);
        }
        
        int CalcNumBars(float UpgradeLevel)
        {
            for (int i = 0; i < UpgradeLevels.Length; i++)
            {
                if (UpgradeLevels[i] >= UpgradeLevel)
                    return i;
            }

            return 4;
        }

        public UpgradeBar CurUpgrade
        {
            get { return Upgrades[GetIndex()]; }
        }

        List<UpgradeBar> Upgrades;
        QuadClass Circle;

        public UpgradeSelect() { }
        public UpgradeSelect(Vector2 Size, int Width, int Height)
        {
            MoveSound = Menu.DefaultMenuInfo.Menu_UpDown_Sound;

            //NoMoveDuration = 9;
            LeftRightWrap = false;
            DrawSelectedLast = false;

            Init(Size, Width, Height);
        }

        Vector2 CircleSize;
        public override void Init(Vector2 Size, int Width, int Height)
        {
            base.Init(Size, Width, Height);

            NumLayers = 3; // Bar shadows, bars, icons + icon shadows

            Upgrades = new List<UpgradeBar>();

            Circle = new QuadClass();
            Circle.SetToDefault();
            Circle.TextureName = "Ring2";
            //Circle.TextureName = "Berry";
            Circle.ScaleYToMatchRatio(80);
            //Circle.ScaleYToMatchRatio(260);
            CircleSize = Circle.Size;
        }

        public void Add(UpgradeBar Upgrade)
        {
            Add();
            Upgrades.Add(Upgrade);
        }

        /// <summary>
        /// The number of frames since the last selection
        /// </summary>
        public int SelectCount = 0;

        protected override void OnSelect()
        {
            base.OnSelect();

            SelectCount = 0;
        }

        protected override void AdditionalPhsx()
        {
            base.AdditionalPhsx();

            SelectCount++;

            Circle.Size = CircleSize;
            Circle.PointxAxisTo(Oscillate.GetAngle(SelectCount, 1));
            //Circle.Scale(1 + .2f * UpgradeSelect.GetAngle(2f));
            Circle.Scale(1 + .03f * Oscillate.GetAngle(SelectCount, 2f));

            //Circle.PointxAxisTo(.04f*GetAngle(1.5f));
            //Circle.Scale(.7f + .1f*.1f * UpgradeSelect.GetAngle(2f));

            Upgrades[GetIndex()].PhsxStep(PlayerIndex);
        }

#if PC_VERSION
        public override bool ItemHitTest(int i, Vector2 MousePos)
        {
            UpgradeBar upgrade = Upgrades[i];

            Vector2 padding = new Vector2(250, 260);
            return Phsx.Inside(MousePos, upgrade.Pos - padding + new Vector2(0, 100), upgrade.Pos + padding);
        }

        public override void ItemMouseInteract(int i)
        {
            base.ItemMouseInteract(i);

            UpgradeBar upgrade = Upgrades[i];

            upgrade.MouseInteract();
        }
#endif

        public override void DrawItem(int i, Vector2 pos, bool selected, int Layer)
        {
            UpgradeBar upgrade = Upgrades[i];

            upgrade.Pos = pos;

            if (!selected)
            {
                //upgrade.NumBars = 2;
                upgrade.Draw(false, Circle, Layer);
            }
            else
            {
                //upgrade.NumBars = 3;
                //upgrade.Scale(2.3f);
                upgrade.Draw(true, Circle, Layer);
                //upgrade.Scale(1f / 2.3f);
                //upgrade.Upgrade.Update(ref upgrade.Base);
            }
        }
    }
}