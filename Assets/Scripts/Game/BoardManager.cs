using UnityEngine;

public enum TileType
{
    Wall,
    Floor,
    Empty,
    Item,
}

public class BoardManager : MonoBehaviour {

    #region Singleton

    [HideInInspector] public BoardManager instance = null;

    private void Start()
    {
        if (instance != null)
        {
            Debug.LogWarning("Only one BoardManager can exist in the scene!");
            return;
        }

        instance = this;
    }

    #endregion

    public GameObject enemyPrefab;

    private GameObject enemyObjects;
    private LevelGenerator levelGenerator;
    private TileType[][] board;

    private void Awake()
    {
        levelGenerator = GetComponentInParent<LevelGenerator>();
        enemyObjects = new GameObject("EnemyTiles");
    }

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
}
