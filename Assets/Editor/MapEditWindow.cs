using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MapEditWindow : EditorWindow {
	
	int currentMapNumber;
	int xSize = 1;
	int ySize = 1;
	float tileSeparation = 0.05f;
	
	BoardMaster currentMap;
	List<Tile> selectedTiles = new List<Tile>();
	
	[MenuItem("Robots/Map Edit Window")]
	static void ShowWindow() {
		EditorWindow.GetWindow(typeof(MapEditWindow));	
	}

    void GrabTilesFromSelection()
    {
        if (Selection.transforms.Length > 0) {
            var newSelection = new List<GameObject>();
            selectedTiles.Clear();
            foreach (Transform transform in Selection.transforms) {
                var temp = transform;
                while (temp != null) {
                    if (temp.GetComponent<Tile>() != null) {
                        newSelection.Add(temp.gameObject);
                        selectedTiles.Add(temp.GetComponent<Tile>());
                        break;
                    }
                    temp = temp.parent;
                }
            }
            Selection.objects = newSelection.ToArray();
        }
    }
	
	void OnGUI() {
		EditorGUILayout.BeginHorizontal();
		xSize = EditorGUILayout.IntField("X", xSize);
		ySize = EditorGUILayout.IntField("Y", ySize);
		EditorGUILayout.EndHorizontal();
		tileSeparation = EditorGUILayout.FloatField("Tile Separation", tileSeparation);
		if (GUILayout.Button("Generate Blank Map")) {
			GenerateNewMap(xSize, ySize);	
		}
		currentMap = (BoardMaster)EditorGUILayout.ObjectField("Current Map", currentMap, typeof(BoardMaster), true);
        if (GUILayout.Button("Save Map as Prefab")) {
            SaveCurrentMapAsPrefab();
        }
		
		EditorGUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Rotate Left")) {
			RotateSelectionLeft();
		}
		if (GUILayout.Button("Rotate Right")) {
			RotateSelectionRight();
		}
		
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.LabelField("Replace Tile");
		
		EditorGUILayout.BeginVertical();
		
		for (int i=0; i < (int)TileType.MAX; i++) {
			if (GUILayout.Button(((TileType)i).ToString())) {
				ReplaceSelectedTiles((TileType)i);	
			}
		}
		
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.LabelField("Walls");
		
		EditorGUILayout.BeginVertical();
		
		for (int i=0; i < 4; i++) {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Wall " + ((Facing)i).ToString())) {
				AddWallInDirection((Facing)i);	
			}
			if (GUILayout.Button("Rotate Wall " + ((Facing)i).ToString())) {
				RotateWallAtDirection((Facing)i);	
			}
			if (GUILayout.Button("Remove Wall " + ((Facing)i).ToString())) {
				RemoveWallInDirection((Facing)i);	
			}
			
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.LabelField("Replace Walls");
		
		EditorGUILayout.BeginVertical();		
		
		
		for (int i=0; i < 4; i++) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(((Facing)i).ToString());
			for (int j=0; j < (int)WallType.MAX; j++) {
				if (GUILayout.Button(((WallType)j).ToString())) {
					ChangeWallInDirection((Facing)i, (WallType)j);
				}
			}			
			EditorGUILayout.EndHorizontal();
		}		
		
		EditorGUILayout.EndVertical();
	}
	
	
	void GenerateNewMap(int xSize, int ySize) {
		if (xSize < 1 || ySize < 1) {
			Debug.Log("Map is too small.");
			return;
		}
		
		if (null == TileVisualizer.instance) {
			Debug.LogError("No TileVisualizer Found!");
			return;
		}
		
		++currentMapNumber;
		
		currentMap = new GameObject().AddComponent<BoardMaster>();
		currentMap.name = "New Map " + currentMapNumber;
		currentMap.basicTilePrefab = (Tile)Resources.LoadAssetAtPath("Assets/Prefabs/Tiles/Basic Tile.prefab", typeof(Tile));
		currentMap.tileSize = 1;
		currentMap.tileSeparation = tileSeparation;
		currentMap.Setup(xSize, ySize);
	}


    void SaveCurrentMapAsPrefab()
    {
        if (null == currentMap) {
            Debug.LogWarning("We don't have a map to save!");
            return;
        }

        PrefabUtility.CreatePrefab("Assets/Prefabs/Maps/" + currentMap.name + ".prefab", currentMap.gameObject, ReplacePrefabOptions.ReplaceNameBased);
        PrefabUtility.DisconnectPrefabInstance(currentMap);
		Debug.Log(currentMap.name + " saved!");
    }
	
	
	void ReplaceSelectedTiles(TileType newType) {
        if (null == currentMap) {
            Debug.Log("We don't have a map!");
            return;
        }

        GrabTilesFromSelection();

		var newSelection = new List<GameObject>();
		var newSelectedTiles = new List<Tile>();

		string prefabPath = "";
		Tile newTile = null;
		
		
		switch(newType) {
			case TileType.Basic:
				prefabPath = "Assets/Prefabs/Tiles/Basic Tile.prefab";
				break;
			case TileType.Starting:
				prefabPath = "Assets/Prefabs/Tiles/Starting Tile.prefab";
				break;			
			case TileType.Conveyor:
				prefabPath = "Assets/Prefabs/Tiles/Conveyor Tile.prefab";
				break;
			case TileType.RotationLeft:
				prefabPath = "Assets/Prefabs/Tiles/Rotation Tile (Left).prefab";
				break;
			case TileType.RotationRight:
				prefabPath = "Assets/Prefabs/Tiles/Rotation Tile (Right).prefab";
				break;
			default:
				Debug.Log("No path for replacement prefab!");
				break;
		}
		
		var prefab = Resources.LoadAssetAtPath(prefabPath, typeof(Tile));
					
		if (null == prefab) {
			Debug.Log("Replacement prefab not found!");	
			return;
		}
		
		foreach (Tile tile in selectedTiles) {
			
			newTile = (Tile)GameObject.Instantiate(prefab, tile.transform.position, tile.transform.rotation);
			
			if (null == newTile) {
				Debug.Log("Wasn't able to create replacement tile!");	
				return;
			}
			
			newTile.transform.parent = tile.transform.parent;
			
			newTile.x_pos = tile.x_pos;
			newTile.y_pos = tile.y_pos;
			newTile.facing = tile.facing;
			newTile.adjacentWalls = tile.adjacentWalls;
			
			newTile.Setup();
			
			currentMap.SetTileForPosition(newTile, tile.x_pos, tile.y_pos);
			TileVisualizer.instance.SetVisualizationForTile(newTile);
			
			newSelection.Add(newTile.gameObject);
			newSelectedTiles.Add(newTile);			
			
			DestroyImmediate(tile.gameObject);
		}
		
		Selection.objects = newSelection.ToArray();
		selectedTiles = newSelectedTiles;
	}
	
	
	void RotateSelectionRight() {
        GrabTilesFromSelection();

		foreach (Tile tile in selectedTiles) {
			tile.facing = Utils.RotateRightFacing(tile.facing);
			tile.transform.localRotation = Utils.RotationForFacing(tile.facing);
		}
	}
	
	
	void RotateSelectionLeft() {
        GrabTilesFromSelection();

		foreach (Tile tile in selectedTiles) {
			tile.facing = Utils.RotateLeftFacing(tile.facing);
			tile.transform.localRotation = Utils.RotationForFacing(tile.facing);
		}
	}
	
	void AddWallInDirection(Facing direction) {
        GrabTilesFromSelection();

		string wallPath = "Assets/Prefabs/Walls/Basic Wall.prefab";
		var prefab = Resources.LoadAssetAtPath(wallPath, typeof(Wall));
				
		if (null == prefab) {
			Debug.Log("Wall prefab not found!");	
			return;
		}
		
		foreach (Tile tile in selectedTiles) {
			if (null != tile.adjacentWalls[(int)direction]) {
				Debug.Log("Wall already exists in that direction!");
				continue;
			}
			
			var rotation = Utils.RotationForFacing(direction);
			var position = (new Vector3(tile.x_pos, 0, tile.y_pos) * (1 + tileSeparation)) + (0.5f * (1 + tileSeparation) * Utils.UnitOffsetForDirection(direction));
			
			Wall newWall = (Wall)GameObject.Instantiate(prefab, position, rotation);
			
			newWall.transform.parent = tile.transform.parent;
			newWall.facing = direction;
			
			newWall.Setup();
			
			newWall.adjacentTiles.Add(tile);
			tile.adjacentWalls[(int)direction] = newWall;
			
			var otherTile = currentMap.GetTileInDirection(tile, direction);
			if (null != otherTile) {
				newWall.adjacentTiles.Add(otherTile);
				otherTile.adjacentWalls[(int)Utils.UTurnFacing(direction)] = newWall;
			}	
			
			TileVisualizer.instance.SetVisualizationForWall(newWall);
		}
	}
	
	
	void RotateWallAtDirection(Facing direction) {
		GrabTilesFromSelection();
		
		foreach (Tile tile in selectedTiles) {
			Wall wallToRotate = tile.adjacentWalls[(int)direction];
			
			if (null == wallToRotate) {
				Debug.Log("Wall doesn't exist in that direction!");
				continue;
			}
			
			wallToRotate.transform.Rotate(0, 180, 0);
			wallToRotate.facing = Utils.UTurnFacing(wallToRotate.facing);
		}
	}
	
	
	void RemoveWallInDirection(Facing direction) {
        GrabTilesFromSelection();

		foreach (Tile tile in selectedTiles) {
			Wall wallToRemove = tile.adjacentWalls[(int)direction];
			
			if (null == wallToRemove) {
				Debug.Log("Wall doesn't exist in that direction!");
				continue;
			}
			
			foreach (Tile adjTile in wallToRemove.adjacentTiles) {
				if (adjTile.adjacentWalls[(int)direction] == wallToRemove) {
					adjTile.adjacentWalls[(int)direction] = null;
				}
				if (adjTile.adjacentWalls[(int)Utils.UTurnFacing(direction)] == wallToRemove) {
					adjTile.adjacentWalls[(int)Utils.UTurnFacing(direction)] = null;	
				}
			}
			
			DestroyImmediate(wallToRemove.gameObject);
		}			
	}
	
	
	void ChangeWallInDirection(Facing direction, WallType newType) {
		GrabTilesFromSelection();
		
		string prefabPath = "";
		Wall newWall = null;
		
		switch(newType) {
			case WallType.Basic:
				prefabPath = "Assets/Prefabs/Walls/Basic Wall.prefab";
				break;
			case WallType.Laser:
				prefabPath = "Assets/Prefabs/Walls/Laser Wall.prefab";
				break;			
			default:
				Debug.Log("No path for replacement prefab!");
				break;
		}
		
		var prefab = Resources.LoadAssetAtPath(prefabPath, typeof(Wall));
					
		if (null == prefab) {
			Debug.Log("Replacement prefab not found!");	
			return;
		}		
		
		
		foreach (Tile tile in selectedTiles) {
			Wall wallToChange = tile.adjacentWalls[(int)direction];
			
			if (null == wallToChange) {
				Debug.Log("Wall doesn't exist in that direction!");
				continue;
			}
			
			if (wallToChange.wallType == newType) {
				Debug.Log("Existing wall is already this type.");
				continue;
			}
			
			newWall = (Wall)GameObject.Instantiate(prefab, wallToChange.transform.position, wallToChange.transform.rotation);
			newWall.transform.parent = wallToChange.transform.parent;
			newWall.facing = wallToChange.facing;
			newWall.adjacentTiles = wallToChange.adjacentTiles;
			newWall.Setup();
			TileVisualizer.instance.SetVisualizationForWall(newWall);
			
			foreach (Tile adjTile in wallToChange.adjacentTiles) {
				if (adjTile.adjacentWalls[(int)direction] == wallToChange) {
					adjTile.adjacentWalls[(int)direction] = newWall;
				}
				if (adjTile.adjacentWalls[(int)Utils.UTurnFacing(direction)] == wallToChange) {
					adjTile.adjacentWalls[(int)Utils.UTurnFacing(direction)] = newWall;	
				}				
			}
			
			DestroyImmediate(wallToChange.gameObject);
		}
	}
}
