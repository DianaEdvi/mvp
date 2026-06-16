using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    [Tooltip("The Cinemachine camera specific to this room.")]
    [SerializeField] private CinemachineCamera roomCamera;
    
    [Tooltip("Priority when the player is inside the room.")]
    [SerializeField] private int activePriority = 20;
    
    private int defaultPriority;
    private Collider roomCollider;

    private void Start()
    {
        if (roomCamera != null)
        {
            // Store whatever baseline priority you set in the inspector
            defaultPriority = roomCamera.Priority.Value;
        }

        roomCollider = GetComponent<Collider>();
        
        // Set the first camera position
        SnapCameraIfPlayerIsInside();
    }

    private void SnapCameraIfPlayerIsInside()
    {        
        if (roomCollider == null || roomCamera == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Check if the player is withing the bounds of the room and set the camera to it if yes 
            if (roomCollider.bounds.Contains(player.transform.position))
            {
                 Debug.Log("Room does not contain player");
                roomCamera.Priority.Value = activePriority;
                if (Camera.main != null)
                {
                    Debug.Log("Main camera not found");
                    Camera.main.transform.SetPositionAndRotation(roomCamera.transform.position, roomCamera.transform.rotation);
                }
            }
        }
        else {
             Debug.Log("Player is null");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && roomCamera != null)
        {
            // Set new camera priority 
            roomCamera.Priority.Value = activePriority;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && roomCamera != null)
        {
            // Revert to default priority
            roomCamera.Priority.Value = defaultPriority;
        }
    }
}