using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private Camera cam;
    [SerializeField] private float endWaitTiime;

    private GameObject lastTileTouched;
    public GameObject player;

    [Range(0f, 10f)]
    public float stepLength;
    [HideInInspector] public List<GameObject> Path;
    [HideInInspector] public List<GameObject> Objectives;
    [HideInInspector] public int point;

    private int objectivesNb;
    private int currentObjectivesReached;
    [HideInInspector] public bool canStartSequence = false;

    [HideInInspector] public Vector2 possiblePosition1;
    [HideInInspector] public Vector2 possiblePosition2;
    [HideInInspector] public Vector2 possiblePosition3;
    [HideInInspector] public Vector2 possiblePosition4;

    private bool sequencIsRunning = false;
    private bool finishedMoovingPlayer = true;
    private bool asLost = false;

    [SerializeField] private bool hasMO;
    private int timer = 4;
    [SerializeField] public bool dir;
    [HideInInspector] public bool curDir;
    [HideInInspector] public bool lastTileWasRemoved;
    [SerializeField] private GameObject SplashR;
    [SerializeField] private GameObject SplashL;

    public event Action onStep;
    public event Action onTiltRight;
    public event Action onTiltLeft;
    public event Action onLive;
    #endregion
    private void Start()
    {
        //Get starting tile
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.zero);
        if (hit)
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            tile?.SetStartTile();
        }
        point = 1;
        objectivesNb = Objectives.Count;
        curDir = dir;
    }
    private void Update()
    {
        #region Click Manager
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit && !sequencIsRunning)
            {
                if (lastTileTouched != hit.collider.gameObject)
                {
                    Tile tile = hit.collider.GetComponent<Tile>();
                    if (tile.isStart == true)
                    {
                        StartSequence();
                    }
                    if (lastTileTouched == null) { }
                    if (Path.Count >= 2 && hit.collider.gameObject == Path[Path.Count - 2])
                    {
                        IClickable clickable = Path[Path.Count - 1].GetComponent<IClickable>();
                        clickable?.Click();
                        lastTileTouched = hit.collider.gameObject;
                    }
                    else
                    {
                        IClickable clickable = hit.collider.GetComponent<IClickable>();
                        clickable?.Click();
                        lastTileTouched = hit.collider.gameObject;
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            lastTileTouched = null;
        }
        #endregion
        //Start player movement sequence
        if (Input.GetKeyDown("space"))
        {
            StartSequence();
        }
    }
    #region Player movement
    IEnumerator MovePlayer(Vector2 pos)
    {
        //Player position
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;
        for (float i = 0; i < stepLength; i += Time.deltaTime)
        {
            float percent = i / stepLength;
            //New position
            float newX = Mathf.Lerp(playerX, pos.x, percent);
            float newY = Mathf.Lerp(playerY, pos.y, percent);
            #region Player Animation
            if (playerX < newX)
            {
                player.GetComponent<Animator>().SetBool("right", true);
                player.GetComponent<Animator>().SetBool("left", false);
                player.GetComponent<Animator>().SetBool("back", false);
                player.GetComponent<Animator>().SetBool("face", false);
                player.GetComponent<Animator>().SetBool("idle", false);
            }
            else if (playerX > newX)
            {
                player.GetComponent<Animator>().SetBool("right", false);
                player.GetComponent<Animator>().SetBool("left", true);
                player.GetComponent<Animator>().SetBool("back", false);
                player.GetComponent<Animator>().SetBool("face", false);
                player.GetComponent<Animator>().SetBool("idle", false);
            }
            else if (playerY < newY)
            {
                player.GetComponent<Animator>().SetBool("right", false);
                player.GetComponent<Animator>().SetBool("left", false);
                player.GetComponent<Animator>().SetBool("back", true);
                player.GetComponent<Animator>().SetBool("face", false);
                player.GetComponent<Animator>().SetBool("idle", false);
            }
            else if (playerY > newY)
            {
                player.GetComponent<Animator>().SetBool("right", false);
                player.GetComponent<Animator>().SetBool("left", false);
                player.GetComponent<Animator>().SetBool("back", false);
                player.GetComponent<Animator>().SetBool("face", true);
                player.GetComponent<Animator>().SetBool("idle", false);
            }
            #endregion
            player.transform.position = new Vector2(newX, newY);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        player.transform.position = pos;
        finishedMoovingPlayer = true;
    }
    #endregion
    #region Sequence
    public void StartSequence()
    {
        if (canStartSequence)
        {
            if (!sequencIsRunning)
            {
                sequencIsRunning = true;
                AudioManager.instance.PlaySfx("Sfx_Step");
                StartResetMO();
                StartCoroutine("PlaySequence");
            }
        }
    }
    IEnumerator PlaySequence()
    {
        int obCount = objectivesNb;
        for (;;)
        {
            if (point < Path.Count)
            {
                //Check if moved to tile
                if (finishedMoovingPlayer)
                {
                    finishedMoovingPlayer = false;
                    Step();
                    point += 1;
                    //is Objective check
                    Tile tile = Path[point - 1].GetComponent<Tile>();
                    if (tile.isObjective)
                    {
                        obCount += -1;
                        AudioManager.instance.PlaySfx("Sfx_Objectif");
                        //if all Objectives reached, stop
                        if (obCount < 1)
                        {
                            yield return new WaitForSeconds(endWaitTiime);
                            #region Player_Idle
                            player.GetComponent<Animator>().SetBool("right", false);
                            player.GetComponent<Animator>().SetBool("left", false);
                            player.GetComponent<Animator>().SetBool("back", false);
                            player.GetComponent<Animator>().SetBool("face", false);
                            player.GetComponent<Animator>().SetBool("idle", true);
                            #endregion
                            Victory();
                            StopCoroutine("PlaySequence");
                        }
                    }
                }
            }
            else
            {
                //Stop Sequence
                StopCoroutine("PlaySequence");
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    private void Step()
    {
        //Step event
        onStep?.Invoke();
        StepMO();
        if (!asLost)
        {
            //Player Step
            Vector2 pos = Path[point].transform.position;
            StartCoroutine(MovePlayer(pos));
        }
    }
    #endregion
    #region Tile List Management
    public void AddTile(GameObject Tile)
    {
        Path.Add(Tile);
        lastTileWasRemoved = false;
        StepMO();
    }
    public void RemoveTile(GameObject Tile)
    {
        Path.Remove(Tile);
        lastTileWasRemoved = true;
        StepBackMO();
    }
    public void AddObjective(GameObject Tile)
    {
        Objectives.Add(Tile);
    }
    public void CountSelectedObjectives(int i)
    {
        currentObjectivesReached += i;
        if (currentObjectivesReached == objectivesNb)
        {
            canStartSequence = true;

            #region tuto

            if (TutorialManager.instance.isTuto)
            {
                TutorialManager.instance.popUpIndex = 3;
            }

            #endregion

            player.GetComponent<Animator>().SetBool("right", false);
            player.GetComponent<Animator>().SetBool("left", false);
            player.GetComponent<Animator>().SetBool("back", false);
            player.GetComponent<Animator>().SetBool("face", false);
            player.GetComponent<Animator>().SetBool("idle", false);
            player.GetComponent<Animator>().SetBool("idleBump", true);

            Tile tile = Path[0].GetComponent<Tile>();
            if (tile.isStart)
            {
                SpriteRenderer sr = Path[0].GetComponent<SpriteRenderer>();
                sr.color = Color.green;
            }
        }
        else
        {
            if (canStartSequence)
            {
                canStartSequence = false;

                player.GetComponent<Animator>().SetBool("right", false);
                player.GetComponent<Animator>().SetBool("left", false);
                player.GetComponent<Animator>().SetBool("back", false);
                player.GetComponent<Animator>().SetBool("face", false);
                player.GetComponent<Animator>().SetBool("idle", true);
                player.GetComponent<Animator>().SetBool("idleBump", false);

                Tile tile = Path[0].GetComponent<Tile>();
                if (tile.isStart)
                {
                    SpriteRenderer sr = Path[0].GetComponent<SpriteRenderer>();
                    sr.color = Color.yellow;
                }
            }
        }
    }
    #endregion
    #region Win/Lose Conditions
    private void Victory()
    {
        Debug.Log("victory");
        AudioManager.instance.StopSfx("Sfx_Step");
        VictoryPanel.instance.isVictory = true;
    }
    public IEnumerator Defeat()
    {
        asLost = true;
        sequencIsRunning = false;
        StopCoroutine("PlaySequence");
        Debug.Log("defeat");
        AudioManager.instance.PlaySfx("Sfx_Crash");
        AudioManager.instance.StopSfx("Sfx_Step");
        Handheld.Vibrate();
        Social.ReportProgress(GPGSIds.achievement_broken_glasses, 100, (bool success) => { });
        yield return new WaitForSeconds(1.0f);
        UI.instance.Retry();
    }
    #endregion
    #region Moving Objects Manager
    private void StepMO()
    {
        if (hasMO)
        {
            if (timer <= 1)
            {
                if (curDir)
                {
                    onTiltRight?.Invoke();
                    curDir = false;
                    if (sequencIsRunning)
                    {
                        SplashR.SetActive(true);
                    }
                }
                else
                {
                    onTiltLeft?.Invoke();
                    curDir = true;
                    if (sequencIsRunning)
                    {
                        SplashL.SetActive(true);
                    }
                }
                if (sequencIsRunning)
                {
                    AudioManager.instance.PlaySfx("Sfx_MovingTable");
                }
                timer = 3;
            }
            else
            {
                timer += -1;
            }
        }
    }
    private void StepBackMO()
    {
        if (hasMO)
        {
            if (timer == 3)
            {
                if (curDir)
                {
                    onTiltRight?.Invoke();
                    curDir = false;
                }
                else
                {
                    onTiltLeft?.Invoke();
                    curDir = true;
                }
                timer = 1;
            }
            else
            {
                timer += 1;
            }
        }
    }
    private void StartResetMO()
    {
        if (hasMO)
        {
            timer = 3;
            onLive?.Invoke();
            if (curDir != dir)
            {
                if (curDir)
                {
                    curDir = false;
                }
                else
                {
                    curDir = true;
                }
            }
        }
    }
    #endregion
}