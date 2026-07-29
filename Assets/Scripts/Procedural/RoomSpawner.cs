using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class RoomSpawner : MonoBehaviour
{
    public static RoomSpawner Instance { get; private set; }

    private float actualCellSize;

    [Header("Grid Config")]
    [SerializeField] private int numCellsWidth = 10;
    [SerializeField] private int numCellsHeight = 10;

    [Header("Start Spawn Settings")]
    [SerializeField] private Room startingRoomPrefab;
    [SerializeField] private int xIndex;
    [SerializeField] private int zIndex;

    [Header("Rooms Settings")]
    [SerializeField] private Room[] roomPrefabs;
    public RoomSpawnConfig[] roomSpawnConfigs;

    [Header("Generation Settings")]
    [Tooltip("Maximum number of times to retry generation if it fails to place all mandatory rooms.")]
    [SerializeField] private int maxRetries = 50;
    private List<Transform> availableDoors = new List<Transform>();
    public LayerMask doorLayerMask;
    [Header("Editor Tools")]
    [Tooltip("The exact folder path in your project to scan")]
    [SerializeField] private string searchFolderPath = "Assets/Prefabs/ProceduralRooms";

    public float NumCellsWidth => numCellsWidth;
    public float NumCellsHeight => numCellsHeight;

    private bool[,] occupiedGrid;

    // Takes the world position and snaps it to the grid
    private Vector2Int PositionToGrid(Vector3 pos) => new Vector2Int(
        Mathf.FloorToInt(pos.x / actualCellSize),
        Mathf.FloorToInt(pos.z / actualCellSize)
    );

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;

        actualCellSize = startingRoomPrefab.cellSize;

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        int attempts = 0;
        bool success = false;

        while (attempts < maxRetries && !success)
        {
            attempts++;
            ResetGrid();

            // Try placing the starting room
            if (TrySpawnRoom(startingRoomPrefab, xIndex, zIndex))
            {
                // Attempt to generate the rest of the map. 
                // It will return true if all mandatory rooms are placed.
                success = GenerateFullMap();
            }
        }

        if (success)
        {
            Debug.Log($"Dungeon generated successfully after {attempts} attempts.");
            TrimExtraDoors();
        }
        else
        {
            Debug.LogError($"Failed to generate a complete dungeon with all mandatory rooms after {maxRetries} attempts. Try increasing the grid size or reducing room counts.");
        }
    }

    private List<Room> PopulateRoomsArray()
    {
        List<Room> standardRooms = new List<Room>(); // Rooms we want to shuffle
        List<Room> finalRooms = new List<Room>();    // Rooms we want to spawn last (bosses)

        if (roomSpawnConfigs == null || roomSpawnConfigs.Length == 0)
        {
            Debug.LogWarning("Room Spawn Configs are empty! Add some rules in the Inspector.");
            return standardRooms;
        }

        // Loop through the rules defined in the inspector
        foreach (RoomSpawnConfig config in roomSpawnConfigs)
        {
            // Find all available prefabs in the pool that match this required tag
            List<Room> matchingPrefabs = new List<Room>();
            foreach (Room prefab in roomPrefabs)
            {
                if (prefab.currentTags == config.requiredTag)
                {
                    matchingPrefabs.Add(prefab);
                }
            }

            if (matchingPrefabs.Count == 0)
            {
                Debug.LogWarning($"No prefabs found with tag {config.requiredTag}! Skipping this config.");
                continue;
            }

            // Randomly draw a room the requested amount of times
            for (int i = 0; i < config.count; i++)
            {
                Room randomSelection = matchingPrefabs[Random.Range(0, matchingPrefabs.Count)];

                // Add all the bosses to the final rooms array and to the standard otherwise
                if (randomSelection.currentTags == RoomTags.TRATM ||
                    randomSelection.currentTags == RoomTags.Prospero ||
                    randomSelection.currentTags == RoomTags.Mesmerist ||
                    randomSelection.currentTags == RoomTags.Rowena)
                {
                    finalRooms.Add(randomSelection);
                }
                else
                {
                    standardRooms.Add(randomSelection);
                }
            }
        }

        // Shuffle the standard deck (Fisher-Yates shuffle)
        for (int i = 0; i < standardRooms.Count; i++)
        {
            Room temp = standardRooms[i];
            int randomIndex = Random.Range(i, standardRooms.Count);
            standardRooms[i] = standardRooms[randomIndex];
            standardRooms[randomIndex] = temp;
        }

        // Combine the decks: Standard shuffled rooms first, final rooms last
        List<Room> completeDeck = new List<Room>(standardRooms);
        completeDeck.AddRange(finalRooms);

        return completeDeck;
    }

    public bool TrySpawnRoom(Room room, int startX, int startZ)
    {
        Vector2Int[] relativeCoords = room.GetRelativeCoordinates(); // vector from origin to each cell in the room
        Vector2Int[] cellWorldGridCoordinates = new Vector2Int[relativeCoords.Length]; // grid coords for each cell in the room

        // Find world grid coords for each cell 
        for (int i = 0; i < relativeCoords.Length; i++)
        {
            cellWorldGridCoordinates[i] = new Vector2Int(startX + relativeCoords[i].x, startZ + relativeCoords[i].y);
        }

        // Check if any of these cells are occupied 
        if (!AreCoordinatesWithinBounds(cellWorldGridCoordinates) || AreCoordinatesOccupied(cellWorldGridCoordinates))
        {
            return false;
        }

        Vector3 targetWorldPos = new Vector3(startX * actualCellSize, 0, startZ * actualCellSize); // World grid coords of origin 
        Vector3 prefabOffset = new Vector3(room.LocalOriginOffset.x * actualCellSize, 0, room.LocalOriginOffset.y * actualCellSize); // The offset of where the origin should be 

        // Instantiate the room where the origin should be
        Room newRoom = Instantiate(room, targetWorldPos - prefabOffset, Quaternion.identity, this.transform);
        newRoom.name = $"{room.name}_at_{startX}_{startZ}";

        // Mark its cells as occupied 
        foreach (Vector2Int cell in cellWorldGridCoordinates)
        {
            occupiedGrid[cell.x, cell.y] = true;
        }

        // Check all the doors in the room and if they are not facing the edge of the grid, they become an available door
        foreach (Transform door in newRoom.doorSockets)
        {
            if (!IsDoorFacingEdge(door))
                availableDoors.Add(door);
        }

        return true;
    }

    private bool IsDoorFacingEdge(Transform door)
    {
        Vector2Int doorGrid = PositionToGrid(door.position); // Get the cell the door is in 

        // Get the coordinates of the cell the door is leading to 
        int targetX = doorGrid.x + Mathf.RoundToInt(door.forward.x);
        int targetZ = doorGrid.y + Mathf.RoundToInt(door.forward.z);

        return !IsWithinBounds(targetX, targetZ);
    }

    private bool IsWithinBounds(int x, int z)
    {
        //Checks if a coordinate is within the bounds of the grid
        return x >= 0 && x < numCellsWidth && z >= 0 && z < numCellsHeight;
    }

    private bool AreCoordinatesWithinBounds(Vector2Int[] cellWorldCoordinates)
    {
        // Check if any coords of an array are out of bounds
        foreach (Vector2Int cell in cellWorldCoordinates)
        {
            if (!IsWithinBounds(cell.x, cell.y)) return false;
        }
        return true;
    }

    private bool AreCoordinatesOccupied(Vector2Int[] cellWorldCoordinates)
    {
        // Check if any coords of an array are occupied
        foreach (Vector2Int cell in cellWorldCoordinates)
        {
            if (occupiedGrid[cell.x, cell.y]) return true;
        }
        return false;
    }

    private bool GenerateFullMap()
    {
        // Generate list of rooms we want 
        List<Room> roomDeck = PopulateRoomsArray();
        int successfullySpawnedCount = 0;

        // Loop through our deck and try to spawn them one by one 
        foreach (Room roomPrefab in roomDeck)
        {
            if (availableDoors.Count == 0)
            {
                // Ran out of doors before placing all rooms; attempt failed
                return false;
            }

            List<Transform> untriedDoors = new List<Transform>(availableDoors);
            bool roomSpawnedSuccessfully = false;

            while (untriedDoors.Count > 0 && !roomSpawnedSuccessfully)
            {
                int randomUntriedIndex = Random.Range(0, untriedDoors.Count);
                Transform doorToSpawnFrom = untriedDoors[randomUntriedIndex];
                Transform newDoorToConnect = GetValidConnectingDoor(roomPrefab, doorToSpawnFrom);

                if (newDoorToConnect == null)
                {
                    untriedDoors.RemoveAt(randomUntriedIndex);
                    continue;
                }

                Vector2Int doorGrid = PositionToGrid(doorToSpawnFrom.position);
                int targetX = doorGrid.x + Mathf.RoundToInt(doorToSpawnFrom.forward.x);
                int targetZ = doorGrid.y + Mathf.RoundToInt(doorToSpawnFrom.forward.z);

                Vector3 insideNewDoorPos = newDoorToConnect.position - (newDoorToConnect.forward * 0.1f);
                Vector2Int newDoorCell = PositionToGrid(insideNewDoorPos);
                Vector2Int originCell = PositionToGrid(roomPrefab.origin.position);

                int originSpawnX = targetX - (newDoorCell.x - originCell.x);
                int originSpawnZ = targetZ - (newDoorCell.y - originCell.y);

                if (TrySpawnRoom(roomPrefab, originSpawnX, originSpawnZ))
                {
                    roomSpawnedSuccessfully = true;
                    successfullySpawnedCount++;
                    availableDoors.Remove(doorToSpawnFrom);
                }
                else
                {
                    untriedDoors.RemoveAt(randomUntriedIndex);
                }
            }

            // If we exit the while loop and a room from the deck couldn't be placed at all
            if (!roomSpawnedSuccessfully)
            {
                return false;
            }
        }

        // Return true only if we spawned exactly the number of rooms we requested
        return successfullySpawnedCount == roomDeck.Count;
    }
    private Transform GetValidConnectingDoor(Room prefab, Transform doorToSpawnFrom)
    {
        // Check the opposite direction of the door we are connecting to 
        Vector3 requiredDirection = -doorToSpawnFrom.forward;

        // For each door in the room to spawn, check if its direction is opposite of the door we want (aka they are facing each other)
        foreach (Transform socket in prefab.doorSockets)
        {
            if (Mathf.RoundToInt(socket.forward.x) == Mathf.RoundToInt(requiredDirection.x) &&
                Mathf.RoundToInt(socket.forward.z) == Mathf.RoundToInt(requiredDirection.z))
            {
                return socket;
            }
        }
        return null;
    }

    private void TrimExtraDoors()
    {
        GameObject[] doorObjects = GameObject.FindGameObjectsWithTag("Door");
        float rayCastDistance = actualCellSize * 0.5f;

        List<Transform> validDoors = new List<Transform>();
        List<Transform> invalidDoors = new List<Transform>();

        // Perform all raycasts 
        foreach (GameObject doorObj in doorObjects)
        {
            Transform door = doorObj.transform;
            Vector3 rayCastDirection = new Vector3(Mathf.RoundToInt(door.forward.x), 0, Mathf.RoundToInt(door.forward.z));
            Vector3 rayCastOrigin = door.position + (rayCastDirection * 0.2f);

            // If we hit another door, it's a valid connection
            if (Physics.Raycast(rayCastOrigin, rayCastDirection, out RaycastHit hit, rayCastDistance, doorLayerMask) && hit.collider.CompareTag("Door"))
            {
                validDoors.Add(door);
            }
            else
            {
                // If we hit nothing, or a standard wall, it's invalid
                invalidDoors.Add(door);
            }
        }

        // Apply visuals
        foreach (Transform door in validDoors)
        {
            SetDoorVisuals(door, isValid: true);
        }

        foreach (Transform door in invalidDoors)
        {
            SetDoorVisuals(door, isValid: false);
        }
    }

    // Activates appropriate wall
    private void SetDoorVisuals(Transform door, bool isValid)
    {
        Transform parent = door.parent;

        foreach (Transform child in parent)
        {
            if (child.CompareTag("DoorWall"))
            {
                // If valid, DoorWall is on
                child.gameObject.SetActive(isValid);
            }
            else if (child.CompareTag("SolidWall"))
            {
                // If valid, SolidWall is off. If invalid, SolidWall is on
                child.gameObject.SetActive(!isValid);
            }
        }

        // Disable the door collider
        door.gameObject.SetActive(false);
    }

    private void ResetGrid()
    {
        // Reset the grid array
        occupiedGrid = new bool[numCellsWidth, numCellsHeight];

        // Clear available doors
        availableDoors.Clear();

        // Destroy all instantiated room children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

#if UNITY_EDITOR
    // Populate prefabs automatically when you call this function in the inspector (right click on script in inspector)
    [ContextMenu("Auto-Populate Room Prefabs")]
    private void AutoPopulateRoomPrefabs()
    {
        // Check if path is valid 
        if (!UnityEditor.AssetDatabase.IsValidFolder(searchFolderPath)) return;

        // 
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:GameObject", new[] { searchFolderPath }); // Find all GameObject prefabs in the specified folder 
        List<Room> validRooms = new List<Room>();

        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid); // Convert the GUIDs to paths 
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path); // Get the GameObject at that path 

            // Check for null or tag
            if (prefab != null && prefab.CompareTag("Room"))
            {
                // If it finds a room component, add it to the list of valid rooms
                if (prefab.TryGetComponent<Room>(out var roomScript))
                {
                    validRooms.Add(roomScript);
                }
            }
        }

        // Convert to array 
        roomPrefabs = validRooms.ToArray();
        // Notify script that a change has been made (will update memory next time the file is saved) 
        UnityEditor.EditorUtility.SetDirty(this);
    }

    // Populate the room spawn configs automatically 
    [ContextMenu("Auto-Populate Spawn Configs")]
    private void AutoPopulateSpawnConfigs()
    {
        // Get all possible values from the RoomTags enum
        System.Array allTags = System.Enum.GetValues(typeof(RoomTags));

        // Store our updated configurations
        List<RoomSpawnConfig> updatedConfigs = new List<RoomSpawnConfig>();

        foreach (RoomTags tag in allTags)
        {
            if (tag == RoomTags.None) continue; // All rooms should have a tag

            int preservedCount = 0;

            // If the array already exists, search it for the current tag to save its count
            if (roomSpawnConfigs != null)
            {
                foreach (RoomSpawnConfig existingConfig in roomSpawnConfigs)
                {
                    if (existingConfig.requiredTag == tag)
                    {
                        preservedCount = existingConfig.count;
                        break;
                    }
                }
            }

            // Create the new config entry and add it to our list
            RoomSpawnConfig newConfig = new RoomSpawnConfig();
            newConfig.requiredTag = tag;
            newConfig.count = preservedCount;

            updatedConfigs.Add(newConfig);
        }

        roomSpawnConfigs = updatedConfigs.ToArray();

        // Notify script that a change has been made (will update memory next time the file is saved) 
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}