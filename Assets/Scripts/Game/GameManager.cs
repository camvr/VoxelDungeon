using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    playerTurn,
    worldTurn,
    gameOver,
}

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance = null;
    #endregion

    [HideInInspector]
    public static GameState gameState = GameState.playerTurn;

    [HideInInspector]
    public LevelGenerator levelGenerator;

    /* Prefabs */
    public GameObject enemyPrefab;

    /* Global Object Containers */
    private GameObject enemyObjects;

    private List<EnemyController> enemies = new List<EnemyController>();

    void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        /* Instantiate global containers */
        enemyObjects = new GameObject("EnemyObjects");

        /* Instantiate generation scripts */
        levelGenerator = GetComponent<LevelGenerator>();

        /* Run game setup */
        InitGame();
	}


    private void InitGame()
    {
        // Generate level
        Vector3 playerStartPos = levelGenerator.Generate();

        /* Place entities */

        // Place the player
        PlayerController.instance.transform.position = playerStartPos;

        // Place enemies
        // TODO: temporary
        for (int i = 0; i < 10; i++)
        {
            GameObject enemyInstance = Instantiate(enemyPrefab, Map.GetLegalPosition(), Quaternion.Euler(0, Random.Range(0, 4) * 90, 0)) as GameObject;
            enemyInstance.transform.parent = enemyObjects.transform;
            enemies.Add(enemyInstance.GetComponent<EnemyController>());
        }
    }


    // Update is called once per frame
    void Update ()
    {
		/* Handle Game States */
        switch (gameState)
        {
            case GameState.worldTurn:
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].chooseAction();
                }
                
                gameState = GameState.playerTurn;
                break;
            case GameState.gameOver:
                // game over screen
                break;
            default:
                break;
        }
	}
}
