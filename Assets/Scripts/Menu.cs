using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject howToPlay;
    public void StartGame()
    {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    public void HowToPlay()
    {
        howToPlay.SetActive(true);
    }

    public void Close()
    {
        howToPlay.SetActive(false);
    }
}
