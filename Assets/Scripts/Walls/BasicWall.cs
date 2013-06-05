using UnityEngine;
using System.Collections;

public class BasicWall : Wall {

	public override void Setup() {
		wallType = WallType.Basic;
		
		name = "Basic Wall";
	}
	
}
