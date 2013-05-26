using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridMaster : MonoBehaviour {
	
	static public GridMaster SharedInstance;
	public List<Tile> tilePrefabs;
	public Wall wallPrefab;
	public int gridSizeX;
	public int gridSizeY;
	public float tileSeparation;
	
	int currentGridSizeX;
	int currentGridSizeY;
	Tile[,] grid;
	
	Transform myTransform;
	
	void Awake() {
		SharedInstance = this;	
		myTransform = transform;
	}	
	
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
				Vector3 position = new Vector3(i, 0, j) * (Tile.tileSize + tileSeparation);
				Tile newTile = (Tile)GameObject.Instantiate(tilePrefabs[Random.Range(0,tilePrefabs.Count)], position, Quaternion.identity);
				newTile.x_pos = i;
				newTile.y_pos = j;
				newTile.transform.parent = myTransform;
				grid[i,j] = newTile;
			}
		}
		
		Tile tileToInform = null;
		
		//Horizontal walls
		for (int i = 0; i < x; ++i) {
			for (int j = 0; j <= y; ++j) {
				if (Random.Range(0, 1f) > 0.2f) {
					continue;
				}
				
				Vector3 position = new Vector3(i, 0, j - 0.5f) * (Tile.tileSize + tileSeparation);
				Wall newWall = (Wall)GameObject.Instantiate(wallPrefab, position, Quaternion.identity);
				newWall.transform.parent = myTransform;
				if (j < y) {
					tileToInform = grid[i,j];
					tileToInform.adjacentWalls[(int)Facing.South] = newWall;
					newWall.adjacentTiles.Add(tileToInform);
				}
				if (j > 0) {
					tileToInform = grid[i,j-1];
					tileToInform.adjacentWalls[(int)Facing.North] = newWall;
					newWall.adjacentTiles.Add(tileToInform);	
				}
			}
		}
		
		//Vertical walls
		for (int i = 0; i <= x; ++i) {
			for (int j = 0; j < y; ++j) {
				if (Random.Range(0, 1f) > 0.2f) {
					continue;
				}
				
				Vector3 position = new Vector3(i - 0.5f, 0, j) * (Tile.tileSize + tileSeparation);
				Wall newWall = (Wall)GameObject.Instantiate(wallPrefab, position, Quaternion.AngleAxis(90, Vector3.up));
				newWall.transform.parent = myTransform;
				if (i < x) {
					tileToInform = grid[i,j];
					tileToInform.adjacentWalls[(int)Facing.West] = newWall;
					newWall.adjacentTiles.Add(tileToInform);
				}
				if (i > 0) {
					tileToInform = grid[i-1,j];
					tileToInform.adjacentWalls[(int)Facing.East] = newWall;
					newWall.adjacentTiles.Add(tileToInform);	
				}
			}
		}
		
		currentGridSizeX = gridSizeX;
		currentGridSizeY = gridSizeY;
	}
	
	public Tile GetForwardTile(Tile currentTile, Facing direction, out Vector3 position) {
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
		
		if (currentTile == null) {
			Debug.LogError("I'm not on the grid!");
			position = Vector3.zero;
			return null;
		}
		
		if (currentTile.adjacentWalls[(int)direction] != null) {
			Debug.Log("There's a wall in the way!");
			position = currentTile.transform.position;
			return currentTile;
		}
		
		if (currentTile.x_pos + offsetX < 0 || currentTile.x_pos + offsetX >= currentGridSizeX || currentTile.y_pos + offsetY < 0 || currentTile.y_pos + offsetY >= currentGridSizeY) {
			position = currentTile.transform.position + new Vector3(offsetX, 0, offsetY) * (Tile.tileSize + tileSeparation);
			return null;
		}		
		
		Tile resultTile = grid[currentTile.x_pos + offsetX, currentTile.y_pos + offsetY];
		position = resultTile.transform.position;
		return resultTile;
	}
	
	public Tile GetBackwardTile(Tile currentTile, Facing direction, out Vector3 position) {
		return GetForwardTile(currentTile, Utils.UTurnFacing(direction), out position);	
		
	}
	
	public Tile GetStartingTile() {
		return grid[0,0];	
	}
	
	public IEnumerator ProcessBoardEffects(Robot robot) {
		Tile tile = robot.currentTile;
		yield return StartCoroutine(tile.ProcessEffect(robot));
	}
	
	public IEnumerator MoveRobotInDirection(Robot robot, Facing direction) {
		Vector3 pos;
		Tile newTile = GetForwardTile(robot.currentTile, direction, out pos);
		Vector3 dir = Vector3.Normalize(pos - robot.transform.position);
		
		while (Vector3.SqrMagnitude(pos - robot.transform.position) > 0.05*0.05) {
			robot.transform.position += dir * Time.deltaTime;
			yield return null;	
		}
		
		robot.currentTile = newTile;
		robot.transform.position = pos;
		
		if (null == newTile) {
			Debug.Log("I'm dead!");
			robot.isDead = true;
		}			
	}
	
	public IEnumerator RotateRobotLeft(Robot robot) {
		Facing newFacing = Utils.RotateLeftFacing(robot.facing);
		float targetAngle = 90 * (int)newFacing;
		while (Mathf.Abs(targetAngle - robot.transform.localEulerAngles.y) > 2) {
			robot.transform.Rotate(0, -Time.deltaTime * 45, 0);
			yield return null;
		}
		robot.facing = newFacing;
		robot.transform.localEulerAngles = targetAngle * Vector3.up;			
	}
	
	
	public IEnumerator RotateRobotRight(Robot robot) {
		Facing newFacing = Utils.RotateRightFacing(robot.facing);
		float targetAngle = 90 * (int)newFacing;
		while (Mathf.Abs(targetAngle - robot.transform.localEulerAngles.y) > 2) {
			robot.transform.Rotate(0, Time.deltaTime * 45, 0);
			yield return null;
		}
		robot.facing = newFacing;
		robot.transform.localEulerAngles = targetAngle * Vector3.up;			
	}	
}
