using UnityEngine;
using TMPro;
public class Dialogue_UI_Attach : MonoBehaviour
{

    public static Dialogue_UI_Attach instance; // Singleton instance
    [Header("Dialogue UI Elements")]
    public GameObject dialoguePanel; // Reference to the dialogue panel GameObject
    public TextMeshProUGUI dialogueText; // Reference to the TextMeshProUGUI component for displaying dialogue
    public float typingSpeed = 0.05f; // Speed at which the text is typed out

    private void Awake()
    {
        // Implementing Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this object across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        dialoguePanel.SetActive(false); // Ensure the dialogue panel is inactive at the start
    }

    
}
    

  