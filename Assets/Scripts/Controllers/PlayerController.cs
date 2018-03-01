using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

    public GameObject player;
    public int viewRadius = 5;

    private PlayerMovement movement;

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
        movement = player.GetComponent<PlayerMovement>();
    }
    #endregion

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (GameManager.gameState == GameState.playerTurn)
        {
            Vector2 move = new Vector2(0, 0);
            if (Input.GetKey(KeyCode.A)) // -x
            {
                move = Vector2.left;
            }
            if (Input.GetKey(KeyCode.D)) // +x
            {
                move = Vector2.right;
            }
            if (Input.GetKey(KeyCode.W)) // +z
            {
                move = Vector2.up;
            }
            if (Input.GetKey(KeyCode.S)) // -z
            {
                move = Vector2.down;
            }

            // Check for legal move, give relevant actions
            RaycastHit ray;
            if (!Physics.Linecast(transform.position, new Vector3(move.x, 0, move.y), out ray))
            {
                movement.AttemptMove(move);
            }
            else if (ray.transform.name == "enemy") // this will not work and needs to be changed
            {
                // attack
            }
        }
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
