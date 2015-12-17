using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class Globals
    {
        public static string ContentDirectory;

        public static Color[] OnOffBlockColors = { Color.LightSeaGreen, Color.BlueViolet, Color.Fuchsia, Color.Gainsboro };
        public static bool[] ColorSwitch = { false, false, false, false };

        public enum Upgrade                    { Fireball, Spike, FallingBlock, FlyBlob, FireSpinner, MovingBlock, EmitBlock, Floater, Floater_Spin, Laser, GhostBlock, BouncyBlock, Cloud, General, Speed, Jump, Ceiling };
        public static string[] UpgradeString = { "Fireball", "Spike", "Falling Block", "Flying Blob", "Fire Spinner", "Moving Block", "Elevator", "Spikey Guy", "Spinner", "Laser", "Ghost Block", "Bouncy Block", "Cloud", "General", "Speed", "Jump", "Ceiling" };
    }
}