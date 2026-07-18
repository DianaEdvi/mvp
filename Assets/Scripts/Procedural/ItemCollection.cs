using UnityEngine;

public class ItemCollection : MonoBehaviour
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
        }
    }

    // Always unsubscribe when disabled to prevent NullReference exceptions
    void OnDisable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnInteractActionPerformed -= CollectItem;
        }
    }

    public void CollectItem()
    {
        if (raycastShooter != null && raycastShooter.Hitting)
        {
            GameObject targetGameObject = raycastShooter.currentHitObject;
            ItemOverworld overworldComponent = targetGameObject.GetComponent<ItemOverworld>();

            if (overworldComponent != null)
            {
                Item collectedItem = overworldComponent.ItemData;
                EventHolder.OnAddItem?.Invoke(collectedItem);
                Destroy(targetGameObject);
            }
            else
            {
                Debug.LogWarning($"Tried to collect {targetGameObject.name}, but it doesn't have an ItemOverworld script!");
            }
        }
    }
}