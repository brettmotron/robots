using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Tile : MonoBehaviour {
	
	static public float tileSize = 1;
	
	public TileType tileType;
	
	public int x_pos;
	public int y_pos;
	
	public Facing facing;
	
    [SerializeField]
	public Wall[] adjacentWalls = new Wall[4];
	
	public GameObject visualization;
	
	public virtual void Setup() {
		
	}
	
	public virtual IEnumerator ProcessEffect(Robot robot) {
		//Debug.Log("I'm a basic tile and don't contribute much");
		yield return null;
	}
}

public enum TileType {
	Basic,
	Starting,
	Conveyor,
	RotationLeft,
	RotationRight,
	MAX
}
