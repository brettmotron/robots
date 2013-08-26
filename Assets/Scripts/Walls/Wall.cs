using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class Wall : MonoBehaviour {
	
	public WallType wallType;
	public Facing facing;
	public GameObject visualization;
	
    [SerializeField]
	public List<Tile> adjacentTiles = new List<Tile>();
	
	public virtual void Setup() {
		
	}	
}

public enum WallType {
	Basic,
	Laser,
	MAX
}
