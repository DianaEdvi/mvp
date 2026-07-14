using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryUI;
    void Start()
    {
        // Start happens after all Awakes, so Player.Instance is guaranteed to be set
        if (Player.Instance != null)
        {
            Player.Instance.OnInventoryActionPerformed += HandleInventoryOpened;
        }
        else
        {
            Debug.LogError("Player instance is null! Make sure the Player script is attached to a GameObject in the scene.");
        }
    }

    void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnInventoryActionPerformed -= HandleInventoryOpened;
        }
    }

    private void HandleInventoryOpened()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
        else
        {
            Debug.LogError("Inventory UI GameObject is not assigned in the inspector.");
        }
    }
}