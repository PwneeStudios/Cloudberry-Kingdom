using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public partial class TileSets
    {
        static TileSet Load_Forest()
        {
            var t = GetOrMakeTileset("Forest");
            var info = t.MyTileSetInfo;

            t._Start();

t.Name = "forest";

t.Pillars.Add(new PieceQuad(50, "pillar_forest_50", -15, 15, 3));
t.Pillars.Add(new PieceQuad(100, "pillar_forest_100", -15, 15, 3));
t.Pillars.Add(new PieceQuad(150, "pillar_forest_150", -15, 15, 3));
t.Pillars.Add(new PieceQuad(250, "pillar_forest_250", -15, 15, 3));
t.Pillars.Add(new PieceQuad(300, "pillar_forest_300", -15, 15, 3));
t.Pillars.Add(new PieceQuad(600, "pillar_forest_600", -15, 15, 3));
t.Pillars.Add(new PieceQuad(1000, "pillar_forest_1000", -15, 15, 3));

t.StartBlock.Add(new PieceQuad(400, "wall_forest", -670, 15, 1500));
t.EndBlock.Add(new PieceQuad(400, "wall_forest", -15, 670, 1500));

info.ShiftStartDoor = 25;
info.ShiftStartBlock = new Vector2(50, 0);

sprite_anim("door_forest", "door_forest", 1, 2, 2);
info.Doors.Sprite.Sprite = "door_forest";
info.Doors.Sprite.Size = new Vector2(296, -1);
info.Doors.Sprite.Offset = new Vector2(-140, 35);
info.Doors.ShiftStart = new Vector2(0, 190);

info.Walls.Sprite.Sprite = "pillar_forest_1000";
info.Walls.Sprite.Size = new Vector2(1500, -1);
info.Walls.Sprite.Offset = new Vector2(0, 4650);
info.Walls.Sprite.Degrees = -90;

info.LavaDrips.Line.End1 = "Flow_Sea_1";
info.LavaDrips.Line.Sprite = "Flow_Sea_2";
info.LavaDrips.Line.End2 = "Flow_Sea_3";

info.Lasers.Line.Sprite = "Laser_Forest";
info.Lasers.Line.RepeatWidth = 135;
info.Lasers.Line.Dir = 0;
info.Lasers.Scale = 1;
info.Lasers.Tint_Full = new Vector4(1, 1, 1, .95f);
info.Lasers.Tint_Half = new Vector4(1, 1, 1, .4f);

sprite_anim("fblock_forest", "fblock_forest", 1, 3, 2);
info.FallingBlocks.Group.Add(new PieceQuad(103, "fblock_forest", -3, 3, 2, false, 103 + 3, false));

sprite_anim("Bouncy_Forest", "Bouncy_Forest", 1, 3, 2);
info.BouncyBlocks.Group.Add(new PieceQuad(124, "bouncy_Forest", -6, 6, 13, false, 124, false));

sprite_anim("flame_forest", "firespinner_flame_forest", 1, 4, 6);
info.Spinners.Flame.Sprite = "flame_forest";
info.Spinners.Flame.Size = new Vector2(47, -1);
info.Spinners.Rotate = false;
info.Spinners.Base.Sprite = "firespinner_base_forest_1";
info.Spinners.Base.Size = new Vector2(75, -1);
info.Spinners.Base.Offset = new Vector2(0, -45);
info.Spinners.SegmentSpacing = 65;
info.Spinners.SpaceFromBase = 45;
info.Spinners.TopOffset = -40;

info.GhostBlocks.Sprite = "ghostblock_forest_1";
info.GhostBlocks.Shift = new Vector2(0, -15);

info.MovingBlocks.Group.Add(new PieceQuad(190, "movingblock_forest_190_v2", -1, 1, 1, false, 190 + 3, false));
info.MovingBlocks.Group.Add(new PieceQuad(150, "movingblock_forest_150", -1, 1, 1, false, 150 + 3, false));
info.MovingBlocks.Group.Add(new PieceQuad(135, "movingblock_forest_135_v2", -1, 1, 1, false, 135 + 3, false));
info.MovingBlocks.Group.Add(new PieceQuad(80, "movingblock_forest_80_v2", -1, 1, 1, false, 80 + 3, false));
info.MovingBlocks.Group.Add(new PieceQuad(40, "movingblock_forest_40_v2", -1, 1, 1, false, 40 + 3, false));

info.Elevators.Group.Add(new PieceQuad(40, "Elevator_Forest_40", -1, 1, 1, false, -1.5f, true));
info.Elevators.Group.Add(new PieceQuad(80, "Elevator_Forest_80", -1, 1, 1, false, -1.5f, true));
info.Elevators.Group.Add(new PieceQuad(135, "Elevator_Forest_135", -1, 1, 3, false, -1.5f, true));
info.Elevators.Group.Add(new PieceQuad(190, "Elevator_Forest_190", -1, 1, 1, false, -1.5f, true));

info.Pendulums.Group.Add(new PieceQuad(40, "Elevator_Forest_40", -1, 1, 1, false, -1.5f, true));
info.Pendulums.Group.Add(new PieceQuad(80, "Elevator_Forest_80", -1, 1, -2, false, -1.5f, true));
info.Pendulums.Group.Add(new PieceQuad(135, "Elevator_Forest_135", -1, 1, 1, false, -1.5f, true));
info.Pendulums.Group.Add(new PieceQuad(190, "Elevator_Forest_190", -1, 1, 1, false, -1.5f, true));

sprite_anim("Serpent_Forest", "Serpent_Forest", 1, 2, 8);
info.Serpents.Serpent.Sprite = "Serpent_Forest";
sprite_anim("Serpent_Fish_Forest", "Serpent_Fish_Forest", 1, 2, 5);
info.Serpents.Fish.Sprite = "Serpent_Fish_Forest";
info.Serpents.Fish.Size = new Vector2(60, -1);
info.Serpents.Fish.Offset = new Vector2(55, 0);

info.Spikes.Spike.Sprite = "spike_forest";
info.Spikes.Spike.Size = new Vector2(38, -1);
info.Spikes.Spike.Offset = new Vector2(0, 1);
info.Spikes.Spike.RelativeOffset = true;
info.Spikes.Base.Sprite = "spike_base_forest_1";
info.Spikes.Base.Size = new Vector2(54, -1);
info.Spikes.PeakHeight = .335f;

info.Boulders.Ball.Sprite = "floater_spikey_forest";
info.Boulders.Ball.Size = new Vector2(160, -1);
info.Boulders.Radius = 120;
info.Boulders.Chain.Sprite = "floater_chain_forest";
info.Boulders.Chain.Width = 55;
info.Boulders.Chain.RepeatWidth = 1900;

info.SpikeyGuys.Ball.Sprite = "floater_spikey_forest";
info.SpikeyGuys.Ball.Size = new Vector2(150, -1);
info.SpikeyGuys.Ball.Offset = new Vector2(0, -22);
info.SpikeyGuys.Base.Sprite = null;
info.SpikeyGuys.Rotate = true;
info.SpikeyGuys.Radius = 130;
info.SpikeyGuys.RotateOffset = .05f;
info.SpikeyGuys.Chain.Sprite = "floater_chain_forest";
info.SpikeyGuys.Chain.Width = 55;
info.SpikeyGuys.Chain.RepeatWidth = 1900;

info.SpikeyGuys.Ball.Sprite = "floater_buzzsaw_forest";
info.SpikeyGuys.Ball.Size = new Vector2(230, -1);
info.SpikeyGuys.Ball.Offset = new Vector2(0, 0);
info.SpikeyGuys.Base.Sprite = null;
info.SpikeyGuys.Rotate = true;
info.SpikeyGuys.RotateSpeed = -.15f;
info.SpikeyGuys.Radius = 130;
info.SpikeyGuys.RotateOffset = -1.57f;
info.SpikeyGuys.Chain.Sprite = "floater_chain_forest";
info.SpikeyGuys.Chain.Width = 55;
info.SpikeyGuys.Chain.RepeatWidth = 1900;

info.SpikeyLines.Ball.Sprite = "Floater_Spikey_Forest";
info.SpikeyLines.Ball.Size = new Vector2(150, -1);
info.SpikeyLines.Ball.Offset = new Vector2(-8, -10);
info.SpikeyLines.Radius = 100;
info.SpikeyLines.Rotate = true;
info.SpikeyLines.RotateSpeed = .05f;

sprite_anim("blob_forest", "blob_forest", 1, 4, 2);
info.Blobs.Body.Sprite = "blob_forest";
info.Blobs.Body.Size = new Vector2(130, -1);
info.Blobs.GooSprite = "BlobGoo3";

info.Clouds.Sprite.Sprite = "cloud_forest";

info.Fireballs.Sprite.ColorMatrix = ColorHelper.HsvTransform(1, 1, 192.5f);

//info.Coins.Sprite.Sprite = "coin_blue";
info.Coins.Sprite.Sprite = "CoinShimmer";
info.Coins.Sprite.Size = new Vector2(105, -1);
info.Coins.ShowCoin = true;
info.Coins.ShowEffect = true;
info.Coins.ShowText = true;

info.AllowLava = false;
info.ObstacleCutoff = 70;

            t._Finish();

            return t;
        }
    }
}
