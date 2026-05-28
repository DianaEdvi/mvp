using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject floor;

    public float LongestWidth
    {
        get
        {
            if (floor == null) return 0f;
            
            MeshFilter mf = floor.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                return mf.sharedMesh.bounds.size.x * floor.transform.localScale.x;
            }
            return 0f;
        }
    }

    public float LongestHeight
    {
        get
        {
            if (floor == null) return 0f;

            MeshFilter mf = floor.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                return mf.sharedMesh.bounds.size.z * floor.transform.localScale.z;
            }
            return 0f;
        }
    }
}