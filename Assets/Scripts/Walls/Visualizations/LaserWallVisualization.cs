using UnityEngine;
using System.Collections;

public class LaserWallVisualization : MonoBehaviour {
	
	LineRenderer laserEffect;
	public GameObject emitPoint;
	
	void Start() {
		laserEffect = GetComponent<LineRenderer>();	
		laserEffect.useWorldSpace = true;
	}
	
	// Update is called once per frame
	void Update () {
		laserEffect.SetPosition(0, emitPoint.transform.position);
		
		RaycastHit hitInfo;
		if (Physics.Raycast(emitPoint.transform.position, emitPoint.transform.forward, out hitInfo)) {
			laserEffect.SetPosition(1, hitInfo.point);
		} else {
			laserEffect.SetPosition(1, emitPoint.transform.position + 100*emitPoint.transform.forward);	
		}
	}
}
