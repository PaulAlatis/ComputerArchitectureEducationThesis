using UnityEngine;
using TMPro;

public class BinaryOrb : MonoBehaviour
{
    [Header("Binary Display")]
    [SerializeField] private TextMeshPro binaryText;
    [SerializeField] private int binaryLength = 4;

    private bool isDestroyed = false; // ✅ prevents double-hit

    private string targetBinary;
    private int currentIndex = 0;
    private ScoreKeeper scoreKeeper; // cached reference

    void Start()
    {
        scoreKeeper = FindAnyObjectByType<ScoreKeeper>();
        GenerateBinary();
    }

    void GenerateBinary()
    {
        targetBinary = "";
        targetBinary += Random.Range(1, 2).ToString(); // always start with 1
        for (int i = 1; i < binaryLength; i++)
            targetBinary += Random.Range(0, 2).ToString();

        binaryText.text = targetBinary;
        currentIndex = 0;
        Debug.Log("Target Binary: " + targetBinary);
    }

    public void CheckShot(int shotValue)
    {
        if (isDestroyed) return; // ✅ ignore hits after orb is done
        if (currentIndex >= targetBinary.Length) return; // ✅ bounds safety

        int expectedValue = int.Parse(targetBinary[currentIndex].ToString());

        if (shotValue == expectedValue)
        {
            currentIndex++;
            Debug.Log("Correct shot!");

            if (currentIndex >= targetBinary.Length)
            {
                isDestroyed = true; // ✅ set before anything else
                int decimalValue = System.Convert.ToInt32(targetBinary, 2);
                scoreKeeper.ModifyScore(decimalValue);
                PopupSpawner.Instance.SpawnPopup(decimalValue, targetBinary, transform.position);
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("Wrong shot! Sequence reset.");
            currentIndex = 0;
        }
    }
}