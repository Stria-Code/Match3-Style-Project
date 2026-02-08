using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static int score;
    public static int winScore;
    TextMeshProUGUI scoreUI;
    // Start is called before the first frame update
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == "Level1")
        {
            winScore = 2500;
        }

        score = 0;
        scoreUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        DrawScore();
    }

    void DrawScore()
    {
        scoreUI.text = "SCORE:" + score.ToString();
    }
}
