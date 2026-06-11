using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera roomCamera;
    
    // Set this higher than your baseline (e.g., 20)
    [SerializeField] private int activePriority = 20; 

    private void OnTriggerEnter(Collider other)
    {
        // Assuming your player has a tag, or you can check for your specific controller component
        if (other.CompareTag("Player"))
        {
            roomCamera.Priority = activePriority;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Drop it back down so another room can take over
            roomCamera.Priority = 10; 
        }
    }
}
