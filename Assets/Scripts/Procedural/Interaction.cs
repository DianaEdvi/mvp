using UnityEngine;

public class Interaction : MonoBehaviour
{
    private Ray raycastShooter;

    void Start()
    {
        // Cache the ItemRay reference so we aren't using GetComponent every frame
        raycastShooter = GetComponentInChildren<Ray>();

        if (raycastShooter == null)
        {
            Debug.LogError($"ItemRay script is missing from {gameObject.name} or its children!");
        }

        if (Player.Instance != null)
        {
            Player.Instance.OnInteractActionPerformed += CollectItem;
            Player.Instance.OnInteractActionPerformed += NPCInteraction;
        }
    }

    // Always unsubscribe when disabled to prevent NullReference exceptions
    void OnDisable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnInteractActionPerformed -= CollectItem;
            Player.Instance.OnInteractActionPerformed -= NPCInteraction;
        }
    }

    public void CollectItem()
    {
        if (raycastShooter != null && raycastShooter.Hitting)
        {
            GameObject targetGameObject = raycastShooter.currentHitObject;

            if (targetGameObject.layer != LayerMask.NameToLayer("Item")) return;

            ItemOverworld overworldComponent = targetGameObject.GetComponent<ItemOverworld>();

            if (overworldComponent != null)
            {
                Item collectedItem = overworldComponent.ItemData;
                EventHolder.OnAddItem?.Invoke(collectedItem);
                Destroy(targetGameObject);
            }
        }
    }

    public void NPCInteraction()
    {
        if (raycastShooter != null && raycastShooter.Hitting)
        {
            GameObject targetGameObject = raycastShooter.currentHitObject;

            if (targetGameObject.layer != LayerMask.NameToLayer("NPC")) return;
            Debug.Log("Interacting with NPC...");
            // npc.Interact().invoke;
        }
    }
}