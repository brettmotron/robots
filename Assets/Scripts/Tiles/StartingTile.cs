using UnityEngine;
using System.Collections;

public class StartingTile : Tile {
	public override void Setup() {
		tileType = TileType.Starting;
		name = string.Format("Starting Tile ({0},{1})", x_pos, y_pos);
	}
}
