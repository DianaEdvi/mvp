using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    [SerializeField] private GameObject targetArrow;

    public void setHealthSlider(int h) {

        healthSlider.value = h;
    }

    public float getHealthSlider() {

        return healthSlider.value;

    }
    public void setTargetArrow(bool b) {

        targetArrow.SetActive(b); 

    }
}
