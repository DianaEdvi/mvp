using UnityEngine;
using UnityEngine.InputSystem;

public class ActionCommandSpawner : MonoBehaviour
{
    [SerializeField] private RectTransform ActionSpawnArea;
    [SerializeField] private GameObject ActionCommand;

    private void OnEnable()
    {
        EventHolder.OnTriggerCircleActionCommand += spawnCommand;
    }

    private void OnDisable()
    {
        EventHolder.OnTriggerCircleActionCommand -= spawnCommand;
    }

    public void spawnCommand()
    {

        float x = Random.Range(-ActionSpawnArea.rect.width / 2, ActionSpawnArea.rect.width / 2);
        float y = Random.Range(-ActionSpawnArea.rect.height / 2, ActionSpawnArea.rect.height / 2);

        GameObject actionCommand = Instantiate(ActionCommand, ActionSpawnArea);
        actionCommand.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
    }
}
