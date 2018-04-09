using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelTransitions : MonoBehaviour
{
    public bool startTutorial = false;

    public void DisableSelf()
    {
        transform.gameObject.SetActive(false);
    }

    public void InvokeNextLevel()
    {
        if (GameManager.instance.isTutorial)
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        else
            LevelManager.instance.NextLevel();
    }

    public void InvokeGameStart()
    {
        if (startTutorial)
            SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
        else
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
