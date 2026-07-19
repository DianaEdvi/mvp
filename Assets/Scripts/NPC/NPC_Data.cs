using UnityEditor.EditorTools;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC_Data", menuName = "Scriptable Objects/NPC_Data")]
public class NPC_Data : ScriptableObject
{
    public string npcName;
    [Tooltip("The list of items this npc is holding")]
    public Item[] items;

    // Include texts as well here
}