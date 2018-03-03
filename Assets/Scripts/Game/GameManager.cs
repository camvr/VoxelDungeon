﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    [HideInInspector] public static GameManager instance = null;
    #endregion

    [HideInInspector] public BoardManager boardManager;
    [HideInInspector] public bool playersTurn = true;
    [HideInInspector] public bool gameOver = false;
    public float turnDelay = 0.5f;


    private List<EnemyController> enemies;
    private bool enemiesMoving = false;
    private bool doingSetup = false;


    void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        enemies = new List<EnemyController>();
        boardManager = GetComponent<BoardManager>();
        InitGame();
	}


    private void InitGame()
    {
        boardManager.Setup();
    }

    public void AddEnemy(EnemyController enemy)
    {
        enemies.Add(enemy);
    }


    // Update is called once per frame
    private void Update ()
    {

        if (playersTurn || enemiesMoving || doingSetup || gameOver)
            return;

        StartCoroutine(EnemyTurn());
        
    }

    private IEnumerator EnemyTurn()
    {
        enemiesMoving = true;

        // Check and handle if any are dead
        foreach (EnemyController enemy in enemies.ToArray())
        {
            if (enemy.isDead)
            {
                // Play death animation
                // drop loot and give XP
                //DestroyObject(enemy.enemyObject);
                enemy.gameObject.SetActive(false);
                enemies.Remove(enemy);
            }
        }

        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].ChooseAction();
            yield return new WaitForSeconds(0.05f);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}
