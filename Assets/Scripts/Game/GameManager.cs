using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    [HideInInspector] public static GameManager instance = null;
    #endregion
    
    [HideInInspector] public bool playersTurn = true;
    [HideInInspector] public bool gameOver = false;
    [HideInInspector] public List<Transform> items { get; private set; }
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
        items = new List<Transform>();
        InitGame();
	}


    private void InitGame()
    {
        BoardManager.instance.Setup();
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
