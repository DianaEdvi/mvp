using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public enum TargetingType {None, Self, SingleEnemy, AllEnemy}

public class CombatTargeting : MonoBehaviour
{
    [SerializeField] private GameObject Player;

    //both need to shrink and grow dynamically since you could target multiple enemies and enemies can die
    private List<GameObject> Enemies = new List<GameObject>();
    private GameObject currentTarget;

    private TargetingType targetingType = TargetingType.None;
    
    private void Start()
    {

        Transform enemyHolder = GameObject.FindGameObjectWithTag("EnemyHolder").transform;
        
        foreach (Transform enemyChild in enemyHolder) {

            Enemies.Add(enemyChild.gameObject);

        }
    }

    private void Update()
    {
        if (targetingType == TargetingType.SingleEnemy && Keyboard.current != null) {

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

            if (Keyboard.current.spaceKey.wasPressedThisFrame) {

                //only able to attack with single enemy targeting right now? Need a variable for what the player is targeting WITH
                //Add that when skills are implemented

                EventHolder.OnPlayerAttack?.Invoke(currentTarget);

            }
        }

        if (targetingType == TargetingType.Self && Keyboard.current != null) {

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {

                EventHolder.OnPlayerBlock?.Invoke();

            }

        }
    }

    private void OnEnable()
    {
        EventHolder.OnEnemyDeath += RemoveEnemy;
        EventHolder.OnBeginTargeting += StartTargeting;
    }

    private void OnDisable()
    {
        EventHolder.OnEnemyDeath -= RemoveEnemy;
        EventHolder.OnBeginTargeting -= StartTargeting;
    }

    public void StartTargeting(TargetingType t) {

        targetingType = t;

        switch (t) {

            case TargetingType.Self:

                Player.GetComponent<PlayerUI>().SetTargetArrow(true);
                currentTarget = Player;

                break;

            case TargetingType.SingleEnemy:

                Enemies[0].GetComponent<EnemyUI>().SetTargetArrow(true);
                currentTarget = Enemies[0];

                break;

            case TargetingType.AllEnemy:

                //find out in a sec

                break;

        }
    }

    public void ResetTargeting() {

        foreach (GameObject enemy in Enemies) {

            enemy.GetComponent<EnemyUI>().SetTargetArrow(false);

        }

        Player.GetComponent<PlayerUI>().SetTargetArrow(false);

        targetingType = TargetingType.None;

        currentTarget = null;
    }

    private void RemoveEnemy(GameObject e) {

        Enemies.Remove(e);
        Debug.Log("Removed " + e.name);

    }
}
