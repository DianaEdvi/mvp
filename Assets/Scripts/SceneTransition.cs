using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTransition : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSceneWithName(string name)
    {
        if (SceneManager.GetSceneByName(name) != null)
        {
            SceneManager.LoadScene(name);        
        }
    }

    public void QuitGame(){
        Debug.Log("Quit application");
        Application.Quit();
    }
}
