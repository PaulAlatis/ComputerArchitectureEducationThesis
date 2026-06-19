using UnityEngine;
using TMPro;

public class NameInputUI : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInput;

    public async void ConfirmName()
    {
        string playerName = nameInput.text;

        if (string.IsNullOrWhiteSpace(playerName))
            return;

        await AuthenticationManager.Instance.SetUsername(playerName);

        gameObject.SetActive(false);

        Debug.Log("Current Name: " +
            AuthenticationManager.Instance.GetPlayerName());
    }
}