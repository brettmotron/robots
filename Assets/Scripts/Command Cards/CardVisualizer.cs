using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardVisualizer : MonoBehaviour {

	public List<GameObject> cardSlots;
	public List<GameObject> handSlots;
	public List<CommandCard> commandCardPrefabs;
	public List<GameObject> currentCards = new List<GameObject>();
	public List<GameObject> currentHandCards = new List<GameObject>();
	
	public void SetActiveCards(List<Robot.Command> commands) {
		for (int i=0; i<commands.Count; ++i) {
			var newCard = CreateCommandCardForCommand(commands[i]);
			if (newCard) {
				newCard.transform.parent = cardSlots[i].transform;
				newCard.transform.localPosition = Vector3.zero;
				currentCards.Add(newCard.gameObject);
			} else {
				currentCards.Add(null);	
			}
			
		}
	}
	
	public void SetHandCards(List<Robot.Command> commands) {
		for (int i=0; i<commands.Count; ++i) {
			var newCard = CreateCommandCardForCommand(commands[i]);
			if (newCard) {
				newCard.transform.parent = handSlots[i].transform;
				newCard.transform.localPosition = Vector3.zero;
				currentHandCards.Add(newCard.gameObject);
			} else {
				currentHandCards.Add(null);	
			}
			
		}		
	}
	
	void ClearCurrentCards() {
		foreach (GameObject gameObj in currentCards) {
			Destroy(gameObj);	
		}
		
		currentCards.Clear();
		
		foreach (GameObject gameObj in currentHandCards) {
			Destroy(gameObj);	
		}
		
		currentHandCards.Clear();		
	}
	
	public void UpdateVisualizations(List<Robot.Command> activeCommands, List<Robot.Command> handCommands) {
		ClearCurrentCards();
		SetActiveCards(activeCommands);
		SetHandCards(handCommands);
	}
	
	
	public CommandCard CreateCommandCardForCommand(Robot.Command command) {
		foreach (CommandCard card in commandCardPrefabs) {
			if (card.command == command) {
				return (CommandCard)GameObject.Instantiate(card);	
			}
		}
		
		Debug.Log("No appropriate card found for command: " + command);
		return null;
	}
	
}
