using UnityEngine;
using System.Collections;

public class MapGrid : MonoBehaviour {

	public int gridSizeX;
	public int gridSizeY;
	public float tileSeparation;
	
	int currentGridSizeX;
	int currentGridSizeY;
	Tile[,] grid;
	
	Transform myTransform;

}
