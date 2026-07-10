using UnityEngine;
using UnityEngine.Events;

public class EventHolder : MonoBehaviour
{
    public static UnityAction OnTriggerCircleActionCommand;

    public static UnityAction<string> OnActionCommandCompletion;

    public static UnityAction<GameObject> OnPlayerAttack;

    public static UnityAction OnPlayerBlock;

    public static UnityAction OnDisableMenu;

    public static UnityAction OnEnableMenu;

    public static UnityAction<GameObject> OnEnemyDeath;

    public static UnityAction OnPlayerDeath;

    public static UnityAction OnPlayerTurnStart;

    public static UnityAction OnEnemyTurnStart;

    public static UnityAction<int> OnPlayerTakeDamage;

    public static UnityAction<int> OnPlayerRemoveHealth;

    public static UnityAction<int> OnPlayerGainBlock;

    public static UnityAction OnDisableBlockUI;

    public static UnityAction OnRemoveActionPoint;

    public static UnityAction<int> OnHealPlayer;

    public static UnityAction<Item> OnAddItem;

    public static UnityAction<Item> OnRemoveItem; 
}
