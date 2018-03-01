using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

    public GameObject player;
    public int viewRadius = 5;

    #region Singleton
    public static PlayerController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("There should only be one instance of the player!");
            return;
        }

        instance = this;
    }
    #endregion

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;


    }

    public bool IsInPlayerView(Vector3 target)
    {
        return (transform.position - target).sqrMagnitude < viewRadius * viewRadius;
    }

    public void KillPlayer()
    {
        // end the game
        Debug.Log("game over");
        GameManager.gameState = GameState.gameOver;
    }
}
