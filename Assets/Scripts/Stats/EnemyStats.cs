using UnityEngine;

public class EnemyStats : EntityStats {

    public override void Die()
    {
        base.Die();
        // show death animation
        // loot drops and player XP
    }
}
