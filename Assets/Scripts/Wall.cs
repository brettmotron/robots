using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wall : MonoBehaviour {

	public List<Tile> adjacentTiles;
	
	void Start() {
		name = string.Format("Wall");		
	}
}
