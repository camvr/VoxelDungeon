using UnityEngine;

public class EntityStats : MonoBehaviour {

    public string entityName = "";
    public int maxHealth = 100;
    public int currentHealth { get; private set; }

    public Stat damage;
    public Stat defense;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Damage(int damage)
    {
        // TODO: play with this mechanic
        damage -= defense.GetValue();

        currentHealth -= Mathf.Clamp(damage, 0, int.MaxValue);

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
