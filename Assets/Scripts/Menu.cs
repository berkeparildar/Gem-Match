using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject howToPlay;
    public Image loadingImage;
    public AudioSource audioSource;
    public GameObject audio;

    private void Awake()
    {
        DontDestroyOnLoad(audio);
    }

    public void StartGame()
    {
        audioSource.Play();
        loadingImage.DOFillAmount(1, 1).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneBuildIndex: 1);
        });
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
