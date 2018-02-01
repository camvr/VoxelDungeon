using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public LevelGenerator levelGenerator;

	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        levelGenerator = GetComponent<LevelGenerator>();
        InitGame();
	}

    void InitGame()
    {
        levelGenerator.Generate();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
