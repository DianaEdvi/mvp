using UnityEngine;
using System.Collections;

public class PlayerCombatActions : MonoBehaviour
{
    private string ACResult;

    private int damage = 2;

    private void OnEnable()
    {
        EventHolder.OnActionCommandCompletion += listenForAC;
        EventHolder.OnPlayerAttack += startAttackAction;
    }

    private void OnDisable()
    {
        EventHolder.OnActionCommandCompletion -= listenForAC;
        EventHolder.OnPlayerAttack -= startAttackAction;
    }

    private void startAttackAction(GameObject t) {
        ACResult = null;
        StartCoroutine(AttackAction(t));
    }

    private IEnumerator AttackAction(GameObject t) {

        EventHolder.OnDisableMenu?.Invoke();
        EventHolder.OnTriggerCircleActionCommand?.Invoke();

        Debug.Log("waiting for command completion");

        //listen using method below until action command is completed
        yield return new WaitUntil(() => ACResult != null);

        switch (ACResult) {

            case "Perfect":
                t.GetComponent<EnemyStats>().takeDamage(damage*2);
                break;

            case "Great":
                t.GetComponent<EnemyStats>().takeDamage((int)Mathf.Ceil(damage * 1.5f));
                break;

            case "Good":
                t.GetComponent<EnemyStats>().takeDamage(damage);
                break;

            case "Miss":
                t.GetComponent<EnemyStats>().takeDamage((int) (damage / 2));
                break;

        }

        Debug.Log("Dealt damage to " +t.name);

        EventHolder.OnEnableMenu?.Invoke();
    }

    private void listenForAC(string s) {

        ACResult = s;

    }
}
