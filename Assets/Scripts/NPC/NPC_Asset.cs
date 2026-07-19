using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NPC_Asset : MonoBehaviour
{
    public NPC_Data npcData;
    [SerializeField] private Image interactPopup;
    [SerializeField] private float displayRange = 3f;

    private string npcName;

    void Start()
    {
        if (npcData != null) npcName = npcData.npcName;
        if (interactPopup != null) interactPopup.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Player.Instance != null && interactPopup != null)
        {
            // Calculate the distance between the NPC and the Player
            float distance = Vector3.Distance(transform.position, Player.Instance.transform.position);

            bool isWithinRange = distance <= displayRange;

            if (interactPopup.gameObject.activeSelf != isWithinRange)
            {
                interactPopup.gameObject.SetActive(isWithinRange);
            }
        }
    }

    public void Interact()
    {
        Debug.Log($"Interacting with NPC: {npcName}");
        // play audio
    }

    void LateUpdate()
    {
        // Only force the rotation math if the image is actually visible
        if (interactPopup != null && interactPopup.gameObject.activeSelf)
        {
            interactPopup.gameObject.transform.rotation = Camera.main.transform.rotation;
        }
    }
}