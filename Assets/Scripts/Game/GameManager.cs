using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public LevelGenerator levelGenerator;
    
    /* Prefabs */
    public GameObject player;

    /* Global Object Containers */
    private GameObject entityTiles;

    /* Global State Variables */
    private static Map.TileType[][] board;


    void Awake () {
        /* Instantiate game manager */
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        /* Instantiate global containers */
        entityTiles = new GameObject("EntityTiles");

        /* Instantiate generation scripts */
        levelGenerator = GetComponent<LevelGenerator>();

        /* Run game setup */
        InitGame();
	}


    private void InitGame()
    {
        // Generate level
        board = levelGenerator.Generate();

        /* Place entities */

        // Place the player
        GameObject playerInstance = Instantiate(player, GetLegalPosition(), Quaternion.identity) as GameObject;
        playerInstance.transform.parent = entityTiles.transform;
        Camera.main.gameObject.GetComponent<CameraFollow>().target = playerInstance.transform;

    }


    public static Vector3 GetLegalPosition()
    {
        int x = Random.Range(0, board.Length);
        int z = Random.Range(0, board[x].Length);

        while (board[x][z] != Map.TileType.Floor)
        {
            x = Random.Range(0, board.Length);
            z = Random.Range(0, board[x].Length);
        }

        return new Vector3(x, 0.5f, z);
    }


    public static bool IsLegalPos(Vector3 pos)
    {
        return board[(int)pos.x][(int)pos.z] == Map.TileType.Floor;
    }


    // Update is called once per frame
    void Update () {
		
	}
}
