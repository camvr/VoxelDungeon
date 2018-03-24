using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TileType
{
    Wall,
    WallTorch,
    Floor,
    Empty,
    Item,
}

public class BoardManager : MonoBehaviour {

    public GameObject enemyPrefab;
    public int minEnemies = 8;
    public int maxEnemies = 15;

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

        Debug.Log("We got here?");
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
                availableTiles[i][j] = board[i][j] == TileType.Floor;
            }
        }

        /* Place entities */

        // Place the player
        Vector3 startPos = GetLegalPosition();
        PlayerController.instance.transform.position = startPos; // TODO: set to entry point
        SetAvailableTile((int)startPos.x, (int)startPos.z, false);

        // Place enemies
        int numEnemies = Random.Range(minEnemies, maxEnemies);
        GameManager.instance.enemiesRemainingText.text = numEnemies.ToString();
        for (int i = 0; i < numEnemies; i++)
        {
            Vector3 enemyPos = GetLegalPosition();
            GameObject enemyInstance = Instantiate(enemyPrefab, enemyPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0)) as GameObject;
            enemyInstance.transform.parent = enemyObjects.transform;
            enemyInstance.GetComponent<EnemyStats>().maxHealth = Random.Range(20, 40);
            SetAvailableTile((int)enemyPos.x, (int)enemyPos.z, false);
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
        return (tile == TileType.Floor || tile == TileType.Item) && availableTiles[x][z];
    }

    public void SetAvailableTile(int x, int z, bool available)
    {
        availableTiles[x][z] = available;
    }

    public void DropItem(Item item, Vector3 position)
    {
        GameObject itemInstance = Instantiate(item.prefab, position, Quaternion.Euler(90, 45, 90)) as GameObject;
        itemInstance.transform.parent = itemObjects.transform;
        GameManager.instance.AddItem(itemInstance.transform);
    }
}
