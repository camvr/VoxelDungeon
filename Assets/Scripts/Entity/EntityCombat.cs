using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityStats))]
public class EntityCombat : MonoBehaviour {

    public event System.Action OnAttack;

    EntityStats stats;

    private void Start()
    {
        stats = GetComponent<EntityStats>();
    }

    public void Attack(EntityStats target)
    {
        target.Damage(stats.damage.GetValue()); // TODO: may need tweaking

        if (OnAttack != null)
            OnAttack();
    }
}
