using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    public List<GameObject> models;
    private GameObject fadeOutPanel;

    private void Awake()
    {
        fadeOutPanel = GameObject.Find("FadeOutPanel");
        fadeOutPanel.SetActive(false);
        models[Random.Range(0, models.Count)].SetActive(true);
    }

    public void StartGame()
    {
        fadeOutPanel.SetActive(true);
    }

    public void StartTutorial()
    {
        fadeOutPanel.GetComponent<PanelTransitions>().startTutorial = true;
        fadeOutPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
