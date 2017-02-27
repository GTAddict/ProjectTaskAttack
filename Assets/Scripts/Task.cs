using UnityEngine;
using System.Collections;


public class Task{

    public int TotalItems { get; set; }
    public int CompleteItems { get; set; }
    //public bool Completed { get; set; }
    private GameObject taskScreen;

    public string UIPrompt {
        get
        {
            string prompt = null;
            switch (Type)
            {
                case TaskType.FileSort:
                    prompt = "Sort Files: ";
                    break;
                case TaskType.Typing:
                    prompt = "Copy Memos: ";
                    break;
                case TaskType.StockSales:
                    prompt = "Make Sales: ";
                    break;
            }
            return string.Format("{0}{1}/{2}", prompt, CompleteItems, TotalItems);
        }
    }
    public TaskType Type { get; set; }

    public Task(TaskType type, int totalItems = 1)
    {
        TotalItems = totalItems;
        Type = type;
    }
}

public enum TaskType
{
    FileSort,
    Typing,
    StockSales
}
