using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GridOperations : MonoBehaviour
{
    public static List<GameObject> SelectedMatchHorizontal;
    public static List<GameObject> SelectedMatchVertical;
    public static List<GameObject> TargetMatchVertical;
    public static List<GameObject> TargetMatchHorizontal;

    public GameObject nullObject;
    public GameObject gemContainer;

    private void Awake()
    {
        SelectedMatchVertical = new List<GameObject>();
        SelectedMatchHorizontal = new List<GameObject>();
        TargetMatchVertical = new List<GameObject>();
        TargetMatchHorizontal = new List<GameObject>();
    }

    void Update()
    {
    }

    public static void SwapGemsOnGrid(Vector2Int gem1Position, Vector2Int gem2Position)
    {
        var gem1 = GameManager.Grid[gem1Position.x, gem1Position.y];
        var gem2 = GameManager.Grid[gem2Position.x, gem2Position.y];
        GameManager.Grid[gem1Position.x, gem1Position.y] = gem2;
        GameManager.Grid[gem2Position.x, gem2Position.y] = gem1;
    }

    public static void CheckMatchForSelected(Vector2Int oldPosition, Vector2Int newPosition, bool isSelected)
    {
        var horizontalMatches = 1;
        var currentGem = GameManager.Grid[oldPosition.x, oldPosition.y];
        var gemTag = currentGem.tag;
        SwapGemsOnGrid(oldPosition, newPosition);
        var col = newPosition.x;
        var row = newPosition.y;
        for (var c = col - 1; c >= 0; c--)
        {
            var adjacentGem = GameManager.Grid[c, row].gameObject;
            if (adjacentGem.CompareTag(gemTag))
            {
                horizontalMatches++;
                if (isSelected)
                {
                    SelectedMatchHorizontal.Add(adjacentGem);
                }
                else
                {
                    TargetMatchHorizontal.Add(adjacentGem);
                }
            }
            else
            {
                break;
            }
        }
        for (var c = col + 1; c < GameManager.Columns; c++)
        {
            var adjacentGem = GameManager.Grid[c, row].gameObject;
            if (adjacentGem.CompareTag(gemTag))
            {
                horizontalMatches++;
                if (isSelected)
                {
                    SelectedMatchHorizontal.Add(adjacentGem);
                }
                else
                {
                    TargetMatchHorizontal.Add(adjacentGem);
                }
            }
            else
            {
                break;
            }
        }

        if (horizontalMatches >= 3)
        {
            Debug.Log("Horizontal match");
        }
        else
        {
            if (isSelected)
            {
                SelectedMatchHorizontal.Clear();
            }
            else
            {
                TargetMatchHorizontal.Clear();
            }
        }

        var verticalMatches = 1;
        for (var r = row - 1; r >= 0; r--)
        {
            var adjacentGem = GameManager.Grid[col, r].gameObject;
            if (adjacentGem.CompareTag(gemTag))
            {
                verticalMatches++;
                if (isSelected)
                {
                    SelectedMatchVertical.Add(adjacentGem);
                }
                else
                {
                    TargetMatchVertical.Add(adjacentGem);
                }
            }
            else
            {
                break;
            }
        }

        for (var r = row + 1; r < GameManager.Rows; r++)
        {
            var adjacentGem = GameManager.Grid[col, r].gameObject;
            if (adjacentGem.CompareTag(gemTag))
            {
                verticalMatches++;
                if (isSelected)
                {
                    SelectedMatchVertical.Add(adjacentGem);
                }
                else
                {
                    TargetMatchVertical.Add(adjacentGem);
                }
            }
            else
            {
                break;
            }
        }

        if (verticalMatches >= 3)
        {
            Debug.Log("Vertical match");
        }
        else
        {
            if (isSelected)
            {
                SelectedMatchVertical.Clear();
            }
            else
            {
                TargetMatchVertical.Clear();
            }
        }
        SwapGemsOnGrid(newPosition, oldPosition);
        if (verticalMatches >= 3 || horizontalMatches >= 3)
        {
            Player.MatchExists = true;
            if (horizontalMatches >= 3 && verticalMatches >= 3)
            {
                if (isSelected)
                {
                    SelectedMatchHorizontal.Add(currentGem);
                }
                else
                {
                    TargetMatchHorizontal.Add(currentGem);
                }
            }
            else
            {
                if (verticalMatches >= 3)
                {
                    if (isSelected)
                    {
                        SelectedMatchVertical.Add(currentGem);
                    }
                    else
                    {
                        TargetMatchVertical.Add(currentGem);
                    }
                }
                else if (horizontalMatches >= 3)
                {
                    if (isSelected)
                    {
                        SelectedMatchHorizontal.Add(currentGem);
                    }
                    else
                    {
                        TargetMatchHorizontal.Add(currentGem);
                    }
                }
            }
        }
    }

    public void RemoveGems(List<GameObject> matchingGems)
    {
        for (int i = 0; i < matchingGems.Count; i++)
        {
            var gemPosition = matchingGems[i].transform.position;
            var gridPos = new Vector2Int((int) gemPosition.x, (int)gemPosition.y);
            GameManager.Grid[gridPos.x, gridPos.y] = Instantiate(nullObject, gemPosition, Quaternion.identity);
            GameManager.Grid[gridPos.x, gridPos.y].transform.SetParent(gemContainer.transform);
            GameManager.Grid[gridPos.x, gridPos.y].name = "(" + gridPos.x + "," + gridPos.y + ")";
            DOTween.Kill(matchingGems[i].transform);
            DOTween.instance.DOKill(matchingGems[i].transform);
            Destroy(matchingGems[i]);
        }
        matchingGems.Clear();
    }

    public static void MoveGemsAfterRemoval()
    {
        List<GameObject> movingGems = new List<GameObject>();
        for (var j = GameManager.Columns - 1; j >= 0; j--)
        {
            for (var i = GameManager.Rows - 1; i >= 0; i--)
            {
                if (GameManager.Grid[j, i].CompareTag("nullObject"))
                {
                    // need to figure out if the null object is a result of horizontal or vertical match
                    if (i is 0 or 1)
                    {
                        Debug.Log("here");
                        // this is 100% horizontal
                        for (int k = 0; k < movingGems.Count; k++)
                        {
                            movingGems[i].transform.DOMoveY(-1, 1).SetRelative();
                        }
                    }
                    else if (GameManager.Grid[j, i - 1].CompareTag("nullObject"))
                    {
                        // this is vertical
                    }
                    else
                    {
                        // this should mean that it is horizontal
                    }
                }
                else
                {
                    movingGems.Add(GameManager.Grid[j, i]);
                }
            }
            movingGems.Clear();
        }
    }
}