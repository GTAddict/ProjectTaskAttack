using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{

    public int CompleteDays { get; set; }
    private List<Task> TaskList = new List<Task>();
    private List<GameObject> UICheckboxes = new List<GameObject>();
    private Text text;

    [SerializeField]
    private int typingMultiplier = 1;
    [SerializeField]
    private int stocksMultiplier = 1;
    [SerializeField]
    private int folderMultiplier = 1;

    [SerializeField]
    private SFXManager sfxManager;
    [SerializeField]
    private AudioClip completedTask;
// 
//     [SerializeField]
//    private GameObject checkMark;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        //CompleteDays = 4;
        NewDayInitialize();
/*        checkMark.GetComponent<SpriteRenderer>().enabled = false;*/
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < TaskList.Count; i++)
        {
            if (TaskList[i].CompleteItems >= TaskList[i].TotalItems)
            {
                TaskList.RemoveAt(i);
                i = 0;
                sfxManager.GetComponent<SFXManager>().PlaySound(completedTask);
                /*checkMark.GetComponent<SpriteRenderer>().enabled = true;*/
                continue;
            }
        }

        //         foreach (Task task in TaskList)
        //         {
        //             if (task.CompleteItems == task.TotalItems)
        //             {
        //                 TaskList.Remove(task);
        //             }

        DisplayTasks();
    }

    public void NewDayInitialize()
    {
        if (TaskList.Count == 0)
        {
            CompleteDays++;
        }

        if (CompleteDays > 2)
        {
            GenerateTask(TaskType.FileSort);
            GenerateTask(TaskType.StockSales);
            GenerateTask(TaskType.Typing);
        }
        else if (CompleteDays == 2)
        {
            GenerateTask(TaskType.FileSort);
            GenerateTask(TaskType.Typing);
        }
        else
        {
            GenerateTask(TaskType.Typing);
        }
/*        checkMark.GetComponent<SpriteRenderer>().enabled = false;*/
    }

    void GenerateTask(TaskType type)
    {
        int taskTotal = 0;

        switch (type)
        {
            case TaskType.FileSort:
                taskTotal = CompleteDays * folderMultiplier;
                break;
            case TaskType.StockSales:
                taskTotal = CompleteDays * stocksMultiplier;
                break;
            case TaskType.Typing:
                taskTotal = CompleteDays * typingMultiplier;
                break;
        }

        if (!CheckDone(type))
        {
            TaskList.Add(new Task(type, taskTotal));

        }
        else
        {
            foreach (Task task in TaskList)
            {
                if (task.Type == type)
                {
                    task.TotalItems += taskTotal;
                }
            }
        }
    }

    public void IncrementTask(TaskType type)
    {
        foreach (Task task in TaskList)
        {
            if (task.Type == type)
            {
                task.CompleteItems++;
            }
        }
    }

    bool CheckDone(TaskType type)
    {
        foreach (Task task in TaskList)
        {
            if (task.Type == type)
            {
                return true;
            }
        }
        return false;
    }

    void DisplayTasks()
    {
        string displayString = "";
        foreach (Task task in TaskList)
        {
            displayString += task.UIPrompt + "\n";
            // Debug.Log(displayString);
        }
        text.text = displayString;
    }
}
