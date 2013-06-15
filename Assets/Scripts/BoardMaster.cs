using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileGrid
{
    [System.Serializable]
    public class TileList
    {
        public Tile[] row;
    }

    public TileList[] column;
}

public class BoardMaster : MonoBehaviour {

	static public BoardMaster SharedInstance;
	public Tile basicTilePrefab;
	public int gridSizeX;
	public int gridSizeY;
	public float tileSize;
	public float tileSeparation;

    [SerializeField]
	public Tile[,] grid;

    public TileGrid betterGrid;
	
	Transform myTransform;
	
	void Awake() {
		myTransform = transform;
	}
	
	public void Setup(int xSize, int ySize) {
		Awake();
		gridSizeX = xSize;
		gridSizeY = ySize;
		
		grid = new Tile[xSize,ySize];

        betterGrid = new TileGrid();
        betterGrid.column = new TileGrid.TileList[xSize];
        for (int i = 0; i < xSize; ++i) {
            betterGrid.column[i] = new TileGrid.TileList();
            betterGrid.column[i].row = new Tile[ySize];
        }
		
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

                betterGrid.column[i].row[j] = newTile;

				TileVisualizer.instance.SetVisualizationForTile(newTile);
			}
		}
	}
	
	public void SetTileForPosition(Tile tile, int xPos, int yPos) {
        if (null == betterGrid) {
            Debug.LogError("Grid is missing!");
            return;
        }

		tile.x_pos = xPos;
		tile.y_pos = yPos;
		//grid[xPos,yPos] = tile;
        betterGrid.column[xPos].row[yPos] = tile;
	}
	
	public Tile GetTileInDirection(Tile currentTile, Facing direction) {
		if (currentTile == null) {
			Debug.LogError("Current tile is not on the grid!");
			return null;
		}
		
		int offsetX = 0;
		int offsetY = 0;
		
		switch (direction) {
			case Facing.North:
				offsetY = 1;
				break;
			case Facing.East:
				offsetX = 1;
				break;
			case Facing.South:
				offsetY = -1;
				break;
			case Facing.West:
				offsetX = -1;
				break;
		}
		
		int newX = currentTile.x_pos + offsetX;
		int newY = currentTile.y_pos + offsetY;
		
		if (newX < 0 || newX >= gridSizeX || newY < 0 || newY >= gridSizeY) {
			return null;
		}		
		
		//Tile resultTile = grid[newX, newY];
        Tile resultTile = betterGrid.column[newX].row[newY];
		return resultTile;
	}	
}
