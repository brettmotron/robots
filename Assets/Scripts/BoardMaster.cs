using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	
	int tileResponsesReceived;

    public TileGrid betterGrid;
	List<Robot> robots = new List<Robot>();
	
	Transform myTransform;
	
	void Awake() {
        SharedInstance = this;
		myTransform = transform;
	}
	
	public void Setup(int xSize, int ySize) {
		Awake();
		
		gridSizeX = xSize;
		gridSizeY = ySize;

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
		
		
        Tile resultTile = betterGrid.column[newX].row[newY];
		return resultTile;
	}

    public Tile GetStartingTile()
    {
    	List<StartingTile> startingList = new List<StartingTile>();
		Tile tileToCheck;
		for (int x=0; x<gridSizeX; ++x) {
			for (int y=0; y<gridSizeY; ++y) {
				tileToCheck = betterGrid.column[x].row[y];
				if (tileToCheck.tileType == TileType.Starting) {
					//Debug.Log("Adding Tile to Starting List: " + tileToCheck.name);
					startingList.Add(tileToCheck as StartingTile);
				}
			}
		}
		
		if (startingList.Count <= 0) {
			Debug.Log("This map has no starting tiles!");
			return null;
		}
		
		return startingList[Random.Range(0, startingList.Count)];
    }

    public void ReceiveTileResponse() {
		tileResponsesReceived++;	
	}
	
	
	public IEnumerator ProcessTurn(List<Robot> robotList) {
		robots = robotList;
		
		Debug.Log("Processing Turn.");
		for(int phase = 1; phase <= 5; phase++) {
			yield return StartCoroutine(ProcessPhase(phase));	
		}
		Debug.Log("Done processing turn.");
		
		GameMaster.SharedInstance.TurnComplete();
	}
	
	
	IEnumerator ProcessPhase(int phaseNum) {
		Debug.Log("Processing Phase " + phaseNum);
		yield return StartCoroutine(ProcessRobots());	
		yield return StartCoroutine(ProcessBoardEffects(phaseNum));
		Debug.Log("Done processing Phase " + phaseNum);
	}

	
	IEnumerator ProcessRobots() {
		foreach (Robot robot in robots) {
			yield return StartCoroutine(robot.ProcessNextCommand());	
		}
	}
	

    IEnumerator ProcessBoardEffects(int phaseNum)
    {	
		Debug.Log("Beginning board effects.");
		yield return StartCoroutine(ProcessConveyors());
		yield return StartCoroutine(ProcessRotations());
		Debug.Log("Board effects complete.");
    }
	
	IEnumerator ProcessConveyors() {
		Debug.Log("Beginning to process conveyors.");
		
		tileResponsesReceived = 0;
		
		for (int i = 0; i < gridSizeX; ++i) {
			for (int j = 0; j < gridSizeY; ++j) {
				Tile tileToProcess = betterGrid.column[i].row[j];
				if (tileToProcess.tileType == TileType.Conveyor) {
					StartCoroutine(tileToProcess.ProcessEffect());
				} else {
					ReceiveTileResponse();
				}
			}
		}	
		
		while (tileResponsesReceived < gridSizeX*gridSizeY) {
			yield return null;	
		}
		
		Debug.Log("Conveyors processed!");
	}
	
	
	IEnumerator ProcessRotations() {
		Debug.Log("Beginning to process rotations.");
		
		tileResponsesReceived = 0;
		
		for (int i = 0; i < gridSizeX; ++i) {
			for (int j = 0; j < gridSizeY; ++j) {
				Tile tileToProcess = betterGrid.column[i].row[j];
				if (tileToProcess.tileType == TileType.RotationLeft || tileToProcess.tileType == TileType.RotationRight) {
					StartCoroutine(tileToProcess.ProcessEffect());
				} else {
					ReceiveTileResponse();
				}
			}
		}	
		
		while (tileResponsesReceived < gridSizeX*gridSizeY) {
			yield return null;	
		}
		
		Debug.Log("Rotations processed!");
	}	
	
	
    public Tile GetForwardTile(Tile currentTile, Facing direction, out Vector3 position)
    {
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

        if (currentTile.x_pos + offsetX < 0 || currentTile.x_pos + offsetX >= gridSizeX || currentTile.y_pos + offsetY < 0 || currentTile.y_pos + offsetY >= gridSizeY) {
            position = currentTile.transform.position + new Vector3(offsetX, 0, offsetY) * (Tile.tileSize + tileSeparation);
            return null;
        }

        //Tile resultTile = grid[currentTile.x_pos + offsetX, currentTile.y_pos + offsetY];
        Tile resultTile = betterGrid.column[currentTile.x_pos + offsetX].row[currentTile.y_pos + offsetY];
        position = resultTile.transform.position;
        return resultTile;
    }


    public Tile GetBackwardTile(Tile currentTile, Facing direction, out Vector3 position)
    {
        return GetForwardTile(currentTile, Utils.UTurnFacing(direction), out position);

    }


    public IEnumerator MoveRobotInDirection(Robot robot, Facing direction)
    {
        Vector3 pos;
        Tile newTile = GetForwardTile(robot.currentTile, direction, out pos);
        Vector3 dir = Vector3.Normalize(pos - robot.transform.position);

        while (Vector3.SqrMagnitude(pos - robot.transform.position) > 0.05 * 0.05) {
            robot.transform.position += dir * Time.deltaTime;
            yield return null;
        }
		
		robot.currentTile.currentRobot = null;
        robot.currentTile = newTile;
		if (robot.currentTile) {
			robot.currentTile.currentRobot = robot;
		}
		
        robot.transform.position = pos;

        if (null == newTile) {
            Debug.Log("I'm dead!");
            robot.isDead = true;
        }
    }


    public IEnumerator RotateRobotLeft(Robot robot)
    {
        Facing newFacing = Utils.RotateLeftFacing(robot.facing);
        float targetAngle = 90 * (int)newFacing;
        while (Mathf.Abs(targetAngle - robot.transform.localEulerAngles.y) > 2) {
            robot.transform.Rotate(0, -Time.deltaTime * 45, 0);
            yield return null;
        }
        robot.facing = newFacing;
        robot.transform.localEulerAngles = targetAngle * Vector3.up;
    }


    public IEnumerator RotateRobotRight(Robot robot)
    {
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
