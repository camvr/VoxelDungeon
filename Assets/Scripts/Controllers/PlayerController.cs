using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public int viewRadius = 5;

    public float healthRegenChance = 0.3f;

    public GameObject[] weapons;

    private PlayerMovement movement;
    private PlayerStats stats;

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
        stats = player.GetComponent<PlayerStats>();
    }
    #endregion

    private void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!GameManager.instance.playersTurn || GameManager.instance.gameOver)
            return;
        
        Vector2 move = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.A)) // -x
        {
            move = Vector2.left;
        }
        else if (Input.GetKey(KeyCode.D)) // +x
        {
            move = Vector2.right;
        }
        else if (Input.GetKey(KeyCode.W)) // +z
        {
            move = Vector2.up;
        }
        else if (Input.GetKey(KeyCode.S)) // -z
        {
            move = Vector2.down;
        }
        else if (Input.GetKey(KeyCode.E)) // item pickup key
        {
            foreach (Transform item in GameManager.instance.items)
            {
                ItemPickup currItem = item.GetComponent<ItemPickup>();
                if (currItem.CanPickup(transform.position))
                {
                    currItem.Interact(); // interact with first item in list
                    GameManager.instance.playersTurn = false;

                    if (GameManager.instance.isTutorial)
                        TutorialManager.instance.ChallengeTrigger(TutorialState.PICKUP);

                    return;
                }
            }
        }

        // Check for legal move, give relevant actions
        if (move != new Vector2(0, 0))
        {
            if (GameManager.instance.isTutorial)
                TutorialManager.instance.ChallengeTrigger(TutorialState.MOVEMENT);

            Vector3 dest = transform.position + new Vector3(move.x, 0, move.y);
            RaycastHit hit;
            if (!Physics.Linecast(transform.position, dest, out hit))
            {
                movement.AttemptMove(move);
            }
            else if (hit.transform.tag == "Enemy")
            {
                movement.RotateToDir(move);

                hit.transform.GetComponent<EnemyController>().Interact();
            }
            else if (hit.transform.name.Contains("fbBrickWall"))
            {
                MessageUI.instance.Log("There's a wall in the way!", Color.white);
            }
            else if (hit.transform.name.Contains("fbBrickVase"))
            {
                MessageUI.instance.Log("There's a vase in the way!", Color.white);
            }

            // Regen health randomly
            if (Random.Range(0f, 1f) <= healthRegenChance)
            {
                stats.RegenHealth(1, 2);
            }

            GameManager.instance.playersTurn = false;
        }
            
    }

    public bool IsInPlayerView(Vector3 target)
    {
        return (transform.position - target).sqrMagnitude < viewRadius * viewRadius;
    }

    public void KillPlayer()
    {
        // end the game
        GameManager.instance.EndGame();
    }

    public PlayerStats GetStats()
    {
        return stats;
    }
}
