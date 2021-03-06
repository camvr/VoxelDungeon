﻿using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : EntityStats {

    public Text healthText;
    public Image healthBar;

    private void Start()
    {
        EquipmentManager.instance.onEquipmentChangedCallback += OnEquipmentChanged;
        UpdateUI();
    }

    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        if (newItem != null)
        {
            defense.AddModifier(newItem.defenseModifier);
            damage.AddModifier(newItem.damageModifier);
            strength.AddModifier(newItem.strengthModifier);
        }

        if (oldItem != null)
        {
            defense.RemoveModifier(oldItem.defenseModifier);
            damage.RemoveModifier(oldItem.damageModifier);
            strength.RemoveModifier(oldItem.strengthModifier);
        }
    }

    public override void Damage(int damage)
    {
        if (currentHealth <= 0 || PlayerController.instance.isGodMode)
            return;
        
        currentHealth -= Mathf.Clamp(damage - defense.GetValue(), 0, currentHealth);

        MessageUI.instance.Log(entityName + " takes " + damage + " damage.", damage == 0 ? new Color(0.8f, 0.8f, 0.8f) : Color.red);

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void RegenHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateUI();
    }

    public override void Die()
    {
        base.Die();
        MessageUI.instance.Log("You died.", new Color(0.7f, 0.0f, 0.0f));
        PlayerController.instance.KillPlayer(); // game over!
    }

    private void UpdateUI()
    {
        healthText.text = Mathf.Clamp(currentHealth, 0, maxHealth).ToString();
        healthBar.fillAmount = Mathf.Clamp((float)currentHealth / (float)maxHealth, 0.0f, 1.0f);
    }
}
