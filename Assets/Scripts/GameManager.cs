using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameObject[,] Grid;
    public const int Rows = 8;
    public const int Columns = 5;
    public GameObject[] gemPrefabs;
    public static int GameScore;
    public static float GameTime;
    public static bool GameOver;
    public static bool CanPlay;
    private bool _adPlayed;
    public RewardedAdsButton rewardedAdsButton;
    public GameObject gemContainer;
    public Animator gameOverAnimator;
    public TextMeshProUGUI gameOverScore;
    public TextMeshProUGUI gameOverHighScore;
    public TextMeshProUGUI scoreText;
    public Image timeImage;
    private static readonly int Death = Animator.StringToHash("death");

    private void Start()
    {
        CanPlay = true;
        GameTime = 10;
        GameScore = 0;
        Grid = new GameObject[Columns, Rows];
        InitializeGrid();
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
        CanPlay = false;
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
                gemPrefabs[Random.Range(0, gemPrefabs.Length)];
            if (!CheckImmediateMatch(col, row, randomGem))
            {
                return randomGem;
            }
            maxAttempts--;
        } while (maxAttempts > 0);
        return gemPrefabs[Random.Range(0, gemPrefabs.Length)];
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
        if (!GameOver && GameTime <= 0)
        {
            if (!_adPlayed)
            {
                rewardedAdsButton.LoadAd();
                _adPlayed = true;
            }
            if (GameScore >= PlayerPrefs.GetInt("HighScore", 0))
            {
                PlayerPrefs.SetInt("HighScore", GameScore);
            }
            GameOver = true;
            gameOverAnimator.SetTrigger(Death);
            gameOverScore.text = "Score: " + GameScore;
            gameOverHighScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0);
        }
    }

    public void PlayAgain()
    {
        CanPlay = true;
        GameTime = 10;
        GameOver = false;
        SceneManager.LoadScene(sceneBuildIndex: 1);
        StartCoroutine(GameManager.CheckGameTime());
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
