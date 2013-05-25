using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandDeck {
	
	List<Robot.Command> deckCards = new List<Robot.Command>();
	List<Robot.Command> discardPile = new List<Robot.Command>();
	
	public void CreateRandomDeck(int numCards) {
		deckCards.Clear();
		discardPile.Clear();
		
		for (int i=0; i<numCards; ++i) {
			deckCards.Add((Robot.Command)(Random.Range(1,(int)Robot.Command.MAX)));		
		}
	}
	
	public void Shuffle() {
		foreach (Robot.Command card in discardPile) {
			deckCards.Add(card);	
		}
		
		discardPile.Clear();
		
		int n = deckCards.Count;
		while (n > 1) {
			--n;
			int randomIndex = Random.Range(0, n + 1);
			Robot.Command tempCard = deckCards[randomIndex];
			deckCards[randomIndex] = deckCards[n];
			deckCards[n] = tempCard;
		}
	}
	
	public List<Robot.Command> DrawCards(int number) {
		if (deckCards.Count < number) {
			Debug.Log("Not enough cards!");
			number = deckCards.Count;
		}
		
		List<Robot.Command> cardsToDraw = new List<Robot.Command>();
		for (int i=0; i<number; ++i) {
			cardsToDraw.Add(deckCards[i]);	
			discardPile.Add(deckCards[i]);
		}
		deckCards.RemoveRange(0, number);
		return cardsToDraw;
	}
	
	public Robot.Command DrawCard() {
		if (deckCards.Count < 1) {
			Debug.Log("Not enough cards!");
			return Robot.Command.None;
		}
		
		
		Robot.Command cardToDraw = deckCards[0];
		discardPile.Add(cardToDraw);
		deckCards.RemoveAt(0);
		return cardToDraw;	
	}
}
