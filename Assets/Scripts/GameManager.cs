using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    [SerializeField] private Room startingRoomPrefab; // CHANGE TO CELL AND CHANGE ROOM SPAWNER TOO 

    public UnityEvent<Vector2Int> OnCombatTriggered;
    private float cellSize;

    public float Cellsize => cellSize; // CHANGE?

    private Vector3 playerPosition;


    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
          Destroy(this.gameObject);
          return;  
        }        
        Instance = this;  
            
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        if (Player.Instance == null)
        {
            Debug.LogError("No player in scene!");
        }

        cellSize = startingRoomPrefab.cellSize;

        OnCombatTriggered.AddListener(PlacePlayerOnMap);

    }

    public void PlacePlayerOnMap(Vector2Int cellCoords)
    {
        Vector3 currentPosition = Player.Instance.gameObject.transform.position;
        playerPosition = new Vector3(cellCoords.x * cellSize + cellSize * 0.5f, currentPosition.y, cellCoords.y * cellSize + cellSize * 0.5f);    
    }

    private void OnEnable()
    {
        if (Instance != null && Instance != this) return;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (Instance != null && Instance != this) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game"){
            Player.Instance.TeleportPlayer(playerPosition);
        }
    }
}
