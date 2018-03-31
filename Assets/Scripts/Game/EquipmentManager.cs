using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {

    public GameObject[] weapons;
    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChangedCallback;

    private Inventory inventory;
    private Equipment[] currentEquipment;

    #region Singleton
    [HideInInspector] public static EquipmentManager instance = null;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Only one instance of Equipment manager allowed in the scene!");
            return;
        }

        instance = this;
        weapons = PlayerController.instance.weapons;
    }
    #endregion

    private void Start()
    {
        inventory = Inventory.instance;
        int numSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;
        currentEquipment = new Equipment[numSlots];
    }

    public void Equip(Equipment newItem)
    {
        int equipSlot = (int)newItem.slot;

        /*if (currentEquipment[equipSlot] != null)
        {
            inventory.Add(currentEquipment[equipSlot]);
            
        }*/

        // Unequip anything already in the equipment slot
        Unequip(equipSlot);

        if (onEquipmentChangedCallback != null)
            onEquipmentChangedCallback.Invoke(newItem, currentEquipment[equipSlot]);

        currentEquipment[equipSlot] = newItem;

        // Equip prefab on player model
        switch (newItem.slot)
        {
            case EquipmentType.Weapon:
                foreach (GameObject weapon in weapons)
                {
                    if (weapon.GetComponent<ItemPickup>().item == newItem)
                    {
                        weapon.SetActive(true);
                        break;
                    }
                }
                break;
            default:
                break;
        }
    }

    public void Unequip(int equipSlot)
    {
        if (currentEquipment[equipSlot] != null)
        {
            inventory.Add(currentEquipment[equipSlot]);

            if (onEquipmentChangedCallback != null)
                onEquipmentChangedCallback.Invoke(null, currentEquipment[equipSlot]);
            
            // Unequip prefab on player model
            switch (equipSlot)
            {
                case (int)EquipmentType.Weapon:
                    foreach (GameObject weapon in weapons)
                    {
                        weapon.SetActive(false);
                    }
                    break;
                default:
                    break;
            }

            currentEquipment[equipSlot] = null;
        }
    }
}
