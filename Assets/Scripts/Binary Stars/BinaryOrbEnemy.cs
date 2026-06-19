using UnityEngine;
using TMPro;

public class BinaryOrbEnemy : MonoBehaviour
{
    [Header("Binary Settings")]
    [SerializeField] private TextMeshPro binaryText; // Text shown above/on orb
    [SerializeField] private int binaryLength = 4;

    private string currentBinary;

    void Start()
    {
        GenerateRandomBinary();
    }

    public void GenerateRandomBinary()
    {
        currentBinary = "";

        for (int i = 0; i < binaryLength; i++)
        {
            int randomBit = Random.Range(0, 2); // 0 or 1
            currentBinary += randomBit.ToString();
        }

        binaryText.text = currentBinary;

        Debug.Log("Orb Binary: " + currentBinary);
    }

    public string GetBinary()
    {
        return currentBinary;
    }

    public int GetDecimalValue()
    {
        return System.Convert.ToInt32(currentBinary, 2);
    }
}
