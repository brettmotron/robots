using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Wall : MonoBehaviour {
	
	public WallType wallType;
	public Facing facing;
	public GameObject visualization;
	
	public List<Tile> adjacentTiles = new List<Tile>();
	
	public virtual void Setup() {
		
	}	
}

public enum WallType {
	Basic,
	MAX
}
