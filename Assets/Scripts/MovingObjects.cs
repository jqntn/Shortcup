using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObjects : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject movOb;
    [SerializeField] private GameObject ghost;
    [SerializeField] private Color ghostColor;

    [SerializeField] private List<GameObject> PosR;
    [SerializeField] private List<GameObject> PosL;
    private GameManager gm;

    [SerializeField] private bool dir;
    private bool curDir;
    private bool isLive = false;
    private bool isBlockedByPlayer = false;
    private Vector2 posR;
    private Vector2 posL;

    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeMagnitude;
    [SerializeField] private float shakeRoughness;
    #endregion

    void Start()
    {
        GameObject manager = GameObject.Find("GameManager");
        gm = manager?.GetComponent<GameManager>();

        if (!dir)
        {
            for (int i = 0; i < PosR.Count; i += 1)
            {
                Tile tile = PosR[i].GetComponent<Tile>();
                tile.ChangeAccessibleState(false);
            }

            posR = movOb.transform.position;
            posL = new Vector2(movOb.transform.position.x - 1, movOb.transform.position.y);
        }
        else
        {
            for (int i = 0; i < PosL.Count; i += 1)
            {
                Tile tile = PosL[i].GetComponent<Tile>();
                tile.ChangeAccessibleState(false);
            }

            posL = movOb.transform.position;
            posR = new Vector2(movOb.transform.position.x + 1, movOb.transform.position.y);
        }

        curDir = dir;

        SpriteRenderer[] sr = ghost.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer SR in sr)
        {
            SR.color = ghostColor;
        }

        gm.onTiltRight += MoveRight;
        gm.onTiltLeft += MoveLeft;
        gm.onLive += ChangeLiveState;
    }

    #region Movement

    private void MoveRight()
    {
        if (curDir)
        {
            PlayerBlockCheck(PosR);
            SetAccessibleZone(true, PosL);
            if (!isBlockedByPlayer)
            {
                if (gm.Path.Count < 4 && dir != gm.dir)
                {

                }
                else
                {
                    if (isLive)
                    {
                        StartCoroutine(MoveObject(posR, PosR));
                    }
                    else
                    {
                        movOb.transform.position = posR;
                        SetAccessibleZone(false, PosR);
                        if (dir != curDir)
                        {
                            ghost.SetActive(false);
                        }
                        else
                        {
                            ghost.SetActive(true);
                        }
                    }
                    curDir = false;
                }
            }
        }
    }

    private void MoveLeft()
    {
        if (!curDir)
        {
            PlayerBlockCheck(PosL);
            SetAccessibleZone(true, PosR);
            if (!isBlockedByPlayer)
            {
                if(gm.Path.Count < 4 && dir != gm.dir)
                {

                }
                else
                {
                    if (isLive)
                    {
                        StartCoroutine(MoveObject(posL, PosL));
                    }
                    else
                    {
                        movOb.transform.position = posL;
                        SetAccessibleZone(false, PosL);
                        if (dir != curDir)
                        {
                            ghost.SetActive(false);
                        }
                        else
                        {
                            ghost.SetActive(true);
                        }
                    }
                    curDir = true;
                }
            }
        }
    }

    IEnumerator MoveObject(Vector2 pos, List<GameObject> list)
    {
        //Player position
        float objectX = movOb.transform.position.x;
        float objectY = movOb.transform.position.y;

        for (float i = 0; i < gm.stepLength; i += Time.deltaTime)
        {
            float percent = i / gm.stepLength;

            //New position
            float newX = Mathf.Lerp(objectX, pos.x, percent);
            float newY = Mathf.Lerp(objectY, pos.y, percent);

            movOb.transform.position = new Vector2(newX, newY);

            yield return new WaitForSeconds(Time.deltaTime);
        }
        movOb.transform.position = pos;

        SetAccessibleZone(false, list);
    }
        #endregion

    private void SetAccessibleZone(bool isActive, List<GameObject> list)
    {
        if (isBlockedByPlayer || ( gm.Path.Count < 4 && dir != gm.dir))
        {

        }
        else
        {
            for (int i = 0; i < list.Count; i += 1)
            {
                Tile tile = list[i].GetComponent<Tile>();
                tile.ChangeAccessibleState(isActive);
            }
        }
    }

    private void ChangeLiveState()
    {
        isLive = !isLive;

        ghost.SetActive(false);

        if(curDir != dir)
        {
            curDir = dir;

            if (dir)
            {
                movOb.transform.position = posL;

                for (int i = 0; i < PosR.Count; i += 1)
                {
                    Tile tile = PosR[i].GetComponent<Tile>();
                    tile.ChangeAccessibleState(true);
                }

                for (int i = 0; i < PosL.Count; i += 1)
                {
                    Tile tile = PosL[i].GetComponent<Tile>();
                    tile.ChangeAccessibleState(false);
                }
            }
            else
            {
                movOb.transform.position = posR;


                for (int i = 0; i < PosL.Count; i += 1)
                {
                    Tile tile = PosL[i].GetComponent<Tile>();
                    tile.ChangeAccessibleState(true);
                }

                for (int i = 0; i < PosR.Count; i += 1)
                {
                    Tile tile = PosR[i].GetComponent<Tile>();
                    tile.ChangeAccessibleState(false);
                }
            }
        }
    }

    IEnumerator Shake (float duration, float magnitude, float Roughness)
    {
        Vector2 initPos = movOb.transform.localPosition;

        for (float f = 0; f < duration; f += Time.deltaTime)
        {
            float x = Random.Range(-1, 1) * magnitude;
            float y = Random.Range(-1, 1) * magnitude;

            movOb.transform.position = new Vector2(initPos.x + x, initPos.y + y);

            yield return new WaitForSeconds(Time.deltaTime * Roughness);
        }
        movOb.transform.position = initPos;
    }

    private void PlayerBlockCheck(List<GameObject> listDown)
    {
        isBlockedByPlayer = false;
        for (int i = 0; i < listDown.Count; i += 1)
        {
            if (isLive)
            {
                if (gm.Path[gm.point].transform.position == listDown[i].transform.position)
                {
                    isBlockedByPlayer = true;
                    StartCoroutine(Shake(shakeDuration, shakeMagnitude, shakeRoughness));
                    break;
                }
            }
            else
            {
                if ((gm.Path[gm.Path.Count - 1].transform.position == listDown[i].transform.position && !gm.lastTileWasRemoved) || (gm.Path[gm.Path.Count - 3].transform.position == listDown[i].transform.position && gm.lastTileWasRemoved))
                {
                    isBlockedByPlayer = true;
                    StartCoroutine(Shake(shakeDuration, shakeMagnitude, shakeRoughness));
                    break;
                }
            }
        }
    }
}


