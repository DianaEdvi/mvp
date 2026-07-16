using Unity.VisualScripting;
using UnityEngine;

public class ItemOverworld : MonoBehaviour
{
    [SerializeField] private Item item;
    public Item ItemData => item;
    [SerializeField] private SpriteRenderer mainSpriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainSpriteRenderer.sprite = item.itemArt;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        mainSpriteRenderer.gameObject.transform.rotation = Camera.main.transform.rotation;
    }
}
