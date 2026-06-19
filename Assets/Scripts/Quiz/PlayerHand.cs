using UnityEngine;
using System.Collections.Generic;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private Deck deck;

    [SerializeField] private Transform[] cardSlots;

    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private int startingHandSize = 2;

    private List<Card> cardsInHand = new List<Card>();


    private void Start()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            DrawNextCard();
        }
    }

    public void DrawNextCard()
    {
        if (cardSlots == null || cardsInHand.Count >= cardSlots.Length)
        {
            Debug.LogWarning("No available card slots or hand is full.");
            return;
        }

        CardData cardData = deck.DrawCard();

        if (cardData == null)
        {
            Debug.LogWarning("No more cards to draw from the deck.");
            return;
        }
        
        int slotIndex = cardsInHand.Count;
        GameObject newCard = Instantiate(cardPrefab, cardSlots[slotIndex].position, Quaternion.identity);
        Card cardComponent = newCard.GetComponent<Card>();
        cardComponent.LoadCardData(cardData);
        cardsInHand.Add(cardComponent);
        cardsInHand[slotIndex].transform.SetParent(cardSlots[slotIndex]);
    }



    
}
