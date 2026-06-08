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

        combatMenu.SetActive(false);
        attackToolTip.SetActive(true);

        //enable targeting in target script
        combatTargetingScript.StartTargeting(targetingType.SingleEnemy);
    }

    public void BlockSelect()
    {
        combatMenu.SetActive(false);
        blockToolTip.SetActive(true);

        //enable targeting in target script
        combatTargetingScript.StartTargeting(targetingType.Self);
    }

    public void SkillSelect() { 
        
    }

    public void BackSelect() {

        attackToolTip.SetActive(false);
        blockToolTip.SetActive(false);
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
