using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : EntityStats {

    public Text healthText;
    public Image healthBar;

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

    public override void Damage(int damage)
    {
        base.Damage(damage);

        healthText.text = Mathf.Clamp(currentHealth, 0, maxHealth).ToString();
        healthBar.fillAmount = Mathf.Clamp((float)currentHealth / (float)maxHealth, 0.0f, 1.0f);
    }

    public override void Die()
    {
        base.Die();
        PlayerController.instance.KillPlayer(); // game over!
    }

}
