using UnityEngine;
using System.Collections;

public class PlayerCombatActions : MonoBehaviour
{
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private PlayerStats playerStats;

    private string ACResult;

    private int damage = 2;

    private void OnEnable()
    {
        EventHolder.OnActionCommandCompletion += ListenForAC;
        EventHolder.OnPlayerAttack += StartAttackAction;
    }

    private void OnDisable()
    {
        EventHolder.OnActionCommandCompletion -= ListenForAC;
        EventHolder.OnPlayerAttack -= StartAttackAction;
    }

    private void StartAttackAction(GameObject t) {
        ACResult = null;
        StartCoroutine(AttackAction(t));

        playerUI.RemoveActionPoint();
        playerStats.RemoveActionPoint();
    }

    private IEnumerator AttackAction(GameObject t) {

        EventHolder.OnDisableMenu?.Invoke();
        EventHolder.OnTriggerCircleActionCommand?.Invoke();

        Debug.Log("waiting for command completion");

        //listen using method below until action command is completed
        yield return new WaitUntil(() => ACResult != null);

        switch (ACResult) {

            case "Perfect":
                t.GetComponent<EnemyStats>().TakeDamage(damage*2);
                break;

            case "Great":
                t.GetComponent<EnemyStats>().TakeDamage((int)Mathf.Ceil(damage * 1.5f));
                break;

            case "Good":
                t.GetComponent<EnemyStats>().TakeDamage(damage);
                break;

            case "Miss":
                t.GetComponent<EnemyStats>().TakeDamage((int) (damage / 2));
                break;

        }

        Debug.Log("Dealt damage to " +t.name);

        EventHolder.OnEnableMenu?.Invoke();
    }

    private void ListenForAC(string s) {

        ACResult = s;

    }
}
