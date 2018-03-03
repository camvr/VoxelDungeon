using UnityEngine;

public class Interactable : MonoBehaviour {

    public float radius = 1f;

    public Transform interactionTransform = null;

    public virtual void Interact()
    {
        //Debug.Log("Interacting with " + transform.name);
    }


    private void OnDrawGizmosSelected()
    {
        if (interactionTransform == null)
            interactionTransform = transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
