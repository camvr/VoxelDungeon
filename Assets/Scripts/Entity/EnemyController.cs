using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Interactable {
    public int viewRadius = 5;

    private int maxMovementRetries = 8;
    private EnemyMovement movement;
    private EnemyStats stats;
    private PlayerController playerInstance;

    private void Start()
    {
        movement = GetComponent<EnemyMovement>();
        stats = GetComponent<EnemyStats>();
        playerInstance = PlayerController.instance;
    }

    public override void Interact()
    {
        base.Interact();
        EntityCombat playerStats = playerInstance.player.GetComponent<EntityCombat>();
    }

    public void chooseAction()
    {
        /* Action priority:
         * 1. Attack player if they're in reach
         * 2. Move to player if they're in sight
         * 3. (interact with items?)
         * 4. Move in a random direction
         * 5. Skip turn (heal abit?)
         */

        Vector2 attackDir;
        
        if (CanAttackPlayer(out attackDir))
        {
            // face player
            movement.RotateToDir(attackDir);
            // attack player
            Debug.Log(transform.name + " attacks player.");
        }
        else if (PlayerInView())
        {
            // move towards player
            MoveTowardsTarget(PlayerController.instance.transform.position); // Change this to move towards memory of location
        }
        else
        {
            Vector2[] moves = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            int numRetries = 0;

            while (!movement.AttemptMove(moves[Random.Range(0, 4)]) || numRetries < maxMovementRetries)
                numRetries++;

            // Otherwise do nothing!

        }
    }

    private bool CanAttackPlayer(out Vector2 dir)
    {
        Vector3 playerPos = PlayerController.instance.transform.position;
        Vector3 enemyPos = transform.position;

        dir = new Vector2(0, 0);

        if ((playerPos - enemyPos).sqrMagnitude < 2)
        {
            if ((int)playerPos.z == (int)enemyPos.z)
            {
                if (playerPos.x > enemyPos.x)
                    dir = Vector2.right;
                else if (playerPos.x < enemyPos.x)
                    dir = Vector2.left;
                else
                    return false;
                return true;
            }
            else if ((int)playerPos.x == (int)enemyPos.x)
            {
                if (playerPos.z > enemyPos.z)
                    dir = Vector2.up;
                else if (playerPos.z < enemyPos.z)
                    dir = Vector2.down;
                else
                    return false;
                return true;
            }
        }

        return false;
    }

    private bool PlayerInView()
    {
        Vector3 playerPos = PlayerController.instance.transform.position;
        Vector3 enemyPos = transform.position;

        if ((playerPos - enemyPos).sqrMagnitude < viewRadius*viewRadius)
        {
            // Player in view radius
            RaycastHit hit; // TODO: Check if a wall is blocking the way
            
            if (!Physics.Raycast(enemyPos, playerPos, out hit, viewRadius) || hit.transform == PlayerController.instance.transform)
            {
                // Player is visible
                return true;
            }

            // TODO: Keep a memory of last known position to move to if player is suddenly not visible
        }

        return false;
    }

    private void MoveTowardsTarget(Vector3 target)
    {
        Vector2[] priorityMoves = new Vector2[4];

        int dx = (int)(target.x - transform.position.x);
        int dz = (int)(target.z - transform.position.z);

        if (Mathf.Abs(dx) > Mathf.Abs(dz))
        {
            priorityMoves[0] = dx > 0 ? Vector2.right : Vector2.left;
            priorityMoves[1] = dz > 0 ? Vector2.up : Vector2.down;
            priorityMoves[2] = dx > 0 ? Vector2.left : Vector2.right;
            priorityMoves[3] = dz > 0 ? Vector2.down : Vector2.up;
        }
        else
        {
            priorityMoves[0] = dz > 0 ? Vector2.up : Vector2.down;
            priorityMoves[1] = dx > 0 ? Vector2.right : Vector2.left;
            priorityMoves[2] = dz > 0 ? Vector2.down : Vector2.up;
            priorityMoves[3] = dx > 0 ? Vector2.left : Vector2.right;
        }

        int moveIndex = 0;
        while (!movement.AttemptMove(priorityMoves[moveIndex]) && moveIndex < 4)
            moveIndex++;
    }
}
