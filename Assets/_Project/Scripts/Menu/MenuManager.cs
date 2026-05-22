using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // start game
    public void StartGame()
    {
        SceneManager.LoadScene("Main_Scene");
    }    
}
