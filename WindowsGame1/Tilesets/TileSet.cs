using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class BlockGroup
    {
        public Dictionary<int, List<PieceQuad>> Dict;
        public int[] Widths;

        public BlockGroup()
        {
            Dict = new Dictionary<int, List<PieceQuad>>();
        }

        public void Add(int width, PieceQuad piece)
        {
            if (!Dict.ContainsKey(width))
                Dict.Add(width, new List<PieceQuad>());

            Dict[width].Add(piece);
        }

        public PieceQuad Choose(int width, Rand rnd)
        {
            return Dict[width].Choose(rnd);
        }

        public void SortWidths()
        {
            var list = Dict.Keys.ToList();
            list.Sort();
            Widths = list.ToArray();
        }

        public int SnapWidthUp(float width)
        {
            return SnapWidthUp(width, Widths);
        }
        public void SnapWidthUp(ref Vector2 size)
        {
            size.X = SnapWidthUp(size.X, Widths);
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
    }

    /// <summary>
    /// Stores a tile set's information, including what obstacles are allowed.
    /// </summary>
    public class TileSet
    {
        public class TileSetInfo
        {
            public Coins.Coin.CoinTileInfo Coins = new Coins.Coin.CoinTileInfo();
            public FireSpinners.FireSpinner.FireSpinnerTileInfo Spinners = new FireSpinners.FireSpinner.FireSpinnerTileInfo();
            public Goombas.Goomba.GoombaTileInfo Blobs = new Goombas.Goomba.GoombaTileInfo();
            public Door.DoorTileInfo Doors = new Door.DoorTileInfo();
            public Fireball.FireballTileInfo Fireballs = new Fireball.FireballTileInfo();
        }
        public TileSetInfo MyTileSetInfo;

        public TileSet()
        {
            MakeNew();
        }

        public void MakeNew()
        {
            IsLoaded = false;

            MyTileSetInfo = new TileSetInfo();

            CustomStartEnd = false;
            DungeonLike = false;
            DoorType = Door.Types.Brick;

            Pillars = new BlockGroup();
            Platforms = new BlockGroup();
            MovingBlocks = new BlockGroup();
            FallingBlocks = new BlockGroup();

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
        public BlockGroup Pillars, Platforms;
        public BlockGroup MovingBlocks, FallingBlocks, BouncyBlocks;

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
            //MyBackgroundType = BackgroundType.Dungeon;
            MyBackgroundType = BackgroundType.MarioSky;

            StreamReader reader = new StreamReader(stream);

            String line;

            line = reader.ReadLine();
            while (line != null)
            {
                var bits = Tools.GetBitsFromLine(line);

                if (bits.Count > 1)
                {
                    var first = bits[0];

                    // Is it a pillar?
                    if (first.Contains("Pillar_"))
                    {
                        ParseBlock(bits, first, Pillars);
                    }
                    // Is it a platform?
                    else if (first.Contains("Platform_"))
                    {
                        ParseBlock(bits, first, Platforms);
                    }
                    // Is it a moving block?
                    else if (first.Contains("MovingBlock_"))
                    {
                        ParseBlock(bits, first, MovingBlocks);
                    }
                    // Is it a falling block?
                    else if (first.Contains("FallingBlock_"))
                    {
                        ParseBlock(bits, first, FallingBlocks);
                    }
                    else switch (first)
                    {
                        case "sprite_anim":
                            var dict = Tools.GetLocations(bits, "name", "file", "size", "frame_length", "reverse_at_end");

                            var name = bits[dict["name"] + 1];
                            var file = bits[dict["file"] + 1];
                            var size = int.Parse(bits[dict["size"] + 1]);

                            var sprite_anim = new AnimationData_Texture(Tools.Texture(file), size);

                            if (dict.ContainsKey("reverse_at_end"))
                                sprite_anim.Reverse = true;
                            if (dict.ContainsKey("frame_length"))
                            {
                                var frame_length = int.Parse(bits[dict["frame_length"] + 1]);
                                sprite_anim.Speed = 1f / frame_length;
                            }
                            
                            Tools.TextureWad.Add(sprite_anim, name);

                            break;

                        case "BackgroundImage":
                            BackgroundTemplate b;
                            try
                            {
                                b = BackgroundType.NameLookup[bits[1]];
                            }
                            catch
                            {
                                b = new BackgroundTemplate();
                                b.Name = bits[1];
                            }


                            break;

                        case "Name": Name = bits[1]; break;
                        default:
                            Tools.ReadLineToObj(MyTileSetInfo, bits);
                            break;
                    }
                }


                line = reader.ReadLine();
            }

            reader.Close();
            stream.Close();

            // Sort widths
            Pillars.SortWidths();
            Platforms.SortWidths();
            MovingBlocks.SortWidths();
            FallingBlocks.SortWidths();
        }

        private void ParseBlock(List<string> bits, string first, BlockGroup group)
        {
            // Get the block width
            var num_str = first.Substring(first.IndexOf("_") + 1);
            int width = int.Parse(num_str);

            // Get the rest of the information
            var piecequad = ParseBlockLine(width, bits);
            group.Add(width, piecequad);
        }

        PieceQuad ParseBlockLine(int width, List<string> bits)
        {
            var c = new PieceQuad();
            c.Init(null, Tools.BasicEffect);

            bool IsTile = Tools.BitsHasBit(bits, "tile");

            if (IsTile)
            {
                c.Data.RepeatWidth = 2000;
                c.Data.RepeatHeight = 2000;
                c.Data.MiddleOnly = false;
                c.Data.CenterOnly = true;
                c.Data.UV_Multiples = new Vector2(0, 0);
                c.Center.U_Wrap = c.Center.V_Wrap = true;
            }
            else
            {
                c.Data.RepeatWidth = 2000;
                c.Data.RepeatHeight = 2000;
                c.Data.MiddleOnly = false;
                c.Data.CenterOnly = true;
                c.Data.UV_Multiples = new Vector2(1, 0);
                c.Center.U_Wrap = c.Center.V_Wrap = false;

                c.FixedHeight = 0; // Flag to tell ParseExtra to set the height properly
            }

            ParseExtraBlockInfo(c, width, bits);

            return c;
        }

        //PieceQuad ParsePlatformLine(int width, List<string> bits)
        //{
        //    var c = new PieceQuad();
        //    c.Init(null, Tools.BasicEffect);
        //    c.Data.RepeatWidth = 2000;
        //    c.Data.RepeatHeight = 2000;
        //    c.Data.MiddleOnly = false;
        //    c.Data.CenterOnly = true;
        //    c.Data.UV_Multiples = new Vector2(1, 1);
        //    c.Center.U_Wrap = c.Center.V_Wrap = false;

        //    ParseExtraBlockInfo(c, width, bits);

        //    return c;
        //}

        void ParseExtraBlockInfo(PieceQuad c, int width, List<string> bits)
        {
            c.Center.SetTextureOrAnim(bits[1]);
            //c.Center.TextureName = bits[1];

            //int tex_width = c.Center.MyTexture.Tex.Width;
            //int tex_height = c.Center.MyTexture.Tex.Height;
            int tex_width = c.Center.TexWidth;
            int tex_height = c.Center.TexHeight;

            for (int i = 2; i < bits.Count; i++)
            {
                switch (bits[i])
                {
                    case "box_height": c.BoxHeight = 2 * float.Parse(bits[i + 1]); break;
                    case "width": c.Data.RepeatWidth = 2*float.Parse(bits[i + 1]); break;
                    case "height": c.Data.RepeatHeight = 2*float.Parse(bits[i + 1]); break;
                    case "left": c.Data.Center_BL_Shift.X = float.Parse(bits[i + 1]); break;
                    case "right": c.Data.Center_TR_Shift.X = float.Parse(bits[i + 1]); break;
                    case "top": 
                        var shift = float.Parse(bits[i + 1]);
                        c.Data.Center_TR_Shift.Y = shift;
                        c.Data.Center_BL_Shift.Y = shift;
                        break;
                }
            }

            // Extend the quad down to properly scale quad
            if (c.FixedHeight == 0)
            {
                float sprite_width = 2 * width + c.Data.Center_TR_Shift.X - c.Data.Center_BL_Shift.X;
                c.FixedHeight = sprite_width * (float)tex_height / (float)tex_width;
            }
        }

        /*
        void ReadBlockToFields(object group, List<string> bits)
        {
            var FieldNames = new List<string>();
            foreach (var field in group.GetType().GetFields())
            {
                FieldNames.Add(field.Name);
            }

            var dict = Tools.GetLocations(bits, FieldNames);

            foreach (var field in group.GetType().GetFields())
            {
                if (!field.IsPublic && dict.ContainsKey(field.Name)) continue;

                if (field.FieldType == typeof(int))
                {
                    field.SetValue(group, int.Parse(bits[dict[field.Name] + 1]));
                }
            }
        }
        */

        public PieceQuad GetPieceTemplate(BlockBase block, Rand rnd) { return GetPieceTemplate(block, rnd, null); }
        public PieceQuad GetPieceTemplate(BlockBase block, Rand rnd, BlockGroup group)
        {
            if (group == null)
            {
                if (block.Box.TopOnly && block.BlockCore.UseTopOnlyTexture)
                    group = Platforms;
                else
                    group = Pillars;
            }
                
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

            int int_width = group.SnapWidthUp(width - .1f);

            // Get the piecequad template
            try
            {
                return group.Choose(int_width, rnd);
            }
            catch
            {
                Tools.Log(string.Format("Could not find {0} of width {1} for tileset {2}",
                    "block", width, Name));
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

        public BackgroundTemplate MyBackgroundType;

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