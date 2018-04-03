using UnityEngine;

public class EntityStats : MonoBehaviour {

    public string entityName = "";
    public int maxHealth = 100;
    public int currentHealth;

    public Stat damage;
    public Stat strength;
    public Stat defense;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void Damage(int damage)
    {
        damage = Mathf.Clamp(damage - defense.GetValue(), 0, int.MaxValue);

        currentHealth -= damage;

        MessageUI.instance.Log(entityName + " takes " + damage + " damage.", Color.white);

        if (currentHealth < 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        //Debug.Log(entityName + " died.");
    }
}
