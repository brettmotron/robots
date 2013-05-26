using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	
	static public float tileSize = 1;
	
	public TileType tileType;
	
	public int x_pos;
	public int y_pos;
	
	public Wall[] adjacentWalls = new Wall[4];
	
	public GameObject visualization;
	
	void Start() {
		name = string.Format("Tile ({0},{1})",x_pos,y_pos);	
	}
	
	public virtual IEnumerator ProcessEffect(Robot robot) {
		//Debug.Log("I'm a basic tile and don't contribute much");
		yield return null;
	}
}

public enum TileType {
	Basic,
	Conveyor,
	RotationLeft,
	RotationRight,
	MAX
}
