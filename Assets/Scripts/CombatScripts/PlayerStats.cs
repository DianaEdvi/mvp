using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    private int currentBlock = 0;

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
            SceneManager.LoadScene("Game"); // Temp

        }
    }

    public void AddBlock(int b) {

        currentBlock += b;

    }

    public void RemoveBlock(int b) {
        
        //subtract but make sure we can't go below zero
        currentBlock -= b;
        currentBlock = Mathf.Clamp(currentBlock, 0, 1000000);

        if (currentBlock <= 0) {

            EventHolder.OnBreakBlock?.Invoke();

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
