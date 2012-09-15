using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public partial class TileSets
    {
        static TileSet Load_Castle()
        {
            var t = GetOrMakeTileset("Cloud");
            var info = t.MyTileSetInfo;

            t._Start();

t.Name = "castle";

t.Pillars.Add(new PieceQuad(50, "pillar_castle_50", -15, 15, 1));
t.Pillars.Add(new PieceQuad(100, "pillar_castle_100", -15, 15, 1));
t.Pillars.Add(new PieceQuad(150, "pillar_castle_150", -15, 15, 1));
t.Pillars.Add(new PieceQuad(250, "pillar_castle_250", -15, 15, 1));
t.Pillars.Add(new PieceQuad(300, "pillar_castle_300", -15, 15, 1));
t.Pillars.Add(new PieceQuad(600, "pillar_castle_600", -15, 15, 1));
t.Pillars.Add(new PieceQuad(1000, "pillar_castle_1000", -15, 15, 1));

t.Ceilings.Add(new PieceQuad(50, "pillar_castle_50", -20, 20, 0, true));
t.Ceilings.Add(new PieceQuad(100, "pillar_castle_100", -20, 20, 0, true));
t.Ceilings.Add(new PieceQuad(150, "pillar_castle_150", -20, 20, 0, true));
t.Ceilings.Add(new PieceQuad(250, "pillar_castle_250", -20, 20, 0, true));
t.Ceilings.Add(new PieceQuad(300, "pillar_castle_300", -20, 20, 0, true));
t.Ceilings.Add(new PieceQuad(600, "pillar_castle_600", -20, 20, 0, true));
t.Ceilings.Add(new PieceQuad(1000, "pillar_castle_1000", -20, 20, 0, true));

//t.StartBlock.Add(new PieceQuad(400, "wall_castle", -670, 15, 1407));
//t.EndBlock.Add(new PieceQuad(400, "wall_castle", -15, 670, 1407));
t.StartBlock.Add(new PieceQuad(400, "wall_castle", -950, 15, 1670));
t.EndBlock.Add(new PieceQuad(400, "wall_castle", -45, 920, 1670));

info.ShiftStartDoor = 0;
info.ShiftStartBlock = new Vector2(100, 0);

sprite_anim("door_castle", "door_castle", 1, 2, 2);
info.Doors.Sprite.Sprite = "door_castle";
info.Doors.Sprite.Size = new Vector2(310, -1);
info.Doors.Sprite.Size = new Vector2(450, 250);
info.Doors.Sprite.Offset = new Vector2(-210, 35);
info.Doors.ShiftStart = new Vector2(0, 190);

info.Walls.Sprite.Sprite = "pillar_castle_1000";
info.Walls.Sprite.Size = new Vector2(1500, -1);
info.Walls.Sprite.Offset = new Vector2(0, 4635);
info.Walls.Sprite.Size = new Vector2(1300, -1);
info.Walls.Sprite.Offset = new Vector2(0, 4815);
info.Walls.Sprite.Degrees = -90;

info.LavaDrips.Line.End1 = "Flow_Castle_1";
info.LavaDrips.Line.Sprite = "Flow_Castle_2";
info.LavaDrips.Line.End2 = "Flow_Castle_3";
info.LavaDrips.Icon.Sprite = "FlowCastle_1";

info.Lasers.Line.Sprite = "Laser_Castle";
info.Lasers.Line.RepeatWidth = 135;
info.Lasers.Line.Dir = 0;
info.Lasers.Scale = 1;
info.Lasers.Tint_Full = new Vector4(1, 1, 1, .95f);
info.Lasers.Tint_Half = new Vector4(1, 1, 1, .4f);
info.Lasers.Icon.Sprite = "Icon_Laser";

sprite_anim("fblock_castle", "fblock_castle", 1, 3, 2);
info.FallingBlocks.Group.Add(new PieceQuad(103, "fblock_castle", -3, 3, 2));
info.FallingBlocks.Icon.Sprite = "fblock_castle";
info.FallingBlocks.Icon.Size = new Vector2(40, -1);

sprite_anim("Bouncy_castle", "Bouncy_castle", 1, 3, 2);
info.BouncyBlocks.Group.Add(new PieceQuad(124, "bouncy_castle", -6, 6, 13));
info.BouncyBlocks.Icon.Sprite = "Bouncy_Castle";

sprite_anim("flame_castle", "firespinner_flame_castle_v1", 1, 4, 6);
info.Spinners.Flame.Sprite = "flame_castle";
info.Spinners.Flame.Size = new Vector2(45, -1);
info.Spinners.Rotate = false;
info.Spinners.RotateStep = .13f;
info.Spinners.Base.Sprite = "firespinner_base_castle_2";
info.Spinners.Base.Size = new Vector2(90, -1);
info.Spinners.Base.Offset = new Vector2(0, -25);
info.Spinners.SegmentSpacing = 65;
info.Spinners.SpaceFromBase = 55;
info.Spinners.Icon.Sprite = "Icon_FireSpinner";

info.GhostBlocks.Sprite = "ghostblock_castle";
info.GhostBlocks.Shift = new Vector2(0, -15);
info.GhostBlocks.Icon.Sprite = "Ghostblock_Castle";
info.GhostBlocks.Icon.Size = new Vector2(40, -1);

info.MovingBlocks.Group.Add(new PieceQuad(190, "movingblock_castle_190", -1, 1, 25));
info.MovingBlocks.Group.Add(new PieceQuad(135, "movingblock_castle_135", -1, 1, 25));
info.MovingBlocks.Group.Add(new PieceQuad(80, "movingblock_castle_80", -1, 1, 5));
info.MovingBlocks.Group.Add(new PieceQuad(40, "movingblock_castle_40", -1, 1, 5));
info.MovingBlocks.Icon.Sprite = "MovingBlock_Castle";
info.MovingBlocks.Icon.Size = new Vector2(40, -1);

info.Elevators.Group.Add(new PieceQuad(40, "Elevator_Castle_40", -1, 1, 1));
info.Elevators.Group.Add(new PieceQuad(80, "Elevator_Castle_80", -1, 1, 1));
info.Elevators.Group.Add(new PieceQuad(135, "Elevator_Castle_135", -1, 1, 1));
info.Elevators.Group.Add(new PieceQuad(190, "Elevator_Castle_190", -1, 1, 1));
info.Elevators.Icon.Sprite = "Elevator_Castle_80";

info.Pendulums.Group.Add(new PieceQuad(40, "Elevator_Castle_40", -1, 1, 1));
info.Pendulums.Group.Add(new PieceQuad(80, "Elevator_Castle_80", -1, 1, 1));
info.Pendulums.Group.Add(new PieceQuad(135, "Elevator_Castle_135", -1, 1, 1));
info.Pendulums.Group.Add(new PieceQuad(190, "Elevator_Castle_190", -1, 1, 1));
info.Pendulums.Icon.Sprite = "Elevator_Castle_80";

sprite_anim("Serpent_Castle", "Serpent_Castle", 1, 2, 8);
info.Serpents.Serpent.Sprite = "Serpent_Castle";
info.Serpents.Serpent.Offset = new Vector2(0, -.675f);
sprite_anim("Serpent_Fish_Castle", "Serpent_Fish_Castle", 1, 2, 5);
info.Serpents.Fish.Sprite = "Serpent_Fish_Castle";
info.Serpents.Fish.Size = new Vector2(60, -1);
info.Serpents.Fish.Offset = new Vector2(55, 0);
info.Serpents.Icon.Sprite = "Serpent_Castle";

info.Spikes.Spike.Sprite = "spike_castle";
info.Spikes.Spike.Size = new Vector2(38, -1);
info.Spikes.Spike.Offset = new Vector2(0, 1);
info.Spikes.Spike.RelativeOffset = true;
info.Spikes.Base.Sprite = "spike_base_castle";
info.Spikes.Base.Size = new Vector2(54, -1);
info.Spikes.PeakHeight = .335f;
info.Spikes.Icon.Sprite = "Spike_Castle";
info.Spikes.Icon.Size = new Vector2(25, -1);

info.SpikeyGuys.Ball.Sprite = "Floater_Spikey_castle_v1";
info.SpikeyGuys.Ball.Size = new Vector2(150, -1);
info.SpikeyGuys.Radius = 106;
info.SpikeyGuys.Chain.Sprite = "floater_chain_castle";
info.SpikeyGuys.Chain.Width = 55;
info.SpikeyGuys.Chain.RepeatWidth = 1900;
info.SpikeyGuys.Icon.Sprite = "Floater_Spikey_Castle_v1";

info.Orbs.Ball.Sprite = "floater_buzzsaw_yellow_castle";
info.Orbs.Ball.Size = new Vector2(190, -1);
info.Orbs.Ball.Offset = new Vector2(0, 0);
info.Orbs.Base.Sprite = null;
info.Orbs.Rotate = true;
info.Orbs.RotateSpeed = -.15f;
info.Orbs.Radius = 130;
info.Orbs.RotateOffset = -1.57f;
info.Orbs.Chain.Sprite = "floater_chain_castle";
info.Orbs.Chain.Width = 55;
info.Orbs.Chain.RepeatWidth = 1900;
info.Orbs.Icon.Sprite = "Floater_Buzzsaw_Yellow_Castle";

info.SpikeyLines.Ball.Sprite = "Floater_Spikey_Castle_v2";
info.SpikeyLines.Ball.Size = new Vector2(150, -1);
info.SpikeyLines.Ball.Offset = new Vector2(-8, 12);
info.SpikeyLines.Radius = 100;
info.SpikeyLines.Rotate = true;
info.SpikeyLines.RotateSpeed = .05f;
info.SpikeyLines.Icon.Sprite = "Floater_Spikey_Castle_v2";

sprite_anim("blob_castle", "blob_castle_v1", 1, 4, 2);
info.Blobs.Body.Sprite = "blob_castle";
info.Blobs.Body.Size = new Vector2(137, -1);
info.Blobs.Body.Offset = new Vector2(20, 20);
info.Blobs.GooSprite = "BlobGoo5";
info.Blobs.Icon.Sprite = "Blob_Castle";

info.Clouds.Sprite.Sprite = "cloud_castle";
info.Clouds.Icon.Sprite = "Cloud_Castle";

info.Coins.Sprite.Sprite = "coin_blue";
info.Coins.Sprite.Size = new Vector2(50, -1);
info.Coins.ShowCoin = true;
info.Coins.ShowEffect = true;
info.Coins.ShowText = true;

info.AllowLava = true;
info.ObstacleCutoff = 70;

            t._Finish();

            return t;
        }
    }
}
