using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TileVisualizer : MonoBehaviour {

	public static TileVisualizer instance;
	public List<GameObject> visualizationPrefabs;
	
	
	void Awake() {
		instance = this;	
	}
	
	public void Setup() {
		Awake();
	}
	
	public void SetVisualizationForTile(Tile tile) {
		if (null != tile.visualization) {
			Debug.Log("Tile already has a visualization!", tile);
			return;
		}
		
		GameObject newVis = (GameObject)GameObject.Instantiate(visualizationPrefabs[(int)tile.tileType]);
		newVis.transform.parent = tile.transform;
		newVis.transform.localPosition = Vector3.zero;
		newVis.transform.localRotation = Quaternion.identity;
		newVis.transform.localScale = Vector3.one;
		tile.visualization = newVis;
	}
}
