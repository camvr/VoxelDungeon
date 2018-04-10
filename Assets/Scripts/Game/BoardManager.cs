using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TileType
{
    Wall,
    WallTorch,
    Floor,
    Exit,
    Empty,
    Item,
    Misc,
    Trap,
    Portal
}

public class BoardManager : MonoBehaviour {

    public GameObject enemyPrefab;
    public GameObject bossPrefab;

    private GameObject enemyObjects;
    private GameObject itemObjects;
    private LevelGenerator levelGenerator;
    private TileType[][] board;
    private bool[][] availableTiles;

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

        // Get available tiles
        availableTiles = new bool[board.Length][];
        for (int i = 0; i < board.Length; i++)
        {
            availableTiles[i] = new bool[board[i].Length];
            for (int j = 0; j < board[i].Length; j++)
            {
                availableTiles[i][j] = (board[i][j] == TileType.Floor || board[i][j] == TileType.Trap);
            }
        }

        /* Place entities */

        // Place the player
        Vector3 startPos = levelGenerator.GetPlayerStartPos();
        PlayerController.instance.transform.position = startPos;
        SetAvailableTile((int)startPos.x, (int)startPos.z, false);

        // Place enemies
        if (GameManager.instance.isFinalLevel)
        {
            for (int i = 0; i < LevelManager.instance.numberFinalBosses; i++)
            {
                Vector3 bossPos = GetLegalPosition();
                SetAvailableTile((int)bossPos.x, (int)bossPos.z, false);
                GameObject bossInstance = Instantiate(bossPrefab, bossPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0)) as GameObject;
                bossInstance.transform.parent = enemyObjects.transform;
            }

            int numEnemies = Random.Range(2, 4);
            for (int i = 0; i < numEnemies; i++)
            {
                Vector3 enemyPos = GetLegalPosition();
                SetAvailableTile((int)enemyPos.x, (int)enemyPos.z, false);
                GameObject enemyInstance = Instantiate(enemyPrefab, enemyPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0)) as GameObject;
                enemyInstance.transform.parent = enemyObjects.transform;

                // Set enemy instance's stats
                enemyInstance.GetComponent<EnemyController>().RandomizeStats(LevelManager.instance.GetLevel());
            }
        }
        else if (!GameManager.instance.isTutorial)
        {
            List<Vector3> enemyPositions = levelGenerator.GetEnemyDistributedPositions();
            foreach (Vector3 enemyPos in enemyPositions)
            {
                SetAvailableTile((int)enemyPos.x, (int)enemyPos.z, false);
                GameObject enemyInstance = Instantiate(enemyPrefab, enemyPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0)) as GameObject;
                enemyInstance.transform.parent = enemyObjects.transform;

                // Set enemy instance's stats
                enemyInstance.GetComponent<EnemyController>().RandomizeStats(LevelManager.instance.GetLevel());
            }
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
        if (x < 0 || x >= board.Length || z < 0 || z >= board[0].Length) return false;

        TileType tile = board[x][z];
        return (tile == TileType.Floor || tile == TileType.Item || tile == TileType.Trap) && availableTiles[x][z];
    }

    public bool IsExitTile(int x, int z)
    {
        if (x < 0 || x >= board.Length || z < 0 || z >= board[0].Length) return false;
        return board[x][z] == TileType.Exit;
    }

    public Vector3 GetExitTile()
    {
        for (int i = 0; i < board.Length; i++)
        {
            for (int j = 0; j < board[i].Length; j++)
            {
                if (board[i][j] == TileType.Exit)
                    return new Vector3(i, 0, j);
            }
        }
        return new Vector3(0, 0, 0);
    }

    public void SetAvailableTile(int x, int z, bool available)
    {
        if (x >= 0 || x < availableTiles.Length || z >= 0 || z < availableTiles[0].Length)
            availableTiles[x][z] = available;
    }

    public void DropItem(Item item, Vector3 position)
    {
        GameObject itemInstance = Instantiate(item.prefab, position, Quaternion.Euler(90, 45, 90)) as GameObject;
        itemInstance.transform.parent = itemObjects.transform;
        GameManager.instance.AddItem(itemInstance.transform);
    }

    public void DropItem(Equipment item, Vector3 position)
    {
        item.Initialize();
        GameObject itemInstance = Instantiate(item.prefab, position, Quaternion.Euler(90, 45, 90)) as GameObject;
        itemInstance.transform.parent = itemObjects.transform;
        GameManager.instance.AddItem(itemInstance.transform);
    }
}
