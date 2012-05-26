using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    /// <summary>
    /// Stores a tile set's information, including what obstacles are allowed.
    /// </summary>
    public class TileSetInfo
    {
        // CRAP
        public bool DungeonLike = false;



        // New tile set stuff
        Dictionary<int, PieceQuad>
            Pillars = new Dictionary<int, PieceQuad>(),
            Platforms = new Dictionary<int, PieceQuad>();

        /// <summary>
        /// Read tileset info from a file.
        /// </summary>
        public void Read(String file)
        {
            FixedWidths = true;
            ProvidesTemplates = true;

            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader reader = new StreamReader(stream);

            String line;

            line = reader.ReadLine();
            while (line != null)
            {
                line = Tools.RemoveComment(line);

                var bits = line.Split(' ').ToList();
                bits.RemoveAll(bit => string.Compare(bit, " ") == 0 || string.Compare(bit, "\t") == 0);

                if (bits.Count > 0)
                {
                    var first = bits[0];

                    // Is it a pillar?
                    if (first.Contains("Pillar_"))
                    {
                        // Get the pillar width
                        var num_str = first.Substring(first.IndexOf("_") + 1);
                        int width = int.Parse(num_str);

                        // Get the rest of the information
                        var piecequad = ParsePillarLine(width, bits);
                        Pillars.Add(width, piecequad);
                    }
                    // Is it a platform?
                    else if (first.Contains("Platform_"))
                    {
                        // Get the platform width
                        var num_str = first.Substring(first.IndexOf("_") + 1);
                        int width = int.Parse(num_str);

                        // Get the rest of the information
                        var piecequad = ParsePlatformLine(width, bits);
                        Pillars.Add(width, piecequad);
                    }
                    else switch (first)
                    {
                        case "Name": Name = bits[1]; break;
                        default: break;
                    }
                }


                line = reader.ReadLine();
            }

            reader.Close();
            stream.Close();

            // Sort widths
            var list = Pillars.Keys.ToList(); list.Sort(); PillarWidths = list.ToArray();
                list = Pillars.Keys.ToList(); list.Sort(); PillarWidths = list.ToArray();
        }

        PieceQuad ParsePillarLine(int width, List<string> bits)
        {
            var c = new PieceQuad();
            c.Init(null, Tools.BasicEffect);
            c.Data.RepeatWidth = 2000;
            c.Data.RepeatHeight = 2000;
            c.Data.MiddleOnly = false;
            c.Data.CenterOnly = true;
            c.Data.UV_Multiples = new Vector2(1, 0);
            
            c.Center.TextureName = bits[1];
            int tex_width = c.Center.MyTexture.Tex.Width;

            float diff = tex_width - width;
            diff /= 2;

            c.Data.Left_TR_Shift.X = diff;
            c.Data.Left_BL_Shift.X = -diff;

            return c;
        }

        PieceQuad ParsePlatformLine(int width, List<string> bits)
        {
            var c = new PieceQuad();
            c.Init(null, Tools.BasicEffect);
            c.Data.RepeatWidth = 2000;
            c.Data.RepeatHeight = 2000;
            c.Data.MiddleOnly = false;
            c.Data.CenterOnly = true;
            c.Data.UV_Multiples = new Vector2(1, 1);

            c.Center.TextureName = bits[1];
            int tex_width = c.Center.MyTexture.Tex.Width;
            int tex_height = c.Center.MyTexture.Tex.Height;

            // Center the quad
            float diff = tex_width - width;
            diff /= 2;

            c.Data.Left_TR_Shift.X = diff;
            c.Data.Left_BL_Shift.X = -diff;

            // Extend the quad down to properly scale quad
            c.Data.Bottom_BL_Shift.Y = -tex_height;

            return c;
        }

        public bool FixedWidths = false;
        public bool ProvidesTemplates = false;
        public int[] PillarWidths, PlatformWidths;

        public void SnapWidthUp_Pillar(ref Vector2 size)
        {
            SnapWidthUp(ref size, PillarWidths);
        }
        public void SnapWidthUp_Platform(ref Vector2 size)
        {
            SnapWidthUp(ref size, PlatformWidths);
        }

        public static void SnapWidthUp(ref Vector2 size, int[] Widths)
        {
            for (int i = 0; i < Widths.Length; i++)
            {
                if (size.X < Widths[i])
                {
                    size.X = Widths[i];
                    return;
                }
            }

            size.X = Widths[Widths.Length - 1];
        }

        public PieceQuad GetPieceTemplate(BlockBase block)
        {
            // Get the block's info
            var box = block.Box;
            var core = block.BlockCore;

            // Get the width of the block (accounting for possible rotations for Meatboy levels)
            float width = 0;
            if (core.MyOrientation == PieceQuad.Orientation.RotateRight ||
                core.MyOrientation == PieceQuad.Orientation.RotateLeft)
                width = box.Current.Size.Y;
            else
                width = box.Current.Size.X;

            // Get the piecequad template


            return null;
        }















        public TileSetInfo StandInType = TileSets.None;

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
            JumpUpgrades = new List<Upgrade>(ObstacleUpgrades.Intersect(RegularLevel.JumpUpgrades));
            DodgeUpgrades = new List<Upgrade>(ObstacleUpgrades.Intersect(RegularLevel.DodgeUpgrades));
        }
    }

    /// <summary>
    /// Static class tracking all tile sets and their information.
    /// </summary>
    public sealed class TileSets
    {
        static readonly TileSets instance = new TileSets();
        public static TileSets Instance { get { return instance; } }

        public static TileSetInfo None, Random, Terrace, Castle, Dungeon, CastlePiece, OutsideGrass, TileBlock, Cement, Catwalk, DarkTerrace, CastlePiece2, Dark, Rain, Island, _Night, _NightSky;
        public static TileSetInfo DefaultTileSet;

        public static List<TileSetInfo> TileSets;

        static TileSets() { }
        TileSets()
        {
            TileSetInfo info;
 
            //public enum TileSet { None, Random, Terrace, Castle, Dungeon, CastlePiece, OutsideGrass, TileBlock, Cement, Catwalk, DarkTerrace, CastlePiece2, Dark, Rain, Island, _Night, _NightSky };

            // None
            DefaultTileSet = None = info = new TileSetInfo(); TileSets.Add(info);
            info.Name = "None";
            info.MyBackgroundType = BackgroundType.None;
            info.ScreenshotString = "Screenshot_Random";
            info.HasCeiling = true;
            info.FlexibleHeight = false;            
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike
            });

            // Random
            Random = info = new TileSetInfo(); TileSets.Add(info);
            info.Name = "Random";
            info.MyBackgroundType = BackgroundType.Random;
            info.ScreenshotString = "Screenshot_Random";
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Fireball, Upgrade.Pinky
            });

            // Outside
            Terrace = info = new TileSetInfo(); TileSets.Add(info);
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
            Dark = info = new TileSetInfo(); TileSets.Add(info);
            info.DungeonLike = true;
            info.Name = "Darkness";
            info.MyBackgroundType = BackgroundType.Dark;
            info.ScreenshotString = "Screenshot_Dark";
            info.HasCeiling = true;
            info.FlexibleHeight = true;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike, Upgrade.Laser, Upgrade.Elevator, Upgrade.FallingBlock, Upgrade.GhostBlock
            });

            // Dark Outside
            DarkTerrace = info = new TileSetInfo(); TileSets.Add(info);
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
            Rain = info = new TileSetInfo(); TileSets.Add(info);
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
            Island = info = new TileSetInfo(); TileSets.Add(info);
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
            _NightSky = info = new TileSetInfo(); TileSets.Add(info);
            info.Name = "Night Sky";
            info.MyBackgroundType = BackgroundType.NightSky;
            info.ScreenshotString = "Screenshot_NightSky";
            info.StandInType = Island;

            // Night
            _Night = info = new TileSetInfo(); TileSets.Add(info);
            info.Name = "Night time";
            info.MyBackgroundType = BackgroundType.Night;
            info.ScreenshotString = "Screenshot_Night";
            info.StandInType = Terrace;

            // Castle inside
            Castle = info = new TileSetInfo(); TileSets.Add(info);
            info.Name = "Castle";
            info.MyBackgroundType = BackgroundType.Castle;
            info.ScreenshotString = "Screenshot_Castle";
            info.HasCeiling = true;
            info.FlexibleHeight = true;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike, Upgrade.Laser, Upgrade.Elevator, Upgrade.FallingBlock, Upgrade.GhostBlock
            });

            // Dungeon inside
            Dungeon = info = new TileSetInfo(); TileSets.Add(info);
            info.DungeonLike = true;
            info.Name = "Dungeon";
            info.MyBackgroundType = BackgroundType.Dungeon;
            info.ScreenshotString = "Screenshot_Dungeon";
            info.HasCeiling = true;
            info.FlexibleHeight = false;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike, Upgrade.Fireball, Upgrade.FireSpinner, Upgrade.SpikeyGuy, Upgrade.Pinky, Upgrade.Cloud
            });

            // Grass
            OutsideGrass = info = new TileSetInfo(); TileSets.Add(info);
            info.Name = "Grass";
            info.HasCeiling = false;
            info.FlexibleHeight = true;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike
            });

            // Cement
            Cement = info = new TileSetInfo(); TileSets.Add(info);
            info.Name = "Cement";
            info.HasCeiling = true;
            info.FlexibleHeight = false;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike
            });

            // Tileblock
            TileBlock = info = new TileSetInfo(); TileSets.Add(info);
            info.Name = "Tiles";
            info.HasCeiling = true;
            info.FlexibleHeight = false;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike
            });

            RegularLevel.InitLists();
            foreach (var _info in TileSets)
                _info.PostProcess();
        }
    }
}