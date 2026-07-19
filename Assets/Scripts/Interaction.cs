using UnityEngine;

public class Interaction : MonoBehaviour
{
    [Header("Proximity Settings")]
    public float interactionRadius = 3f;
    public LayerMask interactableLayers;

    void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnInteractActionPerformed += TryInteract;
        }
    }

    void OnDisable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnInteractActionPerformed -= TryInteract;
        }
    }

    public void TryInteract()
    {
        // Cast a sphere around the player to find everything nearby on the designated layers
        Collider[] hitColliders = Physics.OverlapSphere(Player.Instance.transform.position, interactionRadius, interactableLayers);

        // If nothing is nearby, do nothing
        if (hitColliders.Length == 0) return;

        // Find the closest interactable object
        Collider closestCollider = null;
        float closestDistance = float.MaxValue;

        foreach (Collider col in hitColliders)
        {
            float distance = Vector3.Distance(Player.Instance.transform.position, col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollider = col;
            }
        }

        GameObject targetGameObject = closestCollider.gameObject;

        // Convert the layer integer to a string for a clean switch statement
        string layerName = LayerMask.LayerToName(targetGameObject.layer);

        // Route the interaction logic 
        switch (layerName)
        {
            case "Item":
                ItemOverworld overworldComponent = targetGameObject.GetComponent<ItemOverworld>();
                if (overworldComponent != null)
                {
                    Item collectedItem = overworldComponent.ItemData;
                    EventHolder.OnAddItem?.Invoke(collectedItem);
                    Destroy(targetGameObject);
                }
                break;

            case "NPC":
                NPC_Asset npc = targetGameObject.GetComponent<NPC_Asset>();
                if (npc != null)
                {
                    npc.Interact();
                }
                break;

            default:
                Debug.LogWarning($"Interacted with an unhandled layer: {layerName}");
                break;
        }
    }

    // Draws a visual ring in the Unity Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // Added a quick null check here so it doesn't throw errors in Edit Mode when Player doesn't exist
        if (Application.isPlaying && Player.Instance != null)
        {
            Gizmos.DrawWireSphere(Player.Instance.transform.position, interactionRadius);
        }
    }
}