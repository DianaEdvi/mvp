using UnityEngine;
using TMPro;
public class GridCellsPlane : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    private float cellSize;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Material cellEvenMaterial;
    [SerializeField] private Material cellOddMaterial;
    private TMP_Text cellText;

    void Awake()
    {
        cellSize = cellPrefab.GetComponent<Renderer>().bounds.size.x;
        Debug.Log($"Cell size: {cellSize}");

        GenerateGrid();
    }
    void GenerateGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject cell = GameObject.Instantiate(cellPrefab, new Vector3(i * cellSize, this.transform.position.y, j * cellSize), Quaternion.identity, this.transform);
                cell.name = $"Cell_{i}_{j}";
                TMP_Text cellText = cell.GetComponentInChildren<TMP_Text>();
                cellText.text = $"{i},{j}";
                Renderer cellRenderer = cell.GetComponent<Renderer>();
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
    }
}
