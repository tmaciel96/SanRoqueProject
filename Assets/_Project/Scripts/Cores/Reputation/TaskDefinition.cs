using UnityEngine;

public enum TaskCategory
{
    Cuidado,
    Rescate,
    Tienda,
    Refugio
}

[System.Serializable]
public class TaskDefinition
{
    public string id;
    public string label;
    public TaskCategory category;
    public Color accentColor;
    public int reputationPoints;

    [Header("Dificultad")]
    public int baseRequired;      // cantidad requerida día 1
    public int increasePerDay;    // cuánto sube por día
    public int maxRequired;       // tope máximo

    public int GetRequired(int day)
    {
        int value = baseRequired + (increasePerDay * (day - 1));
        return Mathf.Min(value, maxRequired);
    }
}