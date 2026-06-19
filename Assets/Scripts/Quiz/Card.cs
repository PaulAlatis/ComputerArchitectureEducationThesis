using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class Card : MonoBehaviour
{
    [SerializeField] private SpriteRenderer illustrationRender;

    [SerializeField] private TextMeshPro cardNameText;

    [SerializeField] private TextMeshPro descriptionText;

    [SerializeField] private TextMeshPro actionsText;

    private Vector3 originalScale;

    private Vector3 originalPosition;

    [SerializeField] private float hoverScale =  2f;

    [SerializeField] private float hoverOffset =  2f;

    private SortingGroup sortingGroup;

    private int originalSortingOrder;


    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    private void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
        originalSortingOrder = sortingGroup.sortingOrder;
        
        

    }

    public void LoadCardData(CardData cardData)
    {
        illustrationRender.sprite = cardData.illustration;
        cardNameText.text = cardData.cardName;
        descriptionText.text = cardData.description;
        actionsText.text = cardData.actionCost.ToString();
    }

    private void OnMouseEnter()
    {
        transform.localScale = originalScale * hoverScale; // Scale up by hoverScale
        transform.localPosition += new Vector3(0f, hoverOffset, 0f); // Move up by hoverOffset units
        sortingGroup.sortingOrder += 1; // Bring to front
    }

    private void OnMouseExit()
    {
        transform.localScale = originalScale; // Reset to original scale
        transform.localPosition = originalPosition; // Reset to original position
        sortingGroup.sortingOrder = originalSortingOrder; // Reset sorting order
    }

    private void OnMouseDrag()
    {
        gameObject.transform.position = GetMousePosition();
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    
}
