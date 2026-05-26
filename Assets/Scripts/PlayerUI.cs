using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject targetArrow;

    public void setTargetArrow(bool b) {

        targetArrow.SetActive(b);

    }
}
