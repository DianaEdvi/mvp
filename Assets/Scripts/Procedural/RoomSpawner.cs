using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


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
        // Debug.unityLogger.logEnabled = false;
    }

    void Start()
    {
        availableDoors = new List<Transform>();
        occupiedGrid = new bool[numCellsWidth, numCellsHeight];
        
        // Try spawning inital room at specified grid coordinates
        TrySpawnRoom(startingRoomPrefab, xIndex, zIndex);
        TrySpawnRoom(startingRoomPrefab, 7, 0);

      
    }

    public bool TrySpawnRoom(Room room, int startX, int startZ)
    {

        Vector2Int[] cellWorldCoordinates = new Vector2Int[room.localCellCoordinates.Length];
        for (int i = 0; i < room.localCellCoordinates.Length; i++)
        {
            cellWorldCoordinates[i] = new Vector2Int(startX + room.localCellCoordinates[i].x, startZ + room.localCellCoordinates[i].y);
        }
      
        if (!AreCoordinatesWithinBounds(cellWorldCoordinates)){
            Debug.LogWarning($"Cannot spawn {room.name} at grid coordinates ({startX},{startZ}) because it would be out of bounds!");
             return false;
        }

        if (AreCoordinatesOccupied(cellWorldCoordinates)){
            Debug.LogWarning($"Cannot spawn {room.name} at grid coordinates ({startX},{startZ}) because it would overlap with an existing room!");
             return false;
        }

        Debug.Log($"Spawning {room.name} at grid coordinates ({startX},{startZ})");

        // If room is in valid position, compute the exact grid alignment position
        Vector3 spawnPos = new Vector3(startX * actualCellSize, 0, startZ * actualCellSize);
        Room newRoom = Instantiate(room, spawnPos, Quaternion.identity, this.transform);
        newRoom.name = $"Room_{startX}_{startZ}";

        // Mark cells as occupied
        for (int i = 0; i < cellWorldCoordinates.Length; i++)
        {
            occupiedGrid[cellWorldCoordinates[i].x, cellWorldCoordinates[i].y] = true;
        }

        // Check each door socket to see if it's facing outside the grid. If so, disable it. Otherwise, add it to the list of available doors.
        for (int i = 0; i < newRoom.doorSockets.Length; i++)
        {
            if (isDoorFacingEdge(newRoom.doorSockets[i]))
            {
                newRoom.doorSockets[i].gameObject.SetActive(false); // Disable the door if it's facing outside the grid
                continue;
            }
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
        Debug.Log($"Checking door {door.name} at grid ({doorGridX},{doorGridZ}) facing direction ({dirX},{dirZ}) towards target grid ({targetX},{targetZ})");
        if (!AreCoordinatesWithinBounds(new Vector2Int[] { new Vector2Int(targetX, targetZ) }))
        {
            return true; // Door is facing outside the grid
        }

        return false;        
    }

    private bool AreCoordinatesWithinBounds(Vector2Int[] cellWorldCoordinates)
    {
        foreach (Vector2Int cell in cellWorldCoordinates)
        {
            if (cell.x < 0 || cell.x >= numCellsWidth || cell.y < 0 || cell.y >= numCellsHeight)
            {   
                Debug.LogWarning($"Cell at ({cell.x},{cell.y}) is out of grid boundaries!");
                return false;
            }
        }
        return true;
    }

    private bool AreCoordinatesOccupied(Vector2Int[] cellWorldCoordinates)
    {
        foreach (Vector2Int cell in cellWorldCoordinates)
        {
            if (occupiedGrid[cell.x, cell.y])
            {
                Debug.LogWarning($"Cell at ({cell.x},{cell.y}) is already occupied!");
                return true;
            }
        }
        return false;
    }
}