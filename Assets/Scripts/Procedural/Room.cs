using UnityEngine;

public class Room : MonoBehaviour
{
    // This script is attached to each room prefab and defines the layout of the room in terms of which grid cells it occupies and where its doors are located.
    // The local coordinates begin at (0,0) and the pivot point of the transform component is at the bottom left of cell (0,0).

    public float cellSize = 10f;
    [Tooltip("The local grid coordinates of each floor cell in the room, relative to the room's pivot point")]
    // public Vector2Int[] localCellCoordinates;

    public Transform origin;
    [SerializeField] private Transform[] cellTransforms;
    [SerializeField] private Vector2Int[] relativeCoordinatesToOrigin;

     [Tooltip("The door sockets in the room where new rooms can be attached. Each socket should be in the center of a cell edge and its z forward vector should point outward from the room.")]
    public Transform[] doorSockets;

    public Vector2Int[] RelativeCoordinatesToOrigin => relativeCoordinatesToOrigin;
    void Start()
    {
        relativeCoordinatesToOrigin = new Vector2Int[cellTransforms.Length];
        CalculateRelativeCoordinatesToOrigin(origin, cellTransforms);
        
    }

    private void CalculateRelativeCoordinatesToOrigin(Transform originTransform, Transform[] cellTransforms)
    {

        Vector2Int originInWorldCoords = new Vector2Int(Mathf.FloorToInt(originTransform.position.x / cellSize), Mathf.FloorToInt(originTransform.position.z / cellSize));
        Vector2Int[] cellsInWorldCoords = new Vector2Int[cellTransforms.Length];

        for (int i = 0; i < cellTransforms.Length; i++)
        {
            Vector3 globalPos = cellTransforms[i].position;
            int x = Mathf.FloorToInt(globalPos.x / cellSize);
            int z = Mathf.FloorToInt(globalPos.z / cellSize);
            cellsInWorldCoords[i] = new Vector2Int(x, z);
            Debug.Log($"Cell {i} global position: ({globalPos.x}, {globalPos.z}) => world grid coordinates: ({cellsInWorldCoords[i].x}, {cellsInWorldCoords[i].y})");
        }

        relativeCoordinatesToOrigin = new Vector2Int[cellTransforms.Length];
        for (int i = 0; i < cellTransforms.Length; i++)
        {
            Debug.Log($"Calculating relative coordinates for cell {i}: origin in world coords ({originInWorldCoords.x}, {originInWorldCoords.y}) - cell in world coords ({cellsInWorldCoords[i].x}, {cellsInWorldCoords[i].y})");
            relativeCoordinatesToOrigin[i] =  cellsInWorldCoords[i] - originInWorldCoords;
        }
    }
}