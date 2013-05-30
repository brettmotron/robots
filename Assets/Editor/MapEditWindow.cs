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
		
		if (GUILayout.Button("Replace with conveyor")) {
			ReplaceSelectedTiles(TileType.Conveyor);	
		}
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
		
		foreach (Tile tile in selectedTiles) {
			if (newType == TileType.Conveyor) {
				var prefab = Resources.LoadAssetAtPath("Assets/Prefabs/Tiles/Conveyor Tile.prefab", typeof(Tile));
				var newTile = (ConveyorTile)GameObject.Instantiate(prefab, tile.transform.position, tile.transform.rotation);
				newTile.tileType = newType;
				newTile.x_pos = tile.x_pos;
				newTile.y_pos = tile.y_pos;
				newTile.transform.parent = tile.transform.parent;
				newTile.name = string.Format("Conveyor Tile ({0},{1})", newTile.x_pos, newTile.y_pos);
				newMap.SetTileForPosition(newTile, tile.x_pos, tile.y_pos);
				TileVisualizer.instance.SetVisualizationForTile(newTile);
				newSelection.Add(newTile.gameObject);
				newSelectedTiles.Add(newTile);
				DestroyImmediate(tile.gameObject);
			}
		}
		
		Selection.objects = newSelection.ToArray();
		selectedTiles = newSelectedTiles;
	}
}
