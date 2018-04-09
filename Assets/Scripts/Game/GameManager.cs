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

    public List<EnemyController> enemies { get; private set; }
    private bool enemiesMoving = false;
    private bool doingSetup = true;

    // Variables dealing with final level
    public bool isFinalLevel { get; private set; }
    private int numBossesRemaining;
    private Text bossesRemainingText;
    private GameObject gameCompletePanel;
    private GameObject portal;

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

        isFinalLevel = false;
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
        {
            //floorNumberText.GetComponent<Text>().text = "Helvete";
            isFinalLevel = true;
            numBossesRemaining = LevelManager.instance.numberFinalBosses;
            bossesRemainingText = GameObject.Find("BossesRemainingText").GetComponent<Text>();
            bossesRemainingText.text = numBossesRemaining + " Portal Weavers left";

            gameCompletePanel = GameObject.Find("GameCompletePanel");
            gameCompletePanel.SetActive(false);
        }
        else if (!isTutorial)
            floorNumberText.GetComponent<Text>().text = "Floor " + LevelManager.instance.GetLevel();

        gameOverUI.SetActive(false);
        if (!isFinalLevel) levelCompleteUI.SetActive(false);

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

        if (isFinalLevel)
            portal = GameObject.FindGameObjectWithTag("Portal");

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
        if (!gameOver && !isFinalLevel)
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

        if (playersTurn || enemiesMoving || doingSetup || gameOver)
            return;

        StartCoroutine(EnemyTurn());

        if (isFinalLevel && numBossesRemaining == 0)
        {
            gameOver = true;
            StartCoroutine(InvokeEndGameScene());
        }
    }

    private IEnumerator EnemyTurn()
    {
        enemiesMoving = true;

        // Check and handle if any are dead
        foreach (EnemyController enemy in enemies.ToArray())
        {
            if (enemy.isDead)
            {
                if (enemy.isBoss)
                {
                    bossesRemainingText.text = --numBossesRemaining + " Portal Weavers left";
                }

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

    private IEnumerator InvokeEndGameScene()
    {
        yield return new WaitForSeconds(1f);

        // Move to portal
        Vector3 start = Camera.main.transform.position;
        Vector3 dest = portal.transform.position + new Vector3(-4, 4, -4);
        Quaternion startRotation = Camera.main.transform.rotation;
        Quaternion destRotation = Quaternion.LookRotation((portal.transform.position + new Vector3(1, 0, 1)) - dest);
        
        float t = 0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / 3f;
            Camera.main.transform.position = Vector3.Lerp(start, dest, t);
            Camera.main.transform.rotation = Quaternion.Slerp(startRotation, destRotation, t);
            yield return null;
        }

        Camera.main.transform.position = dest;
        Camera.main.transform.rotation = destRotation;

        // Start animation and trigger explosion animation
        portal.GetComponent<Animator>().SetTrigger("portalExplosion");

        float shakeDuration = 4f;
        float magnitude = 0.001f;
        t = 0.0f;
        Vector3 originalCamPos = Camera.main.transform.localPosition;
        Debug.Log(originalCamPos);

        while (t < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.localPosition = originalCamPos + new Vector3(x, y, 0);

            t += Time.deltaTime;
            magnitude += 0.001f;

            yield return null;
        }

        Camera.main.transform.localPosition = originalCamPos;

        yield return new WaitForSeconds(4f);

        gameCompletePanel.SetActive(true);
    }
}
