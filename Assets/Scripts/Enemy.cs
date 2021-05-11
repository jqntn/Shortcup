using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Variables
    [SerializeField] private List<GameObject> patrolPath;
    [SerializeField] private bool isLooping;
    private bool pingPongDirection = true;
    GameManager gm;
    int point;

    public GameObject crashVFX;
    #endregion

    private void Awake()
    {
        GameObject manager = GameObject.Find("GameManager");
        gm = manager?.GetComponent<GameManager>();

        gm.onStep += Step;
        gm.onLive += Onlive;

        if (!isLooping)
        {
            //Get starting tile
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
            if (hit)
            {
                GameObject tile = hit.collider.gameObject;
                point = patrolPath.IndexOf(tile) + 1;
            }
        }

        HighlightDangerTiles(true);
    }

    private void Step()
    {
        if (isLooping)
        {
            //Restart
            if (point == patrolPath.Count)
            {
                point = 0;
            }

            if(patrolPath[point].transform.position == gm.Path[gm.point].transform.position || (patrolPath[point].transform.position == gm.player.transform.position && gm.Path[gm.point].transform.position == transform.position))
            {
                Instantiate(crashVFX, patrolPath[point].transform.position, Quaternion.identity);
                gm.StartCoroutine(gm.Defeat());

                VictoryAnimation();
            }
            else
            {
                //Move
                StartCoroutine("Move");
            }
        }
        else
        {
            PingPongPath();
        }
    }

    private void PingPongPath()
    {

        if (pingPongDirection)
        {
            if (point < patrolPath.Count)
            {
                if (patrolPath[point].transform.position == gm.Path[gm.point].transform.position || (patrolPath[point].transform.position == gm.player.transform.position && gm.Path[gm.point].transform.position == transform.position))
                {
                    Instantiate(crashVFX, patrolPath[point].transform.position, Quaternion.identity);
                    gm.StartCoroutine(gm.Defeat());

                    VictoryAnimation();
                }
                else
                {
                    //Move
                    StartCoroutine("Move");
                }

            }
            else if (point >= patrolPath.Count)
            {
                point += -2;
                pingPongDirection = false;
                if (patrolPath[point].transform.position == gm.Path[gm.point].transform.position || (patrolPath[point].transform.position == gm.player.transform.position && gm.Path[gm.point].transform.position == transform.position))
                {
                    Instantiate(crashVFX, patrolPath[point].transform.position, Quaternion.identity);
                    gm.StartCoroutine(gm.Defeat());

                    VictoryAnimation();
                }
                else
                {
                    //Move
                    StartCoroutine("MoveBack");
                }

            }
        }
        else
        {
            if (point >= 0)
            {
                if (patrolPath[point].transform.position == gm.Path[gm.point].transform.position || (patrolPath[point].transform.position == gm.player.transform.position && gm.Path[gm.point].transform.position == transform.position))
                {
                    Instantiate(crashVFX, patrolPath[point].transform.position, Quaternion.identity);
                    gm.StartCoroutine(gm.Defeat());

                    VictoryAnimation();
                }
                else
                {
                    //Move
                    StartCoroutine("MoveBack");
                }

            }
            else if (point < 0)
            {
                point = 1;
                pingPongDirection = true;
                if (patrolPath[point].transform.position == gm.Path[gm.point].transform.position || (patrolPath[point].transform.position == gm.player.transform.position && gm.Path[gm.point].transform.position == transform.position))
                {
                    Instantiate(crashVFX, patrolPath[point].transform.position, Quaternion.identity);
                    gm.StartCoroutine(gm.Defeat());

                    VictoryAnimation();
                }
                else
                {
                    //Move
                    StartCoroutine("Move");
                }
            }
        }
    }

    IEnumerator Move()
    {
        Vector2 pos = patrolPath[point].transform.position;
        
        float initX = transform.position.x;
        float initY = transform.position.y;

        for (float i = 0; i < gm.stepLength; i += Time.deltaTime)
        {
            float percent = i / gm.stepLength;

            //New position
            float newX = Mathf.Lerp(initX, pos.x, percent);
            float newY = Mathf.Lerp(initY, pos.y, percent);

            #region Player Animation

            if (initX < newX)
            {
                GetComponent<Animator>().SetBool("right", true);
                GetComponent<Animator>().SetBool("left", false);
                GetComponent<Animator>().SetBool("back", false);
                GetComponent<Animator>().SetBool("face", false);
                GetComponent<Animator>().SetBool("idle", false);
            }
            else if (initX > newX)
            {
                GetComponent<Animator>().SetBool("right", false);
                GetComponent<Animator>().SetBool("left", true);
                GetComponent<Animator>().SetBool("back", false);
                GetComponent<Animator>().SetBool("face", false);
                GetComponent<Animator>().SetBool("idle", false);
            }
            else if (initY < newY)
            {
                GetComponent<Animator>().SetBool("right", false);
                GetComponent<Animator>().SetBool("left", false);
                GetComponent<Animator>().SetBool("back", true);
                GetComponent<Animator>().SetBool("face", false);
                GetComponent<Animator>().SetBool("idle", false);
            }
            else if (initY > newY)
            {
                GetComponent<Animator>().SetBool("right", false);
                GetComponent<Animator>().SetBool("left", false);
                GetComponent<Animator>().SetBool("back", false);
                GetComponent<Animator>().SetBool("face", true);
                GetComponent<Animator>().SetBool("idle", false);
            }

            #endregion

            transform.position = new Vector2(newX, newY);

            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.position = pos;

        point += 1;
    }

    IEnumerator MoveBack()
    {
        Vector2 pos = patrolPath[point].transform.position;

        float initX = transform.position.x;
        float initY = transform.position.y;

        for (float i = 0; i < gm.stepLength; i += Time.deltaTime)
        {
            float percent = i / gm.stepLength;

            //New position
            float newX = Mathf.Lerp(initX, pos.x, percent);
            float newY = Mathf.Lerp(initY, pos.y, percent);

            #region Player Animation

            if (initX < newX)
            {
                GetComponent<Animator>().SetBool("right", true);
                GetComponent<Animator>().SetBool("left", false);
                GetComponent<Animator>().SetBool("back", false);
                GetComponent<Animator>().SetBool("face", false);
                GetComponent<Animator>().SetBool("idle", false);
            }
            else if (initX > newX)
            {
                GetComponent<Animator>().SetBool("right", false);
                GetComponent<Animator>().SetBool("left", true);
                GetComponent<Animator>().SetBool("back", false);
                GetComponent<Animator>().SetBool("face", false);
                GetComponent<Animator>().SetBool("idle", false);
            }
            else if (initY < newY)
            {
                GetComponent<Animator>().SetBool("right", false);
                GetComponent<Animator>().SetBool("left", false);
                GetComponent<Animator>().SetBool("back", true);
                GetComponent<Animator>().SetBool("face", false);
                GetComponent<Animator>().SetBool("idle", false);
            }
            else if (initY > newY)
            {
                GetComponent<Animator>().SetBool("right", false);
                GetComponent<Animator>().SetBool("left", false);
                GetComponent<Animator>().SetBool("back", false);
                GetComponent<Animator>().SetBool("face", true);
                GetComponent<Animator>().SetBool("idle", false);
            }

            #endregion

            transform.position = new Vector2(newX, newY);

            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.position = pos;

        point += -1;
    }

    private void HighlightDangerTiles(bool b)
    {
        for (int i = 0; i < patrolPath.Count; i += 1)
        {
            Tile tile = patrolPath[i].GetComponent<Tile>();
            Vector2 nextTilePosition;
            if (i < patrolPath.Count - 1)
            {
                nextTilePosition = patrolPath[i + 1].transform.position;
            }
            else
            {
                if (isLooping)
                {
                    nextTilePosition = patrolPath[0].transform.position;
                }
                else
                {
                    nextTilePosition = patrolPath[patrolPath.Count - 2].transform.position;
                }

            }
            tile.SetDangerState(b, nextTilePosition);
        }
    }

    private void VictoryAnimation()
    {
        //Enemy Aniamtion
        GetComponent<Animator>().SetBool("right", false);
        GetComponent<Animator>().SetBool("left", false);
        GetComponent<Animator>().SetBool("back", false);
        GetComponent<Animator>().SetBool("face", false);
        GetComponent<Animator>().SetBool("idle", true);
        //Player Animation
        gm.player.GetComponent<Animator>().SetBool("right", false);
        gm.player.GetComponent<Animator>().SetBool("left", false);
        gm.player.GetComponent<Animator>().SetBool("back", false);
        gm.player.GetComponent<Animator>().SetBool("face", false);
        gm.player.GetComponent<Animator>().SetBool("idle", true);
    }

    private void Onlive()
    {
        HighlightDangerTiles(false);
    }
}
