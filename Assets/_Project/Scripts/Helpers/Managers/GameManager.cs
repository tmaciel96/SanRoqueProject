using UnityEngine;

public enum GameState
{
    Playing,
    InMenu
}

public class GameManager : MonoBehaviour
{
    public static GameState CurrentState { get; private set; } = GameState.Playing;

    public static void ChangeState(GameState newState)
    {
        CurrentState = newState;
    }
}