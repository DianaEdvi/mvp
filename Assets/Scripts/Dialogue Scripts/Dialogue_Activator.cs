using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor;
using UnityEngine;


public class Dialogue_Activator : MonoBehaviour
{

    string[] lines = null; //all the lines in the dialogue, defined by the length of the array of strings
    int lineTracker = 0; //which line the dialogue is on, defined with #>1 and decrements down to zero
    
    Interaction interaction; //reference to the interaction script
    public TextMeshProUGUI dialogueText; // Reference to the TextMeshProUGUI component for displaying dialogue
    public GameObject dialoguePanel; // Reference to the dialogue panel GameObject
    public float textSpeed;
    public static bool isAnyDialogueActive = false; // Static flag to track if any dialogue is active
    private bool isDialogueActive = false; // Instance flag to track if this dialogue is active

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //reads teh md file and passes that to dialogue manager
    void Start()
    {
        //makes sure dialogue text box is not active
        dialoguePanel.SetActive(false);
        dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>(); // Get the TextMeshProUGUI component from the dialogue panel
        dialogueText.text = string.Empty; // Initialize dialogueText to an empty string
        string character = gameObject.name.Replace("(Clone)", "").Trim();

        // Print with single quotes to see exact spaces/case
        UnityEngine.Debug.Log($"[DEBUG] Looking for character text doc: '{character}'");

        // 2. Load the asset
        TextAsset dialogueFile = Resources.Load<TextAsset>(character); 
        
         if (dialogueFile != null)
        {
           
            lines = dialogueFile.text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            UnityEngine.Debug.Log($"Lines read from {character}: {lines.Length}");
            //need implementaton that makes it so when a player interacts, it calls Initiate dialogue
            interaction.TryInteract();

        }
        else
        {
            UnityEngine.Debug.LogWarning($"Dialogue file not found for {gameObject.name}. Dialogue lines not loaded.");
        }
            
    }


    

    public void InitiateDialogue()
    {
        isAnyDialogueActive = true; // Set the static flag to indicate that a dialogue is active
        isDialogueActive = true; // Set the instance flag to indicate that this dialogue is active
        string nextLine;
        StartConversation(); // Enter cutscene mode before starting dialogue
        DisplayNextLine(lineTracker); // Display the first line of dialogue
     
    }

    //freezes the game, opens the panel
    public void StartConversation()
    {
        
        Time.timeScale = 0f; // Pause the game
        dialogueText.text = string.Empty; // Clear the dialogue text
        dialoguePanel.SetActive(true); // Activate the dialogue panel

    }

    //closes the panel, unfreezes the player and world, and ends the dialogue
    public void EndConversation()
    {
        Time.timeScale = 1f; // Resume the game
        dialoguePanel.SetActive(false); // Deactivate the dialogue panel

        isDialogueActive = false; // Reset the instance flag to indicate that this dialogue is no longer active
        isAnyDialogueActive = false; // Reset the static flag to indicate that no dialogue is active
        lineTracker = 0; // Reset the line tracker for the next dialogue session
    }

    public void DisplayNextLine(int line)
    {
        if (lineTracker < lines.Length - 1)
        {
            
            dialogueText.text = string.Empty; // Clear the dialogue text
            StartCoroutine(TypeLine(lineTracker)); // Start displaying the first line of dialogue
        }
        else
        {
            gameObject.SetActive(false); // Deactivate the GameObject if there are no lines to display
        }
    }

    //displays the next line of dialogue, if there are no more lines then it ends the conversation
    IEnumerator TypeLine(int line)
    {
        foreach (char c in lines[line].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(textSpeed);
        }
    }

    //shows choices
    public void DiplayChoices()
    {

    }

    //calls the action to be done, does the I/O and the relevent choice 
    public void SelectChoice()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                String nextLine = lines[lineTracker];
                if (dialogueText.text == lines[lineTracker])
                {
                    // If the current line is fully displayed, move to the next line
                    lineTracker++;
                    if (nextLine == "[CHOICES]")
                    {
                        DiplayChoices(); // Display choices to the player
                        SelectChoice(); // Wait for player to select a choice
                    }
                    else if (nextLine != null)
                    {
                        if (lineTracker < lines.Length - 1)
                        {
                            DisplayNextLine(lineTracker);
                        }
                    }
                    else
                    {
                        EndConversation();
                    }
                }
                else
                {
                    // If the current line is not fully displayed, complete it immediately
                    StopAllCoroutines();
                    dialogueText.text = lines[lineTracker];
                }
            }
        }
        else
        {
            return; // Exit the Update method if no dialogue is active
        }

    }
}
