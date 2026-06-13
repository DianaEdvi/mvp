using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject targetArrow;

    public void SetTargetArrow(bool b) {

        targetArrow.SetActive(b);

    }
}
