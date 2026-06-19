using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> Enemies;

    private void OnEnable()
    {
        EventHolder.OnEnemyTurnStart += StartEnemyTurn;
        EventHolder.OnEnemyDeath += RemoveEnemy;
    }
    private void OnDisable()
    {
        EventHolder.OnEnemyTurnStart -= StartEnemyTurn;
        EventHolder.OnEnemyDeath -= RemoveEnemy;

    }

    private void StartEnemyTurn() {

        StartCoroutine(EnemyTurn());

    }

    private void RemoveEnemy(GameObject g) {

        if (Enemies.Contains(g)) {

            Enemies.Remove(g);

        }
    }

    private IEnumerator EnemyTurn() {

        for (int i = 0; i < Enemies.Count; i++) {

            Debug.Log("Taking damage from " +Enemies[i].name);
            yield return new WaitForSeconds(2f);

            //get enemy's damage stat and apply to player
            int damage = Enemies[i].GetComponent<EnemyStats>().GetEnemyDamage();

            EventHolder.OnPlayerTakeDamage?.Invoke(damage);
        }

        //then go back to player's turn 
        EventHolder.OnPlayerTurnStart?.Invoke();
        EventHolder.OnEnableMenu?.Invoke();
    }
}
