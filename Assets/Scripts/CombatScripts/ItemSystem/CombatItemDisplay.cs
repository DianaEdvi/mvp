using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatItemDisplay : MonoBehaviour
{
    [SerializeField] private Item item;

    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;

    public void SetupDisplay(Item i) {

        item = i;
        itemImage.sprite = item.itemArt;
        itemName.text = item.itemName;

    }

    
}
