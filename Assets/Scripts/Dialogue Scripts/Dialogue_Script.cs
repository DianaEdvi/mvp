using UnityEngine;

public class Dialogue_Script : MonoBehaviour
{
    int linetracker = 0; //which line the dialogue is on, defined with #>1 and decrements down to zero
    int maxLines = -1; //the number of lines in the dialogue, defined by the length of the array of strings
    string[] choices = new string[3]; //list of three choices 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //start shoudl define the choices, and the number of lines
    void Start()
    {

    }

    //sees if there is any lines left, if not then it returns 
    public void GetNextOpeningLine(string[] lines)
    {
        if (linetracker != maxLines)
        {
            linetracker++;
            return;
        }
        GetDepartureLine();
    }

    //returns an array of strings that contain the choices for the dialogue 
    string[] GetChoices()
    {
        return choices;
    }

    //allows the player to choose option, give the coresponding object, and put the line counter to a the right number
    void SelectChoice(int choice)
    {
        return;
    }

    //final line, and return the player to play, also sends a signal to cutsceneSystem script
    void GetDepartureLine()
    {
        return;

    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
