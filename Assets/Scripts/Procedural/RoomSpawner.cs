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
    [SerializeField] private int numRoomsToSpawn = 5;

    // Initialized inline to save space
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
        // Debug.unityLogger.logEnabled = false;

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        occupiedGrid = new bool[numCellsWidth, numCellsHeight];

        TrySpawnRoom(startingRoomPrefab, xIndex, zIndex);
        GenerateFullMap();
        TrimExtraDoors();
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

        // Uses a cleaner single-coordinate bounds check instead of array allocation
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

    private void GenerateFullMap()
    {
        for (int i = 0; i < numRoomsToSpawn; i++)
        {
            if (availableDoors.Count == 0) return;

            List<Transform> untriedDoors = new List<Transform>(availableDoors); // Begin list with all available doors as untried 
            bool roomSpawnedSuccessfully = false;
            Room roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)]; // Random from list **TO BE CHANGED**

            // Loop through all untried doors
            while (untriedDoors.Count > 0 && !roomSpawnedSuccessfully)
            {
                int randomUntriedIndex = Random.Range(0, untriedDoors.Count); // Get random door to try
                Transform doorToSpawnFrom = untriedDoors[randomUntriedIndex]; // Get a random available door from the rooms already placed (that are not yet tested)
                Transform newDoorToConnect = GetValidConnectingDoor(roomPrefab, doorToSpawnFrom); // Get a random valid door from the room we want to spawn  

                if (newDoorToConnect == null) // No valid doors to connect to
                {
                    untriedDoors.RemoveAt(randomUntriedIndex);
                    continue;
                }

                Vector2Int doorGrid = PositionToGrid(doorToSpawnFrom.position); // The door to connect to's coords 

                // The coords of the cell it faces
                int targetX = doorGrid.x + Mathf.RoundToInt(doorToSpawnFrom.forward.x);
                int targetZ = doorGrid.y + Mathf.RoundToInt(doorToSpawnFrom.forward.z);

                Vector3 insideNewDoorPos = newDoorToConnect.position - (newDoorToConnect.forward * 0.1f); // Get the position of the new door a bit more inside the cell (for FloorToInt to work)
                Vector2Int newDoorCell = PositionToGrid(insideNewDoorPos); // Get the grid coords of the new door's cell
                Vector2Int originCell = PositionToGrid(roomPrefab.origin.position); // Get the grid coords of the origin of the room

                // Work backwards to get the place the origin SHOULD be to place the door in the right spot 
                int originSpawnX = targetX - (newDoorCell.x - originCell.x);
                int originSpawnZ = targetZ - (newDoorCell.y - originCell.y);

                // Finally, try to spawn the room and update the doors arrays accordingly 
                if (TrySpawnRoom(roomPrefab, originSpawnX, originSpawnZ))
                {
                    roomSpawnedSuccessfully = true;
                    availableDoors.Remove(doorToSpawnFrom);
                }
                else
                {
                    untriedDoors.RemoveAt(randomUntriedIndex);
                }
            }
        }
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

    // Populate prefabs automatically when you call this function in the inspector (right click on script in inspector)
#if UNITY_EDITOR
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
#endif
}