using UnityEngine;

public enum TileType
{
    Wall,
    Floor,
    Empty,
    Item,
}

public class BoardManager : MonoBehaviour {

    public GameObject enemyPrefab;
    public GameObject[] itemPrefabs;

    private GameObject enemyObjects;
    private GameObject itemObjects;
    private LevelGenerator levelGenerator;
    private TileType[][] board;

    #region Singleton

    [HideInInspector] public static BoardManager instance = null;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Only one BoardManager can exist in the scene!");
            return;
        }

        instance = this;
        levelGenerator = GetComponentInParent<LevelGenerator>();
        enemyObjects = new GameObject("EnemyTiles");
        itemObjects = new GameObject("ItemTiles");
    }

    #endregion

    public void Setup()
    {
        // Generate level
        board = levelGenerator.Generate();

        /* Place entities */

        // Place the player
        PlayerController.instance.transform.position = GetLegalPosition(); // TODO: set to entry point

        // Place enemies
        // TODO: temporary
        for (int i = 0; i < 10; i++)
        {
            GameObject enemyInstance = Instantiate(enemyPrefab, GetLegalPosition(), Quaternion.Euler(0, Random.Range(0, 4) * 90, 0)) as GameObject;
            enemyInstance.transform.parent = enemyObjects.transform;
            GameManager.instance.AddEnemy(enemyInstance.GetComponent<EnemyController>());
        }

        // Place items for testing
        for (int i = 0; i < 10; i++)
        {
            GameObject itemInstance = Instantiate(itemPrefabs[0], GetLegalPosition(), Quaternion.Euler(90, 45, 90)) as GameObject;
            itemInstance.transform.parent = itemObjects.transform;
            GameManager.instance.AddItem(itemInstance.transform);
        }
    }

    public Vector3 GetLegalPosition()
    {
        int x = Random.Range(0, board.Length);
        int z = Random.Range(0, board[x].Length);

        while (!IsLegalPos(x, z))
        {
            x = Random.Range(0, board.Length);
            z = Random.Range(0, board[x].Length);
        }

        return new Vector3(x, 0.5f, z);
    }

    public bool IsLegalPos(int x, int z)
    {
        TileType tile = board[x][z];
        return tile == TileType.Floor || tile == TileType.Item;
    }

    public void DropItem(Item item, Vector3 position)
    {
        GameObject itemInstance = Instantiate(item.prefab, position, Quaternion.Euler(90, 45, 90)) as GameObject;
        itemInstance.transform.parent = itemObjects.transform;
        GameManager.instance.AddItem(itemInstance.transform);
    }
}
