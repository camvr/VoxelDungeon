using UnityEngine;

public class SpikeTrap : MonoBehaviour {
	public Transform spike;
    public RandInt damage;

    private PlayerController player;
    private Animator anim;

    private void Awake()
    {
        GameManager.instance.onTriggerWorldMovesCallback += AttemptAttack;
        player = PlayerController.instance;
        anim = GetComponent<Animator>();
    }

    public void AttemptAttack()
    {
        if ((player.transform.position - transform.position).sqrMagnitude < 0.5f)
        {
            anim.SetTrigger("SpikeTrigger");
            player.GetComponent<PlayerStats>().Damage(damage.Random);
        }
    }
}
