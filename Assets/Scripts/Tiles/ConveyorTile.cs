using UnityEngine;
using System.Collections;

public class ConveyorTile : Tile {

	public Facing facing;
	
	void Awake() {
		tileType = TileType.Conveyor;	
	}
	
	void Start() {
		//Random Facing for time being
		facing = (Facing)Random.Range(0,4);
		transform.rotation = Utils.RotationForFacing(facing);
		//
		
		name = string.Format("Conveyor Tile ({0},{1})-{2}",x_pos,y_pos,facing);	
	}
	
	public override IEnumerator ProcessEffect(Robot robot) {
		Debug.Log("I'm a conveyor tile!", this);
		yield return StartCoroutine(GridMaster.SharedInstance.MoveRobotInDirection(robot, facing));
	}
	
}
