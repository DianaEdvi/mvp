using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Item> inventory = new List<Item>();

    private void OnEnable()
    {
        EventHolder.OnAddItem += AddItemToInventory;
        EventHolder.OnRemoveItem += RemoveItemFromInventory;
    }

    private void OnDisable()
    {
        EventHolder.OnAddItem -= AddItemToInventory;
        EventHolder.OnRemoveItem -= RemoveItemFromInventory;
    }

    public int GetInventoryLength()
    {
        return inventory.Count;
    }

    public Item GetItem(int index)
    {
        return inventory[index];
    }

    public void AddItemToInventory(Item i)
    {
        inventory.Add(i);
    }

    public void RemoveItemFromInventory(Item i)
    {
        inventory.Remove(i);
    }
}