using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DayTaskConfig", menuName = "SanRoque/Day Task Config")]
public class DayTaskConfig : ScriptableObject
{
    [Header("Nivel de reputación al que aplica")]
    public int reputationLevel;

    [Header("Tareas fijas de este nivel")]
    public List<TaskData> baseTasks;

    [Header("Pool de tareas aleatorias")]
    public List<TaskData> randomTaskPool;
    public int randomTaskCount; // cuántas del pool se agregan cada día

    public List<TaskData> GetDayTasks()
    {
        var result = new List<TaskData>(baseTasks);

        if (randomTaskCount > 0 && randomTaskPool.Count > 0)
        {
            var shuffled = new List<TaskData>(randomTaskPool);
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
            }
            result.AddRange(shuffled.GetRange(0, Mathf.Min(randomTaskCount, shuffled.Count)));
        }

        return result;
    }
}