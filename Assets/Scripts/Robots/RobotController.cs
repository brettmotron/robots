using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {
	
	public Robot robotToControl;
	public CommandDeck commandDeck;
	public CardVisualizer visualizer;
	
	public List<Robot.Command> commandHand;
	public List<Robot.Command> activeCommands;
	
	void Start() {
		commandDeck = new CommandDeck();
		commandDeck.CreateRandomDeck(60);
		
		commandHand = new List<Robot.Command>();
		activeCommands = new List<Robot.Command>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.N)) {
			//Generate Command Hand
			DrawNewHand();
		}
		
		if (Input.GetKeyUp(KeyCode.Return)) {
			//Finalize active cards
			if (activeCommands.Count != 5) {
				Debug.Log("Incomplete command set.");
			}
			
			foreach (Robot.Command command in activeCommands) {
				robotToControl.QueueCommand(command);	
			}
		}
		
		if (Input.GetKeyUp(KeyCode.Alpha1)) {
			AddCardToActiveCards(0);	
		}
		if (Input.GetKeyUp(KeyCode.Alpha2)) {
			AddCardToActiveCards(1);	
		}
		if (Input.GetKeyUp(KeyCode.Alpha3)) {
			AddCardToActiveCards(2);	
		}
		if (Input.GetKeyUp(KeyCode.Alpha4)) {
			AddCardToActiveCards(3);	
		}
		if (Input.GetKeyUp(KeyCode.Alpha5)) {
			AddCardToActiveCards(4);	
		}
		if (Input.GetKeyUp(KeyCode.Alpha6)) {
			AddCardToActiveCards(5);	
		}
		if (Input.GetKeyUp(KeyCode.Alpha7)) {
			AddCardToActiveCards(6);	
		}
		if (Input.GetKeyUp(KeyCode.Alpha8)) {
			AddCardToActiveCards(7);	
		}
		if (Input.GetKeyUp(KeyCode.Alpha9)) {
			AddCardToActiveCards(8);	
		}
		if (Input.GetKeyUp(KeyCode.Alpha0)) {
			AddCardToActiveCards(9);	
		}		
	}
	
	void AddCardToActiveCards(int index) {
		if (activeCommands.Count >= 5) {
			Debug.Log("Too many commands!");
			return;
		}
		
		
		var temp = commandHand[index];
		activeCommands.Add(temp);
		commandHand[index] = Robot.Command.None;
		
		visualizer.UpdateVisualizations(activeCommands, commandHand);
	}
	
	void DrawNewHand() {
		commandDeck.Shuffle();
		commandHand = commandDeck.DrawCards(10);
		activeCommands.Clear();
		
		visualizer.UpdateVisualizations(activeCommands, commandHand);
	}
}
