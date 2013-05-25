using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileSpawner : MonoBehaviour {
	
	public Transform gridParent;
	public List<Tile> tilePrefabs;
	public int gridSizeX;
	public int gridSizeY;
	public float tileSize;
	public float tileSeparation;
	
	int currentGridSizeX;
	int currentGridSizeY;
	Tile[,] grid;
		
	void Start() {
		currentGridSizeX = gridSizeX;
		currentGridSizeY = gridSizeY;
		GenerateGrid();	
	}
	
	// Update is called once per frame
	void Update () {
		if (gridSizeX < 1) {gridSizeX = 1;}
		if (gridSizeY < 1) {gridSizeY = 1;}
		
		
		if (currentGridSizeX != gridSizeX || currentGridSizeY != gridSizeY) {
			Debug.Log("Not equal grid sizes!");
			GenerateGrid();	
		}
	}
	
	void ClearGrid() {
		if (null == grid) {
			return;
		}
		
		for (int i = 0; i < currentGridSizeX; ++i) {
			for (int j = 0; j < currentGridSizeY; ++j) {	
				if (null != grid[i,j]) {
					Destroy(grid[i,j].gameObject);	
				}
			}
		} 
	}
	
	void GenerateGrid() {
		ClearGrid();
		
		int x = gridSizeX;
		int y = gridSizeY;
		
		grid = new Tile[x,y];
		
		for (int i = 0; i < x; ++i) {
			for (int j = 0; j < y; ++j) {	
				Vector3 position = new Vector3(i * (tileSize + tileSeparation), 0, j * (tileSize + tileSeparation));
				Tile newTile = (Tile)GameObject.Instantiate(tilePrefabs[Random.Range(0,tilePrefabs.Count)], position, Quaternion.identity);
				newTile.transform.parent = gridParent;
				newTile.name = string.Format("Tile ({0},{1})",i,j);
				grid[i,j] = newTile;
			}
		}
		
		currentGridSizeX = gridSizeX;
		currentGridSizeY = gridSizeY;
	}
}