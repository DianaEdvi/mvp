using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CombatTargeting : MonoBehaviour
{
    [SerializeField] private GameObject Player;

    //both need to shrink and grow dynamically since you could target multiple enemies and enemies can die
    private List<GameObject> Enemies = new List<GameObject>();
    private GameObject currentTarget;

    //never really worked with enums before so for now I'm setting targeting as 0 = self, 1 = single enemy, 2 = all enemies

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

                currentTarget.GetComponent<EnemyUI>().setTargetArrow(false);
                currentTarget = Enemies[currentTargetIndex-1];
                currentTarget.GetComponent<EnemyUI>().setTargetArrow(true);

            }

            if (Keyboard.current.rightArrowKey.wasPressedThisFrame && Enemies.IndexOf(currentTarget) < (Enemies.Count-1))
            {

                //get index of current target in enemies list, then disable ui and swap current target to new enemy and enable their ui
                int currentTargetIndex = Enemies.IndexOf(currentTarget);

                currentTarget.GetComponent<EnemyUI>().setTargetArrow(false);
                currentTarget = Enemies[currentTargetIndex+1];
                currentTarget.GetComponent<EnemyUI>().setTargetArrow(true);

            }

            if (Keyboard.current.enterKey.wasPressedThisFrame) {

                EventHolder.OnPlayerAttack?.Invoke(currentTarget);

            }
        }
    }

    public void startTargeting(int t) {

        switch (t) {

            case 0:

                Player.GetComponent<PlayerUI>().setTargetArrow(true);
                currentTarget = Player;

                break;

            case 1:

                Enemies[0].GetComponent<EnemyUI>().setTargetArrow(true);
                currentTarget = Enemies[0];
                targetSwapping = true;

                break;

            case 2:

                //find out in a sec

                break;

        }
    }

    public void swapEnemyTarget() { 
        


    }

    public void resetTargeting() {

        foreach (GameObject enemy in Enemies) {

            enemy.GetComponent<EnemyUI>().setTargetArrow(false);

        }

        Player.GetComponent<PlayerUI>().setTargetArrow(false);

        targetSwapping = false;

    }
}
