using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatItemDisplay : MonoBehaviour
{
    [SerializeField] private Item item;

    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;

    public void SetupButton(Item i) {

        item = i;
        itemImage.sprite = item.itemArt;
        itemName.text = item.itemName;

        gameObject.GetComponent<Button>().onClick.AddListener(StartTargeting);
    }

    //starts the targeting, just need to display the decription and queue up the trigger item method in the combat targeting script
    public void StartTargeting() {

        EventHolder.OnBeginTargeting?.Invoke(item.targetingType);

    }

}
