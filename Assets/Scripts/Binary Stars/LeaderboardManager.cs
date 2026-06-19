using UnityEngine;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;

public class LeaderboardManager : MonoBehaviour
{
    public async Task UploadScore(int score)
    {
        Debug.Log("Uploading score: " + score);

        await LeaderboardsService.Instance.AddPlayerScoreAsync(
            "highscores",
            score
        );

        Debug.Log("Upload complete");
    }
}