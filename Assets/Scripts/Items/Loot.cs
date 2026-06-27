using UnityEngine;

public class Loot : MonoBehaviour, IInteractible
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact()
    {
        Debug.Log($"Interacted with {gameObject.name} loot");

    }
}
