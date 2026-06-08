using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public enum targetingType {Self, SingleEnemy, AllEnemy}

public class CombatTargeting : MonoBehaviour
{
    [SerializeField] private GameObject Player;

    //both need to shrink and grow dynamically since you could target multiple enemies and enemies can die
    private List<GameObject> Enemies = new List<GameObject>();
    private GameObject currentTarget;

    private bool targetSwapping = false;

    private void Start()
    {

        Transform enemyHolder = GameObject.FindGameObjectWithTag("EnemyHolder").transform;
        
        foreach (Transform enemyChild in enemyHolder) {

            Enemies.Add(enemyChild.gameObject);

        }
    }

    private void Update()
    {
        if (targetSwapping && Keyboard.current != null) {

            if (Keyboard.current.leftArrowKey.wasPressedThisFrame && currentTarget != Enemies[0]) {

                //get index of current target in enemies list, then disable ui and swap current target to new enemy and enable their ui
                int currentTargetIndex = Enemies.IndexOf(currentTarget);

                currentTarget.GetComponent<EnemyUI>().SetTargetArrow(false);
                currentTarget = Enemies[currentTargetIndex-1];
                currentTarget.GetComponent<EnemyUI>().SetTargetArrow(true);

            }

            if (Keyboard.current.rightArrowKey.wasPressedThisFrame && Enemies.IndexOf(currentTarget) < (Enemies.Count-1))
            {

                //get index of current target in enemies list, then disable ui and swap current target to new enemy and enable their ui
                int currentTargetIndex = Enemies.IndexOf(currentTarget);

                currentTarget.GetComponent<EnemyUI>().SetTargetArrow(false);
                currentTarget = Enemies[currentTargetIndex+1];
                currentTarget.GetComponent<EnemyUI>().SetTargetArrow(true);

            }

            if (Keyboard.current.enterKey.wasPressedThisFrame) {

                EventHolder.OnPlayerAttack?.Invoke(currentTarget);

            }
        }
    }

    private void OnEnable()
    {
        EventHolder.OnEnemyDeath += RemoveEnemy;
    }

    private void OnDisable()
    {
        EventHolder.OnEnemyDeath -= RemoveEnemy;
    }

    public void StartTargeting(targetingType t) {

        switch (t) {

            case targetingType.Self:

                Player.GetComponent<PlayerUI>().SetTargetArrow(true);
                currentTarget = Player;

                break;

            case targetingType.SingleEnemy:

                Enemies[0].GetComponent<EnemyUI>().SetTargetArrow(true);
                currentTarget = Enemies[0];
                targetSwapping = true;

                break;

            case targetingType.AllEnemy:

                //find out in a sec

                break;

        }
    }

    public void ResetTargeting() {

        foreach (GameObject enemy in Enemies) {

            enemy.GetComponent<EnemyUI>().SetTargetArrow(false);

        }

        Player.GetComponent<PlayerUI>().SetTargetArrow(false);

        targetSwapping = false;

    }

    private void RemoveEnemy(GameObject e) {

        Enemies.Remove(e);
        Debug.Log("Removed " + e.name);

    }
}
