using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {
	
	public static GameMaster SharedInstance;
	List<Robot> robots = new List<Robot>();
	bool[] readyRobots;
	
	
	void Awake() {
		SharedInstance = this;	
	}
	
	
	void Start() {
		readyRobots = new bool[robots.Count];
	}
	
	
	public void RegisterRobot(Robot robot) {
		robots.Add(robot);	
	}
	
	
	public void SetRobotReady(Robot robot) {
		int index = robots.IndexOf(robot);
		
		if (index < 0) {
			Debug.Log("This robot isn't registered.");
		}
		
		Debug.Log("My robot is ready!");
		readyRobots[index] = true;
		
		for (int i=0; i<readyRobots.Length; i++) {
			if (readyRobots[i] == false) {
				Debug.Log("Not all robots are ready.");
				return;	
			}
		}
		
		Debug.Log("All robots are ready!");
		//At this point, all robots are ready
		StartCoroutine(BoardMaster.SharedInstance.ProcessTurn(robots));
	}
	
	
	public bool IsRobotReady(Robot robot) {
		int index = robots.IndexOf(robot);
		
		if (index < 0) {
			Debug.Log("This robot isn't registered.");
			return false;
		}
		
		return readyRobots[index];
	}
	
	
	public void TurnComplete() {
		readyRobots = new bool[robots.Count];	
		
		RobotController.SharedInstance.ResetRobot();
		if (RobotController.SharedInstance.robotToControl.isDead == false) {
			RobotController.SharedInstance.DrawNewHand();
		}
	}
}
