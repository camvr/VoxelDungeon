using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour {

    public Image icon;
    public Button removeButton;

    Equipment equipment;

    public void SetItem(Equipment newItem)
    {
        equipment = newItem;
        icon.sprite = equipment.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot()
    {
        equipment = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton()
    {
        EquipmentManager.instance.Unequip((int)equipment.slot);
    }

    public void UseItem()
    {
        /* doesn't do much...
        if (equipment != null)
        {
            equipment.Use();
        }*/
    }

}
