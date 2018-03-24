using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {

    #region Singleton
    public static EquipmentManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject[] weapons = PlayerController.instance.weapons;

    private Inventory inventory;
    private Equipment[] currentEquipment;

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChangedCallback;

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
