using UnityEngine;

public class PlayerStats : EntityStats {

    private void Start()
    {
        EquipmentManager.instance.onEquipmentChangedCallback += OnEquipmentChanged;
    }

    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        if (newItem != null)
        {
            defense.AddModifier(newItem.defenseModifier);
            damage.AddModifier(newItem.damageModifier);
        }

        if (oldItem != null)
        {
            defense.RemoveModifier(oldItem.defenseModifier);
            damage.RemoveModifier(oldItem.damageModifier);
        }
    }

    public override void Die()
    {
        base.Die();
        // game over!
    }

}
