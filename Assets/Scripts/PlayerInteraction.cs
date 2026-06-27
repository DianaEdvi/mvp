using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object we collided with has the IInteractible interface
        IInteractible interactibleObject = other.GetComponentInParent<IInteractible>();

        // If it does, call its Interact method!
        if (interactibleObject != null)
        {
            interactibleObject.Interact();
        }
    }
}