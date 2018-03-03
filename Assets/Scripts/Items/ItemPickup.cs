using UnityEngine;

public class ItemPickup : Interactable {

    public Item item;

    public override void Interact()
    {
        base.Interact();

        Pickup();
    }

    public void Pickup()
    {
        MessageUI.instance.Log("You pick up a " + item.name, Color.blue);
        if (Inventory.instance.Add(item))
        {
            GameManager.instance.RemoveItem(transform);
            Destroy(gameObject);
        }
    }

    public bool CanPickup(Vector3 pos)
    {
        return (pos - transform.position).sqrMagnitude < radius * radius;
    }
}
