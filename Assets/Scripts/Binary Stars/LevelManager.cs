using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;
using Unity.Services.Core;


public class LevelManager : MonoBehaviour
{
    [SerializeField] float sceneLoadDelay = 2f;
    //[SerializeField] TMP_InputField nameInput;
    ScoreKeeper scoreKeeper;

    void Awake()
{
    scoreKeeper = FindAnyObjectByType<ScoreKeeper>();
    _ = EnsureServicesReady(); // start warming up immediately
}

     public void LoadGame()
    {
        SceneManager.LoadScene("Game1");
        scoreKeeper.ResetScore();
    }

    //public async Task LoadGameOver()
    //{
       // int score = scoreKeeper.GetScore();

        //await FindAnyObjectByType<LeaderboardManager>()
        //.UploadScore(score);
        //StartCoroutine(WaitAndLoad("GameOver", sceneLoadDelay));
    //}

public async void TriggerGameOver(int score)
{
    Debug.Log("GAMEOVER REACHED. Score = " + score);

    await UploadScore(score);

    Debug.Log("UPLOAD FINISHED → loading scene");

    SceneManager.LoadScene("GameOver");
}


    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    public async void ConfirmName()
{
    TMP_InputField input = FindAnyObjectByType<TMP_InputField>();

    string username = input.text.Trim();

    bool success =
        await AuthenticationManager.Instance.SetUsername(username);

    if (success)
    {
        Debug.Log("Username accepted");

        SceneManager.LoadScene("MainMenu");
    }
    else
    {
        Debug.Log("Username unavailable");
    }
}

private async Task UploadScore(int score)
{
    try
    {
        // Guarantee initialization regardless of AuthenticationManager
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            Debug.Log("Services not initialized, initializing now...");
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Not signed in, signing in now...");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        // Extra safety delay to let SDK register all services
        await Task.Delay(500);
        Debug.Log("Services state: " + UnityServices.State + 
          " | SignedIn: " + AuthenticationService.Instance.IsSignedIn +
          " | PlayerID: " + AuthenticationService.Instance.PlayerId);
        Debug.Log("Attempting leaderboard upload. Score: " + score);
        await LeaderboardsService.Instance.AddPlayerScoreAsync("highscores", score);
        Debug.Log("Upload successful!");
    }
    catch (System.Exception e)
    {
        Debug.LogError("Upload failed: " + e);
    }
}

private async Task EnsureServicesReady()
{
    try
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log("LevelManager: Services confirmed ready.");
    }
    catch (System.Exception e)
    {
        Debug.LogError("LevelManager: Services init failed: " + e);
    }
}

}
