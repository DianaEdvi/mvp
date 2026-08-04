using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


public class Dialogue_Activator : MonoBehaviour
{

    string[] lines = null; //all the lines in the dialogue, defined by the length of the array of strings
    int lineTracker = 0; //which line the dialogue is on, defined with #>1 and decrements down to zero
    
    Interaction interaction; //reference to the interaction script
  
    public static bool isAnyDialogueActive = false; // Static flag to track if any dialogue is active
    private bool isDialogueActive = false; // Instance flag to track if this dialogue is active
    private bool isCharacterFileFound = false; // Flag to track if the character's dialogue file was found
    private bool openedThisFrame = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //reads the txt file and passes that to dialogue manager
    void Start()
    {

        //makes sure dialogue text box is not active
        string character = gameObject.name.Replace("(Clone)", "").Trim();

        // Print with single quotes to see exact spaces/case
        UnityEngine.Debug.Log($"[DEBUG] Looking for character text doc: '{character}'");

        // 2. Load the asset
        TextAsset dialogueFile = Resources.Load<TextAsset>(character); 
        
         if (dialogueFile != null)
        {
           
            lines = dialogueFile.text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            UnityEngine.Debug.Log($"Lines read from {character}: {lines.Length}");
            isCharacterFileFound = true;
            //need implementaton that makes it so when a player interacts, it calls Initiate dialogue

        }
        else
        {
            UnityEngine.Debug.LogWarning($"Dialogue file not found for {gameObject.name}. Dialogue lines not loaded.");
        }
            
    }


    

    public void InitiateDialogue()
    {
        if (isDialogueActive)
        {
            return;
        }
        if (!isCharacterFileFound)
        {
            UnityEngine.Debug.LogWarning($"Cannot initiate dialogue for {gameObject.name} because the dialogue file was not found.");
            return; // Exit if the character's dialogue file was not found
        }
        isAnyDialogueActive = true; // Set the static flag to indicate that a dialogue is active
        isDialogueActive = true; // Set the instance flag to indicate that this dialogue is active
        openedThisFrame = true;
        StartConversation(); // Enter cutscene mode before starting dialogue
        DisplayNextLine(lineTracker); // Display the first line of dialogue
     
    }

    //freezes the game, opens the panel
    public void StartConversation()
    {
        

        if (!isCharacterFileFound)
        {
            UnityEngine.Debug.LogWarning($"Cannot initiate dialogue for {gameObject.name} because the dialogue file was not found.");
            return; // Exit if the character's dialogue file was not found
        }
        if (lines == null || lines.Length == 0)
        {
            UnityEngine.Debug.LogWarning($"No dialogue lines found for {gameObject.name}. Cannot start conversation.");
            return; // Exit if there are no dialogue lines to display
        }

        Time.timeScale = 0f; // Pause the game
        Dialogue_UI_Attach.instance.dialoguePanel.SetActive(true); // Activate the dialogue panel
        Dialogue_UI_Attach.instance.dialogueText.text = string.Empty; // Clear the dialogue text

    }

    //closes the panel, unfreezes the player and world, and ends the dialogue
    public void EndConversation()
    {
        Time.timeScale = 1f; // Resume the game
        Dialogue_UI_Attach.instance.dialoguePanel.SetActive(false); // Deactivate the dialogue panel

        isDialogueActive = false; // Reset the instance flag to indicate that this dialogue is no longer active
        isAnyDialogueActive = false; // Reset the static flag to indicate that no dialogue is active
        lineTracker = 0; // Reset the line tracker for the next dialogue session
    }

    public void DisplayNextLine(int line)
    {
        
        if (lineTracker < lines.Length)
        {
            StopAllCoroutines();
            Dialogue_UI_Attach.instance.dialogueText.text = string.Empty; // Clear the dialogue text
            StartCoroutine(TypeLine(lineTracker)); // Start displaying the first line of dialogue
        }
        else
        {
            EndConversation();
        }
    }

    //displays the next line of dialogue, if there are no more lines then it ends the conversation
    IEnumerator TypeLine(int line)
    {
        foreach (char c in lines[line].ToCharArray())
        {
            Dialogue_UI_Attach.instance.dialogueText.text += c; // Append each character to the dialogue text
            yield return new WaitForSecondsRealtime(Dialogue_UI_Attach.instance.typingSpeed);
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
        if (!isDialogueActive)
        {
            return;
        }

        if (openedThisFrame)
        {
            openedThisFrame = false;
            return;
        }

        if (isDialogueActive)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                String nextLine = lines[lineTracker];
                if (Dialogue_UI_Attach.instance.dialogueText.text == nextLine)
                {
                    // If the current line is fully displayed, move to the next line
                    lineTracker++;
                    if (lineTracker < lines.Length)
                    {
                        nextLine = lines[lineTracker];

                        if (nextLine == "[CHOICES]")
                        {
                            DiplayChoices(); // Display choices to the player
                            SelectChoice(); // Wait for player to select a choice
                        }
                        else if (nextLine != null)
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
                    Dialogue_UI_Attach.instance.dialogueText.text = lines[lineTracker];
                }
            }
        }
        else
        {
            return; // Exit the Update method if no dialogue is active
        }

    }
}
