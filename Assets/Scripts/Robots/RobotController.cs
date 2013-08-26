using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {
	
	public static RobotController SharedInstance;
	public Robot robotToControl;
	public CommandDeck commandDeck;
	public CardVisualizer visualizer;
	
	public List<Robot.Command> commandHand;
	public Robot.Command[] activeCommands;
	
	internal int selectedSlot = -1;
	
	void Awake() {
		SharedInstance = this;	
	}
	
	void Start() {
		commandDeck = new CommandDeck();
		commandDeck.CreateRandomDeck(60);
		
		commandHand = new List<Robot.Command>();
		activeCommands = new Robot.Command[5];
		
		DrawNewHand();
	}
	
	
	public void ConfirmCommands() {
	
		for (int i=0; i<5; ++i) {
			robotToControl.QueueCommand(activeCommands[i]);	
		}
		
		GameMaster.SharedInstance.SetRobotReady(robotToControl);
		visualizer.SetCardVisibility(false);
		visualizer.ClearHandCards();		
	}
	
	
	public bool CommandsAreValid() {
		for (int i=0; i<5; ++i) {
			if (activeCommands[i] == Robot.Command.None) {
				return false;
			}
		}
		
		return true;
	}
	
	
	void ResetActiveCommands() {
		for (int i=0; i<5; ++i) {
			activeCommands[i] = Robot.Command.None;	
		}
	}
	
	
	public void ResetRobot() {
		ResetActiveCommands();
		robotToControl.ClearCommandList();		
	}
	
	public void DrawNewHand() {
		commandDeck.Shuffle();
		commandHand = commandDeck.DrawCards(10);
	
		visualizer.SetCardVisibility(true);
		visualizer.UpdateVisualizations(activeCommands, commandHand);
	}
	
	public void MoveCommand(int fromSlot, int toSlot) {
		Robot.Command temp;
		
		if (fromSlot < 5) {
			temp = activeCommands[fromSlot];
			if (toSlot < 5) {
				activeCommands[fromSlot] = activeCommands[toSlot];
				activeCommands[toSlot] = temp;
			} else {
				activeCommands[fromSlot] = commandHand[toSlot-5];
				commandHand[toSlot-5] = temp;
			}
		} else {
			temp = commandHand[fromSlot-5];
			if (toSlot < 5) {
				commandHand[fromSlot-5] = activeCommands[toSlot];
				activeCommands[toSlot] = temp;
			} else {
				commandHand[fromSlot-5] = commandHand[toSlot-5];
				commandHand[toSlot-5] = temp;
			}
		}
		
		selectedSlot = -1;
		
		visualizer.UpdateVisualizations(activeCommands, commandHand);
	}
}
