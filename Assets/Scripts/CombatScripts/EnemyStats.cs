using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    private int health = 10;

    //private int damage = 5;

    [SerializeField] private EnemyUI enemyUI;

    public void TakeDamage(int h) {

        health -= h;

        enemyUI.SetHealthSlider((int) enemyUI.GetHealthSlider() - h);

        if (health <= 0) {

            //Trigger die event and then destroy game object
            EventHolder.OnEnemyDeath?.Invoke(gameObject);
            Destroy(gameObject);

        }
    }
}
