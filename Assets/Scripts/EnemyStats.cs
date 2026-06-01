using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    private int health = 10;

    //private int damage = 5;

    [SerializeField] private EnemyUI enemyUI;

    public void takeDamage(int h) {

        health -= h;

        enemyUI.setHealthSlider((int) enemyUI.getHealthSlider() - h);

        if (health <= 0) {

            //Die event goes here but just destroy for now
            Destroy(gameObject);

        }
    }
}
