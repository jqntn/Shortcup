using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class VictoryPanel : MonoBehaviour
{
    public static VictoryPanel instance;
    [Header("GameObjects/UI")]
    public GameObject Panel;
    public GameObject OneStars;
    public GameObject TwoStars;
    public GameObject TreeStars;
    public Text text;
    public Text lvlText;
    public bool isVictory;
    public int score = 0;
    [SerializeField]
    private int totalStars;
    public GameManager gameManagerScript;
    bool isA;
    void Awake() { if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); } else Destroy(gameObject); }
    void Start()
    {
        isVictory = false;
        Panel.SetActive(false);
        totalStars = 0;
        if (SceneManager.GetActiveScene().name != "0")
        {
            GameObject gameManagerObject = GameObject.Find("GameManager");
            gameManagerScript = gameManagerObject.GetComponent<GameManager>();
        }
    }
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "0" && gameManagerScript == null)
        {
            GameObject gameManagerObject = GameObject.Find("GameManager");
            gameManagerScript = gameManagerObject.GetComponent<GameManager>();
        }
        // Recover Score
        /*if (SceneManager.GetActiveScene().name != "0")*/
        // When isVictory is true
        if (isVictory)
        {
            score = gameManagerScript.Path.Count;
            text.text = score.ToString() + "/" + LevelManager.instance.currentLevelRef.minThree.ToString();
            Panel.SetActive(true);
            lvlText.text = (LevelManager.instance.currentLevelId + 1).ToString("00");
            if (score <= LevelManager.instance.currentLevelRef.minThree && score >= LevelManager.instance.currentLevelRef.minScore)
            {
                TreeStars.SetActive(true);
                TwoStars.SetActive(true);
                OneStars.SetActive(true);
                totalStars = 3;
            }
            else if (score > LevelManager.instance.currentLevelRef.minThree)
            {
                TreeStars.SetActive(false);
            }
            if (score <= LevelManager.instance.currentLevelRef.minTwo && score > LevelManager.instance.currentLevelRef.minThree)
            {
                TwoStars.SetActive(true);
                OneStars.SetActive(true);
                totalStars = 2;
            }
            else if (score > LevelManager.instance.currentLevelRef.minTwo)
            {
                TwoStars.SetActive(false);
            }
            if (score <= LevelManager.instance.currentLevelRef.minOne && score > LevelManager.instance.currentLevelRef.minTwo)
            {
                OneStars.SetActive(true);
                totalStars = 1;
            }
            else if (score > LevelManager.instance.currentLevelRef.minOne)
            {
                OneStars.SetActive(false);
                totalStars = 0;
            }
            if (!isA) SetA();
        }
        else
        {
            Panel.SetActive(false);
        }
    }
    public void NextLevel()
    {
        SetStarsNb();
        LevelManager.instance.StartCoroutine(LevelManager.instance.TransitionAnimationNextLevel());
    }
    public void BackToMenu()
    {
        SetStarsNb();
        UI.instance.StartCoroutine(UI.instance.TransitionAnimationBackToMenu());
    }
    public void Retry()
    {
        SetStarsNb();
        UI.instance.StartCoroutine(UI.instance.TransitionAnimationRetry());
    }
    void SetStarsNb()
    {
        isVictory = false;
        isA = false;
        if (LevelManager.instance.currentLevelRef.stars < totalStars) LevelManager.instance.currentLevelRef.stars = totalStars;
        if (LevelManager.instance.currentLevelRef.steps > score || LevelManager.instance.currentLevelRef.steps == 0) LevelManager.instance.currentLevelRef.steps = score;
    }
    void SetA()
    {
        isA = true;
        LevelManager.instance.currentLevelRef.played++;
        switch (LevelManager.instance.currentRestaurantId)
        {
            case 0:
                switch (LevelManager.instance.currentLevelId)
                {
                    case 0:
                        if (LevelManager.instance.currentLevelRef.played > 0)
                        {
                            Social.ReportProgress(GPGSIds.achievement_welcome_aboard, 100, (bool success) => { });
                            Debug.Log("achievement_welcome_aboard");
                        }
                        break;
                    case 4:
                        if (LevelManager.instance.currentLevelRef.played > 0)
                        {
                            Social.ReportProgress(GPGSIds.achievement_sliding_champion, 100, (bool success) => { });
                            Debug.Log("achievement_sliding_champion");
                        }
                        if (LevelManager.instance.currentLevelRef.steps >= 32)
                        {
                            Social.ReportProgress(GPGSIds.achievement_ulysse, 100, (bool success) => { });
                            Debug.Log("achievement_ulysse");
                        }
                        break;
                    case 9:
                        if (LevelManager.instance.currentLevelRef.steps <= 22 && LevelManager.instance.currentLevelRef.steps != 0)
                        {
                            Social.ReportProgress(GPGSIds.achievement_big_brain, 100, (bool success) => { });
                            Debug.Log("achievement_big_brain");
                        }
                        break;
                }
                if (LevelManager.instance.currentRestaurantRef.stars >= 12)
                {
                    Social.ReportProgress(GPGSIds.achievement_star_collector, 100, (bool success) => { });
                    Debug.Log("achievement_star_collector");
                }
                if (LevelManager.instance.currentRestaurantRef.stars >= LevelManager.instance.currentRestaurantRef.starsNeeded)
                {
                    Social.ReportProgress(GPGSIds.achievement_ashore, 100, (bool success) => { });
                    Debug.Log("achievement_ashore");
                }
                break;
        }
        if (LevelManager.instance.currentLevelRef.played >= 10)
        {
            Social.ReportProgress(GPGSIds.achievement_tryharder, 100, (bool success) => { });
            Debug.Log("achievement_tryharder");
        }
    }
}