using System.Collections;
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

        if (!isTutorial)
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
        if (Input.GetKey(KeyCode.P)) // TODO: temp god mode command
            LevelComplete();

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
