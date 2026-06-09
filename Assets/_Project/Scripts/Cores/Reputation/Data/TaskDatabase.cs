using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskDatabase", menuName = "SanRoque/Task Database")]
public class TaskDatabase : ScriptableObject
{
    public List<TaskDefinition> tasks;

    public List<TaskDefinition> GetByCategory(TaskCategory category)
    {
        return tasks.FindAll(t => t.category == category);
    }
}