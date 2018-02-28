using UnityEngine;

public class EntityStats : MonoBehaviour {

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
        Debug.Log(transform.name + " takes " + damage + " damage."); // TODO temporary

        if (currentHealth < 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log(transform.name + " died.");
    }
}
