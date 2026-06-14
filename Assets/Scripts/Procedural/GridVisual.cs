using UnityEngine;
using TMPro;
public class GridVisual : MonoBehaviour
{    
    private float cellSize;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Material cellEvenMaterial;
    [SerializeField] private Material cellOddMaterial;
    private TMP_Text cellText;

    private float width;
    private float height;

    void Start()
    {
        cellSize = cellPrefab.GetComponentInChildren<Renderer>().bounds.size.x;
        // Debug.Log($"Actual cell size in unity units: {cellSize}");

        if (RoomSpawner.Instance == null)
        {
            Debug.LogError("GridVisual: RoomSpawner instance not found!");
            return;
        }
        width = RoomSpawner.Instance.NumCellsWidth;
        height = RoomSpawner.Instance.NumCellsHeight;

        GenerateGridVisual();
    }
    public float GenerateGridVisual()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject cell = GameObject.Instantiate(cellPrefab, new Vector3(i * cellSize, this.transform.position.y, j * cellSize), Quaternion.identity, this.transform);
                cell.name = $"Cell_{i}_{j}";
                TMP_Text cellText = cell.GetComponentInChildren<TMP_Text>();
                cellText.text = $"{i},{j}";
                Renderer cellRenderer = cell.GetComponentInChildren<Renderer>();
                if ((i + j) % 2 == 0)
                {
                    cellRenderer.material = cellEvenMaterial;
                }
                else
                {
                    cellRenderer.material = cellOddMaterial;
                }
            }
        }

        return cellSize;
    }
}
