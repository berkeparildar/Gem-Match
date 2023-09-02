using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject[] gems;
    public static int GameScore;
    public static float GameTime;
    public Animator gameOverAnimator;
    public TextMeshProUGUI gameOverScore;
    public TextMeshProUGUI gameOverHighScore;
    public RewardedAdsButton rewardedAdsButton;
    public static GameObject[,] Grid;

    public static bool canPlay;
    public GameObject gemContainer;
    public GridOperations gridOperations;

    public const int Rows = 8;

    public const int Columns = 5;
    public static bool gameOver;
    public TextMeshProUGUI scoreText;
    public Image timeImage;
    private static readonly int Death = Animator.StringToHash("death");

    // Start is called before the first frame update
    void Start()
    {
        canPlay = true;
        GameTime = 10;
        GameScore = 0;
        Grid = new GameObject[Columns, Rows];
        InitializeGrid();
        GridOperations.PrintGrid();
        StartCoroutine(CheckGameTime());
    }

    private void Update()
    {
        UpdateUI();
        CheckGameOver();
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
                gem.transform.SetParent(gemContainer.transform);
                Grid[j, i] = gem;
            }
        }
    }

    public static IEnumerator CheckGameTime()
    {
        while (GameTime > 0)
        {
            GameTime -= 1;
            yield return new WaitForSeconds(1);
        }
        canPlay = false;
    }

    private void UpdateUI()
    {
        if (GameTime > 10)
        {
            GameTime = 10;
        }
        scoreText.text = "Score: " + GameScore;
        timeImage.fillAmount = GameTime / 10;
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

    private void CheckGameOver()
    {
        if (!gameOver && GameTime <= 0)
        {
            rewardedAdsButton.LoadAd();
            if (GameScore >= PlayerPrefs.GetInt("HighScore", 0))
            {
                PlayerPrefs.SetInt("HighScore", GameScore);
            }
            gameOver = true;
            gameOverAnimator.SetTrigger(Death);
            gameOverScore.text = "Score: " + GameScore;
            gameOverHighScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0);
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }
}
