using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardVisualizer : MonoBehaviour {

	public List<GameObject> cardSlots;
	public List<CommandCard> commandCardPrefabs;
	List<GameObject> currentCards = new List<GameObject>();
	
	public void SetCards(List<Robot.Command> commands) {
		ClearCurrentCards();
		
		if (commands.Count != cardSlots.Count) {
			Debug.Log("Visualizer has wrong number of cards!");
			return;
		}
		
		for (int i=0; i<cardSlots.Count; ++i) {
			var newCard = CreateCommandCardForCommand(commands[i]);
			newCard.transform.parent = cardSlots[i].transform;
			newCard.transform.localPosition = Vector3.zero;
			currentCards.Add(newCard.gameObject);
		}
	}
	
	void ClearCurrentCards() {
		foreach (GameObject gameObj in currentCards) {
			Destroy(gameObj);	
		}
		
		currentCards.Clear();
	}
	
	CommandCard CreateCommandCardForCommand(Robot.Command command) {
		foreach (CommandCard card in commandCardPrefabs) {
			if (card.command == command) {
				return (CommandCard)GameObject.Instantiate(card);	
			}
		}
		
		Debug.Log("No appropriate card found!");
		return null;
	}
	
}
