using UnityEngine;
using UnityEngine.Events;

public class EventHolder : MonoBehaviour
{
    public static UnityAction OnTriggerCombat;

    public static UnityAction OnTriggerCircleActionCommand;

    public static UnityAction<string> OnActionCommandCompletion;

    public static UnityAction<GameObject> OnPlayerAttack;

    public static UnityAction OnPlayerBlock;

    public static UnityAction OnDisableMenu;

    public static UnityAction OnEnableMenu;
}
