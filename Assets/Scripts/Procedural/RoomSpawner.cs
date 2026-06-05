using UnityEngine;
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
    public LayerMask doorLayerMask;

    // --- NEW EDITOR TOOLS SECTION ---
    [Header("Editor Tools")]
    [Tooltip("The exact folder path in your project to scan (e.g., Assets/Prefabs/ProceduralRooms)")]
    [SerializeField] private string searchFolderPath = "Assets/Prefabs/ProceduralRooms";

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

        actualCellSize = startingRoomPrefab.cellSize;
        Debug.unityLogger.logEnabled = false;
    }

    void Start()
    {
        availableDoors = new List<Transform>();
        occupiedGrid = new bool[numCellsWidth, numCellsHeight];
        
        // Try spawning inital room at specified grid coordinates
        TrySpawnRoom(startingRoomPrefab, xIndex, zIndex);
        GenerateFullMap();
        TrimExtraDoors();
    }

    public bool TrySpawnRoom(Room room, int startX, int startZ)
    {
        Debug.Log($"Spawning {room.name} targeting origin at grid coordinates ({startX},{startZ})");
        
        // Validate the room can fit at the target location by checking all of its cells' world coordinates
        Vector2Int[] relativeCoords = room.GetRelativeCoordinates();
        Vector2Int[] cellWorldCoordinates = new Vector2Int[relativeCoords.Length];
        
        for (int i = 0; i < relativeCoords.Length; i++)
        {
            cellWorldCoordinates[i] = new Vector2Int(startX + relativeCoords[i].x, startZ + relativeCoords[i].y);
        }

        if (!AreCoordinatesWithinBounds(cellWorldCoordinates)){
            Debug.LogWarning($"Cannot spawn {room.name} because it would be out of bounds!");
            return false;
        }

        if (AreCoordinatesOccupied(cellWorldCoordinates)){
            Debug.LogWarning($"Cannot spawn {room.name} because it would overlap!");
            return false;
        }

        // Compute spawn position
        Vector3 targetWorldPos = new Vector3(startX * actualCellSize, 0, startZ * actualCellSize);
        
        // Calculate the physical offset mapping needed to align the custom origin
        Vector3 prefabOffset = new Vector3(room.LocalOriginOffset.x * actualCellSize, 0, room.LocalOriginOffset.y * actualCellSize);
        Vector3 finalSpawnPos = targetWorldPos - prefabOffset;

        // Instantiate using the computed position
        Room newRoom = Instantiate(room, finalSpawnPos, Quaternion.identity, this.transform);
        newRoom.name = $"{room.name}_at_{startX}_{startZ}";

        // Mark cells as occupied
        for (int i = 0; i < cellWorldCoordinates.Length; i++)
        {
            occupiedGrid[cellWorldCoordinates[i].x, cellWorldCoordinates[i].y] = true;
        }

        // Check door sockets
        for (int i = 0; i < newRoom.doorSockets.Length; i++)
        {
            if (isDoorFacingEdge(newRoom.doorSockets[i]))
            {
                newRoom.doorSockets[i].gameObject.SetActive(false);
                continue;
            }
            availableDoors.Add(newRoom.doorSockets[i]);
        }

        return true;
    }

    private bool isDoorFacingEdge(Transform door)
    {
        // Get a step direction based on the door's forward vector
        int dirX = Mathf.RoundToInt(door.forward.x);
        int dirZ = Mathf.RoundToInt(door.forward.z);

        // Convert the door's exact world position to the cell it is resting on
        int doorGridX = Mathf.FloorToInt(door.position.x / actualCellSize);
        int doorGridZ = Mathf.FloorToInt(door.position.z / actualCellSize);

        // Step slightly into into the next cell area
        int targetX = doorGridX + dirX;
        int targetZ = doorGridZ + dirZ;

        // Check boundaries
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
                return true;
            }
        }
        return false;
    }

    private void GenerateFullMap()
    {
        for (int i = 0; i < numRoomsToSpawn; i++)
        {
            if (availableDoors.Count == 0)
            {
                Debug.LogWarning("No more available doors left anywhere to spawn new rooms!");
                return;
            }

            // Create a temporary list tracking doors we haven't tried yet for this room iteration
            List<Transform> untriedDoors = new List<Transform>(availableDoors);
            bool roomSpawnedSuccessfully = false;

            // Randomly select a room prefab to try to fit
            Room roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

            // Keep trying doors until we successfully spawn a room OR run out of doors to try
            while (untriedDoors.Count > 0 && !roomSpawnedSuccessfully)
            {
                // Pick a random door from our untried pool
                int randomUntriedIndex = Random.Range(0, untriedDoors.Count);
                Transform doorToSpawnFrom = untriedDoors[randomUntriedIndex];

                Transform newDoorToConnect = GetValidConnectingDoor(roomPrefab, doorToSpawnFrom);

                if (newDoorToConnect == null)
                {
                    untriedDoors.RemoveAt(randomUntriedIndex);
                    continue; // Skip the rest of the loop and try a different door
                }

                // Calculate where the old door wants the connection to happen (the Target Cell)
                int dirX = Mathf.RoundToInt(doorToSpawnFrom.forward.x);
                int dirZ = Mathf.RoundToInt(doorToSpawnFrom.forward.z);

                int doorGridX = Mathf.FloorToInt(doorToSpawnFrom.position.x / actualCellSize);
                int doorGridZ = Mathf.FloorToInt(doorToSpawnFrom.position.z / actualCellSize);

                int targetX = doorGridX + dirX;
                int targetZ = doorGridZ + dirZ;

                // Find out which grid cell the NEW door is attached to relative to its prefab's origin
                Vector3 insideNewDoorPos = newDoorToConnect.position - (newDoorToConnect.forward * 0.1f);
                int newDoorCellX = Mathf.FloorToInt(insideNewDoorPos.x / actualCellSize);
                int newDoorCellZ = Mathf.FloorToInt(insideNewDoorPos.z / actualCellSize);

                int originCellX = Mathf.FloorToInt(roomPrefab.origin.position.x / actualCellSize);
                int originCellZ = Mathf.FloorToInt(roomPrefab.origin.position.z / actualCellSize);

                int doorOffsetX = newDoorCellX - originCellX;
                int doorOffsetZ = newDoorCellZ - originCellZ;

                // Shift the target origin coordinates backward by the offset so the doors snap together
                int originSpawnX = targetX - doorOffsetX;
                int originSpawnZ = targetZ - doorOffsetZ;

                // Try to spawn the room with the adjusted origin coordinates
                if (TrySpawnRoom(roomPrefab, originSpawnX, originSpawnZ))
                {
                    roomSpawnedSuccessfully = true;

                    // Find this door in the master list and remove it permanently
                    availableDoors.Remove(doorToSpawnFrom);
                }
                else
                {
                    // Remove this door from our untried pool so we don't pick it again for this room prefab
                    untriedDoors.RemoveAt(randomUntriedIndex);
                }
            }

            // Room prefab couldn't fit anywhere
            if (!roomSpawnedSuccessfully)
            {
                Debug.LogWarning($"Skipping room iteration {i}: Checked all {availableDoors.Count} available doors, but '{roomPrefab.name}' didn't fit anywhere.");
            }
        }
    }


    private void TrimExtraDoors()
    {
        GameObject[] doorObjects = GameObject.FindGameObjectsWithTag("Door");
        
        List<GameObject> doorsToDisable = new List<GameObject>();

        // Check each door to see if it's valid or should be disabled
        foreach (GameObject doorObj in doorObjects)
        {
            Transform door = doorObj.transform;

            int dirX = Mathf.RoundToInt(door.forward.x);
            int dirZ = Mathf.RoundToInt(door.forward.z);

            int doorGridX = Mathf.FloorToInt(door.position.x / actualCellSize);
            int doorGridZ = Mathf.FloorToInt(door.position.z / actualCellSize);

            Vector3 rayCastDirection = new Vector3(dirX, 0, dirZ);
            Vector3 rayCastOrigin = door.position + (rayCastDirection * 0.2f);
            float rayCastDistance = actualCellSize * 0.5f; 

            // Debug.DrawRay(rayCastOrigin, rayCastDirection * rayCastDistance, Color.red, 15f);

            if (Physics.Raycast(rayCastOrigin, rayCastDirection, out RaycastHit hit, rayCastDistance, doorLayerMask))
            {
                if (!hit.collider.CompareTag("Door"))
                {
                    // It hit something, but it wasn't a door. Add to the hit list.
                    doorsToDisable.Add(doorObj);
                }
            }
            else
            {
                // It hit nothing. Add to the hit list.
                doorsToDisable.Add(doorObj);
            }
        }

        // Disable the bad doors.
        foreach (GameObject badDoor in doorsToDisable)
        {
            badDoor.SetActive(false);
        }
    }

    private Transform GetValidConnectingDoor(Room prefab, Transform doorToSpawnFrom)
    {
        Vector3 requiredDirection = -doorToSpawnFrom.forward;

        foreach (Transform socket in prefab.doorSockets)
        {
            // Check if this door is facing the correct opposite direction
            bool facesOpposite = Mathf.RoundToInt(socket.forward.x) == Mathf.RoundToInt(requiredDirection.x) &&
                                Mathf.RoundToInt(socket.forward.z) == Mathf.RoundToInt(requiredDirection.z);

            if (facesOpposite)
            {
                return socket; // Returns the first valid door
            }
        }

        return null; // No door found that points in the correct direction
    }

#if UNITY_EDITOR
    [ContextMenu("Auto-Populate Room Prefabs")]
    private void AutoPopulateRoomPrefabs()
    {
        // Verify the folder path is actually valid
        if (!UnityEditor.AssetDatabase.IsValidFolder(searchFolderPath))
        {
            Debug.LogError($"Folder '{searchFolderPath}' does not exist. Make sure the path is correct and starts with 'Assets/'");
            return;
        }

        // Find all GameObject assets in the specified folder
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:GameObject", new[] { searchFolderPath });
        List<Room> validRooms = new List<Room>();

        // Loop through them and check for the "Room" tag and Room.cs component
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null && prefab.CompareTag("Room"))
            {
                Room roomScript = prefab.GetComponent<Room>();
                if (roomScript != null)
                {
                    validRooms.Add(roomScript);
                }
            }
        }

        // Assign to the array and mark the scene as dirty so Unity saves the changes
        roomPrefabs = validRooms.ToArray();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}