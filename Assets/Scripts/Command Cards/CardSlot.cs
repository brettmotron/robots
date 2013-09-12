using UnityEngine;
using System.Collections;

public class CardSlot : MonoBehaviour {

	public int slotID;
	public CommandCard currentCard;
	
	bool shouldScale;
	UIButtonScale scaler;
	
	void OnClick() {
		if (RobotController.SharedInstance.selectedSlot >= 0) {
			RobotController.SharedInstance.MoveCommand(RobotController.SharedInstance.selectedSlot, slotID);	
		} else if (currentCard != null) {
			RobotController.SharedInstance.selectedSlot = slotID;	
		}
	}
	
	void Update() {
		if (RobotController.SharedInstance.selectedSlot >= 0 || currentCard != null) {
			shouldScale = true;	
		} else {
			shouldScale = false;
		}
		
		if (scaler.enabled != shouldScale) {
			scaler.enabled = shouldScale;
		}
	}
	
	void Start() {
		scaler = GetComponent<UIButtonScale>();	
	}
	
}
