
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public partial class TileSets
    {
		static TileSet Load_Anders__Dungeon()
		{
			var t = GetOrMakeTileset("Anders__Dungeon");
			var info = t.MyTileSetInfo;

			t._Start();

			t.Name = "anders__dungeon";

			t.HasCeiling = false;

			t.Pillars.Add(new PieceQuad(60, "pillar_dungeon_60", -15, 15, 3));
			t.Pillars.Add(new PieceQuad(100, "pillar_dungeon_100", -15, 15, 3));
			t.Pillars.Add(new PieceQuad(150, "pillar_dungeon_150", -15, 15, 3));
			t.Pillars.Add(new PieceQuad(250, "pillar_dungeon_250", -15, 15, 3));
			t.Pillars.Add(new PieceQuad(350, "pillar_dungeon_300", -15, 15, 3));

			t.StartBlock.Add(new PieceQuad(900, "wall_dungeon", 625, 10, 2052));
			t.EndBlock.Add(new PieceQuad(900, "wall_dungeon", -10, -625, 2052));

			info.ShiftStartDoor = 0;
			info.ShiftStartBlock = new Vector2(100, 0);

			sprite_anim("door_dungeon", "door_dungeon", 1, 2, 2);
			info.Doors.Sprite.Sprite = "door_dungeon";
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
    }
}
