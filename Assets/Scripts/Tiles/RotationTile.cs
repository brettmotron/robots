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
	
	public override IEnumerator ProcessEffect(Robot robot) {
		Debug.Log("I'm a rotation tile!", this);
		if (rotation == Rotation.Right) {
			yield return StartCoroutine(GridMaster.SharedInstance.RotateRobotRight(robot));
		} else if (rotation == Rotation.Left) {
			yield return StartCoroutine(GridMaster.SharedInstance.RotateRobotLeft(robot));
		} else {
			Debug.Log("This rotation tile is not correct at all.", this);	
		}
	}
}
