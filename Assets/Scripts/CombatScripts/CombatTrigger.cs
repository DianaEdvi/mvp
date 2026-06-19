using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CombatTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2Int coords = new Vector2Int(Mathf.FloorToInt(other.gameObject.transform.position.x / GameManager.Instance.Cellsize), Mathf.FloorToInt(other.gameObject.transform.position.z / GameManager.Instance.Cellsize));
            GameManager.Instance.OnCombatTriggered.Invoke(coords);
            SceneManager.LoadScene("CombatPrototype");
        }
    }
}
