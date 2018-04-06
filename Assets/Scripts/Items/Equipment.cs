using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item {
    public EquipmentType slot;

    public RandInt defenseModifierBase;
    public int defenseLevelModifier;

    public RandInt damageModifierBase;
    public int damageLevelModifier;

    public RandInt strengthModifierBase;
    public int strengthLevelModifier;

    public int defenseModifier { get; private set; }
    public int damageModifier { get; private set; }
    public int strengthModifier { get; private set; }

    public int level { get; private set; }

    private bool isInitialized = false;

    public void Initialize()
    {
        if (!isInitialized)
        {
            if (GameManager.instance.isTutorial)
                level = 0;
            else
                level = LevelManager.instance.GetLevel() - 1;

            defenseModifier = defenseModifierBase.Random + (level * defenseLevelModifier);
            damageModifier = damageModifierBase.Random + (level * damageLevelModifier);
            strengthModifier = strengthModifierBase.Random + (level * strengthLevelModifier);

            isInitialized = true;
        }
    }

    public override void Use()
    {
        base.Use();
        EquipmentManager.instance.Equip(this);
        RemoveFromInventory();
    }
}

public enum EquipmentType { Head, Chest, Legs, Weapon, Shield, }