using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IClickable
{
    #region Variables
    GameManager gm;

    public bool isAccessible;
    public bool isObjective;
    [HideInInspector] public bool isStart;

    [SerializeField] private GameObject dangerousObjectiveSprite;
    [SerializeField] private GameObject objectiveDangerSprite;
    [SerializeField] private GameObject selectedSprite;
    [SerializeField] private GameObject objectiveSprite;
    [SerializeField] private GameObject dangerSprite;

    [SerializeField] private Color notAccessibleColor;

    bool isSelected = false;
    #endregion

    private void Awake()
    {
        GameObject manager = GameObject.Find("GameManager");
        gm = manager?.GetComponent<GameManager>();

        if (!isAccessible)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = notAccessibleColor;
        }
        if (isObjective)
        {
            gm.AddObjective(gameObject);
            objectiveSprite.SetActive(true);
        }
    }

    public void SetStartTile()
    {
        gm.AddTile(gameObject);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = Color.yellow;

        //Curent tile position
        Vector2 pos = transform.position;

        //Set up next tile possible positions
        float Size = transform.localScale.x;

        gm.possiblePosition1 = new Vector2(pos.x + Size, pos.y);
        gm.possiblePosition2 = new Vector2(pos.x - Size, pos.y);
        gm.possiblePosition3 = new Vector2(pos.x, pos.y + Size);
        gm.possiblePosition4 = new Vector2(pos.x, pos.y - Size);

        if (isAccessible)
        {
            isAccessible = false;
        }

        isStart = true;
    }

    public void Click()
    {
        if (isAccessible)
        {
            if (isSelected)
            {
                if(gm.Path[gm.Path.Count-1] == gameObject)
                {
                    ChangeSlelectedState();
                    gm.RemoveTile(gameObject);

                    //Last Tile position
                    Vector2 pos = gm.Path[gm.Path.Count - 1].transform.position;

                    //Set up next tile possible positions
                    float Size = transform.localScale.x;

                    gm.possiblePosition1 = new Vector2(pos.x + Size, pos.y);
                    gm.possiblePosition2 = new Vector2(pos.x - Size, pos.y);
                    gm.possiblePosition3 = new Vector2(pos.x, pos.y + Size);
                    gm.possiblePosition4 = new Vector2(pos.x, pos.y - Size);
                }
            }
            else
            {
                //Curent Tile position
                Vector2 pos = transform.position;

                if (!gm.canStartSequence && (pos == gm.possiblePosition1 || pos == gm.possiblePosition2 || pos == gm.possiblePosition3 || pos == gm.possiblePosition4))
                {
                    ChangeSlelectedState();
                    gm.AddTile(gameObject);

                    //Set up next tile possible positions
                    float Size = transform.localScale.x;

                    gm.possiblePosition1 = new Vector2(pos.x + Size, pos.y);
                    gm.possiblePosition2 = new Vector2(pos.x - Size, pos.y);
                    gm.possiblePosition3 = new Vector2(pos.x, pos.y + Size);
                    gm.possiblePosition4 = new Vector2(pos.x, pos.y - Size);
                }
            } 
        }       
    }


    private void ChangeSlelectedState()
    {
            isSelected = !isSelected;

        selectedSprite.SetActive(isSelected);

        if (isObjective)
        {
            if (isSelected)
            {
                gm.CountSelectedObjectives(1);
            }
            else
            {
                gm.CountSelectedObjectives(-1);
            }
        }
    }

    public void SetDangerState(bool b, Vector2 nextTilePosition)
    {
        if (isObjective)
        {
            if (b)
            {
                objectiveSprite.SetActive(false);
                dangerousObjectiveSprite.SetActive(true);

                Vector2 position = new Vector2(transform.position.x, transform.position.y);
                Vector2 dir = (nextTilePosition - position);
                Debug.Log(dir);
                objectiveDangerSprite.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, dir));
            }
            else
            {
                objectiveSprite.SetActive(true);
                dangerousObjectiveSprite.SetActive(false);
            }

        }
        else
        {
            dangerSprite.SetActive(b);
            if (b)
            {
                Vector2 position = new Vector2(transform.position.x, transform.position.y);
                Vector2 dir = (nextTilePosition - position);
                Debug.Log(dir);
                dangerSprite.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, dir));
            }
        }
    }

    public void ChangeAccessibleState(bool b)
    {
        isAccessible = b;

        if (isAccessible)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = Color.white;
        }
        else
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = notAccessibleColor;
        }
    }
}
