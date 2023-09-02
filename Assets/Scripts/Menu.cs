using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject howToPlay;
    // Start is called before the first frame update
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
