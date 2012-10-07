using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using CoreEngine;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public abstract class TileInfoBase
    {
        public SpriteInfo Icon = new SpriteInfo(null, new Vector2(50, -1));
    }

    /// <summary>
    /// Stores a tile set's information, including what obstacles are allowed.
    /// </summary>
    public class TileSet
    {
        public static TileInfoBase UpgradeToInfo(Upgrade upgrade, TileSet tile)
        {
            TileSet.TileSetInfo info = tile.MyTileSetInfo;

            switch (upgrade)
            {
                case Upgrade.BouncyBlock: return info.BouncyBlocks;
                case Upgrade.Ceiling: return null;
                case Upgrade.Cloud: return info.Clouds;
                case Upgrade.Conveyor: return null;
                case Upgrade.Elevator: return info.Elevators;
                case Upgrade.FallingBlock: return info.FallingBlocks;
                case Upgrade.Fireball: return info.Fireballs;
                case Upgrade.Firesnake: return info.Firesnakes;
                case Upgrade.FireSpinner: return info.Spinners;
                case Upgrade.FlyBlob: return info.Blobs;
                case Upgrade.FlyingBlock: return null;
                case Upgrade.General: return null;
                case Upgrade.GhostBlock: return info.GhostBlocks;
                case Upgrade.Jump: return null;
                case Upgrade.Laser: return info.Lasers;
                case Upgrade.LavaDrip: return info.LavaDrips;
                case Upgrade.MovingBlock: return info.MovingBlocks;
                case Upgrade.Pendulum: return info.Pendulums;
                case Upgrade.Pinky: return info.Orbs;
                case Upgrade.Serpent: return info.Serpents;
                case Upgrade.Speed: return null;
                case Upgrade.Spike: return info.Spikes;
                case Upgrade.SpikeyGuy: return info.SpikeyGuys;
                case Upgrade.SpikeyLine: return info.SpikeyLines;

                default: return null;
            }
        }

        public static implicit operator TileSet(string name)
        {
            return TileSets.NameLookup[name];
        }

        public class TileSetInfo
        {
            public float ScaleAll = 1f, ScaleAllBlocks = 1f, ScaleAllObjects = 1f;

            public bool AllowLava = true;
            public float ObstacleCutoff = 1000;

            public float ShiftStartDoor = 0;
            public Vector2 ShiftStartBlock = Vector2.Zero;

            public bool AllowTopOnlyBlocks = true;

            public Wall.WallTileInfo Walls = new Wall.WallTileInfo();

            public Pendulum.PendulumTileInfo Pendulums = new Pendulum.PendulumTileInfo();
            public LavaDrips.LavaDrip.LavaDripTileInfo LavaDrips = new LavaDrips.LavaDrip.LavaDripTileInfo();
            public Serpents.Serpent.SerpentTileInfo Serpents = new Serpents.Serpent.SerpentTileInfo();
            public Firesnake.FiresnakeTileInfo Firesnakes = new Firesnake.FiresnakeTileInfo();

            public Clouds.Cloud.CloudTileInfo Clouds = new Clouds.Cloud.CloudTileInfo();
            public MovingPlatform.ElevatorTileInfo Elevators = new MovingPlatform.ElevatorTileInfo();
            public MovingBlock.MovingBlockTileInfo MovingBlocks = new MovingBlock.MovingBlockTileInfo();
            public BouncyBlock.BouncyBlockTileInfo BouncyBlocks = new BouncyBlock.BouncyBlockTileInfo();
            public FallingBlock.FallingBlockTileInfo FallingBlocks = new FallingBlock.FallingBlockTileInfo();
            public GhostBlock.GhostBlockTileInfo GhostBlocks = new GhostBlock.GhostBlockTileInfo();
            public Goombas.Goomba.GoombaTileInfo Blobs = new Goombas.Goomba.GoombaTileInfo();

            public Laser.LaserTileInfo Lasers = new Laser.LaserTileInfo();
            public SpikeyLine.SpikeyLineTileInfo SpikeyLines = new SpikeyLine.SpikeyLineTileInfo();
            public Floater_Spin.SpinTileInfo Orbs = new Floater_Spin.SpinTileInfo();
            public Floater.SpikeyGuyTileInfo SpikeyGuys = new Floater.SpikeyGuyTileInfo();
            public Spikes.Spike.SpikeTileInfo Spikes = new Spikes.Spike.SpikeTileInfo();
            public FireSpinners.FireSpinner.FireSpinnerTileInfo Spinners = new FireSpinners.FireSpinner.FireSpinnerTileInfo();
            public Fireball.FireballTileInfo Fireballs = new Fireball.FireballTileInfo();

            public Checkpoint.CheckpointTileInfo Checkpoints = new Checkpoint.CheckpointTileInfo();
            public Coins.Coin.CoinTileInfo Coins = new Coins.Coin.CoinTileInfo();
            public Door.DoorTileInfo Doors = new Door.DoorTileInfo();
        }
        public TileSetInfo MyTileSetInfo;

        public TileSet()
        {
            MakeNew();
        }

        public TileSet SetName(string Name)
        {
            this.Name = Name;
            return this;
        }

        public TileSet SetBackground(string background)
        {
            MyBackgroundType = BackgroundType.NameLookup[background];
            return this;
        }

        public void MakeNew()
        {
            IsLoaded = false;

            MyTileSetInfo = new TileSetInfo();

            CustomStartEnd = false;
            DungeonLike = false;
            //DoorType = Door.Types.Brick;

            Pillars = new BlockGroup();
            Platforms = new BlockGroup();
            Ceilings = new BlockGroup();
            StartBlock = new BlockGroup();
            EndBlock = new BlockGroup();

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

        // New tile set stuff
        public bool IsLoaded;
        public bool CustomStartEnd;
        public BlockGroup Pillars, Platforms, Ceilings, StartBlock, EndBlock;
        public PieceQuad Wall;

        public bool FixedWidths;
        public bool ProvidesTemplates;
        public int[] PillarWidths, PlatformWidths;

        /// <summary>
        /// Read tileset info from a file.
        /// </summary>
        public void Read(String path)
        {
            MyPath = path;
            _Start();

            // Find path
            Tools.UseInvariantCulture();
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
                    // Is it a ceiling?
                    else if (first.Contains("Ceiling_"))
                    {
                        HasCeiling = true;
                        var pq = ParseBlock(bits, first, Ceilings);
                        pq.Data.BottomFlush = true;
                    }
                    // Is it a start piece?
                    if (first.Contains("Start_"))
                    {
                        ParseBlock(bits, first, StartBlock);
                    }
                    // Is it an end piece?
                    if (first.Contains("End_"))
                    {
                        ParseBlock(bits, first, EndBlock);
                    }
                    // Is it a moving block?
                    else if (first.Contains("MovingBlock_"))
                    {
                        ParseBlock(bits, first, MyTileSetInfo.MovingBlocks.Group);
                    }
                    // Is it an elevator block?
                    else if (first.Contains("Elevator_"))
                    {
                        ParseBlock(bits, first, MyTileSetInfo.Elevators.Group);
                    }
                    // Is it a pendulum block?
                    else if (first.Contains("Pendulum_"))
                    {
                        ParseBlock(bits, first, MyTileSetInfo.Pendulums.Group);
                    }
                    // Is it a falling block?
                    else if (first.Contains("FallingBlock_"))
                    {
                        ParseBlock(bits, first, MyTileSetInfo.FallingBlocks.Group);
                    }
                    // Is it a bouncy block?
                    else if (first.Contains("BouncyBlock_"))
                    {
                        ParseBlock(bits, first, MyTileSetInfo.BouncyBlocks.Group);
                    }
                    else switch (first)
                    {
                        case "sprite_anim":
                            var dict = Tools.GetLocations(bits, "name", "file", "size", "frames", "frame_length", "reverse_at_end");

                            var name = bits[dict["name"] + 1];
                            var file = bits[dict["file"] + 1];

                            AnimationData_Texture sprite_anim = null;
                            if (dict.ContainsKey("frames"))
                            {
                                int start_frame = int.Parse(bits[dict["frames"] + 1]);
                                int end_frame;
                                if (bits[dict["frames"] + 2][0] == 't')
                                    end_frame = int.Parse(bits[dict["frames"] + 3]);
                                else
                                    end_frame = int.Parse(bits[dict["frames"] + 2]);
                                sprite_anim = new AnimationData_Texture(file, start_frame, end_frame);
                            }

                            if (dict.ContainsKey("frame_length"))
                            {
                                var frame_length = int.Parse(bits[dict["frame_length"] + 1]);
                                sprite_anim.Anims[0].Speed = 1f / frame_length;
                            }
                            
                            Tools.TextureWad.Add(sprite_anim, name);

                            break;

                        case "BackgroundFile":
                            BackgroundTemplate template;
                            try
                            {
                                template = BackgroundType.NameLookup[bits[1]];
                            }
                            catch
                            {
                                template = new BackgroundTemplate();
                                template.Name = bits[1];
                            }

                            MyBackgroundType = template;

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

            _Finish();
        }

        public void _Finish()
        {
            // Sort widths
            Pillars.SortWidths();
            Platforms.SortWidths();
            Ceilings.SortWidths();
            StartBlock.SortWidths();
            EndBlock.SortWidths();

            if (MyTileSetInfo.Pendulums.Group.Dict.Count == 0) MyTileSetInfo.Pendulums.Group = PieceQuad.ElevatorGroup;
            if (MyTileSetInfo.Elevators.Group.Dict.Count == 0) MyTileSetInfo.Elevators.Group = PieceQuad.ElevatorGroup;
            if (MyTileSetInfo.FallingBlocks.Group.Dict.Count == 0) MyTileSetInfo.FallingBlocks.Group = PieceQuad.FallGroup;
            if (MyTileSetInfo.BouncyBlocks.Group.Dict.Count == 0) MyTileSetInfo.BouncyBlocks.Group = PieceQuad.BouncyGroup;
            if (MyTileSetInfo.MovingBlocks.Group.Dict.Count == 0) MyTileSetInfo.MovingBlocks.Group = PieceQuad.MovingGroup;

            MyTileSetInfo.Pendulums.Group.SortWidths();
            MyTileSetInfo.Elevators.Group.SortWidths();
            MyTileSetInfo.FallingBlocks.Group.SortWidths();
            MyTileSetInfo.MovingBlocks.Group.SortWidths();
            MyTileSetInfo.BouncyBlocks.Group.SortWidths();
        }

        public void _Start()
        {
            IsLoaded = true;
            CustomStartEnd = true;

            MyTileSetInfo.AllowTopOnlyBlocks = false;

            FixedWidths = true;
            ProvidesTemplates = true;
            MyBackgroundType = BackgroundType.MarioSky;

            MyTileSetInfo.Pendulums.Group = new BlockGroup();
            MyTileSetInfo.Elevators.Group = new BlockGroup();
            MyTileSetInfo.FallingBlocks.Group = new BlockGroup();
            MyTileSetInfo.BouncyBlocks.Group = new BlockGroup();
            MyTileSetInfo.MovingBlocks.Group = new BlockGroup();
        }

        public PieceQuad ParseBlock(List<string> bits, string first, BlockGroup group)
        {
            // Get the block width
            var num_str = first.Substring(first.IndexOf("_") + 1);
            int width = int.Parse(num_str);

            // Get the rest of the information
            var piecequad = ParseBlockLine(width, bits);
            group.Add(width, piecequad);

            return piecequad;
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
                c.Data.UV_Multiples = new Vector2(0, 0);
                c.Center.U_Wrap = c.Center.V_Wrap = true;
            }
            else
            {
                c.Data.RepeatWidth = 2000;
                c.Data.RepeatHeight = 2000;
                c.Data.UV_Multiples = new Vector2(1, 0);
                c.Center.U_Wrap = c.Center.V_Wrap = false;

                c.FixedHeight = 0; // Flag to tell ParseExtra to set the height properly
            }

            ParseExtraBlockInfo(c, width, bits);

            return c;
        }

        void ParseExtraBlockInfo(PieceQuad c, int width, List<string> bits)
        {
            c.Center.SetTextureOrAnim(bits[1]);
            //c.Center.TextureName = bits[1];

            //int tex_width = c.Center.MyTexture.Width;
            //int tex_height = c.Center.MyTexture.Height;
            int tex_width = c.Center.TexWidth;
            int tex_height = c.Center.TexHeight;

            for (int i = 2; i < bits.Count; i++)
            {
                switch (bits[i])
                {
                    case "upside_down": c.Data.UpsideDown = true; break;
                    case "mirror": c.Data.Mirror = true; break;
                    case "box_height": c.BoxHeight = 2 * float.Parse(bits[i + 1]); break;
                    case "width": c.Data.RepeatWidth = 2 * float.Parse(bits[i + 1]); break;
                    case "height": c.Data.RepeatHeight = 2 * float.Parse(bits[i + 1]); break;
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

        public PieceQuad GetPieceTemplate(BlockBase block, Rand rnd) { return GetPieceTemplate(block, rnd, null); }
        public PieceQuad GetPieceTemplate(BlockBase block, Rand rnd, BlockGroup group)
        {
            if (group == null)
            {
                if (block.BlockCore.Ceiling)
                    group = Ceilings;
                else if (block.BlockCore.EndPiece)
                    group = EndBlock;
                else if (block.BlockCore.StartPiece)
                    group = StartBlock;
                else
                {
                    if (block.Box.TopOnly && block.BlockCore.UseTopOnlyTexture)
                    {
                        group = Platforms;
                        if (group.Widths.Length == 0)
                            group = Pillars;
                    }
                    else
                        group = Pillars;
                }
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
    public sealed partial class TileSets
    {
        public static TileSet None, Random, Terrace, Castle, Dungeon, CastlePiece, OutsideGrass, TileBlock, Cement, Catwalk, DarkTerrace, CastlePiece2, Dark, Island, _Night, _NightSky;
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
            GuidLookup.AddOrOverwrite(tileset.Guid, tileset);

            // Add the tileset to the Name lookup
            NameLookup.AddOrOverwrite(tileset.Name, tileset);

            // Add the tileset to the Path lookup
            if (tileset.MyPath.Length > 0)
            {
                PathLookup.AddOrOverwrite(tileset.MyPath, tileset);
            }
        }

        public static void LoadCode()
        {
            AddTileSet(Load_Sea().SetBackground("sea"));
            AddTileSet(Load_Sea().SetBackground("sea_rain").SetName("sea_rain"));
            AddTileSet(Load_Hills().SetBackground("hills"));
            AddTileSet(Load_Hills().SetBackground("hills_rain").SetName("hills_rain"));
            AddTileSet(Load_Forest().SetBackground("forest"));
            AddTileSet(Load_Forest().SetBackground("forest_snow").SetName("forest_snow"));
            AddTileSet(Load_Cloud().SetBackground("cloud"));
            AddTileSet(Load_Cloud().SetBackground("cloud_rain").SetName("cloud_rain"));
            AddTileSet(Load_Cave().SetBackground("cave")); 
            AddTileSet(Load_Castle().SetBackground("castle"));
        }

        public static void LoadTileSet(string path)
        {
            var tileset = GetOrMakeTileset(path);
            
            tileset.Read(path);
            AddTileSet(tileset);
        }
        static TileSet GetOrMakeTileset(string path)
        {
            TileSet tileset;
            if (PathLookup.ContainsKey(path))
            {
                tileset = PathLookup[path];
                tileset.MakeNew();
            }
            else
            {
                tileset = new TileSet();

                // Add the tileset to Freeplay
                CustomLevel_GUI.FreeplayTilesets.Add(tileset);
            }

            return tileset;
        }

        /// <summary>
        /// Make a sprite animation and add it to the texture wad.
        /// </summary>
        static void sprite_anim(string name, string texture_root, int start_frame, int end_frame, int frame_length)
        {
            sprite_anim(name, texture_root, start_frame, end_frame, frame_length, false);
        }
        static void sprite_anim(string name, string texture_root, int start_frame, int end_frame, int frame_length, bool reverse_at_end)
        {
            AnimationData_Texture sprite_anim = null;

            sprite_anim = new AnimationData_Texture(texture_root, start_frame, end_frame);

            // Set speed based on how long each frame is.
            sprite_anim.Anims[0].Speed = 1f / frame_length;

            // Add new sprite animation to the texture wad.
            Tools.TextureWad.Add(sprite_anim, name);
        }

        public static void Init()
        {
            TileSet info;

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

            // Sprite effects
            LoadSpriteEffects();

            // New tile sets
            TileSets.LoadCode();

            // Freeplay tilesets
            CustomLevel_GUI.FreeplayTilesets = new List<TileSet>(new TileSet[] { TileSets.Random, "sea", "hills", "forest", "cloud", "cave", "castle" });

            RegularLevel.InitLists();
            foreach (var _info in TileList)
                _info.PostProcess();
        }

        public static void LoadSpriteEffects()
        {
            sprite_anim("CoinShimmer", "Coin", 0, 49, 1);

            sprite_anim("BlobExplosion_v2", "BlobExplosion_v02", 0, 29, 1);
            sprite_anim("BobExplosion_v1", "BobExplosion_v01", 0, 54, 1);
            sprite_anim("CoinCollect_v1", "CoinCollect_v01", 0, 32, 1);
            sprite_anim("CoinCollect_v2", "CoinCollect_v02", 0, 26, 1);
            sprite_anim("CoinCollect_Sparkes_v3", "CoinCollect_Sparkles_v03", 0, 26, 1);
            sprite_anim("CoinCollect_Star_v3", "CoinCollect_Star_v03", 0, 17, 1);
            sprite_anim("CoinCollect_v4", "CoinCollect_v04", 0, 26, 1);
        }
    }
}