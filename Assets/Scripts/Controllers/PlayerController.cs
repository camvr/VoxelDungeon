using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

    public GameObject player;
    public int viewRadius = 5;

    public GameObject[] weapons;

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
                    return;
                }
            }
        }

        // Check for legal move, give relevant actions
        if (move != new Vector2(0, 0))
        {
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
            else if (hit.transform.tag == "Wall")
            {
                MessageUI.instance.Log("There's a wall in the way!", Color.white);
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
}
