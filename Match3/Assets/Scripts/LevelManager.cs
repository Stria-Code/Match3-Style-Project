using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject WinObject;
    [SerializeField] GameObject LoseObject;
    [SerializeField] AudioClip loseGame;
    [SerializeField] AudioClip winGame;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        LoseGame();
        WinGame();
    }

    void LoseGame()
    {
        if (MovesManager.moves <= 0)
        {
            LoseObject.SetActive(true);
            SoundManager.instance.PlaySound(loseGame);
        }
    }

    void WinGame()
    {
        if (ScoreManager.score == ScoreManager.winScore)
        {
            WinObject.SetActive(true);
            SoundManager.instance.PlaySound(winGame);
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
