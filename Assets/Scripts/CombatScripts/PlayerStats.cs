using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private int health = 10;

    private int actionPoints = 3;

    private void OnEnable()
    {
        EventHolder.OnPlayerTurnStart += ResetActionPoints;
        EventHolder.OnPlayerTakeDamage += RemoveHealth;
    }

    private void OnDisable()
    {
        EventHolder.OnPlayerTurnStart -= ResetActionPoints;
        EventHolder.OnPlayerTakeDamage -= RemoveHealth;
    }

    public void AddHealth(int h) {

        health += h;

    }

    public void RemoveHealth(int h)
    {

        health -= h;

        if (health <= 0) {

            //EventHolder.OnPlayerDeath?.Invoke();
            Debug.Log("Player Died");

        }
    }

    public int GetActionPoints() {

        return actionPoints;

    }

    public void RemoveActionPoint() {

        actionPoints -= 1;

    }

    public void ResetActionPoints()
    {

        actionPoints = 3;

    }
}
