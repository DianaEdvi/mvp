using UnityEngine;

public class RoomSpawner : MonoBehaviour
{  
    [SerializeField] private int numCellsPerSide;
    private float gridWidth;
    private float gridHeight;
    [SerializeField] private int xIndex;
    [SerializeField] private int zIndex;
    [SerializeField] private Room room; 
    [SerializeField] private float cellSize = 10f;

    void Start()
    {
        gridWidth = numCellsPerSide * cellSize;
        gridHeight = numCellsPerSide * cellSize;

        // Spawn the room
        Vector3 spawnPos = new Vector3(xIndex * cellSize, 0, zIndex * cellSize);
        Room instantiatedRoom = Instantiate(room, spawnPos, Quaternion.identity, this.transform);

        // Get the dimensions of the instantiated room
        float roomWidth = instantiatedRoom.LongestWidth;
        float roomHeight = instantiatedRoom.LongestHeight;

        Debug.Log($"RoomSpawner Start: Real Room width={roomWidth}, height={roomHeight}");

        // Check if the room fits within the grid boundaries
        if (xIndex * cellSize + roomWidth > gridWidth || zIndex * cellSize + roomHeight > gridHeight)
        {
            Debug.LogError($"RoomSpawner Start: Spawned Room out of bounds! Destoying instance.");
            Destroy(instantiatedRoom.gameObject);
            return;
        }
    }
}