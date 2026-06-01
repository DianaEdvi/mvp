using UnityEngine;

public class Room : MonoBehaviour
{

    [Header("Grid Dimensions")]
    [Tooltip("How many grid units wide (X axis) is this room layout?")]
    public int roomWidthCells = 1;
    [Tooltip("How many grid units deep (Z axis) is this room layout?")]
    public int roomHeightCells = 1;

    [Tooltip("The local grid coordinates of each floor cell in the room, relative to the room's pivot point")]
    public Vector2Int[] localCellCoordinates;

    [Header("Exits")]
    public Transform[] doorSockets;
}