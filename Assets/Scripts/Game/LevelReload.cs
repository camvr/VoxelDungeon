using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelReload : MonoBehaviour
{
	public void ReloadLevel()
    {
        GameManager.instance.gameOver = false;
        LevelManager.instance.ResetState();
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
