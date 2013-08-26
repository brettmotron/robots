using UnityEngine;
using System.Collections;

public class LaserWall : Wall {

	public override void Setup() {
		wallType = WallType.Laser;
		
		name = "Laser Wall";
	}
}
