using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


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
    private List<Transform> availableDoors;

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
        availableDoors = new List<Transform>();
        occupiedGrid = new bool[numCellsWidth, numCellsHeight];
        
        // Try spawning inital room at specified grid coordinates
        TrySpawnRoom(startingRoomPrefab, xIndex, zIndex);
        // TrySpawnRoom(startingRoomPrefab, xIndex, zIndex);

      
    }

    public bool TrySpawnRoom(Room room, int startX, int startZ)
    {
        if (!AreCoordinatesValid(room.localCellCoordinates, startX, startZ)) return false;

        // If room is in valid position, compute the exact grid alignment position
        Vector3 spawnPos = new Vector3(startX * actualCellSize, 0, startZ * actualCellSize);
        Room newRoom = Instantiate(room, spawnPos, Quaternion.identity, this.transform);
        newRoom.name = $"Room_{startX}_{startZ}";

        // Mark cells as occupied
        for (int x = startX; x < startX + room.roomWidthCells; x++)
        {
            for (int z = startZ; z < startZ + room.roomHeightCells; z++)
            {
                occupiedGrid[x, z] = true;
            }
        }

        // Check each door socket to see if it's facing outside the grid. If so, disable it. Otherwise, add it to the list of available doors.
        for (int i = 0; i < newRoom.doorSockets.Length; i++)
        {
            if (isDoorFacingEdge(newRoom.doorSockets[i]))
            {
                Debug.LogWarning($"Door {newRoom.doorSockets[i].name} in {newRoom.name} is facing outside the grid!");
                newRoom.doorSockets[i].gameObject.SetActive(false); // Disable the door if it's facing outside the grid
                continue;
            }
            // Debug.Log($"Door {newRoom.doorSockets[i].name} in {newRoom.name} is valid and facing inside the grid.");
            availableDoors.Add(newRoom.doorSockets[i]);
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

        // Check boundaries
        AreCoordinatesValid(new Vector2Int[] { new Vector2Int(targetX, targetZ) }, doorGridX, doorGridZ);

        return false;        
    }

    private bool AreCoordinatesValid(Vector2Int[] cellCoordinates, int startX, int startZ)
    {
        // Check if the room is valid 
        foreach (Vector2Int cell in cellCoordinates)
        {
            // Convert local cell coordinates to world grid coordinates
            int worldX = startX + cell.x;
            int worldZ = startZ + cell.y;
            // Check if the world grid coordinates are within bounds
            if (worldX < 0 || worldX >= numCellsWidth || worldZ < 0 || worldZ >= numCellsHeight)
            {   
                Debug.LogWarning($"Local cell at ({cell.x},{cell.y}) would be out of grid boundaries when placed at ({startX},{startZ})!");
                return false;
            }

            // Check if the world grid coordinates are already occupied
            if (occupiedGrid[worldX, worldZ])
            {
                Debug.LogWarning($"Local cell at ({cell.x},{cell.y}) would overlap with an occupied cell at ({worldX},{worldZ}) when placed at ({startX},{startZ})!");
                return false;
            }
            
        }
        return true;    }
}