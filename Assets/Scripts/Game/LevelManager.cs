using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int totalLevels = 5;

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
            Inventory.instance.SetInventoryState(playerInventory);
            EquipmentManager.instance.SetEquipedState(playerEquiped);
        }
    }

    /*
     * TODO: Fix this weird reference lost issue:
        MissingReferenceException: The object of type 'Image' has been destroyed but you are still trying to access it.
        Your script should either check if it is null or you should not destroy the object.
        InventorySlot.AddItem (.Item newItem) (at Assets/Scripts/GUI/InventorySlot.cs:15)
        InventoryUI.UpdateUI () (at Assets/Scripts/GUI/InventoryUI.cs:34)
        Inventory.Add (.Item item) (at Assets/Scripts/Game/Inventory.cs:42)
        EquipmentManager.Unequip (Int32 equipSlot) (at Assets/Scripts/Game/EquipmentManager.cs:73)
        EquipmentManager.Equip (.Equipment newItem) (at Assets/Scripts/Game/EquipmentManager.cs:44)
        Equipment.Use () (at Assets/Scripts/Items/Equipment.cs:13)
        InventorySlot.UseItem () (at Assets/Scripts/GUI/InventorySlot.cs:37)
        UnityEngine.Events.InvokableCall.Invoke () (at C:/buildslave/unity/build/Runtime/Export/UnityEvent.cs:165)
        UnityEngine.Events.UnityEvent.Invoke () (at C:/buildslave/unity/build/Runtime/Export/UnityEvent_0.cs:58)
        UnityEngine.UI.Button.Press () (at C:/buildslave/unity/build/Extensions/guisystem/UnityEngine.UI/UI/Core/Button.cs:36)
        UnityEngine.UI.Button.OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) (at C:/buildslave/unity/build/Extensions/guisystem/UnityEngine.UI/UI/Core/Button.cs:45)
        UnityEngine.EventSystems.ExecuteEvents.Execute (IPointerClickHandler handler, UnityEngine.EventSystems.BaseEventData eventData) (at C:/buildslave/unity/build/Extensions/guisystem/UnityEngine.UI/EventSystem/ExecuteEvents.cs:50)
        UnityEngine.EventSystems.ExecuteEvents.Execute[IPointerClickHandler] (UnityEngine.GameObject target, UnityEngine.EventSystems.BaseEventData eventData, UnityEngine.EventSystems.EventFunction`1 functor) (at C:/buildslave/unity/build/Extensions/guisystem/UnityEngine.UI/EventSystem/ExecuteEvents.cs:261)
        UnityEngine.EventSystems.EventSystem:Update()
     */
}
