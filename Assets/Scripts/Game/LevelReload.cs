using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelReload : MonoBehaviour
{
	public void ReloadLevel()
    {
        GameManager.instance.gameOver = false;
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
