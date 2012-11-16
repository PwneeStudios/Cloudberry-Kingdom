using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;




namespace CloudberryKingdom
{
    public class Challenge_HeroFactoryEscalation : Challenge_Escalation
    {
        public static BobPhsx FactoryHero = BobPhsxNormal.Instance;

        static readonly Challenge_HeroFactoryEscalation instance = new Challenge_HeroFactoryEscalation();
        public static Challenge_HeroFactoryEscalation Instance { get { return instance; } }

        protected Challenge_HeroFactoryEscalation()
        {
            GameTypeId = 10;
            Name = Localization.Words.Custom;
        }

        protected override BobPhsx GetHero(int i)
        {
            return FactoryHero;
        }
    }
}