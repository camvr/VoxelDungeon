using UnityEngine;

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
}
