using UnityEngine;

public class CombatItemListDisplay : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private GameObject itemButton;

    private void Start()
    {
        playerInventory = GameObject.Find("PlayerInventoryManager").GetComponent<Inventory>();
    }

    private void OnEnable()
    {
        PopulateList();
    }

    private void PopulateList()
    {

        // first clear the list of all buttons

        for (int i = 0; i < transform.childCount; i++)
        {

            Destroy(transform.GetChild(0).gameObject);

        }

        //then look through inventory and populate the item menu list for each item
        for (int i = 0; i < playerInventory.GetInventoryLength(); i++)
        {

            GameObject itemButtonInstance = Instantiate(itemButton, transform);
            itemButtonInstance.GetComponent<CombatItemDisplay>().SetupButton(playerInventory.GetItem(i));

            //this is cause the z is weird when we instantiate it as a child of the menu
            itemButtonInstance.transform.localPosition = Vector3.one;
        }
    }
}
