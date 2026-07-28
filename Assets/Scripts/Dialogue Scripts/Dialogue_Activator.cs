using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using System;
using System.IO;
using UnityEngine;


public class Dialogue_Activator : MonoBehaviour
{

    string[] lines = null; //all the lines in the dialogue, defined by the length of the array of strings
    [SerializeField] Dialogue_Script dialogueManager; //reference to the dialogue manager script

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //reads teh md file and passes that to dialogue manager
    void Start()
    {
        GameObject origPrefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
        if (origPrefab != null)
        {
            string character = System.IO.Path.GetFileNameWithoutExtension(origPrefab.name) + ".md";
            lines = File.ReadAllLines($"Assets/Resources/Dialogue/{character}");
          
        }
        InitiateDialogue();
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
