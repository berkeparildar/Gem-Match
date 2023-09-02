using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GridOperations : MonoBehaviour
{
    public static List<GameObject> SelectedMatchHorizontal;
    public static List<GameObject> SelectedMatchVertical;
    public static List<GameObject> TargetMatchVertical;
    public static List<GameObject> TargetMatchHorizontal;
    public static List<GameObject> RemovalGems;
    public Image timeImage;
    
    public GameObject scoreTextPopUp;


    public GameObject[] gemPrefabs;

    public GameObject nullObject;
    public GameObject gemContainer;
    private bool _horizontalInit = true;
    private bool _verticalInit = true;

    private void Awake()
    {
        SelectedMatchVertical = new List<GameObject>();
        SelectedMatchHorizontal = new List<GameObject>();
        TargetMatchVertical = new List<GameObject>();
        TargetMatchHorizontal = new List<GameObject>();
        RemovalGems = new List<GameObject>();
    }

    void Update()
    {
    }

    public GameObject GetRandomGem()
    {
        var randomGem = gemPrefabs[Random.Range(0, gemPrefabs.Length - 1)];
        return randomGem;
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
            //Debug.Log(currentGem.tag);
        }
    }

    public void RemoveGems(List<GameObject> matchingGems)
    {
        GameManager.GameScore += matchingGems.Count * 100;
        GameManager.GameTime += matchingGems.Count;
        for (int i = 0; i < matchingGems.Count; i++)
        {            
            var color = matchingGems[i].GetComponent<SpriteRenderer>().color;
            ShowScorePopUp(matchingGems[i].transform.position, 100, color);
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

    public void MoveGemsAfterRemoval()
    {
        List<GameObject> movingGems = new List<GameObject>();
        for (var j = GameManager.Columns - 1; j >= 0; j--)
        {
            for (var i = GameManager.Rows - 1; i >= 0; i--)
            {
                if (GameManager.Grid[j, i].CompareTag("nullObject"))
                {
                    // need to figure out if the null object is a result of horizontal or vertical match
                    if (i >= 1 && GameManager.Grid[j, i - 1].CompareTag("nullObject"))
                    {
                        Debug.Log("In vertical");
                        var nullCount = 1;
                        //Debug.Log("VERTICAL MOVEMENT");
                        for (var c = i - 1; c >= 0; c--)
                        {
                            var adjacentGem = GameManager.Grid[j, c].gameObject;
                            if (adjacentGem.CompareTag("nullObject"))
                            {
                                nullCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (movingGems.Count == 0)
                        {
                            for (int l = nullCount; l > 0; l--)
                            {
                                var newGem = Instantiate(GetRandomGem(), new Vector3(j, 
                                    GameManager.Rows, 0), Quaternion.identity);
                                newGem.transform.DOMoveY(-l, 0.5f).SetRelative();
                                GameManager.Grid[j, GameManager.Rows - l] = newGem;
                                newGem.name = "(" + j + "," + (GameManager.Rows - l) + ")";
                                newGem.transform.SetParent(gemContainer.transform);
                            }
                        }
                        else
                        {
                            for (int k = movingGems.Count - 1; k >= 0; k--)
                            {
                                var currentPos = movingGems[k].transform.position;
                                var gridPos = new Vector2Int((int) currentPos.x, (int) currentPos.y);
                                GameManager.Grid[gridPos.x, gridPos.y - nullCount] = GameManager.Grid[gridPos.x, gridPos.y];
                                //Debug.Log(nullCount);
                                movingGems[k].transform.DOMoveY(-nullCount, 0.5f).SetRelative();
                                if (_verticalInit && k== 0)
                                {
                                    _verticalInit = false;
                                    for (int l = nullCount; l > 0; l--)
                                    {
                                        var newGem = Instantiate(GetRandomGem(), new Vector3(gridPos.x, 
                                            GameManager.Rows, 0), Quaternion.identity);
                                        newGem.transform.DOMoveY(-l, 0.5f).SetRelative();
                                        GameManager.Grid[gridPos.x, GameManager.Rows - l] = newGem;
                                        newGem.name = "(" + gridPos.x + "," + (GameManager.Rows - l) + ")";
                                        newGem.transform.SetParent(gemContainer.transform);
                                    }
                                }
                            }   
                        }
                        //PrintGrid();
                    }
                    else
                    {
                        Debug.Log("In horizotal");
                        // this is 100% horizontal
                        if (movingGems.Count > 0)
                        {
                            for (int k = movingGems.Count - 1; k >= 0; k--)
                            {
                                var currentPos = movingGems[k].transform.position;
                                var gridPos = new Vector2Int((int) currentPos.x, (int) currentPos.y);
                                GameManager.Grid[gridPos.x, gridPos.y - 1] = GameManager.Grid[gridPos.x, gridPos.y];
                                movingGems[k].name = "(" + gridPos.x + "," + (gridPos.y - 1) + ")";
                                movingGems[k].transform.DOMoveY(-1, 0.5f).SetRelative();
                                if (_horizontalInit && k == 0)
                                {
                                    _horizontalInit = false;
                                    var newGem = Instantiate(GetRandomGem(), new Vector3(gridPos.x, 
                                        GameManager.Rows, 0), Quaternion.identity);
                                    newGem.transform.DOMoveY(-1, 0.5f).SetRelative();
                                    GameManager.Grid[gridPos.x, GameManager.Rows - 1] = newGem;
                                    newGem.name = "(" + gridPos.x + "," + (GameManager.Rows - 1) + ")";
                                    newGem.transform.SetParent(gemContainer.transform);
                                }
                                //PrintGrid();
                            }
                        }
                        else
                        {
                            if (_horizontalInit)
                            {
                                _horizontalInit = false;
                                var newGem = Instantiate(GetRandomGem(), new Vector3(j, 
                                    GameManager.Rows, 0), Quaternion.identity);
                                newGem.transform.DOMoveY(-1, 0.5f).SetRelative();
                                GameManager.Grid[j, GameManager.Rows - 1] = newGem;
                                newGem.name = "(" + j + "," + (GameManager.Rows - 1) + ")";
                                newGem.transform.SetParent(gemContainer.transform);
                            }
                        }
                    }
                }
                else
                {
                    movingGems.Add(GameManager.Grid[j, i]);
                }
            }
            _horizontalInit = true;
            _verticalInit = true;
            movingGems.Clear();
        }
    }

    public static void CheckNull()
    {
        for (var i = 0; i < GameManager.Rows; i++)
        {
            for (var j = 0; j < GameManager.Columns; j++)
            {
                if (GameManager.Grid[j, i].CompareTag("nullObject"))
                {
                    Debug.Log("NULL EXISTS AT " + j + "," + i);
                }
            }
        }
    }
    
    private void ShowScorePopUp(Vector3 pos, int score, Color color)
    {
        var textGameObject = Instantiate(scoreTextPopUp, pos, Quaternion.identity);
        var text = textGameObject.GetComponent<TextMeshPro>();
        text.text = score.ToString();
        text.color = color;
        timeImage.color = color;
        text.transform.DOMoveY(1, 0.5f).SetRelative();
        text.DOColor(new Color(0, 0, 0, 0), 0.5f).OnComplete(() =>
        {
            Destroy(text.gameObject, 2);
        });
    }

    public static void PrintGrid()
    {
        var str = "";
        for (var i = GameManager.Rows - 1; i >= 0; i--)
        {
            for (var j = 0; j < GameManager.Columns; j++)
            {
                str += GameManager.Grid[j, i].tag[0];
            }
            str += "\n";
        }
        Debug.Log(str);
    }
}