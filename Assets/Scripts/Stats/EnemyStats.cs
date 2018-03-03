using UnityEngine;

public class EnemyStats : EntityStats {

    public override void Die()
    {
        base.Die();
        GetComponent<EnemyController>().isDead = true;
        MessageUI.instance.Log(entityName + " died.", Color.grey);
        // show death animation
        // loot drops and player XP
    }
}
