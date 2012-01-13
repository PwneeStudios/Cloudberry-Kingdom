using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public enum TileSet { None, Random, Terrace, Castle, Dungeon, CastlePiece, OutsideGrass, TileBlock, Cement, Catwalk, DarkTerrace, CastlePiece2, Dark, Rain, Island, _Night, _NightSky };

    public static class TileSetExtension
    {
        public static bool DungeonLike(this TileSet tile)
        {
            return (tile == TileSet.Dungeon || tile == TileSet.Dark);
        }
    }

    /// <summary>
    /// Stores a tile set's information, including what obstacles are allowed.
    /// </summary>
    public class TileSetInfo
    {
        public TileSet StandInType = TileSet.None;

        public TileSet Type;
        public List<Upgrade> ObstacleUpgrades = new List<Upgrade>();
        public List<Upgrade> JumpUpgrades, DodgeUpgrades;

        public bool FlexibleHeight;
        public bool HasCeiling;

        public string Name;

        public Vector4 Tint = new Vector4(1);

        public string ScreenshotString;

        public BackgroundType MyBackgroundType;

        public Color CoinScoreColor = new Color(220, 255, 255);

        /// <summary>
        /// If true the player can not collide with the sides or bottoms of the blocks.
        /// </summary>
        public bool PassableSides;

        public void PostProcess()
        {
            //JumpUpgrades = new List<Upgrade>();
            //DodgeUpgrades = new List<Upgrade>();

            JumpUpgrades = new List<Upgrade>(ObstacleUpgrades.Intersect(RegularLevel.JumpUpgrades));
            DodgeUpgrades = new List<Upgrade>(ObstacleUpgrades.Intersect(RegularLevel.DodgeUpgrades));
        }
    }

    /// <summary>
    /// Static class tracking all tile sets and their information.
    /// </summary>
    public sealed class TileSets
    {
        static Dictionary<TileSet, TileSetInfo> Dict = new Dictionary<TileSet, TileSetInfo>();

        static readonly TileSets instance = new TileSets();
        public static TileSets Instance { get { return instance; } }

        static TileSets() { }
        TileSets()
        {
            TileSetInfo info;
            
            foreach (TileSet tileset in Tools.GetValues<TileSet>())
            {
                info = new TileSetInfo();
                info.Type = tileset;
                Dict.Add(tileset, info);
            }

            // None
            info = Dict[TileSet.None];
            info.Name = "None";
            info.MyBackgroundType = BackgroundType.None;
            info.ScreenshotString = "Screenshot_Random";
            info.HasCeiling = true;
            info.FlexibleHeight = false;            
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike
            });

            // Random
            info = Dict[TileSet.Random];
            info.Name = "Random";
            info.MyBackgroundType = BackgroundType.Random;
            info.ScreenshotString = "Screenshot_Random";
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Fireball, Upgrade.Pinky
            });

            // Outside
            info = Dict[TileSet.Terrace];
            info.Name = "Terrace";
            info.MyBackgroundType = BackgroundType.Outside;
            info.ScreenshotString = "Screenshot_Terrace";
            info.HasCeiling = false;
            info.FlexibleHeight = false;
            //info.CoinScoreColor = new Color(230, 0, 0);
            info.CoinScoreColor = new Color(234, 0, 255);
            //info.PassableSides = true;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Fireball, Upgrade.Pinky
            });

            // Dark
            info = Dict[TileSet.Dark];
            info.Name = "Darkness";
            info.MyBackgroundType = BackgroundType.Dark;
            info.ScreenshotString = "Screenshot_Dark";
            info.HasCeiling = true;
            info.FlexibleHeight = true;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike, Upgrade.Laser, Upgrade.Elevator, Upgrade.FallingBlock, Upgrade.GhostBlock
            });

            // Dark Outside
            info = Dict[TileSet.DarkTerrace];
            info.Name = "Nightmare";
            info.Tint = new Vector4(.8f, .5f, .45f, 1f);
            info.MyBackgroundType = BackgroundType.Gray;
            info.ScreenshotString = "Screenshot_Terrace";
            info.HasCeiling = false;
            info.FlexibleHeight = false;
            //info.CoinScoreColor = new Color(230, 0, 0);
            info.CoinScoreColor = new Color(234, 0, 255);
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Fireball, Upgrade.Pinky
            });

            // Rain
            info = Dict[TileSet.Rain];
            info.Name = "Rain";
            //info.Tint = new Vector4(.8f, .5f, .45f, 1f);
            info.MyBackgroundType = BackgroundType.Rain;
            info.ScreenshotString = "Screenshot_Rain";
            info.HasCeiling = false;
            info.FlexibleHeight = false;
            //info.CoinScoreColor = new Color(230, 0, 0);
            info.CoinScoreColor = new Color(234, 0, 255);
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Fireball, Upgrade.Pinky
            });

            // Sky
            info = Dict[TileSet.Island];
            info.Name = "Sky";
            info.MyBackgroundType = BackgroundType.Sky;
            info.ScreenshotString = "Screenshot_Sky";
            info.HasCeiling = false;
            info.FlexibleHeight = false;
            info.CoinScoreColor = new Color(234, 0, 255);
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Fireball, Upgrade.Pinky
            });

            // Night sky
            info = Dict[TileSet._NightSky];
            info.Name = "Night Sky";
            info.MyBackgroundType = BackgroundType.NightSky;
            info.ScreenshotString = "Screenshot_NightSky";
            info.StandInType = TileSet.Island;

            // Night
            info = Dict[TileSet._Night];
            info.Name = "Night time";
            info.MyBackgroundType = BackgroundType.Night;
            info.ScreenshotString = "Screenshot_Night";
            info.StandInType = TileSet.Terrace;

            // Castle inside
            info = Dict[TileSet.Castle];
            info.Name = "Castle";
            info.MyBackgroundType = BackgroundType.Castle;
            info.ScreenshotString = "Screenshot_Castle";
            info.HasCeiling = true;
            info.FlexibleHeight = true;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike, Upgrade.Laser, Upgrade.Elevator, Upgrade.FallingBlock, Upgrade.GhostBlock
            });

            // Dungeon inside
            info = Dict[TileSet.Dungeon];
            info.Name = "Dungeon";
            info.MyBackgroundType = BackgroundType.Dungeon;
            info.ScreenshotString = "Screenshot_Dungeon";
            info.HasCeiling = true;
            info.FlexibleHeight = false;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike, Upgrade.Fireball, Upgrade.FireSpinner, Upgrade.SpikeyGuy, Upgrade.Pinky, Upgrade.Cloud
            });

            // Grass
            info = Dict[TileSet.OutsideGrass];
            info.Name = "Grass";
            info.HasCeiling = false;
            info.FlexibleHeight = true;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike
            });

            // Cement
            info = Dict[TileSet.Cement];
            info.Name = "Cement";
            info.HasCeiling = true;
            info.FlexibleHeight = false;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike
            });

            // Tileblock
            info = Dict[TileSet.TileBlock];
            info.Name = "Tiles";
            info.HasCeiling = true;
            info.FlexibleHeight = false;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike
            });

            RegularLevel.InitLists();
            foreach (TileSetInfo _info in Dict.Values)
                _info.PostProcess();
        }

        /// <summary>
        /// Returns a tile set's information.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TileSetInfo Get(TileSet type)
        {
            return Dict[type];
        }
    }
}