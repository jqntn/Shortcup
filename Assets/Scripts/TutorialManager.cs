using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public GameObject TutoPanel;

    public GameObject[] popUps;

    public int popUpIndex;
    public bool isTuto;

    void Awake() { if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); } else Destroy(gameObject); }

    void Start()
    {
        TutoPanel.SetActive(false);
        isTuto = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1 && !SaveManager.isTuto1Done)
        {
            isTuto = true;
            TutoPanel.SetActive(true);
            popUpIndex = 0;
        }

        if (isTuto)
        {
            /*for (popUpIndex = 0; popUpIndex < popUps.Length;)
            {
                if (popUpIndex < popUps.Length)
                {
                    popUps[popUpIndex].SetActive(true);
                    if(popUpIndex > 0)
                    {
                        popUps[popUpIndex + 1].SetActive(false);
                    }
                }
            }*/

            #region Pop-Ups

            if (popUpIndex == 0)
            {
                // Welcome
                popUps[0].SetActive(true);
                popUps[1].SetActive(false);
                popUps[2].SetActive(false);
                popUps[3].SetActive(false);
                popUps[4].SetActive(false);
                popUps[5].SetActive(false);
                popUps[6].SetActive(false);
                SaveManager.isTuto1Done = true;
            }
            else if (popUpIndex == 1)
            {
                // Let's start
                popUps[0].SetActive(false);
                popUps[1].SetActive(true);
                popUps[2].SetActive(false);
                popUps[3].SetActive(false);
                popUps[4].SetActive(false);
                popUps[5].SetActive(false);
                popUps[6].SetActive(false);
            }
            else if (popUpIndex == 2)
            {
                // Animation + trace
                popUps[0].SetActive(false);
                popUps[1].SetActive(false);
                popUps[2].SetActive(true);
                popUps[3].SetActive(false);
                popUps[4].SetActive(false);
                popUps[5].SetActive(false);
                popUps[6].SetActive(false);
            }
            else if (popUpIndex == 3)
            {
                // Well
                popUps[0].SetActive(false);
                popUps[1].SetActive(false);
                popUps[2].SetActive(false);
                popUps[3].SetActive(true);
                popUps[4].SetActive(false);
                popUps[5].SetActive(false);
                popUps[6].SetActive(false);
            }
            else if (popUpIndex == 4)
            {
                // We used
                popUps[0].SetActive(false);
                popUps[1].SetActive(false);
                popUps[2].SetActive(false);
                popUps[3].SetActive(false);
                popUps[4].SetActive(true);
                popUps[5].SetActive(false);
                popUps[6].SetActive(false);
            }
            else if (popUpIndex == 5)
            {
                // You Don't
                popUps[0].SetActive(false);
                popUps[1].SetActive(false);
                popUps[2].SetActive(false);
                popUps[3].SetActive(false);
                popUps[4].SetActive(false);
                popUps[5].SetActive(true);
                popUps[6].SetActive(false);
            }
            else if (popUpIndex == 6)
            {
                // And there
                popUps[0].SetActive(false);
                popUps[1].SetActive(false);
                popUps[2].SetActive(false);
                popUps[3].SetActive(false);
                popUps[4].SetActive(false);
                popUps[5].SetActive(false);
                popUps[6].SetActive(true);
            }
            else if (popUpIndex == 7)
            {
                popUps[0].SetActive(false);
                popUps[1].SetActive(false);
                popUps[2].SetActive(false);
                popUps[3].SetActive(false);
                popUps[4].SetActive(false);
                popUps[5].SetActive(false);
                popUps[6].SetActive(false);

                TutoPanel.SetActive(false);
                isTuto = false;
            }

            #endregion

            if (Input.GetMouseButtonDown(0))
            {
                if ((0 <= popUpIndex && popUpIndex < 2) || (2 < popUpIndex && popUpIndex < 7))
                {
                    popUpIndex++;
                }
                /*else if (popUpIndex == 7)
                {
                    popUps[0].SetActive(false);
                    popUps[1].SetActive(false);
                    popUps[2].SetActive(false);
                    popUps[3].SetActive(false);
                    popUps[4].SetActive(false);
                    popUps[5].SetActive(false);
                    popUps[6].SetActive(false);

                    TutoPanel.SetActive(false);
                    isTuto = false;
                }*/
            }
        }

        
    }
}
