using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityStats))]
[RequireComponent(typeof(EntityCombat))]
public class EnemyController : Interactable {

    public int viewRadius = 7;
    public bool isDead = false;
    public bool isBoss = false;
    public List<GameObject> equipRefs;
    public List<Equipment> equipmentDrops;
    public List<Item> drops;

    private int maxMovementRetries = 8;
    private EnemyMovement movement;
    private EnemyStats stats;
    private EntityCombat combat;
    private PlayerController playerInstance;
    private Transform playerMemory = null;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        stats = GetComponent<EnemyStats>();
        combat = GetComponent<EntityCombat>();
    }

    private void Start()
    {
        playerInstance = PlayerController.instance;
        GameManager.instance.AddEnemy(this);
    }

    public void RandomizeStats(int level) // TODO: Could use some tweaking
    {
        // Randomize base stats
        stats.maxHealth = Random.Range(10 + (level * 5), 10 + (level * 10));

        stats.damage.InitValue(Random.Range(0, level), Random.Range(level + 1, ((level - 1) * 2) + 2));
        stats.strength.InitValue(Random.Range(0, level), Random.Range(level + 1, ((level - 1) * 2) + 2));
        stats.defense.InitValue(Random.Range(0, level), Random.Range(level + 1, ((level - 1) * 2) + 2));

        float equipChance;
        List<EquipmentType> equipped = new List<EquipmentType>();
        List<Equipment> mustDrop = new List<Equipment>();

        foreach (Equipment equipmentDrop in equipmentDrops)
        {
            equipChance = Random.Range(0f, 1f);
            if (equipChance < (float)level * equipmentDrop.dropChance && !equipped.Contains(equipmentDrop.slot))
            {
                equipmentDrop.Initialize();
                equipped.Add(equipmentDrop.slot);
                mustDrop.Add(equipmentDrop);
                foreach (GameObject equipRef in equipRefs)
                {
                    if (equipRef.GetComponent<ItemPickup>().item.name == equipmentDrop.name)
                    {
                        equipRef.SetActive(true);
                        stats.SetModifiers(equipmentDrop);
                        break;
                    }
                }
            }
        }

        stats.SetDrops(drops, mustDrop);
    }

    public override void Interact()
    {
        base.Interact();
        EntityCombat playerCombat = playerInstance.player.GetComponent<EntityCombat>();

        if (playerCombat != null)
        {
            playerCombat.Attack(stats);
        }
    }

    public void ChooseAction()
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
            EntityStats playerStats = playerInstance.player.GetComponent<EntityStats>();
            if (playerStats != null)
                combat.Attack(playerStats);
        }
        else if (PlayerInView())
        {
            // move towards player
            Vector3 target = PlayerController.instance.transform.position;
            if (playerMemory != null)
                target = playerMemory.position;

            MoveTowardsTarget(target);
        }
        else
        {
            Vector2[] moves = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            
            for (int i = 0; i < maxMovementRetries; i++)
            {
                if (movement.AttemptMove(moves[Random.Range(0, 4)]))
                    return;
            }
            // Otherwise do nothing!
            return;
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
            RaycastHit hit;
            Physics.Linecast(enemyPos, playerPos, out hit);
            if (hit.transform == null || hit.transform.tag != "Wall")
            {
                playerMemory = playerInstance.transform;
                return true;
            }
            else if (playerMemory != null && Mathf.Approximately((transform.position - playerMemory.position).sqrMagnitude, 0))
            {
                playerMemory = null;
            }
        }

        return false;
    }

    // TODO: implement more intuitive way of finding the best path
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

        for (int i = 0; i < priorityMoves.Length; i++)
        {
            if (movement.AttemptMove(priorityMoves[i]))
                return;
        }
    }
}
