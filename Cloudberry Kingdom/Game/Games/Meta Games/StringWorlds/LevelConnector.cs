using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom
{
    /// <summary>
    /// A singleton class to store data about LevelConnectors.
    /// </summary>
    public class LevelConnector
    {
        /// <summary> The object core string code for an end of level object level connector. </summary>
        public static string EndOfLevelCode = "End of Level Connector";

        /// <summary> The object core string code for a start of level object level connector. </summary>
        public static string StartOfLevelCode = "Start of Level Connector";
    }

    /// <summary>
    /// Implemented by IObjects that connect levels together.
    /// </summary>
    public interface ILevelConnector
    {
        LevelSeedData NextLevelSeedData { get; set; }
        DoorAction OnOpen { get; set; }
    }
}