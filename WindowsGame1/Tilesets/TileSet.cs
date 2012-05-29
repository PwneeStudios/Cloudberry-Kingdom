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
    public class TileSet
    {
        public TileSet()
        {
            MakeNew();
        }

        public void MakeNew()
        {
            IsLoaded = false;

            CustomStartEnd = false;
            DungeonLike = false;
            DoorType = Door.Types.Brick;

            Pillars = new Dictionary<int, PieceQuad>();
            Platforms = new Dictionary<int, PieceQuad>();

            FixedWidths = false;
            ProvidesTemplates = false;

            StandInType = TileSets.None;

            ObstacleUpgrades = new List<Upgrade>();

            FlexibleHeight = false;
            HasCeiling = false;

            Name = "";
            MyPath = "";
            Guid = 0;

            Tint = new Vector4(1);

            ScreenshotString = "";

            MyBackgroundType = BackgroundType.Dungeon;

            CoinScoreColor = new Color(220, 255, 255);
        }

        // CRAP
        public bool DungeonLike;
        public Door.Types DoorType;

        // New tile set stuff
        public bool IsLoaded;
        public bool CustomStartEnd;
        Dictionary<int, PieceQuad> Pillars, Platforms;

        public bool FixedWidths;
        public bool ProvidesTemplates;
        public int[] PillarWidths, PlatformWidths;


        /// <summary>
        /// Read tileset info from a file.
        /// </summary>
        public void Read(String path)
        {
            MyPath = path;
            IsLoaded = true;
            CustomStartEnd = true;

            // Find path
            FileStream stream = null;
            string original_path = path;
            try
            {
                stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch
            {
                try
                {
                    path = Path.Combine(Globals.ContentDirectory, original_path);
                    stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch
                {
                    try
                    {
                        path = Path.Combine(Globals.ContentDirectory, Path.Combine("DynamicLoad", original_path));
                        stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                    }
                    catch
                    {
                        Tools.Log(string.Format("Attempting to load a .tileset file. Path <{0}> not found."));
                    }
                }
            }




            FixedWidths = true;
            ProvidesTemplates = true;
            MyBackgroundType = BackgroundType.Dungeon;

            StreamReader reader = new StreamReader(stream);

            String line;

            line = reader.ReadLine();
            while (line != null)
            {
                var bits = Tools.GetBitsFromLine(line);

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
                        Platforms.Add(width, piecequad);
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
                list = Platforms.Keys.ToList(); list.Sort(); PlatformWidths = list.ToArray();
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

            ParseExtraBlockInfo(c, width, bits);

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

            ParseExtraBlockInfo(c, width, bits);

            return c;
        }

        void ParseExtraBlockInfo(PieceQuad c, int width, List<string> bits)
        {
            c.Center.U_Wrap = c.Center.V_Wrap = false;

            c.Center.TextureName = bits[1];
            int tex_width = c.Center.MyTexture.Tex.Width;
            int tex_height = c.Center.MyTexture.Tex.Height;

            for (int i = 2; i < bits.Count; i++)
            {
                switch (bits[i])
                {
                    case "left": c.Data.Center_BL_Shift.X = float.Parse(bits[i + 1]); break;
                    case "right": c.Data.Center_TR_Shift.X = float.Parse(bits[i + 1]); break;
                    case "top": c.Data.Center_TR_Shift.Y = float.Parse(bits[i + 1]); break;
                }
            }

            // Extend the quad down to properly scale quad
            float sprite_width = 2 * width + c.Data.Center_TR_Shift.X - c.Data.Center_BL_Shift.X;
            c.FixedHeight = sprite_width * (float)tex_height / (float)tex_width;
            //c.Data.Bottom_BL_Shift.Y = -tex_height;
        }

        public void SnapWidthUp_Pillar(ref Vector2 size)
        {
            size.X = SnapWidthUp(size.X, PillarWidths);
        }
        public void SnapWidthUp_Platform(ref Vector2 size)
        {
            size.X = SnapWidthUp(size.X, PlatformWidths);
        }

        public static int SnapWidthUp(float width, int[] Widths)
        {
            int int_width = 0;

            int_width = Widths[Widths.Length - 1];
            for (int i = 0; i < Widths.Length; i++)
            {
                if (width < Widths[i])
                {
                    int_width = Widths[i];
                    break;
                }
            }
            
            width = int_width;

            return int_width;
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

            int int_width = SnapWidthUp(width - .1f, PillarWidths);

            // Get the piecequad template
            try
            {
                if (block.Box.TopOnly && block.BlockCore.UseTopOnlyTexture)
                    return Platforms[int_width];
                else
                    return Pillars[int_width];
            }
            catch
            {
                Tools.Log(string.Format("Could not find {0} of width {1} for tileset {2}",
                    block.Box.TopOnly ? "Platform" : "Pillar", width, Name));
                return null;
            }
        }















        public TileSet StandInType = TileSets.None;

        public List<Upgrade> ObstacleUpgrades = new List<Upgrade>();
        public List<Upgrade> JumpUpgrades, DodgeUpgrades;

        public bool FlexibleHeight;
        public bool HasCeiling;

        public string Name, MyPath;
        public int Guid;

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
        public static TileSet None, Random, Terrace, Castle, Dungeon, CastlePiece, OutsideGrass, TileBlock, Cement, Catwalk, DarkTerrace, CastlePiece2, Dark, Rain, Island, _Night, _NightSky;
        public static TileSet DefaultTileSet;

        public static List<TileSet> TileList = new List<TileSet>();
        public static Dictionary<int, TileSet> GuidLookup = new Dictionary<int, TileSet>();
        public static Dictionary<string, TileSet> NameLookup = new Dictionary<string, TileSet>(), PathLookup = new Dictionary<string,TileSet>();

        public static void KillDynamic()
        {
            TileList.RemoveAll(tile => tile.IsLoaded);
            GuidLookup.RemoveAll(pair => pair.Value.IsLoaded);
            NameLookup.RemoveAll(pair => pair.Value.IsLoaded);
            PathLookup.RemoveAll(pair => pair.Value.IsLoaded);
        }

        public static void AddTileSet(TileSet tileset)
        {
            TileList.Add(tileset);

            // Add the tileset to the Guid lookup
            try
            {
                GuidLookup.Add(tileset.Guid, tileset);
            }
            catch
            {
                Tools.Log(string.Format("TileSet Guid {0} already exists (Name = {1})", tileset.Guid, tileset.Name));
                GuidLookup[tileset.Guid] = tileset;
            }

            // Add the tileset to the Name lookup
            try
            {
                NameLookup.Add(tileset.Name, tileset);
            }
            catch
            {
                Tools.Log(string.Format("TileSet name {0} already exists (Guid = {1})", tileset.Name, tileset.Guid));
                NameLookup[tileset.Name] = tileset;
            }

            // Add the tileset to the Path lookup
            if (tileset.MyPath.Length > 0)
            {
                try
                {
                    PathLookup.Add(tileset.MyPath, tileset);
                }
                catch
                {
                    Tools.Log(string.Format("TileSet path {0} already exists (Name = {1})", tileset.MyPath, tileset.Name));
                    PathLookup[tileset.Name] = tileset;
                }
            }
        }

        public static void LoadTileSet(string path)
        {
            TileSet tileset;
            if (PathLookup.ContainsKey(path))
            {
                tileset = PathLookup[path];
                tileset.MakeNew();
            }
            else
                tileset = new TileSet();

            tileset.Read(path);
            AddTileSet(tileset);
        }

        public static void Init()
        {
            TileSet info;

            //public enum TileSet { None, Random, Terrace, Castle, Dungeon, CastlePiece, OutsideGrass, TileBlock, Cement, Catwalk, DarkTerrace, CastlePiece2, Dark, Rain, Island, _Night, _NightSky };


            // None
            DefaultTileSet = None = info = new TileSet();
            info.Name = "None";
            info.Guid = 5551;
            AddTileSet(info);
            info.MyBackgroundType = BackgroundType.None;
            info.ScreenshotString = "Screenshot_Random";
            info.HasCeiling = true;
            info.FlexibleHeight = false;            
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike
            });

            // Random
            Random = info = new TileSet();
            info.Name = "Random";
            info.Guid = 5552;
            AddTileSet(info);
            info.MyBackgroundType = BackgroundType.Random;
            info.ScreenshotString = "Screenshot_Random";
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Fireball, Upgrade.Pinky
            });

            // Outside
            Terrace = info = new TileSet();
            info.Name = "Terrace";
            info.Guid = 5553;
            AddTileSet(info);
            info.DoorType = Door.Types.Grass;
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
            Dark = info = new TileSet();
            info.DungeonLike = true;
            info.Name = "Darkness";
            info.Guid = 5554;
            AddTileSet(info);
            info.DoorType = Door.Types.Dark;
            info.MyBackgroundType = BackgroundType.Dark;
            info.ScreenshotString = "Screenshot_Dark";
            info.HasCeiling = true;
            info.FlexibleHeight = true;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike, Upgrade.Laser, Upgrade.Elevator, Upgrade.FallingBlock, Upgrade.GhostBlock
            });

            // Dark Outside
            DarkTerrace = info = new TileSet();
            info.Name = "Nightmare";
            info.Guid = 5555;
            AddTileSet(info);
            info.DoorType = Door.Types.Grass;
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
            Rain = info = new TileSet();
            info.Name = "Rain";
            info.Guid = 5556;
            AddTileSet(info);
            info.DoorType = Door.Types.Grass;
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
            Island = info = new TileSet();
            info.Name = "Sky";
            info.Guid = 5557;
            AddTileSet(info);
            info.MyBackgroundType = BackgroundType.Sky;
            info.ScreenshotString = "Screenshot_Sky";
            info.HasCeiling = false;
            info.FlexibleHeight = false;
            info.CoinScoreColor = new Color(234, 0, 255);
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Fireball, Upgrade.Pinky
            });

            // Night sky
            _NightSky = info = new TileSet();
            info.Name = "Night Sky";
            info.Guid = 5558;
            AddTileSet(info);
            info.MyBackgroundType = BackgroundType.NightSky;
            info.ScreenshotString = "Screenshot_NightSky";
            info.StandInType = Island;

            // Night
            _Night = info = new TileSet();
            info.Name = "Night time";
            info.Guid = 5559;
            AddTileSet(info);
            info.MyBackgroundType = BackgroundType.Night;
            info.ScreenshotString = "Screenshot_Night";
            info.StandInType = Terrace;

            // Castle inside
            Castle = info = new TileSet();
            info.Name = "Castle";
            info.Guid = 5560;
            AddTileSet(info);
            info.MyBackgroundType = BackgroundType.Castle;
            info.ScreenshotString = "Screenshot_Castle";
            info.HasCeiling = true;
            info.FlexibleHeight = true;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike, Upgrade.Laser, Upgrade.Elevator, Upgrade.FallingBlock, Upgrade.GhostBlock
            });

            // Dungeon inside
            Dungeon = info = new TileSet();
            info.DungeonLike = true;
            info.Name = "Dungeon";
            info.Guid = 5561;
            AddTileSet(info);
            info.DoorType = Door.Types.Rock;
            info.MyBackgroundType = BackgroundType.Dungeon;
            info.ScreenshotString = "Screenshot_Dungeon";
            info.HasCeiling = true;
            info.FlexibleHeight = false;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike, Upgrade.Fireball, Upgrade.FireSpinner, Upgrade.SpikeyGuy, Upgrade.Pinky, Upgrade.Cloud
            });

            // Grass
            OutsideGrass = info = new TileSet();
            info.Name = "Grass";
            info.Guid = 5562;
            AddTileSet(info);
            info.HasCeiling = false;
            info.FlexibleHeight = true;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike
            });

            // Cement
            Cement = info = new TileSet();
            info.Name = "Cement";
            info.Guid = 5563;
            AddTileSet(info);
            info.HasCeiling = true;
            info.FlexibleHeight = false;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike
            });

            // Catwalk
            Catwalk = info = new TileSet();
            info.Name = "Catwalk";
            info.Guid = 5564;
            AddTileSet(info);
            info.HasCeiling = false;
            info.FlexibleHeight = false;

            // Tileblock
            TileBlock = info = new TileSet();
            info.Name = "Tiles";
            info.Guid = 5565;
            AddTileSet(info);
            info.HasCeiling = true;
            info.FlexibleHeight = false;
            info.ObstacleUpgrades.AddRange(new Upgrade[] {
                Upgrade.BouncyBlock, Upgrade.FlyBlob, Upgrade.MovingBlock, Upgrade.Spike
            });

            // CastlePiece
            CastlePiece = info = new TileSet();
            info.Name = "CastlePiece";
            info.Guid = 5566;
            AddTileSet(info);
            info.HasCeiling = false;
            info.FlexibleHeight = false;

            // CastlePiece2
            CastlePiece2 = info = new TileSet();
            info.Name = "CastlePiece2";
            info.Guid = 5567;
            AddTileSet(info);
            info.HasCeiling = false;
            info.FlexibleHeight = false;


            RegularLevel.InitLists();
            foreach (var _info in TileList)
                _info.PostProcess();
        }
    }
}