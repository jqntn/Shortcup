using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    [Serializable]
    public class Restaurant
    {
        public string name;
        public int stars;
        public int starsNeeded;
        public List<Level> levels;
    }
    [Serializable]
    public class Level
    {
        public int played;
        public int stars;
        public int steps;
        public int minOne;
        public int minTwo;
        public int minThree;
        public int minScore;
    }
    public List<Restaurant> restaurants;
    public int currentRestaurantId;
    public int currentLevelId;
    public Restaurant currentRestaurantRef;
    public Level currentLevelRef;
    public int starsPerLevel;
    public Animator transition;
    void Awake() { if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); } else Destroy(gameObject); }
    void SetRefs()
    {
        LevelManager.instance.currentRestaurantRef = LevelManager.instance.restaurants[currentRestaurantId];
        LevelManager.instance.currentLevelRef = LevelManager.instance.restaurants[currentRestaurantId].levels[currentLevelId];
    }
    public void OpenLevel(int restaurant, int level)
    {
        UI.instance.isMenued = false;
        UI.instance.isLeveled = false;
        LevelManager.instance.currentRestaurantId = restaurant;
        LevelManager.instance.currentLevelId = level;
        SceneManager.LoadScene(string.Format("{0}_{1}", currentRestaurantId + 1, currentLevelId + 1));
        SetRefs();
        AudioManager.instance.StopMusic("Music_Menu");
        AudioManager.instance.PlayMusic("Music_InGame");
    }
    public IEnumerator TransitionAnimationOpenLevel(int restautant, int level)
    {
        UI.instance.ES.enabled = false;
        transition.SetTrigger("End");
        AudioManager.instance.PlaySfx("Sfx_Transition");
        yield return new WaitForSecondsRealtime(UI.instance.transitionTime);
        OpenLevel(restautant, level);
        transition.SetTrigger("Start");
        UI.instance.ES.enabled = true;
    }
    public void NextLevel()
    {
        if (currentLevelId > LevelManager.instance.currentRestaurantRef.levels.Count - 2)
        {
            if (LevelManager.instance.restaurants[currentRestaurantId + 1].levels.Count == 0)
            {
                UI.instance.Menu();
                SetRefs();
                return;
            }
            else
            {
                currentRestaurantId++;
                currentLevelId = 0;
            }
        }
        else currentLevelId++;
        SceneManager.LoadScene(string.Format("{0}_{1}", currentRestaurantId + 1, currentLevelId + 1));
        SetRefs();
    }
    public IEnumerator TransitionAnimationNextLevel()
    {
        UI.instance.ES.enabled = false;
        transition.SetTrigger("End");
        AudioManager.instance.PlaySfx("Sfx_Transition");
        yield return new WaitForSecondsRealtime(UI.instance.transitionTime);
        NextLevel();
        transition.SetTrigger("Start");
        UI.instance.ES.enabled = true;
    }
}