using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]

public abstract class Item : ScriptableObject
{

    public string itemName;

    public TargetingType targetingType;
    public string itemDescription;

    public Sprite itemArt;

    public abstract void TriggerItem();

}
