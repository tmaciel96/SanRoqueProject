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
    public static GameState CurrentState { get; private set; } = GameState.Playing;

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
