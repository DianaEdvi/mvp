using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    public float cellSize = 10f;

    public Transform origin;
    [SerializeField] private Transform[] cellTransforms;
    private Vector2Int[] relativeCoordinatesToOrigin;

    [Tooltip("The door sockets in the room where new rooms can be attached.")]
    public Transform[] doorSockets;
    public Vector2Int LocalOriginOffset
    {
        get
        {
            if (origin == null) return Vector2Int.zero;
            
            // Transforms origin position relative to this root object's transform matrix
            Vector3 localPosToRoot = transform.InverseTransformPoint(origin.position);

            return new Vector2Int(
                Mathf.FloorToInt(localPosToRoot.x / cellSize),
                Mathf.FloorToInt(localPosToRoot.z / cellSize)
            );
        }
    }

    // Returns the world coordinates of all cells in this room relative to the origin cell (ie the step directions needed to move from the origin cell to each cell in the room)
    public Vector2Int[] GetRelativeCoordinates()
    {
        if (relativeCoordinatesToOrigin == null || relativeCoordinatesToOrigin.Length == 0)
        {
            CalculateRelativeCoordinatesToOrigin(origin, cellTransforms);
        }
        return relativeCoordinatesToOrigin;
    }

    void Start()
    {
        GetRelativeCoordinates();
    }

    private void CalculateRelativeCoordinatesToOrigin(Transform originTransform, Transform[] cellTransforms)
    {
        if (originTransform == null || cellTransforms == null) return;

        Vector2Int originInWorldCoords = new Vector2Int(
            Mathf.FloorToInt(originTransform.position.x / cellSize), 
            Mathf.FloorToInt(originTransform.position.z / cellSize)
        );
        
        Vector2Int[] cellsInWorldCoords = new Vector2Int[cellTransforms.Length];
        for (int i = 0; i < cellTransforms.Length; i++)
        {
            Vector3 globalPos = cellTransforms[i].position;
            cellsInWorldCoords[i] = new Vector2Int(
                Mathf.FloorToInt(globalPos.x / cellSize),
                Mathf.FloorToInt(globalPos.z / cellSize)
            );
        }

        relativeCoordinatesToOrigin = new Vector2Int[cellTransforms.Length];
        for (int i = 0; i < cellTransforms.Length; i++)
        {
            relativeCoordinatesToOrigin[i] = cellsInWorldCoords[i] - originInWorldCoords;
        }
    }

    private void OnValidate()
    {
        // 1. Grab every single transform attached to this object or its children (including inactive ones)
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        
        // 2. Create a temporary list to hold the ones that match our criteria
        List<Transform> matchingCells = new List<Transform>();

        // 3. Loop through and find the ones tagged "Cell"
        foreach (Transform child in allChildren)
        {
            // Use CompareTag as it's much safer and more performant than child.tag == "Cell"
            if (child.CompareTag("Cell"))
            {
                matchingCells.Add(child);
            }
        }

        // 4. Assign the results back to your serialized array so it updates in the Inspector
        cellTransforms = matchingCells.ToArray();
    }
}