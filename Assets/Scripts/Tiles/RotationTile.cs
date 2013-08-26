using UnityEngine;
using System.Collections;

public class RotationTile : Tile {
	
	public Rotation rotation;
	
	public override void Setup ()
	{
		switch (rotation) {
			case Rotation.Left:
				tileType = TileType.RotationLeft;
				break;
			case Rotation.Right:
				tileType = TileType.RotationRight;
				break;
		}
		
		name = string.Format("Rotation Tile ({0},{1}) ({2})",x_pos,y_pos,rotation);	
	}
	
	public override IEnumerator ProcessEffect() {
		if (currentRobot == null) {
			yield return null;
		} else if (rotation == Rotation.Right) {
			yield return StartCoroutine(BoardMaster.SharedInstance.RotateRobotRight(currentRobot));
		} else if (rotation == Rotation.Left) {
			yield return StartCoroutine(BoardMaster.SharedInstance.RotateRobotLeft(currentRobot));
		} else {
			Debug.Log("This rotation tile is not correct at all.", this);	
		}
		
		BoardMaster.SharedInstance.ReceiveTileResponse();
	}
}
