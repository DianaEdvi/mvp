using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using System;
using System.IO;
using UnityEngine;
using System.Runtime.CompilerServices;


public class Dialogue_Activator : MonoBehaviour
{

    string[] lines = null; //all the lines in the dialogue, defined by the length of the array of strings
    [SerializeField] Dialogue_Script dialogueManager; //reference to the dialogue manager script

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //reads teh md file and passes that to dialogue manager
    void Start()
    {
       
        
        string character = gameObject.name.Replace("(Clone)", "").Trim();

        // Print with single quotes to see exact spaces/case
        UnityEngine.Debug.Log($"[DEBUG] Looking for character text doc: '{character}'");

        // 2. Load the asset
        TextAsset dialogueFile = Resources.Load<TextAsset>(character); 
        
         if (dialogueFile != null)
        {
           
            lines = dialogueFile.text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            UnityEngine.Debug.Log($"Lines read from {character}: {lines.Length}");
            InitiateDialogue();
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Dialogue file not found for {gameObject.name}. Dialogue lines not loaded.");
        }
            
    }


    private void OnTriggerEnter(Collider other)
    {
        
    }

    string[] InitiateDialogue()
    {
        dialogueManager.GetNextOpeningLine(lines);
        return lines;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    
}
