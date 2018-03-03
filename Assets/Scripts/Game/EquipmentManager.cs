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

    Inventory inventory;

    Equipment[] currentEquipment;
    private GameObject[] equipmentPrefabs;

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChangedCallback;

    private void Start()
    {
        inventory = Inventory.instance;
        int numSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;
        currentEquipment = new Equipment[numSlots];
        equipmentPrefabs = new GameObject[numSlots];
    }

    public void Equip(Equipment newItem)
    {
        int equipSlot = (int)newItem.slot;

        if (currentEquipment[equipSlot] != null)
        {
            inventory.Add(currentEquipment[equipSlot]);
        }

        if (onEquipmentChangedCallback != null)
            onEquipmentChangedCallback.Invoke(newItem, currentEquipment[equipSlot]);

        currentEquipment[equipSlot] = newItem;

        // Equip prefab on player model
        switch (newItem.slot)
        {
            case EquipmentType.Weapon:
                Transform parent = GameObject.FindGameObjectWithTag("RH_Melee").transform;
                if (parent != null)
                    equipmentPrefabs[equipSlot] = Instantiate(newItem.prefab, parent) as GameObject;
                else
                    Debug.Log("Couldn't wield weapon");
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

            currentEquipment[equipSlot] = null;

            // Unequip prefab on player model
            Destroy(equipmentPrefabs[equipSlot]);
            equipmentPrefabs[equipSlot] = null;
        }
    }
}
