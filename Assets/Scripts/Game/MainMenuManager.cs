using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private GameObject fadeOutPanel;

    private void Awake()
    {
        fadeOutPanel = GameObject.Find("FadeOutPanel");
        fadeOutPanel.SetActive(false);
    }

    public void StartGame()
    {
        fadeOutPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
