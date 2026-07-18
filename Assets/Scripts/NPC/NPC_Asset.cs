using UnityEngine;
using UnityEngine.UI;

public class NPC_Asset : MonoBehaviour
{
    public NPC_Data npcData;
    [SerializeField] private Image image;

    [Header("Interaction UI")]
    [SerializeField] private float displayRange = 5f;

    private string npcName;

    void Start()
    {
        if (npcData != null)
        {
            npcName = npcData.npcName;
        }

        if (image != null)
        {
            image.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Player.Instance != null && image != null)
        {
            // Calculate the distance between the NPC and the Player
            float distance = Vector3.Distance(transform.position, Player.Instance.transform.position);

            bool isWithinRange = distance <= displayRange;

            if (image.gameObject.activeSelf != isWithinRange)
            {
                image.gameObject.SetActive(isWithinRange);
            }
        }
    }

    public void Interact()
    {
        Debug.Log($"Interacting with NPC: {npcName}");
        // Add interaction logic here, e.g., dialogue
    }

    void LateUpdate()
    {
        // Only force the rotation math if the image is actually visible
        if (image != null && image.gameObject.activeSelf)
        {
            image.gameObject.transform.rotation = Camera.main.transform.rotation;
        }
    }
}