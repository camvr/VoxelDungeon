using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public LevelGenerator levelGenerator;
    
    public GameObject player;

    private GameObject entityTiles;

    void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        levelGenerator = GetComponent<LevelGenerator>();

        entityTiles = new GameObject("EntityTiles");

        InitGame();
	}

    void InitGame()
    {
        // Generate level
        Vector3 startPos = levelGenerator.Generate();

        /* Place entities */

        // Place the player
        GameObject playerInstance = Instantiate(player, startPos, Quaternion.identity) as GameObject;
        playerInstance.transform.parent = entityTiles.transform;
        Camera.main.gameObject.GetComponent<CameraFollow>().target = playerInstance.transform;

    }

    // Update is called once per frame
    void Update () {
		
	}
}
