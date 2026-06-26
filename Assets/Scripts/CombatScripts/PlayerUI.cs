using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject targetArrow;

    [SerializeField] private Image[] actionPointsUI;

    [SerializeField] private Slider healthSlider;

    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private GameObject blockIndicator;

    [SerializeField] private TextMeshProUGUI blockText;

    //need this reference to stats script to add block, otherwise it's weird since the ui and numbers get updated in parallel via the same event
    //maybe it's a band-aid, maybe it's just how it works now, who knows

    [SerializeField] private PlayerStats playerStats;

    private Color filledColor = new Color(0.8584906f, 0.8578606f, 0.1822268f);
    private Color emptyColor = new Color(0.3f, 0.3f, 0.3f);

    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    private void OnEnable()
    {
        EventHolder.OnPlayerTurnStart += ResetActionPoints;
        EventHolder.OnPlayerRemoveHealth += RemoveHealth;
        EventHolder.OnPlayerTakeDamage += RemoveBlockUI;
        EventHolder.OnRemoveActionPoint += RemoveActionPoint;
        EventHolder.OnPlayerGainBlock += AddBlockUI;
        EventHolder.OnDisableBlockUI += DisableBlockUI;
    }

    private void OnDisable()
    {
        EventHolder.OnPlayerTurnStart -= ResetActionPoints;
        EventHolder.OnPlayerRemoveHealth -= RemoveHealth;
        EventHolder.OnPlayerTakeDamage += RemoveBlockUI;
        EventHolder.OnRemoveActionPoint -= RemoveActionPoint;
        EventHolder.OnDisableBlockUI -= DisableBlockUI;
    }

    private void RemoveHealth(int h) {
        healthSlider.value -= h;
        healthText.text = healthSlider.value +"/10";
    }

    private void addHealth(int h)
    {
        healthSlider.value += h;
        healthText.text = healthSlider.value + "/10";
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
                actionPointsUI[i].color = emptyColor;
                break;
            }
        }
    }

    private void ResetActionPoints() {

        for (int i = 0; i < 3; i++)
        {
            actionPointsUI[i].color = filledColor;
        }
    }

    private void AddBlockUI(int b) {

        blockIndicator.SetActive(true);
        blockText.text = "" + playerStats.GetCurrentBlock();

    }

    private void RemoveBlockUI(int b) {

        blockText.text = "" + playerStats.GetCurrentBlock();

    }

    private void DisableBlockUI() {

        blockText.text = "0";
        blockIndicator.SetActive(false);

    }

    public void SetTargetArrow(bool b) {

        targetArrow.SetActive(b);

    }
}
