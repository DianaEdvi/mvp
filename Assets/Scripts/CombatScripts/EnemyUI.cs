using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    [SerializeField] private GameObject targetArrow;

    public void SetHealthSlider(int h) {

        healthSlider.value = h;
    }

    public float GetHealthSlider() {

        return healthSlider.value;

    }
    public void SetTargetArrow(bool b) {

        targetArrow.SetActive(b); 

    }
}
