using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChangedCallback;
    
    private Equipment[] currentEquipment;
    private GameObject[] weaponRefs;
    private GameObject[] defenseRefs;

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

        int numSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;
        currentEquipment = new Equipment[numSlots];

        weaponRefs = PlayerController.instance.weaponRefs;
        defenseRefs = PlayerController.instance.defenseRefs;
    }
    #endregion

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
                foreach (GameObject weapon in weaponRefs)
                {
                    if (weapon.name == newItem.name)
                    {
                        weapon.SetActive(true);
                        break;
                    }
                }
                break;
            case EquipmentType.Shield:
                foreach (GameObject defense in defenseRefs)
                {
                    if (defense.name == newItem.name)
                    {
                        defense.SetActive(true);
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
            Inventory.instance.Add(currentEquipment[equipSlot]);

            if (onEquipmentChangedCallback != null)
                onEquipmentChangedCallback.Invoke(null, currentEquipment[equipSlot]);
            
            // Unequip prefab on player model
            switch (equipSlot)
            {
                case (int)EquipmentType.Weapon:
                    foreach (GameObject weapon in weaponRefs)
                    {
                        weapon.SetActive(false);
                    }
                    break;
                case (int)EquipmentType.Shield:
                    foreach (GameObject defense in defenseRefs)
                    {
                        defense.SetActive(false);
                    }
                    break;
                default:
                    break;
            }

            currentEquipment[equipSlot] = null;
        }
    }

    public Equipment[] GetEquipedState()
    {
        return currentEquipment;
    }

    public void SetEquipedState(Equipment[] equipment)
    {
        foreach (Equipment item in equipment)
        {
            if (item != null)
            {
                Equip(item);
            }
        }
    }
}
