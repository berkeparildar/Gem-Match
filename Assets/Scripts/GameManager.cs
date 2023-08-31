using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] gems;
    
    public static GameObject[,] Grid;

    public GameObject GemContainer;

    public const int Rows = 10;

    public const int Columns = 7;

    // Start is called before the first frame update
    void Start()
    {
        Grid = new GameObject[Columns, Rows];
        InitializeGrid();
        GridOperations.PrintGrid();
    }
    
    private void InitializeGrid()
    {
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                var spawnPosition = new Vector3(j, i, 0);
                var randomGemPrefab = GetRandomGemPrefabWithoutMatch(j, i);
                var gem = Instantiate(randomGemPrefab, spawnPosition,
                    Quaternion.identity);
                gem.name = "(" + j + "," + i + ")";
                gem.transform.SetParent(GemContainer.transform);
                Grid[j, i] = gem;
            }
        }
    }
    
    private GameObject GetRandomGemPrefabWithoutMatch(int col, int row)
    {
        var maxAttempts = 10;
        do
        {
            var randomGem =
                gems[Random.Range(0, gems.Length)];
            if (!CheckImmediateMatch(col, row, randomGem))
            {
                return randomGem;
            }
            maxAttempts--;
        } while (maxAttempts > 0);
        return gems[Random.Range(0, gems.Length)];
    }
    
    private bool CheckImmediateMatch(int col, int row, GameObject gemPrefab)
    {
        var horizontalMatches = 1;
        for (var c = col - 1; c >= 0; c--)
        {
            var adjacentGem = Grid[c, row];
            if (adjacentGem.CompareTag(gemPrefab.tag))
            {
                horizontalMatches++;
            }
            else
            {
                break;
            }
        }
        if (horizontalMatches >= 3)
        {
            return true;
        }
        var verticalMatches = 1;
        for (var r = row - 1; r >= 0; r--)
        {
            var adjacentGemTransform = Grid[col, r];
            if (adjacentGemTransform.CompareTag(gemPrefab.tag))
            {
                verticalMatches++;
            }
            else
            {
                break;
            }
        }
        return verticalMatches >= 3;
    }
}
