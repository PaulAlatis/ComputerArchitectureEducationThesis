using System.Collections;
using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] float floatSpeed = 1.5f;
    [SerializeField] float fadeSpeed = 1.5f;
    [SerializeField] float lifetime = 1f;

    TextMeshProUGUI text;
    CanvasGroup canvasGroup;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        Debug.Log("ScorePopup text component: " + text); // should not be null
    }

    public void Setup(int decimalValue, string binaryString)
    {
        // Shows both binary and decimal e.g. "1010 = 10pts"
        text.text = $"{binaryString} = {decimalValue}pts";
        StartCoroutine(AnimatePopup());
    }

    IEnumerator AnimatePopup()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lifetime;

            // Float upward
            transform.position = startPos + Vector3.up * floatSpeed * elapsed;

            // Fade out in second half
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}