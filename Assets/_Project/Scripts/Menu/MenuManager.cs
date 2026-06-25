using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private GameObject optionsPanel;

    // Start Game
    public void StartGame()
    {
        SceneManager.LoadScene("Main_Scene");
    }

    // Credits Panel
    public void ShowCreditPanel()
    {
        creditPanel.SetActive(true);
    }

    public void HideCreditPanel()
    {
        creditPanel.SetActive(false);
    }

    // Options Panel
    public void ShowOptionsPanel()
    {
        optionsPanel.SetActive(true);
    }

    public void HideOptionsPanel()
    {
        optionsPanel.SetActive(false);
    }
}