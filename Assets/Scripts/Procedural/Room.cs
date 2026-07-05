using UnityEngine;
using System.Collections.Generic;

using System;

public enum RoomTags
{
    None,
    Shop,
    Loot,
    Quest,
    Healing,
    Trade,
    Mob,
    MiniBoss,
    Boss,
    Sacrifice,
    Secret
}
public class Room : MonoBehaviour
{
    public float cellSize = 10f;
    public Transform origin;
    [SerializeField] private Transform[] cellTransforms;
    private Vector2Int[] relativeCoordinatesToOrigin;

    [Tooltip("The door sockets in the room where new rooms can be attached.")]
    public Transform[] doorSockets;

    [Tooltip("The tags that describe the purpose or characteristics of this room.")]
    public RoomTags currentTags;

    // Takes the world position and snaps it to the grid
    private Vector2Int PositionToGrid(Vector3 pos) => new Vector2Int(
        Mathf.FloorToInt(pos.x / cellSize),
        Mathf.FloorToInt(pos.z / cellSize)
    );

    // This makes sure to spawn the room taking into account the offset if the origin is not 0,0
    public Vector2Int LocalOriginOffset
    {
        get
        {
            if (origin == null) return Vector2Int.zero; // If origin isnt assigned, default cell 0,0 as origin (might cause issues tbh bcs not all origins have doors)
            Vector3 localPosToRoot = transform.InverseTransformPoint(origin.position); // Convert from world space to local space 
            return PositionToGrid(localPosToRoot);
        }
    }

    public Vector2Int[] GetRelativeCoordinates()
    {
        if (relativeCoordinatesToOrigin == null || relativeCoordinatesToOrigin.Length == 0)
        {
            CalculateRelativeCoordinatesToOrigin();
        }
        return relativeCoordinatesToOrigin;
    }

    void Start()
    {
        GetRelativeCoordinates();
    }

    private void CalculateRelativeCoordinatesToOrigin()
    {
        if (origin == null || cellTransforms == null) return;

        Vector2Int originGrid = PositionToGrid(origin.position); // Get origin in world grid coords
        relativeCoordinatesToOrigin = new Vector2Int[cellTransforms.Length]; // Store vectors from origin to cells

        // For each cell, calculate a vector from the origin to the cell (aka direction steps)
        for (int i = 0; i < cellTransforms.Length; i++)
        {
            relativeCoordinatesToOrigin[i] = PositionToGrid(cellTransforms[i].position) - originGrid;
        }
    }

    private void OnValidate()
    {
        // Find all children of the room that are cells and assign them to cellTransforms automatically
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        List<Transform> matchingCells = new List<Transform>();

        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("Cell"))
            {
                matchingCells.Add(child);
            }
        }

        cellTransforms = matchingCells.ToArray();
    }
}