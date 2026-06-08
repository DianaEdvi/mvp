using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CombatTransition : MonoBehaviour
{

    //This update block is only for testing the scene transition, event invoking will be done in the combat collider
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) {

            LoadCombat();

        }
    }

    private void LoadCombat() {

        SceneManager.LoadScene("CombatPrototype");

    }

}
