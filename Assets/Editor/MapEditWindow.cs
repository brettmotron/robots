using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MapEditWindow : EditorWindow {
	
	int currentMapNumber;
	int xSize = 1;
	int ySize = 1;
	float tileSeparation = 0.1f;
	
	bool tileGrabbingActive;
	
	BoardMaster newMap;
	List<Tile> selectedTiles = new List<Tile>();
	
	[MenuItem("Robots/Map Edit Window")]
	static void ShowWindow() {
		EditorWindow.GetWindow(typeof(MapEditWindow));	
	}
	
	void Update() {
		if (tileGrabbingActive) {
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
		newMap = (BoardMaster)EditorGUILayout.ObjectField("Current Map", newMap, typeof(BoardMaster), true);
		if (GUILayout.Button(string.Format("Set TileGrabbing {0}", !tileGrabbingActive))) {
			tileGrabbingActive = !tileGrabbingActive;
			Debug.Log("Tile grabbing is now " + tileGrabbingActive);
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
			if (GUILayout.Button("Add Wall " + ((Facing)i).ToString())) {
				AddWallInDirection((Facing)i);	
			}
		}
		
		EditorGUILayout.EndVertical();
	}
	
	
	void GenerateNewMap(int xSize, int ySize) {
		if (xSize < 1 || ySize < 1) {
			Debug.Log("Map is too small.");
			return;
		}
		
		if (null == TileVisualizer.instance) {
			//GameObject.Find("TileVisualizer").GetComponent<TileVisualizer>().Setup();
		}
		
		++currentMapNumber;
		
		newMap = new GameObject().AddComponent<BoardMaster>();
		newMap.name = "New Map " + currentMapNumber;
		newMap.basicTilePrefab = (Tile)Resources.LoadAssetAtPath("Assets/Prefabs/Tiles/Basic Tile.prefab", typeof(Tile));
		newMap.tileSize = 1;
		newMap.tileSeparation = tileSeparation;
		newMap.Setup(xSize, ySize);
	}
	
	
	void ReplaceSelectedTiles(TileType newType) {
		var newSelection = new List<GameObject>();
		var newSelectedTiles = new List<Tile>();

		string prefabPath = "";
		Tile newTile = null;
		
		
		switch(newType) {
			case TileType.Basic:
				prefabPath = "Assets/Prefabs/Tiles/Basic Tile.prefab";
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
			
			newTile.Setup();
			
			newMap.SetTileForPosition(newTile, tile.x_pos, tile.y_pos);
			TileVisualizer.instance.SetVisualizationForTile(newTile);
			
			newSelection.Add(newTile.gameObject);
			newSelectedTiles.Add(newTile);			
			
			DestroyImmediate(tile.gameObject);
		}
		
		Selection.objects = newSelection.ToArray();
		selectedTiles = newSelectedTiles;
	}
	
	
	void RotateSelectionRight() {
		foreach (Tile tile in selectedTiles) {
			tile.facing = Utils.RotateRightFacing(tile.facing);
			tile.transform.localRotation = Utils.RotationForFacing(tile.facing);
		}
	}
	
	
	void RotateSelectionLeft() {
		foreach (Tile tile in selectedTiles) {
			tile.facing = Utils.RotateLeftFacing(tile.facing);
			tile.transform.localRotation = Utils.RotationForFacing(tile.facing);
		}
	}
	
	void AddWallInDirection(Facing direction) {
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
			
			var otherTile = newMap.GetTileInDirection(tile, direction);
			if (null != otherTile) {
				newWall.adjacentTiles.Add(otherTile);
				otherTile.adjacentWalls[(int)Utils.UTurnFacing(direction)] = newWall;
			}	
			
			TileVisualizer.instance.SetVisualizationForWall(newWall);
		}
	}
}
