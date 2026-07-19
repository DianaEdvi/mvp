using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Heal Item")]

public class HealItem : Item
{

    public int healAmount;

    public override void TriggerItem()
    {
        EventHolder.OnHealPlayer?.Invoke(healAmount);
    }

}
