using UnityEngine;

public class EnemyStats : EntityStats {

    public Item[] drops;
    public float dropChance = 0.3f;

    public override void Die()
    {
        base.Die();
        GetComponent<EnemyController>().isDead = true;
        MessageUI.instance.Log(entityName + " died.", new Color(0.8f, 0.8f, 0.8f));
        // show death animation
        
        if (Random.Range(0f, 1f) < dropChance) // loot drops and player XP?
            BoardManager.instance.DropItem(drops[Random.Range(0, drops.Length - 1)], transform.position);
    }
}
