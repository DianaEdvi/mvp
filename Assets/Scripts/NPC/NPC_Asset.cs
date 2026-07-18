using UnityEngine;

public class NPC_Asset : MonoBehaviour
{
    public NPC_Data npcData;

    private string npcName;

    void Start()
    {
        if (npcData != null)
        {
            npcName = npcData.npcName;
        }
    }

    public void Interact()
    {
        Debug.Log($"Interacting with NPC: {npcName}");
        // Add interaction logic here, e.g., dialogue
    }
}
