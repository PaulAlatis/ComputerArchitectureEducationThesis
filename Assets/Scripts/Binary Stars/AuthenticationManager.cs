using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager Instance;
    public static bool IsReady { get; private set; }
    private async void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject); // ← also add this, prevents duplicate managers
        return;
    }

    await UnityServices.InitializeAsync();
    Debug.Log("Unity Services initialized.");

    if (!AuthenticationService.Instance.IsSignedIn)
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Signed in!");
    }

    IsReady = true; // ← moved here, only true when FULLY ready
    Debug.Log("Unity Services READY");
}

    public async Task<bool> SetUsername(string username)
{
    try
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(username);

        Debug.Log("Username Set: " + username);

        return true;
    }
    catch (System.Exception e)
    {
        Debug.LogError(e);

        return false;
    }
}

    public string GetPlayerName()
    {
        return AuthenticationService.Instance.PlayerName;
    }
}