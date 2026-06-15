using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject targetArrow;

    [SerializeField] private Image[] actionPointsUI;

    [SerializeField] private Slider healthSlider;

    private Color filledColor = new Color(0.8584906f, 0.8578606f, 0.1822268f);
    private Color emptyColor = new Color(0.3f, 0.3f, 0.3f);

    private void OnEnable()
    {
        EventHolder.OnPlayerTurnStart += ResetActionPoints;
        EventHolder.OnPlayerTakeDamage += removeHealth;
    }

    private void OnDisable()
    {
        EventHolder.OnPlayerTurnStart -= ResetActionPoints;
        EventHolder.OnPlayerTakeDamage -= removeHealth;
    }

    private void removeHealth(int h) {
        healthSlider.value -= h;
    }

    private void addHealth(int h)
    {
        healthSlider.value += h;
    }


    public void RefillActionPoints() {

        foreach (Image actionPointImage in actionPointsUI)
        {

            actionPointImage.color = filledColor;

        }
    }

    public void RemoveActionPoint() {

        for (int i = 0; i < 3; i++) {

            if (actionPointsUI[i].color == filledColor)
            {
                Debug.Log("Changed colour at " + i);
                actionPointsUI[i].color = emptyColor;
                break;
            }
            else {
                Debug.Log("Already empty at " +i);
            }

        }
    }

    private void ResetActionPoints() {

        for (int i = 0; i < 3; i++)
        {
            actionPointsUI[i].color = filledColor;
        }
    }

    public void SetTargetArrow(bool b) {

        targetArrow.SetActive(b);

    }
}
