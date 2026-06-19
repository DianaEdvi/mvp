using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private void Start()
    {
        StartPlayerTurn();       
    }

    public void StartPlayerTurn() {

        EventHolder.OnPlayerTurnStart?.Invoke();
        EventHolder.OnEnableMenu?.Invoke();

    }

    public void StartEnemyTurn() {

        EventHolder.OnEnemyTurnStart?.Invoke();
        EventHolder.OnDisableMenu?.Invoke();

    }

}
