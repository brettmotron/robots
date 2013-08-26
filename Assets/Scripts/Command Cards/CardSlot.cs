using UnityEngine;
using System.Collections;

public class CardSlot : MonoBehaviour {

	public int slotID;
	public CommandCard currentCard;
	
	
	void OnClick() {
		if (RobotController.SharedInstance.selectedSlot >= 0) {
			RobotController.SharedInstance.MoveCommand(RobotController.SharedInstance.selectedSlot, slotID);	
		} else if (currentCard != null) {
			RobotController.SharedInstance.selectedSlot = slotID;	
		}
	}
	
	void Start() {
		
	}	
	
}
