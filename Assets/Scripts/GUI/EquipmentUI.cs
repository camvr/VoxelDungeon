using UnityEngine;

public class EquipmentUI : MonoBehaviour {

    public Transform equipmentParent;

    private EquipmentSlot[] slots;

    private EquipmentManager equipment;

    // Use this for initialization
    void Start()
    {
        equipment = EquipmentManager.instance;
        equipment.onEquipmentChangedCallback += UpdateUI;

        slots = equipmentParent.GetComponentsInChildren<EquipmentSlot>();

        foreach (Equipment item in equipment.GetEquipedState())
        {
            if (item != null)
                UpdateUI(item, null);
        }
    }

    void UpdateUI(Equipment newItem, Equipment oldItem)
    {
        if (oldItem == null)
        {
            slots[(int)newItem.slot].SetItem(newItem);
        }
        else if (newItem == null)
        {
            slots[(int)oldItem.slot].ClearSlot();
        }
        else
        {
            slots[(int)oldItem.slot].SetItem(newItem);
        }
    }
}
