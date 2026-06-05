using System.Collections.Generic;
using UnityEngine;

public class TaskListUI : MonoBehaviour
{
    [SerializeField] private TaskItemUI[] taskItems;

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

    public void UpdateTask(string taskId, int current, int required)
    {
        foreach (var item in taskItems)
        {
            if (item.TaskId == taskId)
            {
                item.UpdateProgress(current, required);

                if (current >= required)
                    ReputationManager.Instance.AddPoints(item.GetReputationPoints());

                break;
            }
        }
    }
}