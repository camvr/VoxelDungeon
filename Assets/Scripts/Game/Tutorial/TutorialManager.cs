using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialState
{
    MOVEMENT,
    PICKUP,
    INVENTORY,
    COMBAT,
    EXIT
};

public class TutorialManager : MonoBehaviour {

    #region Singleton

    [HideInInspector] public static TutorialManager instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    #endregion

    public List<GameObject> panels;
    public List<Text> progress;
    public Item droppedWeapon;
    public GameObject enemy;
    public GameObject objectiveLight;

    [HideInInspector] public TutorialState state { get; private set; }
    private Color currentColor = new Color(0.184f, 0.184f, 0.184f);
    private Color completedColor = new Color(0f, 0.69f, 0.035f);
    private GameObject exitLadder;
    
	private void Start()
    {
        state = TutorialState.MOVEMENT;
        exitLadder = GameObject.Find("ladder(Clone)");
	}

    private void NextChallenge()
    {
        if (state != TutorialState.EXIT)
        {
            panels[(int)state].SetActive(false);
            panels[(int)state + 1].SetActive(true);

            progress[(int)state].color = completedColor;
            progress[(int)state + 1].color = currentColor;

            state = (TutorialState)((int)state + 1);

            // Run any challenge specific tasks
            switch (state)
            {
                case TutorialState.PICKUP: // spawn object
                    Vector3 pos = GetLegalPositionNearPlayer(2);
                    BoardManager.instance.DropItem((Equipment)droppedWeapon, pos);
                    objectiveLight.SetActive(true);
                    objectiveLight.transform.position = new Vector3(pos.x, 3, pos.z);
                    break;
                case TutorialState.INVENTORY:
                    objectiveLight.SetActive(false);
                    break;
                case TutorialState.COMBAT: // spawn enemy
                    Vector3 enemyPos = GetLegalPositionNearPlayer(4);
                    BoardManager.instance.SetAvailableTile((int)enemyPos.x, (int)enemyPos.z, false);
                    GameObject enemyInstance = Instantiate(enemy, enemyPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0)) as GameObject;
                    enemyInstance.GetComponent<EnemyStats>().SetDrops(new List<Item>(), new List<Equipment>());

                    objectiveLight.SetActive(true);
                    objectiveLight.transform.parent = enemyInstance.transform;
                    objectiveLight.transform.localPosition = new Vector3(0, 3, 0);
                    break;
                case TutorialState.EXIT:
                    objectiveLight.transform.parent = null;
                    objectiveLight.transform.localPosition = new Vector3(exitLadder.transform.position.x, 3, exitLadder.transform.position.z);
                    break;
            }
        }
    }

    private Vector3 GetLegalPositionNearPlayer(int r)
    {
        Vector3 pos;
        Vector3 playerPos = PlayerController.instance.transform.position;

        pos = new Vector3(playerPos.x + Random.Range(-r, r), playerPos.y, playerPos.z + Random.Range(-r, r));
        while (!BoardManager.instance.IsLegalPos((int)pos.x, (int)pos.z))
        {
            pos = new Vector3(playerPos.x + Random.Range(-r, r), playerPos.y, playerPos.z + Random.Range(-r, r));
        }

        return pos;
    }

    public void ChallengeTrigger(TutorialState challenge)
    {
        if (state == challenge)
            NextChallenge();
    }
}
