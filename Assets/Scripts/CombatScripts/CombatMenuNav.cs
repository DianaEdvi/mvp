using UnityEngine;
using UnityEngine.InputSystem;

public class CombatMenuNav : MonoBehaviour
{
    [SerializeField] private GameObject combatMenu;

    [SerializeField] private GameObject attackToolTip;
    [SerializeField] private GameObject blockToolTip;
    [SerializeField] private GameObject itemMenu;

    //add nested list for skills 

    //add variable for target handling script, enable in relevant methods
    [SerializeField] private CombatTargeting combatTargetingScript;

    [SerializeField] private PlayerStats playerStats;

    private void OnEnable()
    {
        EventHolder.OnEnableMenu += EnableMenu;
        EventHolder.OnDisableMenu += DisableMenu;
    }

    private void OnDisable()
    {
        EventHolder.OnEnableMenu -= EnableMenu;
        EventHolder.OnDisableMenu -= DisableMenu;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasReleasedThisFrame) {

            BackSelect();

        }
    }

    public void AttackSelect() {

        if (playerStats.GetActionPoints() > 0)
        {
            combatMenu.SetActive(false);
            attackToolTip.SetActive(true);

            //enable targeting in target script
            combatTargetingScript.StartTargeting(TargetingType.SingleEnemy);
        }
        else {

            Debug.Log("No more action points!");

        }
    }

    public void BlockSelect()
    {
        if (playerStats.GetActionPoints() > 0)
        {
            combatMenu.SetActive(false);
            blockToolTip.SetActive(true);

            //enable targeting in target script
            combatTargetingScript.StartTargeting(TargetingType.Self);
        }
        else {
            Debug.Log("No more action points!");
        }
    }

    public void SkillSelect() { 
        
        //implement player skills list then can use this

    }

    public void ItemSelect() {

        if (playerStats.GetActionPoints() > 0)
        {
            combatMenu.SetActive(false);
            itemMenu.SetActive(true);
        }
        else
        {
            Debug.Log("No more action points!");
        }

    }

    public void BackSelect() {

        attackToolTip.SetActive(false);
        blockToolTip.SetActive(false);
        itemMenu.SetActive(false);
        //add skill list active here

        combatTargetingScript.ResetTargeting();

        combatMenu.SetActive(true);

    }

    public void DisableMenu() {

        BackSelect();
        combatMenu.SetActive(false);

    }

    public void EnableMenu() {

        combatMenu.SetActive(true);

    }
}
