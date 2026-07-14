using UnityEngine;

[System.Serializable]
public class RoomSpawnConfig
{
    [Tooltip("The type of room you want to spawn based on its tags")]
    public RoomTags requiredTag;

    [Tooltip("How many of this specific room type to place in the dungeon")]
    public int count;
}