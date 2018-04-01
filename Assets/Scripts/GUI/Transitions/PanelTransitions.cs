using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelTransitions : MonoBehaviour
{
    public void DisableSelf()
    {
        transform.gameObject.SetActive(false);
    }

    public void InvokeNextLevel()
    {
        LevelManager.instance.NextLevel();
    }

    public void InvokeGameStart()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single); // TODO: should load to tutorial
    }
}
