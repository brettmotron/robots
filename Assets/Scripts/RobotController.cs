using UnityEngine;
using System.Collections;

public class RobotController : MonoBehaviour {
	
	public Robot robotToControl;
	public CommandDeck commandDeck;
	public CardVisualizer visualizer;
	
	void Start() {
		commandDeck = new CommandDeck();
		commandDeck.CreateRandomDeck(60);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.N)) {
			//Generate Command Hand
			commandDeck.Shuffle();
			var commandHand = commandDeck.DrawCards(5);
			foreach (Robot.Command command in commandHand) {
				robotToControl.QueueCommand(command);	
			}
			visualizer.SetCards(commandHand);
		}
	}
}
