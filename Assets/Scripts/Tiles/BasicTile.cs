using UnityEngine;
using System.Collections;

public class BasicTile : Tile {
	public override void Setup() {
		tileType = TileType.Basic;
		name = string.Format("Basic Tile ({0},{1})", x_pos, y_pos);
	}
}
