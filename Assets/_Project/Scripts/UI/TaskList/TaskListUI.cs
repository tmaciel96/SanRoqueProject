using System.Collections.Generic;
using UnityEngine;

public class TaskListUI : MonoBehaviour
{
    public static TaskListUI Instance { get; private set; }

    [SerializeField] private TaskItemUI[] taskItems;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetTasks(List<TaskData> tasks)
    {
        for (int i = 0; i < taskItems.Length; i++)
        {
            if (i < tasks.Count)
            {
                taskItems[i].gameObject.SetActive(true);
                taskItems[i].Setup(tasks[i]);
            }
            else
            {
                taskItems[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateTask(string taskId, int current)
    {
        foreach (var item in taskItems)
        {
            if (item.TaskId == taskId)
            {
                bool wasDone = item.IsCompleted;
                item.UpdateProgress(current);

                if (!wasDone && item.IsCompleted)
                    ReputationManager.Instance.AddPoints(item.GetReputationPoints());

                break;
            }
        }
    }

    public bool AllTasksCompleted()
    {
        foreach (var item in taskItems)
            if (item.gameObject.activeSelf && !item.IsCompleted)
                return false;
        return true;
    }
}