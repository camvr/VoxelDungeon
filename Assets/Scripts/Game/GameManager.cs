﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    [HideInInspector] public static GameManager instance = null;
    #endregion

    [HideInInspector] public bool playersTurn = false;
    [HideInInspector] public bool gameOver = false;
    [HideInInspector] public List<Transform> items { get; private set; }
    public float turnDelay = 0.25f;
    public float startDelay = 2f;
    public bool isTutorial = false;

    public delegate void OnTriggerWorldMoves();
    public OnTriggerWorldMoves onTriggerWorldMovesCallback;

    private List<EnemyController> enemies;
    private bool enemiesMoving = false;
    private bool doingSetup = true;

    private GameObject gameOverUI;
    private GameObject levelCompleteUI;
    private GameObject floorNumberText;

    private int godModeCounter = 0;
    private float deltaGodCommand = 0;

    void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
	}

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        SetupLevel();
    }

    private void SetupLevel()
    {
        gameOverUI = GameObject.Find("GameOverBG");
        levelCompleteUI = GameObject.Find("LevelCompletePanel");
        floorNumberText = GameObject.Find("FloorNumberText");

        if (SceneManager.GetActiveScene().name == "PortalLevel")
            floorNumberText.GetComponent<Text>().text = "Helvete";
        else if (!isTutorial)
            floorNumberText.GetComponent<Text>().text = "Floor " + LevelManager.instance.GetLevel();
        gameOverUI.SetActive(false);
        levelCompleteUI.SetActive(false);

        enemiesMoving = false;
        playersTurn = false;
        gameOver = false;
        doingSetup = true;

        enemies = new List<EnemyController>();
        items = new List<Transform>();

        enemies.Clear();
        items.Clear();
        BoardManager.instance.Setup();

        // Load state from previous level if applicable
        if (!isTutorial)
            LevelManager.instance.LoadState();

        doingSetup = false;
        playersTurn = true;
    }

    public void AddEnemy(EnemyController enemy)
    {
        enemies.Add(enemy);
    }

    public void AddItem(Transform item)
    {
        items.Add(item);
    }

    public void RemoveItem(Transform item)
    {
        items.Remove(item);
    }

    public void EndGame()
    {
        if (!gameOver)
        {
            gameOver = true;
            gameOverUI.SetActive(true);
        }   
    }

    public void LevelComplete()
    {
        if (!gameOver)
        {
            gameOver = true;
            levelCompleteUI.SetActive(true);
        }
    }

    // Update is called once per frame
    private void Update ()
    {
        // Activate god mode
        if (Input.GetKeyDown(KeyCode.G))
        {
            float newTime = Time.realtimeSinceStartup;
            if (newTime - deltaGodCommand < 0.5f)
                godModeCounter++;
            else
                godModeCounter = 1;

            if (godModeCounter >= 5)
            {
                if (PlayerController.instance.isGodMode)
                {
                    PlayerController.instance.isGodMode = false;
                    MessageUI.instance.Log("God Mode disabled.", Color.green);
                }
                else
                {
                    PlayerController.instance.isGodMode = true;
                    MessageUI.instance.Log("God Mode enabled.", Color.green);
                }

                godModeCounter = 0;
            }

            deltaGodCommand = newTime;
        }

        // god mode commands
        if (PlayerController.instance.isGodMode)
        {
            if (Input.GetKeyDown(KeyCode.L)) // next level
                LevelComplete();
            else if (Input.GetKeyDown(KeyCode.K)) // kill all in radius
            {
                foreach (EnemyController enemy in enemies.ToArray())
                {
                    if ((enemy.transform.position - PlayerController.instance.transform.position).sqrMagnitude < 9)
                    {
                        enemy.GetComponent<EnemyStats>().Die();
                    }
                }
            }

            playersTurn = false;
        }

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
                enemy.gameObject.SetActive(false);
                enemies.Remove(enemy);
                BoardManager.instance.SetAvailableTile((int)enemy.transform.position.x, (int)enemy.transform.position.z, true);
            }
        }

        yield return new WaitForSeconds(turnDelay);

        foreach (EnemyController enemy in enemies)
        {
            if (!enemy.isDead)
            {
                enemy.ChooseAction();
            }
        }

        if (onTriggerWorldMovesCallback != null)
            onTriggerWorldMovesCallback.Invoke();

        yield return new WaitForSeconds(turnDelay);

        playersTurn = true;
        enemiesMoving = false;
    }
}
