using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    private int currentBlock = 0;

    private int blockGain = 2;

    private int health = 10;

    private int actionPoints = 3;

    private int damage = 2;

    private void OnEnable()
    {
        EventHolder.OnPlayerTurnStart += ResetActionPoints;
        EventHolder.OnPlayerTurnStart += ResetBlock;
        EventHolder.OnPlayerTakeDamage += HitPlayer;
        EventHolder.OnRemoveActionPoint += RemoveActionPoint;
        EventHolder.OnPlayerGainBlock += AddBlock;
    }

    private void OnDisable()
    {
        EventHolder.OnPlayerTurnStart -= ResetActionPoints;
        EventHolder.OnPlayerTurnStart -= ResetBlock;
        EventHolder.OnPlayerTakeDamage -= HitPlayer;
        EventHolder.OnRemoveActionPoint -= RemoveActionPoint;
        EventHolder.OnPlayerGainBlock -= AddBlock;
    }

    public int GetDamage() {

        return damage;

    }

    public void AddHealth(int h) {

        health += h;

    }

    //this is the encompassing "player gets hit" method, the below remove health and remove block methods
    //serve to fine tune when needed, like if the player is poisoned or their block is eroded
    private void HitPlayer(int d)
    {

        if (currentBlock > 0 && d > currentBlock)
        {

            //deal excess damage
            RemoveHealth(d - currentBlock);

            //break all block
            RemoveBlock(currentBlock);

        }
        else if (currentBlock == 0)
        {

            RemoveHealth(d);

        }
        else if (d <= currentBlock) {

            RemoveBlock(d);

        }

    }

    public void RemoveHealth(int d)
    {

        health -= d;

        EventHolder.OnPlayerRemoveHealth?.Invoke(d);

        if (health <= 0)
        {

            //EventHolder.OnPlayerDeath?.Invoke();
            Debug.Log("Player Died");
            SceneManager.LoadScene("Game"); // Temp

        }
    }

    public void AddBlock(int b) {

        currentBlock += b;
        Debug.Log(currentBlock);
    }

    public void RemoveBlock(int b) {
        
        //subtract but make sure we can't go below zero
        currentBlock -= b;
        currentBlock = Mathf.Clamp(currentBlock, 0, 999);

        if (currentBlock <= 0) {

            EventHolder.OnDisableBlockUI?.Invoke();

        }
    }

    private void ResetBlock() {

        currentBlock = 0;
        EventHolder.OnDisableBlockUI?.Invoke();

    }

    public int GetCurrentBlock() {

        return currentBlock;

    }

    public int GetBlockGain() {

        return blockGain;

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
