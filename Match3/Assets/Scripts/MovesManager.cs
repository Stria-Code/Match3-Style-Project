using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovesManager : MonoBehaviour
{
    public static int moves;
    TextMeshProUGUI movesUI;

    // Start is called before the first frame update
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;


        if(sceneName == "Level1")
        {
            moves = 30;
        }

        movesUI = GetComponent<TextMeshProUGUI>();


    }

    // Update is called once per frame
    void Update()
    {
        DrawMoves();
    }

    void DrawMoves()
    {
        movesUI.text = "MOVES:" + moves.ToString();
    }
}
