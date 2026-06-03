using UnityEngine;

public class Room : MonoBehaviour
{
    // This script is attached to each room prefab and defines the layout of the room in terms of which grid cells it occupies and where its doors are located.
    // The local coordinates begin at (0,0) and the pivot point of the transform component is at the bottom left of cell (0,0).

    [Tooltip("The local grid coordinates of each floor cell in the room, relative to the room's pivot point")]
    public Vector2Int[] localCellCoordinates;

     [Tooltip("The door sockets in the room where new rooms can be attached. Each socket should be in the center of a cell edge and its z forward vector should point outward from the room.")]
    public Transform[] doorSockets;
}