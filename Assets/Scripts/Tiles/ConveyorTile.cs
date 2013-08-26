using UnityEngine;
using System.Collections;

public class ConveyorTile : Tile {
	
	public override void Setup() {
		tileType = TileType.Conveyor;
		name = string.Format("Conveyor Tile ({0},{1}) ({2})", x_pos, y_pos, facing);
	}
	
	public override IEnumerator ProcessEffect() {
		if (currentRobot != null) {
			yield return StartCoroutine(BoardMaster.SharedInstance.MoveRobotInDirection(currentRobot, facing));
		} else {
			yield return null;
		}
		
		BoardMaster.SharedInstance.ReceiveTileResponse();
	}
	
}
