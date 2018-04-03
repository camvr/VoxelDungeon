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
        int totalDamage = 0;
        int damage = stats.damage.GetValue();
        int strength = stats.strength.GetValue();

        totalDamage = damage + Mathf.RoundToInt(strength * Random.Range(0f, 2f));

        target.Damage(totalDamage);

        if (OnAttack != null)
            OnAttack();
    }
}
