using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
class PlayManager : MonoBehaviour
{
    public static PlayManager instance;
    public static bool authed = false;
    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) =>
        {
            switch (result)
            {
                case SignInStatus.Success:
                    authed = true;
                    DebugManager.instance.additionalInformation += "\nSuccessful SignIn";
                    break;
                default:
                    authed = false;
                    DebugManager.instance.additionalInformation += "\nSignIn Failed";
                    break;
            }
        });
    }
    public static void UpdateLeaderboard()
    {
        int totalStars = 0;
        foreach (var i in LevelManager.instance.restaurants) totalStars += i.stars;
        Social.ReportScore(totalStars, GPGSIds.leaderboard_best_rated_waiters, (bool success) => { });
    }
}