using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject targetArrow;

    [SerializeField] private Image[] actionPointsUI;

    private Color filledColor = new Color(0.8584906f, 0.8578606f, 0.1822268f);
    private Color emptyColor = new Color(0.3f, 0.3f, 0.3f);

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

    public void SetTargetArrow(bool b) {

        targetArrow.SetActive(b);

    }
}
