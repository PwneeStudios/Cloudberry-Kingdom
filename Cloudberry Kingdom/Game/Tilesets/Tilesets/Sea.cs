using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public partial class TileSets
    {
        static TileSet Load_Sea()
        {
            var t = GetOrMakeTileset("Cloud");
            var info = t.MyTileSetInfo;

            t._Start();

t.Name = "sea";

t.Pillars.Add(new PieceQuad(50, "pillar_sea_50", -15, 15, 3));
t.Pillars.Add(new PieceQuad(100, "pillar_sea_100", -15, 15, 3));
t.Pillars.Add(new PieceQuad(150, "pillar_sea_150", -15, 15, 3));
t.Pillars.Add(new PieceQuad(250, "pillar_sea_250", -15, 15, 3));
t.Pillars.Add(new PieceQuad(300, "pillar_sea_300", -15, 15, 3));
t.Pillars.Add(new PieceQuad(600, "pillar_sea_600", -15, 15, 3));
t.Pillars.Add(new PieceQuad(1000, "pillar_sea_1000", -15, 15, 3));

t.StartBlock.Add(new PieceQuad(400, "wall_sea", -670, 15, 1420));
t.EndBlock.Add(new PieceQuad(400, "wall_sea", -55, 630, 1420));

info.ShiftStartDoor = -140;
info.ShiftStartBlock = new Vector2(320, 0);

sprite_anim("door_sea", "door_sea", 1, 2, 2);
info.Doors.Sprite.Sprite = "door_sea";
info.Doors.Sprite.Size = new Vector2(270, -1);
info.Doors.Sprite.Offset = new Vector2(-140, 38);
info.Doors.ShiftStart = new Vector2(0, 190);

info.Walls.Sprite.Sprite = "pillar_sea_1000";
info.Walls.Sprite.Size = new Vector2(1500, -1);
info.Walls.Sprite.Offset = new Vector2(0, 4550);
info.Walls.Sprite.Degrees = -90;

info.LavaDrips.Line.End1 = "Flow_Sea_1";
info.LavaDrips.Line.Sprite = "Flow_Sea_2";
info.LavaDrips.Line.End2 = "Flow_Sea_3";

info.Lasers.Line.Sprite = "Laser_Sea";
info.Lasers.Line.RepeatWidth = 135;
info.Lasers.Line.Dir = 0;
info.Lasers.Scale = 1;
info.Lasers.Tint_Full = new Vector4(1, 1, 1, .95f);
info.Lasers.Tint_Half = new Vector4(1, 1, 1, .4f);

sprite_anim("fblock_sea", "fblock_sea", 1, 3, 2);
info.FallingBlocks.Group.Add(new PieceQuad(110, "fblock_sea", -3, 3, 2));

sprite_anim("Bouncy_sea", "Bouncy_Cloud", 1, 3, 2);
info.BouncyBlocks.Group.Add(new PieceQuad(124, "bouncy_sea", -6, 6, 13));

sprite_anim("flame_Sea", "firespinner_flame_Sea", 1, 4, 6);
info.Spinners.Flame.Sprite = "flame_Sea";
info.Spinners.Flame.Size = new Vector2(45, -1);
info.Spinners.Rotate = false;
info.Spinners.RotateStep = .13f;
info.Spinners.Base.Sprite = "firespinner_gear_dkpurp";
info.Spinners.Base.Size = new Vector2(90, -1);
info.Spinners.Base.Offset = new Vector2(0, -25);
info.Spinners.SegmentSpacing = 65;
info.Spinners.SpaceFromBase = 55;

info.GhostBlocks.Sprite = "ghostblock_sea";
info.GhostBlocks.Shift = new Vector2(0, -15);

info.MovingBlocks.Group.Add(new PieceQuad(190, "movingblock_sea_190", -1, 1, 12));
info.MovingBlocks.Group.Add(new PieceQuad(135, "movingblock_sea_135", -1, 1, 12));
info.MovingBlocks.Group.Add(new PieceQuad(80, "movingblock_sea_80", -1, 1, 4));
info.MovingBlocks.Group.Add(new PieceQuad(40, "movingblock_sea_40", -1, 1, 4));

info.Elevators.Group.Add(new PieceQuad(40, "Elevator_Sea_40", -1, 1, 1));
info.Elevators.Group.Add(new PieceQuad(80, "Elevator_Sea_80", -1, 1, 1));
info.Elevators.Group.Add(new PieceQuad(135, "Elevator_Sea_135", -1, 1, 1));
info.Elevators.Group.Add(new PieceQuad(190, "Elevator_Sea_190", -1, 1, 1));

info.Pendulums.Group.Add(new PieceQuad(40, "Elevator_Sea_40", -1, 1, 1));
info.Pendulums.Group.Add(new PieceQuad(80, "Elevator_Sea_80", -1, 1, 1));
info.Pendulums.Group.Add(new PieceQuad(135, "Elevator_Sea_135", -1, 1, 1));
info.Pendulums.Group.Add(new PieceQuad(190, "Elevator_Sea_190", -1, 1, 1));

sprite_anim("Serpent_Sea", "Serpent_Sea", 1, 2, 8);
info.Serpents.Serpent.Sprite = "Serpent_Sea";
sprite_anim("Serpent_Fish_Sea", "Serpent_Fish_Sea", 1, 2, 5);
info.Serpents.Fish.Sprite = "Serpent_Fish_Sea";
info.Serpents.Fish.Size = new Vector2(60, -1);
info.Serpents.Fish.Offset = new Vector2(55, 0);

info.Spikes.Spike.Sprite = "Spike_Sea_2";
info.Spikes.Spike.Size = new Vector2(38, -1);
info.Spikes.Spike.Offset = new Vector2(0, 1);
info.Spikes.Spike.RelativeOffset = true;
info.Spikes.Base.Sprite = "spike_base_sea_1";
info.Spikes.Base.Size = new Vector2(54, -1);
info.Spikes.PeakHeight = .335f;

info.Boulders.Ball.Size = new Vector2(170, -1);
info.Boulders.Radius = 120;
info.Boulders.Ball.Sprite = "Floater_Boulder_Cloud";
info.Boulders.Ball.Size = new Vector2(200, -1);
info.Boulders.Radius = 140;
info.Boulders.Chain.Sprite = "Floater_Rope_Cloud";
info.Boulders.Chain.RepeatWidth = 1900;
info.Boulders.Chain.Width = 55;

info.SpikeyGuys.Ball.Sprite = "Floater_Spikey_Sea";
info.SpikeyGuys.Ball.Size = new Vector2(170, -1);
info.SpikeyGuys.Ball.Offset = new Vector2(0, 9);
info.SpikeyGuys.Base.Sprite = null;
info.SpikeyGuys.Rotate = true;
info.SpikeyGuys.Radius = 124;
info.SpikeyGuys.RotateOffset = -1.95f;
info.SpikeyGuys.Chain.Sprite = "floater_chain_sea";
info.SpikeyGuys.Chain.Width = 55;
info.SpikeyGuys.Chain.RepeatWidth = 1900;

info.SpikeyLines.Ball.Sprite = "Floater_Spikey_Sea";
info.SpikeyLines.Ball.Size = new Vector2(150, -1);
info.SpikeyLines.Ball.Offset = new Vector2(-8, 12);
info.SpikeyLines.Radius = 100;
info.SpikeyLines.Rotate = true;
info.SpikeyLines.RotateSpeed = .05f;

sprite_anim("blob_sea", "blob_sea", 1, 4, 2);
info.Blobs.Body.Sprite = "blob_sea";
info.Blobs.Body.Size = new Vector2(130, -1);
info.Blobs.Body.Offset = new Vector2(20, 20);
info.Blobs.GooSprite = "BlobGoo5";

info.Clouds.Sprite.Sprite = "cloud_sea";

info.Fireballs.Sprite.ColorMatrix = ColorHelper.HsvTransform(1, 1, 132);

//info.Coins.Sprite.Sprite = "coin_blue";
info.Coins.Sprite.Sprite = "CoinShimmer";
info.Coins.Sprite.Size = new Vector2(105, -1);
info.Coins.ShowCoin = true;
info.Coins.ShowEffect = true;
info.Coins.ShowText = true;

info.AllowLava = false;
info.ObstacleCutoff =  200;

            t._Finish();

            return t;
        }
    }
}
