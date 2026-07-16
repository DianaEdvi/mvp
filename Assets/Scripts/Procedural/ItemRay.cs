using UnityEngine;

public class RaycastShooter : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 10f;
    public LayerMask hitLayers;

    void Update()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        bool didHit = Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, hitLayers);

        Color rayColor = didHit ? Color.green : Color.red;

        if (didHit)
        {
            Debug.DrawLine(origin, hit.point, rayColor);
        }
        else
        {
            Debug.DrawRay(origin, direction * rayDistance, rayColor);
        }
    }
}