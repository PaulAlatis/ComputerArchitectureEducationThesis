using UnityEngine;

public class PopupSpawner : MonoBehaviour
{
    public static PopupSpawner Instance;
    [SerializeField] GameObject scorePopupPrefab;
    [SerializeField] Canvas worldCanvas;

    void Awake()
    {
        Instance = this;
        Debug.Log("PopupSpawner ready: " + gameObject.name);
    }

    public void SpawnPopup(int decimalValue, string binaryString, Vector3 worldPosition)
    {
        Debug.Log("SpawnPopup called! decimal: " + decimalValue + " binary: " + binaryString);

        if (scorePopupPrefab == null) { Debug.LogError("scorePopupPrefab is NULL!"); return; }
        if (worldCanvas == null) { Debug.LogError("worldCanvas is NULL!"); return; }

        GameObject popup = Instantiate(scorePopupPrefab, worldCanvas.transform);
        Debug.Log("Popup instantiated: " + popup.name);

        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            worldCanvas.GetComponent<RectTransform>(),
            screenPos,
            worldCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 canvasPos
        );

        popup.GetComponent<RectTransform>().localPosition = canvasPos;
        popup.GetComponent<ScorePopup>().Setup(decimalValue, binaryString); // ✅ now matches
    }
}
