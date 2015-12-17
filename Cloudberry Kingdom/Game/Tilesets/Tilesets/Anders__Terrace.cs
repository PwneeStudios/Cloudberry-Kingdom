
using Microsoft.Xna.Framework;

using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom
{
    public partial class TileSets
    {
		static TileSet Load_Anders__Terrace()
		{
			var t = GetOrMakeTileset("Anders__Terrace");
			var info = t.MyTileSetInfo;

			t._Start();

			t.Name = "anders__terrace";

			t.HasCeiling = false;

			t.Pillars.Add(new PieceQuad(100, "pillar_terrace_70", -15, 15, 3));
			t.Pillars.Add(new PieceQuad(250, "pillar_terrace_200", -15, 15, 3));

			t.StartBlock.Add(new PieceQuad(900, "wall_terrace", 625, 10, 2052));
			t.EndBlock.Add(new PieceQuad(900, "wall_terrace", -10, -625, 2052));

			info.ShiftStartDoor = 0;
			info.ShiftStartBlock = new Vector2(100, 0);

			sprite_anim("door_terrace", "door_terrace", 1, 2, 2);
			info.Doors.Sprite.Sprite = "door_terrace";
			info.Doors.Sprite.Size = new Vector2(300, 200);
			info.Doors.Sprite.Offset = new Vector2(-140, 3);
			info.Doors.ShiftStart = new Vector2(0, 190);
			info.Doors.SizePadding = new Vector2(10, 0);

			info.Walls.Sprite.Sprite = "pillar_castle_1000";
			info.Walls.Sprite.Size = new Vector2(1500, -1);
			info.Walls.Sprite.Offset = new Vector2(0, 4635);
			info.Walls.Sprite.Size = new Vector2(1300, -1);
			info.Walls.Sprite.Offset = new Vector2(0, 4815);
			info.Walls.Sprite.Degrees = -90;

			Anders__ObstacleTilesetSetup(info);

			t._Finish();

			return t;
		}

		private static void Anders__ObstacleTilesetSetup(TileSet.TileSetInfo info)
		{
			info.LavaDrips.Line.End1 = "Flow_Castle_1";
			info.LavaDrips.Line.Sprite = "Flow_Castle_2";
			info.LavaDrips.Line.End2 = "Flow_Castle_3";
			info.LavaDrips.Icon.Sprite = "Flow_Icon_Castle";

			info.Lasers.Line.Sprite = "Laser_Anders";
			info.Lasers.Line.RepeatWidth = 135;
			info.Lasers.Line.Dir = 0;
			info.Lasers.Scale = 1;
			info.Lasers.Tint_Full = Laser.Laser_DefaultTint_Full;
			info.Lasers.Tint_Half = new Vector4(1, 1, 1, .4f);
			info.Lasers.Icon.Sprite = "Icon_Laser";
			info.Lasers.Icon.Offset = new Vector2(0, -8);

			sprite_anim("FBlock_Anders", "FBlock_Anders", 1, 3, 2);
			info.FallingBlocks.Group.Add(new PieceQuad(103, "FBlock_Anders", -3, 3, 2, false, 103 + 3, false));
			info.FallingBlocks.Icon.Sprite = "FBlock_Anders";
			info.FallingBlocks.Icon.Size = new Vector2(40, -1);

			sprite_anim("Bouncy_Anders", "Bouncy_Anders", 1, 3, 2);
			info.BouncyBlocks.Group.Add(new PieceQuad(124, "Bouncy_Anders", -6, 6, 13, false, 124, false));
			info.BouncyBlocks.Icon.Sprite = "Bouncy_Anders";

			info.Spinners.Flame.Sprite = "Firespinner_Flame_Anders";
			info.Spinners.Flame.Size = new Vector2(88, -1);
			info.Spinners.RotateStep = .15f;
			info.Spinners.Base.Sprite = null;
			info.Spinners.SegmentSpacing = 68;
			info.Spinners.SpaceFromBase = 35;
			info.Spinners.Icon.Sprite = "Icon_FireSpinner";

			info.GhostBlocks.Sprite = "Ghostblock_Anders";
			info.GhostBlocks.Shift = new Vector2(0, -15);
			info.GhostBlocks.Icon.Sprite = "Ghostblock_Anders";
			info.GhostBlocks.Icon.Size = new Vector2(40, -1);

			info.MovingBlocks.Group.Add(new PieceQuad(210, "Movingblock_Anders_190", -1, 1, 3, false, 190 - 21, false));
			info.MovingBlocks.Group.Add(new PieceQuad(155, "Movingblock_Anders_135", -1, 1, 3, false, 135 - 15.5f, false));
			info.MovingBlocks.Group.Add(new PieceQuad(100, "Movingblock_Anders_80", -1, 1, 3, false, 80 - 5, false));
			info.MovingBlocks.Group.Add(new PieceQuad(60, "Movingblock_Anders_40", -1, 1, 3, false, 40 - 1, false));
			info.MovingBlocks.Icon.Sprite = "MovingBlock_Anders_40";
			info.MovingBlocks.Icon.Size = new Vector2(40, -1);
			info.MovingBlocks.Icon_Big = new SpriteInfo(null);
			info.MovingBlocks.Icon_Big.Sprite = "MovingBlock_Anders_135";
			info.MovingBlocks.Icon_Big.Size = new Vector2(40, -1);

			info.Elevators.Group.Add(new PieceQuad(40, "Elevator_Anders", -1, 1, 1, false, -1.5f, true));
			info.Elevators.Group.Add(new PieceQuad(80, "Elevator_Anders", -1, 1, 1, false, -1.5f, true));
			info.Elevators.Group.Add(new PieceQuad(135, "Elevator_Anders", -1, 1, 1, false, -1.5f, true));
			info.Elevators.Group.Add(new PieceQuad(190, "Elevator_Anders", -1, 1, 1, false, -1.5f, true));
			info.Elevators.Icon.Sprite = "Elevator_Anders_80";

			info.Pendulums.Group.Add(new PieceQuad(40, "Elevator_Anders", -1, 1, 1, false, -1.5f, true));
			info.Pendulums.Group.Add(new PieceQuad(80, "Elevator_Anders", -1, 1, 1, false, -1.5f, true));
			info.Pendulums.Group.Add(new PieceQuad(135, "Elevator_Anders", -1, 1, 1, false, -1.5f, true));
			info.Pendulums.Group.Add(new PieceQuad(190, "Elevator_Anders", -1, 1, 1, false, -1.5f, true));
			info.Pendulums.UseModifiedChain = false;
			info.Pendulums.Icon.Sprite = "Pendulum_Icon_Anders";

			sprite_anim("Serpent_Castle", "Serpent_Castle", 1, 2, 8);
			info.Serpents.Serpent.Sprite = "Serpent_Castle";
			sprite_anim("Serpent_Fish_Castle", "Serpent_Fish_Castle", 1, 2, 5);
			info.Serpents.Fish.Sprite = "Serpent_Fish_Castle";
			info.Serpents.Fish.Size = new Vector2(60, -1);
			info.Serpents.Fish.Offset = new Vector2(55, 0);
			info.Serpents.Icon.Sprite = "SerpentHead_Castle_1";

			info.Spikes.Spike.Sprite = "spike_Anders";
			info.Spikes.Spike.Size = new Vector2(52, 75);
			info.Spikes.Spike.Offset = new Vector2(0, 1);
			info.Spikes.Spike.RelativeOffset = true;
			info.Spikes.Base.Sprite = "spike_base_Anders";
			info.Spikes.Base.Size = new Vector2(58, 33);
			info.Spikes.PeakHeight = .335f;
			info.Spikes.Icon.Sprite = "SpikeIcon";
			info.Spikes.Icon.Offset = new Vector2(0, 5.5f);
			info.Spikes.Icon.Size = new Vector2(29, -1);

			info.Boulders.Ball.Sprite = "Floater_Spikey_Anders_v1";
			info.Boulders.Ball.Size = new Vector2(185, -1);
			info.Boulders.Radius = 135;
			info.Boulders.Chain.Sprite = "floater_chain_Anders";
			info.Boulders.Chain.Width = 55;
			info.Boulders.Chain.RepeatWidth = 85;
			info.Boulders.Chain.Wrap = true;
			info.Boulders.Icon.Sprite = "Floater_Spikey_Anders_v1";

			info.SpikeyGuys.Ball.Sprite = "floater_Spikey_Anders_v2";
			info.SpikeyGuys.Ball.Size = new Vector2(190, -1);
			info.SpikeyGuys.Ball.Offset = new Vector2(0, 0);
			info.SpikeyGuys.Base.Sprite = null;
			info.SpikeyGuys.Rotate = false;
			info.SpikeyGuys.RotateSpeed = .1f;
			info.SpikeyGuys.Radius = 130;
			info.SpikeyGuys.RotateOffset = -1.57f;
			info.SpikeyGuys.Chain.Sprite = "floater_chain_Anders";
			info.SpikeyGuys.Chain.Width = 55;
			info.SpikeyGuys.Chain.RepeatWidth = 85;
			info.SpikeyGuys.Chain.Wrap = true;
			info.SpikeyGuys.Icon.Sprite = "floater_Spikey_Anders_v2";
			info.SpikeyGuys.Icon.Offset = new Vector2(0, -16);

			info.SpikeyLines.Ball.Sprite = "Floater_Spikey_Anders_v2";
			info.SpikeyLines.Ball.Size = new Vector2(150, -1);
			info.SpikeyLines.Ball.Offset = new Vector2(-8, 12);
			info.SpikeyLines.Radius = 100;
			info.SpikeyLines.Rotate = true;
			info.SpikeyLines.RotateSpeed = .05f;
			info.SpikeyLines.Icon.Sprite = "Floater_Spikey_Anders_v2";

			sprite_anim("Blob_Anders", "Blob_Anders", 1, 2, 5);
			info.Blobs.Body.Sprite = "Blob_Anders";
			info.Blobs.Body.Size = new Vector2(137, -1);
			info.Blobs.Body.Offset = new Vector2(20, 20);
			info.Blobs.GooSprite = "BlobGoo";
			info.Blobs.Icon.Sprite = "Blob_Anders";

			info.Clouds.Sprite.Sprite = "Cloud_Anders";
			info.Clouds.Icon.Sprite = "Cloud_Anders";

			info.Fireballs.Sprite.ColorMatrix = ColorHelper.HsvTransform(1, 1, 355);
			info.Fireballs.Icon.Sprite = "Icon_Fireball";

			info.Coins.Sprite.Sprite = "CoinBlue";
			info.Coins.Sprite.Size = new Vector2(68, -1);
			info.Coins.ShowCoin = true;
			info.Coins.ShowEffect = true;
			info.Coins.ShowText = true;

			info.AllowLava = false;
			info.ObstacleCutoff = 70;
		}
    }
}
