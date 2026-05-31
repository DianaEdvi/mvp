using UnityEngine;

using UnityEngine.InputSystem;


public class RoomSpawner : MonoBehaviour
{  
    public static RoomSpawner Instance { get; private set; } // Singleton

    private InputAction jumpAction; // temp


    [Header("Grid Config")]
    [SerializeField] private int numCellsWidth = 10;
    [SerializeField] private int numCellsHeight = 10;
    [SerializeField] private float actualCellSize = 10f;

    [Header("Start Spawn Settings")]
    [SerializeField] private Room startingRoomPrefab; 
    [SerializeField] private int xIndex;
    [SerializeField] private int zIndex;

    [Header("Rooms Settings")]
    [SerializeField] private Room[] roomPrefabs;
    [SerializeField] private int numRoomsToSpawn = 5;
    private Transform availableDoors;

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
        jumpAction = InputSystem.actions.FindAction("Jump"); // temp
    }

    void Start()
    {
        occupiedGrid = new bool[numCellsWidth, numCellsHeight];
        
        // Try spawning inital room at specified grid coordinates
        TrySpawnRoom(startingRoomPrefab, xIndex, zIndex);
    }

    void Update()
    {
        // temp

        if (jumpAction.WasPressedThisFrame())
        {
            for (int i = 0; i < numRoomsToSpawn; i++)
            {
                Room randomRoom = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
                int randomX = Random.Range(0, numCellsWidth);
                int randomZ = Random.Range(0, numCellsHeight);
                TrySpawnRoom(randomRoom, randomX, randomZ);
            }
        }
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

        for (int i = 0; i < newRoom.doorSockets.Length; i++)
        {
            if (isDoorFacingEdge(newRoom.doorSockets[i]))
            {
                Debug.LogWarning($"Door {newRoom.doorSockets[i].name} in {newRoom.name} is facing outside the grid!");
                newRoom.doorSockets[i].gameObject.SetActive(false); // Disable the door if it's facing outside the grid
            }
        }

        return true;
    }

    private bool isDoorFacingEdge(Transform door)
    {
        // Get a step direction based on the door's forward vector
        // North = (0, 1), East = (1, 0), South = (0, -1), West = (-1, 0)
        int dirX = Mathf.RoundToInt(door.forward.x);
        int dirZ = Mathf.RoundToInt(door.forward.z);

        // Convert the door's exact world position to the cell it is resting on
        int doorGridX = Mathf.FloorToInt(door.position.x / actualCellSize);
        int doorGridZ = Mathf.FloorToInt(door.position.z / actualCellSize);

        // Step slightly into into the next cell area
        int targetX = doorGridX + dirX;
        int targetZ = doorGridZ + dirZ;

        Debug.Log($"[{door.name}] resides on cell ({doorGridX}, {doorGridZ}). Checking neighbor branch at: ({targetX}, {targetZ})");

        // Check boundaries
        if (targetX < 0 || targetX >= numCellsWidth || 
            targetZ < 0 || targetZ >= numCellsHeight)
        {
            return true;
        }

        return false;        
    }
}