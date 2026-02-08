using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject CreditsObj;
    [SerializeField] GameObject MenuObj;
    [SerializeField] GameObject TutorialObj;
    // Start is called before the first frame update
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void ToggleTutorial(bool showTutorial)
    {
        TutorialObj.SetActive(showTutorial);
        MenuObj.SetActive(!showTutorial);
    }

    public void ToggleCredits(bool showCredits)
    {
        CreditsObj.SetActive(showCredits);
        MenuObj.SetActive(!showCredits);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
