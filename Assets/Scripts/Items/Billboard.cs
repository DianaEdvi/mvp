using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cameraTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraTransform == null) return;
        gameObject.transform.forward = cameraTransform.forward;

    }
}
