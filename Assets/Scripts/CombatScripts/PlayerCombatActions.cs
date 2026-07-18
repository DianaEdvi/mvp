using UnityEngine;
using System.Collections;

public class PlayerCombatActions : MonoBehaviour
{
    //reference needed to get relevant stats for attack and block actions
    [SerializeField] private PlayerStats playerStats;

    private string ACResult;

    private void OnEnable()
    {
        EventHolder.OnActionCommandCompletion += ListenForAC;
        EventHolder.OnPlayerAttack += StartAttackAction;
        EventHolder.OnPlayerBlock += StartBlockAction;
    }

    private void OnDisable()
    {
        EventHolder.OnActionCommandCompletion -= ListenForAC;
        EventHolder.OnPlayerAttack -= StartAttackAction;
        EventHolder.OnPlayerBlock -= StartBlockAction;
    }

    private void StartAttackAction(GameObject t) {
        ACResult = null;
        StartCoroutine(AttackAction(t));

        EventHolder.OnRemoveActionPoint?.Invoke();
    }

    private IEnumerator AttackAction(GameObject t) {

        EventHolder.OnDisableMenu?.Invoke();
        EventHolder.OnTriggerCircleActionCommand?.Invoke();

        Debug.Log("waiting for command completion");

        //listen using method below until action command is completed
        yield return new WaitUntil(() => ACResult != null);

        switch (ACResult) {

            case "Perfect":
                t.GetComponent<EnemyStats>().TakeDamage(playerStats.GetDamage()*2);
                break;

            case "Great":
                t.GetComponent<EnemyStats>().TakeDamage((int)Mathf.Ceil(playerStats.GetDamage() * 1.5f));
                break;

            case "Good":
                t.GetComponent<EnemyStats>().TakeDamage(playerStats.GetDamage());
                break;

            case "Miss":
                t.GetComponent<EnemyStats>().TakeDamage((int) (playerStats.GetDamage() / 2));
                break;

        }

        Debug.Log("Dealt damage to " +t.name);

        EventHolder.OnEnableMenu?.Invoke();
    }

    private void StartBlockAction() {
        ACResult = null;
        StartCoroutine(BlockAction());

        EventHolder.OnRemoveActionPoint?.Invoke();
    }

    private IEnumerator BlockAction() {

        EventHolder.OnDisableMenu?.Invoke();
        EventHolder.OnTriggerCircleActionCommand?.Invoke();

        yield return new WaitUntil(() => ACResult != null);

        switch (ACResult)
        {

            case "Perfect":
                EventHolder.OnPlayerGainBlock?.Invoke(playerStats.GetBlockGain() * 2);
                break;

            case "Great":
                EventHolder.OnPlayerGainBlock?.Invoke((int)Mathf.Ceil(playerStats.GetBlockGain() * 1.5f));
                break;

            case "Good":
                EventHolder.OnPlayerGainBlock?.Invoke(playerStats.GetBlockGain());
                break;

            case "Miss":
                EventHolder.OnPlayerGainBlock?.Invoke(playerStats.GetBlockGain() / 2);
                break;

        }

        EventHolder.OnEnableMenu?.Invoke();
    }

    private void ListenForAC(string s) {

        ACResult = s;

    }
}
