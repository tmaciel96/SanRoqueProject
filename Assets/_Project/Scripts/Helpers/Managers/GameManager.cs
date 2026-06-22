using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    InMenu,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CapacityManager capacityManager;
    [SerializeField] private AnimalPrefabCatalog animalPrefabCatalog;

    public static GameState CurrentState { get; private set; } = GameState.Playing;

    private void Awake()
    {
        if (capacityManager == null)
        {
            Debug.LogError("[GameManager] No se asignó CapacityManager en el inspector. Asegurate de asignar manualmente en el inspector del GameManager.");
        }

        if (animalPrefabCatalog == null)
        {
            Debug.LogError("[GameManager] No se asignó AnimalPrefabCatalog en el inspector. Asegurate de asignar manualmente en el inspector del GameManager.");
        }
    }

    private void Start()
    {
        if (capacityManager != null)
        {
            capacityManager.InitializeDefaultPens();
        }

        SetupStarterAnimal();
    }

    //RULE: Start first day with an animal in the shelter
    private void SetupStarterAnimal()
    {
        
        var starterAnimal = AnimalGenerator.GenerateRandomAnimal(animalPrefabCatalog);


        if (!capacityManager.AssignAnimalToPen(starterAnimal))
        {
            Debug.LogError("GameManager: Error al asignar el animal inicial al corral.");
        }
        else
        {
            Debug.Log("GameManager: animal inicial asignado al corral.");
        }
    }

    public static void ChangeState(GameState newState)
    {
        CurrentState = newState;
    }

    public static void RestartGame()
    {
        Time.timeScale = 1f;
        CurrentState = GameState.Playing;

        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}
