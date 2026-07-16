using UnityEngine;

public class ItemCollection : MonoBehaviour
{
    private ItemRay raycastShooter;

    void Start()
    {
        // Cache the ItemRay reference so we aren't using GetComponent every frame
        raycastShooter = GetComponentInChildren<ItemRay>();

        if (raycastShooter == null)
        {
            Debug.LogError($"ItemRay script is missing from {gameObject.name} or its children!");
        }
    }

    // Subscribe to the player's interact input
    void OnEnable()
    {
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
        // 1. Ensure we have a valid raycaster and it is currently pointing at something
        if (raycastShooter != null && raycastShooter.Hitting)
        {
            GameObject targetGameObject = raycastShooter.currentHitObject;

            // 2. Grab the ItemOverworld script from the object we are looking at
            ItemOverworld overworldComponent = targetGameObject.GetComponent<ItemOverworld>();

            if (overworldComponent != null)
            {
                // 3. Extract the ScriptableObject data
                Item collectedItem = overworldComponent.ItemData;

                // 4. Fire the event! (The ?. ensures it doesn't crash if nothing is listening)
                EventHolder.OnAddItem?.Invoke(collectedItem);

                // 5. Destroy the physical object in the scene
                Destroy(targetGameObject);
            }
            else
            {
                Debug.LogWarning($"Tried to collect {targetGameObject.name}, but it doesn't have an ItemOverworld script!");
            }
        }
    }
}