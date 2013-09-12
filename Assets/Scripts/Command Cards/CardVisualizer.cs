using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardVisualizer : MonoBehaviour {
	
	public GameObject cardUI;
	public List<CardSlot> cardSlots;
	public List<CardSlot> handSlots;
	public List<CommandCard> commandCardPrefabs;
	public List<GameObject> programmedCards = new List<GameObject>();
	public List<GameObject> currentHandCards = new List<GameObject>();
	
	public void SetActiveCards(Robot.Command[] commands) {
		for (int i=0; i<commands.Length; ++i) {
			var newCard = CreateCommandCardForCommand(commands[i]);
			if (newCard) {
				newCard.transform.parent = cardSlots[i].transform;
				newCard.transform.localPosition = Vector3.zero;
				newCard.GetComponent<UITexture>().MakePixelPerfect();
				programmedCards.Add(newCard.gameObject);
			} else {
				programmedCards.Add(null);	
			}
			
			cardSlots[i].currentCard = newCard;
		}
	}
	
	public void SetHandCards(List<Robot.Command> commands) {
		for (int i=0; i<commands.Count; ++i) {
			var newCard = CreateCommandCardForCommand(commands[i]);
			if (newCard) {
				newCard.transform.parent = handSlots[i].transform;
				newCard.transform.localPosition = Vector3.zero;
				newCard.GetComponent<UITexture>().MakePixelPerfect();
				currentHandCards.Add(newCard.gameObject);
			} else {
				currentHandCards.Add(null);	
			}
			
			handSlots[i].currentCard = newCard;
		}		
	}
	
	void ClearCurrentCards() {
		foreach (GameObject gameObj in programmedCards) {
			Destroy(gameObj);	
		}
		
		programmedCards.Clear();
		
		ClearHandCards();
	}
	
	public void UpdateVisualizations(Robot.Command[] activeCommands, List<Robot.Command> handCommands) {
		ClearCurrentCards();
		SetActiveCards(activeCommands);
		SetHandCards(handCommands);
	}
	
	public void ClearHandCards() {
		foreach (GameObject gameObj in currentHandCards) {
			Destroy(gameObj);	
		}
		
		currentHandCards.Clear();				
	}
	
	
	public CommandCard CreateCommandCardForCommand(Robot.Command command) {
		foreach (CommandCard card in commandCardPrefabs) {
			if (card.command == command) {
				return (CommandCard)GameObject.Instantiate(card);	
			}
		}
		
		if (command != Robot.Command.None) {
			Debug.Log("No appropriate card found for command: " + command);
		}
		
		return null;
	}
	
	public void SetCardVisibility(bool visible) {
		cardUI.SetActive(visible);
	}
}
