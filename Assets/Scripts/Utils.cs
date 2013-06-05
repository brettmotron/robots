using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour {
	
	static public Facing RotateRightFacing(Facing originalFacing) {
		return (Facing)(((int)originalFacing + 1) % 4);
	}
	
	static public Facing UTurnFacing(Facing originalFacing) {
		return (Facing)(((int)originalFacing + 2) % 4);	
	}
	
	static public Facing RotateLeftFacing(Facing originalFacing) {
		return (Facing)(((int)originalFacing + 3) % 4);
	}
	
	static public Quaternion RotationForFacing(Facing originalFacing) {
		return Quaternion.AngleAxis((int)originalFacing * 90, Vector3.up);	
	}
	
	static public Vector3 UnitOffsetForDirection(Facing direction) {
		switch (direction) {
			case Facing.North:
				return Vector3.forward;
			case Facing.East:
				return Vector3.right;
			case Facing.South:
				return Vector3.back;
			case Facing.West:
				return Vector3.left;
		}
		return Vector3.zero;
	}
		
}

public enum Facing {
	North,
	East,
	South,
	West
}

public enum Rotation {
	Left,
	Right
}
