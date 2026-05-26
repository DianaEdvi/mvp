using UnityEngine;
using UnityEngine.InputSystem;

public class CombatMenuNav : MonoBehaviour
{
    [SerializeField] private GameObject combatMenu;

    [SerializeField] private GameObject attackToolTip;
    [SerializeField] private GameObject blockToolTip;

    //add nested list for skills 

    //add variable for target handling script, enable in relevant methods
    [SerializeField] private CombatTargeting combatTargetingScript;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasReleasedThisFrame) {

            backSelect();

        }
    }

    public void attackSelect() {

        combatMenu.SetActive(false);
        attackToolTip.SetActive(true);

        //enable targeting in target script
        combatTargetingScript.startTargeting(1);
    }

    public void blockSelect()
    {
        combatMenu.SetActive(false);
        blockToolTip.SetActive(true);

        //enable targeting in target script
        combatTargetingScript.startTargeting(0);
    }

    public void skillSelect() { 
    
    }

    public void backSelect() {

        attackToolTip.SetActive(false);
        blockToolTip.SetActive(false);
        //add skill list active here

        combatTargetingScript.resetTargeting();

        combatMenu.SetActive(true);

    }
}
