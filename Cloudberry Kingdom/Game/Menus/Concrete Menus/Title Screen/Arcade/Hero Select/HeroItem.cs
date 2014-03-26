using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class HeroItem : MenuItem
    {
        public BobPhsx Hero;
        public BobPhsx RequiredHero;
        public int RequiredHeroLevel;

        public HeroItem(Tuple<BobPhsx, Tuple<BobPhsx, int>> pair)
            : base(new EzText(pair.Item1.Name, Resources.Font_Grobold42_2))
        {
            this.Hero = pair.Item1;
            this.RequiredHero = pair.Item2.Item1;
            this.RequiredHeroLevel = pair.Item2.Item2;
        }
    }}