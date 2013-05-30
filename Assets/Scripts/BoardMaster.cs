using UnityEngine;
using System.Collections;

public class BoardMaster : MonoBehaviour {

	static public BoardMaster SharedInstance;
	public Tile basicTilePrefab;
	public int gridSizeX;
	public int gridSizeY;
	public float tileSize;
	public float tileSeparation;

	Tile[,] grid;
	
	Transform myTransform;
	
	void Awake() {
		myTransform = transform;
	}
	
	public void Setup(int xSize, int ySize) {
		Awake();
		gridSizeX = xSize;
		gridSizeY = ySize;
		
		grid = new Tile[xSize,ySize];
		
		GenerateGrid();
	}
	
	void GenerateGrid() {
		for (int i = 0; i < gridSizeX; ++i) {
			for (int j = 0; j < gridSizeY; ++j) {	
				Vector3 position = new Vector3(i, 0, j) * (tileSize + tileSeparation);
				Tile newTile = (Tile)GameObject.Instantiate(basicTilePrefab, position, Quaternion.identity);
				newTile.x_pos = i;
				newTile.y_pos = j;
				newTile.transform.parent = myTransform;
				
				newTile.Setup();
				
				grid[i,j] = newTile;
				TileVisualizer.instance.SetVisualizationForTile(newTile);
			}
		}
	}
	
	public void SetTileForPosition(Tile tile, int xPos, int yPos) {
		tile.x_pos = xPos;
		tile.y_pos = yPos;
		grid[xPos,yPos] = tile;
	}
}
