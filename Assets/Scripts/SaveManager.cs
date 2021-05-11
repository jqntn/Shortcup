using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public static bool isTuto1Done;
    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
        Load();
    }
#if UNITY_EDITOR
    void OnDestroy() { Save(); }
#endif
    void OnApplicationPause() { Save(); }
    void Load()
    {
        isTuto1Done = PlayerPrefs.GetInt("isTuto1Done", 0) == 1 ? true : false;
        AudioManager.instance.isMusicOn = PlayerPrefs.GetInt("isMusicOn", 1) == 1 ? true : false;
        AudioManager.instance.isSfxOn = PlayerPrefs.GetInt("isSfxOn", 1) == 1 ? true : false;
        LevelManager.instance.currentRestaurantId = PlayerPrefs.GetInt("currentRestaurantId", 0);
        for (int i = 0; i < LevelManager.instance.restaurants.Count; i++)
            for (int j = 0; j < LevelManager.instance.restaurants[i].levels.Count; j++)
            {
                LevelManager.instance.restaurants[i].levels[j].played = PlayerPrefs.GetInt(string.Format("{0}_{1}_played", i, j), 0);
                LevelManager.instance.restaurants[i].levels[j].stars = PlayerPrefs.GetInt(string.Format("{0}_{1}_stars", i, j), 0);
                LevelManager.instance.restaurants[i].levels[j].steps = PlayerPrefs.GetInt(string.Format("{0}_{1}_steps", i, j), 0);
            }
    }
    void Save()
    {
#if !UNITY_EDITOR
        PlayerPrefs.SetString("locale", LocalizationSettings.SelectedLocale.Identifier.Code);
#endif
        PlayerPrefs.SetInt("isTuto1Done", isTuto1Done ? 1 : 0);
        PlayerPrefs.SetInt("isMusicOn", AudioManager.instance.isMusicOn ? 1 : 0);
        PlayerPrefs.SetInt("isSfxOn", AudioManager.instance.isSfxOn ? 1 : 0);
        PlayerPrefs.SetInt("currentRestaurantId", LevelManager.instance.currentRestaurantId);
        for (int i = 0; i < LevelManager.instance.restaurants.Count; i++)
            for (int j = 0; j < LevelManager.instance.restaurants[i].levels.Count; j++)
            {
                PlayerPrefs.SetInt(string.Format("{0}_{1}_played", i, j), LevelManager.instance.restaurants[i].levels[j].played);
                PlayerPrefs.SetInt(string.Format("{0}_{1}_stars", i, j), LevelManager.instance.restaurants[i].levels[j].stars);
                PlayerPrefs.SetInt(string.Format("{0}_{1}_steps", i, j), LevelManager.instance.restaurants[i].levels[j].steps);
            }
    }
}