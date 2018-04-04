using UnityEngine;
using System.Collections.Generic;

public class EnemyStats : EntityStats {

    public float dropChance = 0.6f;

    private List<Item> drops;

    public void SetDrops(List<Item> _drops)
    {
        drops = _drops;
    }

    public void SetModifiers(Equipment newItem)
    {
        defense.AddModifier(newItem.defenseModifier);
        damage.AddModifier(newItem.damageModifier);
        strength.AddModifier(newItem.strengthModifier);
    }

    public override void Die()
    {
        base.Die();
        GetComponent<EnemyController>().isDead = true;
        MessageUI.instance.Log(entityName + " died.", new Color(0.8f, 0.8f, 0.8f));
        // show death animation
        
        if (drops.Count > 0 && Random.Range(0f, 1f) < dropChance) // loot drops and player XP?
            BoardManager.instance.DropItem(drops[Random.Range(0, drops.Count - 1)], transform.position);

        if (GameManager.instance.isTutorial)
            TutorialManager.instance.ChallengeTrigger(TutorialState.COMBAT);
    }
}
