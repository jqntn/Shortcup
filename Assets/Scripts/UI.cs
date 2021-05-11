using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
class UI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public static UI instance;
    public bool isPaused;
    public bool isMenued;
    public bool isSettinged;
    public bool isCredited;
    public bool isLeveled;
    [Header("Layers")]
    public GameObject L_InGame;
    public GameObject L_Pause;
    public GameObject L_Menu;
    public GameObject L_Settings;
    public GameObject L_Credits;
    public GameObject L_Levels;
    [Header("Buttons")]
    public Button B_Pause;
    public Button B_Resume;
    public Button B_Retry;
    public Button B_Play;
    public Button B_Menu;
    public Button B_Settings_P;
    public Button B_Settings_M;
    public Button B_Credits;
    public Button B_Back_S;
    public Button B_Back_C;
    public Button B_Back_L;
    public Button B_Prev_L;
    public Button B_Next_L;
    public Button B_Achievements;
    public Button B_Leaderboard;
    [Header("Toggles")]
    public Toggle Toggle_Music;
    public Toggle Toggle_SFX;
    [Header("#")]
    public Transform P_Layout;
    public GameObject P_Section;
    public GameObject B_Level;
    public GameObject T_CS;
    public GameObject L_Settings_BG;
    public EventSystem ES;
    public Text T_Version;
    public float threshold;
    public float ease;
    Vector2 panPos;
    List<GameObject> generatedSections = new List<GameObject>();
    [Header("Animator")]
    public Animator transition;
    public float transitionTime = 0.8f;
    void Awake()
    {
        Application.targetFrameRate = 60;
        if (instance == null) { instance = this; transform.GetChild(0).gameObject.SetActive(true); DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }
    void Start()
    {
        B_Pause.onClick.AddListener(SwitchIsPaused);
        B_Resume.onClick.AddListener(SwitchIsPaused);
        B_Retry.onClick.AddListener(delegate { StartCoroutine(TransitionAnimationRetry()); });
        B_Play.onClick.AddListener(Play);
        B_Menu.onClick.AddListener(delegate { StartCoroutine(TransitionAnimationBackToMenu()); });
        B_Settings_P.onClick.AddListener(SwitchIsSettinged);
        B_Settings_M.onClick.AddListener(SwitchIsSettinged);
        B_Credits.onClick.AddListener(SwitchIsCredited);
        B_Back_S.onClick.AddListener(SwitchIsSettinged);
        B_Back_C.onClick.AddListener(SwitchIsCredited);
        B_Back_L.onClick.AddListener(SwitchIsLeveled);
        B_Prev_L.onClick.AddListener(PrevSection);
        B_Next_L.onClick.AddListener(NextSection);
        B_Achievements.onClick.AddListener(Social.ShowAchievementsUI);
        B_Leaderboard.onClick.AddListener(Social.ShowLeaderboardUI);
        Toggle_Music.onValueChanged.AddListener(delegate { AudioManager.instance.isMusicOn = Toggle_Music.isOn; });
        Toggle_SFX.onValueChanged.AddListener(delegate { AudioManager.instance.isSfxOn = Toggle_SFX.isOn; });
        Toggle_Music.isOn = AudioManager.instance.isMusicOn;
        Toggle_SFX.isOn = AudioManager.instance.isSfxOn;
        P_Layout.transform.position -= new Vector3(LevelManager.instance.currentRestaurantId * Screen.width, 0);
        panPos = P_Layout.transform.position;
        T_Version.text = "v" + Application.version;
        GenerateButtons();
        AudioManager.instance.PlayMusic("Music_Menu");
    }
    void Update()
    {
        Time.timeScale = isPaused ? 0 : 1;
        L_InGame.SetActive(!isPaused && !isMenued);
        L_Pause.SetActive(isPaused && !isSettinged && !isCredited && !isLeveled);
        L_Menu.SetActive(isMenued && !isSettinged && !isCredited && !isLeveled);
        L_Settings.SetActive(isSettinged);
        L_Credits.SetActive(isCredited);
        L_Levels.SetActive(isLeveled);
        B_Prev_L.interactable = LevelManager.instance.currentRestaurantId > 0 ? true : false;
        B_Next_L.interactable = LevelManager.instance.currentRestaurantId < LevelManager.instance.restaurants.Count - 1 ? true : false;
        L_Settings_BG.SetActive(!isPaused);
    }
    public void Retry() { SceneManager.LoadScene(string.Format("{0}_{1}", LevelManager.instance.currentRestaurantId + 1, LevelManager.instance.currentLevelId + 1)); isPaused = false; }
    public IEnumerator TransitionAnimationRetry()
    {
        ES.enabled = false;
        transition.SetTrigger("End");
        AudioManager.instance.PlaySfx("Sfx_Transition");
        yield return new WaitForSecondsRealtime(transitionTime);
        VictoryPanel.instance.isVictory = false;
        Retry();
        transition.SetTrigger("Start");
        ES.enabled = true;
    }
    void Play() { SwitchIsLeveled(); }
    public void Menu()
    {
        SceneManager.LoadScene("0");
        isMenued = true;
        isPaused = false;
        GenerateButtons();
        AudioManager.instance.StopMusic("Music_InGame");
        AudioManager.instance.PlayMusic("Music_Menu");
    }
    public IEnumerator TransitionAnimationBackToMenu()
    {
        UI.instance.ES.enabled = false;
        transition.SetTrigger("End");
        AudioManager.instance.PlaySfx("Sfx_Transition");
        yield return new WaitForSecondsRealtime(transitionTime);
        Menu();
        transition.SetTrigger("Start");
        UI.instance.ES.enabled = true;
    }
    void SwitchIsPaused() { isPaused ^= true; }
    void SwitchIsSettinged() { isSettinged ^= true; }
    void SwitchIsCredited() { isCredited ^= true; }
    void SwitchIsLeveled() { isLeveled ^= true; }
    void GenerateButtons()
    {
        foreach (var i in generatedSections) Destroy(i);
        for (int i = 0, x = 0; i < LevelManager.instance.restaurants.Count; i++, x += Screen.width)
        {
            var section = Instantiate(P_Section, P_Layout);
            section.name = section.name.Replace("(Clone)", i.ToString());
            section.transform.position += new Vector3(x, 0);
            var s = 0;
            foreach (var _ in LevelManager.instance.restaurants[i].levels) s += _.stars;
            LevelManager.instance.restaurants[i].stars = s;
            LevelManager.instance.restaurants[i].starsNeeded = LevelManager.instance.starsPerLevel * LevelManager.instance.restaurants[i].levels.Count;
            section.GetComponentInChildren<Text>().text = string.Format("{0}/{1} - {2} ({3}/{4})", i + 1, LevelManager.instance.restaurants.Count, LevelManager.instance.restaurants[i].name, LevelManager.instance.restaurants[i].stars, LevelManager.instance.restaurants[i].starsNeeded);
            generatedSections.Add(section);
            if (LevelManager.instance.restaurants[i].levels.Count > 0)
                for (int j = 0; j < LevelManager.instance.restaurants[i].levels.Count; j++)
                {
                    var button = Instantiate(B_Level, section.transform.GetChild(1).GetChild(0).GetChild(0));
                    button.name = button.name.Replace("(Clone)", i.ToString() + j.ToString());
                    var t = (j > 9) ? 1 : 0;
                    button.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        LevelManager.instance.StartCoroutine(LevelManager.instance.TransitionAnimationOpenLevel(
                            int.Parse(button.name[button.name.Length - t - 2].ToString()),
                            int.Parse(button.name.Substring(button.name.Length - t - 1))
                        ));
                    });
                    button.GetComponentInChildren<Text>().text = (j + 1).ToString();
                    if (LevelManager.instance.restaurants[i].levels[j].played > 0) button.GetComponent<Image>().color = new Color(.8f, .8f, .8f);
                    for (int _ = 3; _ < LevelManager.instance.restaurants[i].levels[j].stars + 3; _++) button.transform.GetChild(_).gameObject.SetActive(true);
                    section.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta += new Vector2(0, (
                        button.GetComponent<RectTransform>().sizeDelta.y +
                        section.GetComponentInChildren<GridLayoutGroup>().spacing.y / 2
                    ) / 4);
                }
            else Instantiate(T_CS, section.transform.GetChild(1));
        }
        PlayManager.UpdateLeaderboard();
    }
    void PrevSection()
    {
        LevelManager.instance.currentRestaurantId--;
        Vector2 newPos = panPos + new Vector2(Screen.width, 0);
        StartCoroutine(Swipe(P_Layout.transform.position, newPos));
        panPos = newPos;
    }
    void NextSection()
    {
        LevelManager.instance.currentRestaurantId++;
        Vector2 newPos = panPos - new Vector2(Screen.width, 0);
        StartCoroutine(Swipe(P_Layout.transform.position, newPos));
        panPos = newPos;
    }
    public void OnDrag(PointerEventData data)
    {
        if (isLeveled)
        {
            float difference = data.pressPosition.x - data.position.x;
            P_Layout.transform.position = panPos - new Vector2(difference, 0);
        }
    }
    public void OnEndDrag(PointerEventData data)
    {
        if (isLeveled)
        {
            float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
            if (Mathf.Abs(percentage) >= threshold)
            {
                Vector2 newPos = panPos;
                if (percentage > 0 && LevelManager.instance.currentRestaurantId < LevelManager.instance.restaurants.Count - 1)
                {
                    LevelManager.instance.currentRestaurantId++;
                    newPos -= new Vector2(Screen.width, 0);
                }
                else if (percentage < 0 && LevelManager.instance.currentRestaurantId > 0)
                {
                    LevelManager.instance.currentRestaurantId--;
                    newPos += new Vector2(Screen.width, 0);
                }
                StartCoroutine(Swipe(P_Layout.transform.position, newPos));
                panPos = newPos;
            }
            else StartCoroutine(Swipe(P_Layout.transform.position, panPos));
        }
    }
    IEnumerator Swipe(Vector2 start, Vector2 end)
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.unscaledDeltaTime / ease;
            P_Layout.transform.position = Vector2.Lerp(start, end, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
    }
    public static void SelectLocale(int i)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
        PlayerPrefs.SetString("locale", LocalizationSettings.SelectedLocale.Identifier.Code);
    }
    public void ClickSfx() { AudioManager.instance.PlaySfx("Sfx_Click"); }
}