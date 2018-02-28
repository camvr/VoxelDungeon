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
        Debug.Log("Picking up " + item.name);
        if (Inventory.instance.Add(item))
            Destroy(gameObject);
    }
}
