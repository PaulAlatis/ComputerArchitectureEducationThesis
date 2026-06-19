using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<CardData> drawPile = new List<CardData>();

    [SerializeField] private GameObject cardBack;

    private const float VERTICAL_SPACING = .1F;
    private void Start()
    {
        DeckDrawVisuals();
    }

    public CardData DrawCard()
    {
        if (drawPile.Count > 0)
        {
            //Draw and remove top card
            int topIndex = drawPile.Count - 1;
            CardData data = drawPile[topIndex];
            drawPile.RemoveAt(topIndex);
            return data;
        }
        return null; // No cards left to draw

        
    }

    private void DeckDrawVisuals()
    {
        for (int i = 0; i < drawPile.Count; i++)
        {
            GameObject newCardBack = Instantiate(cardBack, transform);
            newCardBack.transform.localPosition = new Vector3(0f, -i * VERTICAL_SPACING, 0f); // Slightly offset each card for visual stacking
        }
    }

    public void Shuffle()
    {
        for (int i = 0; i < drawPile.Count; i++)
        {
            //Fisher-Yates shuffle algorithm
            CardData temp = drawPile[i];
            int randomIndex = Random.Range(i, drawPile.Count);
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
    }
}
