using UnityEngine;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour {

    public delegate void OnUpdateUI();
    public OnUpdateUI onUpdateUICallback;

    public GameObject contextMenu;

    #region Singleton
    [HideInInspector] public static ContextMenu instance = null;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Only one instance of context menu allowed in the scene!");
            return;
        }

        instance = this;
    }
    #endregion

    void Update()
    {
        if (Input.GetButtonDown("Inventory") && !GameManager.instance.gameOver)
        {
            contextMenu.SetActive(!contextMenu.activeSelf);

            if (GameManager.instance.isTutorial && !contextMenu.activeSelf)
                TutorialManager.instance.ChallengeTrigger(TutorialState.INVENTORY);

            if (contextMenu.activeSelf && onUpdateUICallback != null)
                onUpdateUICallback.Invoke();
        }
    }
}
