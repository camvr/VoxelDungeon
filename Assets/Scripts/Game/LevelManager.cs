using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int totalLevels = 5;
    public List<Item> allItemRefs;

    private int level = 1;
    private bool isDefaultSetup = true;
    private int playerHealth = 100;
    private List<Item> playerInventory = new List<Item>();
    private Equipment[] playerEquiped = new Equipment[System.Enum.GetNames(typeof(EquipmentType)).Length];

    #region Singleton
    [HideInInspector] public static LevelManager instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (SceneManager.GetActiveScene().name == "GameScene")
            DontDestroyOnLoad(gameObject);
        else
        {
            instance = null;
            Destroy(gameObject);
        }
    }
    #endregion

    public int GetLevel()
    {
        return level;
    }

    public void NextLevel()
    {
        level++;
        SaveState();
        GameManager.instance.gameOver = false;
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void ResetState()
    {
        isDefaultSetup = true;
        level = 1;
        playerHealth = 100;
        playerInventory = new List<Item>();
        playerEquiped = new Equipment[System.Enum.GetNames(typeof(EquipmentType)).Length];
    }

    private void SaveState()
    {
        // save basic player stats
        playerHealth = PlayerController.instance.gameObject.GetComponent<PlayerStats>().currentHealth;

        // Save inventory state
        playerInventory = Inventory.instance.items;

        // Save equipment state
        playerEquiped = EquipmentManager.instance.GetEquipedState();

        isDefaultSetup = false;
    }

    public void LoadState()
    {
        if (!isDefaultSetup)
        {
            PlayerController.instance.gameObject.GetComponent<PlayerStats>().currentHealth = playerHealth;

            List<Item> item_refs = new List<Item>();
            foreach (Item item in playerInventory)
            {
                if (item != null)
                {
                    foreach (Item item_ref in allItemRefs)
                    {
                        if (item_ref.name.Equals(item.name))
                        {
                            item_refs.Add(item_ref);
                            break;
                        }
                    }
                }
            }

            Inventory.instance.SetInventoryState(item_refs);

            Equipment[] equip_refs = new Equipment[playerEquiped.Length];
            for (int i = 0; i < equip_refs.Length; i++)
            {
                Equipment equip = playerEquiped[i];
                if (equip != null)
                {
                    foreach (Item item_ref in allItemRefs)
                    {
                        if (item_ref.name.Equals(equip.name))
                        {
                            equip_refs[i] = (Equipment)item_ref;
                            break;
                        }
                    }
                }
            }

            EquipmentManager.instance.SetEquipedState(equip_refs);
        }
    }
}
