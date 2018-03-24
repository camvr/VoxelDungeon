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
    public Text enemiesRemainingText;
    public GameObject gameOverUI;
    public GameObject levelCompleteUI;
    public int totalLevels = 5;

    private int level = 1;
    private List<EnemyController> enemies;
    private bool enemiesMoving = false;
    private bool doingSetup = true;


    void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);

        //SetupLevel();
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
        Debug.Log("Level Loaded");
        level++;
        SetupLevel();
        Debug.Log(mode);
    }

    private void SetupLevel()
    {
        enemiesMoving = false;
        playersTurn = false;
        gameOver = false;
        doingSetup = true;

        enemies = new List<EnemyController>();
        items = new List<Transform>();

        enemies.Clear();
        items.Clear();
        BoardManager.instance.Setup();

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
        if (enemies.Count <= 0)
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
                enemiesRemainingText.text = enemies.Count.ToString();
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

        yield return new WaitForSeconds(turnDelay);

        playersTurn = true;
        enemiesMoving = false;
    }
}
