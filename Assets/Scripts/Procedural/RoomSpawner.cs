using UnityEngine;

public class RoomSpawner : MonoBehaviour
{  
    public static RoomSpawner Instance { get; private set; } // Singleton

    [Header("Grid Config")]
    [SerializeField] private int numCellsWidth = 10;
    [SerializeField] private int numCellsHeight = 10;
    [SerializeField] private float actualCellSize = 10f;

    [Header("Start Spawn Settings")]
    [SerializeField] private Room startingRoomPrefab; 
    [SerializeField] private int xIndex;
    [SerializeField] private int zIndex;

    public float NumCellsWidth => numCellsWidth;
    public float NumCellsHeight => numCellsHeight;

    // A 2D array tracking which grid cells are already full
    private bool[,] occupiedGrid;

    void Awake()
    {
        // Implement singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        occupiedGrid = new bool[numCellsWidth, numCellsHeight];
        
        // Try spawning inital room at specified grid coordinates
        TrySpawnRoom(startingRoomPrefab, xIndex, zIndex);
    }

    public bool TrySpawnRoom(Room prefab, int startX, int startZ)
    {
        // Check if the room can fit within the grid boundaries
        if (startX + prefab.roomWidthCells > numCellsWidth || 
            startZ + prefab.roomHeightCells > numCellsHeight ||
            startX < 0 || startZ < 0)
        {
            Debug.LogWarning($"Cannot spawn {prefab.name}: Out of grid boundaries.");
            return false;
        }

        // Check if the required grid cells for this room are already occupied
        for (int x = startX; x < startX + prefab.roomWidthCells; x++)
        {
            for (int z = startZ; z < startZ + prefab.roomHeightCells; z++)
            {
                if (occupiedGrid[x, z])
                {
                    Debug.LogWarning($"Cannot spawn {prefab.name}: Space at ({x},{z}) is already taken!");
                    return false;
                }
            }
        }

        // If both checks pass, compute the exact grid alignment position
        Vector3 spawnPos = new Vector3(startX * actualCellSize, 0, startZ * actualCellSize);
        Room newRoom = Instantiate(prefab, spawnPos, Quaternion.identity, this.transform);
        newRoom.name = $"Room_{startX}_{startZ}";

        // Mark cells as occupied
        for (int x = startX; x < startX + prefab.roomWidthCells; x++)
        {
            for (int z = startZ; z < startZ + prefab.roomHeightCells; z++)
            {
                occupiedGrid[x, z] = true;
            }
        }

        return true;
    }
}