using UnityEngine;
using System.Collections;

public class ConfirmButton : MonoBehaviour {
	
	bool allowClick;
	public Color enabledColor;
	public Color disabledColor;

	void Update () {
		allowClick = RobotController.SharedInstance.CommandsAreValid();
		
		if (allowClick) {
			GetComponent<UITexture>().color = enabledColor;
		} else {
			GetComponent<UITexture>().color = disabledColor;
		}
	}
	
	void OnClick() {
		if (!allowClick) {
			return;	
		}
		
		RobotController.SharedInstance.ConfirmCommands();
	}
}
