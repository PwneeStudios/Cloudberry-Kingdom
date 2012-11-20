using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public partial class TileSets
    {
        static TileSet Load_Hills()
        {
            var t = GetOrMakeTileset("Hills");
            var info = t.MyTileSetInfo;

            t._Start();

t.Name = "hills";

t.Pillars.Add(new PieceQuad(50, "pillar_hills_50", -15, 15, 3));
t.Pillars.Add(new PieceQuad(100, "pillar_hills_100", -15, 15, 3));
t.Pillars.Add(new PieceQuad(150, "pillar_hills_150", -15, 15, 3));
t.Pillars.Add(new PieceQuad(250, "pillar_hills_250", -15, 15, 3));
t.Pillars.Add(new PieceQuad(300, "pillar_hills_300", -15, 15, 3));
t.Pillars.Add(new PieceQuad(600, "pillar_hills_600", -15, 15, 3));
t.Pillars.Add(new PieceQuad(1000, "pillar_hills_1000", -15, 15, 3));

t.StartBlock.Add(new PieceQuad(900, "wall_hills", -880 + 1000, 40 - 0, 1650));
t.EndBlock.Add(new PieceQuad(900, "wall_hills", -40, 880 - 1000, 1650));

info.ShiftStartDoor = -140;
info.ShiftStartBlock = new Vector2(200, 0);

sprite_anim("door_hills", "door_hills", 1, 2, 2);
info.Doors.Sprite.Sprite = "door_hills";
info.Doors.Sprite.Size = new Vector2(305, -1);
info.Doors.Sprite.Offset = new Vector2(-140, 33);
info.Doors.ShiftStart = new Vector2(0, 190);

info.Walls.Sprite.Sprite = "pillar_hills_1000";
info.Walls.Sprite.Size = new Vector2(1500, -1);
info.Walls.Sprite.Offset = new Vector2(0, 4615);
info.Walls.Sprite.Degrees = -90;

info.LavaDrips.Line.End1 = "Flow_cave_1";
info.LavaDrips.Line.Sprite = "Flow_cave_2";
info.LavaDrips.Line.End2 = "Flow_Cave_3";

info.Lasers.Line.Sprite = "Laser_Hills";
info.Lasers.Line.RepeatWidth = 135;
info.Lasers.Line.Dir = 0;
info.Lasers.Scale = 1;
info.Lasers.Tint_Full = new Vector4(1, 1, 1, .95f);
info.Lasers.Tint_Half = new Vector4(1, 1, 1, .4f);

sprite_anim("fblock_hills", "fblock_hills", 1, 3, 2);
info.FallingBlocks.Group.Add(new PieceQuad(110, "fblock_hills", -3, 3, 2, false, 103 + 10, false));

sprite_anim("Bouncy_hills", "Bouncy_hills", 1, 3, 2);
info.BouncyBlocks.Group.Add(new PieceQuad(124, "bouncy_hills", -15, 15, 13, false, 124, false));

sprite_anim("flame_Hills", "firespinner_flame_Hills", 1, 4, 6);
info.Spinners.Flame.Sprite = "flame_Hills";
info.Spinners.Flame.Size = new Vector2(45, -1);
info.Spinners.Rotate = false;
info.Spinners.RotateStep = .13f;
info.Spinners.Base.Sprite = "firespinner_base_hills";
info.Spinners.Base.Size = new Vector2(90, -1);
info.Spinners.Base.Offset = new Vector2(0, -25);
info.Spinners.SegmentSpacing = 65;
info.Spinners.SpaceFromBase = 55;

info.GhostBlocks.Sprite = "ghostblock_hills";
info.GhostBlocks.Shift = new Vector2(0, -15);

info.MovingBlocks.Group.Add(new PieceQuad(190, "movingblock_hills_190", -1, 1, 7, false, 190 + 3, false));
info.MovingBlocks.Group.Add(new PieceQuad(135, "movingblock_hills_135", -1, 1, 6, false, 135 - 1, false));
info.MovingBlocks.Group.Add(new PieceQuad(80, "movingblock_hills_80", -1, 1, 1, false, 80 + 3, false));
info.MovingBlocks.Group.Add(new PieceQuad(40, "movingblock_hills_40", -1, 1, 1, false, 40 + 3, false));

info.Elevators.Group.Add(new PieceQuad(40, "Elevator_Hills_40", -1, 1, 1, false, -3f, true));
info.Elevators.Group.Add(new PieceQuad(80, "Elevator_Hills_80", -1, 1, 1, false, -1.5f, true));
info.Elevators.Group.Add(new PieceQuad(135, "Elevator_Hills_135", -1, 1, 1, false, -1.5f, true));
info.Elevators.Group.Add(new PieceQuad(190, "Elevator_Hills_190", -1, 1, 1, false, -1.5f, true));

info.Pendulums.Group.Add(new PieceQuad(40, "Elevator_Hills_40", -1, 1, 1, false, -3f, true));
info.Pendulums.Group.Add(new PieceQuad(80, "Elevator_Hills_80", -1, 1, 1, false, -1.5f, true));
info.Pendulums.Group.Add(new PieceQuad(135, "Elevator_Hills_135", -1, 1, 1, false, -1.5f, true));
info.Pendulums.Group.Add(new PieceQuad(190, "Elevator_Hills_190", -1, 1, 1, false, -1.5f, true));

sprite_anim("Serpent_Hills", "Serpent_Castle", 1, 2, 5);
info.Serpents.Serpent.Sprite = "Serpent_Hills";
sprite_anim("Serpent_Fish_Hills", "Serpent_Fish_Castle", 1, 2, 8);
info.Serpents.Fish.Sprite = "Serpent_Fish_Hills";
info.Serpents.Fish.Size = new Vector2(60, -1);
info.Serpents.Fish.Offset = new Vector2(55, 0);

info.Spikes.Spike.Sprite = "spike_hills";
info.Spikes.Spike.Size = new Vector2(38, -1);
info.Spikes.Spike.Offset = new Vector2(0, 1);
info.Spikes.Spike.RelativeOffset = true;
info.Spikes.Base.Sprite = "spike_base_hills_1";
info.Spikes.Base.Size = new Vector2(54, -1);
info.Spikes.PeakHeight = .335f;

info.Boulders.Ball.Sprite = "floater_boulder_hills";
info.Boulders.Ball.Size = new Vector2(200, -1);
info.Boulders.Radius = 140;
info.Boulders.Chain.Sprite = "floater_rope_hills";
info.Boulders.Chain.Width = 55;
info.Boulders.Chain.RepeatWidth = 1900;

info.SpikeyGuys.Ball.Sprite = "floater_spikey_hills";
info.SpikeyGuys.Ball.Size = new Vector2(150, -1);
info.SpikeyGuys.Ball.Offset = new Vector2(0, 8);
info.SpikeyGuys.Base.Sprite = null;
info.SpikeyGuys.Rotate = true;
info.SpikeyGuys.Radius = 116;
info.SpikeyGuys.RotateOffset = -1.57f;
info.SpikeyGuys.Chain.Sprite = "floater_chain_hills";
info.SpikeyGuys.Chain.Width = 55;
info.SpikeyGuys.Chain.RepeatWidth = 1900;

info.SpikeyLines.Ball.Sprite = "Floater_Spikey_Hills";
info.SpikeyLines.Ball.Size = new Vector2(150, -1);
info.SpikeyLines.Ball.Offset = new Vector2(-8, 12);
info.SpikeyLines.Radius = 100;
info.SpikeyLines.Rotate = true;
info.SpikeyLines.RotateSpeed = .05f;

sprite_anim("blob_hills", "blob_hills", 1, 4, 2);
info.Blobs.Body.Sprite = "blob_hills";
info.Blobs.Body.Size = new Vector2(130, -1);
info.Blobs.Body.Offset = new Vector2(20, 20);
info.Blobs.GooSprite = "BlobGoo5";

info.Clouds.Sprite.Sprite = "cloud_hills";

info.Fireballs.Sprite.ColorMatrix = ColorHelper.HsvTransform(1, 1, 0);

//info.Coins.Sprite.Sprite = "coin_blue";
info.Coins.Sprite.Sprite = "CoinShimmer";
info.Coins.Sprite.Size = new Vector2(105, -1);
info.Coins.ShowCoin = true;
info.Coins.ShowEffect = true;
info.Coins.ShowText = true;

info.AllowLava = false;

            t._Finish();
            
            return t;
        }
    }
}
