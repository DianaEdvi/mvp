using UnityEngine;

public class Ray : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 10f;
    public LayerMask hitLayers;
    public GameObject currentHitObject;
    public bool Hitting;
    void Update()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Hitting = Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, hitLayers);

        Color rayColor = Hitting ? Color.green : Color.red;

        if (Hitting)
        {
            Debug.DrawLine(origin, hit.point, rayColor);
            currentHitObject = hit.collider.gameObject;
        }
        else
        {
            Debug.DrawRay(origin, direction * rayDistance, rayColor);
        }
    }
}