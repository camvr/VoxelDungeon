using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single); // TODO: should load to tutorial
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
