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

        if (currentEquipment[equipSlot] != null)
        {
            inventory.Add(currentEquipment[equipSlot]);
        }

        if (onEquipmentChangedCallback != null)
            onEquipmentChangedCallback.Invoke(newItem, currentEquipment[equipSlot]);

        currentEquipment[equipSlot] = newItem;
    }

    public void Unequip(int equipSlot)
    {
        if (currentEquipment[equipSlot] != null)
        {
            inventory.Add(currentEquipment[equipSlot]);

            if (onEquipmentChangedCallback != null)
                onEquipmentChangedCallback.Invoke(null, currentEquipment[equipSlot]);

            currentEquipment[equipSlot] = null;
        }
    }
}
